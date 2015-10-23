function getCountries(successCallback, errorCallback) {
	$.ajax({
		url: "/Lookup/GetCountries",
		dataType: "json"
	}).success(function(result) {
		if (successCallback) {
			successCallback(result);
		}
	}).error(function(result) {
		if (errorCallback) {
			errorCallback(result);
		}
	});
}

function getCreditCardTypes(successCallback, errorCallback) {
	$.ajax({
		url: "/Lookup/GetCreditCardTypes",
		dataType: "json"
	}).success(function (result) {
		if (successCallback) {
			successCallback(result);
		}
	}).error(function (result) {
		if (errorCallback) {
			errorCallback(result);
		}
	});
}

function deleteCreditCard(idCreditCard,successCallback, errorCallback) {
	$.ajax({
		type: "POST",
		url: "/Profile/DeleteBillingInfo/" + idCreditCard,
		dataType: "json"
	}).success(function (result) {
		if (successCallback) {
			successCallback(result);
		}
	}).error(function (result) {
		if (errorCallback) {
			errorCallback(result);
		}
	});
}

function deleteShippingAddress(idAddress, successCallback, errorCallback) {
	$.ajax({
		type: "POST",
		url: "/Profile/DeleteShippingInfo/" + idAddress,
		dataType: "json"
	}).success(function (result) {
		if (successCallback) {
			successCallback(result);
		}
	}).error(function (result) {
		if (errorCallback) {
			errorCallback(result);
		}
	});
}