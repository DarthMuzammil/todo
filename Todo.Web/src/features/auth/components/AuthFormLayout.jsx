import { Link } from 'react-router-dom'
import { BlurFade, DotPattern } from '@/shared/components/magic-ui'
import './AuthFormLayout.css'

export default function AuthFormLayout({ title, subtitle, children, footer }) {
  return (
    <div className="auth-form-layout">
      <div className="auth-form-layout__ambient" aria-hidden="true">
        <div className="auth-form-layout__gradient" />
        <DotPattern className="auth-form-layout__dots" width={24} height={24} cr={0.6} />
      </div>

      <aside className="auth-form-layout__hero" aria-hidden="true">
        <BlurFade delay={0.05}>
          <h2 className="auth-form-layout__hero-title">
            Focus on what matters.
          </h2>
          <p className="auth-form-layout__hero-text">
            A calm workspace for lists and tasks — no clutter, no noise.
            Just the work in front of you.
          </p>
        </BlurFade>
      </aside>

      <div className="auth-form-layout__panel">
        <BlurFade delay={0.1}>
          <section className="auth-form-layout__card" aria-labelledby="auth-form-title">
            <header className="auth-form-layout__header">
              <p className="auth-form-layout__brand">
                <span className="auth-form-layout__brand-icon" aria-hidden="true">
                  T
                </span>
                <span className="auth-form-layout__brand-text">Todo</span>
              </p>
              <h1 id="auth-form-title" className="auth-form-layout__title">
                {title}
              </h1>
              {subtitle && <p className="auth-form-layout__subtitle">{subtitle}</p>}
            </header>
            {children}
            {footer && <footer className="auth-form-layout__footer">{footer}</footer>}
          </section>
        </BlurFade>
      </div>
    </div>
  )
}

export function AuthFormFooterLink({ prompt, linkText, to, state }) {
  return (
    <p className="auth-form-layout__switch">
      {prompt}{' '}
      <Link to={to} state={state} className="auth-form-layout__link">
        {linkText}
      </Link>
    </p>
  )
}
