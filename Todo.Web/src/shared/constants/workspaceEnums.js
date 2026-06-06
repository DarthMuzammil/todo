export const WORKSPACE_ROLES = {
  Owner: 0,
  Editor: 1,
  Viewer: 2,
}

export const WORKSPACE_ROLE_OPTIONS = [
  { value: WORKSPACE_ROLES.Editor, label: 'Editor — can add and edit tasks' },
  { value: WORKSPACE_ROLES.Viewer, label: 'Viewer — read only' },
]

export function getWorkspaceRoleLabel(role) {
  switch (role) {
    case WORKSPACE_ROLES.Owner:
      return 'Owner'
    case WORKSPACE_ROLES.Editor:
      return 'Editor'
    case WORKSPACE_ROLES.Viewer:
      return 'Viewer'
    default:
      return 'Member'
  }
}

export function canWriteWorkspaceRole(role) {
  return role === WORKSPACE_ROLES.Owner || role === WORKSPACE_ROLES.Editor
}

export function isWorkspaceOwner(role) {
  return role === WORKSPACE_ROLES.Owner
}
