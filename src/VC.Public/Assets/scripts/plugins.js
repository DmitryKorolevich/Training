var lhnAccountN = 7307;
var lhnButtonN = 5138;
var lhnVersion = 5.3;
var lhnJsHost = (("https:" == document.location.protocol) ? "https://" : "http://");
var lhnInviteEnabled = 1;
var lhnInviteChime = 0;
var lhnWindowN = 0;
var lhnDepartmentN = 0;
var lhnCustomInvitation = '';
var lhnCustom1 = '';
var lhnCustom2 = '';
var lhnCustom3 = '';
var lhnTrackingEnabled = 't';
var lhnScriptSrc = lhnJsHost + 'www.livehelpnow.net/lhn/scripts/livehelpnow.aspx?lhnid=' + lhnAccountN + '&iv=' + lhnInviteEnabled + '&d=' + lhnDepartmentN + '&ver=' + lhnVersion + '&rnd=' + Math.random();
var lhnScript = document.createElement("script");
lhnScript.setAttribute("defer", "defer");
lhnScript.type = "text/javascript";
lhnScript.src = lhnScriptSrc;

var googleSearchcx = '006613472277305802095:2wviofnvpvs';
var googleSearchFAQcx = '006613472277305802095:ne3iw8mrxlk';


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

var settingsLeft = {
	content: undefined,
	contentAsHTML: true,
	animation: 'grow',
	delay: 0,
	theme: 'tooltipster-default',
	touchDevices: false,
	trigger: 'hover',
	interactive: 'true',
	position: 'left',
	offsetY: 0,
	fixedWidth: 250,
	maxWidth: 250,
	onlyOne: true
};

var settingsVertical = {
	content: undefined,
	contentAsHTML: true,
	animation: 'grow',
	delay: 0,
	theme: 'tooltipster-default',
	touchDevices: false,
	trigger: 'hover',
	interactive: 'true',
	position: 'right',
	offsetY: 0,
	fixedWidth: 250,
	maxWidth: 250,
	onlyOne: true
};

var settingsHorizontal = {
	content: undefined,
	contentAsHTML: true,
	animation: 'grow',
	delay: 0,
	theme: 'tooltipster-default',
	touchDevices: false,
	trigger: 'hover',
	interactive: 'true',
	position: 'top',
	offsetY: 60,
	fixedWidth: 250,
	maxWidth: 250,
	onlyOne: true
};
var getBaseHtml = function (title, body) {
	var toReturn = '<span class="default"><strong class="title">{0}</strong><br><br>{1}</span>'.format(title, body);
	return toReturn;
};

var Google = {
    LoadActions: [],
};

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

    Google.LoadActions.push(function ()
    {
        google.load('search', '1', { language: 'en', callback: googleAutoCompleteInit });
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

    	//tooltips
	    registerTooltips();

        $('.small-window-open-link').click(function(e){
            var href= $(this).attr('href');
            if(href)
            {
                window.open(href,'', 'menubar=no,toolbar=no,resizable=yes,scrollbars=yes,height=600,width=600');
                e.preventDefault();
            }
        });

        $('textarea.line-limit').keydown(function (e)
        {
            var maxLines = $(this).data('line-limit');
            if (maxLines)
            {
                var keynum;
                if (window.event)
                {
                    keynum = e.keyCode;
                } else if (e.which)
                {
                    keynum = e.which;
                }
                if (this.value.split('\n').length > maxLines && keynum !== 8 && keynum !== 46)
                {
                    return false;
                }
            }
        });

        $('#search-area-form input[type=text]').focusout(function (){
            $('#search-area-form .active').hide();
            $('#search-area-form .not-active').show();
        });

        $('#search-area-form input[type=text]').focusin(function (){
            $('#search-area-form .active').show();
            $('#search-area-form .not-active').hide();            
        });
    });
})(jQuery);

function googleAutoCompleteInit()
{
    google.search.CustomSearchControl.attachAutoCompletion(
           googleSearchcx,
           $('.search-area form input[type=text]').get(0),
           'search-area-form');
};

function googleLoadCallback()
{
    if (Google && Google.LoadActions)
    {
        $.each(Google.LoadActions, function (key, item)
        {
            item();
        });
    }
};

function registerTooltips() {
	$('.tooltip-v').each(function () {
		var title = $(this).data("tooltip-title");
		var body = $(this).data("tooltip-body");
		settingsVertical.content = getBaseHtml(title, body);
		$(this).tooltipster(settingsVertical);
	});
	$('.tooltip-h').each(function () {
		var title = $(this).data("tooltip-title");
		var body = $(this).data("tooltip-body");
		settingsHorizontal.content = getBaseHtml(title, body);
		$(this).tooltipster(settingsHorizontal);
	});
	$('.tooltip-l').each(function () {
		var title = $(this).data("tooltip-title");
		var body = $(this).data("tooltip-body");
		settingsLeft.content = getBaseHtml(title, body);
		$(this).tooltipster(settingsLeft);
	});
}

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
    if ($(data).find('form').length > 0)
    {
        reparseElementValidators($(data).find('form').selector);
    }
}

function ajaxFormSubmitError(data)
{
    notifyError("Server error occured");
}

var successMessage;
var defaultModalSize = 461;

$(function () {
    registerRequiredIf();

    Ladda.bind('button[type=submit]');

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

	if (document.getElementById('lhnContainer')) {
		initLiveHelp();
	}

	$.each($("textarea[charcount=true]"), function (index, elem)
	{
		processCharcount({ target: elem });
	});

	$('body').on("keyup", "textarea[charcount=true]", function (elem) { processCharcount(elem); });

	if ($('body a[data-video-id]').length > 0)
	{
	    var popup = $("<div id='up'><div id='yPlayer' class='youtube-popup-container'></div></div>");

	    $( "body" ).append(popup);

	    $("#up").dialog({
	        resizable: false,
	        modal: true,
	        dialogClass: "youtube-dialog"
	    }).dialog("close");

	    var tag = document.createElement('script');
	    tag.src = "https://www.youtube.com/player_api";
	    var firstScriptTag = document.getElementsByTagName('script')[0];
	    firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);

	    $("body").on("click", "#btnVideoClose", function ()
	    {
	        stopModalVideo();
	        return false;
	    });
	};

	$('body').on("click", "a[data-video-id][data-video-modal]", function ()
	{
	    if (YouTubeModalPopup.Loaded)
	    {
	        var videoId = $(this).attr("data-video-id");

	        var params = { iv_load_policy: 3, rel: 0, showinfo: 0, wmode: 'opaque', enablejsapi: 1, origin: location.origin };
	        if ($(this).attr("data-video-autoplay"))
	        {
	            params.autoplay = 1;
	        }
	        var events = {};
	        if ($(this).attr("data-video-autoclose"))
	        {
	            events.onStateChange = onModalPlayerAutoClose;
	        }

	        YouTubeModalPopup.Player = new YT.Player('yPlayer', {
	            videoId: videoId,
	            playerVars: params,
	            events: events,
	        });

	        $("#up").parent().removeClass(function (index, css)
	        {
	            return $.grep(css.split(' '), function (item)
	            {
	                return item.startsWith('youtube-dialog-size');
	            }).join(' ');
	        });
	        var customClass = $(this).attr("data-video-class");
	        if (customClass)
	        {
	            $("#up").parent().addClass(customClass);
	        }
	        else
	        {
	            $("#up").parent().addClass('youtube-dialog-size-standart');
	        }

	        $('#' + $("#up").parent().attr('aria-describedby')).dialog('open');

	        var jCloseButton = $("<a id='btnVideoClose' class='youtube-popup-close' href='#'>" +
							"	<img src='/assets/images/close_button.png'/>" +
							"</a>");

	        var jYoutube = $("#up").parent();

	        jCloseButton.css("top", jYoutube.css("top"));
	        jCloseButton.css("left", jYoutube.offset().left + jYoutube.width());

	        jYoutube.before(jCloseButton);
	    }

	    return false;
	});

	if ($('.bronto-subscribe-top-wrapper').length == 1)
	{
	    var header = $('header');
	    if (Cookies.get('bronto-signup') !== "hidden")
	    {
	        setTimeout(function ()
	        {
	            header.find(".bronto-subscribe-top-wrapper").show();
	            header.find(".bronto-form").show();
	        }, 1000);
	    }
	    $(".btnPostEmail").click(function ()
	    {
	        if (header.find(".bronto-form").is(":visible"))
	        {
	            if (header.find(".txtEmail").val() !== "")
	            {
	                $.get("/Help/SubscribeBronto/" + encodeURIComponent(header.find("#bronto-subEmail").val()), null, function (data, status, xhr)
	                {
	                    if (!data.Data)
	                    {
	                        header.find(".bubble").show();
	                    }
	                    else
	                    {
	                        header.find(".bronto-form").hide();
	                        header.find(".bronto-form-success").show();

	                        //setTimeout(function ()
	                        //{
	                        //    header.find(".bronto-form-success").slideUp(400, function() {
	                        //        header.find(".bronto-subscribe-top-wrapper").hide();
	                        //    });
	                        //}, 5000);
	                        Cookies.set("bronto-signup", "hidden", { expires: 1 });
	                    }
	                });
	            }
	            else
	            {
	                header.find(".bubble").show();
	            }
	        }
	    });

	    header.find(".txtEmail").click(function ()
	    {
	        if (header.find(".bubble").is(":visible"))
	        {
	            header.find(".bubble").hide();
	        }
	    });
	    header.find(".txtEmail").keydown(function (ev)
	    {
	        if (ev.keyCode === 13)
	        {
	            if (header.find(".txtEmail").val() !== "")
	            {
	                header.find(".btnPostEmail").click();
	            }
	            else
	            {
	                header.find(".bubble").show();
	            }
	        }
	    });
	    header.find(".close-form").click(function ()
	    {
	        header.find(".bronto-form").slideUp(400, function() {
	            header.find(".bronto-subscribe-top-wrapper").hide();
	        });
	        Cookies.set("bronto-signup", "hidden", { expires: 1 });
	    });
	};
});

var YouTubeModalPopup = {
    Loaded: false,
    Player: null
};

function destroyYouTubeModalPlayer()
{
    if (YouTubeModalPopup.Player !== null)
    {
        YouTubeModalPopup.Player.destroy();
        YouTubeModalPopup.Player = null;
    }
};

function onYouTubePlayerAPIReady() {
    YouTubeModalPopup.Loaded = true;
};

function onModalPlayerAutoClose(event)
{
    if (event.data == 0) {
        stopModalVideo();
        destroyYouTubeModalPlayer();
    }
}

function stopModalVideo() {
    if(YouTubeModalPopup.Player)
    {
        YouTubeModalPopup.Player.stopVideo();
        destroyYouTubeModalPlayer();
    }

    $("#btnVideoClose").remove();
    $('#' + $("#up").parent().attr('aria-describedby')).dialog('close');
};

function playModalVideo() {
    if(YouTubeModalPopup.Player)
    {
        YouTubeModalPopup.Player.seekTo(0);
        YouTubeModalPopup.Player.playVideo();
    }
};

function processCharcount(ev) {
	var elem = ev.target;

	var max = $(elem).attr("maxlength");
	var len = $(elem).val().length;
	if (len >= max) {
		$(elem).next().html('you have reached the limit');
	} else {
		var char = max - len;
		$(elem).next().html('<b>' + char + '</b> characters remaining');
	}
}

function initLiveHelp() {
	if (window.addEventListener) {
	    window.addEventListener('load',
	        function() {
	            setTimeout(function() {
	                document.getElementById('lhnContainer').appendChild(lhnScript);
	            }, 1);
	        },
	        false);
	}
	else if (window.attachEvent) {
	    window.attachEvent('onload',
	        function() {
	            setTimeout(function() {
	                    document.getElementById('lhnContainer').appendChild(lhnScript);
	                },
	                1);
	        });
	}
}

function confirmAction(successCallback, errorCallback, text) {
	var message = "Are you sure?";
	if (text != null) {
		message = text;
	}

	$('<div id="dialog-confirm" title="Confirm your action">' +
		'<p>' + message + '</p></div>').dialog({
		resizable: false,
		modal: true,
		close: function () {
			$(this).dialog('destroy').remove();
		},
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
		return '';
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

function getQueryParameterByName(name)
{
    var match = RegExp('[?&]' + name + '=([^&]*)').exec(window.location.search);
    return match && decodeURIComponent(match[1].replace(/\+/g, ' '));
}

function registerRequiredIf() {
	$.validator.unobtrusive.adapters.add('requiredif', ['dependentproperty', 'desiredvalue'], function (options) {
		options.rules['requiredif'] = options.params;
		options.messages['requiredif'] = options.message;
	});

	$.validator.addMethod('requiredif', function (value, element, parameters) {
		var desiredvalue = parameters.desiredvalue;
		desiredvalue = (desiredvalue == null ? '' : desiredvalue).toString();
		var controlType = $("input[id$='" + parameters.dependentproperty + "']").attr("type");
		var actualvalue = {}
		if (controlType == "checkbox" || controlType == "radio") {
			var control = $("input[id$='" + parameters.dependentproperty + "']:checked");
			actualvalue = control.val();
		} else {
			actualvalue = $("#" + parameters.dependentproperty).val();
		}
		if ($.trim(desiredvalue).toLowerCase() === $.trim(actualvalue).toLocaleLowerCase()) {
			var isValid = $.validator.methods.required.call(this, value, element, parameters);
			return isValid;
		}
		return true;
	});
}

function trySetFormErrors(result) {
    $("div.validation-summary-errors").html('');
    var globalErrors = [];
    for (var i = 0; i < result.Messages.length; i++) {
        var items = $("span[data-valmsg-for='" + result.Messages[i].Field + "']");
        if (items.length == 0) {
            globalErrors.push(result.Messages[i].Message);
        }
        else {
            if (items.text().length > 0) {
                items.append('<p>' + result.Messages[i].Message + '</p>');
            }
            else {
                items.text(result.Messages[i].Message);
            }
        }
    }
    if (globalErrors.length > 0) {
        var globalBlocks = $("div.validation-summary-errors");
        if (globalBlocks.length > 0) {
            var globalsFormatted = [];
            for (var i = 0; i < globalErrors.length; i++) {
                globalsFormatted.push('<li>' + globalErrors[i] + '</li>');
            }
            globalBlocks.html('<ul>' + globalsFormatted.join() + '</ul>');
        }
        else {
            for (var i = 0; i < globalErrors.length; i++) {
                notifyError(globalErrors[i]);
            }
        }
    }
}

function brontoSignupValidateEmail(e)
{
    var emailRegex = /^(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*|"(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21\x23-\x5b\x5d-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])*")@(?:(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?|\[(?:(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?)\.){3}(?:25[0-5]|2[0-4][0-9]|[01]?[0-9][0-9]?|[a-z0-9-]*[a-z0-9]:(?:[\x01-\x08\x0b\x0c\x0e-\x1f\x21-\x5a\x53-\x7f]|\\[\x01-\x09\x0b\x0c\x0e-\x7f])+)\])$/;

    var email = $(e.target).find('.bronto-email').val();
    if (emailRegex.test(email))
    {
        $(e.target).find(".sugnup-bubble").hide();
        return true;
    }
    else
    {
        $(e.target).find(".sugnup-bubble").show();
        return false;
    }
}