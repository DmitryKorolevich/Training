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
        $(".date-picker").datepicker();

        $('.drop-menu a.trigger').click(function(e)
        {
            $(this).parent().find('> ul').slideToggle();
            e.preventDefault();
        });

        $('.print-button').click(function (e)
        {
            window.print();
            e.preventDefault();
        });

        $('.content-ajax-form-wrapper').on("click", ".content-form-submit-button", function (e)
        {
            var jForm = $(this).closest('form');
            reparseElementValidators($(this).closest('form'));
            jForm.validate()
            if (jForm.valid())
            {
                jForm.submit();
            }
            e.preventDefault();
        });
    });
})(jQuery);

function onloadRecaptchaCallback()
{
    if ($('.content-ajax-form-wrapper .google-captcha').length > 0 && captchaSiteKey)
    {
        $.each($('.content-ajax-form-wrapper .google-captcha'), function (key, item)
        {
            grecaptcha.render(item, {
                'sitekey': captchaSiteKey
            });
        });
    };
};

function ajaxFormSubmitSuccess(data)
{
    if (successMessage)
    {
        refreshAjaxForm(data);
        notifySuccess(successMessage);
    } else
    {
        refreshAjaxForm(data);
    }
}

function refreshAjaxForm(data)
{
    if ($(data).find('#ddCountry').length > 0 && refreshCountries)
    {
        refreshCountries();
    }
    if ($('form.content-ajax-form .google-captcha').length > 0 && captchaSiteKey)
    {
        $.each($('form.content-ajax-form .google-captcha'), function (key, item)
        {
            grecaptcha.render(item, {
                'sitekey': captchaSiteKey
            });
        });
    };
}

function ajaxFormSubmitError(data)
{
    notifyError("Server error occured");
}

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


String.prototype.format = function ()
{
    return String.format(this, arguments.length == 1 ? arguments[0] : arguments);
};

String.format = function (fSource, fParams)
{
    var _toString = function (fObject, o)
    {
        var ctor = function (fObject)
        {
            if (typeof fObject == 'number')
                return Number;
            else if (typeof fObject == 'boolean')
                return Boolean;
            else if (typeof fObject == 'string')
                return String;
            else
                return fObject.constructor;
        }(fObject);
        var proto = ctor.prototype;
        var tFormatter = typeof fObject != 'string' ? proto ? proto.format || proto.toString : fObject.format || fObject.toString : fObject.toString;
        if (tFormatter)
        {
            if (typeof o == 'undefined' || o == "")
                return tFormatter.call(fObject);
            return tFormatter.call(fObject, o);
        }
        return "";
    };
    if (arguments.length == 1)
        return function ()
        {
            return String.format.apply(null, [fSource].concat(Array.prototype.slice.call(arguments, 0)));
        };
    if (arguments.length == 2 && typeof fParams != 'object' && typeof fParams != 'array')
        fParams = [fParams];
    if (arguments.length > 2)
        fParams = Array.prototype.slice.call(arguments, 1);
    fSource = fSource.replace(/\{\{|\}\}|\{([^}: ]+?)(?::([^}]*?))?\}/g, function (match, num, format)
    {
        if (match == "{{") return "{";
        if (match == "}}") return "}";
        if (typeof fParams[num] != 'undefined' && fParams[num] !== null)
        {
            return _toString(fParams[num], format);
        } else
        {
            return "";
        }
    });
    return fSource;
};