/**
 * EmptyState — centered empty/error panel with optional icon and CTA.
 * @see designdoc.md §6.8
 */
import './EmptyState.css'

/**
 * @param {object} props
 * @param {string} props.title
 * @param {string} [props.description]
 * @param {import('react').ReactNode} [props.action]
 * @param {import('react').ComponentType<{size?: number, strokeWidth?: number, className?: string}>} [props.icon]
 */
export default function EmptyState({
  title,
  description,
  action,
  icon: Icon,
  className = '',
}) {
  return (
    <section
      className={`empty-state${className ? ` ${className}` : ''}`}
      aria-labelledby="empty-state-title"
    >
      {Icon && (
        <Icon
          className="empty-state__icon"
          aria-hidden="true"
          size={48}
          strokeWidth={1.5}
        />
      )}
      <h2 id="empty-state-title" className="empty-state__title">
        {title}
      </h2>
      {description && (
        <p className="empty-state__description">{description}</p>
      )}
      {action && <div className="empty-state__action">{action}</div>}
    </section>
  )
}
