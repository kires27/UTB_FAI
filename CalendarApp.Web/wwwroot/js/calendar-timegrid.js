// Time-grid (daily/weekly) calendar interactions.
// Runs after full DOM parse (jQuery loads later in the page, so no $ here).
document.addEventListener('DOMContentLoaded', function () {
    var ROW = 48; // px per hour

    // Auto-scroll to current time (default 7:00)
    var now = new Date();
    var target = now.getHours() >= 9 ? (now.getHours() - 2) * ROW : 7 * ROW;
    var scroll = document.getElementById('tgScroll');
    if (scroll) scroll.scrollTop = target;

    // Current time indicator (in today's column)
    function updateNowLine() {
        var col = document.querySelector('.tg-day-col.today-col');
        if (!col) return;
        var n = new Date();
        var top = (n.getHours() * 60 + n.getMinutes()) / 60 * ROW;
        var line = col.querySelector('.tg-now-line');
        if (!line) {
            line = document.createElement('div');
            line.className = 'tg-now-line';
            var label = document.createElement('span');
            label.className = 'tg-now-label';
            line.appendChild(label);
            col.appendChild(line);
        }
        line.style.top = top + 'px';
        line.querySelector('.tg-now-label').textContent =
            String(n.getHours()).padStart(2, '0') + ':' + String(n.getMinutes()).padStart(2, '0');
    }
    updateNowLine();
    setInterval(updateNowLine, 60000);

    // Hover time indicator in the left gutter (15-min granularity, same as clicks)
    var grid = document.querySelector('.tg-grid');
    if (grid) {
        var hoverLabel = document.createElement('div');
        hoverLabel.className = 'tg-hover-label';
        grid.appendChild(hoverLabel);

        document.querySelectorAll('.tg-day-col').forEach(function (col) {
            col.addEventListener('mousemove', function (e) {
                var yCol = e.clientY - col.getBoundingClientRect().top;
                if (yCol < 0) { hoverLabel.style.display = 'none'; return; }
                var mins = Math.min(23 * 60 + 45, Math.floor((yCol / ROW) * 60 / 15) * 15);
                hoverLabel.textContent = String(Math.floor(mins / 60)).padStart(2, '0') +
                    ':' + String(mins % 60).padStart(2, '0');
                // snap to the 15-min slot the cursor is in (no smooth movement)
                var colTop = col.getBoundingClientRect().top - grid.getBoundingClientRect().top;
                hoverLabel.style.top = (colTop + (mins / 60) * ROW - 9) + 'px';
                hoverLabel.style.display = 'block';
            });
        });
        grid.addEventListener('mouseleave', function () {
            hoverLabel.style.display = 'none';
        });
    }

    // Split overlapping events side by side within each day column
    document.querySelectorAll('.tg-day-col').forEach(function (col) {
        var evs = Array.prototype.map.call(col.querySelectorAll('.tg-event'), function (el) {
            return { el: el, s: +el.dataset.start, e: +el.dataset.end };
        }).sort(function (a, b) { return a.s - b.s; });

        var groups = [], cur = [];
        evs.forEach(function (ev) {
            var latest = cur.length ? Math.max.apply(null, cur.map(function (x) { return x.e; })) : -1;
            if (cur.length && ev.s >= latest) { groups.push(cur); cur = []; }
            cur.push(ev);
        });
        if (cur.length) groups.push(cur);

        groups.forEach(function (g) {
            var lanes = [];
            g.forEach(function (ev) {
                var placed = false;
                for (var i = 0; i < lanes.length; i++) {
                    if (lanes[i].every(function (x) { return x.e <= ev.s; })) {
                        lanes[i].push(ev); placed = true; break;
                    }
                }
                if (!placed) lanes.push([ev]);
            });
            // Events use only part of the column so the free space beside them stays clickable
            var span = 90;
            lanes.forEach(function (lane, i) {
                lane.forEach(function (ev) {
                    var w = span / lanes.length;
                    ev.el.style.left = 'calc(' + (i * w) + '% + 3px)';
                    ev.el.style.width = 'calc(' + w + '% - 6px)';
                });
            });
        });
    });

    // Click an empty slot -> create event at that date/time (rounded to 15 min)
    var gridView = document.querySelector('.time-grid-wrap');
    var sourceView = gridView ? gridView.dataset.view : 'list';
    document.querySelectorAll('.tg-day-col').forEach(function (col) {
        col.addEventListener('click', function (e) {
            if (e.target !== col) return;
            var rect = col.getBoundingClientRect();
            var mins = Math.min(23 * 60 + 45, Math.floor((e.clientY - rect.top) / ROW * 60 / 15) * 15);
            var hh = Math.floor(mins / 60), mm = mins % 60;
            window.location.href = '/Events/Create?date=' + col.dataset.date +
                'T' + String(hh).padStart(2, '0') + ':' + String(mm).padStart(2, '0') +
                '&view=' + encodeURIComponent(sourceView);
        });
    });

    // Click empty all-day lane -> create all-day event
    document.querySelectorAll('.tg-allday-cell').forEach(function (cell) {
        cell.addEventListener('click', function (e) {
            if (e.target !== cell) return;
            window.location.href = '/Events/Create?date=' + cell.dataset.date +
                'T00:00&allDay=true&view=' + encodeURIComponent(sourceView);
        });
    });
});
