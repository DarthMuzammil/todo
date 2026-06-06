# Todo

A full-stack todo application: ASP.NET Web API backend (`Todo.Api`) and React frontend (`Todo.Web`).

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- [Node.js 20+](https://nodejs.org/) (for the frontend)

## Repository layout

| Path | Purpose |
|------|---------|
| `Todo.Api` | ASP.NET Web API — lists and tasks REST endpoints |
| `Todo.Web` | React 19 + Vite + TypeScript frontend |
| `Todo.Domain` / `Application` / `Infrastructure` | Backend layers (CQRS, repositories) |
| `Todo.Console` | Console client |
| `data/` | JSON persistence for the API (`tasks.json`, `lists.json`) |

## Local development

Run both services from the **repository root** so the API can resolve the `data/` folder.

### 1. Start the API

```bash
dotnet run --project Todo.Api
```

The API listens on **http://localhost:5167** (see `Todo.Api/Properties/launchSettings.json`).

The `data/` directory must exist at the repo root. It is checked in with empty JSON arrays; the API reads and writes `data/tasks.json` and `data/lists.json`.

### 2. Start the frontend

```bash
cd Todo.Web
npm install
npm run dev
```

The dev server runs at **http://localhost:5173**.

### 3. API connectivity

Two options for calling the API from the frontend:

| Mode | `VITE_API_BASE_URL` | Notes |
|------|---------------------|-------|
| **Vite proxy (recommended for local dev)** | `/api` | Vite proxies `/api` → `http://localhost:5167` — no CORS issues |
| **Direct** | `http://localhost:5167` | Requires CORS (enabled on the API for `http://localhost:5173`) |

Copy `.env.example` to `.env.local` and adjust if needed:

```bash
cp .env.example .env.local
```

## Frontend scripts (`Todo.Web`)

| Command | Description |
|---------|-------------|
| `npm run dev` | Start Vite dev server |
| `npm run build` | Production build |
| `npm run lint` | ESLint (CI-ready) |
| `npm run format` | Prettier write |
| `npm run format:check` | Prettier check (CI-ready) |

### Format on save (VS Code / Cursor)

Install the [Prettier extension](https://marketplace.visualstudio.com/items?itemName=esbenp.prettier-vscode). Project settings in `.vscode/settings.json` enable format-on-save for TypeScript and TSX files.

## Source structure (`Todo.Web/src`)

```
src/
  app/        — routing, layout, root App component
  features/   — feature modules (lists, tasks, …)
  shared/     — reusable components, hooks, config
  api/        — typed HTTP client and DTOs
```

Import aliases: `@/app`, `@/features`, `@/shared`, `@/api` (configured in `vite.config.ts` and `tsconfig.app.json`).

## Backend tests

```bash
dotnet test
```

## Further reading

- `rules.md` — architecture decisions and learning guide
- `plan.md` — frontend product and sprint plan
