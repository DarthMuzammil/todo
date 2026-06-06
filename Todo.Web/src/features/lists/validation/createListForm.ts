export interface CreateListFormValues {
  title: string
  color: string
}
export type CreateListFieldErrors = Partial<
  Record<keyof CreateListFormValues, string>
>
// e.g. { title: "Title is required" }
const TITLE_MAX_LENGTH: number = 100 // pick a constant; backend has no limit yet

export function validateCreateListForm(
  values: CreateListFormValues,
): CreateListFieldErrors {
  const errors: CreateListFieldErrors = {}

  const trimmedTitle = values.title.trim()

  if (!trimmedTitle) {
    errors.title = 'Title is required'
  } else if (trimmedTitle.length > TITLE_MAX_LENGTH) {
    errors.title = `Title must be at most ${TITLE_MAX_LENGTH} characters`
  }

  return errors
}

export function hasCreateListFormErrors(
  errors: CreateListFieldErrors,
): boolean {
  return Object.keys(errors).length > 0
}
