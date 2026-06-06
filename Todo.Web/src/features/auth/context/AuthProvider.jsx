import { useCallback, useEffect, useMemo, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import * as authApi from '@/api/auth'
import { setUnauthorizedHandler } from '@/api/unauthorized'
import {
  clearAccessToken,
  getAccessToken,
  setAccessToken,
} from '@/shared/auth/tokenStorage'
import { AuthContext } from './AuthContext'

export function AuthProvider({ children }) {
  const navigate = useNavigate()
  const [user, setUser] = useState(null)
  const [status, setStatus] = useState('loading')

  useEffect(() => {
    let cancelled = false

    async function loadSession() {
      const token = getAccessToken()
      if (!token) {
        if (!cancelled) {
          setUser(null)
          setStatus('anonymous')
        }
        return
      }

      try {
        const profile = await authApi.getCurrentUser()
        if (!cancelled) {
          setUser(profile)
          setStatus('authenticated')
        }
      } catch {
        clearAccessToken()
        if (!cancelled) {
          setUser(null)
          setStatus('anonymous')
        }
      }
    }

    void loadSession()

    return () => {
      cancelled = true
    }
  }, [])

  useEffect(() => {
    setUnauthorizedHandler(() => {
      clearAccessToken()
      setUser(null)
      setStatus('anonymous')
      navigate('/login', {
        replace: true,
        state: { message: 'Your session expired. Please sign in again.' },
      })
    })

    return () => setUnauthorizedHandler(null)
  }, [navigate])

  const completeAuth = useCallback(async (authResponse) => {
    setAccessToken(authResponse.accessToken)
    setUser({
      id: authResponse.userId,
      email: authResponse.email,
      name: authResponse.name,
    })
    setStatus('authenticated')
  }, [])

  const login = useCallback(
    async (credentials) => {
      const response = await authApi.login(credentials)
      await completeAuth(response)
      return response
    },
    [completeAuth],
  )

  const register = useCallback(
    async (payload) => {
      const response = await authApi.register(payload)
      await completeAuth(response)
      return response
    },
    [completeAuth],
  )

  const logout = useCallback(async () => {
    try {
      await authApi.logout()
    } catch {
      // Clear local session even if the server call fails.
    }

    clearAccessToken()
    setUser(null)
    setStatus('anonymous')
    navigate('/login', { replace: true })
  }, [navigate])

  const updateUser = useCallback((profile) => {
    setUser({
      id: profile.id,
      email: profile.email,
      name: profile.name,
    })
  }, [])

  const value = useMemo(
    () => ({
      user,
      status,
      isAuthenticated: status === 'authenticated',
      isLoading: status === 'loading',
      login,
      register,
      logout,
      updateUser,
    }),
    [user, status, login, register, logout, updateUser],
  )

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>
}
