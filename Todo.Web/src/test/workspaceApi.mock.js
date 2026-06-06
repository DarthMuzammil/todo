import { vi } from 'vitest'

vi.mock('@/api/workspaces', () => ({
  getWorkspaces: vi.fn().mockResolvedValue([
    {
      id: 'ws-personal',
      name: 'Personal',
      isPersonal: true,
      currentUserRole: 0,
    },
  ]),
  createWorkspace: vi.fn(),
  getWorkspaceMembers: vi.fn().mockResolvedValue([]),
  getWorkspaceInvites: vi.fn().mockResolvedValue([]),
  sendWorkspaceInvite: vi.fn(),
  removeWorkspaceMember: vi.fn(),
  acceptInvite: vi.fn(),
  declineInvite: vi.fn(),
}))
