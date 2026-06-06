import { QueryClient } from '@tanstack/react-query'

// staleTime: 60s — avoid refetch on every remount during dev
// retry: 1 — fail fast when Todo.Api isn't running
export const queryClient = new QueryClient({
  defaultOptions: {
    queries: {
      staleTime: 60_000,
      retry: 1,
    },
    mutations: {
      retry: 0,
    },
  },
})
