﻿'use strict';

angular.module('app.modules.demo.controllers.productDetailController', [])
.controller('productDetailController', ['$scope', function ($scope) {
	$scope.description = '<table width="100%" border="0" cellspacing="0" cellpadding="0">   <tbody><tr>     <td valign="top"><br>Our Electronic Gift Certificates make great last-minute gifts! <br><br>Simply add the Gift Certificate of your choice to your cart, then enter the recipient\'s name and email address (not yours) into the appropriate "SHIP TO" fields during checkout. <br><br>The Gift Certificate will be delivered to your chosen recipient by email shortly after the completion of your order.<br><br><span style="font-weight: bold; ">IMPORTANT</span><br>There are no shipping or handling charges for e-Gift Certificates.<br>The e-Gift Certificate will be sent to the Name and Email address you provide in the "SHIP TO" section during checkout.<br><br>Do not select the "Use my billing address" auto-fill option unless you wish to receive the certificate yourself.<br><br>If your order contains no other items, you may simply enter "N/A" in the mandatory address fields.<br><br>If you\'re uncertain of the recipient\'s email address, you may send the e-Gift to yourself and forward when convenient.<br><br>Questions? Call us anytime at (800) 608-4825<br><br></td>        </tr> </tbody></table> ';
	$scope.descriptionExpanded = false;

	$scope.toogleEditorState = function(property) {
		$scope[property] = !$scope[property];
	};
}]);