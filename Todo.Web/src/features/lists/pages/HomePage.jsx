import { useState } from 'react'
import { Link, useNavigate } from 'react-router-dom'
import { LayoutList, Plus } from 'lucide-react'
import { createList } from '@/api/lists'
import { useAuth } from '@/features/auth'
import { useCurrentWorkspaceRole } from '@/features/workspaces/hooks/useWorkspaceRole'
import { useWorkspace } from '@/features/workspaces/hooks/useWorkspace'
import { useLists } from '@/features/lists/hooks/useLists'
import { Alert, Button, Card, EmptyState, Input, Skeleton } from '@/shared/components/ui'
import { BlurFade, MagicCard } from '@/shared/components/magic-ui'
import { getErrorMessage } from '@/shared/utils/getErrorMessage'
import {
  hasCreateListFormErrors,
  validateCreateListForm,
} from '@/features/lists/validation/createListForm'
import './HomePage.css'

const RECENT_LIST_LIMIT = 5

export function HomePage() {
  const navigate = useNavigate()
  const { user } = useAuth()
  const { currentWorkspace } = useWorkspace()
  const { canWrite } = useCurrentWorkspaceRole()
  const { lists, status, error, refetch } = useLists()
  const [title, setTitle] = useState('')
  const [color, setColor] = useState('')
  const [fieldErrors, setFieldErrors] = useState({})
  const [submitting, setSubmitting] = useState(false)
  const [submitError, setSubmitError] = useState(null)

  const recentLists = lists.slice(0, RECENT_LIST_LIMIT)
  const hasLists = status === 'success' && lists.length > 0

  async function handleCreate(e) {
    e.preventDefault()

    const errors = validateCreateListForm({ title, color })
    if (hasCreateListFormErrors(errors)) {
      setFieldErrors(errors)
      return
    }

    setFieldErrors({})
    setSubmitError(null)
    setSubmitting(true)

    try {
      const list = await createList({
        title: title.trim(),
        color: color.trim() || null,
        workspaceId: currentWorkspace?.id,
      })
      setTitle('')
      setColor('')
      await refetch()
      navigate(`/lists/${list.id}`)
    } catch (err) {
      setSubmitError(getErrorMessage(err, 'Failed to create list'))
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <section className="home-page" aria-labelledby="home-heading">
      <BlurFade>
        <header className="home-page__header">
          <p className="home-page__eyebrow">
            {currentWorkspace?.isPersonal ? 'Personal workspace' : currentWorkspace?.name ?? 'Workspace'}
          </p>
          <h2 id="home-heading" className="home-page__title">
            {user?.name ? `Hello, ${user.name}` : 'Your workspace'}
          </h2>
          <p className="home-page__subtitle">
            {hasLists
              ? 'Pick up where you left off, or start something new.'
              : 'Create your first list to begin tracking what matters.'}
          </p>
        </header>
      </BlurFade>

      <div className="home-page__grid">
        <BlurFade delay={0.08}>
          <MagicCard className="home-page__create-card">
            <Card padding="lg" elevation={0} className="home-page__create-inner card--flat">
              <h3 className="home-page__section-title">
                <Plus aria-hidden="true" size={18} strokeWidth={2} />
                Quick create
              </h3>
              {!canWrite ? (
                <EmptyState
                  title="View only"
                  description="You can browse lists in this workspace but cannot create new ones."
                />
              ) : (
              <>
              <form className="home-page__form" onSubmit={handleCreate} noValidate>
                <Input
                  id="list-title"
                  name="title"
                  label="Title"
                  value={title}
                  onChange={(e) => {
                    setTitle(e.target.value)
                    if (fieldErrors.title) {
                      setFieldErrors((prev) => ({ ...prev, title: undefined }))
                    }
                  }}
                  error={fieldErrors.title}
                />
                <Input
                  id="list-color"
                  name="color"
                  label="Color (optional)"
                  value={color}
                  onChange={(e) => setColor(e.target.value)}
                />
                <div className="home-page__actions">
                  <Button type="submit" variant="primary" disabled={submitting} loading={submitting}>
                    Create list
                  </Button>
                </div>
              </form>
              {submitError && <Alert variant="error" message={submitError} />}
              </>
              )}
            </Card>
          </MagicCard>
        </BlurFade>

        <BlurFade delay={0.14}>
          <section className="home-page__recent" aria-labelledby="recent-lists-heading">
            <h3 id="recent-lists-heading" className="home-page__section-title">
              <LayoutList aria-hidden="true" size={18} strokeWidth={2} />
              Recent lists
            </h3>

            {status === 'loading' && (
              <div className="home-page__recent-loading" aria-busy="true">
                <Skeleton variant="rect" height="52px" />
                <Skeleton variant="rect" height="52px" />
              </div>
            )}

            {status === 'error' && (
              <EmptyState
                title="Couldn't load lists"
                description={getErrorMessage(error, 'Failed to load lists')}
                action={
                  <Button variant="secondary" onClick={refetch}>
                    Try again
                  </Button>
                }
              />
            )}

            {status === 'success' && recentLists.length === 0 && (
              <EmptyState
                title="No lists yet"
                description="Your lists will show up here once you create one."
              />
            )}

            {status === 'success' && recentLists.length > 0 && (
              <ul className="home-page__recent-list">
                {recentLists.map((list) => (
                  <li key={list.id}>
                    <Link to={`/lists/${list.id}`} className="home-page__recent-link">
                      <span
                        className="home-page__recent-swatch"
                        style={{ backgroundColor: list.color || 'var(--color-brand-600)' }}
                        aria-hidden="true"
                      />
                      <span className="home-page__recent-copy">
                        <span className="home-page__recent-title">{list.title}</span>
                      </span>
                    </Link>
                  </li>
                ))}
              </ul>
            )}
          </section>
        </BlurFade>
      </div>
    </section>
  )
}
