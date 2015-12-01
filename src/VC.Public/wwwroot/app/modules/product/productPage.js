window.addEventListener("load", function () {
	$("input[name=sku]:first").attr("checked", true);

	$("body").on("click", "#lnkReviewsTab", function () {
		$(".tabs-control").tabs({ "active": 1 });
	});

	$("body").on("click", "#lnkDescriptionTab", function () {
		$(".tabs-control").tabs({ "active": 0 });
	});

	$("body").on("click", "#btnVideoClose", function () {
		$("#btnVideoClose").remove();
		$('#' + $(".youtube-dialog").attr('aria-describedby')).dialog('destroy').remove;
		return false;
	});

	$("body").on("change", "input[name=sku]", function () {
		var jChecked = $("input[name=sku]:checked");

		$("#spSelectedPrice").text("Selected Price " + jChecked.attr("data-price"));
		$("#hSelectedCode").text("Product #" + jChecked.val());
	});

	$("body").on("click", ".write-review-link", function () {
		$.ajax({
			url: "/Product/AddReview/" + productPublicId,
			dataType: "html"
		}).success(function (result) {
			$(result).dialog({
				resizable: false,
				modal: true,
				minWidth: defaultModalSize,
				dialogClass: "product-reviews-dialog",
				open: function () {
					grecaptcha.render('googleCaptcha', {
						'sitekey': captchaSiteKey
					});
				},
				close: function () {
					$(this).dialog('destroy').remove();
				},
				buttons: [
					{
						text: "Submit Your Review",
					    'class': "main-dialog-button",
					    click: function () {
						    var selector = "#reviewDialog form";
						    var jForm = $(selector);
						    reparseElementValidators(selector);
						    jForm.validate()
						    if (jForm.valid()) {
							    jForm.submit();
						    }
						}
					},
					{
						text: "Cancel",
						click: function () {
							$(this).dialog("close");
						}
					}
				]
			});
		}).error(function (result) {
			notifyError();
		});

		return false;
	});

	$('body').on("click", "a[data-video-id]", function () {
		var youtubeLink = $(this).attr("data-video-id");
		$("<div>" +
				"<iframe class='youtube-popup-container' frameborder='0' allowfullscreen='1' title='YouTube video player' " +
					"src='http://www.youtube.com/embed/" + youtubeLink + "?autoplay=1&iv_load_policy=3&rel=0&showinfo=0&wmode=opaque&enablejsapi=1&origin=http://staging.g2-dg.com/'>" +
				"</iframe>" +
			"</div>")
			.dialog({
				open: function () {
					var jCloseButton = $("<a id='btnVideoClose' class='youtube-popup-close' href='#'>" +
						"	<img src='/assets/images/close_button.png'/>" +
						"</a>");

					var jYoutube = $(".youtube-dialog");

					jCloseButton.css("top", jYoutube.css("top"));
					jCloseButton.css("left", jYoutube.offset().left + jYoutube.width());

					jYoutube.before(jCloseButton);
				},
				resizable: false,
				modal: true,
				dialogClass: "youtube-dialog"
			});
		return false;
	});
}, false);

function reviewSubmitSuccess() {
	if (successMessage) {
		notifySuccess(successMessage);
		$("#reviewDialog").dialog("close");
	} else {
		grecaptcha.render('googleCaptcha', {
			'sitekey': captchaSiteKey
		});
	}
}

function reviewSubmitError() {
	notifyError("Server error occured");
}

function onYouTubeIframeAPIReady() {
	ytplayer = document.getElementById(".youtube-popup-container");
	ytplayer.addEventListener("onStateChange", function (event) {
		if (event.data == 0) {
			$(".youtube-dialog").dialog("close");
		}
	});
}