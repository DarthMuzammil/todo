# Design Tokens v2

Token reference for Todo Design System v2. Source of truth: `src/index.css`.  
See also `designdoc.md` for component usage rules.

## Neutral scale

| Token | Hex | Usage |
|-------|-----|-------|
| `--color-neutral-50` | `#f8fafc` | Page background (light) |
| `--color-neutral-100` | `#f1f5f9` | Subtle fills |
| `--color-neutral-200` | `#e2e8f0` | Borders default |
| `--color-neutral-300` | `#cbd5e1` | Borders strong |
| `--color-neutral-400` | `#94a3b8` | Placeholder, disabled |
| `--color-neutral-500` | `#64748b` | Text subtle |
| `--color-neutral-600` | `#475569` | Text secondary |
| `--color-neutral-700` | `#334155` | Headings on dark |
| `--color-neutral-800` | `#1e293b` | Dark surfaces |
| `--color-neutral-900` | `#0f172a` | Text primary (light) |

## Brand

| Token | Hex |
|-------|-----|
| `--color-brand-50` | `#eff6ff` |
| `--color-brand-100` | `#dbeafe` |
| `--color-brand-500` | `#3b82f6` |
| `--color-brand-600` | `#2563eb` |
| `--color-brand-700` | `#1d4ed8` |
| `--color-brand-800` | `#1e40af` |

## Semantic colors

| Role | Text | Surface | Border |
|------|------|---------|--------|
| Success | `--color-success-text` | `--color-success-surface` | `--color-success-border` |
| Warning | `--color-warning-text` | `--color-warning-surface` | `--color-warning-border` |
| Danger | `--color-danger-text` | `--color-danger-surface` | `--color-danger-border` |
| Info | `--color-info-text` | `--color-info-surface` | `--color-info-border` |

## Semantic aliases (use in components)

| Token | Light value |
|-------|-------------|
| `--color-text-primary` | `neutral-900` |
| `--color-text-secondary` | `neutral-600` |
| `--color-text-subtle` | `neutral-500` |
| `--color-bg-page` | `neutral-50` |
| `--color-surface-default` | `#ffffff` |
| `--color-border-default` | `neutral-200` |
| `--color-action-primary` | `brand-600` |

## Typography

| Token | Size | Usage |
|-------|------|-------|
| `--font-heading-lg` | 1.5rem | List title |
| `--font-heading-md` | 1.125rem | Task title |
| `--font-body` | 1rem | Body, inputs |
| `--font-body-sm` | 0.875rem | Descriptions |
| `--font-caption` | 0.75rem | Badges |

## Spacing (4px grid)

| Token | Value |
|-------|-------|
| `--space-1` | 4px |
| `--space-2` | 8px |
| `--space-3` | 12px |
| `--space-4` | 16px |
| `--space-6` | 24px |
| `--space-8` | 32px |

## Layout

| Token | Value |
|-------|-------|
| `--layout-header-height` | 56px |
| `--layout-sidebar-width` | 240px |
| `--layout-content-max` | 45rem (720px) |

## Elevation

| Token | Usage |
|-------|-------|
| `--shadow-sm` | Raised cards |
| `--shadow-md` | Dropdowns |
| `--shadow-lg` | Modals |

## Dark mode

Set `data-theme="dark"` on `<html>`. Semantic aliases invert per `designdoc.md` §1.8. Status surfaces use `color-mix` on `neutral-800`.

## v1 aliases (deprecated)

| v1 | v2 |
|----|-----|
| `--color-text` | `--color-text-primary` |
| `--color-bg` | `--color-bg-page` |
| `--space-lg` | `--space-4` |
| `--font-size-xl` | `--font-heading-lg` |
