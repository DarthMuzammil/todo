# ADR 002 — Real-time conflict strategy

## Status

Accepted — 2026-06-06

## Context

Sprint H adds SignalR so collaborators viewing the same list see task and list changes without manual refresh. Multiple users may edit the same list concurrently.

## Decision

Use **last-write-wins (LWW)** with a monotonic **`Version`** column on `TodoList` and `TodoTask`.

- Each successful mutation increments `Version` on the affected entity.
- Realtime payloads include `version`.
- Clients apply an incoming event only when `incoming.version >= local.version` for that entity.
- No operational transform or CRDT in v1.

## Consequences

**Pros**

- Simple to implement and reason about.
- Fits todo-app expectations (status toggles, renames).
- Low server overhead — broadcast after commit, no merge engine.

**Cons**

- Simultaneous edits to the same field can silently overwrite without user warning.
- Delete + update races may show stale UI briefly until reconnect/refetch.

## Future work

- Surface conflict toasts when `incoming.version` is not `local.version + 1`.
- Optional field-level merge for description edits.
- Evaluate OT/CRDT only if collaborative editing becomes a core differentiator.
