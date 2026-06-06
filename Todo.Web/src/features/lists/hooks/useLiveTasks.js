import { useMemo } from 'react'
import { useTasks } from './useTasks'
import { useListSync } from './useListSync'

export function useLiveTasks(listId) {
  const { tasks, status, error, refetch } = useTasks(listId)

  const syncHandlers = useMemo(
    () => ({
      onTaskCreated: () => refetch(),
      onTaskUpdated: () => refetch(),
      onTaskDeleted: () => refetch(),
      onListUpdated: () => refetch(),
      onListDeleted: () => refetch(),
    }),
    [refetch],
  )

  const { connectionState } = useListSync(
    status === 'success' ? listId : null,
    syncHandlers,
  )

  return {
    tasks,
    status,
    error,
    refetch,
    connectionState,
  }
}
