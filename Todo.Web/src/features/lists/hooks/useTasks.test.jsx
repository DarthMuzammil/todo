import { renderHook, waitFor } from '@testing-library/react'
import { describe, expect, it, vi } from 'vitest'
import { getTasksByListId } from '@/api/tasks'
import { useTasks } from './useTasks'

vi.mock('@/api/tasks', () => ({
  getTasksByListId: vi.fn(),
}))

describe('useTasks', () => {
  it('loads tasks for a list', async () => {
    getTasksByListId.mockResolvedValue([
      { id: 'task-1', title: 'Buy milk', status: 0, priority: 1 },
    ])

    const { result } = renderHook(() => useTasks('list-1'))

    await waitFor(() => {
      expect(result.current.status).toBe('success')
    })

    expect(result.current.tasks).toHaveLength(1)
    expect(getTasksByListId).toHaveBeenCalledWith('list-1')
  })

  it('surfaces API errors', async () => {
    getTasksByListId.mockRejectedValue(new Error('Failed to load tasks'))

    const { result } = renderHook(() => useTasks('list-1'))

    await waitFor(() => {
      expect(result.current.status).toBe('error')
    })

    expect(result.current.error).toEqual(new Error('Failed to load tasks'))
  })
})
