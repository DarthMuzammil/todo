interface ListHeaderProps {
    title: string
    color: string | null | undefined 
}
export default function ListHeader({ title, color }: ListHeaderProps) {
    return (
        <header>
            <h1>{title}</h1>
            {color && <span className="color-swatch" style={{ backgroundColor: color }} />}
        </header>
    )
}