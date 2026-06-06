import { useState, type SubmitEvent } from 'react'
import { useNavigate } from 'react-router-dom'
import { useMutation } from '@tanstack/react-query'
import { createList } from '@/api/lists'
import { ApiClientError } from '@/api/client'
import type { CreateListRequest, TodoList } from '@/api/types'
import {
  hasCreateListFormErrors,
  validateCreateListForm,
  type CreateListFieldErrors,
} from '@/features/lists/validation/createListForm'
import { DEV_OWNER_ID } from '@/shared/config/dev'

export function HomePage() {
  const navigate = useNavigate()
  const [title, setTitle] = useState('')
  const [color, setColor] = useState('')
  const [fieldErrors, setFieldErrors] = useState<CreateListFieldErrors>({})

  const createListMutation = useMutation<TodoList, Error, CreateListRequest>({
    mutationFn: createList,
    onSuccess: (list) => {
      navigate(`/lists/${list.id}`)
    },
  })

  function handleCreate(e: SubmitEvent<HTMLFormElement>) {
    e.preventDefault()

    const errors = validateCreateListForm({ title, color })
    if (hasCreateListFormErrors(errors)) {
      setFieldErrors(errors)
      return
    }

    setFieldErrors({})
    createListMutation.mutate({
      ownerId: DEV_OWNER_ID,
      title: title.trim(),
      color: color.trim() || null,
    })
  }

  const errorMessage =
    createListMutation.isError && createListMutation.error instanceof ApiClientError
      ? createListMutation.error.body.error
      : createListMutation.isError
        ? 'Failed to create list'
        : null

  return (
    <>
      <p>No lists found. Create one to get started.</p>
      <form onSubmit={handleCreate} noValidate>
        <label>
          Title
          <input
            name="title"
            value={title}
            onChange={(e) => {
              setTitle(e.target.value)
              if (fieldErrors.title) {
                setFieldErrors((prev) => ({ ...prev, title: undefined }))
              }
            }}
            aria-invalid={!!fieldErrors.title}
            aria-describedby={fieldErrors.title ? 'title-error' : undefined}
          />
        </label>
        {fieldErrors.title && (
          <p id="title-error" role="alert">
            {fieldErrors.title}
          </p>
        )}
        <label>
          Color (optional)
          <input
            name="color"
            value={color}
            onChange={(e) => setColor(e.target.value)}
          />
        </label>
        <button type="submit" disabled={createListMutation.isPending}>
          {createListMutation.isPending ? 'Creating…' : 'Create'}
        </button>
      </form>
      {errorMessage && <p role="alert">{errorMessage}</p>}
    </>
  )
}
