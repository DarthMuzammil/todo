import { expect, test } from '@playwright/test'
import { registerUser } from './helpers/auth.js'

const VIEWPORTS = [
  { name: 'mobile', width: 375, height: 812 },
  { name: 'desktop', width: 1280, height: 800 },
]

for (const viewport of VIEWPORTS) {
  test.describe(`visual regression @ ${viewport.name}`, () => {
    test.use({ viewport: { width: viewport.width, height: viewport.height } })

    test('home page', async ({ page }) => {
      await registerUser(page)
      await expect(page.getByRole('heading', { name: /hello,/i })).toBeVisible()
      await expect(page).toHaveScreenshot(`home-${viewport.name}.png`, {
        maxDiffPixelRatio: 0.02,
      })
    })

    test('list page', async ({ page }) => {
      await registerUser(page)

      const listTitle = `Visual List ${Date.now()}`
      const taskTitle = `Visual Task ${Date.now()}`

      await page.getByRole('textbox', { name: /^title$/i }).fill(listTitle)
      await page.getByRole('button', { name: /create list/i }).click()
      await expect(page).toHaveURL(/\/lists\//)

      await page.getByLabel(/^title$/i).fill(taskTitle)
      await page.getByRole('button', { name: /add task/i }).click()
      await expect(
        page.getByRole('heading', { name: taskTitle, level: 3 }),
      ).toBeVisible()

      await expect(page).toHaveScreenshot(`list-${viewport.name}.png`, {
        maxDiffPixelRatio: 0.02,
      })
    })
  })
}
