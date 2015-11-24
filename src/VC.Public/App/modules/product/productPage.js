window.addEventListener("load", function () {
	$("input[name=sku]:first").attr("checked", true);

	$("body").on("click", "#lnkReviewsTab", function () {
		$(".tabs-control").tabs({ "active": 1 });
	});

	$("body").on("click", "#lnkDescriptionTab", function () {
		$(".tabs-control").tabs({ "active": 0 });
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