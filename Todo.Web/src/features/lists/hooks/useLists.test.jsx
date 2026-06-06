import { renderHook, waitFor } from '@testing-library/react'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import { getLists } from '@/api/lists'
import { personalWorkspace } from '@/test/workspaceFixtures'
import { WorkspaceTestProviders } from '@/test/workspaceTestUtils'
import { useLists } from './useLists'

vi.mock('@/api/lists', () => ({
  getLists: vi.fn(),
}))

describe('useLists', () => {
  beforeEach(() => {
    getLists.mockReset()
  })

  it('loads lists for the selected workspace', async () => {
    getLists.mockResolvedValue([
      { id: 'list-1', title: 'Groceries', workspaceId: personalWorkspace.id },
      { id: 'list-2', title: 'Other', workspaceId: 'ws-other' },
    ])

    const { result } = renderHook(() => useLists(), {
      wrapper: ({ children }) => (
        <WorkspaceTestProviders>{children}</WorkspaceTestProviders>
      ),
    })

    await waitFor(() => {
      expect(result.current.status).toBe('success')
    })

    expect(result.current.lists).toEqual([
      { id: 'list-1', title: 'Groceries', workspaceId: personalWorkspace.id },
    ])
  })
})
