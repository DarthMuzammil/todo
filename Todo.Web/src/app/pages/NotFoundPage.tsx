import { Link } from 'react-router-dom'

export function NotFoundPage() {
  return (
    <>
      <h1>404</h1>
      <p>Page not found.</p>
      <p>
        <Link to="/">Back to home</Link>
      </p>
    </>
  )
}
