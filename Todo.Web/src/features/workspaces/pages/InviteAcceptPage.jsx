import { useState } from 'react'
import { Link, useLocation, useNavigate, useParams } from 'react-router-dom'
import { acceptInvite, declineInvite } from '@/api/workspaces'
import { useAuth } from '@/features/auth'
import { Alert, Button, Card } from '@/shared/components/ui'
import { getErrorMessage } from '@/shared/utils/getErrorMessage'
import './InviteAcceptPage.css'

const SELECTED_WORKSPACE_STORAGE_KEY = 'todo:selectedWorkspaceId'

export function InviteAcceptPage() {
  const { token } = useParams()
  const location = useLocation()
  const navigate = useNavigate()
  const { user, isAuthenticated, isLoading } = useAuth()
  const [status, setStatus] = useState('idle')
  const [error, setError] = useState(null)

  const returnPath = location.pathname

  if (!isLoading && isAuthenticated) {
    return (
      <AuthenticatedInviteAccept
        token={token}
        user={user}
        status={status}
        error={error}
        setStatus={setStatus}
        setError={setError}
        navigate={navigate}
      />
    )
  }

  if (isLoading) {
    return (
      <section className="invite-page" aria-live="polite" aria-busy="true">
        <Card padding="lg" className="invite-page__card">
          <p className="invite-page__copy">Loading invitation…</p>
        </Card>
      </section>
    )
  }

  return (
    <section className="invite-page" aria-labelledby="invite-heading">
      <Card padding="lg" className="invite-page__card">
        <h1 id="invite-heading" className="invite-page__title">
          Workspace invitation
        </h1>
        <p className="invite-page__copy">
          Sign in or create an account with the email that received this invite, then
          return here to accept it.
        </p>
        <div className="invite-page__actions">
          <Link
            to="/login"
            state={{ from: returnPath }}
            className="btn btn--primary btn--md"
          >
            Sign in
          </Link>
          <Link
            to="/register"
            state={{ from: returnPath }}
            className="btn btn--secondary btn--md"
          >
            Create account
          </Link>
        </div>
      </Card>
    </section>
  )
}

function AuthenticatedInviteAccept({
  token,
  user,
  status,
  error,
  setStatus,
  setError,
  navigate,
}) {
  async function handleAccept() {
    setStatus('accepting')
    setError(null)

    try {
      const result = await acceptInvite(token)
      sessionStorage.setItem(SELECTED_WORKSPACE_STORAGE_KEY, result.workspaceId)
      navigate('/', { replace: true })
    } catch (err) {
      setError(getErrorMessage(err, 'Could not accept invite'))
      setStatus('idle')
    }
  }

  async function handleDecline() {
    setStatus('declining')
    setError(null)

    try {
      await declineInvite(token)
      navigate('/', { replace: true })
    } catch (err) {
      setError(getErrorMessage(err, 'Could not decline invite'))
      setStatus('idle')
    }
  }

  return (
    <section className="invite-page" aria-labelledby="invite-heading">
      <Card padding="lg" className="invite-page__card">
        <h1 id="invite-heading" className="invite-page__title">
          Workspace invitation
        </h1>
        <p className="invite-page__copy">
          You are signed in as <strong>{user?.email}</strong>. Accepting adds you to the
          shared workspace so you can see its lists and tasks.
        </p>
        {error && <Alert variant="error" message={error} />}
        <div className="invite-page__actions">
          <Button
            type="button"
            variant="primary"
            onClick={handleAccept}
            disabled={status !== 'idle'}
            loading={status === 'accepting'}
          >
            Accept invite
          </Button>
          <Button
            type="button"
            variant="secondary"
            onClick={handleDecline}
            disabled={status !== 'idle'}
            loading={status === 'declining'}
          >
            Decline
          </Button>
        </div>
      </Card>
    </section>
  )
}
