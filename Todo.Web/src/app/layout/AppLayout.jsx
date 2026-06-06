import './AppLayout.css'
import { Outlet } from 'react-router-dom'
import { ThemeToggle } from '@/shared/components/ui'
import ListsSidebar from '@/features/lists/components/ListsSidebar'

export function AppLayout() {
  return (
    <div className="layout">
      <a className="layout__skip-link" href="#main-content">
        Skip to main content
      </a>
      <header className="layout__header">
        <div className="layout__brand">
          <h1>Todo</h1>
        </div>
        <div className="layout__header-actions">
          <ThemeToggle />
        </div>
      </header>
      <div className="layout__body">
        <aside className="layout__sidebar" aria-label="Lists navigation">
          <ListsSidebar />
        </aside>
        <main id="main-content" className="layout__main" tabIndex={-1}>
          <Outlet />
        </main>
      </div>
    </div>
  )
}
