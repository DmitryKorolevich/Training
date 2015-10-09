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