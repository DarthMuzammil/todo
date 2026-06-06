import { expect, test } from '@playwright/test'
import { registerUser } from './helpers/auth.js'

test('register, create list, and add task', async ({ page }) => {
  await registerUser(page)

  const listTitle = `E2E List ${Date.now()}`
  const taskTitle = `E2E Task ${Date.now()}`

  await expect(page.getByRole('heading', { name: /hello,/i })).toBeVisible()

  await page.getByRole('textbox', { name: /^title$/i }).fill(listTitle)
  await page.getByRole('button', { name: /create list/i }).click()

  await expect(page).toHaveURL(/\/lists\//)
  await expect(page.getByRole('heading', { name: listTitle })).toBeVisible()

  await page.getByLabel(/^title$/i).fill(taskTitle)
  await page.getByRole('button', { name: /add task/i }).click()

  await expect(page.getByRole('heading', { name: taskTitle, level: 3 })).toBeVisible()
})

test('redirects unauthenticated users to login', async ({ page }) => {
  await page.goto('/')
  await expect(page).toHaveURL(/\/login/)
})
