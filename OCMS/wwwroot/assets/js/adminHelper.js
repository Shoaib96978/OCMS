/**
* OCMS Admin JS
* Sidebar toggle + AJAX helpers for admin pages
*/
$(function () {
    'use strict';

    /* ── Sidebar Toggle ────────────────────────────────── */
    $('#sidebar-toggle').on('click', function () {
        const $sidebar = $('#admin-sidebar');
        if ($(window).width() <= 900) {
            $sidebar.toggleClass('mobile-open');
        } else {
            $sidebar.toggleClass('collapsed');
        }
    });

    /* ── Active nav link ───────────────────────────────── */
    const path = window.location.pathname.split('/').pop();
    $('.sidebar-link').each(function () {
        const href = $(this).data('href') || '';
        if (href && path.includes(href)) $(this).addClass('active');
    });

});