import TaskListEmpty from './TaskListEmpty'

import TaskListItem from './TaskListItem'

import './TaskList.css'

export default function TaskList({ listId, tasks, onChanged }) {
  if (tasks.length === 0) {
    return <TaskListEmpty />
  }

  return (
    <ul className="task-list">
      {tasks.map((task) => (
        <TaskListItem
          key={task.id}
          listId={listId}
          task={task}
          onChanged={onChanged}
        />
      ))}
    </ul>
  )
}
