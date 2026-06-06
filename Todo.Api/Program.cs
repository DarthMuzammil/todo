using Microsoft.OpenApi;

using Todo.Application;

using Todo.Infrastructure;



var builder = WebApplication.CreateBuilder(args);



builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(options =>

{

    options.SwaggerDoc("v1", new OpenApiInfo

    {

        Title = "Todo API",

        Version = "v1",

        Description = "API for managing todo lists and tasks"

    });

});



var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()

    ?? ["http://localhost:5173"];



builder.Services.AddCors(options =>

{

    options.AddDefaultPolicy(policy =>

    {

        policy.WithOrigins(allowedOrigins)

            .AllowAnyHeader()

            .AllowAnyMethod();

    });

});



var tasksFile = builder.Configuration["Data:TasksFile"] ?? "data/tasks.json";

var listsFile = builder.Configuration["Data:ListsFile"] ?? "data/lists.json";



builder.Services.AddInfrastructure(tasksFile, listsFile);

builder.Services.AddApplication();



var app = builder.Build();



if (app.Environment.IsDevelopment())

{

    app.UseSwagger();

    app.UseSwaggerUI(options =>

    {

        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API v1");

        options.RoutePrefix = "swagger";

    });

}



app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();

app.MapControllers();



app.Run();


