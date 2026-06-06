import { useCallback, useRef, useState } from 'react'
import { Pencil, Trash2 } from 'lucide-react'
import { deleteList, updateList } from '@/api/lists'
import {
  hasCreateListFormErrors,
  validateCreateListForm,
} from '@/features/lists/validation/createListForm'
import { Alert, Badge, Button, ConfirmDialog, Input } from '@/shared/components/ui'
import { getErrorMessage } from '@/shared/utils/getErrorMessage'
import './ListHeader.css'

export default function ListHeader({
  listId,
  title,
  color,
  readOnly = false,
  isViewer = false,
  connectionState = 'idle',
  onUpdated,
  onDeleted,
}) {
  const [isEditing, setIsEditing] = useState(false)
  const [editTitle, setEditTitle] = useState(title)
  const [editColor, setEditColor] = useState(color ?? '')
  const [fieldErrors, setFieldErrors] = useState({})
  const [isSaving, setIsSaving] = useState(false)
  const [saveError, setSaveError] = useState(null)
  const [confirmingDelete, setConfirmingDelete] = useState(false)
  const [isDeletePending, setIsDeletePending] = useState(false)
  const [deleteError, setDeleteError] = useState(null)
  const editButtonRef = useRef(null)
  const deleteButtonRef = useRef(null)

  const isBusy = isSaving || isDeletePending

  const handleCancelEdit = useCallback(() => {
    setIsEditing(false)
    setEditTitle(title)
    setEditColor(color ?? '')
    setFieldErrors({})
    setSaveError(null)
    editButtonRef.current?.focus()
  }, [color, title])

  const handleCancelDelete = useCallback(() => {
    setConfirmingDelete(false)
    setDeleteError(null)
    deleteButtonRef.current?.focus()
  }, [])

  function startEditing() {
    setEditTitle(title)
    setEditColor(color ?? '')
    setFieldErrors({})
    setSaveError(null)
    setIsEditing(true)
  }

  async function handleSave(e) {
    e.preventDefault()

    const errors = validateCreateListForm({ title: editTitle, color: editColor })
    if (hasCreateListFormErrors(errors)) {
      setFieldErrors(errors)
      return
    }

    const trimmedTitle = editTitle.trim()
    const trimmedColor = editColor.trim()
    const body = { title: trimmedTitle }
    if (trimmedColor) {
      body.color = trimmedColor
    }

    setFieldErrors({})
    setSaveError(null)
    setIsSaving(true)

    try {
      await updateList(listId, body)
      setIsEditing(false)
      onUpdated?.()
    } catch (err) {
      setSaveError(getErrorMessage(err, 'Failed to update list'))
    } finally {
      setIsSaving(false)
    }
  }

  async function handleConfirmDelete() {
    setDeleteError(null)
    setIsDeletePending(true)

    try {
      await deleteList(listId)
      setConfirmingDelete(false)
      onDeleted?.()
    } catch (err) {
      setDeleteError(getErrorMessage(err, 'Failed to delete list'))
      throw err
    } finally {
      setIsDeletePending(false)
    }
  }

  const connectionLabel =
    connectionState === 'live'
      ? 'Live'
      : connectionState === 'reconnecting'
        ? 'Reconnecting…'
        : connectionState === 'connecting'
          ? 'Connecting…'
          : null

  if (isEditing) {
    return (
      <header className="list-header">
        <form className="list-header__edit-form" onSubmit={handleSave} noValidate>
          <Input
            id="list-edit-title"
            name="title"
            label="Title"
            value={editTitle}
            onChange={(e) => {
              setEditTitle(e.target.value)
              if (fieldErrors.title) {
                setFieldErrors((prev) => ({ ...prev, title: undefined }))
              }
            }}
            error={fieldErrors.title}
            disabled={isBusy}
          />
          <Input
            id="list-edit-color"
            name="color"
            label="Color (optional)"
            value={editColor}
            onChange={(e) => setEditColor(e.target.value)}
            disabled={isBusy}
          />
          <div className="list-header__edit-actions">
            <Button type="submit" variant="primary" disabled={isBusy} loading={isSaving}>
              Save
            </Button>
            <Button
              type="button"
              variant="secondary"
              onClick={handleCancelEdit}
              disabled={isBusy}
            >
              Cancel
            </Button>
          </div>
        </form>
        {saveError && <Alert variant="error" compact message={saveError} />}
      </header>
    )
  }

  return (
    <>
      <header className="list-header">
        <div className="list-header__main">
          <h1 className="list-header__title">{title}</h1>
          {connectionLabel && (
            <Badge
              variant={connectionState === 'live' ? 'success' : 'neutral'}
              data-testid="sync-status-badge"
            >
              {connectionLabel}
            </Badge>
          )}
          {isViewer && (
            <Badge variant="neutral" data-testid="view-only-badge">
              View only
            </Badge>
          )}
          {color && (
            <span
              className="list-header__swatch"
              style={{ backgroundColor: color }}
              aria-hidden="true"
            />
          )}
        </div>
        {!readOnly && (
        <div className="list-header__actions">
          <Button
            ref={editButtonRef}
            variant="secondary"
            size="sm"
            onClick={startEditing}
            disabled={isBusy || confirmingDelete}
            aria-label={`Rename ${title}`}
          >
            <Pencil aria-hidden="true" size={16} strokeWidth={2} />
            Rename
          </Button>
          <Button
            ref={deleteButtonRef}
            variant="danger"
            size="sm"
            onClick={() => setConfirmingDelete(true)}
            disabled={isBusy || confirmingDelete}
            aria-label={`Delete ${title}`}
          >
            <Trash2 aria-hidden="true" size={16} strokeWidth={2} />
            Delete
          </Button>
        </div>
        )}
        {deleteError && <Alert variant="error" compact message={deleteError} />}
      </header>

      <ConfirmDialog
        open={confirmingDelete}
        title="Delete list?"
        description={`"${title}" and all its tasks will be permanently removed.`}
        confirmLabel="Delete"
        cancelLabel="Cancel"
        variant="danger"
        onConfirm={handleConfirmDelete}
        onCancel={handleCancelDelete}
      />
    </>
  )
}
