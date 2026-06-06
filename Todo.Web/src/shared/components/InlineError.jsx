import Alert from './ui/Alert'

/** @deprecated Use Alert from ui/ instead */
export default function InlineError({ message, id }) {
  return <Alert id={id} variant="error" compact message={message} />
}
