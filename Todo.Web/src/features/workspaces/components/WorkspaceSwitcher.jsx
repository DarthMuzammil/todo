import { useState } from 'react'
import { Plus, Share2 } from 'lucide-react'
import { Button, Input, Select } from '@/shared/components/ui'
import { isWorkspaceOwner } from '@/shared/constants/workspaceEnums'
import { getErrorMessage } from '@/shared/utils/getErrorMessage'
import { useWorkspace } from '../hooks/useWorkspace'
import './WorkspaceSwitcher.css'

export default function WorkspaceSwitcher() {
  const {
    workspaces,
    status,
    currentWorkspace,
    selectedWorkspaceId,
    setSelectedWorkspaceId,
    createSharedWorkspace,
    openShare,
  } = useWorkspace()

  const [creating, setCreating] = useState(false)
  const [newName, setNewName] = useState('')
  const [createError, setCreateError] = useState(null)
  const [submitting, setSubmitting] = useState(false)

  async function handleCreateWorkspace(e) {
    e.preventDefault()
    if (!newName.trim()) {
      return
    }

    setSubmitting(true)
    setCreateError(null)

    try {
      await createSharedWorkspace(newName.trim())
      setNewName('')
      setCreating(false)
    } catch (err) {
      setCreateError(getErrorMessage(err, 'Failed to create workspace'))
    } finally {
      setSubmitting(false)
    }
  }

  const canShare =
    currentWorkspace &&
    !currentWorkspace.isPersonal &&
    isWorkspaceOwner(currentWorkspace.currentUserRole)

  return (
    <div className="workspace-switcher">
      <div className="workspace-switcher__row">
        <Select
          id="workspace-switcher"
          label="Workspace"
          className="workspace-switcher__select"
          value={selectedWorkspaceId ?? ''}
          disabled={status !== 'success' || workspaces.length === 0}
          onChange={(e) => setSelectedWorkspaceId(e.target.value)}
        >
          {workspaces.map((workspace) => (
            <option key={workspace.id} value={workspace.id}>
              {workspace.isPersonal ? `${workspace.name} (Personal)` : workspace.name}
            </option>
          ))}
        </Select>
      </div>

      <div className="workspace-switcher__actions">
        {canShare && (
          <Button type="button" variant="secondary" size="sm" onClick={openShare}>
            <Share2 aria-hidden="true" size={16} strokeWidth={2} />
            Share
          </Button>
        )}
        <Button
          type="button"
          variant="ghost"
          size="sm"
          onClick={() => setCreating((open) => !open)}
        >
          <Plus aria-hidden="true" size={16} strokeWidth={2} />
          New shared
        </Button>
      </div>

      {creating && (
        <form className="workspace-switcher__create" onSubmit={handleCreateWorkspace}>
          <Input
            id="new-workspace-name"
            label="Shared workspace name"
            value={newName}
            onChange={(e) => setNewName(e.target.value)}
            disabled={submitting}
            hint="Example: Family, Roommates, Team project"
          />
          {createError && <p className="workspace-switcher__error">{createError}</p>}
          <Button type="submit" variant="primary" size="sm" disabled={submitting} loading={submitting}>
            Create
          </Button>
        </form>
      )}
    </div>
  )
}
