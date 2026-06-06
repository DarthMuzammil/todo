# Todo.Web folder structure

## Boundaries

| Path | Owns |
|------|------|
| `src/app/` | Routing, layout shell, global pages (404) |
| `src/api/` | HTTP client + endpoint functions (API contract) |
| `src/features/lists/` | List/task UI, feature hooks, feature utils |
| `src/shared/` | Cross-feature hooks, components, constants, utils |

**Rule:** features import from `shared/` and `api/`; they do not import from other features (only one feature today).

## Barrel exports

| Barrel | Exports |
|--------|---------|
| `features/lists/index.js` | `HomePage`, `ListPage`, `useList`, `useTasks` |
| `shared/components/index.js` | `ErrorBoundary`, `InlineError` |
| `shared/hooks/index.js` | `useAsync` |
| `shared/utils/index.js` | `getErrorMessage` |
| `shared/constants/index.js` | Task enum maps + label helpers |

Import barrels at app boundaries (e.g. `App.jsx`); prefer direct imports inside a feature to keep dependency graphs obvious.
