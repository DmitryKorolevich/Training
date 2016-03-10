var loaded = false;

window.addEventListener("load", function ()
{
    var setSelectedSku = function ()
    {
        var jChecked = $("input[name=sku]:checked");

        var inStock = jChecked.data("in-stock");
        if (inStock)
        {
            $(".product-action-right .in-stock").show();
            $(".product-action-right .out-of-stock").hide();
            $("#spSelectedPrice").text("Selected Price " + jChecked.attr("data-price"));
            if (jChecked.attr("data-autoship").toLowerCase() == "true") {
	            $(".product-autoship-container").show();
            } else {
            	$(".product-autoship-container").hide();
            }
        }
        else
        {
            $(".product-action-right .in-stock").hide();
            $(".product-action-right .out-of-stock").show();
        }
        $("#hSelectedCode").text("Product #" + jChecked.val());
    };

    $("input[name=sku]:first").attr("checked", true);
    setSelectedSku();

	$("body").on("click", "#lnkReviewsTab", function () {
		$(".tabs-control").tabs({ "active": 1 });
	});

	$("body").on("click", "#lnkDescriptionTab", function () {
		$(".tabs-control").tabs({ "active": 0 });
	});

	$("body").on("click", "#btnVideoClose", function () {
		stopVideo();
		return false;
	});

	$("body").on("change", "input[name=sku]", setSelectedSku);

	$("body").on("click", "#lnkAddToCart", addToCart);
	$("body").on("click", "#lnkClose", closeCartLite);

	$("body").on("click", ".proposals-item-link", addCrossToCart);

	$(".product-action-right .out-of-stock a").click(function (e)
	{
	    $.ajax({
	        url: "/Product/AddOutOfStockProductRequest/" + productPublicId,
	        dataType: "html"
	    }).success(function (result)
	    {
	        $(result).dialog({
	            resizable: false,
	            modal: true,
	            minWidth: defaultModalSize,
	            dialogClass: "add-out-of-stock-request-dialog",
	            open: function ()
	            {
	                grecaptcha.render('googleCaptcha', {
	                    'sitekey': captchaSiteKey
	                });
	            },
	            close: function ()
	            {
	                $(this).dialog('destroy').remove();
	            },
	            buttons: [
					{
					    text: "Submit Out of Stock Request",
					    'class': "main-dialog-button",
					    click: function ()
					    {
					        var selector = "#addOutOfStockProductRequestDialog form";
					        var jForm = $(selector);
					        reparseElementValidators(selector);
					        jForm.validate()
					        if (jForm.valid())
					        {
					            jForm.submit();
					        }
					    }
					},
					{
					    text: "Cancel",
					    click: function ()
					    {
					        $(this).dialog("close");
					    }
					}
	            ]
	        });
	    }).error(function (result)
	    {
	        notifyError();
	    });
        
	    e.preventDefault();
	    return false;
	});

	$.each([1, 2, 3, 4], function (index, targetNumber) {
		$("#yPlayer" + targetNumber).wrap("<div id=up" + targetNumber + "></div>")

		$("#up" + targetNumber)
			.dialog({
				resizable: false,
				modal: true,
				dialogClass: "youtube-dialog"
			}).dialog("close");
	});

	$('body').on("click", "a[data-video-id]", function () {
		if (loaded) {
			var videoId = $(this).attr("data-video-id");

			var index = 0;
			$.each($("a[data-video-id]"), function (ind, elem) {
				if ($(elem).attr("data-video-id") == videoId) {
					index = ind + 1;
					return;
				}
			});

			$('#' + $("#up" + index).parent().attr('aria-describedby')).dialog('open');

			var jCloseButton = $("<a id='btnVideoClose' class='youtube-popup-close' href='#'>" +
							"	<img src='/assets/images/close_button.png'/>" +
							"</a>");

			var jYoutube = $("#up" + index).parent();

			jCloseButton.css("top", jYoutube.css("top"));
			jCloseButton.css("left", jYoutube.offset().left + jYoutube.width());

			jYoutube.before(jCloseButton);

			playVideo(index - 1);
		}

		return false;
	});

	var tag = document.createElement('script');
	tag.src = "https://www.youtube.com/player_api";
	var firstScriptTag = document.getElementsByTagName('script')[0];
	firstScriptTag.parentNode.insertBefore(tag, firstScriptTag);
}, false);

$(document).ready(function ()
{
    var review = getQueryParameterByName("review");
    if (review=='true')
    {
        productPageAddReview();
    }
});

function addOutOfStockProductRequestFormSubmitSuccess(data)
{
    ajaxFormSubmitSuccess();
    if (successMessage)
    {
        $("#addOutOfStockProductRequestDialog").dialog("close");
    }
}

var players = [];

function onYouTubePlayerAPIReady() {
	var videoId = $($("a[data-video-id]")[0]).attr("data-video-id");

	players[0] = new YT.Player('yPlayer1', {
		videoId: videoId,
		playerVars: { iv_load_policy: 3, rel: 0, showinfo: 0, wmode: 'opaque', enablejsapi: 1, origin: location.origin },
		events: {
			'onStateChange': onPlayerStateChange
		}
	});

	videoId = $($("a[data-video-id]")[1]).attr("data-video-id");

	players[1] = new YT.Player('yPlayer2', {
		videoId: videoId,
		playerVars: { iv_load_policy: 3, rel: 0, showinfo: 0, wmode: 'opaque', enablejsapi: 1, origin: location.origin },
		events: {
			'onStateChange': onPlayerStateChange
		}
	});

	videoId = $($("a[data-video-id]")[2]).attr("data-video-id");

	players[2] = new YT.Player('yPlayer3', {
		videoId: videoId,
		playerVars: { iv_load_policy: 3, rel: 0, showinfo: 0, wmode: 'opaque', enablejsapi: 1, origin: location.origin },
		events: {
			'onStateChange': onPlayerStateChange
		}
	});

	videoId = $($("a[data-video-id]")[3]).attr("data-video-id");

	players[3] = new YT.Player('yPlayer4', {
		videoId: videoId,
		playerVars: { iv_load_policy: 3, rel: 0, showinfo: 0, wmode: 'opaque', enablejsapi: 1, origin: location.origin },
		events: {
			'onStateChange': onPlayerStateChange
		}
	});

	loaded = true;
}

function onPlayerStateChange(event) {
	if (event.data == 0) {
		stopVideo();
	}
}
function stopVideo() {
	players[0].stopVideo();
	players[1].stopVideo();
	players[2].stopVideo();
	players[3].stopVideo();


	$("#btnVideoClose").remove();
	$('#' + $("#up" + 1).parent().attr('aria-describedby')).dialog('close');
	$('#' + $("#up" + 2).parent().attr('aria-describedby')).dialog('close');
	$('#' + $("#up" + 3).parent().attr('aria-describedby')).dialog('close');
	$('#' + $("#up" + 4).parent().attr('aria-describedby')).dialog('close');
}

function playVideo(index) {
	players[index].seekTo(0);
	players[index].playVideo();
}

function addToCart(event, sku) {
	if (!sku) {
		var jChecked = $("input[name=sku]:checked");
		sku = jChecked.val();
	}

	$.ajax({
		url: "/Cart/AddToCartView?skuCode=" + sku,
		dataType: "html",
		type: "POST"
	}).success(function (result) {
		$(result).dialog({
			resizable: false,
			modal: true,
			minWidth: 520,
			close: function() {
				$(this).dialog('destroy').remove();
			}
		});
	}).error(function(result) {
		notifyError();
	});

	return false;
}

function addCrossToCart() {
	closeCartLite();
	addToCart(null, $(this).attr("data-sku-code"));
	return false;
}

function closeCartLite() {
	$("#" + $("#lnkClose").closest("div[aria-describedby]").attr('aria-describedby')).dialog('close');
	return false;
}