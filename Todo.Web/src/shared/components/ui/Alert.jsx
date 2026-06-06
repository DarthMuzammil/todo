/**
 * Alert — inline error, warning, and info messages.
 * @see designdoc.md §6.9
 */
import { AlertCircle, AlertTriangle, Info } from 'lucide-react'
import './Alert.css'

const ICONS = {
  error: AlertCircle,
  warning: AlertTriangle,
  info: Info,
}

/**
 * @param {object} props
 * @param {'error' | 'warning' | 'info'} [props.variant]
 * @param {string} props.message
 * @param {string} [props.id]
 * @param {boolean} [props.compact]
 */
export default function Alert({
  variant = 'error',
  message,
  id,
  compact = false,
}) {
  if (!message) {
    return null
  }

  const Icon = ICONS[variant]
  const role = variant === 'error' ? 'alert' : 'status'

  return (
    <div
      id={id}
      className={`alert alert--${variant}${compact ? ' alert--compact' : ''}`}
      role={role}
    >
      <Icon className="alert__icon" aria-hidden="true" size={16} strokeWidth={2} />
      <p className="alert__message">{message}</p>
    </div>
  )
}
