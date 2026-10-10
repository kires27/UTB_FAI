# Memory

# Preferences
- Verify UI via authenticated curl (cookie + `__RequestVerificationToken`); cookie dies on web restart → re-login with the test account.

# Decisions
- Decline is final: attendee `Status=Declined`, row kept, whole series; owner gets EventUpdate notification; `AcceptInvitationAsync` refuses declined.
- Attendees cannot edit/delete; Created/Updated shown only to admin.
- Details Back returns to origin via `returnView`/`returnDate` (whitelisted in controller).
- Daily/weekly events use 80% column width; overlap lanes split that 80%.
- Monthly day click prefills current local time (`date=YYYY-MM-DDThh:mm`).
- Recurrence expands in-memory (`Application/Helpers/RecurrenceExpander.cs`); per-occurrence edits in `EventOccurrenceExceptions`.
- Create/Edit share one form (`wwwroot/js/event-form.js`); hidden ISO StartTime/EndTime.
- Create takes owner from claims; Edit ignores posted OwnerId.
- Weeks start Monday; grep `firstDayOfWeek` before touching week math.
- All ViewModels live in `CalendarApp.Application/Models` (namespace `CalendarApp.Application.Models`). Only `ErrorViewModel` stays in `CalendarApp.Web/Models`, used only by `HomeController.Error` (keeps `using CalendarApp.Web.Models`).
- Login accepts username or email (`AccountAppService.LoginAsync` resolves email → username).

# Gotchas & Lessons
- Never `$(document).ready`: jQuery loads after RenderBody, script dies silently. Use DOMContentLoaded.
- Hot reload: `dotnet watch` applies most changes; restart only when curl shows stale HTML.
- Host has no dotnet: `docker compose run --rm migrator bash -c "dotnet build CalendarApp.Web"`.
- Normalize editData after JSON.parse; a missing key silently kills the feature.
- `List<int>` binding needs separate form entries per id.
- EF delete must `Include` children or MySQL FK throws; catch-then-redirect hides it.
- Container `TZ: Europe/Prague`: `DateTime.Now` is Prague time.

# Open threads
- Unverified in browser: decline flow, Back-to-origin, 80% weekly width, monthly prefill.
- Reminders may fire 2h off: `NotifyAt` UtcNow vs local `StartTime`.
