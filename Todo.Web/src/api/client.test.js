import { beforeEach, describe, expect, it, vi } from 'vitest'
import { request } from './client'

vi.mock('@/shared/config/env', () => ({
  API_BASE_URL: 'http://test.api',
}))

describe('request', () => {
  beforeEach(() => {
    vi.stubGlobal('fetch', vi.fn())
  })

  it('returns parsed JSON on success', async () => {
    fetch.mockResolvedValue({
      ok: true,
      status: 200,
      json: () => Promise.resolve({ id: 'list-1', title: 'Groceries' }),
    })

    const result = await request('/lists/list-1')

    expect(fetch).toHaveBeenCalledWith(
      'http://test.api/lists/list-1',
      expect.objectContaining({
        headers: expect.objectContaining({
          'Content-Type': 'application/json',
        }),
      }),
    )
    expect(result).toEqual({ id: 'list-1', title: 'Groceries' })
  })

  it('returns undefined for 204 No Content', async () => {
    fetch.mockResolvedValue({
      ok: true,
      status: 204,
    })

    const result = await request('/lists/list-1/tasks/task-1', {
      method: 'DELETE',
    })

    expect(result).toBeUndefined()
  })

  it('throws ApiClientError with API message on failure', async () => {
    fetch.mockResolvedValue({
      ok: false,
      status: 404,
      json: () => Promise.resolve({ error: 'List not found' }),
    })

    await expect(request('/lists/missing')).rejects.toMatchObject({
      name: 'ApiClientError',
      status: 404,
      body: { error: 'List not found' },
    })
  })

  it('throws ApiClientError with fallback when error body is invalid JSON', async () => {
    fetch.mockResolvedValue({
      ok: false,
      status: 500,
      json: () => Promise.reject(new Error('invalid json')),
    })

    await expect(request('/lists')).rejects.toMatchObject({
      name: 'ApiClientError',
      status: 500,
      body: { error: 'Request failed' },
    })
  })
})
