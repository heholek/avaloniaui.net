(function ($) {
  'use strict';

  // Sticky Menu
  $(window).scroll(function () {
    var header = $('header.toplevel');
    if (header.offset().top > 10) {
      header.addClass('bg');
    } else {
      header.removeClass('bg');
    }
  });
})(jQuery)
