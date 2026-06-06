import { useState } from 'react'
import { Link } from 'react-router-dom'
import { logout } from '@/api/auth'
import { changePassword, logoutAllSessions, updateProfile } from '@/api/users'
import { useAuth } from '@/features/auth'
import { Alert, Button, Card, Input } from '@/shared/components/ui'
import {
  hasAuthFormErrors,
  validateChangePasswordForm,
  validateProfileForm,
} from '@/features/auth/validation/authForm'
import { getErrorMessage } from '@/shared/utils/getErrorMessage'
import './SettingsPage.css'

export function SettingsPage() {
  const { user, updateUser } = useAuth()
  const [name, setName] = useState(user?.name ?? '')
  const [profileErrors, setProfileErrors] = useState({})
  const [profileMessage, setProfileMessage] = useState(null)
  const [profileError, setProfileError] = useState(null)
  const [profileSubmitting, setProfileSubmitting] = useState(false)

  const [currentPassword, setCurrentPassword] = useState('')
  const [newPassword, setNewPassword] = useState('')
  const [passwordErrors, setPasswordErrors] = useState({})
  const [passwordMessage, setPasswordMessage] = useState(null)
  const [passwordError, setPasswordError] = useState(null)
  const [passwordSubmitting, setPasswordSubmitting] = useState(false)

  const [logoutAllSubmitting, setLogoutAllSubmitting] = useState(false)
  const [logoutAllMessage, setLogoutAllMessage] = useState(null)
  const [logoutAllError, setLogoutAllError] = useState(null)

  async function handleProfileSubmit(e) {
    e.preventDefault()
    const errors = validateProfileForm({ name })
    if (hasAuthFormErrors(errors)) {
      setProfileErrors(errors)
      return
    }

    setProfileErrors({})
    setProfileError(null)
    setProfileMessage(null)
    setProfileSubmitting(true)

    try {
      const profile = await updateProfile({ name: name.trim() })
      updateUser(profile)
      setProfileMessage('Profile updated.')
    } catch (err) {
      setProfileError(getErrorMessage(err, 'Failed to update profile'))
    } finally {
      setProfileSubmitting(false)
    }
  }

  async function handlePasswordSubmit(e) {
    e.preventDefault()
    const errors = validateChangePasswordForm({ currentPassword, newPassword })
    if (hasAuthFormErrors(errors)) {
      setPasswordErrors(errors)
      return
    }

    setPasswordErrors({})
    setPasswordError(null)
    setPasswordMessage(null)
    setPasswordSubmitting(true)

    try {
      await changePassword({ currentPassword, newPassword })
      setCurrentPassword('')
      setNewPassword('')
      setPasswordMessage('Password changed.')
    } catch (err) {
      setPasswordError(getErrorMessage(err, 'Failed to change password'))
    } finally {
      setPasswordSubmitting(false)
    }
  }

  async function handleLogoutAll() {
    setLogoutAllError(null)
    setLogoutAllMessage(null)
    setLogoutAllSubmitting(true)

    try {
      await logoutAllSessions()
      await logout()
      setLogoutAllMessage('Signed out everywhere.')
    } catch (err) {
      setLogoutAllError(getErrorMessage(err, 'Failed to sign out everywhere'))
    } finally {
      setLogoutAllSubmitting(false)
    }
  }

  return (
    <section className="settings-page" aria-labelledby="settings-heading">
      <header className="settings-page__header">
        <h1 id="settings-heading" className="settings-page__title">
          Settings
        </h1>
        <p className="settings-page__subtitle">
          Manage your profile and account security.
        </p>
      </header>

      <Card padding="lg" className="settings-page__card">
        <h2 className="settings-page__section-title">Profile</h2>
        <p className="settings-page__meta">Signed in as {user?.email}</p>
        <form className="settings-page__form" onSubmit={handleProfileSubmit} noValidate>
          <Input
            id="settings-name"
            name="name"
            label="Display name"
            autoComplete="name"
            value={name}
            onChange={(e) => {
              setName(e.target.value)
              if (profileErrors.name) {
                setProfileErrors((prev) => ({ ...prev, name: undefined }))
              }
            }}
            error={profileErrors.name}
            disabled={profileSubmitting}
          />
          <Button type="submit" variant="primary" loading={profileSubmitting} disabled={profileSubmitting}>
            Save profile
          </Button>
        </form>
        {profileMessage && <Alert variant="info" message={profileMessage} />}
        {profileError && <Alert variant="error" message={profileError} />}
      </Card>

      <Card padding="lg" className="settings-page__card">
        <h2 className="settings-page__section-title">Password</h2>
        <form className="settings-page__form" onSubmit={handlePasswordSubmit} noValidate>
          <Input
            id="settings-current-password"
            name="currentPassword"
            type="password"
            autoComplete="current-password"
            label="Current password"
            value={currentPassword}
            onChange={(e) => {
              setCurrentPassword(e.target.value)
              if (passwordErrors.currentPassword) {
                setPasswordErrors((prev) => ({ ...prev, currentPassword: undefined }))
              }
            }}
            error={passwordErrors.currentPassword}
            disabled={passwordSubmitting}
          />
          <Input
            id="settings-new-password"
            name="newPassword"
            type="password"
            autoComplete="new-password"
            label="New password"
            hint="At least 8 characters with a number and uppercase letter."
            value={newPassword}
            onChange={(e) => {
              setNewPassword(e.target.value)
              if (passwordErrors.newPassword) {
                setPasswordErrors((prev) => ({ ...prev, newPassword: undefined }))
              }
            }}
            error={passwordErrors.newPassword}
            disabled={passwordSubmitting}
          />
          <Button type="submit" variant="primary" loading={passwordSubmitting} disabled={passwordSubmitting}>
            Change password
          </Button>
        </form>
        {passwordMessage && <Alert variant="info" message={passwordMessage} />}
        {passwordError && <Alert variant="error" message={passwordError} />}
      </Card>

      <Card padding="lg" className="settings-page__card">
        <h2 className="settings-page__section-title">Sessions</h2>
        <p className="settings-page__copy">
          Sign out on every device that is currently logged in to your account.
        </p>
        <div className="settings-page__actions">
          <Button
            type="button"
            variant="danger"
            onClick={handleLogoutAll}
            loading={logoutAllSubmitting}
            disabled={logoutAllSubmitting}
          >
            Sign out everywhere
          </Button>
          <Link to="/" className="btn btn--secondary btn--md">
            Back to home
          </Link>
        </div>
        {logoutAllMessage && <Alert variant="info" message={logoutAllMessage} />}
        {logoutAllError && <Alert variant="error" message={logoutAllError} />}
      </Card>
    </section>
  )
}
