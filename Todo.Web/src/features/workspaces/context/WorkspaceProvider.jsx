import { useCallback, useEffect, useMemo, useState } from 'react'
import { createWorkspace, getWorkspaces } from '@/api/workspaces'
import { useAsync } from '@/shared/hooks/useAsync'
import { WorkspaceContext } from './WorkspaceContext'

const STORAGE_KEY = 'todo:selectedWorkspaceId'

export function WorkspaceProvider({ children }) {
  const { data: workspaces, status, error, refetch: loadWorkspaces } = useAsync(
    'workspaces',
    getWorkspaces,
  )
  const [preferredWorkspaceId, setPreferredWorkspaceId] = useState(() =>
    sessionStorage.getItem(STORAGE_KEY),
  )
  const [shareOpen, setShareOpen] = useState(false)

  const workspaceList = workspaces ?? []

  const selectedWorkspaceId = useMemo(() => {
    if (
      preferredWorkspaceId &&
      workspaceList.some((workspace) => workspace.id === preferredWorkspaceId)
    ) {
      return preferredWorkspaceId
    }

    const personal = workspaceList.find((workspace) => workspace.isPersonal)
    return personal?.id ?? workspaceList[0]?.id ?? null
  }, [preferredWorkspaceId, workspaceList])

  useEffect(() => {
    if (status === 'success' && selectedWorkspaceId) {
      sessionStorage.setItem(STORAGE_KEY, selectedWorkspaceId)
    }
  }, [selectedWorkspaceId, status])

  const setSelectedWorkspaceId = useCallback((workspaceId) => {
    setPreferredWorkspaceId(workspaceId)
    if (workspaceId) {
      sessionStorage.setItem(STORAGE_KEY, workspaceId)
    } else {
      sessionStorage.removeItem(STORAGE_KEY)
    }
  }, [])

  const createSharedWorkspace = useCallback(
    async (name) => {
      const workspace = await createWorkspace({ name })
      await loadWorkspaces()
      setSelectedWorkspaceId(workspace.id)
      return workspace
    },
    [loadWorkspaces, setSelectedWorkspaceId],
  )

  const currentWorkspace = useMemo(
    () => workspaceList.find((workspace) => workspace.id === selectedWorkspaceId) ?? null,
    [workspaceList, selectedWorkspaceId],
  )

  const value = useMemo(
    () => ({
      workspaces: workspaceList,
      status,
      error,
      selectedWorkspaceId,
      currentWorkspace,
      setSelectedWorkspaceId,
      refetchWorkspaces: loadWorkspaces,
      createSharedWorkspace,
      shareOpen,
      openShare: () => setShareOpen(true),
      closeShare: () => setShareOpen(false),
    }),
    [
      workspaceList,
      status,
      error,
      selectedWorkspaceId,
      currentWorkspace,
      setSelectedWorkspaceId,
      loadWorkspaces,
      createSharedWorkspace,
      shareOpen,
    ],
  )

  return (
    <WorkspaceContext.Provider value={value}>{children}</WorkspaceContext.Provider>
  )
}
