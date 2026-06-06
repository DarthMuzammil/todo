import { ApiClientError } from '@/api/client'

export function getErrorMessage(error, fallback = 'Something went wrong') {
  if (error instanceof ApiClientError) {
    return error.body?.error ?? fallback
  }

  if (error instanceof Error && error.message) {
    return error.message
  }

  return fallback
}
