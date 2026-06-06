/**
 * SidebarNavItem — navigation link for the lists sidebar.
 */
import { NavLink } from 'react-router-dom'
import './SidebarNavItem.css'

/**
 * @param {object} props
 * @param {string} props.href
 * @param {string} props.label
 * @param {boolean} [props.end]
 * @param {import('react').ComponentType<{size?: number, className?: string}>} [props.icon]
 * @param {string} [props.swatchColor]
 */
export default function SidebarNavItem({
  href,
  label,
  end = false,
  icon: Icon,
  swatchColor,
}) {
  return (
    <NavLink
      to={href}
      className={({ isActive }) =>
        `sidebar-nav-item${isActive ? ' sidebar-nav-item--active' : ''}`
      }
      end={end}
    >
      {swatchColor ? (
        <span
          className="sidebar-nav-item__swatch"
          style={{ backgroundColor: swatchColor }}
          aria-hidden="true"
        />
      ) : (
        Icon && (
          <Icon className="sidebar-nav-item__icon" aria-hidden="true" size={20} />
        )
      )}
      <span className="sidebar-nav-item__label">{label}</span>
    </NavLink>
  )
}
