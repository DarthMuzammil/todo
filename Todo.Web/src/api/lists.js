import { request } from '@/api/client'

export function getListById(id) {
  return request(`/lists/${id}`)
}

export function getLists(ownerId) {
  return request(`/lists?ownerId=${ownerId}`)
}

export function createList(body) {
  return request('/lists', {
    method: 'POST',
    body: JSON.stringify(body),
  })
}
