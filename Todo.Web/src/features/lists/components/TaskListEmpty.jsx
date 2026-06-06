import { ClipboardList } from 'lucide-react'
import { EmptyState } from '@/shared/components/ui'

export default function TaskListEmpty() {
  return (
    <EmptyState
      icon={ClipboardList}
      title="No tasks yet"
      description="Add your first task to this list."
    />
  )
}
