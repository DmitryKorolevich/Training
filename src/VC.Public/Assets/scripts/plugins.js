// Avoid `console` errors in browsers that lack a console.
(function() {
    var method;
    var noop = function () {};
    var methods = [
        'assert', 'clear', 'count', 'debug', 'dir', 'dirxml', 'error',
        'exception', 'group', 'groupCollapsed', 'groupEnd', 'info', 'log',
        'markTimeline', 'profile', 'profileEnd', 'table', 'time', 'timeEnd',
        'timeline', 'timelineEnd', 'timeStamp', 'trace', 'warn'
    ];
    var length = methods.length;
    var console = (window.console = window.console || {});

    while (length--) {
        method = methods[length];

        // Only stub undefined methods.
        if (!console[method]) {
            console[method] = noop;
        }
    }
}());

// Place any jQuery/helper plugins in here.

(function ($) {
	$.support.placeholder = ('placeholder' in document.createElement('input'));
})(jQuery);


$(function () {
	//fix for IE7 and IE8
	if (!$.support.placeholder) {
		$("[placeholder]").focus(function () {
			if ($(this).val() == $(this).attr("placeholder")) $(this).val("");
		}).blur(function () {
			if ($(this).val() == "") $(this).val($(this).attr("placeholder"));
		}).blur();

		$("[placeholder]").parents("form").submit(function () {
			$(this).find('[placeholder]').each(function () {
				if ($(this).val() == $(this).attr("placeholder")) {
					$(this).val("");
				}
			});
		});
	}

	$('#slider').nivoSlider({
		effect: 'fade', // Specify sets like: 'fold,fade,sliceDown'
		animSpeed: 1000, //Sets animation speed to 1 second
		pauseTime: 10000, //Sets slide duration to 10 seconds
		startSlide: 0, // Set starting Slide (0 index)
		directionNav: true, // Next & Prev navigation ***default true***
		directionNavHide: false, // Only show on hover
		controlNav: false, // 1,2,3... navigation
		keyboardNav: true, // Use left & right arrows
		pauseOnHover: true, // Stop animation while hovering
		captionOpacity: 0.8, // Universal caption opacity
		randomStart: false, // Start on a random slide
	});

	$("#menuSidebar").accordion({
		accordion: true
	});

	$("body").on("click", ".back-button", function() {
		history.back();
		return false;
	});
});