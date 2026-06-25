# 14 — Scope Control and No-Overengineering Rules

## Why this file exists

AI agents tend to overbuild. This project must stay inside PRN222. The goal is a complete academic web assignment, not a startup-grade production platform.

## Allowed complexity

Allowed because it maps to PRN222:

- Layered architecture.
- EF Core with SQL Server.
- MVC controllers/views.
- Razor Pages.
- Blazor dashboard.
- SignalR.
- Worker Service.
- Simple service/repository pattern.
- Basic role-based authorization.
- Mock AI API/client.

## Not allowed by default

Do not implement:

- Microservices.
- Cloud deployment.
- Kubernetes.
- RabbitMQ/Kafka.
- Distributed cache.
- CQRS/MediatR.
- Event sourcing.
- Complex DDD aggregates.
- Payment system.
- Real-time collaborative canvas drawing.
- Real ML training.
- Mobile app.
- React/Angular/Vue frontend.
- GraphQL.
- OAuth unless time remains.
- Full ASP.NET Identity migration unless the team specifically chooses it early.

## Optional only after core is done

- AI segmentation mock.
- Canvas region drawing UI.
- Export CSV/PDF.
- User profile images.
- Charts.
- Email sending.
- More advanced SignalR groups.

## Core first policy

Build in this order:

1. Data connection.
2. Series MVC.
3. Chapter/page MVC.
4. Task assignment.
5. Assistant Razor Pages.
6. Submission review.
7. Editor comment.
8. Board vote/ranking.
9. SignalR.
10. Blazor dashboard.
11. Worker Service.
12. Optional AI/polish.

## Kill criteria for a feature

Postpone a feature if:

- It does not map to PRN222.
- It requires a new technology not in the course.
- It blocks core demo.
- It requires external paid API.
- It requires redesigning the database.
- It cannot be explained in presentation.

## Presentation-first mindset

Every feature should help answer:

> Which PRN222 chapter does this prove?

If the answer is unclear, do not build it.
