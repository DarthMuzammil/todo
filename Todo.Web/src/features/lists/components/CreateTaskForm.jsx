import { useState } from 'react'
import { createTask } from '@/api/tasks'
import {
  DEFAULT_TASK_PRIORITY,
  PRIORITY_OPTIONS,
} from '@/shared/constants/taskEnums'
import {
  Alert,
  Button,
  Input,
  Select,
  Textarea,
} from '@/shared/components/ui'
import { getErrorMessage } from '@/shared/utils/getErrorMessage'
import {
  hasCreateTaskFormErrors,
  validateCreateTaskForm,
} from '@/features/lists/validation/createTaskForm'
import './CreateTaskForm.css'

export default function CreateTaskForm({ listId, onTaskCreated }) {
  const [title, setTitle] = useState('')
  const [description, setDescription] = useState('')
  const [priority, setPriority] = useState(DEFAULT_TASK_PRIORITY)
  const [dueDate, setDueDate] = useState('')
  const [fieldErrors, setFieldErrors] = useState({})
  const [submitting, setSubmitting] = useState(false)
  const [submitError, setSubmitError] = useState(null)

  function resetForm() {
    setTitle('')
    setDescription('')
    setPriority(DEFAULT_TASK_PRIORITY)
    setDueDate('')
    setFieldErrors({})
  }

  async function handleSubmit(e) {
    e.preventDefault()

    const errors = validateCreateTaskForm({
      title,
      description,
      priority,
      dueDate,
    })
    if (hasCreateTaskFormErrors(errors)) {
      setFieldErrors(errors)
      return
    }

    setFieldErrors({})
    setSubmitError(null)
    setSubmitting(true)

    try {
      await createTask(listId, {
        title: title.trim(),
        description: description.trim() || null,
        priority,
        dueDate: dueDate || null,
      })
      resetForm()
      onTaskCreated?.()
    } catch (err) {
      setSubmitError(getErrorMessage(err, 'Failed to create task'))
    } finally {
      setSubmitting(false)
    }
  }

  return (
    <CardSection aria-labelledby="create-task-heading">
      <h2 id="create-task-heading" className="create-task-form__title">
        Add task
      </h2>
      <form className="create-task-form" onSubmit={handleSubmit} noValidate>
        <Input
          id="task-title"
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

        <Textarea
          id="task-description"
          name="description"
          label="Description (optional)"
          value={description}
          onChange={(e) => setDescription(e.target.value)}
          rows={3}
        />

        <Select
          id="task-priority"
          name="priority"
          label="Priority"
          value={priority}
          onChange={(e) => setPriority(Number(e.target.value))}
        >
          {PRIORITY_OPTIONS.map((option) => (
            <option key={option.value} value={option.value}>
              {option.label}
            </option>
          ))}
        </Select>

        <Input
          id="task-due-date"
          name="dueDate"
          type="date"
          label="Due date (optional)"
          value={dueDate}
          onChange={(e) => setDueDate(e.target.value)}
        />

        <div className="create-task-form__actions">
          <Button type="submit" variant="primary" disabled={submitting} loading={submitting}>
            Add task
          </Button>
        </div>
      </form>
      {submitError && <Alert variant="error" message={submitError} />}
    </CardSection>
  )
}

function CardSection({ children, ...rest }) {
  return (
    <section className="create-task-form__section" {...rest}>
      {children}
    </section>
  )
}
