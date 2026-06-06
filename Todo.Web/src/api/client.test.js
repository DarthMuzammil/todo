import { beforeEach, describe, expect, it, vi } from 'vitest'
import { request } from './client'

vi.mock('@/shared/config/env', () => ({
  API_BASE_URL: 'http://test.api',
}))

vi.mock('@/shared/auth/tokenStorage', () => ({
  getAccessToken: vi.fn(),
  setAccessToken: vi.fn(),
}))

vi.mock('@/api/unauthorized', () => ({
  notifyUnauthorized: vi.fn(),
}))

import { getAccessToken, setAccessToken } from '@/shared/auth/tokenStorage'
import { notifyUnauthorized } from '@/api/unauthorized'

describe('request', () => {
  beforeEach(() => {
    vi.stubGlobal('fetch', vi.fn())
    getAccessToken.mockReset()
    setAccessToken.mockReset()
    notifyUnauthorized.mockReset()
  })

  it('returns parsed JSON on success', async () => {
    getAccessToken.mockReturnValue(null)

    fetch.mockResolvedValue({
      ok: true,
      status: 200,
      json: () => Promise.resolve({ id: 'list-1', title: 'Groceries' }),
    })

    const result = await request('/lists/list-1')

    expect(fetch).toHaveBeenCalledWith(
      'http://test.api/lists/list-1',
      expect.objectContaining({
        credentials: 'include',
        headers: expect.objectContaining({
          'Content-Type': 'application/json',
        }),
      }),
    )
    expect(result).toEqual({ id: 'list-1', title: 'Groceries' })
  })

  it('attaches bearer token when present', async () => {
    getAccessToken.mockReturnValue('test-token')

    fetch.mockResolvedValue({
      ok: true,
      status: 200,
      json: () => Promise.resolve([]),
    })

    await request('/lists')

    expect(fetch).toHaveBeenCalledWith(
      'http://test.api/lists',
      expect.objectContaining({
        headers: expect.objectContaining({
          Authorization: 'Bearer test-token',
        }),
      }),
    )
  })

  it('returns undefined for 204 No Content', async () => {
    getAccessToken.mockReturnValue(null)

    fetch.mockResolvedValue({
      ok: true,
      status: 204,
    })

    const result = await request('/auth/logout', {
      method: 'POST',
      skipAuthRefresh: true,
    })

    expect(result).toBeUndefined()
  })

  it('throws ApiClientError with API message on failure', async () => {
    getAccessToken.mockReturnValue(null)

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

  it('retries once after a successful silent refresh on 401', async () => {
    getAccessToken
      .mockReturnValueOnce('expired-token')
      .mockReturnValueOnce('fresh-token')

    fetch
      .mockResolvedValueOnce({
        ok: false,
        status: 401,
        json: () => Promise.resolve({ error: 'Unauthorized' }),
      })
      .mockResolvedValueOnce({
        ok: true,
        status: 200,
        json: () => Promise.resolve({ accessToken: 'fresh-token' }),
      })
      .mockResolvedValueOnce({
        ok: true,
        status: 200,
        json: () => Promise.resolve([]),
      })

    const result = await request('/lists')

    expect(setAccessToken).toHaveBeenCalledWith('fresh-token')
    expect(fetch).toHaveBeenCalledTimes(3)
    expect(result).toEqual([])
    expect(notifyUnauthorized).not.toHaveBeenCalled()
  })

  it('notifies unauthorized handler when refresh fails on 401', async () => {
    getAccessToken.mockReturnValue('expired-token')

    fetch
      .mockResolvedValueOnce({
        ok: false,
        status: 401,
        json: () => Promise.resolve({ error: 'Unauthorized' }),
      })
      .mockResolvedValueOnce({
        ok: false,
        status: 401,
        json: () => Promise.resolve({ error: 'Invalid refresh token' }),
      })

    await expect(request('/lists')).rejects.toMatchObject({
      status: 401,
    })

    expect(notifyUnauthorized).toHaveBeenCalledTimes(1)
  })

  it('throws ApiClientError with fallback when error body is invalid JSON', async () => {
    getAccessToken.mockReturnValue(null)

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
