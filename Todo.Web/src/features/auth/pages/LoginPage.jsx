import { useState } from 'react'
import { Navigate, useLocation, useNavigate } from 'react-router-dom'
import { Alert, Button, Input } from '@/shared/components/ui'
import AuthFormLayout, {
  AuthFormFooterLink,
} from '@/features/auth/components/AuthFormLayout'
import { useAuth } from '@/features/auth/context/AuthContext'
import {
  hasAuthFormErrors,
  validateLoginForm,
} from '@/features/auth/validation/authForm'
import { getErrorMessage } from '@/shared/utils/getErrorMessage'

export function LoginPage() {
  const navigate = useNavigate()
  const location = useLocation()
  const { isAuthenticated, isLoading, login } = useAuth()
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [fieldErrors, setFieldErrors] = useState({})
  const [submitting, setSubmitting] = useState(false)
  const [submitError, setSubmitError] = useState(null)

  const redirectTo = location.state?.from ?? '/'
  const sessionMessage = location.state?.message
  const inviteReturnPath = typeof redirectTo === 'string' && redirectTo.startsWith('/invites/')
    ? redirectTo
    : null
  const authState = inviteReturnPath ? { from: inviteReturnPath } : location.state

  if (!isLoading && isAuthenticated) {
    return <Navigate to={redirectTo} replace />
  }

  async function handleSubmit(e) {
    e.preventDefault()

    const errors = validateLoginForm({ email, password })
    if (hasAuthFormErrors(errors)) {
      setFieldErrors(errors)
      return
    }

    setFieldErrors({})
    setSubmitError(null)
    setSubmitting(true)

    try {
      await login({ email: email.trim(), password })
      navigate(redirectTo, { replace: true })
    } catch (err) {
      setSubmitError(getErrorMessage(err, 'Sign in failed'))
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <AuthFormLayout
      title="Sign in"
      subtitle="Welcome back. Enter your account details."
      footer={
        <AuthFormFooterLink
          prompt="Don't have an account?"
          linkText="Create one"
          to="/register"
          state={authState}
        />
      }
    >
      {inviteReturnPath && (
        <Alert
          variant="info"
          message="Sign in with the email address that received the workspace invite. You will return here to accept it."
        />
      )}
      {sessionMessage && <Alert variant="info" message={sessionMessage} />}
      <form className="auth-form-layout__form" onSubmit={handleSubmit} noValidate>
        <Input
          id="login-email"
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
          id="login-password"
          name="password"
          type="password"
          autoComplete="current-password"
          label="Password"
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
            Sign in
          </Button>
        </div>
      </form>
      {submitError && <Alert variant="error" message={submitError} />}
    </AuthFormLayout>
  )
}
