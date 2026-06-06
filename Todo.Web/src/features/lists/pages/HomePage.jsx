import { useState } from 'react'
import { useNavigate } from 'react-router-dom'
import { createList } from '@/api/lists'
import { Alert, Button, Input } from '@/shared/components/ui'
import { DEV_OWNER_ID } from '@/shared/config/dev'
import { getErrorMessage } from '@/shared/utils/getErrorMessage'
import {
  hasCreateListFormErrors,
  validateCreateListForm,
} from '@/features/lists/validation/createListForm'
import './HomePage.css'

export function HomePage() {
  const navigate = useNavigate()
  const [title, setTitle] = useState('')
  const [color, setColor] = useState('')
  const [fieldErrors, setFieldErrors] = useState({})
  const [submitting, setSubmitting] = useState(false)
  const [submitError, setSubmitError] = useState(null)

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
        ownerId: DEV_OWNER_ID,
        title: title.trim(),
        color: color.trim() || null,
      })
      navigate(`/lists/${list.id}`)
    } catch (err) {
      setSubmitError(getErrorMessage(err, 'Failed to create list'))
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <section className="home-page" aria-labelledby="create-list-heading">
      <h2 id="create-list-heading" className="home-page__title">
        Create a list
      </h2>
      <p className="home-page__subtitle">
        No lists found. Create one to get started.
      </p>
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
    </section>
  )
}
