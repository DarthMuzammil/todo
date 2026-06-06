/** Callback invoked when the API returns 401 (session expired or invalid token). */
let onUnauthorized = null

export function setUnauthorizedHandler(handler) {
  onUnauthorized = handler
}

export function notifyUnauthorized() {
  onUnauthorized?.()
}
