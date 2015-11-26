window.addEventListener("load", function () {
	$("input[name=sku]:first").attr("checked", true);

	$("body").on("click", "#lnkReviewsTab", function () {
		$(".tabs-control").tabs({ "active": 1 });
	});

	$("body").on("click", "#lnkDescriptionTab", function () {
		$(".tabs-control").tabs({ "active": 0 });
	});

	$("body").on("click", "#btnVideoClose", function () {
		$("#btnVideoClose").parent().dialog('destroy').remove();
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
				"<a id='btnVideoClose' class='youtube-popup-close' href='#'>" +
				"	<img src='/assets/images/close_button.png'/>" +
				"</a>" +
				"<iframe class='youtube-popup-container' frameborder='0' allowfullscreen='1' title='YouTube video player' " +
					"src='https://www.youtube.com/embed/" + youtubeLink + "?autoplay=1&amp;iv_load_policy=3&amp;rel=0&amp;showinfo=0&amp;wmode=opaque&amp;enablejsapi=1&amp;origin=http%3A%2F%2Fwww.vitalchoice.com'>" +
				"</iframe>" +
			"</div>")
			.dialog({
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