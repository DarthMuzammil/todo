import { MemoryRouter } from 'react-router-dom'
import { WorkspaceProvider } from '@/features/workspaces'

export function WorkspaceTestProviders({ children, initialEntries = ['/'] }) {
  return (
    <MemoryRouter initialEntries={initialEntries}>
      <WorkspaceProvider>{children}</WorkspaceProvider>
    </MemoryRouter>
  )
}
