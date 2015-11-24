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

    //checkbox required validation
    $.validator.unobtrusive.adapters.add("checkboxtrue", function (options)
    {
        if (options.element.tagName.toUpperCase() == "INPUT" && options.element.type.toUpperCase() == "CHECKBOX")
        {
            options.rules["required"] = true;
            if (options.message)
            {
                options.messages["required"] = options.message;
            }
        }
    });

    $(document).ready(function ()
    {
        $( ".date-picker" ).datepicker();
    });
})(jQuery);

var successMessage;
var defaultModalSize = 461;

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

	$("body").on("click", "[data-back-button]", function () {
		var customNav = $(this).attr("data-back-button");
		if (customNav) {
			location.href = customNav;
		} else {
			history.back();
		}
		
		return false;
	});

	$(".phone-mask").mask("(999) 999-9999? x99999");

	$(".tabs-control").tabs();

	if (successMessage) {
		notifySuccess(successMessage);
	}
});

function confirmAction(successCallback, errorCallback, text) {
	var message = "Are you sure?";
	if (text != null) {
		message = text;
	}

	$('<div id="dialog-confirm" title="Confirm your action">' +
		'<p>' + message + '</p></div>').dialog({
		resizable: false,
		modal: true,
		buttons: [
			{
				text: "Ok",
				'class': "main-dialog-button",
				click: function () {
					if (successCallback) {
						successCallback();
					}
					$(this).dialog("close");
				}
			},
			{
				text: "Cancel",
				click: function() {
					if (errorCallback) {
						errorCallback();
					}
					$(this).dialog("close");
				}
			}
		]
	});
}

function reparseElementValidators(selector) {
	var $form = $(selector);
	$form.removeData("validator").removeData("unobtrusiveValidation");
	$.validator.unobtrusive.parse($form);
}

function notifySuccess(text) {
	var message = "Successfully saved";
	if (text != null) {
		message = text;
	}

	$.toast({
		heading: 'Success',
		text: message,
		showHideTransition: 'slide',
		icon: 'success',
		position: 'bottom-right'
	})
}

function notifyError(text) {
	var message = "Unknown error occured";
	if (text != null) {
		message = text;
	}

	$.toast({
		heading: 'Error',
		text: message,
		showHideTransition: 'slide',
		icon: 'error',
		position: 'bottom-right'
	})
}

function getLast4(str) {
	if (str == null)
		return undefined;
	var start = str.length - 4;
	if (start < 0)
		start = 0;
	return str.slice(start, str.length);
};

function reInitFormValidation(form)
{
    if ($(form).length > 0)
    {
        $(form).removeData("validator").removeData("unobtrusiveValidation");
        jQuery.validator.unobtrusive.parse($(form));
    }
};