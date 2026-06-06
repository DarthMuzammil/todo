import { request } from '@/api/client'
import type { TodoTask, CreateTaskRequest } from '@/api/types'

export function getTasksByListId(id: string) {
  return request<TodoTask[]>(`/lists/${id}/tasks`)
}

export function createTask(id: string, body: CreateTaskRequest) {
  return request<TodoTask>(`/lists/${id}/tasks`, {
    method: 'POST',
    body: JSON.stringify(body),
  })
}

export function updateTaskStatus(
  listId: string,
  taskId: string,
  body: CreateTaskRequest,
) {
  return request<void>(`/lists/${listId}/tasks/${taskId}`, {
    method: 'PATCH',
    body: JSON.stringify(body),
  })
}

export function deleteTask(listId: string, taskId: string) {
  return request<void>(`/lists/${listId}/tasks/${taskId}`, {
    method: 'DELETE',
  })
}
