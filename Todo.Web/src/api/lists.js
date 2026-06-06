import { request } from '@/api/client'

export function getListById(id) {
  return request(`/lists/${id}`)
}

export function getLists() {
  return request('/lists')
}

export function createList(body) {
  return request('/lists', {
    method: 'POST',
    body: JSON.stringify(body),
  })
}

export function updateList(id, body) {
  return request(`/lists/${id}`, {
    method: 'PATCH',
    body: JSON.stringify(body),
  })
}

export function deleteList(id) {
  return request(`/lists/${id}`, {
    method: 'DELETE',
  })
}
