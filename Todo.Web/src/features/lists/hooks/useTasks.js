import { getTasksByListId } from '@/api/tasks'
import { useAsync } from '@/shared/hooks/useAsync'

export function useTasks(listId) {
  const { data, status, error, refetch } = useAsync(listId, getTasksByListId, {
    initialData: [],
  })

  return { tasks: data ?? [], status, error, refetch }
}
