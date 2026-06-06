/**
 * MVP owner ID
 *
 * createList requires ownerId; auth is not implemented yet.
 * Strategy: fixed dev UUID — predictable for local dev, one owner per machine.
 * Override via VITE_DEV_OWNER_ID in .env.development if needed.
 *
 * Replaced later by an ownerId from the authenticated user.
 */
const DEFAULT_DEV_OWNER_ID = '00000000-0000-0000-0000-000000000001'

export const DEV_OWNER_ID =
  import.meta.env.VITE_DEV_OWNER_ID ?? DEFAULT_DEV_OWNER_ID
