import { Moon, Sun } from 'lucide-react'
import { useTheme } from '@/shared/hooks/useThemeState'
import Button from './Button'

export default function ThemeToggle() {
  const { theme, toggle } = useTheme()
  const isDark = theme === 'dark'

  return (
    <Button
      variant="ghost"
      size="sm"
      onClick={toggle}
      aria-label={isDark ? 'Switch to light mode' : 'Switch to dark mode'}
      className="theme-toggle"
    >
      {isDark ? (
        <Sun aria-hidden="true" size={20} strokeWidth={2} />
      ) : (
        <Moon aria-hidden="true" size={20} strokeWidth={2} />
      )}
    </Button>
  )
}
