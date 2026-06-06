import { request } from '@/api/client'
import type { TodoList, CreateListRequest } from '@/api/types'

export function getListById(id: string) {
  return request<TodoList>(`/lists/${id}`)
}

export function createList(body: CreateListRequest) {
    return request<TodoList>('/lists', {
      method: 'POST',
      body: JSON.stringify(body),
    })
  }