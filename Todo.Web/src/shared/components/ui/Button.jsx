/**
 * Button — primary, secondary, ghost, and danger variants.
 * @see designdoc.md §6.1
 */
import { forwardRef } from 'react'
import './Button.css'

/** @typedef {'primary' | 'secondary' | 'ghost' | 'danger'} ButtonVariant */
/** @typedef {'sm' | 'md' | 'lg'} ButtonSize */

const Button = forwardRef(function Button(
  {
    variant = 'secondary',
    size = 'md',
    disabled = false,
    loading = false,
    type = 'button',
    className = '',
    children,
    ...rest
  },
  ref,
) {
  const isDisabled = disabled || loading

  return (
    <button
      ref={ref}
      type={type}
      disabled={isDisabled}
      aria-busy={loading || undefined}
      className={`btn btn--${variant} btn--${size}${className ? ` ${className}` : ''}`}
      {...rest}
    >
      {loading ? `${children}…` : children}
    </button>
  )
})

export default Button
