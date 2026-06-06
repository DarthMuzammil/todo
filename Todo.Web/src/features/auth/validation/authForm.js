const EMAIL_PATTERN = /^[^\s@]+@[^\s@]+\.[^\s@]+$/

export function validateLoginForm({ email, password }) {
  const errors = {}

  if (!email.trim()) {
    errors.email = 'Email is required'
  } else if (!EMAIL_PATTERN.test(email.trim())) {
    errors.email = 'Enter a valid email address'
  }

  if (!password) {
    errors.password = 'Password is required'
  }

  return errors
}

export function validateRegisterForm({ name, email, password }) {
  const errors = validateLoginForm({ email, password })

  if (!name.trim()) {
    errors.name = 'Name is required'
  }

  if (password && password.length < 8) {
    errors.password = 'Password must be at least 8 characters'
  }

  return errors
}

export function hasAuthFormErrors(errors) {
  return Object.keys(errors).length > 0
}

export function validateProfileForm({ name }) {
  const errors = {}

  if (!name.trim()) {
    errors.name = 'Name is required'
  }

  return errors
}

export function validateChangePasswordForm({ currentPassword, newPassword }) {
  const errors = {}

  if (!currentPassword) {
    errors.currentPassword = 'Current password is required'
  }

  if (!newPassword) {
    errors.newPassword = 'New password is required'
  } else if (newPassword.length < 8) {
    errors.newPassword = 'Password must be at least 8 characters'
  }

  return errors
}
