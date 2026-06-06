/**
 * Input — labeled text field with error and hint support.
 * @see designdoc.md §6.2
 */
import Alert from './Alert'
import './Input.css'

/**
 * @param {object} props
 * @param {string} props.id
 * @param {string} props.label
 * @param {string} [props.error]
 * @param {string} [props.hint]
 * @param {string} [props.className]
 */
export default function Input({
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
    <div className={`input-field${className ? ` ${className}` : ''}`}>
      <label className="input-field__label" htmlFor={id}>
        {label}
      </label>
      <input
        id={id}
        className={`input-field__control${error ? ' input-field__control--error' : ''}`}
        aria-invalid={error ? true : undefined}
        aria-describedby={describedBy}
        {...rest}
      />
      {hint && (
        <p id={hintId} className="input-field__hint">
          {hint}
        </p>
      )}
      {error && (
        <Alert id={errorId} variant="error" compact message={error} />
      )}
    </div>
  )
}
