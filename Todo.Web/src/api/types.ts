export type TaskStatus = 0 | 1 | 2 | 3

export type Priority = 0 | 1 | 2

export interface TodoList {
  id: string
  ownerId: string
  title: string
  color: string
  createdAt: string
  updatedAt: string
  isDeleted: boolean
}

export interface TodoTask {
  id: string
  title: string
  description: string
  status: TaskStatus
  priority: Priority
  dueDate: string | null
  assigneeId: string | null
  parentTaskId: string | null
  sortOrder: number
  createdAt: string
  updatedAt: string
  isDeleted: boolean
  deletedAt: string | null
  listId: string
}

export interface ApiError {
  error: string
}

export interface CreateListRequest {
  ownerId: string
  title: string
  color?: string | null
}

export interface CreateTaskRequest {
  title: string
  description: string
  status: TaskStatus
  priority: Priority
  dueDate: string | null
  assigneeId: string | null
  parentTaskId: string | null
}

export interface UpdateTaskStatusRequest {
  id: string
  status: TaskStatus
}