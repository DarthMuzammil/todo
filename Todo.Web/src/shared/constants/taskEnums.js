export const STATUS_OPTIONS = [
  { value: 0, label: 'Todo' },
  { value: 1, label: 'In progress' },
  { value: 2, label: 'Done' },
  { value: 3, label: 'Cancelled' },
]

export const PRIORITY_OPTIONS = [
  { value: 0, label: 'Low' },
  { value: 1, label: 'Medium' },
  { value: 2, label: 'High' },
]

export const DEFAULT_TASK_PRIORITY = 1

export const TASK_STATUS_DONE = 2

export function getOptionLabel(options, value) {
  return options.find((option) => option.value === value)?.label ?? 'Unknown'
}

export function getStatusLabel(status) {
  return getOptionLabel(STATUS_OPTIONS, status)
}

export function getPriorityLabel(priority) {
  return getOptionLabel(PRIORITY_OPTIONS, priority)
}
