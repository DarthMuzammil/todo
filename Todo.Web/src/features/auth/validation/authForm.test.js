import { describe, expect, it } from 'vitest'
import {
  hasAuthFormErrors,
  validateLoginForm,
  validateRegisterForm,
} from './authForm'

describe('validateLoginForm', () => {
  it('requires email and password', () => {
    const errors = validateLoginForm({ email: '', password: '' })
    expect(hasAuthFormErrors(errors)).toBe(true)
    expect(errors.email).toBeTruthy()
    expect(errors.password).toBeTruthy()
  })
})

describe('validateRegisterForm', () => {
  it('requires name and enforces minimum password length', () => {
    const errors = validateRegisterForm({
      name: '',
      email: 'user@test.com',
      password: 'short',
    })

    expect(errors.name).toBeTruthy()
    expect(errors.password).toMatch(/8 characters/)
  })
})
