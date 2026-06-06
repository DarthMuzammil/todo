/**
 * Register a fresh user and land on the authenticated home page.
 */
export async function registerUser(page, { name = 'E2E User' } = {}) {
  const unique = Date.now()
  const email = `e2e-${unique}@test.local`
  const password = 'TestPass1'

  await page.goto('/register')
  await page.getByLabel(/^name$/i).fill(name)
  await page.getByLabel(/^email$/i).fill(email)
  await page.getByLabel(/^password$/i).fill(password)
  await page.getByRole('button', { name: /create account/i }).click()

  await page.waitForURL('/')

  return { email, password, name }
}

/**
 * Sign in with an existing account.
 */
export async function loginUser(page, { email, password }) {
  await page.goto('/login')
  await page.getByLabel(/^email$/i).fill(email)
  await page.getByLabel(/^password$/i).fill(password)
  await page.getByRole('button', { name: /sign in/i }).click()
  await page.waitForURL('/')
}
