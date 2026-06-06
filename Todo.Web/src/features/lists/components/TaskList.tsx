import type { TodoTask } from '@/api/types'
import TaskListEmpty from './TaskListEmpty'
import TaskListItem from './TaskListItem'

interface TaskListProps {
    tasks: TodoTask[]
}

export default function TaskList({ tasks }: TaskListProps) {
    if (tasks.length === 0) {
        return <TaskListEmpty />
    }
    return (
        <ul>
            {tasks.map((task) => (
                <TaskListItem key={task.id} task={task} />
            ))}
        </ul>
    )
}