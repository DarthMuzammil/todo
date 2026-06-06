import { describe, expect, it } from 'vitest'
import { ApiClientError } from '@/api/client'
import { getErrorMessage } from './getErrorMessage'

describe('getErrorMessage', () => {
  it('returns API error body message for ApiClientError', () => {
    const error = new ApiClientError(400, { error: 'Title is required' })
    expect(getErrorMessage(error, 'Fallback')).toBe('Title is required')
  })

  it('returns fallback when ApiClientError has no body message', () => {
    const error = new ApiClientError(500, {})
    expect(getErrorMessage(error, 'Fallback')).toBe('Fallback')
  })

  it('returns Error.message for generic errors', () => {
    expect(getErrorMessage(new Error('Network down'), 'Fallback')).toBe(
      'Network down',
    )
  })

  it('returns fallback for unknown values', () => {
    expect(getErrorMessage('oops', 'Fallback')).toBe('Fallback')
  })
})
