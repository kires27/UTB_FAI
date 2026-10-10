// Admin event creation: attendee search/filter, owner exclusion and
// selection restore, plus the recurring-fields toggle.
document.addEventListener('DOMContentLoaded', function () {
    const userSearch = document.getElementById('userSearch');
    const attendeeSelect = document.getElementById('attendeeSelect');
    const ownerSelect = document.getElementById('ownerSelect');

    if (userSearch && attendeeSelect && ownerSelect) {
        const allOptions = Array.from(attendeeSelect.options);
        const selectedValuesList = [];

        // Store current selections
        attendeeSelect.addEventListener('change', function () {
            selectedValuesList.length = 0; // Clear array
            Array.from(this.selectedOptions).forEach(option => {
                selectedValuesList.push(parseInt(option.value));
            });
        });

        // Search functionality
        userSearch.addEventListener('input', function () {
            const searchTerm = this.value.toLowerCase();
            const selectedOwnerId = parseInt(ownerSelect.value);

            // Only filter if search term is at least 2 characters or empty
            if (searchTerm.length < 2 && searchTerm !== '') {
                return;
            }

            // Filter options based on search and exclude owner
            const filteredOptions = allOptions.filter(option => {
                const userId = parseInt(option.value);
                const emailMatch = searchTerm === '' || option.textContent.toLowerCase().includes(searchTerm);
                const notOwner = isNaN(selectedOwnerId) || userId !== selectedOwnerId;
                return emailMatch && notOwner;
            });

            // Update select options
            attendeeSelect.innerHTML = '';
            filteredOptions.forEach(option => {
                const newOption = option.cloneNode(true);
                // Restore selection if this option was previously selected
                if (selectedValuesList.includes(parseInt(newOption.value))) {
                    newOption.selected = true;
                }
                attendeeSelect.add(newOption);
            });
        });

        // Update attendee list when owner changes
        ownerSelect.addEventListener('change', function () {
            userSearch.dispatchEvent(new Event('input')); // Trigger search filter
        });
    }

    // Recurring fields toggle
    const isRecurringCheckbox = document.querySelector('input[name="IsRecurring"]');
    const recurringFields = document.getElementById('recurringFields');

    if (isRecurringCheckbox && recurringFields) {
        isRecurringCheckbox.addEventListener('change', function () {
            recurringFields.style.display = this.checked ? 'block' : 'none';
        });
    }
});
