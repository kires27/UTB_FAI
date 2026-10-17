// Notifications list: submit invitation accept/decline forms via fetch,
// then update the list item in place. Falls back to a normal form
// submission when the AJAX call fails.
document.addEventListener('submit', function (e) {
    var form = e.target.closest('form[data-invitation-action]');
    if (!form) return;

    e.preventDefault();

    var action = form.getAttribute('data-invitation-action');
    var url = form.action; // rendered by asp-action/asp-route-id, carries the notification id

    var formData = new FormData(form);

    fetch(url, {
        method: 'POST',
        body: formData,
        headers: {
            'RequestVerificationToken': document.querySelector('input[name="__RequestVerificationToken"]')?.value,
            'X-Requested-With': 'XMLHttpRequest'
        }
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                const listItem = form.closest('.list-group-item');
                listItem.classList.remove('bg-white', 'border-start', 'border-4', 'border-primary');
                listItem.classList.add('bg-light');

                const newBadge = listItem.querySelector('.badge.bg-primary');
                if (newBadge) {
                    newBadge.remove();
                }

                const buttonGroup = form.closest('.d-flex.gap-1');
                if (action === 'accept') {
                    buttonGroup.innerHTML = '<span class="text-success"><i class="bi bi-check-circle-fill"></i> Accepted</span>';
                } else {
                    buttonGroup.innerHTML = '<span class="text-danger"><i class="bi bi-x-circle-fill"></i> Declined</span>';
                }

                var badge = document.getElementById('sidebarUnread');
                if (badge) {
                    var count = (typeof data.unreadCount === 'number') ? data.unreadCount : 0;
                    if (count > 0) {
                        badge.textContent = count > 99 ? '99+' : count;
                        badge.classList.remove('d-none');
                    } else {
                        badge.classList.add('d-none');
                    }
                }
            }
        })
        .catch(error => {
            console.error('Error:', error);
            form.submit();
        });
});
