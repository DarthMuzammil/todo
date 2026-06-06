/**
 * Skeleton — loading placeholder with shimmer animation.
 * @see designdoc.md §6.7
 */
import './Skeleton.css'

/** @typedef {'text' | 'rect' | 'circle'} SkeletonVariant */

/**
 * @param {object} props
 * @param {string} [props.width]
 * @param {string} [props.height]
 * @param {SkeletonVariant} [props.variant]
 * @param {string} [props.className]
 */
export default function Skeleton({
  width,
  height,
  variant = 'rect',
  className = '',
  ...rest
}) {
  const style = {
    width,
    height,
  }

  return (
    <div
      className={`skeleton skeleton--${variant}${className ? ` ${className}` : ''}`}
      style={style}
      aria-hidden="true"
      {...rest}
    />
  )
}
