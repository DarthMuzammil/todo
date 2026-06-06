import { describe, expect, it } from 'vitest'
import { formatDueDate, isTaskOverdue } from './formatDueDate'

describe('formatDueDate', () => {
  it('formats a valid ISO date', () => {
    const formatted = formatDueDate('2026-06-15T00:00:00Z')
    expect(formatted).toMatch(/Jun/)
    expect(formatted).toMatch(/15/)
  })

  it('returns null for empty or invalid values', () => {
    expect(formatDueDate(null)).toBeNull()
    expect(formatDueDate('not-a-date')).toBeNull()
  })
})

describe('isTaskOverdue', () => {
  it('returns true when due date is in the past and task is not done', () => {
    expect(isTaskOverdue('2020-01-01', 0)).toBe(true)
  })

  it('returns false when task is done', () => {
    expect(isTaskOverdue('2020-01-01', 2)).toBe(false)
  })

  it('returns false when there is no due date', () => {
    expect(isTaskOverdue(null, 0)).toBe(false)
  })
})
