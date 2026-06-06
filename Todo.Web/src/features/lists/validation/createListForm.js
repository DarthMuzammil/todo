const TITLE_MAX_LENGTH = 100

export function validateCreateListForm(values) {
  const errors = {}

  const trimmedTitle = values.title.trim()

  if (!trimmedTitle) {
    errors.title = 'Title is required'
  } else if (trimmedTitle.length > TITLE_MAX_LENGTH) {
    errors.title = `Title must be at most ${TITLE_MAX_LENGTH} characters`
  }

  return errors
}

export function hasCreateListFormErrors(errors) {
  return Object.keys(errors).length > 0
}
