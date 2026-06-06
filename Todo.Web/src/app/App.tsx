import { Route, Routes } from 'react-router-dom'
import { NotFoundPage } from '@/app/pages/NotFoundPage'
import { HomePage } from '@/features/lists/pages/HomePage'
import { ListPage } from '@/features/lists/pages/ListPage'
import { AppLayout } from '@/app/layout/AppLayout'

function App() {
  return (
      <Routes>
        <Route element={<AppLayout />}>
          <Route path="/" element={<HomePage />} />
          <Route path="/lists/:listId" element={<ListPage />} />
        </Route>
        <Route path="*" element={<NotFoundPage />} />
      </Routes>
  )
}

export default App
