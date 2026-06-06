import { API_BASE_URL } from '@/shared/config/env'

export const LIST_SYNC_HUB_URL = import.meta.env.DEV
  ? '/hubs/lists'
  : `${API_BASE_URL}/hubs/lists`

export const SYNC_EVENTS = {
  TaskCreated: 'TaskCreated',
  TaskUpdated: 'TaskUpdated',
  TaskDeleted: 'TaskDeleted',
  ListUpdated: 'ListUpdated',
  ListDeleted: 'ListDeleted',
}

export function mapTaskDto(dto) {
  return {
    id: dto.id,
    listId: dto.listId,
    title: dto.title,
    description: dto.description ?? '',
    status: dto.status,
    priority: dto.priority,
    dueDate: dto.dueDate ?? null,
    sortOrder: dto.sortOrder ?? 0,
    createdAt: dto.createdAt,
    updatedAt: dto.updatedAt,
    version: dto.version ?? 1,
  }
}
