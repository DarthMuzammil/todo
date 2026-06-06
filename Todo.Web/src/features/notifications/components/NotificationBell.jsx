import { useCallback, useEffect, useId, useRef, useState } from 'react'
import { Link } from 'react-router-dom'
import { Bell } from 'lucide-react'
import {
  getNotifications,
  markAllNotificationsRead,
  markNotificationRead,
} from '@/api/notifications'
import { useAsync } from '@/shared/hooks/useAsync'
import { Button } from '@/shared/components/ui'
import { getErrorMessage } from '@/shared/utils/getErrorMessage'
import './NotificationBell.css'

const EMPTY_SUMMARY = { unreadCount: 0, items: [] }

export default function NotificationBell() {
  const menuId = useId()
  const panelRef = useRef(null)
  const [open, setOpen] = useState(false)
  const [actionError, setActionError] = useState(null)
  const [localSummary, setLocalSummary] = useState(null)

  const {
    data,
    status,
    error,
    refetch,
  } = useAsync('notifications-summary', getNotifications, {
    initialData: EMPTY_SUMMARY,
  })

  const summary = localSummary ?? data ?? EMPTY_SUMMARY
  const unreadCount = summary.unreadCount ?? 0
  const items = summary.items ?? []

  const loadNotifications = useCallback(async () => {
    setActionError(null)
    setLocalSummary(null)
    await refetch()
  }, [refetch])

  useEffect(() => {
    if (!open) {
      return undefined
    }

    function handlePointerDown(event) {
      if (panelRef.current && !panelRef.current.contains(event.target)) {
        setOpen(false)
      }
    }

    function handleKeyDown(event) {
      if (event.key === 'Escape') {
        setOpen(false)
      }
    }

    document.addEventListener('mousedown', handlePointerDown)
    window.addEventListener('keydown', handleKeyDown)
    return () => {
      document.removeEventListener('mousedown', handlePointerDown)
      window.removeEventListener('keydown', handleKeyDown)
    }
  }, [open])

  async function handleOpen() {
    const nextOpen = !open
    setOpen(nextOpen)
    if (nextOpen) {
      await loadNotifications()
    }
  }

  async function handleNotificationClick(notification) {
    if (!notification.isRead) {
      try {
        await markNotificationRead(notification.id)
        setLocalSummary({
          unreadCount: Math.max(0, unreadCount - 1),
          items: items.map((item) =>
            item.id === notification.id ? { ...item, isRead: true } : item,
          ),
        })
      } catch {
        // Navigation still helps even if mark-read fails.
      }
    }

    setOpen(false)
  }

  async function handleMarkAllRead() {
    try {
      await markAllNotificationsRead()
      setLocalSummary({
        unreadCount: 0,
        items: items.map((item) => ({ ...item, isRead: true })),
      })
    } catch (err) {
      setActionError(getErrorMessage(err, 'Failed to mark notifications read'))
    }
  }

  const loading = status === 'loading'
  const loadError = error ? getErrorMessage(error, 'Failed to load notifications') : null

  return (
    <div className="notification-bell" ref={panelRef}>
      <Button
        type="button"
        variant="ghost"
        className="notification-bell__trigger"
        aria-expanded={open}
        aria-controls={menuId}
        aria-label={
          unreadCount > 0
            ? `Notifications, ${unreadCount} unread`
            : 'Notifications'
        }
        onClick={handleOpen}
      >
        <Bell aria-hidden="true" size={18} strokeWidth={2} />
        {unreadCount > 0 && (
          <span className="notification-bell__badge" aria-hidden="true">
            {unreadCount > 9 ? '9+' : unreadCount}
          </span>
        )}
      </Button>

      {open && (
        <div id={menuId} className="notification-bell__panel" role="menu">
          <div className="notification-bell__header">
            <h2 className="notification-bell__title">Notifications</h2>
            {unreadCount > 0 && (
              <Button type="button" variant="ghost" size="sm" onClick={handleMarkAllRead}>
                Mark all read
              </Button>
            )}
          </div>

          {loading && <p className="notification-bell__status">Loading…</p>}
          {(loadError || actionError) && (
            <p className="notification-bell__error">{loadError ?? actionError}</p>
          )}

          {!loading && !loadError && items.length === 0 && (
            <p className="notification-bell__status">You&apos;re all caught up.</p>
          )}

          {!loading && !loadError && items.length > 0 && (
            <ul className="notification-bell__list">
              {items.map((notification) => (
                <li key={notification.id}>
                  <Link
                    to={`/lists/${notification.listId}`}
                    className={`notification-bell__item${notification.isRead ? '' : ' notification-bell__item--unread'}`}
                    role="menuitem"
                    onClick={() => handleNotificationClick(notification)}
                  >
                    <span>{notification.message}</span>
                    <time dateTime={notification.createdAt}>
                      {new Date(notification.createdAt).toLocaleString(undefined, {
                        month: 'short',
                        day: 'numeric',
                        hour: 'numeric',
                        minute: '2-digit',
                      })}
                    </time>
                  </Link>
                </li>
              ))}
            </ul>
          )}
        </div>
      )}
    </div>
  )
}
