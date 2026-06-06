import { AlertCircle } from 'lucide-react'
import { Button, EmptyState } from '@/shared/components/ui'

export default function TaskListError({ message, onRetry }) {
  return (
    <EmptyState
      icon={AlertCircle}
      title="Couldn't load tasks"
      description={message}
      action={
        onRetry ? (
          <Button variant="secondary" onClick={onRetry}>
            Try again
          </Button>
        ) : undefined
      }
    />
  )
}
