/**
 * Badge — labeled status/priority indicator.
 * @see designdoc.md §6.5
 */
import './Badge.css'

/** @typedef {'neutral' | 'info' | 'success' | 'warning' | 'danger'} BadgeVariant */

/**
 * @param {object} props
 * @param {BadgeVariant} [props.variant]
 * @param {boolean} [props.strikethrough]
 * @param {import('react').ReactNode} props.children
 */
export default function Badge({
  variant = 'neutral',
  strikethrough = false,
  className = '',
  children,
  ...rest
}) {
  return (
    <span
      className={`badge badge--${variant}${strikethrough ? ' badge--strikethrough' : ''}${className ? ` ${className}` : ''}`}
      {...rest}
    >
      {children}
    </span>
  )
}
