# ADR 001: Frontend type-safety approach

**Status:** Accepted  
**Date:** 2026-06-06  
**Context:** Sprint 4 (FE-17)

## Decision

Stay on **plain JavaScript (ESM)** for `Todo.Web`. Do not add TypeScript, JSDoc `@typedef` checks, or PropTypes in this phase.

## Options considered

| Option | Pros | Cons |
|--------|------|------|
| **Plain JS (chosen)** | Matches current plan and learning goals; smallest toolchain; explicit data flow stays readable | No compile-time safety; API contract drift caught at runtime or in tests |
| **JSDoc + `checkJs: true`** | Lightweight typing without a build step change; documents shapes in place | Noisy in a small codebase; `checkJs` still immature vs. TS for React |
| **PropTypes** | Runtime prop checks in development | Extra dependency and boilerplate; does not cover hooks, API layer, or non-component code |
| **TypeScript** | Strongest safety; best IDE support at scale | Contradicts current plan; migration cost; team is still building fluency in React patterns |

## Rationale

1. The product plan explicitly chose plain JS to keep data flow easy to learn, with optional hardening later.
2. Sprint 5 adds Vitest + RTL — API and hook tests will guard the HTTP boundary without static types.
3. Enums and DTO shapes are already centralized (`shared/constants/taskEnums.js`, `src/api/*`) which reduces scatter without a type system.
4. Revisit when: the frontend grows past ~15–20 interactive screens, multiple contributors ship in parallel, or API DTO churn causes repeated runtime bugs.

## Consequences

- Continue centralizing contracts in `src/api` functions and shared constants.
- Add automated tests at the API module boundary before introducing any type layer.
- If we adopt types later, prefer **TypeScript** over PropTypes/JSDoc — a single system rather than two partial ones.
