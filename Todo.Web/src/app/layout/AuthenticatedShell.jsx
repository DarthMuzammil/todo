import { WorkspaceProvider } from '@/features/workspaces'
import { Outlet } from 'react-router-dom'

export function AuthenticatedShell() {
  return (
    <WorkspaceProvider>
      <Outlet />
    </WorkspaceProvider>
  )
}
