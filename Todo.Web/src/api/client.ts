import type { ApiError } from '@/api/types'
import { API_BASE_URL } from '@/shared/config/env'

export class ApiClientError extends Error {
  status: number
  body: ApiError

  constructor(status: number, body: ApiError) {
    super(body.error)
    this.name = 'ApiClientError'
    this.status = status
    this.body = body
  }
}

function buildUrl(path: string): string {
  return `${API_BASE_URL}${path.startsWith('/') ? path : `/${path}`}`
}

export async function request<T>(
  path: string,
  options?: RequestInit,
): Promise<T> {
  const response = await fetch(buildUrl(path), {
    ...options,
    headers: {
      'Content-Type': 'application/json',
      ...options?.headers,
    },
  })

  if (!response.ok) {
    const body = (await response.json()) as ApiError
    throw new ApiClientError(response.status, body)
  }

  if (response.status === 204) {
    return undefined as T
  }

  const data = await response.json()
  return data as T
}
