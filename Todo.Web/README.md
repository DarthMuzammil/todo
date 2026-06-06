# Todo.Web

React frontend for the Todo app. See the [root README](../README.md) for setup, environment variables, and the local dev workflow.

## Quick start

```bash
npm install
npm run dev
```

Open http://localhost:5173 (with `Todo.Api` running at http://localhost:5167).

## Scripts

| Command | Description |
|---------|-------------|
| `npm run dev` | Vite dev server with `/api` proxy |
| `npm run build` | Production build (Vite) |
| `npm run test` | Vitest unit + component tests (jsdom) |
| `npm run test:watch` | Vitest in watch mode |
| `npm run e2e` | Playwright smoke test (starts API + dev server) |
| `npm run e2e:install` | Install Playwright Chromium browser |
| `npm run verify:build` | Fail if dev URLs are baked into `dist/` |
| `npm run lint` | ESLint |
| `npm run format` | Prettier write |
| `npm run format:check` | Prettier check |

## Environment

See `.env.example`. In local development, `.env.development` sets `VITE_API_BASE_URL=/api` so requests go through the Vite proxy.

The shared default (when no env var is set) is `http://localhost:5167` — see `src/shared/config/env.js`.

## Tech stack

Plain JavaScript (no TypeScript) + React 19 + Vite + React Router. Data fetching uses
plain React (`useState` + `useEffect` custom hooks in `src/features/lists/hooks/`) — no
data-fetching library. This is an intentionally simple baseline we will improve incrementally.

## MVP owner ID

`POST /api/lists` requires an `ownerId`. Auth is not implemented yet, so the app uses a
stand-in dev owner.

| Item | Detail |
|------|--------|
| **Strategy** | Fixed UUID in `src/shared/config/dev.js` |
| **Default** | `00000000-0000-0000-0000-000000000001` |
| **Override** | Set `VITE_DEV_OWNER_ID` in `.env.development` (see `.env.example`) |
| **Usage** | `HomePage` passes `DEV_OWNER_ID` when calling `createList` |
| **Replaced by** | real `ownerId` from the signed-in user (later auth work) |

All lists created in local dev belong to this owner until user authentication lands.
Not suitable for production multi-user scenarios.

## Testing

```bash
# Unit + component tests
npm run test

# E2E (requires .NET SDK — starts Todo.Api and Vite automatically)
npm run e2e:install   # first time only
npm run e2e
```

Tests mock `fetch` at the API module boundary. Component tests query by role (Testing Library).

## CI and staging

GitHub Actions workflows live in the repo root:

| Workflow | Trigger | What it does |
|----------|---------|--------------|
| `frontend-ci.yml` | PR + push to `main` | lint, format check, `npm run test`, build, Playwright E2E |
| `frontend-staging.yml` | push to `main` (`Todo.Web/**`) | staging build, `verify:build`, deploy to GitHub Pages |

**Staging setup:** configure these in the GitHub repo:

1. **Settings → Secrets and variables → Actions → Variables**
   - `STAGING_API_URL` — production API base URL baked into the bundle (e.g. `https://todo-api.example.com`)
2. **Settings → Pages** — set source to **GitHub Actions**
3. Optional **environment** named `staging` for deployment URL tracking

Staging builds set `VITE_API_BASE_URL` from `STAGING_API_URL` and fail `verify:build` if `localhost:5167` or `localhost:5173` appear in the output bundle.
