import { useQuery } from '@tanstack/react-query'
import { useParams } from 'react-router-dom'
import { getListById } from '@/api/lists'
import { getTasksByListId } from '@/api/tasks'
import { ApiClientError } from '@/api/client'
import ListHeader from '../components/ListHeader'
import ListNotFound from '../components/ListNotFound'
import ListHeaderSkeleton from '../components/ListHeaderSkeleton'
import TaskListSkeleton from '../components/TaskListSkeleton'
import TaskListError from '../components/TaskListError'
import TaskList from '../components/TaskList'

export function ListPage() {
  const { listId } = useParams<{ listId: string }>()

  const {
    data: list,
    isPending,
    isError,
    error,
  } = useQuery({
    queryKey: ['list', listId],
    queryFn: () => getListById(listId!),
    enabled: !!listId,
  })

  const tasksQuery = useQuery({
    queryKey: ['list', listId, 'tasks'],
    queryFn: () => getTasksByListId(listId!),
    enabled: !!listId,
  })

  if (!listId) {
    return <p>Invalid list URL.</p>
  }

  if (isPending) {
    return <ListHeaderSkeleton />
  }

  if (isError) {
    if (error instanceof ApiClientError && error.status === 404) {
      return <ListNotFound message={error.body.error} />
    }
    const message =
      error instanceof ApiClientError ? error.body.error : 'Failed to load list'
    return <p role="alert">{message}</p>   // or <GenericError message={message} />
  }

  return (
    <>
      <ListHeader title={list.title} color={list.color} />
      {tasksQuery.isPending && <TaskListSkeleton />}
      {tasksQuery.isError && <TaskListError message="Failed to load tasks" />}
      {tasksQuery.isSuccess && (
        <TaskList tasks={tasksQuery.data || []} />
      )}
    </>
  )
}
