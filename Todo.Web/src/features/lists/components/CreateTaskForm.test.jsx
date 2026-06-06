import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import { createTask } from '@/api/tasks'
import CreateTaskForm from './CreateTaskForm'

vi.mock('@/api/tasks', () => ({
  createTask: vi.fn(),
}))

describe('CreateTaskForm', () => {
  const onTaskCreated = vi.fn()

  beforeEach(() => {
    createTask.mockReset()
    onTaskCreated.mockReset()
  })

  it('creates a task and notifies the parent', async () => {
    const user = userEvent.setup()
    createTask.mockResolvedValue({ id: 'task-1', title: 'Buy milk' })

    render(<CreateTaskForm listId="list-1" onTaskCreated={onTaskCreated} />)

    await user.type(
      screen.getByRole('textbox', { name: /^title$/i }),
      'Buy milk',
    )
    await user.click(screen.getByRole('button', { name: /add task/i }))

    await waitFor(() => {
      expect(createTask).toHaveBeenCalledWith('list-1', {
        title: 'Buy milk',
        description: null,
        priority: 1,
        dueDate: null,
      })
    })

    expect(onTaskCreated).toHaveBeenCalled()
    expect(screen.getByRole('textbox', { name: /^title$/i })).toHaveValue('')
  })
})
