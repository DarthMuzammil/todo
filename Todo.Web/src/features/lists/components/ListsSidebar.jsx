import { Home, LayoutList } from 'lucide-react'
import { Alert, Skeleton, SidebarNavItem } from '@/shared/components/ui'
import { getErrorMessage } from '@/shared/utils/getErrorMessage'
import { useLists } from '@/features/lists/hooks/useLists'
import './ListsSidebar.css'

export default function ListsSidebar({ onNavigate }) {
  const { lists, status, error } = useLists()

  return (
    <nav className="lists-sidebar" aria-label="Your lists">
      <SidebarNavItem href="/" label="Home" icon={Home} end onClick={onNavigate} />

      {status === 'loading' && (
        <div className="lists-sidebar__loading" aria-busy="true">
          <Skeleton variant="rect" height="40px" />
          <Skeleton variant="rect" height="40px" />
        </div>
      )}

      {status === 'error' && (
        <Alert
          variant="error"
          compact
          message={getErrorMessage(error, 'Failed to load lists')}
        />
      )}

      {status === 'success' && lists.length === 0 && (
        <p className="lists-sidebar__empty">No lists yet</p>
      )}

      {status === 'success' && lists.length > 0 && (
        <ul className="lists-sidebar__list">
          {lists.map((list) => (
            <li key={list.id}>
              <SidebarNavItem
                href={`/lists/${list.id}`}
                label={list.title}
                icon={LayoutList}
                swatchColor={list.color}
                onClick={onNavigate}
              />
            </li>
          ))}
        </ul>
      )}
    </nav>
  )
}
