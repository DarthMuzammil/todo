import { getErrorMessage } from '@/shared/utils/getErrorMessage'
import { Alert, Card } from '@/shared/components/ui'
import { useListActivity } from '../hooks/useListActivity'
import './ActivityFeed.css'

function formatWhen(timestamp) {
  return new Date(timestamp).toLocaleString(undefined, {
    month: 'short',
    day: 'numeric',
    hour: 'numeric',
    minute: '2-digit',
  })
}

export default function ActivityFeed({ listId, refreshVersion = 0 }) {
  const { data, status, error } = useListActivity(listId, refreshVersion)
  const items = data ?? []

  return (
    <section className="activity-feed" aria-labelledby="activity-feed-heading">
      <Card padding="md" className="activity-feed__card">
        <h2 id="activity-feed-heading" className="activity-feed__title">
          Recent activity
        </h2>

        {status === 'loading' && (
          <p className="activity-feed__status" aria-live="polite">
            Loading activity…
          </p>
        )}

        {status === 'error' && (
          <Alert
            variant="error"
            compact
            message={getErrorMessage(error, 'Failed to load activity')}
          />
        )}

        {status === 'success' && items.length === 0 && (
          <p className="activity-feed__empty">No activity on this list yet.</p>
        )}

        {status === 'success' && items.length > 0 && (
          <ul className="activity-feed__list">
            {items.map((item) => (
              <li key={item.id} className="activity-feed__item">
                <p className="activity-feed__message">{item.message}</p>
                <time className="activity-feed__time" dateTime={item.createdAt}>
                  {formatWhen(item.createdAt)}
                </time>
              </li>
            ))}
          </ul>
        )}
      </Card>
    </section>
  )
}
