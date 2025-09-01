
/*

import persist from "./lib/persist.js";
import flatpickr from "./lib/flatpicker.js";
import Dropzone from "./lib/dropzone.js";




Alpine.plugin(persist);
window.Alpine = Alpine;
Alpine.start();

// Init flatpickr
flatpickr(".datepicker", {
    mode: "range",
    // Admin page utilities (guard DOM queries so this file can be loaded on any page)
    document.documentElement.classList.toggle('dark', localStorage.theme === 'dark' || (!('theme' in localStorage) && window.matchMedia('(prefers-color-scheme: dark)').matches));

    document.addEventListener('DOMContentLoaded', function () {
        // Toggle mobile sidebar
        const menuBtn = document.getElementById('menu');
        const sidebar = document.getElementById('sidebar');
        const overlay = document.getElementById('overlay');

        if (menuBtn && sidebar && overlay) {
            menuBtn.addEventListener('click', function () {
                sidebar.classList.toggle('mobile-open');
                overlay.classList.toggle('hidden');
            });

            overlay.addEventListener('click', function () {
                sidebar.classList.remove('mobile-open');
                this.classList.add('hidden');
            });
        }

        // Toggle dropdowns (only bind when toggles exist)
        const toggles = document.querySelectorAll('[id$="-toggle"]');
        if (toggles && toggles.length) {
            toggles.forEach(toggle => {
                toggle.addEventListener('click', function () {
                    const targetId = this.id.replace('-toggle', '-dropdown');
                    const iconId = this.id.replace('-toggle', '-dropdown-icon');
                    const dropdown = document.getElementById(targetId);
                    const icon = document.getElementById(iconId);
                    if (dropdown) dropdown.classList.toggle('hidden');
                    if (icon) icon.classList.toggle('rotate-180');
                });
            });
        }
    });
}

// For Copy//
document.addEventListener("DOMContentLoaded", () => {
    const copyInput = document.getElementById("copy-input");
    if (copyInput) {
        // Select the copy button and input field
        const copyButton = document.getElementById("copy-button");
        const copyText = document.getElementById("copy-text");
        const websiteInput = document.getElementById("website-input");

        // Event listener for the copy button
        copyButton.addEventListener("click", () => {
            // Copy the input value to the clipboard
            navigator.clipboard.writeText(websiteInput.value).then(() => {
                // Change the text to "Copied"
                copyText.textContent = "Copied";

                // Reset the text back to "Copy" after 2 seconds
                setTimeout(() => {
                    copyText.textContent = "Copy";
                }, 2000);
            });
        });
    }
});

document.addEventListener("DOMContentLoaded", function () {
    const searchInput = document.getElementById("search-input");
    const searchButton = document.getElementById("search-button");

    // Function to focus the search input
    function focusSearchInput() {
        searchInput.focus();
    }

    // Add click event listener to the search button
    searchButton.addEventListener("click", focusSearchInput);

    // Add keyboard event listener for Cmd+K (Mac) or Ctrl+K (Windows/Linux)
    document.addEventListener("keydown", function (event) {
        if ((event.metaKey || event.ctrlKey) && event.key === "k") {
            event.preventDefault(); // Prevent the default browser behavior
            focusSearchInput();
        }
    });

    // Add keyboard event listener for "/" key
    document.addEventListener("keydown", function (event) {
        if (event.key === "/" && document.activeElement !== searchInput) {
            event.preventDefault(); // Prevent the "/" character from being typed
            focusSearchInput();
        }
    });
});

*/
document.documentElement.classList.toggle("dark", localStorage.theme === "dark" || (!("theme" in localStorage) && window.matchMedia("(prefers-color-scheme: dark)").matches),);
document.addEventListener('DOMContentLoaded', function () {
    // Toggle mobile sidebar
    document.getElementById('menu').addEventListener('click', function () {
        const sidebar = document.getElementById('sidebar');
        const overlay = document.getElementById('overlay');

        sidebar.classList.toggle('mobile-open');
        overlay.classList.toggle('hidden');
    });

    // Close sidebar when clicking on overlay
    document.getElementById('overlay').addEventListener('click', function () {
        const sidebar = document.getElementById('sidebar');
        sidebar.classList.remove('mobile-open');
        this.classList.add('hidden');
    });

    // Toggle dropdowns
    document.querySelectorAll('[id$="-toggle"]').forEach(toggle => {
        toggle.addEventListener('click', function () {
            const targetId = this.id.replace('-toggle', '-dropdown');
            const iconId = this.id.replace('-toggle', '-dropdown-icon');
            const dropdown = document.getElementById(targetId);
            const icon = document.getElementById(iconId);

            dropdown.classList.toggle('hidden');
            icon.classList.toggle('rotate-180');
        });
    });

});