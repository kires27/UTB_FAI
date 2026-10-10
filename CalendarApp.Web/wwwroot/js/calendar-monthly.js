// Monthly calendar interactions.
// Runs after full DOM parse (jQuery loads later in the page, so no $ here).
document.addEventListener('DOMContentLoaded', function () {
    // Make calendar days clickable (only when not clicking on events)
    document.querySelectorAll('.calendar-day.current-month').forEach(function (day) {
        day.addEventListener('click', function (e) {
            // Don't navigate if click was on an event
            if (e.target.closest('.event-item')) return;
            var date = day.getAttribute('data-date');
            if (date) {
                // Navigate to create event for this day, prefilled with the current time
                var now = new Date();
                var hh = String(now.getHours()).padStart(2, '0');
                var mm = String(now.getMinutes()).padStart(2, '0');
                window.location.href = '/Events/Create?date=' + date + 'T' + hh + ':' + mm + '&view=monthly';
            }
        });
    });

    // Make events clickable
    document.querySelectorAll('.event-item').forEach(function (chip) {
        chip.addEventListener('click', function (e) {
            e.stopPropagation(); // Prevent day click
            var link = chip.querySelector('.event-link');
            if (link) window.location.href = link.getAttribute('href');
        });
    });

    // Highlight the day while hovering its events
    // (day hover styling comes from CSS, see .calendar-day:hover)
    document.querySelectorAll('.event-item').forEach(function (chip) {
        chip.addEventListener('mouseenter', function () {
            var d = chip.closest('.calendar-day');
            if (d) d.classList.add('has-hover-event');
        });
        chip.addEventListener('mouseleave', function () {
            var d = chip.closest('.calendar-day');
            if (d) d.classList.remove('has-hover-event');
        });
    });
});
