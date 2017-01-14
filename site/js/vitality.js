/*!
 * Vitality v1.4.0 (http://themes.startbootstrap.com/vitality-v1.4.0)
 * Copyright 2013-2016 Start Bootstrap Themes
 * To use this theme you must have a license purchased at WrapBootstrap (https://wrapbootstrap.com)
 */

// Load WOW.js on non-touch devices
var isPhoneDevice = "ontouchstart" in document.documentElement;
$(document).ready(function() {
    if (isPhoneDevice) {
        //mobile
    } else {
        //desktop               
        // Initialize WOW.js
        wow = new WOW({
            offset: 50
        })
        wow.init();
    }
});

(function($) {
    "use strict"; // Start of use strict

    // Smooth Scrolling: Smooth scrolls to an ID on the current page.
    // To use this feature, add a link on your page that links to an ID, and add the .page-scroll class to the link itself. See the docs for more details.
    $('a.page-scroll').bind('click', function(event) {
        var $anchor = $(this);
        $('html, body').stop().animate({
            scrollTop: ($($anchor.attr('href')).offset().top - 50)
        }, 1250, 'easeInOutExpo');
        event.preventDefault();
    });

    // Activates floating label headings for the contact form.
    $("body").on("input propertychange", ".floating-label-form-group", function(e) {
        $(this).toggleClass("floating-label-form-group-with-value", !!$(e.target).val());
    }).on("focus", ".floating-label-form-group", function() {
        $(this).addClass("floating-label-form-group-with-focus");
    }).on("blur", ".floating-label-form-group", function() {
        $(this).removeClass("floating-label-form-group-with-focus");
    });

    // Closes the Responsive Menu on Menu Item Click
    $('.navbar-collapse ul li a').click(function() {
        $('.navbar-toggle:visible').click();
    });

    // Formstone Background - Video Background Settings
    $("header.video").background({
        source: {
            poster: "img/agency/backgrounds/bg-mobile-fallback.jpg",
            mp4: "mp4/camera.mp4"
        }
    });

    // Scrollspy: Highlights the navigation menu items while scrolling.
    $('body').scrollspy({
        target: '.navbar-fixed-top',
        offset: 51
    })

    // Portfolio Filtering Scripts & Hover Effect
    var filterList = {
        init: function() {

            // MixItUp plugin
            $('#portfoliolist').mixItUp();

        },

    };

    filterList.init();

})(jQuery); // End of use strict
