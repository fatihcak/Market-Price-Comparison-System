# PROJECT_RULES.md

> **This file is the specific constitution for the Market Price Comparison System.**
> It combines the core philosophy of `AI_ASSIST.md` with the technical realities of this codebase.

---

## 1. Prime Directives
1.  **Strict Architecture Adherence:** You MUST follow the Controller -> Service -> Repository flow. Never bypass layers.
2.  **No Magic Strings:** All configuration keys, roles, and status strings must be constants or enums.
3.  **Security First:** No API keys or secrets in code. Use `appsettings.json` or Environment Variables.

## 2. Technology Standards

### Backend (.NET 8)
- **Async/Await:** All I/O bound operations must be `async Task`. usage of `.Result` or `.Wait()` is strictly prohibited.
- **DTOs:** Always use DTOs for API input/output. Never return Domain Entities directly (e.g., no `Product` in Controller response).
- **Validation:** Validate all DTOs before processing (Prefer FluentValidation if available, otherwise DataAnnotations).
- **Date/Time:** Always use `DateTime.UtcNow` for storage and logic. Convert to local time only at the Presentation layer (Frontend).

### Frontend (React + Vite)
- **Strict TypeScript:** No `any`. Define proper interfaces for all props and API responses.
- **Functional Components:** Use React Hooks. No Class components.
- **Tailwind:** Use utility classes for styling. Avoid inline `style={{}}` unless dynamic.
- **State:** Prefer local state for UI, and Context/Global state only for shared data (Auth, User Profile).

## 3. Workflow Rules
- **Before Coding:** Check `docs/activeContext.md` and `docs/systemPatterns.md`.
- **After Coding:** Update `docs/activeContext.md` with progress.
- **When Stuck:** Ask the user specific questions. Do not guess business logic.

## 4. Specific "Red Lines" for This Project
- ❌ **Do not introduce new Architectures** (e.g., don't add MediatR unless explicitly asked).
- ❌ **Do not changing naming conventions** (Stick to existing PascalCase/camelCase mix).
- ❌ **Do not commit commented-out code.**

## 5. Documentation
- Keep `docs/` folder up to date.
- If you add a new Service, update `docs/systemPatterns.md` if it introduces a new pattern.
