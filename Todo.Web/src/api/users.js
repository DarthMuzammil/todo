import { request } from '@/api/client'

export function updateProfile(body) {
  return request('/users/me', {
    method: 'PATCH',
    body: JSON.stringify(body),
  })
}

export function changePassword(body) {
  return request('/users/me/change-password', {
    method: 'POST',
    body: JSON.stringify(body),
  })
}

export function logoutAllSessions() {
  return request('/users/me/logout-all', {
    method: 'POST',
  })
}
