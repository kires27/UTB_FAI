// Dark/light theme toggle
(function () {
    function applyThemeIcon() {
        var icon = document.getElementById('themeIcon');
        if (!icon) return;
        var dark = document.documentElement.getAttribute('data-bs-theme') !== 'light';
        icon.className = dark ? 'bi bi-sun' : 'bi bi-moon-stars';
    }
    applyThemeIcon();
    var toggle = document.getElementById('themeToggle');
    if (toggle) toggle.addEventListener('click', function () {
        var next = document.documentElement.getAttribute('data-bs-theme') === 'dark' ? 'light' : 'dark';
        document.documentElement.setAttribute('data-bs-theme', next);
        try { localStorage.setItem('cw-theme', next); } catch (e) { }
        applyThemeIcon();
    });
})();

// Sidebar fold (desktop) / drawer (mobile)
(function () {
    function applyFoldIcon() {
        var icon = document.getElementById('sidebarFoldIcon');
        if (!icon) return;
        var folded = document.documentElement.getAttribute('data-sidebar') === 'folded';
        icon.className = folded ? 'bi bi-chevron-right' : 'bi bi-chevron-left';
    }
    applyFoldIcon();

    var fold = document.getElementById('sidebarFold');
    if (fold) fold.addEventListener('click', function () {
        var next = document.documentElement.getAttribute('data-sidebar') === 'folded' ? 'open' : 'folded';
        document.documentElement.setAttribute('data-sidebar', next);
        try { localStorage.setItem('cw-sidebar', next); } catch (e) { }
        applyFoldIcon();
    });

    var mobileBtn = document.getElementById('sidebarMobileBtn');
    var backdrop = document.getElementById('sidebarBackdrop');
    function closeMobile() { document.documentElement.removeAttribute('data-sidebar-mobile'); }
    if (mobileBtn) mobileBtn.addEventListener('click', function () {
        document.documentElement.setAttribute('data-sidebar-mobile', 'open');
    });
    if (backdrop) backdrop.addEventListener('click', closeMobile);
})();

// Poll unread notification count for the sidebar badge
// (only when the user is authenticated, signaled via data-authenticated on <body>)
$(document).ready(function () {
    if (document.body.dataset.authenticated !== 'true') return;
    if (!document.getElementById('sidebarUnread')) return;

    updateNotificationCount();
    setInterval(updateNotificationCount, 30000);
});

function updateNotificationCount() {
    $.get('/Notifications/GetUnreadCount', function (data) {
        var badge = document.getElementById('sidebarUnread');
        if (!badge) return;
        var count = (data && data.unreadCount) || 0;
        if (count > 0) {
            badge.textContent = count > 99 ? '99+' : count;
            badge.classList.remove('d-none');
        } else {
            badge.classList.add('d-none');
        }
    });
}
