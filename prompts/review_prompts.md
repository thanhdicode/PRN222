# Review Prompts

## PRN222 alignment review

```text
Review this implementation for PRN222 alignment. Check whether it demonstrates MVC/Razor Pages/Blazor/SignalR/Worker/EF Core/DI/async as intended. Identify anything overengineered or unrelated to the course. Provide concrete fixes.
```

## Architecture review

```text
Review the current solution architecture. Ensure controllers/PageModels are thin, services contain business logic, repositories/DAO handle data access, and DbContext is registered correctly. Do not suggest microservices/CQRS/event sourcing.
```

## Demo readiness review

```text
Review whether the current project can support the final demo flow: series -> board vote -> page/task -> assistant submission -> mangaka review -> editor comment -> ranking -> SignalR -> Worker -> Blazor dashboard. List blockers and fastest fixes.
```

## Security/basic validation review

```text
Review role access and input validation for this PRN222 project. Focus on simple practical issues: unauthorized screens, missing required validation, unsafe upload, duplicate board vote, hardcoded IDs. Do not propose enterprise security architecture.
```
