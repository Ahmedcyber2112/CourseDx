console.log("confirm.js loaded");

// Wait for DOM to be ready
document.addEventListener("DOMContentLoaded", function () {
    console.log("DOM fully loaded");

    function showDeleteModal(id, name) {
        const controller = window.deleteConfig?.controller || 'Admin';
        const action = window.deleteConfig?.action || 'DeleteConfirmed';

        const form = document.getElementById('deleteForm');
        const modal = document.getElementById('deleteModal');
        const userNameDiv = document.querySelector('.user-name-div');
        const userNameSpan = document.getElementById('modalUserName');

        if (!form) {
            console.error("Delete form not found! Check ID: deleteForm");
            return;
        }
        if (!modal) {
            console.error("Delete modal not found! Check ID: deleteModal");
            return;
        }

        // Set action URL
        form.setAttribute('action', `/${controller}/${action}/${id}`);

        // Handle name display
        if (name && userNameDiv && userNameSpan) {
            userNameSpan.textContent = name;
            userNameDiv.classList.remove('hidden');
        } else if (userNameDiv) {
            userNameDiv.classList.add('hidden');
        }

        // Show modal
        modal.classList.remove('hidden');
    }

    function showAdminWarningModal(userName) {
        const modal = document.getElementById('adminWarningModal');
        if (modal) {
            modal.classList.remove('hidden');
        }
    }

    function closeAdminWarningModal() {
        const modal = document.getElementById('adminWarningModal');
        if (modal) {
            modal.classList.add('hidden');
        }
    }

    function closeModal() {
        const modal = document.getElementById('deleteModal');
        if (modal) modal.classList.add('hidden');
    }

    // Make functions globally accessible
    window.closeModal = closeModal;
    window.closeAdminWarningModal = closeAdminWarningModal;

    // Attach to all delete links
    document.addEventListener('click', function (e) {
        const link = e.target.closest('.delete-btn');
        if (link) {
            e.preventDefault();
            const id = link.getAttribute('data-id');
            const name = link.getAttribute('data-name');
            const isAdmin = link.getAttribute('data-isadmin');

            // Best of both worlds: Check both username AND isAdmin property
            const isProtectedAdmin = (name && name.toLowerCase() === 'admin') ||
                (isAdmin === 'true');

            if (isProtectedAdmin) {
                // Show admin warning instead of delete confirmation
                showAdminWarningModal(name);
                return;
            }

            if (id && name) {
                showDeleteModal(id, name);
            } else {
                console.warn("Delete link missing data-id or data-name");
            }
        }
    });

    // Close on ESC
    document.addEventListener('keydown', e => {
        if (e.key === 'Escape') {
            closeModal();
            closeAdminWarningModal();
        }
    });

    // Close on backdrop click
    document.getElementById('deleteModal')?.addEventListener('click', function (e) {
        if (e.target === this) closeModal();
    });

    document.getElementById('adminWarningModal')?.addEventListener('click', function (e) {
        if (e.target === this) closeAdminWarningModal();
    });

    // Handle Cancel button click
    document.querySelector('#deleteModal button[type="button"]')?.addEventListener('click', closeModal);
});