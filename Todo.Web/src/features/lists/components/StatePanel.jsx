import './StatePanel.css'

export default function StatePanel({ title, message, children }) {
  return (
    <section className="state-panel">
      {title && <h2 className="state-panel__title">{title}</h2>}
      {message && <p className="state-panel__message">{message}</p>}
      {children && <div className="state-panel__actions">{children}</div>}
    </section>
  )
}
