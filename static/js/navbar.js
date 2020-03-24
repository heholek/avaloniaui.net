$(function() {
  'use strict';

  var navbar = $('#navbar');

  // Sticky Menu
  $(window).scroll(function () {
    if (navbar.offset().top > 10) {
      navbar.addClass('bg');
    } else {
      navbar.removeClass('bg');
    }
  });

  // Hamburger menu
  $('#toplevel-menu-toggle').click(function() {
    navbar.toggleClass('open');
  });
})
