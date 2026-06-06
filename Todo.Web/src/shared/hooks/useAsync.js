import { useCallback, useEffect, useRef, useState } from 'react'

/**
 * Small async-data hook shared by feature loaders.
 * Re-runs when `key` changes or when `refetch()` is called.
 */
export function useAsync(key, fetcher, { initialData = null } = {}) {
  const [data, setData] = useState(initialData)
  const [status, setStatus] = useState('loading')
  const [error, setError] = useState(null)
  const [reloadCount, setReloadCount] = useState(0)
  const fetcherRef = useRef(fetcher)
  const enabled = key != null && key !== ''

  fetcherRef.current = fetcher

  const refetch = useCallback(() => {
    setReloadCount((count) => count + 1)
  }, [])

  useEffect(() => {
    if (!enabled) {
      return
    }

    let active = true

    async function load() {
      setStatus('loading')
      setError(null)

      try {
        const result = await fetcherRef.current(key)
        if (!active) return
        setData(result)
        setStatus('success')
      } catch (err) {
        if (!active) return
        setError(err)
        setStatus('error')
      }
    }

    load()

    return () => {
      active = false
    }
  }, [key, reloadCount, enabled])

  return { data, status, error, refetch }
}
