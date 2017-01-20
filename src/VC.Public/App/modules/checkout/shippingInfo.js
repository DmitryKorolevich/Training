var Shipments;

$(function ()
{
    Shipments = function (data)
    {
        var self = this;

        self.options = {};
        self.options.AddButtonText = ko.observable('');
        self.options.Countries = [];

        self.refreshing = ko.observable(true);
        self.loaded = ko.observable(false);

        var load = function ()
        {
            getCountries(undefined, function (resultCountries)
            {
                self.options.Countries = resultCountries.Data;
                $.ajax({
                    url: "/Checkout/GetShipments",
                    dataType: "json",
                    contentType: "application/json; charset=utf-8",
                    type: "GET"
                }).success(function (result)
                {
                    $("span[data-valmsg-for]").text('');
                    $("div.validation-summary-errors").html('');
                    if (result.Success)
                    {
                        self.Model = ko.mapping.fromJS(result.Data);
                        $.each(self.Model.Shipments(), function (i, item)
                        {
                            item.Collapsed = ko.observable(!item.FromOrder());
                            if (item.FromOrder())
                            {
                            }

                            setShipmentHandlers(item);
                        });
                        updateAddButtonText();

                        checkCanadaIssue();
                        self.loaded(true);
                    } else
                    {
                        processErrorResponse(result, self);
                        self.loaded(false);
                    }
                    self.refreshing(false);
                }).error(function (result)
                {
                    $("span[data-valmsg-for]").text('');
                    processErrorResponse();
                    self.refreshing(false);
                });
            }, function () { });
        };

        var openCollapsibleForClientErrors = function (validator)
        {
            $.each(validator.errorList, function (i, item)
            {
                var index = parseInt($(item.element).closest('.item').data('index'));
                self.Model.Shipments()[index].Collapsed(false);
            });
            if(validator.errorList.length>0)
            {
                $(validator.errorList[validator.errorList.length-1].element).focus();
            }
        };

        var setShipmentHandlers = function (shipment)
        {
            if (shipment.FromOrder() && self.Model.AvalibleAddresses().length > 0)
            {
                shipment.SelectedAvalibleAddress = ko.observable(self.Model.AvalibleAddresses()[0].Address);
            }
            else
            {
                shipment.SelectedAvalibleAddress = ko.observable(null);
            }
            shipment.SelectedAvalibleAddress.subscribe(function (newValue)
            {
                if (!newValue)
                {
                    return;
                }

                if (newValue.FromOrder())
                {
                    shipment.SaveToProfile(false);
                }

                shipment.IdCustomerShippingAddress(newValue.IdCustomerShippingAddress());
                shipment.DeliveryInstructions(newValue.DeliveryInstructions());
                shipment.PreferredShipMethod(newValue.PreferredShipMethod());
                shipment.PreferredShipMethodName(newValue.PreferredShipMethodName());
                shipment.AddressType(newValue.AddressType());

                shipment.FirstName(newValue.FirstName());
                shipment.LastName(newValue.LastName());
                shipment.Company(newValue.Company());
                shipment.IdCountry(newValue.IdCountry());
                shipment.IdState(newValue.IdState());
                shipment.Address1(newValue.Address1());
                shipment.Address2(newValue.Address2());
                shipment.City(newValue.City());
                shipment.County(newValue.County());
                shipment.PostalCode(newValue.PostalCode());
                shipment.Phone(newValue.Phone());
                shipment.Fax(newValue.Fax());
            });

            shipment.SelectedCountry = ko.observable(null);
            setSelectCountry(shipment);
            shipment.IdCountry.subscribe(function (newValue)
            {
                setSelectCountry(shipment);
            });
        };

        var setSelectCountry = function (shipment)
        {
            $.each(self.options.Countries, function (i, item)
            {
                if (item.Id == shipment.IdCountry())
                {
                    shipment.SelectedCountry(item);
                }
            });
        };

        self.remove = function (item, event)
        {
            if (confirm('Are you sure you want to delete this order?'))
            {
                self.Model.Shipments.remove(item);
                if (self.Model.Shipments().length == 1)
                {
                    self.Model.Shipments()[0].Collapsed(false);
                    updateAddButtonText();
                }
            }
        };

        self.add = function ()
        {
            reparseElementValidators("#mainForm");
            var validator = $("#mainForm").validate();
            validator.settings.ignore = "";

            if (!$("#mainForm").valid())
            {
                openCollapsibleForClientErrors(validator);
                return;
            }
            $.each(self.Model.Shipments(), function (i, innerItem)
            {
                innerItem.Collapsed(true);
            });

            var shipment = {};
            shipment.SaveToProfile = ko.observable(false);
            shipment.Collapsed = ko.observable(false);
            shipment.FromOrder = ko.observable(false);
            shipment.UseBillingAddress = ko.observable(false);
            shipment.IdCustomerShippingAddress = ko.observable(null);
            shipment.DeliveryInstructions = ko.observable(null);
            shipment.PreferredShipMethod = ko.observable(1);
            shipment.PreferredShipMethodName = ko.observable('Best');
            shipment.AddressType = ko.observable(1);
            shipment.IsGiftOrder = ko.observable(false);
            shipment.GiftMessage = ko.observable(null);

            shipment.FirstName = ko.observable('');
            shipment.LastName = ko.observable('');
            shipment.Company = ko.observable(null);
            shipment.IdCountry = ko.observable(appSettings.DefaultCountryId);
            shipment.IdState = ko.observable(0);
            shipment.Address1 = ko.observable('');
            shipment.Address2 = ko.observable(null);
            shipment.City = ko.observable(null);
            shipment.County = ko.observable(null);
            shipment.PostalCode = ko.observable(null);
            shipment.Phone = ko.observable(null);
            shipment.Fax = ko.observable(null);

            setShipmentHandlers(shipment);
            self.Model.Shipments.push(shipment);
            updateAddButtonText();
        };

        self.collapse = function (item, event)
        {
            item.Collapsed(true);
        };

        self.expand = function (item, event)
        {
            $.each(self.Model.Shipments(), function(i, innerItem){
                innerItem.Collapsed(true);
            });
            item.Collapsed(false);
        };

        self.save = function ()
        {
            reparseElementValidators("#mainForm");
            var validator = $("#mainForm").validate();
            validator.settings.ignore = "";

            if (!$("#mainForm").valid())
            {
                openCollapsibleForClientErrors(validator);
                return;
            }

            self.refreshing(true);
            ajaxRequest = $.ajax({
                url: "/Checkout/UpdateShipments",
                dataType: "json",
                data: ko.toJSON(self.Model),
                contentType: "application/json; charset=utf-8",
                type: "POST"
            }).success(function (result) {
                $("span[data-valmsg-for]").text('');
                $("div.validation-summary-errors").html('');
                if (result.Success) {
                    processJsonCommands(result);
                } else {
                    processErrorResponse(result, self);
                    self.refreshing(false);
                }
            }).error(function (result) {
                $("div.validation-summary-errors").html('');
                $("span[data-valmsg-for]").text('');
                processErrorResponse();
                self.refreshing(false);
            });
        };

        var updateAddButtonText = function(){
            if (self.Model.Shipments().length > 1)
            {
                self.options.AddButtonText('Add Another Recipient');
            }
            else
            {
                self.options.AddButtonText('Send Order To Multiple Receipents');
            }
        };

        function checkCanadaIssue()
        {
            var param = getQueryParameterByName("canadaissue");
            if (param)
            {
                self.refreshing(true);
                $.ajax({
                    url: "/Checkout/CanadaShippingIssueView",
                    dataType: "html",
                    type: "GET"
                }).success(function (result, textStatus, xhr)
                {
                    $(result).dialog({
                        resizable: false,
                        modal: true,
                        minWidth: 450,
                        dialogClass: "canada-shipping-notice",
                        open: function (event, ui)
                        {
                            $(this).parent().focus();
                        },
                        close: function ()
                        {
                            $(this).dialog('destroy').remove();
                        },
                        buttons: [
                            {
                                text: "Ok",
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
                }).complete(function ()
                {
                    self.refreshing(false);
                });
            }
        };

        load();
    }

    $('#mainForm').removeClass('hide');
    var viewModel = new Shipments();
    ko.applyBindings(viewModel);
});

function processErrorResponse(result, root)
{
    if (result)
    {
        if (result.Command != null)
        {
            processJsonCommands(result);
        }
        else
        {
            trySetFormErrors(result);
            if (root)
            {
                $.each(result.Messages, function (i, item)
                {
                    var element = $('#' + item.Field.replace('.', "\\."));
                    if (element.length == 1)
                    {
                        var index = parseInt(element.closest('.item').data('index'));
                        root.Model.Shipments()[index].Collapsed(false);
                        if (i == (result.Messages.length - 1))
                        {
                            element.focus();
                        }
                    }
                });
            }
        }
    }
    else
    {
        notifyError();
    }
};