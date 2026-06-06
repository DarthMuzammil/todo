/**
 * Textarea — multiline input with label, error, and hint support.
 * @see designdoc.md §6.4
 */
import Alert from './Alert'
import './Textarea.css'

/**
 * @param {object} props
 * @param {string} props.id
 * @param {string} props.label
 * @param {string} [props.error]
 * @param {string} [props.hint]
 */
export default function Textarea({
  id,
  label,
  error,
  hint,
  className = '',
  ...rest
}) {
  const hintId = hint ? `${id}-hint` : undefined
  const errorId = error ? `${id}-error` : undefined
  const describedBy =
    [hintId, errorId].filter(Boolean).join(' ') || undefined

  return (
    <div className={`textarea-field${className ? ` ${className}` : ''}`}>
      <label className="textarea-field__label" htmlFor={id}>
        {label}
      </label>
      <textarea
        id={id}
        className={`textarea-field__control${error ? ' textarea-field__control--error' : ''}`}
        aria-invalid={error ? true : undefined}
        aria-describedby={describedBy}
        {...rest}
      />
      {hint && (
        <p id={hintId} className="textarea-field__hint">
          {hint}
        </p>
      )}
      {error && (
        <Alert id={errorId} variant="error" compact message={error} />
      )}
    </div>
  )
}
