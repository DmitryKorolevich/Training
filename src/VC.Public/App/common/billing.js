var creditCardTypes = null;

function populateCardTypes(callback) {
	getCreditCardTypes(function (result) {
		creditCardTypes = result.Data;

		$.each(result.Data, function (creditTypeIndex, creditType) {
			$("#ddCardType").append($('<option></option>').val(creditType.Key).html(creditType.Text));
		});

		var idCreditType = $("#hdCardType").val();
		if (idCreditType) {
			$("#ddCardType").val(idCreditType);
		}

		if (callback) {
			callback();
		}

	}, function (errorResult) {
		notifyError();
	});
}