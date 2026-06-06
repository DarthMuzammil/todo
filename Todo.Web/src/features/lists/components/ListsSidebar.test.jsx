import { render, screen, waitFor } from '@testing-library/react'
import { MemoryRouter } from 'react-router-dom'
import { beforeEach, describe, expect, it, vi } from 'vitest'
import { getLists } from '@/api/lists'
import ListsSidebar from './ListsSidebar'

vi.mock('@/api/lists', () => ({
  getLists: vi.fn(),
}))

vi.mock('@/shared/config/dev', () => ({
  DEV_OWNER_ID: '00000000-0000-0000-0000-000000000001',
}))

describe('ListsSidebar', () => {
  beforeEach(() => {
    getLists.mockReset()
  })

  it('renders lists from the API', async () => {
    getLists.mockResolvedValue([
      { id: 'list-1', title: 'Groceries', color: '#ff0000', updatedAt: '2026-01-02' },
      { id: 'list-2', title: 'Work', color: '#00ff00', updatedAt: '2026-01-01' },
    ])

    render(
      <MemoryRouter initialEntries={['/lists/list-1']}>
        <ListsSidebar />
      </MemoryRouter>,
    )

    await waitFor(() => {
      expect(getLists).toHaveBeenCalledWith('00000000-0000-0000-0000-000000000001')
    })

    expect(screen.getByRole('link', { name: /groceries/i })).toHaveAttribute(
      'href',
      '/lists/list-1',
    )
    expect(screen.getByRole('link', { name: /work/i })).toHaveAttribute(
      'href',
      '/lists/list-2',
    )
  })
})
