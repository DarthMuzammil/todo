import { TASK_STATUS_DONE } from '@/shared/constants/taskEnums'

export function formatDueDate(dueDate) {
  if (!dueDate) {
    return null
  }

  const date = new Date(dueDate)
  if (Number.isNaN(date.getTime())) {
    return null
  }

  return new Intl.DateTimeFormat(undefined, { dateStyle: 'medium' }).format(
    date,
  )
}

export function isTaskOverdue(dueDate, status) {
  if (!dueDate || status === TASK_STATUS_DONE) {
    return false
  }

  const due = new Date(dueDate)
  if (Number.isNaN(due.getTime())) {
    return false
  }

  const today = new Date()
  today.setHours(0, 0, 0, 0)
  due.setHours(0, 0, 0, 0)

  return due < today
}
