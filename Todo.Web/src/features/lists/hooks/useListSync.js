import { useEffect, useRef, useState } from 'react'
import * as signalR from '@microsoft/signalr'
import { getAccessToken } from '@/shared/auth/tokenStorage'
import { LIST_SYNC_HUB_URL, SYNC_EVENTS } from '@/shared/constants/syncEvents'

const RECONNECT_DELAYS_MS = [0, 2000, 5000, 10000, 30000]

/**
 * Subscribes to list-scoped realtime events over SignalR.
 * Returns connection state for UI indicators.
 */
export function useListSync(listId, handlers = {}) {
  const [connectionState, setConnectionState] = useState('idle')
  const handlersRef = useRef(handlers)

  useEffect(() => {
    handlersRef.current = handlers
  }, [handlers])

  useEffect(() => {
    if (!listId) {
      return undefined
    }

    let active = true
    let joined = false

    const connection = new signalR.HubConnectionBuilder()
      .withUrl(LIST_SYNC_HUB_URL, {
        accessTokenFactory: () => getAccessToken() ?? '',
      })
      .withAutomaticReconnect(RECONNECT_DELAYS_MS)
      .configureLogging(signalR.LogLevel.Warning)
      .build()

    function bindHandler(eventName, callbackName) {
      connection.on(eventName, (payload) => {
        handlersRef.current[callbackName]?.(payload)
      })
    }

    bindHandler(SYNC_EVENTS.TaskCreated, 'onTaskCreated')
    bindHandler(SYNC_EVENTS.TaskUpdated, 'onTaskUpdated')
    bindHandler(SYNC_EVENTS.TaskDeleted, 'onTaskDeleted')
    bindHandler(SYNC_EVENTS.ListUpdated, 'onListUpdated')
    bindHandler(SYNC_EVENTS.ListDeleted, 'onListDeleted')

    connection.onreconnecting(() => {
      if (active) setConnectionState('reconnecting')
    })

    connection.onreconnected(async () => {
      if (!active) return
      setConnectionState('live')
      if (joined) {
        await connection.invoke('JoinList', listId)
      }
    })

    connection.onclose(() => {
      if (active) setConnectionState('disconnected')
    })

    async function start() {
      setConnectionState('connecting')

      try {
        await connection.start()
        if (!active) return

        await connection.invoke('JoinList', listId)
        joined = true
        setConnectionState('live')
      } catch {
        if (active) setConnectionState('disconnected')
      }
    }

    void start()

    return () => {
      active = false
      joined = false

      if (connection.state === signalR.HubConnectionState.Connected) {
        connection.invoke('LeaveList', listId).catch(() => {})
      }

      connection.stop().catch(() => {})
    }
  }, [listId])

  return { connectionState: listId ? connectionState : 'idle' }
}
