---
name: MangaWorkflow PRN222 Rule
description: Keep AI coding aligned with PRN222 scope and MangaWorkflow architecture.
---

Review all code changes for the following:

- Does the change support a PRN222 topic?
- Does it keep MVC/Razor Pages/Blazor/SignalR/Worker responsibilities separate?
- Does it avoid overengineering?
- Does it use async EF Core calls?
- Does it avoid hardcoding database IDs?
- Does it preserve the demo flow and seed data?
- Does it update documentation if behavior changed?

Reject changes that introduce microservices, CQRS, real AI training, payment, mobile apps, or non-course technologies without explicit approval.
