import { useState } from 'react'
import { useNavigate, useParams } from 'react-router-dom'
import { AlertCircle } from 'lucide-react'
import { ApiClientError } from '@/api/client'
import { useWorkspaceRole } from '@/features/workspaces/hooks/useWorkspaceRole'
import { Button, EmptyState } from '@/shared/components/ui'
import { getErrorMessage } from '@/shared/utils/getErrorMessage'
import { useList } from '../hooks/useList'
import { useLiveTasks } from '../hooks/useLiveTasks'
import ListHeader from '../components/ListHeader'
import ListNotFound from '../components/ListNotFound'
import ListHeaderSkeleton from '../components/ListHeaderSkeleton'
import TaskListSkeleton from '../components/TaskListSkeleton'
import TaskListError from '../components/TaskListError'
import TaskList from '../components/TaskList'
import ActivityFeed from '../components/ActivityFeed'
import CreateTaskForm from '../components/CreateTaskForm'

export function ListPage() {
  const navigate = useNavigate()
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
    connectionState,
  } = useLiveTasks(listId)
  const { canWrite, isViewer } = useWorkspaceRole(list?.workspaceId)
  const [activityVersion, setActivityVersion] = useState(0)

  function refreshTasksAndActivity() {
    refetchTasks()
    setActivityVersion((version) => version + 1)
  }

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
      <ListHeader
        listId={listId}
        title={list.title}
        color={list.color}
        readOnly={!canWrite}
        isViewer={isViewer}
        connectionState={connectionState}
        onUpdated={refetchList}
        onDeleted={() => navigate('/')}
      />
      {canWrite ? (
        <CreateTaskForm listId={listId} onTaskCreated={refreshTasksAndActivity} />
      ) : null}
      {tasksStatus === 'loading' && <TaskListSkeleton />}
      {tasksStatus === 'error' && (
        <TaskListError
          message={getErrorMessage(tasksError, 'Failed to load tasks')}
          onRetry={refetchTasks}
        />
      )}
      {tasksStatus === 'success' && (
        <>
          <TaskList
            listId={listId}
            tasks={tasks}
            onChanged={refreshTasksAndActivity}
            readOnly={!canWrite}
          />
          <ActivityFeed listId={listId} refreshVersion={activityVersion} />
        </>
      )}
    </>
  )
}
