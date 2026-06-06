/**
 * Maps task status/priority enum values to Badge variants.
 * @see designdoc.md §2.6
 */

/** @param {number} status */
export function getStatusBadgeVariant(status) {
  switch (status) {
    case 0:
      return 'neutral'
    case 1:
      return 'info'
    case 2:
      return 'success'
    case 3:
      return 'neutral'
    default:
      return 'neutral'
  }
}

/** @param {number} priority */
export function getPriorityBadgeVariant(priority) {
  switch (priority) {
    case 0:
      return 'neutral'
    case 1:
      return 'info'
    case 2:
      return 'warning'
    default:
      return 'neutral'
  }
}

/** @param {number} status */
export function isCancelledStatus(status) {
  return status === 3
}
