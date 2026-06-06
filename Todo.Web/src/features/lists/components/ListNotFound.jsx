import { Link } from 'react-router-dom'
import { FileQuestion } from 'lucide-react'
import { EmptyState } from '@/shared/components/ui'

export default function ListNotFound({ message }) {
  return (
    <EmptyState
      icon={FileQuestion}
      title="List not found"
      description={message ?? 'This list does not exist or was removed.'}
      action={
        <Link to="/" className="btn btn--primary btn--md">
          Back to home
        </Link>
      }
    />
  )
}
