window.addEventListener("load", function () {
	$("input[name=sku]:first").attr("checked", true);

	$("body").on("change", "input[name=sku]", function () {
		var jChecked = $("input[name=sku]:checked");

		$("#spSelectedPrice").text("Selected Price " + jChecked.attr("data-price"));
		$("#hSelectedCode").text("Product #" + jChecked.val());
	});
	$("body").on("click", "#writeReview", function() {
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
						    var jForm = $("#reviewDialog form");
						    reparseElementValidators(jForm);
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
	notifySuccess(successMessage);
	$("#reviewDialog").dialog("close");
}

function reviewSubmitError() {
	grecaptcha.reset();
}