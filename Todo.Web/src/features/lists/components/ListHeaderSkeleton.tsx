export default function ListHeaderSkeleton() {
    return (
        <header className="list-header-skeleton" aria-busy="true" aria-label="Loading list">
            <div className="skeleton skeleton--title" />
            <div className="skeleton skeleton--swatch" />
        </header>
    )
}