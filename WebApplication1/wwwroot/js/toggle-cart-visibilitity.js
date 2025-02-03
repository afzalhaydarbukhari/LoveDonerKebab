$(document).ready(function () {
    // Toggle cart visibility
    $('.rd-navbar-basket').on('click', function () {
        $('.cart-inline').toggleClass('active');

    });

    // Toggle search bar visibility
    $('.rd-navbar-search-toggle').on('click', function () {
        $('.rd-navbar-search').toggleClass('active');
    });

});