/**
 * ConfirmDialog — accessible modal for destructive confirmations.
 * @see designdoc.md §6.10
 */
import { useCallback, useEffect, useId, useRef, useState } from 'react'
import { useFocusTrap } from '@/shared/hooks/useFocusTrap'
import Button from './Button'
import './ConfirmDialog.css'

/**
 * @param {object} props
 * @param {boolean} props.open
 * @param {string} props.title
 * @param {string} [props.description]
 * @param {string} [props.confirmLabel]
 * @param {string} [props.cancelLabel]
 * @param {'danger' | 'default'} [props.variant]
 * @param {() => void | Promise<void>} props.onConfirm
 * @param {() => void} props.onCancel
 */
export default function ConfirmDialog({
  open,
  title,
  description,
  confirmLabel = 'Confirm',
  cancelLabel = 'Cancel',
  variant = 'default',
  onConfirm,
  onCancel,
}) {
  const [pending, setPending] = useState(false)
  const panelRef = useRef(/** @type {HTMLDivElement | null} */ (null))
  const cancelRef = useRef(/** @type {HTMLButtonElement | null} */ (null))
  const confirmRef = useRef(/** @type {HTMLButtonElement | null} */ (null))
  const titleId = useId()
  const descriptionId = useId()

  const handleCancel = useCallback(() => {
    setPending(false)
    onCancel()
  }, [onCancel])

  useFocusTrap(panelRef, open, handleCancel)

  useEffect(() => {
    if (!open) {
      return
    }

    const previousOverflow = document.body.style.overflow
    document.body.style.overflow = 'hidden'
    return () => {
      document.body.style.overflow = previousOverflow
    }
  }, [open])

  if (!open) {
    return null
  }

  async function handleConfirm() {
    setPending(true)
    try {
      await onConfirm()
    } finally {
      setPending(false)
    }
  }

  const confirmVariant = variant === 'danger' ? 'danger' : 'primary'

  return (
    <div className="confirm-dialog">
      <button
        type="button"
        className="confirm-dialog__backdrop"
        aria-label="Close dialog"
        onClick={handleCancel}
      />
      <div
        ref={panelRef}
        className="confirm-dialog__panel"
        role="alertdialog"
        aria-modal="true"
        aria-labelledby={titleId}
        aria-describedby={description ? descriptionId : undefined}
      >
        <h2 id={titleId} className="confirm-dialog__title">
          {title}
        </h2>
        {description && (
          <p id={descriptionId} className="confirm-dialog__description">
            {description}
          </p>
        )}
        <div className="confirm-dialog__actions">
          {variant === 'danger' ? (
            <>
              <Button
                ref={cancelRef}
                variant="secondary"
                onClick={handleCancel}
                disabled={pending}
              >
                {cancelLabel}
              </Button>
              <Button
                ref={confirmRef}
                variant={confirmVariant}
                onClick={handleConfirm}
                disabled={pending}
                loading={pending}
              >
                {confirmLabel}
              </Button>
            </>
          ) : (
            <>
              <Button
                ref={confirmRef}
                variant={confirmVariant}
                onClick={handleConfirm}
                disabled={pending}
                loading={pending}
              >
                {confirmLabel}
              </Button>
              <Button
                ref={cancelRef}
                variant="secondary"
                onClick={handleCancel}
                disabled={pending}
              >
                {cancelLabel}
              </Button>
            </>
          )}
        </div>
      </div>
    </div>
  )
}
