import { render, screen, waitFor } from '@testing-library/react'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import { getLists } from '@/api/lists'
import { personalWorkspace } from '@/test/workspaceFixtures'
import { WorkspaceTestProviders } from '@/test/workspaceTestUtils'
import ListsSidebar from './ListsSidebar'

vi.mock('@/api/lists', () => ({
  getLists: vi.fn(),
}))

describe('ListsSidebar', () => {
  beforeEach(() => {
    getLists.mockReset()
  })

  it('renders lists from the API', async () => {
    getLists.mockResolvedValue([
      {
        id: 'list-1',
        title: 'Groceries',
        color: '#ff0000',
        workspaceId: personalWorkspace.id,
        updatedAt: '2026-01-02',
      },
      {
        id: 'list-2',
        title: 'Work',
        color: '#00ff00',
        workspaceId: personalWorkspace.id,
        updatedAt: '2026-01-01',
      },
    ])

    render(
      <WorkspaceTestProviders initialEntries={['/lists/list-1']}>
        <ListsSidebar />
      </WorkspaceTestProviders>,
    )

    await waitFor(() => {
      expect(getLists).toHaveBeenCalled()
    })

    expect(screen.getByRole('link', { name: /groceries/i })).toHaveAttribute(
      'href',
      '/lists/list-1',
    )
    expect(screen.getByRole('link', { name: /work/i })).toHaveAttribute(
      'href',
      '/lists/list-2',
    )
  })
})
