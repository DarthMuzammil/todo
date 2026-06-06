import { Skeleton } from '@/shared/components/ui'
import './TaskListSkeleton.css'

export default function TaskListSkeleton() {
  return (
    <ul className="task-list-skeleton" aria-busy="true" aria-label="Loading tasks">
      <li>
        <Skeleton variant="rect" height="8rem" />
      </li>
      <li>
        <Skeleton variant="rect" height="8rem" />
      </li>
    </ul>
  )
}
