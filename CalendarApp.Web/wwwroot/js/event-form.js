// Create / Edit event form behaviour (Thunderbird-style layout, shared).
// Vanilla JS only — jQuery loads after RenderBody, so $(...) is unavailable here.
document.addEventListener('DOMContentLoaded', function () {
    function pad2(n) { return (n < 10 ? '0' : '') + n; }

    // Parse a rendered DateTime value (culture "M/D/YYYY H:mm:ss" or ISO) into { d, t }
    function parseDT(raw) {
        raw = (raw || '').trim();
        var m = raw.match(/^(\d{4})-(\d{2})-(\d{2})[T ](\d{1,2}):(\d{2})/);
        if (m) return { d: m[1] + '-' + m[2] + '-' + m[3], t: pad2(+m[4]) + ':' + m[5] };
        m = raw.match(/^(\d{1,2})\/(\d{1,2})\/(\d{4})[T ]?(\d{1,2}):(\d{2})/);
        if (m) return { d: m[3] + '-' + pad2(+m[1]) + '-' + pad2(+m[2]), t: pad2(+m[4]) + ':' + m[5] };
        m = raw.match(/^(\d{4})-(\d{2})-(\d{2})/);
        if (m) return { d: m[1] + '-' + m[2] + '-' + m[3], t: '00:00' };
        return { d: '', t: '' };
    }

    // Date -> local "yyyy-MM-ddTHH:mm" (container runs in the user's TZ)
    function isoOf(date) {
        return date.getFullYear() + '-' + pad2(date.getMonth() + 1) + '-' + pad2(date.getDate()) +
            'T' + pad2(date.getHours()) + ':' + pad2(date.getMinutes());
    }

    var WEEKDAYS = ['SU', 'MO', 'TU', 'WE', 'TH', 'FR', 'SA'];
    var FREQ_UNITS = { DAILY: 'DAILY', WEEKLY: 'WEEKLY', MONTHLY: 'MONTHLY', YEARLY: 'YEARLY' };
    var weeklyDays = [];

    function weekdayCode(dateStr) {
        if (!dateStr) return 'MO';
        var d = new Date(dateStr + 'T00:00');
        return WEEKDAYS[d.getDay()] || 'MO';
    }

    // ---------- Start / End split inputs ----------
    var allDay = document.getElementById('AllDay');
    var startD = document.getElementById('startDate');
    var startT = document.getElementById('startTime');
    var endD = document.getElementById('endDate');
    var endT = document.getElementById('endTime');
    var startHidden = document.getElementById('StartTime');
    var endHidden = document.getElementById('EndTime');

    function composeStart() {
        startHidden.value = (startD.value || '') + 'T' + (startT.value || '00:00');
    }
    function composeEnd() {
        endHidden.value = (endD.value || '') + 'T' + (endT.value || '00:00');
    }
    function syncAllDay() {
        startT.classList.toggle('d-none', allDay.checked);
        endT.classList.toggle('d-none', allDay.checked);
    }
    (function initWhen() {
        var s = parseDT(startHidden.value);
        var e = parseDT(endHidden.value);
        if (s.d) { startD.value = s.d; startT.value = s.t; }
        if (e.d) { endD.value = e.d; endT.value = e.t; }
        composeStart(); composeEnd(); // normalize hidden values to ISO
        syncAllDay();
        allDay.addEventListener('change', syncAllDay);
        startD.addEventListener('change', function () {
            // End cannot start before the event
            if (endD.value && endD.value < startD.value) endD.value = startD.value;
            composeStart(); composeEnd();
            weeklyDays = [weekdayCode(startD.value)];
            syncWeeklyChips();
            updateMonthlyDay();
            buildRule();
            refreshReminder();
        });
        startT.addEventListener('change', function () { composeStart(); refreshReminder(); });
        endD.addEventListener('change', composeEnd);
        endT.addEventListener('change', composeEnd);
    })();

    // ---------- Repeat editor (dropdown + structured panel) ----------
    var repeatFreq = document.getElementById('repeatFreq');
    var repeatInterval = document.getElementById('repeatInterval');
    var repeatUntil = document.getElementById('repeatUntil');
    var untilLabel = document.getElementById('untilLabel');
    var unitLabel = document.getElementById('unitLabel');
    var intervalRow = document.getElementById('intervalRow');
    var weeklyChipsRow = document.getElementById('weeklyChips');
    var monthlyNote = document.getElementById('monthlyNote');
    var monthlyDayEl = document.getElementById('monthlyDay');
    var ruleHidden = document.getElementById('RecurrenceRule');
    var isRecurringHidden = document.getElementById('IsRecurring');
    var UNITS = { DAILY: 'days', WEEKLY: 'weeks', MONTHLY: 'months', YEARLY: 'years' };

    function untilFmt(v) { return v.replace(/-/g, '') + 'T000000'; }
    function untilParse(s) {
        return /^\d{8}T/.test(s) ? s.slice(0, 4) + '-' + s.slice(4, 6) + '-' + s.slice(6, 8) : '';
    }
    function syncWeeklyChips() {
        document.querySelectorAll('#weeklyChips .chip-toggle').forEach(function (b) {
            b.classList.toggle('active', weeklyDays.indexOf(b.dataset.day) !== -1);
        });
    }
    function updateMonthlyDay() {
        if (monthlyDayEl) monthlyDayEl.textContent = (parseDT(startHidden.value).d || '').slice(8) || '?';
    }
    function buildRule() {
        var f = repeatFreq.value;
        isRecurringHidden.value = f ? 'true' : 'false';
        var rule = '';
        if (f) {
            rule = 'FREQ=' + f;
            var n = parseInt(repeatInterval.value, 10) || 1;
            if (n > 1) rule += ';INTERVAL=' + n;
            if (f === 'WEEKLY' && weeklyDays.length) rule += ';BYDAY=' + weeklyDays.join(',');
            if (repeatUntil.value) rule += ';UNTIL=' + untilFmt(repeatUntil.value);
        }
        ruleHidden.value = rule;
    }
    function syncRepeatUI() {
        var f = repeatFreq.value;
        var active = f !== '';
        repeatUntil.classList.toggle('d-none', !active);
        if (untilLabel) untilLabel.classList.toggle('d-none', !active);
        intervalRow.classList.toggle('d-none', !active);
        if (unitLabel) unitLabel.textContent = UNITS[f] || 'days';
        weeklyChipsRow.classList.toggle('d-none', f !== 'WEEKLY');
        if (monthlyNote) monthlyNote.classList.toggle('d-none', f !== 'MONTHLY');
        buildRule();
    }
    function bindWeekdayChips() {
        document.querySelectorAll('#weeklyChips .chip-toggle').forEach(function (b) {
            b.addEventListener('click', function () {
                var day = b.dataset.day;
                var i = weeklyDays.indexOf(day);
                if (i === -1) { weeklyDays.push(day); b.classList.add('active'); }
                else if (weeklyDays.length > 1) { weeklyDays.splice(i, 1); b.classList.remove('active'); }
                buildRule();
            });
        });
    }
    (function initRepeat() {
        var rule = (ruleHidden.value || '').toUpperCase();
        var freq = (rule.match(/FREQ=([A-Z]+)/) || [])[1];
        var byday = (rule.match(/BYDAY=([A-Z,]+)/) || [])[1];
        var interval = (rule.match(/INTERVAL=(\d+)/) || [])[1];
        var until = (rule.match(/UNTIL=(\d{8}T\d{6})/) || [])[1];

        repeatFreq.value = ['DAILY', 'WEEKLY', 'MONTHLY', 'YEARLY'].indexOf(freq) !== -1 ? freq : '';
        repeatInterval.value = interval || 1;
        weeklyDays = byday ? byday.split(',') : [weekdayCode(startD.value)];
        if (until) repeatUntil.value = untilParse(until);
        syncWeeklyChips();
        updateMonthlyDay();
        syncRepeatUI();

        repeatFreq.addEventListener('change', function () {
            if (repeatFreq.value === 'WEEKLY' && !weeklyDays.length) {
                weeklyDays = [weekdayCode(startD.value)];
                syncWeeklyChips();
            }
            syncRepeatUI();
        });
        repeatInterval.addEventListener('input', buildRule);
        repeatUntil.addEventListener('change', buildRule);
        bindWeekdayChips();
    })();

    // ---------- Reminder presets ----------
    var reminderPreset = document.getElementById('reminderPreset');
    var reminderCustom = document.getElementById('reminderCustom');
    var reminderHidden = document.getElementById('ReminderTime');

    function refreshReminder() {
        var v = reminderPreset.value;
        reminderCustom.classList.toggle('d-none', v !== 'custom');
        if (v === '') {
            reminderHidden.value = '';
        } else if (v === 'custom') {
            reminderHidden.value = reminderCustom.value || '';
        } else {
            var base = new Date((startD.value || '') + 'T' + (startT.value || '00:00'));
            reminderHidden.value = isNaN(base) ? '' : isoOf(new Date(base.getTime() - parseInt(v, 10) * 60000));
        }
    }
    (function initReminder() {
        var start = parseDT(startHidden.value);
        var rem = parseDT(reminderHidden.value);
        if (rem.d && start.d) {
            var diffMin = Math.round(
                (new Date(rem.d + 'T' + rem.t) - new Date(start.d + 'T' + start.t)) / 60000);
            var preset = String(-diffMin);
            if (['0', '5', '15', '30', '60', '1440'].indexOf(preset) !== -1) {
                reminderPreset.value = preset;
            } else {
                reminderPreset.value = 'custom';
                reminderCustom.value = rem.d + 'T' + rem.t;
            }
        }
        refreshReminder();
        reminderPreset.addEventListener('change', refreshReminder);
        reminderCustom.addEventListener('input', refreshReminder);
    })();

    // ---------- Color swatches ----------
    var swatchBox = document.getElementById('colorSwatches');
    var colorHidden = document.getElementById('Color');
    var customPicker = document.getElementById('colorCustom');
    var customLabel = customPicker ? customPicker.closest('.color-swatch-custom') : null;

    function markActive(hex) {
        if (!swatchBox) return;
        var preset = false;
        swatchBox.querySelectorAll('button.color-swatch').forEach(function (b) {
            var match = (b.dataset.color || '').toLowerCase() === (hex || '').toLowerCase();
            b.classList.toggle('active', match);
            if (match) preset = true;
        });
        // The "Custom" chip lights up when no preset matches the chosen color
        if (customLabel) customLabel.classList.toggle('active', !preset && !!hex);
    }
    if (swatchBox) {
        swatchBox.querySelectorAll('button.color-swatch').forEach(function (b) {
            b.addEventListener('click', function () {
                colorHidden.value = b.dataset.color;
                markActive(b.dataset.color);
            });
        });
        if (customPicker) {
            customPicker.addEventListener('input', function () {
                colorHidden.value = this.value;
                markActive(this.value);
            });
        }
        markActive(colorHidden.value);
    }

    // ---------- Attendee removal with undo ----------
    var removedIds = [];
    var container = document.getElementById('removeAttendeeIdsContainer');
    document.querySelectorAll('.attendee-row').forEach(function (row) {
        var btn = row.querySelector('.attendee-remove');
        if (!btn) return;
        btn.addEventListener('click', function () {
            var id = row.dataset.userId;
            var idx = removedIds.indexOf(id);
            if (idx === -1) {
                removedIds.push(id);
                var input = document.createElement('input');
                input.type = 'hidden';
                input.name = 'RemoveAttendeeIds';
                input.value = id;
                container.appendChild(input);
                row.classList.add('attendee-removed');
                btn.classList.replace('btn-outline-danger', 'btn-outline-secondary');
                btn.innerHTML = '<i class="bi bi-arrow-counterclockwise"></i> Undo';
            } else {
                removedIds.splice(idx, 1);
                var hid = container.querySelector('input[value="' + id + '"]');
                if (hid) hid.remove();
                row.classList.remove('attendee-removed');
                btn.classList.replace('btn-outline-secondary', 'btn-outline-danger');
                btn.innerHTML = '<i class="bi bi-x-lg"></i> Remove';
            }
        });
    });

    // ---------- Invite user search (typeahead) ----------
    var dataEl = document.getElementById('editData');
    var cfg = { attendeeEmails: [], attendeeIds: [], excludedIds: [] };
    function refreshCfg() {
        try { if (dataEl) cfg = JSON.parse(dataEl.textContent); } catch (e) { /* defaults */ }
        cfg.attendeeEmails = cfg.attendeeEmails || [];
        cfg.attendeeIds = cfg.attendeeIds || [];
        cfg.excludedIds = cfg.excludedIds || [];
    }
    refreshCfg();

    var searchInput = document.getElementById('inviteSearch');
    var resultsBox = document.getElementById('inviteResults');
    var chipsBox = document.getElementById('inviteChips');
    var hiddenField = document.querySelector('input[name="InviteEmails"]');
    var debounce;

    var getInvited = function () {
        return hiddenField.value.split(',').map(function (e) { return e.trim(); }).filter(Boolean);
    };
    var isExcluded = function (id, email) {
        if (cfg.excludedIds.indexOf(id) !== -1) return true; // owner / current user
        if (cfg.attendeeIds.indexOf(id) !== -1) return true;
        return (cfg.attendeeEmails || []).indexOf(email) !== -1;
    };

    function renderChips() {
        chipsBox.innerHTML = '';
        getInvited().forEach(function (email) {
            var chip = document.createElement('span');
            chip.className = 'invite-chip';
            var text = document.createElement('span');
            text.textContent = email;
            var btn = document.createElement('button');
            btn.type = 'button';
            btn.className = 'btn-close';
            btn.setAttribute('aria-label', 'Remove ' + email);
            btn.addEventListener('click', function () {
                hiddenField.value = getInvited().filter(function (e) { return e !== email; }).join(',');
                renderChips();
            });
            chip.appendChild(text);
            chip.appendChild(btn);
            chipsBox.appendChild(chip);
        });
    }

    function hideResults() {
        resultsBox.style.display = 'none';
        resultsBox.innerHTML = '';
    }

    if (searchInput) {
        searchInput.addEventListener('input', function () {
            clearTimeout(debounce);
            var term = this.value.trim();
            if (term.length < 3) { hideResults(); return; }
            debounce = setTimeout(function () {
                fetch('/Events/SearchUsers?term=' + encodeURIComponent(term))
                    .then(function (r) { return r.json(); })
                    .then(function (users) {
                        refreshCfg(); // admin pages update excludedIds when the owner changes
                        var invited = getInvited();
                        resultsBox.innerHTML = '';
                        users.forEach(function (u) {
                            if (isExcluded(u.id, u.email)) return;
                            if (invited.indexOf(u.email) !== -1) return;
                            var item = document.createElement('button');
                            item.type = 'button';
                            item.className = 'list-group-item list-group-item-action py-1';
                            var name = document.createElement('strong');
                            name.textContent = u.userName;
                            var mail = document.createElement('small');
                            mail.className = 'text-muted ms-1';
                            mail.textContent = u.email;
                            item.appendChild(name);
                            item.appendChild(mail);
                            item.addEventListener('click', function () {
                                hiddenField.value = getInvited().concat(u.email).join(',');
                                renderChips();
                                searchInput.value = '';
                                hideResults();
                            });
                            resultsBox.appendChild(item);
                        });
                        resultsBox.style.display = resultsBox.childElementCount ? 'block' : 'none';
                    });
            }, 250);
        });

        document.addEventListener('click', function (e) {
            if (!resultsBox.contains(e.target) && e.target !== searchInput) hideResults();
        });

        renderChips();
    }

    // ---------- Delete confirmation ----------
    var deleteForm = document.getElementById('deleteForm');
    if (deleteForm) {
        deleteForm.addEventListener('submit', function (e) {
            if (!confirm('Delete this event permanently? Its invitations and reminders are removed too.')) {
                e.preventDefault();
            }
        });
    }
});
