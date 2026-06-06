import { request } from '@/api/client'

export function getListActivity(listId) {
  return request(`/lists/${listId}/activity`)
}
