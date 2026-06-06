import { useParams } from 'react-router-dom'
import { AlertCircle } from 'lucide-react'
import { ApiClientError } from '@/api/client'
import { Button, EmptyState } from '@/shared/components/ui'
import { getErrorMessage } from '@/shared/utils/getErrorMessage'
import { useList } from '../hooks/useList'
import { useTasks } from '../hooks/useTasks'
import ListHeader from '../components/ListHeader'
import ListNotFound from '../components/ListNotFound'
import ListHeaderSkeleton from '../components/ListHeaderSkeleton'
import TaskListSkeleton from '../components/TaskListSkeleton'
import TaskListError from '../components/TaskListError'
import TaskList from '../components/TaskList'
import CreateTaskForm from '../components/CreateTaskForm'

export function ListPage() {
  const { listId } = useParams()
  const {
    list,
    status: listStatus,
    error: listError,
    refetch: refetchList,
  } = useList(listId)
  const {
    tasks,
    status: tasksStatus,
    error: tasksError,
    refetch: refetchTasks,
  } = useTasks(listId)

  if (!listId) {
    return <p>Invalid list URL.</p>
  }

  if (listStatus === 'loading') {
    return <ListHeaderSkeleton />
  }

  if (listStatus === 'error') {
    if (listError instanceof ApiClientError && listError.status === 404) {
      return <ListNotFound message={listError.body.error} />
    }

    return (
      <EmptyState
        icon={AlertCircle}
        title="Couldn't load list"
        description={getErrorMessage(listError, 'Failed to load list')}
        action={
          <Button variant="secondary" onClick={refetchList}>
            Try again
          </Button>
        }
      />
    )
  }

  return (
    <>
      <ListHeader title={list.title} color={list.color} />
      <CreateTaskForm listId={listId} onTaskCreated={refetchTasks} />
      {tasksStatus === 'loading' && <TaskListSkeleton />}
      {tasksStatus === 'error' && (
        <TaskListError
          message={getErrorMessage(tasksError, 'Failed to load tasks')}
          onRetry={refetchTasks}
        />
      )}
      {tasksStatus === 'success' && (
        <TaskList listId={listId} tasks={tasks} onChanged={refetchTasks} />
      )}
    </>
  )
}
