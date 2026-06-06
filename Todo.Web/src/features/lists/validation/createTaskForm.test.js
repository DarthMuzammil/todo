import { describe, expect, it } from 'vitest'
import {
  hasCreateTaskFormErrors,
  validateCreateTaskForm,
} from './createTaskForm'

describe('validateCreateTaskForm', () => {
  it('returns no errors for a valid title', () => {
    const errors = validateCreateTaskForm({
      title: '  Buy milk  ',
      description: '',
      priority: 1,
      dueDate: '',
    })
    expect(errors).toEqual({})
    expect(hasCreateTaskFormErrors(errors)).toBe(false)
  })

  it('requires a non-empty title', () => {
    const errors = validateCreateTaskForm({
      title: ' ',
      description: '',
      priority: 1,
      dueDate: '',
    })
    expect(errors.title).toBe('Title is required')
    expect(hasCreateTaskFormErrors(errors)).toBe(true)
  })

  it('rejects titles over the max length', () => {
    const errors = validateCreateTaskForm({
      title: 'a'.repeat(201),
      description: '',
      priority: 1,
      dueDate: '',
    })
    expect(errors.title).toBe('Title must be at most 200 characters')
  })
})
