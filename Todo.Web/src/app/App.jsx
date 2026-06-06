import { Route, Routes } from 'react-router-dom'
import { NotFoundPage } from '@/app/pages/NotFoundPage'
import { AppLayout } from '@/app/layout/AppLayout'
import { AuthenticatedShell } from '@/app/layout/AuthenticatedShell'
import {
  LoginPage,
  ProtectedRoute,
  RegisterPage,
  SettingsPage,
} from '@/features/auth'
import { HomePage, ListPage } from '@/features/lists'
import { InviteAcceptPage } from '@/features/workspaces'

function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route path="/register" element={<RegisterPage />} />
      <Route path="/invites/:token" element={<InviteAcceptPage />} />
      <Route element={<ProtectedRoute />}>
        <Route element={<AuthenticatedShell />}>
          <Route element={<AppLayout />}>
            <Route path="/" element={<HomePage />} />
            <Route path="/settings" element={<SettingsPage />} />
            <Route path="/lists/:listId" element={<ListPage />} />
          </Route>
        </Route>
      </Route>
      <Route path="*" element={<NotFoundPage />} />
    </Routes>
  )
}

export default App
