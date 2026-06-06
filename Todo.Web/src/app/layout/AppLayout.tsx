import './AppLayout.css'
import { Outlet } from 'react-router-dom'

export function AppLayout() {
    return (
        <div className="layout">
            <header className="layout__header">
                <h1>Todo</h1>
                <p>Create a list to get started.</p>
            </header>
            <main className="layout__main">
                <Outlet />
            </main>
        </div>
    )
}