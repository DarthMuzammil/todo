import { request } from '@/api/client'

export function getWorkspaces() {
  return request('/workspaces')
}

export function createWorkspace(body) {
  return request('/workspaces', {
    method: 'POST',
    body: JSON.stringify(body),
  })
}

export function getWorkspaceMembers(workspaceId) {
  return request(`/workspaces/${workspaceId}/members`)
}

export function getWorkspaceInvites(workspaceId) {
  return request(`/workspaces/${workspaceId}/invites`)
}

export function sendWorkspaceInvite(workspaceId, body) {
  return request(`/workspaces/${workspaceId}/invites`, {
    method: 'POST',
    body: JSON.stringify(body),
  })
}

export function removeWorkspaceMember(workspaceId, memberUserId) {
  return request(`/workspaces/${workspaceId}/members/${memberUserId}`, {
    method: 'DELETE',
  })
}

export function resendWorkspaceInvite(workspaceId, inviteId) {
  return request(`/workspaces/${workspaceId}/invites/${inviteId}/resend`, {
    method: 'POST',
  })
}

export function acceptInvite(token) {
  return request(`/invites/${encodeURIComponent(token)}/accept`, {
    method: 'POST',
  })
}

export function declineInvite(token) {
  return request(`/invites/${encodeURIComponent(token)}/decline`, {
    method: 'POST',
  })
}
