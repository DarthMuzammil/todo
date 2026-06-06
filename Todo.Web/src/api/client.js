import { API_BASE_URL } from '@/shared/config/env'
import { getAccessToken, setAccessToken } from '@/shared/auth/tokenStorage'
import { notifyUnauthorized } from '@/api/unauthorized'

/**
 * Error thrown when the API responds with a non-2xx status.
 * `body` is the parsed JSON error shape: { error: string }.
 */
export class ApiClientError extends Error {
  constructor(status, body) {
    super(body?.error ?? 'Request failed')
    this.name = 'ApiClientError'
    this.status = status
    this.body = body
  }
}

function buildUrl(path) {
  return `${API_BASE_URL}${path.startsWith('/') ? path : `/${path}`}`
}

function isAuthPath(path) {
  return (
    path.includes('/auth/login') ||
    path.includes('/auth/register') ||
    path.includes('/auth/refresh') ||
    path.includes('/auth/logout')
  )
}

let refreshPromise = null

async function refreshAccessToken() {
  if (!refreshPromise) {
    refreshPromise = fetch(buildUrl('/auth/refresh'), {
      method: 'POST',
      credentials: 'include',
      headers: {
        'Content-Type': 'application/json',
      },
    }).finally(() => {
      refreshPromise = null
    })
  }

  const response = await refreshPromise
  if (!response.ok) {
    return false
  }

  const body = await response.json()
  if (!body?.accessToken) {
    return false
  }

  setAccessToken(body.accessToken)
  return true
}

/**
 * Thin fetch wrapper: sets JSON headers, attaches bearer token when present,
 * throws ApiClientError on failure, and parses the JSON response (or undefined for 204).
 */
export async function request(path, options = {}) {
  const token = getAccessToken()
  const headers = {
    'Content-Type': 'application/json',
    ...options.headers,
  }

  if (token && !headers.Authorization) {
    headers.Authorization = `Bearer ${token}`
  }

  const response = await fetch(buildUrl(path), {
    ...options,
    headers,
    credentials: 'include',
  })

  if (
    response.status === 401 &&
    !options.skipAuthRefresh &&
    !options.isRetry &&
    !isAuthPath(path)
  ) {
    const refreshed = await refreshAccessToken()
    if (refreshed) {
      return request(path, { ...options, isRetry: true })
    }

    notifyUnauthorized()
  }

  if (!response.ok) {
    const body = await response
      .json()
      .catch(() => ({ error: 'Request failed' }))
    throw new ApiClientError(response.status, body)
  }

  if (response.status === 204) {
    return undefined
  }

  return response.json()
}
