import { describe, expect, it } from 'vitest'
import {
  hasAuthFormErrors,
  validateChangePasswordForm,
  validateProfileForm,
} from './authForm'

describe('settings form validation', () => {
  it('requires a display name', () => {
    const errors = validateProfileForm({ name: '   ' })
    expect(errors.name).toBeTruthy()
    expect(hasAuthFormErrors(errors)).toBe(true)
  })

  it('requires current and new password', () => {
    const errors = validateChangePasswordForm({
      currentPassword: '',
      newPassword: 'short',
    })

    expect(errors.currentPassword).toBeTruthy()
    expect(errors.newPassword).toBeTruthy()
  })
})
