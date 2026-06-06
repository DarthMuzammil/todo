import { useCallback, useRef, useState } from 'react'
import { Trash2 } from 'lucide-react'
import { deleteTask, updateTaskStatus } from '@/api/tasks'
import {
  getPriorityLabel,
  getStatusLabel,
  STATUS_OPTIONS,
} from '@/shared/constants/taskEnums'
import {
  getPriorityBadgeVariant,
  getStatusBadgeVariant,
  isCancelledStatus,
} from '@/shared/constants/badgeVariants'
import {
  Alert,
  Badge,
  Button,
  Card,
  ConfirmDialog,
  Select,
} from '@/shared/components/ui'
import { MagicCard, StaggerItem } from '@/shared/components/magic-ui'
import { getErrorMessage } from '@/shared/utils/getErrorMessage'
import {
  formatDueDate,
  isTaskOverdue,
} from '@/features/lists/utils/formatDueDate'
import './TaskListItem.css'

export default function TaskListItem({ listId, task, index = 0, onChanged, readOnly = false }) {
  const [isPending, setIsPending] = useState(false)
  const [isDeletePending, setIsDeletePending] = useState(false)
  const [confirmingDelete, setConfirmingDelete] = useState(false)
  const [actionError, setActionError] = useState(null)
  const deleteButtonRef = useRef(null)
  const isBusy = isPending || isDeletePending

  const formattedDueDate = formatDueDate(task.dueDate)
  const overdue = isTaskOverdue(task.dueDate, task.status)
  const statusSelectId = `task-status-${task.id}`

  const handleCancelDelete = useCallback(() => {
    setConfirmingDelete(false)
    deleteButtonRef.current?.focus()
  }, [])

  async function handleStatusChange(event) {
    const newStatus = Number(event.target.value)
    if (newStatus === task.status) {
      return
    }

    setActionError(null)
    setIsPending(true)

    try {
      await updateTaskStatus(listId, task.id, { newStatus })
      onChanged?.()
    } catch (err) {
      setActionError(getErrorMessage(err, 'Failed to update status'))
    } finally {
      setIsPending(false)
    }
  }

  async function handleConfirmDelete() {
    setActionError(null)
    setIsDeletePending(true)

    try {
      await deleteTask(listId, task.id)
      setConfirmingDelete(false)
      onChanged?.()
    } catch (err) {
      setActionError(getErrorMessage(err, 'Failed to delete task'))
      throw err
    } finally {
      setIsDeletePending(false)
    }
  }

  return (
    <>
      <StaggerItem index={index} className="task-list__item-wrap">
        <MagicCard className="task-item">
          <Card as="div" padding="lg" className="task-item__inner card--flat">
            <div className="task-item__header">
              <h3 className="task-item__title">{task.title}</h3>
              <div className="task-item__badges">
                <Badge variant={getPriorityBadgeVariant(task.priority)}>
                  {getPriorityLabel(task.priority)}
                </Badge>
                <Badge
                  variant={getStatusBadgeVariant(task.status)}
                  strikethrough={isCancelledStatus(task.status)}
                  data-testid={`task-status-badge-${task.id}`}
                >
                  {getStatusLabel(task.status)}
                </Badge>
              </div>
            </div>

            {task.description && (
              <p className="task-item__description">{task.description}</p>
            )}

            {formattedDueDate && (
              <p
                className={`task-item__due${overdue ? ' task-item__due--overdue' : ''}`}
              >
                Due {formattedDueDate}
                {overdue && ' (overdue)'}
              </p>
            )}

            {!readOnly && (
            <div className="task-item__actions">
              <Select
                id={statusSelectId}
                label="Status"
                className="task-item__status-select"
                value={task.status}
                disabled={isBusy || confirmingDelete}
                onChange={handleStatusChange}
              >
                {STATUS_OPTIONS.map((option) => (
                  <option key={option.value} value={option.value}>
                    {option.label}
                  </option>
                ))}
              </Select>

              <Button
                ref={deleteButtonRef}
                variant="danger"
                size="sm"
                onClick={() => setConfirmingDelete(true)}
                disabled={isBusy}
                aria-label={`Delete ${task.title}`}
              >
                <Trash2 aria-hidden="true" size={16} strokeWidth={2} />
                Delete
              </Button>
            </div>
            )}

            {isPending && (
              <p className="task-item__feedback" aria-live="polite">
                Updating…
              </p>
            )}
            {actionError && <Alert variant="error" compact message={actionError} />}
          </Card>
        </MagicCard>
      </StaggerItem>

      <ConfirmDialog
        open={confirmingDelete}
        title="Delete task?"
        description={`"${task.title}" will be permanently removed.`}
        confirmLabel="Delete"
        cancelLabel="Cancel"
        variant="danger"
        onConfirm={handleConfirmDelete}
        onCancel={handleCancelDelete}
      />
    </>
  )
}
