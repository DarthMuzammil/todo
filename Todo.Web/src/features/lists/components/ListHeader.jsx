import './ListHeader.css'

export default function ListHeader({ title, color }) {
  return (
    <header className="list-header">
      <h1 className="list-header__title">{title}</h1>
      {color && (
        <span
          className="list-header__swatch"
          style={{ backgroundColor: color }}
          aria-hidden="true"
        />
      )}
    </header>
  )
}
