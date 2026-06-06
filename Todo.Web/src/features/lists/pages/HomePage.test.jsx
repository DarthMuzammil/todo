import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { MemoryRouter } from 'react-router-dom'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import { createList } from '@/api/lists'
import { HomePage } from './HomePage'

const navigate = vi.fn()

vi.mock('@/api/lists', () => ({
  createList: vi.fn(),
}))

vi.mock('@/shared/config/dev', () => ({
  DEV_OWNER_ID: '00000000-0000-0000-0000-000000000001',
}))

vi.mock('react-router-dom', async () => {
  const actual = await vi.importActual('react-router-dom')
  return {
    ...actual,
    useNavigate: () => navigate,
  }
})

describe('HomePage', () => {
  beforeEach(() => {
    navigate.mockReset()
    createList.mockReset()
  })

  it('creates a list and navigates to the list page', async () => {
    const user = userEvent.setup()
    createList.mockResolvedValue({ id: 'list-123', title: 'Groceries' })

    render(
      <MemoryRouter>
        <HomePage />
      </MemoryRouter>,
    )

    await user.type(
      screen.getByRole('textbox', { name: /title/i }),
      'Groceries',
    )
    await user.click(screen.getByRole('button', { name: /create list/i }))

    await waitFor(() => {
      expect(createList).toHaveBeenCalledWith({
        ownerId: '00000000-0000-0000-0000-000000000001',
        title: 'Groceries',
        color: null,
      })
    })

    expect(navigate).toHaveBeenCalledWith('/lists/list-123')
  })
})
