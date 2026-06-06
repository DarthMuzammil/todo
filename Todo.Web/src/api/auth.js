import { request } from '@/api/client'

export function login(body) {
  return request('/auth/login', {
    method: 'POST',
    body: JSON.stringify(body),
  })
}

export function register(body) {
  return request('/auth/register', {
    method: 'POST',
    body: JSON.stringify(body),
  })
}

export function logout() {
  return request('/auth/logout', {
    method: 'POST',
    skipAuthRefresh: true,
  })
}

export function getCurrentUser() {
  return request('/users/me')
}
