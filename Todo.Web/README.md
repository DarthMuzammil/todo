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
| `npm run build` | Type-check and production build |
| `npm run lint` | ESLint |
| `npm run format` | Prettier write |
| `npm run format:check` | Prettier check |

## Environment

See `.env.example`. In local development, `.env.development` sets `VITE_API_BASE_URL=/api` so requests go through the Vite proxy.

The shared default (when no env var is set) is `http://localhost:5167` — see `src/shared/config/env.ts`.

## MVP owner ID (FE-306)

`POST /api/lists` requires an `ownerId`. Auth is not implemented yet, so the app uses a
stand-in dev owner.

| Item | Detail |
|------|--------|
| **Strategy** | Fixed UUID in `src/shared/config/dev.ts` |
| **Default** | `00000000-0000-0000-0000-000000000001` |
| **Override** | Set `VITE_DEV_OWNER_ID` in `.env.development` (see `.env.example`) |
| **Usage** | `HomePage` passes `DEV_OWNER_ID` when calling `createList` |
| **Replaced by** | FE-704 — real `ownerId` from the signed-in user |

All lists created in local dev belong to this owner until user authentication lands.
Not suitable for production multi-user scenarios.
