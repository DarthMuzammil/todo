import { renderHook, waitFor } from '@testing-library/react'
import { describe, expect, it, vi } from 'vitest'
import { getListById } from '@/api/lists'
import { useList } from './useList'

vi.mock('@/api/lists', () => ({
  getListById: vi.fn(),
}))

describe('useList', () => {
  it('loads a list by id', async () => {
    getListById.mockResolvedValue({ id: 'list-1', title: 'Groceries' })

    const { result } = renderHook(() => useList('list-1'))

    await waitFor(() => {
      expect(result.current.status).toBe('success')
    })

    expect(result.current.list).toEqual({ id: 'list-1', title: 'Groceries' })
    expect(getListById).toHaveBeenCalledWith('list-1')
  })

  it('surfaces API errors', async () => {
    getListById.mockRejectedValue(new Error('List not found'))

    const { result } = renderHook(() => useList('missing'))

    await waitFor(() => {
      expect(result.current.status).toBe('error')
    })

    expect(result.current.error).toEqual(new Error('List not found'))
  })
})
