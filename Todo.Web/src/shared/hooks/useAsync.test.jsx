import { act, renderHook, waitFor } from '@testing-library/react'
import { describe, expect, it, vi } from 'vitest'
import { useAsync } from './useAsync'

describe('useAsync', () => {
  it('loads data successfully', async () => {
    const fetcher = vi.fn().mockResolvedValue({ id: '1' })
    const { result } = renderHook(() => useAsync('list-1', fetcher))

    expect(result.current.status).toBe('loading')

    await waitFor(() => {
      expect(result.current.status).toBe('success')
    })

    expect(result.current.data).toEqual({ id: '1' })
    expect(fetcher).toHaveBeenCalledWith('list-1')
  })

  it('captures errors from the fetcher', async () => {
    const fetcher = vi.fn().mockRejectedValue(new Error('Load failed'))
    const { result } = renderHook(() => useAsync('list-1', fetcher))

    await waitFor(() => {
      expect(result.current.status).toBe('error')
    })

    expect(result.current.error).toEqual(new Error('Load failed'))
  })

  it('refetches when refetch is called', async () => {
    const fetcher = vi
      .fn()
      .mockResolvedValueOnce([{ id: 'task-1' }])
      .mockResolvedValueOnce([{ id: 'task-1' }, { id: 'task-2' }])

    const { result } = renderHook(() =>
      useAsync('list-1', fetcher, { initialData: [] }),
    )

    await waitFor(() => {
      expect(result.current.status).toBe('success')
    })

    await act(async () => {
      result.current.refetch()
    })

    await waitFor(() => {
      expect(result.current.data).toHaveLength(2)
    })

    expect(fetcher).toHaveBeenCalledTimes(2)
  })

  it('does not fetch when key is empty', () => {
    const fetcher = vi.fn()
    const { result } = renderHook(() => useAsync('', fetcher))

    expect(result.current.status).toBe('loading')
    expect(fetcher).not.toHaveBeenCalled()
  })

  it('does not refetch when only the fetcher reference changes', async () => {
    const firstFetcher = vi.fn().mockResolvedValue([{ id: 'list-1' }])
    const secondFetcher = vi.fn().mockResolvedValue([{ id: 'list-2' }])

    const { rerender } = renderHook(
      ({ fetcher }) => useAsync('owner-1', fetcher),
      { initialProps: { fetcher: firstFetcher } },
    )

    await waitFor(() => {
      expect(firstFetcher).toHaveBeenCalledTimes(1)
    })

    rerender({ fetcher: secondFetcher })

    await waitFor(() => {
      expect(secondFetcher).not.toHaveBeenCalled()
    })

    expect(firstFetcher).toHaveBeenCalledTimes(1)
  })
})
