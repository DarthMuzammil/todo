# Todo Frontend — Product & Engineering Plan

Production-ready React frontend for the Todo application, consuming `Todo.Api` and aligned with the long-term vision in `rules.md` (multi-user workspaces, RBAC, SignalR).

---

## Current state

| Layer | Status |
|-------|--------|
| `Todo.Api` | Lists + tasks REST endpoints (JSON file persistence); CORS enabled for `http://localhost:5173` |
| `Todo.Web` | React 19 + Vite + TypeScript scaffold — Sprint 1 complete (DevEx, folder structure, proxy, lint/format) |

**Sprint 1 done.** Next: Sprint 2 — API client & application shell.

---

## Planning assumptions

- **Sprint length:** 2 weeks
- **Team:** 2 frontend engineers, 0.5 designer (shared), 1 backend engineer (API/CORS support as needed)
- **Velocity:** ~20–25 story points per sprint (team total)
- **Definition of Done (DoD):** code reviewed, typed (TypeScript strict), accessible (WCAG 2.1 AA for touched UI), unit tests for logic, PR merged to `main`, deployed to staging, product owner sign-off on acceptance criteria
- **Ticket prefix:** `FE-` (frontend). Backend dependencies prefixed `BE-`.
- **Out of scope for v1 frontend:** billing, mobile native apps, offline-first PWA (may be revisited in a later epic)

---

## Epic overview

| Epic | Name | Goal | Target phase |
|------|------|------|--------------|
| FE-EPIC-01 | Foundation & DevEx | Runnable, consistent dev workflow for all contributors | Phase 1 |
| FE-EPIC-02 | API integration layer | Typed, testable client for `Todo.Api` | Phase 1 |
| FE-EPIC-03 | App shell & navigation | Layout, routing, list context | Phase 1 |
| FE-EPIC-04 | List management | Create, view, and switch todo lists | Phase 1 |
| FE-EPIC-05 | Task management | Full task lifecycle in the UI | Phase 1 |
| FE-EPIC-06 | Design system & UX polish | Consistent, responsive, accessible UI | Phase 1 |
| FE-EPIC-07 | Resilience & feedback | Loading, empty, and error states; optimistic UX where safe | Phase 1 |
| FE-EPIC-08 | Testing & quality gates | Automated confidence before release | Phase 1 |
| FE-EPIC-09 | Auth & identity | Login, session, protected routes | Phase 2 |
| FE-EPIC-10 | Collaboration & real-time | Live updates, presence, notifications | Phase 2 |
| FE-EPIC-11 | Production release | CI/CD, environments, monitoring, launch | Phase 1 → GA |

---

## Phase 1 — MVP (Sprints 1–6)

Goal: A user can open the web app, pick or create a list, and manage tasks end-to-end against the existing API.

---

### Sprint 1 — Scaffold & developer experience

**Sprint goal:** Every engineer can clone, install, run frontend + API locally, and merge via a standard PR workflow.

| ID | Type | Title | Points | Acceptance criteria |
|----|------|-------|--------|---------------------|
| FE-101 | Story | Hello-world scaffold (React + Vite + TS) | 1 | `Todo.Web` builds and shows static greeting; no API calls |
| FE-102 | Task | Monorepo documentation in README | 2 | Root README documents `dotnet run --project Todo.Api` and `npm run dev` in `Todo.Web`; documents `data/` path requirement |
| FE-103 | Task | Environment variable convention | 2 | `VITE_API_BASE_URL` documented; `.env.example` added; dev default points to `http://localhost:5167` |
| FE-104 | Task | ESLint + Prettier baseline | 3 | Lint script passes; format on save documented; CI-ready `npm run lint` |
| FE-105 | Task | Path aliases & folder structure | 3 | Agreed structure: `src/app`, `src/features`, `src/shared`, `src/api` (folders created, barrel exports optional) |
| FE-106 | Task | Vite dev proxy for API | 2 | `vite.config.ts` proxies `/api` → `Todo.Api` to avoid CORS during local dev |
| BE-101 | Task | Enable CORS for frontend origin | 2 | `Todo.Api` allows `http://localhost:5173` (and staging URL later); preflight succeeds |

**Sprint 1 deliverable:** Runnable scaffold with documented local dev loop and API connectivity path.

---

### Sprint 2 — API client & application shell

**Sprint goal:** Typed API module exists; app has routing skeleton and global providers.

| ID | Type | Title | Points | Acceptance criteria |
|----|------|-------|--------|---------------------|
| FE-201 | Story | API types mirroring backend models | 5 | TypeScript interfaces for `TodoList`, `TodoTask`, `TaskStatus`, `Priority`, API error shape |
| FE-202 | Story | HTTP client wrapper | 5 | `fetch`-based client with base URL, JSON parse, maps non-2xx to typed `ApiError` |
| FE-203 | Task | API module: lists endpoints | 3 | `getListById`, `createList` functions calling `GET/POST /api/lists` |
| FE-204 | Task | API module: tasks endpoints | 5 | `getTasksByListId`, `createTask`, `updateTaskStatus`, `deleteTask` |
| FE-205 | Story | React Router setup | 3 | Routes: `/` (home), `/lists/:listId` (placeholder); 404 page |
| FE-206 | Task | App layout component | 3 | Header with app name; main content area; responsive container |
| FE-207 | Task | React Query (TanStack Query) setup | 3 | `QueryClientProvider` wired; default stale/retry options documented |

**Sprint 2 deliverable:** Shell app that can call the API from the browser (verified via devtools or a temporary debug page — no product UI yet).

---

### Sprint 3 — List management

**Sprint goal:** User can create a list and navigate into it.

| ID | Type | Title | Points | Acceptance criteria |
|----|------|-------|--------|---------------------|
| FE-301 | Story | Home / list picker page | 5 | Shows message when no list selected; input to create list (title, optional color); uses `POST /api/lists` |
| FE-302 | Story | Navigate to list by ID | 3 | After create, redirect to `/lists/:id`; direct URL load fetches list via `GET /api/lists/:id` |
| FE-303 | Story | List header on list page | 3 | Displays list title and color; handles 404 from API gracefully |
| FE-304 | Task | List creation form validation | 2 | Title required, max length enforced client-side; inline error messages |
| FE-305 | Task | Loading & skeleton states for list | 2 | Spinner or skeleton while `getListById` in flight |
| FE-306 | Spike | Owner ID strategy for MVP | 2 | Document interim approach (hardcoded dev `ownerId` in config vs. random per session); implement chosen approach |

**Sprint 3 deliverable:** User can create a list and land on an empty list view.

**Note:** Backend has no `GET /api/lists` (all lists) endpoint yet. **BE-201** (list all lists) is a recommended follow-up; until then, navigation relies on create response or manual URL/ID entry.

---

### Sprint 4 — Task list & CRUD

**Sprint goal:** User can view, add, update status, and delete tasks within a list.

| ID | Type | Title | Points | Acceptance criteria |
|----|------|-------|--------|---------------------|
| FE-401 | Story | Task list view | 5 | `GET /api/lists/:listId/tasks` rendered as list; empty state when no tasks |
| FE-402 | Story | Create task form | 5 | Fields: title (required), description, priority, due date; submits to `POST` |
| FE-403 | Story | Update task status | 3 | Control per task (dropdown or buttons) calls `PATCH .../status` |
| FE-404 | Story | Delete task | 3 | Delete with confirmation modal; calls `DELETE`; list refreshes |
| FE-405 | Task | Priority & status badges | 2 | Visual distinction for `Priority` and `TaskStatus` enums |
| FE-406 | Task | Due date display | 2 | Formats due date; shows overdue styling when past due and not Done |
| FE-407 | Task | Query invalidation after mutations | 3 | React Query cache updates correctly after create/update/delete |

**Sprint 4 deliverable:** Core task workflow complete against existing API.

---

### Sprint 5 — UX polish & resilience

**Sprint goal:** App feels production-quality for a single-user MVP: responsive, accessible, and informative when things fail.

| ID | Type | Title | Points | Acceptance criteria |
|----|------|-------|--------|---------------------|
| FE-501 | Story | Global error boundary | 3 | Uncaught render errors show recovery UI, not blank screen |
| FE-502 | Story | Toast / inline error feedback | 3 | API failures surface user-friendly messages (not raw JSON) |
| FE-503 | Story | Empty & error states (lists & tasks) | 3 | Dedicated components for empty list, failed load, retry action |
| FE-504 | Task | Responsive layout (mobile + desktop) | 5 | Usable on 375px width; no horizontal scroll on primary flows |
| FE-505 | Task | Keyboard navigation & focus management | 3 | Forms and task actions reachable via keyboard; focus trapped in modals |
| FE-506 | Task | Basic design tokens (CSS variables) | 3 | Colors, spacing, typography tokens; light mode only for MVP |
| FE-507 | Task | Component library decision | 2 | ADR: headless (Radix) + custom styles vs. MUI vs. shadcn — record decision in `docs/adr/` |

**Sprint 5 deliverable:** MVP UI ready for internal dogfooding.

---

### Sprint 6 — Testing, CI & staging deploy

**Sprint goal:** Automated checks gate merges; staging environment hosts the frontend.

| ID | Type | Title | Points | Acceptance criteria |
|----|------|-------|--------|---------------------|
| FE-601 | Story | Unit tests for API client & mappers | 5 | MSW or fetch mock; covers success and error paths |
| FE-602 | Story | Component tests for critical flows | 5 | Testing Library: create list, create task, update status (happy path) |
| FE-603 | Task | E2E smoke test (Playwright) | 5 | One flow: create list → add task → mark done; runs in CI |
| FE-604 | Task | GitHub Actions: frontend CI | 3 | `lint`, `typecheck`, `test`, `build` on PR |
| FE-605 | Task | Staging deployment pipeline | 5 | Frontend deploys to staging (e.g. Azure Static Web Apps, Vercel, or nginx container); env `VITE_API_BASE_URL` set |
| FE-606 | Task | Production build verification | 2 | `npm run build` artifact size budget documented; no dev URLs in bundle |

**Sprint 6 deliverable:** Phase 1 MVP releasable to staging with automated regression safety net.

---

## Phase 2 — Multi-user & collaboration (Sprints 7–9)

Goal: Align frontend with `rules.md` vision — authenticated users, workspaces, roles, live updates.

---

### Sprint 7 — Authentication & session

**Sprint goal:** Only signed-in users access the app; API calls carry credentials.

| ID | Type | Title | Points | Acceptance criteria |
|----|------|-------|--------|---------------------|
| BE-301 | Story | Auth API (login, refresh, me) | 8 | JWT or cookie session; `GET /api/users/me` |
| FE-701 | Story | Login & logout pages | 5 | Form validation; redirect after login; logout clears session |
| FE-702 | Story | Auth context & protected routes | 5 | Unauthenticated users redirected to `/login` |
| FE-703 | Task | Attach auth header to API client | 3 | Token/cookie injected on every request; 401 triggers re-login |
| FE-704 | Task | Owner ID from authenticated user | 2 | `createList` uses real `ownerId` from `/me` |
| FE-705 | Spike | Auth provider choice | 2 | ADR: ASP.NET Identity vs. Auth0 vs. Entra — documented |

---

### Sprint 8 — Workspaces, lists sidebar & RBAC

**Sprint goal:** User sees all their lists; permissions enforced in UI.

| ID | Type | Title | Points | Acceptance criteria |
|----|------|-------|--------|---------------------|
| BE-401 | Story | `GET /api/lists` for current user | 5 | Returns owned/shared lists |
| BE-402 | Story | Workspace membership & roles API | 8 | Owner, Admin, Member, Viewer per `rules.md` |
| FE-801 | Story | Sidebar list navigation | 5 | All lists fetched; active list highlighted; create list from sidebar |
| FE-802 | Story | Role-based UI guards | 5 | Viewer: read-only; Member: create/edit own tasks; Admin/Owner: full control |
| FE-803 | Task | Shared list indicator | 2 | Visual badge when list is shared |
| FE-804 | Task | User settings page (profile) | 3 | Display name, email (read-only from API) |

---

### Sprint 9 — Real-time updates (SignalR)

**Sprint goal:** Changes from other users appear without manual refresh.

| ID | Type | Title | Points | Acceptance criteria |
|----|------|-------|--------|---------------------|
| BE-501 | Story | SignalR hub for list/task events | 8 | Broadcast create/update/delete/status change |
| FE-901 | Story | SignalR client connection | 5 | Connect on list page; reconnect with backoff |
| FE-902 | Story | Live task list sync | 8 | Incoming events update React Query cache |
| FE-903 | Task | Connection status indicator | 2 | Subtle “live” / “reconnecting” UI |
| FE-904 | Task | Conflict handling strategy | 3 | ADR: last-write-wins vs. merge; document behavior |

---

## Phase 3 — Production GA (Sprint 10+)

**Sprint goal:** Harden, observe, and launch to production.

| ID | Type | Title | Points | Acceptance criteria |
|----|------|-------|--------|---------------------|
| FE-1001 | Story | Production deployment & rollback | 5 | Blue/green or slot deploy; documented rollback |
| FE-1002 | Task | CDN + cache headers for static assets | 3 | Immutable hashed assets; HTML no-cache |
| FE-1003 | Task | CSP & security headers | 3 | Content-Security-Policy aligned with API and SignalR origins |
| FE-1004 | Task | Frontend observability | 5 | Error tracking (e.g. Sentry); basic Web Vitals reporting |
| FE-1005 | Story | Accessibility audit | 5 | axe / manual audit; critical issues fixed |
| FE-1006 | Story | Performance budget | 3 | LCP < 2.5s on staging; bundle split for routes |
| FE-1007 | Task | Runbook & on-call docs | 2 | How to diagnose API vs. UI failures |
| FE-1008 | Story | Dark mode (optional) | 3 | Respects `prefers-color-scheme` or toggle |

---

## Backlog — future epics (post-GA)

Tickets are intentionally coarse; break down when prioritized.

### FE-EPIC-12 — Task detail & rich editing
- Task detail drawer/page (description markdown, comments)
- Subtasks (`ParentTaskId`)
- Drag-and-drop reorder (`SortOrder`)
- Tags and filters

### FE-EPIC-13 — Search & productivity
- Full-text search across tasks
- Filters: status, priority, due date, assignee
- Keyboard shortcuts (e.g. `n` new task, `/` search)
- Bulk actions

### FE-EPIC-14 — Notifications & activity
- In-app notification center
- Activity log UI per list/task
- Email digest integration (if backend provides)

### FE-EPIC-15 — Internationalization & locale
- i18n framework (react-i18next)
- Date/number formatting per locale
- RTL layout support

---

## Cross-cutting non-functional requirements

| Area | Requirement |
|------|-------------|
| **Accessibility** | WCAG 2.1 AA on all customer-facing flows |
| **Browser support** | Last 2 versions of Chrome, Firefox, Safari, Edge |
| **Performance** | Initial JS bundle < 250 KB gzip (excluding lazy routes) |
| **Security** | No secrets in frontend; sanitize user-generated HTML if rich text added |
| **API contract** | Frontend types updated when `Todo.Api` DTOs change; consider OpenAPI codegen (FE-1101 spike) |

---

## Dependency map (frontend ↔ backend)

| Frontend ticket | Backend dependency |
|-----------------|-------------------|
| FE-201–204 | Existing `Todo.Api` controllers |
| FE-301, FE-801 | **BE-201** `GET /api/lists` (not implemented yet) |
| FE-701–704 | **BE-301** Auth endpoints |
| FE-802 | **BE-402** RBAC |
| FE-901–902 | **BE-501** SignalR hub |

---

## Suggested ticket workflow (Jira / Linear / Azure DevOps)

1. **Epic** → linked to phase milestone (MVP, Collaboration, GA)
2. **Story** → demonstrable user value; fits in one sprint
3. **Task / Spike** → technical work under a story or epic
4. **Labels:** `frontend`, `backend`, `blocked`, `tech-debt`
5. **PR title:** `FE-401: Task list view`

---

## Quick start (current scaffold)

```bash
# Terminal 1 — API (from repo root)
dotnet run --project Todo.Api

# Terminal 2 — Frontend
cd Todo.Web
npm install
npm run dev
```

Open `http://localhost:5173` — you should see **Hello, Todo**.

---

## Revision history

| Date | Change |
|------|--------|
| 2026-06-05 | Initial plan: `Todo.Web` scaffold + Phase 1–3 epic/sprint breakdown |
