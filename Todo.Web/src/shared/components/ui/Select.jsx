/**
 * Select — native select with label, error, and hint support.
 * @see designdoc.md §6.3
 */
import { ChevronDown } from 'lucide-react'
import Alert from './Alert'
import './Select.css'

/**
 * @param {object} props
 * @param {string} props.id
 * @param {string} props.label
 * @param {string} [props.error]
 * @param {string} [props.hint]
 * @param {import('react').ReactNode} props.children
 */
export default function Select({
  id,
  label,
  error,
  hint,
  className = '',
  children,
  ...rest
}) {
  const hintId = hint ? `${id}-hint` : undefined
  const errorId = error ? `${id}-error` : undefined
  const describedBy =
    [hintId, errorId].filter(Boolean).join(' ') || undefined

  return (
    <div className={`select-field${className ? ` ${className}` : ''}`}>
      <label className="select-field__label" htmlFor={id}>
        {label}
      </label>
      <div className="select-field__wrapper">
        <select
          id={id}
          className={`select-field__control${error ? ' select-field__control--error' : ''}`}
          aria-invalid={error ? true : undefined}
          aria-describedby={describedBy}
          {...rest}
        >
          {children}
        </select>
        <ChevronDown
          className="select-field__chevron"
          aria-hidden="true"
          size={16}
          strokeWidth={2}
        />
      </div>
      {hint && (
        <p id={hintId} className="select-field__hint">
          {hint}
        </p>
      )}
      {error && (
        <Alert id={errorId} variant="error" compact message={error} />
      )}
    </div>
  )
}
