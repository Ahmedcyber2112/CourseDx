// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

import ApexCharts from "apexcharts";
/*this is for the Main Page only */

// State variables
let displayText = '';
let showLine = false;
let showElements = false;
let animateSelf = false;
let chartVisible = false;
const fullText = "Invest in your Self";

// DOM elements
const heroHeading = document.getElementById('hero-heading');
const animatedUnderline = document.getElementById('animated-underline');
const heroParagraph = document.getElementById('hero-paragraph');
const heroButtons = document.getElementById('hero-buttons');
const chartContainer = document.getElementById('chart-container');


//background element i added it here to improve performance 
document.addEventListener('DOMContentLoaded', function () {
    const backgroundDivs = `
        <div class="absolute top-1/4 left-1/4 w-64 h-64 bg-yellow-400 rounded-full filter blur-3xl animate-pulse-slow opacity-40"></div>
        <div class="absolute bottom-1/4 right-1/4 w-96 h-96 bg-purple-500 rounded-full filter blur-3xl opacity-50 animate-pulse-slow delay-1000"></div>
    `;
    document.querySelector('#Background-element').innerHTML = backgroundDivs;
});
// Typing animation

function startTypingAnimation() {
    let index = 0;
    const timer = setInterval(() => {
        if (index < fullText.length) {
            displayText = fullText.substring(0, index + 1);
            heroHeading.innerHTML = displayText.includes("Self")
                ? `Invest in your <span id="self-text" class="inline-block px-1 text-gray-300 transition-all duration-500">Self</span>`
                : displayText;

            if (displayText !== fullText) {
                heroHeading.innerHTML += '<span id="typing-cursor" class="typing-cursor"></span>';
            }

            index++;
        } else {
            clearInterval(timer);

            setTimeout(() => {
                showLine = true;
                animatedUnderline.classList.remove('w-0');
                animatedUnderline.classList.add('w-full');
                updateUnderlineWidth();
            }, 200);

            setTimeout(() => {
                animateSelf = true;
                const selfTextElement = document.getElementById('self-text');
                if (selfTextElement) {
                    selfTextElement.classList.remove('text-gray-300');
                    selfTextElement.classList.add('text-yellow-500');
                }
            }, 400);

            setTimeout(() => {
                showElements = true;
                if (heroParagraph) heroParagraph.classList.remove('opacity-0', 'translate-y-4');
                if (heroButtons) heroButtons.classList.remove('opacity-0', 'translate-y-4');
            }, 600);
        }
    }, 80);
}

// Update underline width
function updateUnderlineWidth() {
    const heading = document.getElementById('hero-heading');
    const underline = document.getElementById('animated-underline');
    if (heading && underline) {
        const headingWidth = heading.offsetWidth;
        underline.style.maxWidth = (headingWidth + 20) + 'px';
    }
}

// Theme-aware color function
function getThemeColors() {
    const isDarkMode = document.documentElement.classList.contains('dark');
    return {
        backgroundColor: isDarkMode ? '#1f2937' : 'transparent',
        gridBorderColor: isDarkMode ? '#374151' : '#e5e5e5',
        textColor: isDarkMode ? '#d1d5db' : '#4B5563',
        axisColor: isDarkMode ? '#9ca3af' : '#D4AF37',
        lineColors: isDarkMode ? ['#D4AF37', '#E8D18F'] : ['#D4AF37', '#E8D18F']
    };
}

// Chart initialization
function initChart() {
    const themeColors = getThemeColors();

    const chartOptions = {
        chart: {
            type: 'line',
            height: '100%',
            background: themeColors.backgroundColor,
            toolbar: { show: false },
            animations: {
                enabled: chartVisible,
                speed: 1000,
                animateGradually: { enabled: true, delay: 100 }
            }
        },
        colors: themeColors.lineColors,
        stroke: { curve: 'smooth', width: 3 },
        grid: {
            borderColor: themeColors.gridBorderColor,
            xaxis: { lines: { show: true } },
            yaxis: { lines: { show: true } }
        },
        xaxis: {
            categories: ['2018', '2019', '2020', '2021', '2022', '2023'],
            labels: {
                style: { colors: themeColors.textColor, fontSize: '14px' }
            },
            axisBorder: { show: true, color: themeColors.axisColor },
            axisTicks: { show: true, color: themeColors.axisColor }
        },
        yaxis: {
            labels: {
                style: { colors: themeColors.textColor, fontSize: '14px' },
                formatter: (value) => `$${value}K`
            }
        },
        legend: {
            show: true,
            position: 'top',
            horizontalAlign: 'center',
            labels: { colors: themeColors.textColor }
        },
        tooltip: {
            theme: 'dark',
            y: { formatter: (value) => `$${value}K` }
        }
    };

    const chartSeries = [
        { name: 'US Average', data: [45, 47, 49, 50, 51, 52] },
        { name: 'IT Average', data: [85, 90, 95, 100, 105, 110] }
    ];

    const chartElement = document.querySelector("#chart");
    if (chartElement) {
        const chart = new ApexCharts(chartElement, {
            ...chartOptions,
            series: chartSeries,
            responsive: [{
                breakpoint: 768,
                options: {
                    chart: { height: 260 },
                    xaxis: { labels: { style: { fontSize: '12px' } } },
                    yaxis: { labels: { style: { fontSize: '12px' } } }
                }
            }]
        });

        chart.render();
    }
}

// Scroll event for chart animation
function handleScroll() {
    if (!chartVisible && chartContainer) {
        const rect = chartContainer.getBoundingClientRect();
        const triggerPoint = window.innerHeight * 0.8;
        const isVisible = rect.top < triggerPoint && rect.bottom >= 0;

        if (isVisible) {
            chartVisible = true;
            initChart();
        }
    }
}

// Initialize components after DOM is loaded
document.addEventListener('DOMContentLoaded', () => {
    startTypingAnimation();
    window.addEventListener('scroll', handleScroll);
    handleScroll(); // Check on load
    window.addEventListener('resize', updateUnderlineWidth);
});
