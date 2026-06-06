import type { TodoTask } from '@/api/types'

interface TaskListItemProps {
    task: TodoTask
}

export default function TaskListItem({ task }: TaskListItemProps) {
    return (
        <li>
            <h3>{task.title}</h3>
            <p>{task.description}</p>
            <p>{task.status}</p>
            <p>{task.priority}</p>
            <p>{task.dueDate}</p>
            <p>{task.assigneeId}</p>
            <p>{task.parentTaskId}</p>
        </li>
    )
} 