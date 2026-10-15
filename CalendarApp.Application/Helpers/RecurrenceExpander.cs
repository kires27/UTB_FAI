using System.Linq;
using CalendarApp.Domain.Entities;

namespace CalendarApp.Application.Helpers
{
    public static class RecurrenceExpander
    {
        private const int MaxOccurrencesPerEvent = 500;

        private static readonly Dictionary<string, DayOfWeek> WeekdayCodes = new()
        {
            ["MO"] = DayOfWeek.Monday,
            ["TU"] = DayOfWeek.Tuesday,
            ["WE"] = DayOfWeek.Wednesday,
            ["TH"] = DayOfWeek.Thursday,
            ["FR"] = DayOfWeek.Friday,
            ["SA"] = DayOfWeek.Saturday,
            ["SU"] = DayOfWeek.Sunday
        };

        private sealed record ParsedRule(string Freq, int Interval, List<DayOfWeek>? ByDay, DateTime? Until);

        public static List<Event> Expand(IEnumerable<Event> events, DateTime rangeStart, DateTime rangeEnd)
        {
            var result = new List<Event>();
            foreach (var @event in events)
            {
                result.AddRange(Expand(@event, rangeStart, rangeEnd));
            }
            return result;
        }

        public static IEnumerable<Event> Expand(Event @event, DateTime rangeStart, DateTime rangeEnd)
        {
            var rule = (@event.IsRecurring && !string.IsNullOrWhiteSpace(@event.RecurrenceRule))
                ? ParseRule(@event.RecurrenceRule)
                : null;

            if (rule == null)
            {
                if (Overlaps(@event.StartTime, @event.EndTime, rangeStart, rangeEnd))
                {
                    yield return @event;
                }
                yield break;
            }

            var exceptions = @event.Exceptions ?? new List<EventOccurrenceException>();
            var emitted = 0;

            foreach (var occurrenceDate in OccurrenceDates(@event, rule))
            {
                var ex = exceptions.FirstOrDefault(x => x.OriginalDate.Date == occurrenceDate.Date);
                if (ex != null && ex.IsCancelled)
                {
                    continue; // occurrence deleted (this occurrence only)
                }

                var occurrenceStart = occurrenceDate.Date + @event.StartTime.TimeOfDay;
                var occurrenceEnd = occurrenceStart + (@event.EndTime - @event.StartTime);
                if (!Overlaps(occurrenceStart, occurrenceEnd, rangeStart, rangeEnd))
                {
                    continue;
                }

                if (emitted++ >= MaxOccurrencesPerEvent)
                {
                    yield break;
                }

                yield return BuildOccurrence(@event, occurrenceStart, occurrenceEnd, ex);
            }
        }

        // Human-readable summary
        public static string? Describe(Event @event)
        {
            if (!@event.IsRecurring || string.IsNullOrWhiteSpace(@event.RecurrenceRule)) return null;

            var rule = ParseRule(@event.RecurrenceRule);
            if (rule == null) return null;

            var adverb = rule.Freq switch
            {
                "DAILY" => "Daily",
                "WEEKLY" => "Weekly",
                "MONTHLY" => "Monthly",
                "YEARLY" => "Yearly",
                _ => null
            };
            var unit = rule.Freq switch
            {
                "DAILY" => "day",
                "WEEKLY" => "week",
                "MONTHLY" => "month",
                "YEARLY" => "year",
                _ => null
            };
            if (adverb == null || unit == null) return null;

            var text = rule.Interval == 1 ? adverb : $"Every {rule.Interval} {unit}s";

            if (rule.Freq == "WEEKLY" && rule.ByDay != null && rule.ByDay.Count > 0)
            {
                text += " on " + string.Join(", ", rule.ByDay);
            }

            var end = rule.Until;
            if (@event.RecurrenceEnd.HasValue && (end == null || @event.RecurrenceEnd.Value.Date < end.Value.Date))
            {
                end = @event.RecurrenceEnd.Value.Date;
            }
            if (end.HasValue)
            {
                text += " until " + end.Value.ToString("MMM d, yyyy", System.Globalization.CultureInfo.GetCultureInfo("en-US"));
            }

            return text;
        }

        private static bool Overlaps(DateTime start, DateTime end, DateTime rangeStart, DateTime rangeEnd)
            => end >= rangeStart && start <= rangeEnd;

        private static ParsedRule? ParseRule(string rule)
        {
            var upper = rule.ToUpperInvariant();
            var freq = upper.Match("FREQ=([A-Z]+)");
            if (freq == null) return null;

            var intervalRaw = upper.Match("INTERVAL=(\\d+)");
            var interval = int.TryParse(intervalRaw, out var n) && n > 0 ? n : 1;

            List<DayOfWeek>? byDay = null;
            var byDayRaw = upper.Match("BYDAY=([A-Z,]+)");
            if (byDayRaw != null)
            {
                byDay = byDayRaw
                    .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                    .Where(code => WeekdayCodes.ContainsKey(code))
                    .Select(code => WeekdayCodes[code])
                    .Distinct()
                    .OrderBy(d => ((int)d + 6) % 7) // Monday-first order
                    .ToList();
            }

            DateTime? until = null;
            var untilRaw = upper.Match("UNTIL=(\\d{8})");
            if (untilRaw != null && DateTime.TryParseExact(untilRaw, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var untilDate))
            {
                until = untilDate; // inclusive: occurrences up to and on this date
            }

            return new ParsedRule(freq, interval, byDay, until);
        }

        private static IEnumerable<DateTime> OccurrenceDates(Event @event, ParsedRule rule)
        {
            var start = @event.StartTime.Date;

            var horizon = DateTime.Today.AddYears(30);
            if (rule.Until.HasValue && rule.Until.Value.Date < horizon) horizon = rule.Until.Value.Date;
            if (@event.RecurrenceEnd.HasValue && @event.RecurrenceEnd.Value.Date < horizon) horizon = @event.RecurrenceEnd.Value.Date;

            var anchorYielded = false;

            IEnumerable<DateTime> dates = rule.Freq switch
            {
                "DAILY" => Daily(start, rule.Interval, horizon),
                "WEEKLY" => rule.ByDay != null && rule.ByDay.Count > 0
                    ? WeeklyByDay(start, rule.Interval, rule.ByDay, horizon)
                    : Weekly(start, rule.Interval, horizon),
                "MONTHLY" => Monthly(start, rule.Interval, horizon),
                "YEARLY" => Yearly(start, rule.Interval, horizon),
                _ => Enumerable.Empty<DateTime>()
            };

            foreach (var date in dates)
            {
                if (date == start)
                {
                    anchorYielded = true;
                    yield return date;
                    continue;
                }

                if (!anchorYielded && date > start)
                {
                    if (AnchorInSeries(start, rule))
                    {
                        yield return start;
                    }
                    anchorYielded = true;
                }

                if (date < start)
                {
                    continue; // BYDAY slots before the anchor are not occurrences
                }

                if (date > horizon)
                {
                    yield break;
                }

                yield return date;
            }

            if (!anchorYielded && AnchorInSeries(start, rule))
            {
                yield return start;
            }
        }

        private static bool AnchorInSeries(DateTime start, ParsedRule rule)
            => !(rule.Until.HasValue && start > rule.Until.Value.Date);

        private static IEnumerable<DateTime> Daily(DateTime start, int interval, DateTime horizon)
        {
            for (var date = start; date <= horizon; date = date.AddDays(interval))
            {
                yield return date;
            }
        }

        private static IEnumerable<DateTime> Weekly(DateTime start, int interval, DateTime horizon)
        {
            for (var date = start; date <= horizon; date = date.AddDays(7 * interval))
            {
                yield return date;
            }
        }

        private static IEnumerable<DateTime> WeeklyByDay(DateTime start, int interval, List<DayOfWeek> byDay, DateTime horizon)
        {
            var anchorWeek = start.AddDays(-(((int)start.DayOfWeek + 6) % 7)); // Monday of anchor week
            for (var week = anchorWeek; week <= horizon; week = week.AddDays(7 * interval))
            {
                foreach (var day in byDay)
                {
                    var date = week.AddDays(((int)day + 6) % 7);
                    if (date > horizon) break;
                    yield return date;
                }
            }
        }

        private static IEnumerable<DateTime> Monthly(DateTime start, int interval, DateTime horizon)
        {
            for (var month = start; month <= horizon; month = month.AddMonths(interval))
            {
                var daysInMonth = DateTime.DaysInMonth(month.Year, month.Month);
                if (start.Day > daysInMonth)
                {
                    continue;
                }
                yield return new DateTime(month.Year, month.Month, start.Day);
            }
        }

        private static IEnumerable<DateTime> Yearly(DateTime start, int interval, DateTime horizon)
        {
            for (var date = start; date <= horizon; date = date.AddYears(interval))
            {
                yield return date;
            }
        }

        private static Event BuildOccurrence(Event master, DateTime occurrenceStart, DateTime occurrenceEnd, EventOccurrenceException? ex)
        {
            var clone = new Event
            {
                Id = master.Id,
                Title = master.Title,
                Description = master.Description,
                StartTime = occurrenceStart,
                EndTime = occurrenceEnd,
                AllDay = master.AllDay,
                Location = master.Location,
                IsRecurring = master.IsRecurring,
                RecurrenceRule = master.RecurrenceRule,
                RecurrenceEnd = master.RecurrenceEnd,
                OwnerId = master.OwnerId,
                ReminderTime = master.ReminderTime,
                Color = master.Color,
                Status = master.Status,
                CreatedAt = master.CreatedAt,
                UpdatedAt = master.UpdatedAt,
                EventAttendees = master.EventAttendees,
                Exceptions = master.Exceptions
            };

            clone.OccurrenceOriginalDate = (ex != null ? ex.OriginalDate : occurrenceStart).Date;

            if (ex != null)
            {
                clone.Title = ex.Title ?? clone.Title;
                clone.Description = ex.Description ?? clone.Description;
                clone.StartTime = ex.StartTime ?? clone.StartTime;
                clone.EndTime = ex.EndTime ?? clone.EndTime;
                clone.AllDay = ex.AllDay ?? clone.AllDay;
                clone.Location = ex.Location ?? clone.Location;
                clone.ReminderTime = ex.ReminderTime ?? clone.ReminderTime;
                clone.Color = ex.Color ?? clone.Color;
                clone.Status = ex.Status ?? clone.Status;
            }

            return clone;
        }
    }

    // regex helper used by ParseRule 
    internal static class StringMatchExtensions
    {
        public static string? Match(this string input, string pattern)
        {
            var m = System.Text.RegularExpressions.Regex.Match(input, pattern);
            return m.Success ? m.Groups[1].Value : null;
        }
    }
}
