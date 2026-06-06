import { useCallback, useEffect, useId, useRef, useState } from 'react'
import { Copy, UserMinus, Users } from 'lucide-react'
import {
  getWorkspaceInvites,
  getWorkspaceMembers,
  removeWorkspaceMember,
  resendWorkspaceInvite,
  sendWorkspaceInvite,
} from '@/api/workspaces'
import { useWorkspace } from '@/features/workspaces/hooks/useWorkspace'
import { useCurrentWorkspaceRole } from '@/features/workspaces/hooks/useWorkspaceRole'
import {
  Alert,
  Badge,
  Button,
  Input,
  Select,
} from '@/shared/components/ui'
import { useFocusTrap } from '@/shared/hooks/useFocusTrap'
import {
  getWorkspaceRoleLabel,
  isWorkspaceOwner,
  WORKSPACE_ROLE_OPTIONS,
  WORKSPACE_ROLES,
} from '@/shared/constants/workspaceEnums'
import { getErrorMessage } from '@/shared/utils/getErrorMessage'
import './ShareDialog.css'

function buildInviteLink(token) {
  return `${window.location.origin}/invites/${encodeURIComponent(token)}`
}

export default function ShareDialog() {
  const { currentWorkspace, shareOpen, closeShare, refetchWorkspaces } = useWorkspace()
  const { isOwner } = useCurrentWorkspaceRole()
  const panelRef = useRef(null)
  const titleId = useId()

  const [members, setMembers] = useState([])
  const [invites, setInvites] = useState([])
  const [loading, setLoading] = useState(false)
  const [loadError, setLoadError] = useState(null)

  const [email, setEmail] = useState('')
  const [role, setRole] = useState(WORKSPACE_ROLES.Editor)
  const [inviteError, setInviteError] = useState(null)
  const [inviting, setInviting] = useState(false)
  const [inviteLink, setInviteLink] = useState(null)
  const [copyMessage, setCopyMessage] = useState(null)
  const [removingUserId, setRemovingUserId] = useState(null)
  const [resendingInviteId, setResendingInviteId] = useState(null)

  const handleClose = useCallback(() => {
    setEmail('')
    setInviteError(null)
    setInviteLink(null)
    setCopyMessage(null)
    closeShare()
  }, [closeShare])

  useFocusTrap(panelRef, shareOpen, handleClose)

  useEffect(() => {
    if (!shareOpen || !currentWorkspace) {
      return undefined
    }

    let cancelled = false

    async function load() {
      setLoading(true)
      setLoadError(null)

      try {
        const [memberData, inviteData] = await Promise.all([
          getWorkspaceMembers(currentWorkspace.id),
          isOwner ? getWorkspaceInvites(currentWorkspace.id) : Promise.resolve([]),
        ])

        if (!cancelled) {
          setMembers(memberData)
          setInvites(inviteData)
        }
      } catch (err) {
        if (!cancelled) {
          setLoadError(getErrorMessage(err, 'Failed to load sharing details'))
        }
      } finally {
        if (!cancelled) {
          setLoading(false)
        }
      }
    }

    load()

    return () => {
      cancelled = true
    }
  }, [shareOpen, currentWorkspace, isOwner])

  useEffect(() => {
    if (!shareOpen) {
      return undefined
    }

    const previousOverflow = document.body.style.overflow
    document.body.style.overflow = 'hidden'
    return () => {
      document.body.style.overflow = previousOverflow
    }
  }, [shareOpen])

  if (!shareOpen || !currentWorkspace) {
    return null
  }

  async function handleInvite(e) {
    e.preventDefault()
    setInviteError(null)
    setInviteLink(null)
    setCopyMessage(null)
    setInviting(true)

    try {
      const result = await sendWorkspaceInvite(currentWorkspace.id, {
        email: email.trim(),
        role: Number(role),
      })
      setEmail('')
      setInviteLink(buildInviteLink(result.token))
      const inviteData = await getWorkspaceInvites(currentWorkspace.id)
      setInvites(inviteData)
    } catch (err) {
      setInviteError(getErrorMessage(err, 'Failed to send invite'))
    } finally {
      setInviting(false)
    }
  }

  async function handleCopyLink() {
    if (!inviteLink) {
      return
    }

    try {
      await navigator.clipboard.writeText(inviteLink)
      setCopyMessage('Invite link copied to clipboard.')
    } catch {
      setCopyMessage('Could not copy automatically — select and copy the link below.')
    }
  }

  async function handleResendInvite(inviteId) {
    setResendingInviteId(inviteId)
    setInviteError(null)
    setCopyMessage(null)

    try {
      const result = await resendWorkspaceInvite(currentWorkspace.id, inviteId)
      setInviteLink(buildInviteLink(result.token))
      setCopyMessage('New invite link generated — copy it below.')
    } catch (err) {
      setInviteError(getErrorMessage(err, 'Failed to regenerate invite link'))
    } finally {
      setResendingInviteId(null)
    }
  }

  async function handleRemoveMember(memberUserId) {
    setRemovingUserId(memberUserId)

    try {
      await removeWorkspaceMember(currentWorkspace.id, memberUserId)
      setMembers((current) => current.filter((member) => member.userId !== memberUserId))
      await refetchWorkspaces()
    } catch (err) {
      setLoadError(getErrorMessage(err, 'Failed to remove member'))
    } finally {
      setRemovingUserId(null)
    }
  }

  const isPersonal = currentWorkspace.isPersonal

  return (
    <div className="share-dialog">
      <button
        type="button"
        className="share-dialog__backdrop"
        aria-label="Close share dialog"
        onClick={handleClose}
      />
      <div
        ref={panelRef}
        className="share-dialog__panel"
        role="dialog"
        aria-modal="true"
        aria-labelledby={titleId}
      >
        <header className="share-dialog__header">
          <Users aria-hidden="true" size={20} strokeWidth={2} />
          <div>
            <h2 id={titleId} className="share-dialog__title">
              Share {currentWorkspace.name}
            </h2>
            <p className="share-dialog__subtitle">
              People invited to this workspace can see all lists inside it.
            </p>
          </div>
        </header>

        {isPersonal && (
          <Alert
            variant="info"
            message="Personal workspaces are private. Create a shared workspace from the header menu to collaborate."
          />
        )}

        {loadError && <Alert variant="error" compact message={loadError} />}

        {!isPersonal && isOwner && (
          <form className="share-dialog__invite-form" onSubmit={handleInvite} noValidate>
            <Input
              id="share-email"
              name="email"
              type="email"
              label="Invite by email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              disabled={inviting}
              hint="They must register or log in with this exact email to accept."
            />
            <Select
              id="share-role"
              label="Role"
              value={role}
              onChange={(e) => setRole(Number(e.target.value))}
              disabled={inviting}
            >
              {WORKSPACE_ROLE_OPTIONS.map((option) => (
                <option key={option.value} value={option.value}>
                  {option.label}
                </option>
              ))}
            </Select>
            <Button type="submit" variant="primary" disabled={inviting} loading={inviting}>
              Send invite
            </Button>
          </form>
        )}

        {inviteError && <Alert variant="error" compact message={inviteError} />}

        {inviteLink && (
          <div className="share-dialog__link-box">
            <p className="share-dialog__link-label">Share this link with your invitee:</p>
            <code className="share-dialog__link">{inviteLink}</code>
            <Button type="button" variant="secondary" size="sm" onClick={handleCopyLink}>
              <Copy aria-hidden="true" size={16} strokeWidth={2} />
              Copy link
            </Button>
            {copyMessage && <p className="share-dialog__copy-message">{copyMessage}</p>}
          </div>
        )}

        <section className="share-dialog__section" aria-labelledby="share-members-heading">
          <h3 id="share-members-heading" className="share-dialog__section-title">
            Members
          </h3>
          {loading && <p className="share-dialog__muted">Loading…</p>}
          {!loading && members.length === 0 && (
            <p className="share-dialog__muted">No members yet.</p>
          )}
          <ul className="share-dialog__member-list">
            {members.map((member) => (
              <li key={member.userId} className="share-dialog__member">
                <div className="share-dialog__member-copy">
                  <span className="share-dialog__member-name">{member.name}</span>
                  <span className="share-dialog__member-email">{member.email}</span>
                </div>
                <Badge variant="neutral">{getWorkspaceRoleLabel(member.role)}</Badge>
                {isOwner && !isWorkspaceOwner(member.role) && (
                  <Button
                    type="button"
                    variant="ghost"
                    size="sm"
                    disabled={removingUserId === member.userId}
                    aria-label={`Remove ${member.name}`}
                    onClick={() => handleRemoveMember(member.userId)}
                  >
                    <UserMinus aria-hidden="true" size={16} strokeWidth={2} />
                  </Button>
                )}
              </li>
            ))}
          </ul>
        </section>

        {!isPersonal && isOwner && invites.length > 0 && (
          <section className="share-dialog__section" aria-labelledby="share-pending-heading">
            <h3 id="share-pending-heading" className="share-dialog__section-title">
              Pending invites
            </h3>
            <ul className="share-dialog__member-list">
              {invites.map((invite) => (
                <li key={invite.id} className="share-dialog__member">
                  <div className="share-dialog__member-copy">
                    <span className="share-dialog__member-name">{invite.email}</span>
                    <span className="share-dialog__member-email">
                      Expires {new Date(invite.expiresAt).toLocaleDateString()}
                    </span>
                  </div>
                  <Badge variant="default">{getWorkspaceRoleLabel(invite.role)}</Badge>
                  <Button
                    type="button"
                    variant="ghost"
                    size="sm"
                    aria-label={`Copy invite link for ${invite.email}`}
                    disabled={resendingInviteId === invite.id}
                    loading={resendingInviteId === invite.id}
                    onClick={() => handleResendInvite(invite.id)}
                  >
                    <Copy aria-hidden="true" size={16} strokeWidth={2} />
                    Copy link
                  </Button>
                </li>
              ))}
            </ul>
          </section>
        )}

        <div className="share-dialog__actions">
          <Button type="button" variant="secondary" onClick={handleClose}>
            Close
          </Button>
        </div>
      </div>
    </div>
  )
}
