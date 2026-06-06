import { Link } from "react-router-dom"

interface ListNotFoundProps {
    message?: string
  }
  
  export default function ListNotFound({ message }: ListNotFoundProps) {
    return (
      <section>
        <h1>List not found</h1>
        <p>{message ?? 'This list does not exist or was removed.'}</p>
        <Link to="/">Back to home</Link>
      </section>
    )
  }