import { expect, test } from '@playwright/test'
import { registerUser } from './helpers/auth.js'

test('create list, add task, and mark done', async ({ page }) => {
  await registerUser(page)

  const listTitle = `E2E List ${Date.now()}`
  const taskTitle = `E2E Task ${Date.now()}`

  await page.getByRole('textbox', { name: /^title$/i }).fill(listTitle)
  await page.getByRole('button', { name: /create list/i }).click()

  await expect(page).toHaveURL(/\/lists\//)
  await expect(page.getByRole('heading', { name: listTitle })).toBeVisible()

  await page.getByLabel(/^title$/i).fill(taskTitle)
  await page.getByRole('button', { name: /add task/i }).click()

  await expect(page.getByRole('heading', { name: taskTitle, level: 3 })).toBeVisible()

  const taskItem = page
    .getByRole('listitem')
    .filter({ has: page.getByRole('heading', { name: taskTitle, level: 3 }) })

  const statusSelect = taskItem.getByRole('combobox', { name: /status/i })

  await statusSelect.selectOption('2')

  await expect(statusSelect).toHaveValue('2')
  await expect(taskItem.getByTestId(/^task-status-badge-/)).toHaveText('Done')
})
