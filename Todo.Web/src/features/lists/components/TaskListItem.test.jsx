import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import { deleteTask, updateTaskStatus } from '@/api/tasks'
import TaskListItem from './TaskListItem'

vi.mock('@/api/tasks', () => ({
  updateTaskStatus: vi.fn(),
  deleteTask: vi.fn(),
}))

const task = {
  id: 'task-1',
  title: 'Buy milk',
  description: '2%',
  status: 0,
  priority: 1,
  dueDate: null,
}

describe('TaskListItem', () => {
  const onChanged = vi.fn()

  beforeEach(() => {
    updateTaskStatus.mockReset()
    deleteTask.mockReset()
    onChanged.mockReset()
    updateTaskStatus.mockResolvedValue({ ...task, status: 2 })
    deleteTask.mockResolvedValue(undefined)
  })

  it('updates task status', async () => {
    const user = userEvent.setup()

    render(
      <ul>
        <TaskListItem listId="list-1" task={task} onChanged={onChanged} />
      </ul>,
    )

    await user.selectOptions(
      screen.getByRole('combobox', { name: /status/i }),
      '2',
    )

    await waitFor(() => {
      expect(updateTaskStatus).toHaveBeenCalledWith('list-1', 'task-1', {
        newStatus: 2,
      })
    })

    expect(onChanged).toHaveBeenCalled()
  })

  it('deletes a task after confirmation', async () => {
    const user = userEvent.setup()

    render(
      <ul>
        <TaskListItem listId="list-1" task={task} onChanged={onChanged} />
      </ul>,
    )

    await user.click(screen.getByRole('button', { name: /delete buy milk/i }))
    await user.click(screen.getByRole('button', { name: /^delete$/i }))

    await waitFor(() => {
      expect(deleteTask).toHaveBeenCalledWith('list-1', 'task-1')
    })

    expect(onChanged).toHaveBeenCalled()
  })
})
