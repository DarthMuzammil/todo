import { Skeleton } from '@/shared/components/ui'
import './ListHeaderSkeleton.css'

export default function ListHeaderSkeleton() {
  return (
    <header
      className="list-header-skeleton"
      aria-busy="true"
      aria-label="Loading list"
    >
      <Skeleton variant="rect" width="60%" height="2rem" className="list-header-skeleton__title" />
      <Skeleton variant="circle" width="1.25rem" height="1.25rem" />
    </header>
  )
}
