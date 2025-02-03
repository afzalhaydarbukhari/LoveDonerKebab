document.addEventListener('DOMContentLoaded', function () {
    const toggle = document.querySelector('.rd-navbar-toggle');
    const navWrap = document.querySelector('.rd-navbar-nav-wrap');
    const searchToggle = document.querySelector('.rd-navbar-search-toggle');
    const searchForm = document.querySelector('.rd-search');

    toggle.addEventListener('click', function () {
        navWrap.classList.toggle('active');
    });

    searchToggle.addEventListener('click', function () {
        searchForm.classList.toggle('active');
    });

    // Close menu when clicking outside
    document.addEventListener('click', function (event) {
        if (!event.target.closest('.rd-navbar-main') && navWrap.classList.contains('active')) {
            navWrap.classList.remove('active');
        }
        if (!event.target.closest('.rd-navbar-search') && searchForm.classList.contains('active')) {
            searchForm.classList.remove('active');
        }
    });

    // Handle window resize
    window.addEventListener('resize', function () {
        if (window.innerWidth > 991) {
            navWrap.classList.remove('active');
            searchForm.classList.remove('active');
        }
    });
});