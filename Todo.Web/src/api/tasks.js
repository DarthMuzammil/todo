import { request } from '@/api/client'

export function getTasksByListId(id) {
  return request(`/lists/${id}/tasks`)
}

export function createTask(id, body) {
  return request(`/lists/${id}/tasks`, {
    method: 'POST',
    body: JSON.stringify(body),
  })
}

export function updateTaskStatus(listId, taskId, body) {
  return request(`/lists/${listId}/tasks/${taskId}/status`, {
    method: 'PATCH',
    body: JSON.stringify(body),
  })
}

export function deleteTask(listId, taskId) {
  return request(`/lists/${listId}/tasks/${taskId}`, {
    method: 'DELETE',
  })
}
