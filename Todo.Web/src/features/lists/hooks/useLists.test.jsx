import { renderHook, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import { getLists } from '@/api/lists'
import { useLists } from './useLists'

vi.mock('@/api/lists', () => ({
  getLists: vi.fn(),
}))

vi.mock('@/shared/config/dev', () => ({
  DEV_OWNER_ID: 'owner-1',
}))

describe('useLists', () => {
  beforeEach(() => {
    getLists.mockReset()
  })

  it('loads lists for the dev owner', async () => {
    getLists.mockResolvedValue([{ id: 'list-1', title: 'Groceries' }])

    const { result } = renderHook(() => useLists(), {
      wrapper: ({ children }) => (
        <MemoryRouter initialEntries={['/']}>{children}</MemoryRouter>
      ),
    })

    await waitFor(() => {
      expect(result.current.status).toBe('success')
    })

    expect(getLists).toHaveBeenCalledWith('owner-1')
    expect(result.current.lists).toEqual([{ id: 'list-1', title: 'Groceries' }])
  })
})
