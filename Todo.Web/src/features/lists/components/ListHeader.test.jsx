import { render, screen, waitFor } from '@testing-library/react'
import userEvent from '@testing-library/user-event'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import { deleteList, updateList } from '@/api/lists'
import ListHeader from './ListHeader'

vi.mock('@/api/lists', () => ({
  updateList: vi.fn(),
  deleteList: vi.fn(),
}))

describe('ListHeader', () => {
  const onUpdated = vi.fn()
  const onDeleted = vi.fn()

  beforeEach(() => {
    updateList.mockReset()
    deleteList.mockReset()
    onUpdated.mockReset()
    onDeleted.mockReset()
    updateList.mockResolvedValue({ id: 'list-1', title: 'Updated', color: '#336699' })
    deleteList.mockResolvedValue(undefined)
  })

  it('renames a list', async () => {
    const user = userEvent.setup()

    render(
      <ListHeader
        listId="list-1"
        title="Groceries"
        color="#336699"
        onUpdated={onUpdated}
        onDeleted={onDeleted}
      />,
    )

    await user.click(screen.getByRole('button', { name: /rename groceries/i }))
    await user.clear(screen.getByRole('textbox', { name: /title/i }))
    await user.type(screen.getByRole('textbox', { name: /title/i }), 'Shopping')
    await user.click(screen.getByRole('button', { name: /^save$/i }))

    await waitFor(() => {
      expect(updateList).toHaveBeenCalledWith('list-1', {
        title: 'Shopping',
        color: '#336699',
      })
    })

    expect(onUpdated).toHaveBeenCalled()
  })

  it('deletes a list after confirmation', async () => {
    const user = userEvent.setup()

    render(
      <ListHeader
        listId="list-1"
        title="Groceries"
        color="#336699"
        onUpdated={onUpdated}
        onDeleted={onDeleted}
      />,
    )

    await user.click(screen.getByRole('button', { name: /delete groceries/i }))
    await user.click(screen.getByRole('button', { name: /^delete$/i }))

    await waitFor(() => {
      expect(deleteList).toHaveBeenCalledWith('list-1')
    })

    expect(onDeleted).toHaveBeenCalled()
  })
})
