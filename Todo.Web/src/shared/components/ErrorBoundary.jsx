import { Component } from 'react'
import { AlertCircle } from 'lucide-react'
import { Button, EmptyState } from '@/shared/components/ui'

export class ErrorBoundary extends Component {
  constructor(props) {
    super(props)
    this.state = { hasError: false }
  }

  static getDerivedStateFromError() {
    return { hasError: true }
  }

  handleTryAgain = () => {
    this.setState({ hasError: false })
  }

  handleReload = () => {
    window.location.reload()
  }

  render() {
    if (this.state.hasError) {
      return (
        <EmptyState
          icon={AlertCircle}
          title="Something went wrong"
          description="An unexpected error occurred. You can try again or reload the page."
          action={
            <>
              <Button variant="secondary" onClick={this.handleTryAgain}>
                Try again
              </Button>
              <Button variant="primary" onClick={this.handleReload}>
                Reload page
              </Button>
            </>
          }
        />
      )
    }

    return this.props.children
  }
}
