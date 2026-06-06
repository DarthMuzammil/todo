import { describe, expect, it } from 'vitest'
import {
  hasCreateListFormErrors,
  validateCreateListForm,
} from './createListForm'

describe('validateCreateListForm', () => {
  it('returns no errors for a valid title', () => {
    const errors = validateCreateListForm({ title: '  Groceries  ', color: '' })
    expect(errors).toEqual({})
    expect(hasCreateListFormErrors(errors)).toBe(false)
  })

  it('requires a non-empty title', () => {
    const errors = validateCreateListForm({ title: '   ', color: '' })
    expect(errors.title).toBe('Title is required')
    expect(hasCreateListFormErrors(errors)).toBe(true)
  })

  it('rejects titles over the max length', () => {
    const errors = validateCreateListForm({
      title: 'a'.repeat(101),
      color: '',
    })
    expect(errors.title).toBe('Title must be at most 100 characters')
  })
})
