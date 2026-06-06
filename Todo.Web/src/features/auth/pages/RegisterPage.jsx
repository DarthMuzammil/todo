import { useState } from 'react'
import { Navigate, useLocation, useNavigate } from 'react-router-dom'
import { Alert, Button, Input } from '@/shared/components/ui'
import AuthFormLayout, {
  AuthFormFooterLink,
} from '@/features/auth/components/AuthFormLayout'
import { useAuth } from '@/features/auth/context/AuthContext'
import {
  hasAuthFormErrors,
  validateRegisterForm,
} from '@/features/auth/validation/authForm'
import { getErrorMessage } from '@/shared/utils/getErrorMessage'

export function RegisterPage() {
  const navigate = useNavigate()
  const location = useLocation()
  const { isAuthenticated, isLoading, register } = useAuth()
  const [name, setName] = useState('')
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [fieldErrors, setFieldErrors] = useState({})
  const [submitting, setSubmitting] = useState(false)
  const [submitError, setSubmitError] = useState(null)

  const redirectTo = location.state?.from ?? '/'
  const inviteReturnPath = typeof redirectTo === 'string' && redirectTo.startsWith('/invites/')
    ? redirectTo
    : null
  const authState = inviteReturnPath ? { from: inviteReturnPath } : location.state

  if (!isLoading && isAuthenticated) {
    return <Navigate to={redirectTo} replace />
  }

  async function handleSubmit(e) {
    e.preventDefault()

    const errors = validateRegisterForm({ name, email, password })
    if (hasAuthFormErrors(errors)) {
      setFieldErrors(errors)
      return
    }

    setFieldErrors({})
    setSubmitError(null)
    setSubmitting(true)

    try {
      await register({
        name: name.trim(),
        email: email.trim(),
        password,
      })
      navigate(redirectTo, { replace: true })
    } catch (err) {
      setSubmitError(getErrorMessage(err, 'Registration failed'))
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <AuthFormLayout
      title="Create account"
      subtitle={
        inviteReturnPath
          ? 'Use the same email address that received the workspace invite.'
          : 'Start organizing tasks in seconds.'
      }
      footer={
        <AuthFormFooterLink
          prompt="Already have an account?"
          linkText="Sign in"
          to="/login"
          state={authState}
        />
      }
    >
      {inviteReturnPath && (
        <Alert
          variant="info"
          message="After creating your account, you will return to the invite page to accept it."
        />
      )}
      <form className="auth-form-layout__form" onSubmit={handleSubmit} noValidate>
        <Input
          id="register-name"
          name="name"
          autoComplete="name"
          label="Name"
          value={name}
          onChange={(e) => {
            setName(e.target.value)
            if (fieldErrors.name) {
              setFieldErrors((prev) => ({ ...prev, name: undefined }))
            }
          }}
          error={fieldErrors.name}
        />
        <Input
          id="register-email"
          name="email"
          type="email"
          autoComplete="email"
          label="Email"
          value={email}
          onChange={(e) => {
            setEmail(e.target.value)
            if (fieldErrors.email) {
              setFieldErrors((prev) => ({ ...prev, email: undefined }))
            }
          }}
          error={fieldErrors.email}
        />
        <Input
          id="register-password"
          name="password"
          type="password"
          autoComplete="new-password"
          label="Password"
          hint="At least 8 characters with a number and uppercase letter."
          value={password}
          onChange={(e) => {
            setPassword(e.target.value)
            if (fieldErrors.password) {
              setFieldErrors((prev) => ({ ...prev, password: undefined }))
            }
          }}
          error={fieldErrors.password}
        />
        <div className="auth-form-layout__actions">
          <Button type="submit" variant="primary" disabled={submitting} loading={submitting}>
            Create account
          </Button>
        </div>
      </form>
      {submitError && <Alert variant="error" message={submitError} />}
    </AuthFormLayout>
  )
}
