# 10 — Testing and QA Checklist

## Manual test strategy

This project can pass PRN222 with manual tests if the demo is stable. Unit tests are useful but not mandatory unless the instructor requires them.

## Smoke test checklist

Run this after each major coding session.

### Database

- [ ] App connects to `MangaWorkflowDB`.
- [ ] Series list loads.
- [ ] Task list loads.
- [ ] Notifications list loads.
- [ ] Dashboard views load.

### Auth

- [ ] Can login as Admin.
- [ ] Can login as Mangaka.
- [ ] Can login as Assistant.
- [ ] Can login as Editor.
- [ ] Can login as Board.
- [ ] Menus change by role.

### MVC

- [ ] Series index loads.
- [ ] Series details loads.
- [ ] Create series works.
- [ ] Edit series works.
- [ ] Submit series changes status.
- [ ] Board vote works.
- [ ] Ranking import works.

### Razor Pages

- [ ] Assistant task inbox loads.
- [ ] Task detail loads.
- [ ] Submission form works.
- [ ] Assistant earnings page loads.

### Blazor

- [ ] Dashboard component loads.
- [ ] Chapter progress appears.
- [ ] Ranking appears.
- [ ] Task counts appear.

### SignalR

- [ ] Browser connects to hub.
- [ ] Creating task pushes notification.
- [ ] Submitting task pushes notification.
- [ ] Reviewing submission pushes notification.

### Worker Service

- [ ] Worker starts.
- [ ] Worker logs to console.
- [ ] Worker writes `BackgroundJobLogs`.
- [ ] Overdue scanner changes at least one task in controlled test.

## Data validation checklist

- [ ] Required fields cannot be blank.
- [ ] Deadline field accepts valid date/time.
- [ ] File URL or upload field is validated.
- [ ] Price cannot be negative.
- [ ] Region width/height must be positive.
- [ ] Vote duplicate is prevented.

## Security checklist

- [ ] Assistant cannot review another assistant's task as Mangaka.
- [ ] Mangaka cannot access Board-only screens.
- [ ] Board cannot edit manga pages.
- [ ] Editor cannot create Board vote.
- [ ] Admin-only logs are hidden from normal users.

## Demo stability checklist

Before final demo:

- [ ] Re-run SQL v2 + v3 in a clean database.
- [ ] Run app.
- [ ] Run worker or worker demo button.
- [ ] Open two browser windows for SignalR demo.
- [ ] Verify `Neon Samurai`, `Starfall Idol Manga`, and `Hollow Brush` data exist.
- [ ] Prepare fallback screenshots in case live SignalR fails.

## Common bug guide

### EF scaffold namespace errors

Fix project references and namespaces first. Do not rewrite schema.

### SignalR script not found

Install client library or use CDN/local static file. Ensure `_Layout.cshtml` references it.

### Worker cannot resolve DbContext

Use `IServiceScopeFactory`. Do not inject scoped DbContext directly into singleton worker.

### Login role missing

Check `UserRoles` and `Roles` seed data.

### Duplicate board vote error

This is expected because database has unique constraint `(SeriesId, BoardMemberId)`. Show validation message.
