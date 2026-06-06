# Sprint 1 Reference — Status update + delete

Syntax + pseudocode only. No business logic. Fill in the real fields, endpoints,
and copy yourself. This maps to FE-1 → FE-4 in `plan.md`.

> What's already done for you: `updateTaskStatus()` and `deleteTask()` exist in
> `src/api/tasks.js`, and `useTasks()` already returns a `refetch`. Sprint 1 is
> about *calling* them from the UI and showing pending/error feedback.

> **Keep it simple:** no shared mutation hook. Use the same inline `useState` +
> `async` handler pattern as `CreateTaskForm` — one component, local state, call
> the API, then `refetch`.

---

## Files to add / change

Paths are under `Todo.Web/src/`.

### Already done — do not touch for Sprint 1

| File | Why |
|------|-----|
| `api/tasks.js` | `updateTaskStatus()`, `deleteTask()` already exist |
| `api/client.js` | `ApiClientError` + `request()` already exist |
| `features/lists/hooks/useTasks.js` | `refetch` already exposed |
| `features/lists/components/CreateTaskForm.jsx` | Reference only — copy its inline submit pattern |

### Change (required)

| File | Tickets | What to do |
|------|---------|------------|
| `features/lists/pages/ListPage.jsx` | FE-4 | Pass `listId` and `onChanged={refetchTasks}` into `TaskList` (same callback `CreateTaskForm` already uses as `onTaskCreated`) |
| `features/lists/components/TaskList.jsx` | FE-4 | Add props `listId`, `onChanged`; forward both to each `TaskListItem` |
| `features/lists/components/TaskListItem.jsx` | FE-1, FE-2, FE-3 | Add status control + delete button; inline `isPending` / `actionError`; call `updateTaskStatus` / `deleteTask`; call `onChanged()` on success |

### Add (optional)

| File | Tickets | What to do |
|------|---------|------------|
| — | — | **No new files required.** Status options can live inline in `TaskListItem` for now. |
| `features/lists/constants/taskStatus.js` *(or similar)* | FE-1 | Optional — extract a `STATUS_OPTIONS` array (mirror `PRIORITY_OPTIONS` in `validation/createTaskForm.js`). Sprint 2 (FE-5) will centralize labels/badges properly. |

### Wiring sketch (props flow)

```
ListPage
  listId, refetchTasks
    └─ TaskList          listId, onChanged={refetchTasks}
         └─ TaskListItem listId, task, onChanged
              ├─ status select/button  → updateTaskStatus(listId, task.id, { newStatus })
              └─ delete button         → deleteTask(listId, task.id)
                                         → onChanged() after success
```

### Out of scope for Sprint 1

| File | Why wait |
|------|----------|
| `api/tasks.js` | Endpoints already wired |
| `hooks/useList.js`, `hooks/useTasks.js` | Reads already work; refetch is enough |
| `validation/createTaskForm.js` | Create flow is done |
| New shared hooks under `shared/` | Keep writes inline per component |
| CSS / layout polish | Sprint 2–3 (FE-5–FE-13) |

---

## 0. Mental model

| | Query (`useList`, `useTasks`) | Write action (status / delete) |
|---|---|---|
| Runs | automatically in `useEffect` | only when the user acts |
| State | `status`, `data`, `error` | `isPending`, `actionError` (local to the control) |
| After success | nothing | call the parent's `refetch` callback |

Reads stay in hooks. Writes stay in the component that owns the button/select.

---

## 1. Inline pending + error state (the core pattern)

Mirror `CreateTaskForm`: local booleans/strings, no extracted hook.

```js
import { useState } from 'react'
import { SOME_API_FN } from '@/api/...'
import { ApiClientError } from '@/api/client'

// inside the component that owns the control:
const [isPending, setIsPending] = useState(false)
const [actionError, setActionError] = useState(null)

async function handleAction(/* args */) {
  setActionError(null)
  setIsPending(true)
  try {
    await SOME_API_FN(/* args */)
    onChanged?.()              // parent's refetch (FE-4)
  } catch (err) {
    const message =
      err instanceof ApiClientError ? err.body.error : 'Request failed'
    setActionError(message)
  } finally {
    setIsPending(false)
  }
}
```

```jsx
<button onClick={() => handleAction(/* ... */)} disabled={isPending}>
  {isPending ? 'Working…' : 'Label'}
</button>
{actionError && <span role="alert">{actionError}</span>}
```

---

## 2. FE-1 Update status — control syntax options

### Option A: a `<select>` (one control, all statuses)

```jsx
<label>
  <span className="sr-only">Status</span>
  <select
    value={currentValue}
    disabled={isPending}
    onChange={(e) => handleAction(Number(e.target.value))}
  >
    {STATUS_OPTIONS.map((opt) => (
      <option key={opt.value} value={opt.value}>
        {opt.label}
      </option>
    ))}
  </select>
</label>
```

### Option B: buttons (e.g. a "mark done" toggle)

```jsx
<button
  type="button"
  aria-pressed={isDone}
  disabled={isPending}
  onClick={() => handleAction(nextStatusValue)}
>
  {label}
</button>
```

> Note: backend expects `{ newStatus }` and enum values are **numbers**, so
> coerce with `Number(...)` from a `<select>`.

---

## 3. FE-2 Delete — confirmation pseudocode

Two common syntaxes. Pick one for now.

### Quick path: native confirm

```js
async function handleDelete() {
  const ok = window.confirm('Delete this task?')
  if (!ok) return
  // same try/catch/finally block as §1, calling delete API fn
}
```

### Better path: confirm state (no extra libs)

```
state: confirming = false

render:
  if confirming:
    "Are you sure?"  [Confirm]  [Cancel]
       Confirm -> run handleDelete (§1 pattern), then confirming = false
       Cancel  -> confirming = false
  else:
    [Delete] -> confirming = true
```

```jsx
{confirming ? (
  <>
    <button onClick={confirmDelete} disabled={isPending}>Confirm</button>
    <button onClick={() => setConfirming(false)} disabled={isPending}>Cancel</button>
  </>
) : (
  <button onClick={() => setConfirming(true)}>Delete</button>
)}
```

Accessibility to consider (rules.md): move focus to the confirm button when it
appears; `Esc` cancels.

---

## 4. FE-3 Pending + error feedback (the rules)

- **Disable** the control while the request is in flight: `disabled={isPending}`.
- **Never render the raw error object.** Map `ApiClientError` to a string in the
  `catch` block (see §1).
- Show the message near the action: `{actionError && <span role="alert">…</span>}`.

---

## 5. FE-4 Shared refetch — wiring direction

You already do this for create-task. Reuse the *same* `refetch`. Don't add a
second source of truth.

```
ListPage
  const { tasks, refetch } = useTasks(listId)
  <TaskList tasks onChanged={refetch} />        // pass it down
        TaskListItem onChanged={onChanged}      // forward it
              status/delete: await API fn -> onChanged()
```

Props down, events up: the page owns the data and the refetch; children only
call the callback after a successful write.

---

## 6. Where to look (research, don't copy)

- React: "You Might Not Need an Effect" (writes are events, not effects),
  `useState`, lifting state up.
- MDN: `<select>` / `<option>`, `window.confirm`, `disabled` attribute,
  `aria-pressed`, `role="alert"` (live regions).
- Your own code: `CreateTaskForm.jsx` (inline submit pattern),
  `useTasks.js` (refetch), `client.js` (`ApiClientError`), `tasks.js` (API fns).

---

## 7. Done checklist (DoD per plan.md)

- [ ] Status changes via UI; list refreshes on success (FE-1)
- [ ] Delete asks for confirmation; cancel is a no-op (FE-2)
- [ ] Controls disable while pending; errors show friendly text (FE-3)
- [ ] All writes call the page's existing `refetch` (FE-4)
- [ ] ESLint + Prettier clean; `npm run build` passes
