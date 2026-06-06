import { useLocation } from 'react-router-dom'
import { getLists } from '@/api/lists'
import { DEV_OWNER_ID } from '@/shared/config/dev'
import { useAsync } from '@/shared/hooks/useAsync'

function fetchOwnerLists() {
  return getLists(DEV_OWNER_ID)
}

export function useLists() {
  const location = useLocation()
  const { data, status, error, refetch } = useAsync(
    `${DEV_OWNER_ID}:${location.pathname}`,
    fetchOwnerLists,
  )

  return {
    lists: data ?? [],
    status,
    error,
    refetch,
  }
}
