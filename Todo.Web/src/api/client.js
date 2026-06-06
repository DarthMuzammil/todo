import { API_BASE_URL } from '@/shared/config/env'

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

/**
 * Thin fetch wrapper: sets JSON headers, throws ApiClientError on failure,
 * and parses the JSON response (or returns undefined for 204 No Content).
 */
export async function request(path, options) {
  const response = await fetch(buildUrl(path), {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...options?.headers,
    },
  })

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
