import { request } from '@/api/client'

export function getNotifications(limit = 20) {
  return request(`/notifications?limit=${limit}`)
}

export function getUnreadNotificationCount() {
  return request('/notifications/unread-count')
}

export function markNotificationRead(notificationId) {
  return request(`/notifications/${notificationId}/read`, {
    method: 'PATCH',
  })
}

export function markAllNotificationsRead() {
  return request('/notifications/mark-all-read', {
    method: 'POST',
  })
}
