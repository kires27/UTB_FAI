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

    // Split overlapping events side by side within each day column.
    // Kept as a function so a drag & drop move can re-layout the affected columns.
    function layoutOverlaps() {
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
    }
    layoutOverlaps();

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

    // --- Drag & drop: move an event to another lane (day) / time slot ----------
    // Owner-only (data-editable="true"; the server enforces ownership again).
    // A drop POSTs to /Events/MoveOccurrence: non-recurring events are rescheduled
    // directly, one occurrence of a recurring series is stored as an exception row.
    // Drops outside a valid lane revert silently; success updates the grid in place.
    var dragWrap = document.querySelector('.time-grid-wrap');
    var tokenInput = dragWrap ? dragWrap.querySelector('input[name="__RequestVerificationToken"]') : null;
    var DAY_MINUTES = 24 * 60;
    var PX_PER_MIN = 0.8; // ROW / 60

    function pad2(n) { return String(n).padStart(2, '0'); }

    function fmtLocal(d) {
        return d.getFullYear() + '-' + pad2(d.getMonth() + 1) + '-' + pad2(d.getDate()) +
            'T' + pad2(d.getHours()) + ':' + pad2(d.getMinutes()) + ':00';
    }

    function attachDrag(el) {
        el.addEventListener('pointerdown', function (e) {
            if (e.button !== 0) return;
            e.preventDefault(); // no text selection / native link drag

            var isAllDay = el.dataset.allday === 'true';
            var startX = e.clientX, startY = e.clientY;
            var srcRect = el.getBoundingClientRect();
            var grabOffsetX = startX - srcRect.left;
            var grabOffsetY = startY - srcRect.top;
            var grabMins = (grabOffsetY / ROW) * 60;
            var duration = parseInt(el.dataset.duration, 10) || (isAllDay ? DAY_MINUTES : 60);
            var timeOfDay = el.dataset.timeofday || '00:00';

            var targets = Array.prototype.map.call(
                isAllDay
                    ? document.querySelectorAll('.tg-allday-cell')
                    : document.querySelectorAll('.tg-day-col'),
                function (c) { return { el: c, rect: c.getBoundingClientRect() }; });

            var active = false, ghost = null, hover = null, dropTarget = null, dropMins = 0;

            function beginDrag() {
                active = true;
                ghost = el.cloneNode(true);
                ghost.className = isAllDay ? 'tg-drag-ghost tg-ghost-chip' : 'tg-drag-ghost';
                ghost.removeAttribute('data-editable');
                document.body.appendChild(ghost);
                el.classList.add('tg-drag-origin');
            }

            function findTarget(x, y) {
                for (var i = 0; i < targets.length; i++) {
                    var r = targets[i].rect;
                    if (x >= r.left && x <= r.right && y >= r.top - 6 && y <= r.bottom + 6) return targets[i];
                }
                return null;
            }

            function setHover(t) {
                if (hover === t) return;
                if (hover) hover.el.classList.remove('tg-drag-over');
                hover = t;
                if (hover) hover.el.classList.add('tg-drag-over');
            }

            // Gutter time indicator during a drag: shows the landing time of the
            // event (start - end), aligned to its snapped top edge. Driven from the
            // drag loop because pointerdown preventDefault suppresses the ordinary
            // mousemove handler while dragging. All-day drags hide it (no time).
            function updateDragLabel(target) {
                if (!hoverLabel || !grid) return;
                if (!target) {
                    hoverLabel.style.display = 'none';
                    return;
                }
                var endMins = dropMins + duration;
                hoverLabel.textContent =
                    pad2(Math.floor(dropMins / 60)) + ':' + pad2(dropMins % 60) +
                    ' \u2013 ' + pad2(Math.floor(endMins / 60) % 24) + ':' + pad2(endMins % 60) +
                    (endMins >= DAY_MINUTES ? ' (+1d)' : '');
                var colTop = target.rect.top - grid.getBoundingClientRect().top;
                hoverLabel.style.top = (colTop + dropMins * PX_PER_MIN - 9) + 'px';
                hoverLabel.style.display = 'block';
            }

            function onMove(ev) {
                if (!active && Math.abs(ev.clientX - startX) < 3 && Math.abs(ev.clientY - startY) < 3) return;
                if (!active) beginDrag();

                var t = findTarget(ev.clientX, ev.clientY);
                setHover(t);
                dropTarget = t;

                if (isAllDay) {
                    ghost.style.left = (ev.clientX - grabOffsetX) + 'px';
                    ghost.style.top = (ev.clientY - grabOffsetY) + 'px';
                    updateDragLabel(null);
                } else if (t) {
                    var pointerMins = Math.min(DAY_MINUTES - 1,
                        Math.max(0, (ev.clientY - t.rect.top) / ROW * 60));
                    dropMins = Math.min(DAY_MINUTES - 15,
                        Math.max(0, Math.round((pointerMins - grabMins) / 15) * 15));
                    ghost.style.left = t.rect.left + 'px';
                    ghost.style.top = (t.rect.top + dropMins * PX_PER_MIN) + 'px';
                    ghost.style.width = (t.rect.width - 8) + 'px';
                    ghost.style.height = Math.max(20, duration * PX_PER_MIN) + 'px';
                    updateDragLabel(t);
                } else {
                    ghost.style.left = (ev.clientX - grabOffsetX) + 'px';
                    ghost.style.top = (ev.clientY - grabOffsetY) + 'px';
                    updateDragLabel(null);
                }
            }

            function postMove(newStart, newEnd, allDay, apply) {
                var body = new URLSearchParams();
                body.set('id', el.dataset.id);
                if (el.dataset.recurring === 'true') body.set('occurrenceDate', el.dataset.occurrence || '');
                body.set('newStart', fmtLocal(newStart));
                body.set('newEnd', fmtLocal(newEnd));
                body.set('allDay', allDay ? 'true' : 'false');
                body.set('__RequestVerificationToken', tokenInput.value);

                fetch('/Events/MoveOccurrence', { method: 'POST', body: body, credentials: 'same-origin' })
                    .then(function (res) {
                        if (!res.ok) throw new Error('Move failed (' + res.status + ')');
                        apply();
                    })
                    .catch(function (err) { console.warn(err.message || err); });
            }

            function onUp() {
                window.removeEventListener('pointermove', onMove);
                window.removeEventListener('pointerup', onUp);
                setHover(null);
                if (hoverLabel) hoverLabel.style.display = 'none';
                if (ghost) ghost.remove();
                el.classList.remove('tg-drag-origin');

                if (!active) return; // below threshold: plain click -> Details link

                // Swallow the click that follows a drag (would otherwise navigate)
                document.addEventListener('click', function (ev) {
                    ev.preventDefault();
                    ev.stopPropagation();
                }, { capture: true, once: true });

                if (!dropTarget) return; // dropped outside a lane — nothing happens

                if (isAllDay) {
                    var alldayStart = new Date(dropTarget.el.dataset.date + 'T' + timeOfDay);
                    var alldayEnd = new Date(alldayStart.getTime() + duration * 60000);
                    var targetCell = dropTarget.el;
                    postMove(alldayStart, alldayEnd, true, function () {
                        targetCell.appendChild(el);
                    });
                } else {
                    var timedStart = new Date(dropTarget.el.dataset.date + 'T00:00:00');
                    timedStart.setMinutes(dropMins);
                    var timedEnd = new Date(timedStart.getTime() + duration * 60000);
                    var targetCol = dropTarget.el;
                    var newStartMins = dropMins;
                    var clampedEnd = Math.min(DAY_MINUTES, newStartMins + duration);
                    postMove(timedStart, timedEnd, false, function () {
                        targetCol.appendChild(el);
                        el.dataset.start = String(newStartMins);
                        el.dataset.end = String(clampedEnd);
                        el.style.top = Math.round(newStartMins * PX_PER_MIN) + 'px';
                        el.style.height = Math.max(20, Math.round((clampedEnd - newStartMins) * PX_PER_MIN)) + 'px';
                        var timeSpan = el.querySelector('.tg-event-time');
                        if (timeSpan) {
                            timeSpan.textContent = pad2(Math.floor(newStartMins / 60)) + ':' + pad2(newStartMins % 60);
                        }
                        layoutOverlaps();
                    });
                }
            }

            window.addEventListener('pointermove', onMove);
            window.addEventListener('pointerup', onUp);
        });
    }

    if (tokenInput) {
        document.querySelectorAll('.tg-event[data-editable="true"], .tg-allday-chip[data-editable="true"]')
            .forEach(attachDrag);
    }
});
