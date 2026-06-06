import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import { createList } from '@/api/lists'
import { personalWorkspace } from '@/test/workspaceFixtures'
import { WorkspaceTestProviders } from '@/test/workspaceTestUtils'
import { HomePage } from './HomePage'

const navigate = vi.fn()

vi.mock('@/api/lists', () => ({
  createList: vi.fn(),
  getLists: vi.fn().mockResolvedValue([]),
}))

vi.mock('@/features/auth', () => ({
  useAuth: () => ({ user: { name: 'Test User' } }),
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

  it('creates a list in the current workspace and navigates to the list page', async () => {
    const user = userEvent.setup()
    createList.mockResolvedValue({ id: 'list-123', title: 'Groceries' })

    render(
      <WorkspaceTestProviders>
        <HomePage />
      </WorkspaceTestProviders>,
    )

    await waitFor(() => {
      expect(screen.getByRole('textbox', { name: /title/i })).toBeInTheDocument()
    })

    await user.type(
      screen.getByRole('textbox', { name: /title/i }),
      'Groceries',
    )
    await user.click(screen.getByRole('button', { name: /create list/i }))

    await waitFor(() => {
      expect(createList).toHaveBeenCalledWith({
        title: 'Groceries',
        color: null,
        workspaceId: personalWorkspace.id,
      })
    })

    expect(navigate).toHaveBeenCalledWith('/lists/list-123')
  })
})
