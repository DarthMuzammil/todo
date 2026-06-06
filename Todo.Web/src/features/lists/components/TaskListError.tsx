export default function TaskListError({ message }: TaskListErrorProps) {
    return (
        <p>{message}</p>
    )
}

interface TaskListErrorProps {
    message: string
}