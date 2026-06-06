/**
 * Card — surface container for tasks and panels.
 * @see designdoc.md §6.6
 */
import './Card.css'

/**
 * @param {object} props
 * @param {'md' | 'lg'} [props.padding]
 * @param {0 | 1} [props.elevation]
 * @param {'div' | 'li' | 'section'} [props.as]
 */
export default function Card({
  padding = 'md',
  elevation = 0,
  as: Component = 'div',
  className = '',
  children,
  ...rest
}) {
  return (
    <Component
      className={`card card--padding-${padding} card--elevation-${elevation}${className ? ` ${className}` : ''}`}
      {...rest}
    >
      {children}
    </Component>
  )
}
