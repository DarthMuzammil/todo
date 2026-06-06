import { useLocation } from 'react-router-dom'
import { getLists } from '@/api/lists'
import { useWorkspace } from '@/features/workspaces/hooks/useWorkspace'
import { useAsync } from '@/shared/hooks/useAsync'

function fetchLists() {
  return getLists()
}

export function useLists() {
  const location = useLocation()
  const { selectedWorkspaceId } = useWorkspace()
  const cacheKey = `${location.pathname}:${selectedWorkspaceId ?? 'none'}`
  const { data, status, error, refetch } = useAsync(cacheKey, fetchLists)

  const lists = (data ?? []).filter(
    (list) => !selectedWorkspaceId || list.workspaceId === selectedWorkspaceId,
  )

  return { lists, status, error, refetch }
}
