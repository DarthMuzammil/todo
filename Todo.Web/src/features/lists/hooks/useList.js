import { getListById } from '@/api/lists'
import { useAsync } from '@/shared/hooks/useAsync'

export function useList(listId) {
  const { data: list, status, error, refetch } = useAsync(listId, getListById)

  return { list, status, error, refetch }
}
