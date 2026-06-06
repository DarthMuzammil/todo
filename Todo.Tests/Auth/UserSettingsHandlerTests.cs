using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Todo.Application.Commands.ChangePassword;
using Todo.Application.Commands.LogoutAllSessions;
using Todo.Application.Commands.UpdateProfile;
using Todo.Application.Identity;
using Todo.Application.Interfaces;
using Todo.Infrastructure.Identity;
using Todo.Infrastructure.Persistence;
using Todo.Tests.Support;

namespace Todo.Tests.Auth;

[TestFixture]
public class UserSettingsHandlerTests
{
    private string _dbPath = null!;
    private TodoDbContext _context = null!;
    private UserManager<ApplicationUser> _userManager = null!;
    private ITokenService _tokenService = null!;

    [SetUp]
    public async Task SetUp()
    {
        _dbPath = Path.Combine(Path.GetTempPath(), $"user-settings-{Guid.NewGuid():N}.db");

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddDbContext<TodoDbContext>(options =>
            options.UseSqlite($"Data Source={_dbPath}"));
        services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequiredLength = 8;
                options.Password.RequireDigit = true;
                options.Password.RequireUppercase = true;
            })
            .AddEntityFrameworkStores<TodoDbContext>()
            .AddDefaultTokenProviders();
        services.Configure<JwtSettings>(options =>
        {
            options.Key = "test-key-at-least-32-characters-long!";
            options.Issuer = "test";
            options.Audience = "test";
            options.AccessTokenMinutes = 15;
            options.RefreshTokenDays = 7;
        });
        services.AddScoped<ITokenService, TokenService>();

        var provider = services.BuildServiceProvider();
        _context = provider.GetRequiredService<TodoDbContext>();
        await _context.Database.MigrateAsync();
        _userManager = provider.GetRequiredService<UserManager<ApplicationUser>>();
        _tokenService = provider.GetRequiredService<ITokenService>();
    }

    [TearDown]
    public async Task TearDown()
    {
        await _context.DisposeAsync();
        if (File.Exists(_dbPath))
            File.Delete(_dbPath);
    }

    private static async Task<ApplicationUser> SeedUserAsync(
        UserManager<ApplicationUser> userManager,
        string name = "Test User")
    {
        var user = ApplicationUserTestFactory.Create(name: name);
        var createResult = await userManager.CreateAsync(user, "Password1!");

        Assert.That(
            createResult.Succeeded,
            Is.True,
            string.Join("; ", createResult.Errors.Select(error => error.Description)));

        return user;
    }

    [Test]
    public async Task UpdateProfile_ChangesDisplayName()
    {
        var user = await SeedUserAsync(_userManager, "Before");
        var handler = new UpdateProfileHandler(_userManager);

        var result = await handler.HandleAsync(new UpdateProfileCommand(user.Id, "After"));

        Assert.That(result.IsSuccess, Is.True, result.Error);
        Assert.That(result.Value!.Name, Is.EqualTo("After"));
    }

    [Test]
    public async Task ChangePassword_WithValidCurrentPassword_Succeeds()
    {
        var user = await SeedUserAsync(_userManager);
        var handler = new ChangePasswordHandler(_userManager);

        var result = await handler.HandleAsync(
            new ChangePasswordCommand(user.Id, "Password1!", "Newpass2!"));

        Assert.That(result.IsSuccess, Is.True, result.Error);
    }

    [Test]
    public async Task LogoutAllSessions_RevokesActiveRefreshTokens()
    {
        var userId = await ApplicationUserTestFactory.SeedAsync(_context);
        await _tokenService.IssueRefreshTokenAsync(userId);
        var handler = new LogoutAllSessionsHandler(_tokenService);

        var result = await handler.HandleAsync(new LogoutAllSessionsCommand(userId));

        Assert.That(result.IsSuccess, Is.True);

        var activeCount = await _context.RefreshTokens
            .CountAsync(t => t.UserId == userId && t.RevokedAt == null);

        Assert.That(activeCount, Is.Zero);
    }
}
