const STORAGE_KEY = 'theme'

/** @returns {'light' | 'dark'} */
export function getInitialTheme() {
  if (typeof window === 'undefined') {
    return 'light'
  }

  const stored = localStorage.getItem(STORAGE_KEY)
  if (stored === 'dark' || stored === 'light') {
    return stored
  }

  return window.matchMedia('(prefers-color-scheme: dark)').matches
    ? 'dark'
    : 'light'
}

/** @param {'light' | 'dark'} theme */
export function applyTheme(theme) {
  document.documentElement.setAttribute('data-theme', theme)
  localStorage.setItem(STORAGE_KEY, theme)
}

export function initTheme() {
  applyTheme(getInitialTheme())
}

/** @param {'light' | 'dark'} theme */
export function toggleThemeValue(theme) {
  return theme === 'dark' ? 'light' : 'dark'
}
