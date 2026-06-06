import { useMemo } from 'react'
import {
  canWriteWorkspaceRole,
  isWorkspaceOwner,
  WORKSPACE_ROLES,
} from '@/shared/constants/workspaceEnums'
import { useWorkspace } from './useWorkspace'

export function useWorkspaceRole(workspaceId) {
  const { workspaces } = useWorkspace()

  return useMemo(() => {
    const workspace = workspaces.find((item) => item.id === workspaceId)
    const role = workspace?.currentUserRole

    return {
      role,
      canWrite: role != null && canWriteWorkspaceRole(role),
      isViewer: role === WORKSPACE_ROLES.Viewer,
      isOwner: role != null && isWorkspaceOwner(role),
      workspaceName: workspace?.name ?? null,
    }
  }, [workspaces, workspaceId])
}

export function useCurrentWorkspaceRole() {
  const { currentWorkspace, status } = useWorkspace()
  const role = currentWorkspace?.currentUserRole

  return useMemo(
    () => ({
      role,
      canWrite:
        status === 'success' &&
        role != null &&
        canWriteWorkspaceRole(role),
      isViewer: role === WORKSPACE_ROLES.Viewer,
      isOwner: role != null && isWorkspaceOwner(role),
      workspaceName: currentWorkspace?.name ?? null,
    }),
    [currentWorkspace?.name, role, status],
  )
}
