/*this css global for the user side*/


/*
------------------------------------------------------------
*/
// site.js - Global JavaScript for all pages

// Theme management for Tailwind v4
const themeToggle = document.getElementById('theme-toggle');
const sunIcon = document.getElementById('sun-icon');
const moonIcon = document.getElementById('moon-icon');

// Check for saved theme or determine based on system preference
function getPreferredTheme() {
    const savedTheme = localStorage.getItem('theme');
    if (savedTheme) {
        return savedTheme;
    }
    return 'dark'; // Default
}

// Apply theme to document
function setTheme(theme) {
    if (theme === 'dark') {
        document.documentElement.classList.add('dark');
        if (sunIcon) sunIcon.classList.remove('hidden');
        if (moonIcon) moonIcon.classList.add('hidden');
    } else {
        document.documentElement.classList.remove('dark');
        if (sunIcon) sunIcon.classList.add('hidden');
        if (moonIcon) moonIcon.classList.remove('hidden');
    }
    localStorage.setItem('theme', theme);
}

// Toggle theme
function toggleTheme() {
    const isDark = document.documentElement.classList.contains('dark');
    setTheme(isDark ? 'light' : 'dark');
}

// Mobile Search Panel
const mobileSearchPanel = document.getElementById('mobile-search-panel');
const mobileSearchToggle = document.getElementById('mobile-search-toggle');
const mobileSearchClose = document.getElementById('mobile-search-close');
const mobileSearchInput = document.getElementById('mobile-search-input');

let searchActive = false;

function toggleMobileSearch() {
    searchActive = !searchActive;
    if (searchActive) {
        if (mobileSearchPanel) {
            mobileSearchPanel.classList.remove('mobile-search-inactive');
            mobileSearchPanel.classList.add('mobile-search-active');
        }
        setTimeout(() => {
            if (mobileSearchInput) mobileSearchInput.focus();
        }, 300);
    } else {
        if (mobileSearchPanel) {
            mobileSearchPanel.classList.remove('mobile-search-active');
            mobileSearchPanel.classList.add('mobile-search-inactive');
        }
    }
}

if (mobileSearchToggle) {
    mobileSearchToggle.addEventListener('click', toggleMobileSearch);
}
if (mobileSearchClose) {
    mobileSearchClose.addEventListener('click', toggleMobileSearch);
}

// Set initial theme
setTheme(getPreferredTheme());

if (themeToggle) {
    themeToggle.addEventListener('click', toggleTheme);
}



