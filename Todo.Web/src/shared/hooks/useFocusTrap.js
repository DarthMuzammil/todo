import { useEffect, useRef } from 'react'

const FOCUSABLE =
  'a[href], button:not([disabled]), textarea:not([disabled]), input:not([disabled]), select:not([disabled]), [tabindex]:not([tabindex="-1"])'

/**
 * @param {React.RefObject<HTMLElement | null>} containerRef
 * @param {boolean} active
 * @param {() => void} [onEscape]
 */
export function useFocusTrap(containerRef, active, onEscape) {
  const previouslyFocused = useRef(/** @type {HTMLElement | null} */ (null))

  useEffect(() => {
    if (!active || !containerRef.current) {
      return
    }

    previouslyFocused.current = document.activeElement

    const container = containerRef.current
    const focusable = container.querySelectorAll(FOCUSABLE)
    const first = /** @type {HTMLElement | undefined} */ (focusable[0])
    const last = /** @type {HTMLElement | undefined} */ (
      focusable[focusable.length - 1]
    )

    first?.focus()

    function handleKeyDown(event) {
      if (event.key === 'Escape') {
        onEscape?.()
        return
      }

      if (event.key !== 'Tab' || !first || !last) {
        return
      }

      if (event.shiftKey && document.activeElement === first) {
        event.preventDefault()
        last.focus()
      } else if (!event.shiftKey && document.activeElement === last) {
        event.preventDefault()
        first.focus()
      }
    }

    container.addEventListener('keydown', handleKeyDown)

    return () => {
      container.removeEventListener('keydown', handleKeyDown)
      previouslyFocused.current?.focus()
    }
  }, [active, containerRef, onEscape])
}
