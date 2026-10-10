// Apply saved theme and sidebar state before first paint (dark + open = default).
// ?theme=light|dark in the URL overrides the theme without saving.
(function () {
    var theme = 'dark';
    try { theme = localStorage.getItem('cw-theme') || 'dark'; } catch (e) { }
    try {
        var q = new URLSearchParams(window.location.search).get('theme');
        if (q === 'light' || q === 'dark') theme = q;
    } catch (e) { }
    document.documentElement.setAttribute('data-bs-theme', theme);

    var sidebar = 'open';
    try { sidebar = localStorage.getItem('cw-sidebar') || 'open'; } catch (e) { }
    document.documentElement.setAttribute('data-sidebar', sidebar === 'folded' ? 'folded' : 'open');
})();
