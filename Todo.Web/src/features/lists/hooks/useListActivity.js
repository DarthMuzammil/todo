import { useAsync } from '@/shared/hooks/useAsync'
import { getListActivity } from '@/api/activity'

export function useListActivity(listId, refreshVersion = 0) {
  return useAsync(
    listId ? `activity:${listId}:${refreshVersion}` : null,
    () => getListActivity(listId),
    {
      initialData: [],
    },
  )
}
