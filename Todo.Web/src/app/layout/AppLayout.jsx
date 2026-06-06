import { useEffect, useId, useState } from 'react'
import { Link } from 'react-router-dom'
import './AppLayout.css'
import { Menu, Settings, X } from 'lucide-react'
import { Outlet } from 'react-router-dom'
import { useAuth } from '@/features/auth'
import { ShareDialog, WorkspaceSwitcher } from '@/features/workspaces'
import { NotificationBell } from '@/features/notifications'
import { Button, ThemeToggle } from '@/shared/components/ui'
import { BlurFade, DotPattern } from '@/shared/components/magic-ui'
import ListsSidebar from '@/features/lists/components/ListsSidebar'

export function AppLayout() {
  const { user, logout } = useAuth()
  const [sidebarOpen, setSidebarOpen] = useState(false)
  const sidebarId = useId()

  useEffect(() => {
    if (!sidebarOpen) {
      return undefined
    }

    function handleKeyDown(event) {
      if (event.key === 'Escape') {
        setSidebarOpen(false)
      }
    }

    window.addEventListener('keydown', handleKeyDown)
    return () => window.removeEventListener('keydown', handleKeyDown)
  }, [sidebarOpen])

  function closeSidebar() {
    setSidebarOpen(false)
  }

  return (
    <div className="layout">
      <ShareDialog />

      <div className="layout__ambient" aria-hidden="true">
        <div className="layout__ambient-gradient" />
        <DotPattern className="layout__ambient-dots" width={24} height={24} cr={0.6} />
      </div>

      <a className="layout__skip-link" href="#main-content">
        Skip to main content
      </a>

      <header className="layout__header">
        <div className="layout__brand">
          <Button
            type="button"
            variant="ghost"
            className="layout__menu-button"
            aria-expanded={sidebarOpen}
            aria-controls={sidebarId}
            onClick={() => setSidebarOpen((open) => !open)}
          >
            {sidebarOpen ? (
              <X aria-hidden="true" size={20} strokeWidth={2} />
            ) : (
              <Menu aria-hidden="true" size={20} strokeWidth={2} />
            )}
            <span className="layout__menu-label">
              {sidebarOpen ? 'Close lists menu' : 'Open lists menu'}
            </span>
          </Button>
          <div className="layout__brand-mark">
            <span className="layout__brand-icon" aria-hidden="true">
              T
            </span>
            <h1>Todo</h1>
          </div>
        </div>

        <div className="layout__header-center">
          <WorkspaceSwitcher />
        </div>

        <div className="layout__header-actions">
          {user?.name && (
            <span className="layout__user" aria-label={`Signed in as ${user.name}`}>
              {user.name}
            </span>
          )}
          <ThemeToggle />
          <NotificationBell />
          <Link to="/settings" className="layout__settings-link">
            <Settings aria-hidden="true" size={18} strokeWidth={2} />
            <span className="layout__settings-label">Settings</span>
          </Link>
          <Button type="button" variant="ghost" onClick={logout}>
            Sign out
          </Button>
        </div>
      </header>

      {sidebarOpen && (
        <button
          type="button"
          className="layout__backdrop"
          aria-label="Close lists menu"
          onClick={closeSidebar}
        />
      )}

      <div className="layout__body">
        <aside
          id={sidebarId}
          className={`layout__sidebar${sidebarOpen ? ' layout__sidebar--open' : ''}`}
          aria-label="Lists navigation"
        >
          <ListsSidebar onNavigate={closeSidebar} />
        </aside>
        <main id="main-content" className="layout__main" tabIndex={-1}>
          <BlurFade inView>
            <Outlet />
          </BlurFade>
        </main>
      </div>
    </div>
  )
}
