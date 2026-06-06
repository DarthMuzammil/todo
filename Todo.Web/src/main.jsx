import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { BrowserRouter } from 'react-router-dom'
import './tailwind.css'
import './index.css'
import App from '@/app/App'
import { ErrorBoundary } from '@/shared/components/ErrorBoundary'
import { AuthProvider } from '@/features/auth'
import { initTheme } from '@/shared/hooks/useTheme'

initTheme()

createRoot(document.getElementById('root')).render(
  <StrictMode>
    <ErrorBoundary>
      <BrowserRouter>
        <AuthProvider>
          <App />
        </AuthProvider>
      </BrowserRouter>
    </ErrorBoundary>
  </StrictMode>,
)
