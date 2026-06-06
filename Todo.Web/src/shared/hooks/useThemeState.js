import { useCallback, useSyncExternalStore } from 'react'
import {
  applyTheme,
  getInitialTheme,
  toggleThemeValue,
} from './useTheme.js'

function subscribe(onStoreChange) {
  window.addEventListener('theme-change', onStoreChange)
  const media = window.matchMedia('(prefers-color-scheme: dark)')
  media.addEventListener('change', onStoreChange)
  return () => {
    window.removeEventListener('theme-change', onStoreChange)
    media.removeEventListener('change', onStoreChange)
  }
}

function getSnapshot() {
  return (
    document.documentElement.getAttribute('data-theme') ?? getInitialTheme()
  )
}

function getServerSnapshot() {
  return 'light'
}

export function useTheme() {
  const theme = useSyncExternalStore(subscribe, getSnapshot, getServerSnapshot)

  const setTheme = useCallback((next) => {
    applyTheme(next)
    window.dispatchEvent(new Event('theme-change'))
  }, [])

  const toggle = useCallback(() => {
    const next = toggleThemeValue(theme === 'dark' ? 'dark' : 'light')
    applyTheme(next)
    window.dispatchEvent(new Event('theme-change'))
    return next
  }, [theme])

  return { theme, setTheme, toggle }
}
