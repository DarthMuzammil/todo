# Todo Design System — Engineering Handoff

**Status:** Ready for implementation (Sprint A complete → unblocks FE-25–FE-31)  
**Author:** Design Engineering  
**Date:** 2026-06-06  
**Covers:** DS-1 through DS-7 (`plan.md` Phase 1)  
**Figma:** _[Link TBD — mirror specs below in `docs/design/figma-export/`]_

---

## 0. Purpose & audience

This document translates design research into **implementation-ready specifications** for the engineering team building Todo Design System v2. It supersedes ad-hoc styling in feature CSS (v1 tokens in `src/index.css`) and defines the contract for shared UI in `src/shared/components/ui/`.

**Engineering tickets unblocked by this doc:**

| Ticket | What to build from this doc |
|--------|----------------------------|
| FE-25 | §1–§4 token tables → `src/index.css` + `docs/design/tokens.md` |
| FE-26 | §5 component specs → `Button`, `Input`, `Select`, `Badge`, `Card` |
| FE-27 | §3 layout grid → `AppLayout` shell + sidebar placeholder |
| FE-28 | §5.10 `ConfirmDialog` |
| FE-29 | §5.8 Empty state + §5.9 Skeleton |
| FE-30 | §1.4 dark mode token overrides |
| FE-31 | Screenshot baselines at breakpoints in §3.2 |

---

## 1. Design research summary

### 1.1 Competitive audit (task-product category)

| Product | Visual posture | Relevant pattern for Todo |
|---------|----------------|---------------------------|
| **Todoist** | Warm red accent, dense lists, colored priority flags | Priority/status must be **labeled + colored**; red reserved for destructive/overdue |
| **Things** | Generous whitespace, soft neutrals, minimal chrome | **Calm density** — our north star for list readability |
| **Linear** | Dark-first, tight typography, subtle borders | Semantic tokens + elevation levels; sidebar navigation model (Phase 3) |
| **Microsoft To Do** | Fluent blues, rounded cards, clear empty states | Card-based task rows; friendly empty-state copy + single CTA |
| **Apple Reminders** | System-native, grouped lists | Sidebar list nav (Phase 3); status groups later |

### 1.2 v1 audit (current codebase)

| Finding | v1 location | v2 action |
|---------|-------------|-----------|
| ~15 hard-coded hex values in badge CSS | `TaskListItem.css` | Replace with semantic status/priority tokens (§1.3) |
| Buttons duplicated in 4 stylesheets | `forms.css`, `TaskListItem.css`, `StatePanel.css` | Single `Button` component (§5.1) |
| No elevation system | flat `border: 1px` only | Introduce shadow scale for modals/dropdowns (§4) |
| System font only | `index.css` | Keep system stack as default; optional Inter via `font-display: swap` (§2) |
| Light mode only | `:root` tokens | Add `[data-theme="dark"]` block (§1.4) |
| Inline delete confirm | `TaskListItem.jsx` | `ConfirmDialog` pattern (§5.10) |

### 1.3 Ratified design principles

1. **Calm density** — 16px base body; task cards breathe with 16–24px padding; max one primary button per view.
2. **Status at a glance** — every status/priority badge includes a text label; color never carries meaning alone (WCAG 1.4.1).
3. **One primary action per view** — list page: "Add task"; home: "Create list"; destructive actions are secondary/ghost.
4. **Progressive disclosure** — delete confirm, sidebar collapse, and modals layer on demand; default surface stays quiet.
5. **System-native when possible** — prefer platform fonts and `prefers-reduced-motion` over bespoke motion.

---

## 2. DS-1 — Brand & color palette

### 2.1 Brand identity

| Attribute | Value | Rationale |
|-----------|-------|-----------|
| Product name | **Todo** | Single word mark in header; no logo asset in v2 |
| Personality | Capable, calm, trustworthy | Avoid playful illustration; use geometric empty states |
| Accent hue | Blue 600 family | Aligns with v1 `--color-primary`; reads as "action" not "error" |

### 2.2 Neutral scale (slate-derived)

Use CSS custom properties `--color-neutral-{50..900}`. Map semantic tokens to this scale — **never reference raw neutrals in components**.

| Token | Hex | Usage |
|-------|-----|-------|
| `--color-neutral-50` | `#f8fafc` | Page background (light) |
| `--color-neutral-100` | `#f1f5f9` | Subtle fills, disabled surfaces |
| `--color-neutral-200` | `#e2e8f0` | Borders default |
| `--color-neutral-300` | `#cbd5e1` | Borders strong, dividers |
| `--color-neutral-400` | `#94a3b8` | Placeholder text, icons muted |
| `--color-neutral-500` | `#64748b` | Text subtle, captions |
| `--color-neutral-600` | `#475569` | Text secondary |
| `--color-neutral-700` | `#334155` | Text primary (dark surfaces) |
| `--color-neutral-800` | `#1e293b` | Headings on dark |
| `--color-neutral-900` | `#0f172a` | Text primary (light surfaces) |

### 2.3 Brand & action colors

| Token | Hex | Contrast on white | Usage |
|-------|-----|-------------------|-------|
| `--color-brand-50` | `#eff6ff` | — | Primary subtle bg, selected nav |
| `--color-brand-100` | `#dbeafe` | — | Info badges, in-progress status bg |
| `--color-brand-500` | `#3b82f6` | 3.4:1 (large text only) | Hover links |
| `--color-brand-600` | `#2563eb` | 4.6:1 ✓ | Primary buttons, focus ring |
| `--color-brand-700` | `#1d4ed8` | 6.2:1 ✓ | Primary button hover |
| `--color-brand-800` | `#1e40af` | 8.1:1 ✓ | Medium priority text |

### 2.4 Semantic colors

| Role | Text token | Surface token | Border token | Notes |
|------|------------|---------------|--------------|-------|
| **Success** | `--color-success-text` `#166534` | `--color-success-surface` `#dcfce7` | `#bbf7d0` | Done status |
| **Warning** | `--color-warning-text` `#9a3412` | `--color-warning-surface` `#ffedd5` | `#fed7aa` | High priority, overdue |
| **Danger** | `--color-danger-text` `#b91c1c` | `--color-danger-surface` `#fef2f2` | `#fecaca` | Delete, errors |
| **Info** | `--color-info-text` `#1d4ed8` | `--color-info-surface` `#dbeafe` | `#bfdbfe` | In progress |

### 2.5 Semantic surface hierarchy (light mode)

Map these in `:root` — components use **only** semantic names:

```css
/* Semantic aliases — light mode */
--color-text-primary:    var(--color-neutral-900);
--color-text-secondary:  var(--color-neutral-600);
--color-text-subtle:     var(--color-neutral-500);
--color-text-disabled:   var(--color-neutral-400);
--color-text-inverse:    var(--color-neutral-50);

--color-bg-page:         var(--color-neutral-50);
--color-bg-subtle:       var(--color-neutral-100);
--color-surface-default: #ffffff;
--color-surface-raised:  #ffffff;   /* + shadow in §4 */
--color-surface-sunken:  var(--color-neutral-100);

--color-border-default:  var(--color-neutral-200);
--color-border-strong:   var(--color-neutral-300);
--color-border-focus:    var(--color-brand-600);

--color-action-primary:        var(--color-brand-600);
--color-action-primary-hover:  var(--color-brand-700);
--color-action-primary-text:   #ffffff;
```

### 2.6 Task status & priority tokens

Replace hard-coded badge colors in `TaskListItem.css`:

| Domain | Variant | Text | Surface |
|--------|---------|------|---------|
| Priority | Low (0) | `--color-text-subtle` | `--color-bg-subtle` |
| Priority | Medium (1) | `--color-brand-800` | `--color-brand-100` |
| Priority | High (2) | `--color-warning-text` | `--color-warning-surface` |
| Status | Todo (0) | `--color-neutral-700` | `--color-neutral-200` |
| Status | In progress (1) | `--color-info-text` | `--color-info-surface` |
| Status | Done (2) | `--color-success-text` | `--color-success-surface` |
| Status | Cancelled (3) | `--color-text-subtle` | `--color-bg-subtle` + `line-through` |

### 2.7 DS-1 accessibility requirements

- Body text (`--color-text-primary` on `--color-bg-page`): **≥ 4.5:1** ✓ (12.6:1)
- Secondary text on page: **≥ 4.5:1** ✓ (`neutral-600` on `neutral-50` = 5.7:1)
- Primary button text on brand-600: **≥ 4.5:1** ✓ (white on `#2563eb` = 4.6:1)
- Never use brand-500 for body-sized text on white
- Overdue due date: use `--color-danger-text` + **"(overdue)"** label (already implemented)

### 2.8 DS-1 dark mode (`[data-theme="dark"]`)

FE-30 implements this block. Key inversions:

| Semantic token | Dark value |
|----------------|------------|
| `--color-text-primary` | `neutral-50` |
| `--color-text-secondary` | `neutral-400` |
| `--color-bg-page` | `neutral-900` |
| `--color-surface-default` | `neutral-800` |
| `--color-surface-raised` | `neutral-700` |
| `--color-border-default` | `neutral-700` |
| `--color-border-strong` | `neutral-600` |

Status/priority surfaces: use **20% opacity** of hue on `neutral-800` instead of pastel backgrounds (pastels fail contrast on dark). Engineering formula:

```css
/* Example: success surface dark */
--color-success-surface: color-mix(in srgb, #22c55e 15%, var(--color-neutral-800));
```

Toggle: header icon button; persist `theme` in `localStorage`; respect `prefers-color-scheme` on first visit if no stored preference.

---

## 3. DS-2 — Typography system

### 3.1 Font stack

| Token | Value | Notes |
|-------|-------|-------|
| `--font-family-sans` | `'Inter', system-ui, -apple-system, 'Segoe UI', Roboto, sans-serif` | Inter optional via Google Fonts or self-hosted woff2; **system-ui alone is acceptable** if font loading is deferred |
| `--font-family-mono` | `ui-monospace, 'Cascadia Code', 'Segoe UI Mono', monospace` | Due dates, IDs — rarely used in v2 |

**Recommendation:** Ship v2 with system stack only (zero latency). Add Inter in a follow-up PR if stakeholders want tighter word spacing.

### 3.2 Type scale

Base: `16px` (`1rem`). Scale ratio: **1.25 (major third)**.

| Token | Size | Line height | Weight | Letter-spacing | Usage |
|-------|------|-------------|--------|----------------|-------|
| `--font-display` | 1.875rem (30px) | 1.2 | 700 | -0.02em | Marketing only — not in app shell v2 |
| `--font-heading-lg` | 1.5rem (24px) | 1.3 | 600 | -0.01em | List title (`ListHeader`) |
| `--font-heading-md` | 1.125rem (18px) | 1.35 | 600 | 0 | Task title, section headings |
| `--font-body` | 1rem (16px) | 1.5 | 400 | 0 | Body, inputs, buttons |
| `--font-body-sm` | 0.875rem (14px) | 1.45 | 400 | 0 | Descriptions, labels, due dates |
| `--font-caption` | 0.75rem (12px) | 1.4 | 600 | 0.02em | Badges, metadata |
| `--font-label` | 0.875rem (14px) | 1.4 | 500 | 0 | Form field labels |

### 3.3 Typography rules

1. **One H1 per route** — app header "Todo" is brand chrome; page content starts at `heading-lg`.
2. **Task title** → `heading-md`; never bold body text as a heading substitute.
3. **Truncation** — single-line labels truncate with ellipsis; descriptions `word-break: break-word` (keep).
4. **Minimum interactive text** — 14px (`body-sm`); 12px only for badges/captions, never for buttons.
5. **Tabular nums** — add `font-variant-numeric: tabular-nums` on due dates when ISO sorting is shown.

### 3.4 v1 → v2 migration

| v1 token | v2 token |
|----------|----------|
| `--font-size-sm` | `--font-body-sm` |
| `--font-size-base` | `--font-body` |
| `--font-size-lg` | `--font-heading-md` |
| `--font-size-xl` | `--font-heading-lg` |
| `--line-height` | per-scale tokens above |

---

## 4. DS-3 — Spacing & layout grid

### 4.1 Base grid

**4px base unit.** All spacing is a multiple of 4px.

| Token | Value | px |
|-------|-------|-----|
| `--space-1` | 0.25rem | 4 |
| `--space-2` | 0.5rem | 8 |
| `--space-3` | 0.75rem | 12 |
| `--space-4` | 1rem | 16 |
| `--space-5` | 1.25rem | 20 |
| `--space-6` | 1.5rem | 24 |
| `--space-8` | 2rem | 32 |
| `--space-10` | 2.5rem | 40 |
| `--space-12` | 3rem | 48 |
| `--space-16` | 4rem | 64 |

**Alias migration** (support both during refactor):

| v1 | v2 |
|----|-----|
| `--space-xs` | `--space-1` |
| `--space-sm` | `--space-2` |
| `--space-md` | `--space-3` |
| `--space-lg` | `--space-4` |
| `--space-xl` | `--space-6` |

### 4.2 Breakpoints

| Name | Min-width | Layout behavior |
|------|-----------|-----------------|
| `mobile` | 0 | Single column; sidebar hidden (drawer in Phase 3) |
| `tablet` | 640px | Increased main padding; inline forms |
| `desktop` | 1024px | Sidebar visible (240px); main content fluid |
| `wide` | 1280px | Max content width enforced |

```css
--breakpoint-sm: 640px;
--breakpoint-md: 768px;
--breakpoint-lg: 1024px;
--breakpoint-xl: 1280px;
```

### 4.3 Page layout (FE-27)

```
┌─────────────────────────────────────────────────────────────┐
│ Header (56px height) — brand, theme toggle, user slot (P2)  │
├──────────────┬──────────────────────────────────────────────┤
│ Sidebar      │ Main content area                            │
│ 240px        │ max-width: 720px (--layout-content-max)      │
│ (placeholder │ centered in remaining space                  │
│  Phase 3)    │ padding: space-4 mobile / space-6 tablet+    │
│              │                                              │
└──────────────┴──────────────────────────────────────────────┘
```

| Token | Value | Notes |
|-------|-------|-------|
| `--layout-header-height` | 56px | Fixed |
| `--layout-sidebar-width` | 240px | Collapsed: 0 (hidden) on mobile |
| `--layout-content-max` | 45rem (720px) | v1 was 40rem — widen slightly for badges row |
| `--layout-gutter` | `--space-4` mobile, `--space-6` ≥640px | |

**Phase 1 sidebar:** render empty `<aside aria-label="Lists navigation" hidden>` or visible stub with "Lists coming soon" — preserves layout grid for Phase 3 without fake data.

### 4.4 Component spacing recipes

| Pattern | Spec |
|---------|------|
| Task card padding | `--space-4` vertical, `--space-6` horizontal |
| Gap between task cards | `--space-3` |
| Form field gap | `--space-1` label→input; `--space-4` between fields |
| Form section gap | `--space-6` |
| Badge gap in row | `--space-2` |
| Page title → content | `--space-6` |

---

## 5. DS-4 — Elevation & borders

### 5.1 Border radii

| Token | Value | Usage |
|-------|-------|-------|
| `--radius-sm` | 6px | Buttons, inputs, inline errors |
| `--radius-md` | 8px | Cards, task items, panels |
| `--radius-lg` | 12px | Modals, large cards |
| `--radius-full` | 9999px | Badges, avatars, swatches |

### 5.2 Border widths

| Token | Value | Usage |
|-------|-------|-------|
| `--border-width-default` | 1px | Cards, inputs, dividers |
| `--border-width-focus` | 2px | Focus ring (outline) |

**Divider rule:** use `border-top: 1px solid var(--color-border-default)` inside cards (task actions row). Do not use standalone `<hr>` in v2.

### 5.3 Shadow scale

| Level | Token | CSS value | Usage |
|-------|-------|-----------|-------|
| 0 | `--shadow-none` | `none` | Flat cards on page bg |
| 1 | `--shadow-sm` | `0 1px 2px rgba(15,23,42,0.06)` | Raised cards (optional hover) |
| 2 | `--shadow-md` | `0 4px 12px rgba(15,23,42,0.08)` | Dropdowns, popovers |
| 3 | `--shadow-lg` | `0 12px 32px rgba(15,23,42,0.12)` | Modals, confirm dialog |

Dark mode: increase opacity to `0.24` base — shadows are subtle on dark surfaces; prefer border `neutral-600` + `shadow-lg`.

### 5.4 Z-index stack

| Token | Value | Layer |
|-------|-------|-------|
| `--z-base` | 0 | Default |
| `--z-dropdown` | 100 | Select menus |
| `--z-sticky` | 200 | Header |
| `--z-modal` | 300 | Confirm dialog backdrop |
| `--z-toast` | 400 | Future toast stack |

---

## 6. DS-5 — Component specifications

All components live in `src/shared/components/ui/`. Each exports a single default component + documents props in a file header comment. Styling: **CSS module or co-located `.css`** matching existing project convention (co-located `.css` preferred for consistency).

### 6.1 Button (`Button.jsx`)

**Anatomy:** `[leading icon?] [label] [trailing icon?]`

| Prop | Type | Default | Notes |
|------|------|---------|-------|
| `variant` | `'primary' \| 'secondary' \| 'ghost' \| 'danger'` | `'secondary'` | |
| `size` | `'sm' \| 'md' \| 'lg'` | `'md'` | |
| `disabled` | `boolean` | `false` | |
| `type` | `'button' \| 'submit'` | `'button'` | |
| `children` | `node` | required | |

| Variant | Background | Text | Border | Hover |
|---------|------------|------|--------|-------|
| primary | `--color-action-primary` | white | none | `--color-action-primary-hover` |
| secondary | `--color-surface-default` | `--color-text-primary` | `--color-border-strong` | `--color-bg-subtle` |
| ghost | transparent | `--color-text-secondary` | none | `--color-bg-subtle` |
| danger | `--color-surface-default` | `--color-danger-text` | `--color-danger` border | `--color-danger-surface` |

| Size | Height | Padding (h) | Font |
|------|--------|-------------|------|
| sm | 32px | `--space-3` | `--font-body-sm` |
| md | 40px | `--space-4` | `--font-body` |
| lg | 44px | `--space-5` | `--font-body` |

**States:** default, hover, `:focus-visible` (2px ring offset 2px), disabled (opacity 0.5, `cursor: not-allowed`), loading (`aria-busy="true"`, spinner or ellipsis label).

**Migration targets:** `forms.css` `.form__actions button`, `TaskListItem.css` buttons, `StatePanel.css` buttons. Primary submit = `variant="primary"`.

### 6.2 Input (`Input.jsx`)

| Prop | Type | Notes |
|------|------|-------|
| `id`, `label` | string | Label required for a11y; `htmlFor` wired |
| `error` | string? | Sets `aria-invalid`, shows error text below |
| `hint` | string? | `aria-describedby` |
| `size` | `'md'` only in v2 | Height 40px |
| `...rest` | | `type`, `placeholder`, `value`, `onChange` |

**Visual:** bg `--color-surface-default`, border `--color-border-strong`, radius `--radius-sm`, padding `--space-2` `--space-3`. Error border `--color-danger-text`.

### 6.3 Select (`Select.jsx`)

Wrap native `<select>` in v2 (no custom dropdown yet).

Same props as Input. Chevron: Lucide `ChevronDown` 16px positioned end, `pointer-events: none`.

### 6.4 Textarea (`Textarea.jsx`)

Min-height 96px; resize vertical only. Same error/hint pattern as Input.

### 6.5 Badge (`Badge.jsx`)

| Prop | Type | Notes |
|------|------|-------|
| `variant` | `'neutral' \| 'info' \| 'success' \| 'warning' \| 'danger'` | |
| `children` | string | Required text label |

**Visual:** `font-caption`, padding `--space-1` `--space-2`, `radius-full`. Map task status/priority via helper:

```js
// shared/constants/badgeVariants.js
export function getStatusBadgeVariant(status) { /* 0→neutral, 1→info, 2→success, 3→neutral */ }
export function getPriorityBadgeVariant(priority) { /* 0→neutral, 1→info, 2→warning */ }
```

### 6.6 Card (`Card.jsx`)

| Prop | Type | Default |
|------|------|---------|
| `padding` | `'md' \| 'lg'` | `'md'` |
| `elevation` | `0 \| 1` | `0` |
| `as` | `'div' \| 'li'` | `'div'` |

**Visual:** bg surface-default, border default, radius-md, padding space-4 (md) or space-4/space-6 (lg). Task list item = `<Card as="li" padding="lg">`.

### 6.7 Skeleton (`Skeleton.jsx`)

| Prop | Type | Notes |
|------|------|-------|
| `width`, `height` | CSS string | |
| `variant` | `'text' \| 'rect' \| 'circle'` | |

**Visual:** `--color-bg-subtle` base; shimmer animation (§8) unless `prefers-reduced-motion`. Used by existing `ListHeaderSkeleton`, `TaskListSkeleton` — refactor to shared component.

### 6.8 Empty state (`EmptyState.jsx`)

| Prop | Type | Notes |
|------|------|-------|
| `title` | string | `heading-md` |
| `description` | string | `body-sm`, secondary color |
| `action` | `node` | Single primary Button |
| `icon` | Lucide component? | 48px, `--color-text-subtle`, stroke 1.5 |

**Layout:** center-aligned, max-width 320px, padding space-8. Replace copy in `TaskListEmpty`, home zero-state.

| Screen | Title | Description | CTA |
|--------|-------|-------------|-----|
| Empty task list | No tasks yet | Add your first task to this list. | Focus create-task form (no nav) |
| Home (no lists, P3) | Create your first list | Lists help you organize tasks by project or area. | Create list (primary) |

### 6.9 Alert / inline error (`Alert.jsx`)

Consolidate `InlineError` into variant of Alert:

| Variant | Border | Bg | Icon |
|---------|--------|-----|------|
| error | danger | danger-surface | `AlertCircle` |
| warning | warning | warning-surface | `AlertTriangle` |
| info | info | info-surface | `Info` |

`role="alert"` for error; `role="status"` for info. Keep compact inline size for form errors.

### 6.10 Confirm dialog (`ConfirmDialog.jsx`) — FE-28

**Pattern:** modal overlay + centered panel. Replace inline delete UI in `TaskListItem`.

| Prop | Type | Notes |
|------|------|-------|
| `open` | boolean | |
| `title` | string | e.g. "Delete task?" |
| `description` | string? | |
| `confirmLabel` | string | default "Confirm" |
| `cancelLabel` | string | default "Cancel" |
| `variant` | `'danger' \| 'default'` | danger confirm button for delete |
| `onConfirm` | async fn | disable while pending |
| `onCancel` | fn | |

**A11y (required):**

- `role="alertdialog"`, `aria-modal="true"`, `aria-labelledby`, `aria-describedby`
- Focus trap inside dialog
- Initial focus on cancel (destructive) or confirm (non-destructive) — **delete: focus Cancel**
- `Escape` → cancel
- Backdrop click → cancel
- Return focus to trigger on close

**Visual:** panel max-width 400px, padding space-6, shadow-lg, radius-lg. Backdrop `rgba(15,23,42,0.4)`.

### 6.11 Sidebar nav item (`SidebarNavItem.jsx`) — stub for FE-27

| Prop | Type | Notes |
|------|------|-------|
| `href` | string | |
| `label` | string | |
| `active` | boolean | |
| `icon` | Lucide? | optional |

**Visual:** height 40px, padding space-2 space-3, radius-sm, active bg `--color-brand-50`, text `--color-brand-700`. Full sidebar wired in Phase 3 (FE-36).

### 6.12 Toast (deferred)

Not in FE-26 scope. Document API for future: top-right stack, auto-dismiss 5s, `role="status"`.

---

## 7. DS-6 — Iconography

### 7.1 Library decision

**Selected: [Lucide React](https://lucide.dev)** (`lucide-react`)

| Criterion | Lucide | Heroicons | Phosphor |
|-----------|--------|-----------|----------|
| Bundle size | Tree-shakeable | Tree-shakeable | Heavier |
| Stroke consistency | 2px default | 1.5/2 mixed | Variable |
| React 19 support | Yes | Yes | Yes |
| License | ISC | MIT | MIT |

Install: `npm install lucide-react` — import only used icons per file.

### 7.2 Size grid

| Token | px | Stroke | Usage |
|-------|-----|--------|-------|
| `--icon-sm` | 16 | 2 | Inline with body-sm, select chevron |
| `--icon-md` | 20 | 2 | Buttons, nav items |
| `--icon-lg` | 24 | 2 | Header actions, empty states |
| `--icon-xl` | 48 | 1.5 | Empty state hero |

### 7.3 Icon usage rules

1. **Decorative icons** — `aria-hidden="true"`.
2. **Icon-only buttons** — require `aria-label`.
3. **Color** — inherit `currentColor`; never hard-code fill.
4. **Alignment** — `display: inline-flex; vertical-align: middle` in buttons.

### 7.4 Recommended icon mapping

| Action | Icon |
|--------|------|
| Add / create | `Plus` |
| Delete | `Trash2` |
| Edit | `Pencil` |
| Close / cancel | `X` |
| Confirm | `Check` |
| Theme toggle | `Sun` / `Moon` |
| Sidebar lists | `LayoutList` |
| Overdue indicator | `Clock` (optional, text label still required) |
| Error | `AlertCircle` |
| Retry | `RefreshCw` |

---

## 8. DS-7 — Motion

### 8.1 Duration tokens

| Token | Value | Usage |
|-------|-------|-------|
| `--duration-fast` | 150ms | Hover, opacity, color |
| `--duration-normal` | 250ms | Panel open, dialog enter |
| `--duration-slow` | 350ms | Sidebar slide (Phase 3) |

### 8.2 Easing

| Token | Value |
|-------|-------|
| `--ease-default` | `cubic-bezier(0.4, 0, 0.2, 1)` |
| `--ease-in` | `cubic-bezier(0.4, 0, 1, 1)` |
| `--ease-out` | `cubic-bezier(0, 0, 0.2, 1)` |

### 8.3 What animates

| Interaction | Property | Duration |
|-------------|----------|----------|
| Button hover | background-color | fast |
| Dialog open | opacity + scale(0.96→1) | normal |
| Dialog close | opacity | fast |
| Skeleton shimmer | background-position | 1.5s loop |
| Theme switch | background-color, color | fast |

### 8.4 Reduced motion

```css
@media (prefers-reduced-motion: reduce) {
  *, *::before, *::after {
    animation-duration: 0.01ms !important;
    animation-iteration-count: 1 !important;
    transition-duration: 0.01ms !important;
  }
}
```

**Do not animate:** list refetch/reorder, task status changes, error appearance (instant).

---

## 9. Engineering implementation guide

### 9.1 Suggested file structure

```
Todo.Web/
├── designdoc.md                 ← this file
├── docs/design/
│   └── tokens.md                ← FE-25: copy §2–§5 token tables (generated)
├── src/
│   ├── index.css                ← :root + [data-theme="dark"] tokens
│   ├── shared/
│   │   ├── components/
│   │   │   ├── ui/
│   │   │   │   ├── Button.jsx + .css
│   │   │   │   ├── Input.jsx + .css
│   │   │   │   ├── Select.jsx + .css
│   │   │   │   ├── Textarea.jsx + .css
│   │   │   │   ├── Badge.jsx + .css
│   │   │   │   ├── Card.jsx + .css
│   │   │   │   ├── Alert.jsx + .css
│   │   │   │   ├── EmptyState.jsx + .css
│   │   │   │   ├── Skeleton.jsx + .css
│   │   │   │   ├── ConfirmDialog.jsx + .css
│   │   │   │   ├── SidebarNavItem.jsx + .css
│   │   │   │   └── index.js
│   │   │   └── index.js         ← re-export ui + ErrorBoundary
│   │   └── constants/
│   │       └── badgeVariants.js
│   └── app/layout/
│       ├── AppLayout.jsx        ← FE-27 shell
│       └── AppLayout.css
```

### 9.2 Implementation order (recommended)

1. **Tokens** — `index.css` semantic aliases; keep v1 aliases temporarily to avoid big-bang CSS diff.
2. **Button + Input + Select** — migrate forms (`CreateTaskForm`, `HomePage` create list).
3. **Badge + Card** — migrate `TaskListItem`.
4. **Alert** — replace `InlineError` usages.
5. **ConfirmDialog** — migrate delete flow.
6. **EmptyState + Skeleton** — polish states.
7. **AppLayout** — header height, sidebar stub, content max width.
8. **Dark mode** — token block + toggle.
9. **Visual regression** — Playwright screenshots (FE-31).

### 9.3 Acceptance checklist (design sign-off)

- [ ] No raw hex in feature CSS (grep `#` in `src/features` except list color swatches)
- [ ] All interactive elements reachable by keyboard
- [ ] Focus visible on all controls
- [ ] Contrast spot-check: primary button, badge text, error alert
- [ ] `prefers-reduced-motion` honored
- [ ] 375px: no horizontal scroll on home + list page
- [ ] Delete flow uses `ConfirmDialog` with focus return
- [ ] Dark mode: task cards and badges readable

### 9.4 Visual regression viewports (FE-31)

| Viewport | Width | Height | Pages |
|----------|-------|--------|-------|
| Mobile | 375 | 812 | `/`, `/lists/:id` (seed data) |
| Desktop | 1280 | 800 | same |

Allow 0.5% pixel diff threshold for font rasterization.

---

## 10. Screen-level wire notes

### 10.1 Home (create list)

```
┌──────────────────────────────────────┐
│ [header: Todo | theme]               │
├──────────────────────────────────────┤
│                                      │
│   heading-lg: "Create a list"        │
│   body-sm secondary: subtitle        │
│                                      │
│   [ Input: List name ]               │
│   [ Input: Color ] (optional)        │
│   [ Button primary: Create list ]    │
│                                      │
└──────────────────────────────────────┘
```

### 10.2 List page

```
┌──────────────────────────────────────┐
│ [header]                             │
├──────────────────────────────────────┤
│ heading-lg + color swatch            │
│ body-sm: task count (future)         │
│ ─────────────────────────────────    │
│ [ Create task form — Card elevation 0]│
│ ─────────────────────────────────    │
│ [ Task Card ]                        │
│ [ Task Card ]                        │
│ [ EmptyState ] when zero tasks       │
└──────────────────────────────────────┘
```

**Hierarchy:** List title (heading-lg) > task title (heading-md) > description (body-sm) > metadata (caption badges).

---

## 11. Open questions & decisions log

| # | Question | Decision | Date |
|---|----------|----------|------|
| 1 | Custom font vs system? | System first; Inter optional | 2026-06-06 |
| 2 | CSS-in-JS vs CSS files? | Co-located `.css` (match codebase) | 2026-06-06 |
| 3 | Custom select vs native? | Native `<select>` v2 | 2026-06-06 |
| 4 | Illustrations in empty states? | Icon only (Lucide 48px), no illustration pack | 2026-06-06 |
| 5 | Content max width 40rem → 45rem? | **Yes** — badge row needs room | 2026-06-06 |

---

## 12. References

- [WCAG 2.1 Quick Reference](https://www.w3.org/WAI/WCAG21/quickref/)
- [Inclusive Components — Modal](https://inclusive-components.design/modals/)
- [Lucide Icon set](https://lucide.dev/icons/)
- Internal: `plan.md` Phase 1 (FE-25–FE-31), `docs/adr/001-frontend-type-safety.md`
- v1 source: `src/index.css`, `TaskListItem.css`, `shared/styles/forms.css`

---

## Revision history

| Date | Change |
|------|--------|
| 2026-06-06 | Initial design doc — DS-1 through DS-7 complete for engineering handoff |
