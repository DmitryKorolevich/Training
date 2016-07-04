using System;
using System.Threading;
using System.Web.Services;
using System.Diagnostics;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using System.CodeDom.Compiler;
using System.ComponentModel;
using System.Web.Services.Description;
using VitalChoice.Ecommerce.Domain.Attributes;


namespace VitalChoice.Business.Services.VeraCore
{
    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [WebServiceBinding(Name = "OrderSoap", Namespace = "http://sma-promail/")]
    [XmlInclude(typeof(MarshalByRefObject))]
    [XmlInclude(typeof(PersistentObject))]
    public partial class VeraCoreOrderSoapClient : SoapHttpClientProtocol
    {
        private SendOrPostCallback AddProductOperationCompleted;

        private SendOrPostCallback SaveOfferOperationCompleted;

        private SendOrPostCallback AddPurchaseOrderOperationCompleted;

        private SendOrPostCallback SaveExpectedArrivalOperationCompleted;

        private SendOrPostCallback CancelExpectedArrivalOperationCompleted;

        private SendOrPostCallback CancelExpectedArrivalComponentOperationCompleted;

        private SendOrPostCallback GetOrderInfoOperationCompleted;

        private SendOrPostCallback GetOffersOperationCompleted;

        private SendOrPostCallback AddMailerOperationCompleted;

        private SendOrPostCallback AddPreRegisteredUserOperationCompleted;

        private SendOrPostCallback GetDetailedBillingOperationCompleted;

        private SendOrPostCallback GetProductShipmentAllProdsOperationCompleted;

        private SendOrPostCallback GetProductShipmentProdListOperationCompleted;

        private SendOrPostCallback GetShippingActivityOperationCompleted;

        private SendOrPostCallback GetProductReturnsOperationCompleted;

        private SendOrPostCallback GetShippingChargeOperationCompleted;

        private SendOrPostCallback AddOrderOperationCompleted;

        private SendOrPostCallback AddSizeColorClusterOperationCompleted;

        private SendOrPostCallback AddProductListClusterOperationCompleted;

        private SendOrPostCallback CreateProductFromJSONOperationCompleted;

        private SendOrPostCallback CreateOfferFromJSONOperationCompleted;

        private SendOrPostCallback CreateExpectedArrivalFromJSONOperationCompleted;

        private SendOrPostCallback CreatePurchaseOrderFromJSONOperationCompleted;

        private SendOrPostCallback CreateUserPreRegisteredFromJSONOperationCompleted;

        private SendOrPostCallback CreateSizeColorClusterFromJSONOperationCompleted;

        private SendOrPostCallback CreateProductListClusterFromJSONOperationCompleted;

        private SendOrPostCallback GetProductAvailabilitiesOperationCompleted;

        private SendOrPostCallback GetProductOperationCompleted;

        private SendOrPostCallback GetExpectedArrivalsOperationCompleted;

        /// <remarks/>
        public VeraCoreOrderSoapClient()
        {
            Url = "https://rhu003.veracore.com/pmomsws/order.asmx";
        }

        public AuthenticationHeader AuthenticationHeaderValue { get; set; }

        public DebugHeader DebugHeaderValue { get; set; }

        /// <remarks/>
        public event AddProductCompletedEventHandler AddProductCompleted;

        /// <remarks/>
        public event SaveOfferCompletedEventHandler SaveOfferCompleted;

        /// <remarks/>
        public event AddPurchaseOrderCompletedEventHandler AddPurchaseOrderCompleted;

        /// <remarks/>
        public event SaveExpectedArrivalCompletedEventHandler SaveExpectedArrivalCompleted;

        /// <remarks/>
        public event CancelExpectedArrivalCompletedEventHandler CancelExpectedArrivalCompleted;

        /// <remarks/>
        public event CancelExpectedArrivalComponentCompletedEventHandler CancelExpectedArrivalComponentCompleted;

        /// <remarks/>
        public event GetOrderInfoCompletedEventHandler GetOrderInfoCompleted;

        /// <remarks/>
        public event GetOffersCompletedEventHandler GetOffersCompleted;

        /// <remarks/>
        public event AddMailerCompletedEventHandler AddMailerCompleted;

        /// <remarks/>
        public event AddPreRegisteredUserCompletedEventHandler AddPreRegisteredUserCompleted;

        /// <remarks/>
        public event GetDetailedBillingCompletedEventHandler GetDetailedBillingCompleted;

        /// <remarks/>
        public event GetProductShipmentAllProdsCompletedEventHandler GetProductShipmentAllProdsCompleted;

        /// <remarks/>
        public event GetProductShipmentProdListCompletedEventHandler GetProductShipmentProdListCompleted;

        /// <remarks/>
        public event GetShippingActivityCompletedEventHandler GetShippingActivityCompleted;

        /// <remarks/>
        public event GetProductReturnsCompletedEventHandler GetProductReturnsCompleted;

        /// <remarks/>
        public event GetShippingChargeCompletedEventHandler GetShippingChargeCompleted;

        /// <remarks/>
        public event AddOrderCompletedEventHandler AddOrderCompleted;

        /// <remarks/>
        public event AddSizeColorClusterCompletedEventHandler AddSizeColorClusterCompleted;

        /// <remarks/>
        public event AddProductListClusterCompletedEventHandler AddProductListClusterCompleted;

        /// <remarks/>
        public event CreateProductFromJSONCompletedEventHandler CreateProductFromJSONCompleted;

        /// <remarks/>
        public event CreateOfferFromJSONCompletedEventHandler CreateOfferFromJSONCompleted;

        /// <remarks/>
        public event CreateExpectedArrivalFromJSONCompletedEventHandler CreateExpectedArrivalFromJSONCompleted;

        /// <remarks/>
        public event CreatePurchaseOrderFromJSONCompletedEventHandler CreatePurchaseOrderFromJSONCompleted;

        /// <remarks/>
        public event CreateUserPreRegisteredFromJSONCompletedEventHandler CreateUserPreRegisteredFromJSONCompleted;

        /// <remarks/>
        public event CreateSizeColorClusterFromJSONCompletedEventHandler CreateSizeColorClusterFromJSONCompleted;

        /// <remarks/>
        public event CreateProductListClusterFromJSONCompletedEventHandler CreateProductListClusterFromJSONCompleted;

        /// <remarks/>
        public event GetProductAvailabilitiesCompletedEventHandler GetProductAvailabilitiesCompleted;

        /// <remarks/>
        public event GetProductCompletedEventHandler GetProductCompleted;

        /// <remarks/>
        public event GetExpectedArrivalsCompletedEventHandler GetExpectedArrivalsCompleted;

        /// <remarks/>
        [SoapHeader("AuthenticationHeaderValue")]
        [SoapHeader("DebugHeaderValue", Direction = SoapHeaderDirection.InOut)]
        [SoapDocumentMethod("http://sma-promail/AddProduct", RequestNamespace = "http://sma-promail/",
            ResponseNamespace = "http://sma-promail/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public AddProductResult AddProduct(Product product, Offer offer)
        {
            object[] results = Invoke("AddProduct", new object[]
            {
                product,
                offer
            });
            return ((AddProductResult) (results[0]));
        }

        /// <remarks/>
        public IAsyncResult BeginAddProduct(Product product, Offer offer, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("AddProduct", new object[]
            {
                product,
                offer
            }, callback, asyncState);
        }

        /// <remarks/>
        public AddProductResult EndAddProduct(IAsyncResult asyncResult)
        {
            object[] results = EndInvoke(asyncResult);
            return ((AddProductResult) (results[0]));
        }

        /// <remarks/>
        public void AddProductAsync(Product product, Offer offer)
        {
            AddProductAsync(product, offer, null);
        }

        /// <remarks/>
        public void AddProductAsync(Product product, Offer offer, object userState)
        {
            if ((AddProductOperationCompleted == null))
            {
                AddProductOperationCompleted = new SendOrPostCallback(OnAddProductOperationCompleted);
            }
            InvokeAsync("AddProduct", new object[]
            {
                product,
                offer
            }, AddProductOperationCompleted, userState);
        }

        private void OnAddProductOperationCompleted(object arg)
        {
            if ((AddProductCompleted != null))
            {
                InvokeCompletedEventArgs invokeArgs = ((InvokeCompletedEventArgs) (arg));
                AddProductCompleted(this,
                    new AddProductCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [SoapHeader("AuthenticationHeaderValue")]
        [SoapHeader("DebugHeaderValue", Direction = SoapHeaderDirection.InOut)]
        [SoapDocumentMethod("http://sma-promail/SaveOffer", RequestNamespace = "http://sma-promail/",
            ResponseNamespace = "http://sma-promail/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public SaveOfferResult SaveOffer(Offer offer)
        {
            object[] results = Invoke("SaveOffer", new object[]
            {
                offer
            });
            return ((SaveOfferResult) (results[0]));
        }

        /// <remarks/>
        public IAsyncResult BeginSaveOffer(Offer offer, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("SaveOffer", new object[]
            {
                offer
            }, callback, asyncState);
        }

        /// <remarks/>
        public SaveOfferResult EndSaveOffer(IAsyncResult asyncResult)
        {
            object[] results = EndInvoke(asyncResult);
            return ((SaveOfferResult) (results[0]));
        }

        /// <remarks/>
        public void SaveOfferAsync(Offer offer)
        {
            SaveOfferAsync(offer, null);
        }

        /// <remarks/>
        public void SaveOfferAsync(Offer offer, object userState)
        {
            if ((SaveOfferOperationCompleted == null))
            {
                SaveOfferOperationCompleted = new SendOrPostCallback(OnSaveOfferOperationCompleted);
            }
            InvokeAsync("SaveOffer", new object[]
            {
                offer
            }, SaveOfferOperationCompleted, userState);
        }

        private void OnSaveOfferOperationCompleted(object arg)
        {
            if ((SaveOfferCompleted != null))
            {
                InvokeCompletedEventArgs invokeArgs = ((InvokeCompletedEventArgs) (arg));
                SaveOfferCompleted(this,
                    new SaveOfferCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [SoapHeader("AuthenticationHeaderValue")]
        [SoapHeader("DebugHeaderValue", Direction = SoapHeaderDirection.InOut)]
        [SoapDocumentMethod("http://sma-promail/AddPurchaseOrder", RequestNamespace = "http://sma-promail/",
            ResponseNamespace = "http://sma-promail/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public AddPurchaseOrderResult AddPurchaseOrder(PurchaseOrder purchaseOrder)
        {
            object[] results = Invoke("AddPurchaseOrder", new object[]
            {
                purchaseOrder
            });
            return ((AddPurchaseOrderResult) (results[0]));
        }

        /// <remarks/>
        public IAsyncResult BeginAddPurchaseOrder(PurchaseOrder purchaseOrder, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("AddPurchaseOrder", new object[]
            {
                purchaseOrder
            }, callback, asyncState);
        }

        /// <remarks/>
        public AddPurchaseOrderResult EndAddPurchaseOrder(IAsyncResult asyncResult)
        {
            object[] results = EndInvoke(asyncResult);
            return ((AddPurchaseOrderResult) (results[0]));
        }

        /// <remarks/>
        public void AddPurchaseOrderAsync(PurchaseOrder purchaseOrder)
        {
            AddPurchaseOrderAsync(purchaseOrder, null);
        }

        /// <remarks/>
        public void AddPurchaseOrderAsync(PurchaseOrder purchaseOrder, object userState)
        {
            if ((AddPurchaseOrderOperationCompleted == null))
            {
                AddPurchaseOrderOperationCompleted = new SendOrPostCallback(OnAddPurchaseOrderOperationCompleted);
            }
            InvokeAsync("AddPurchaseOrder", new object[]
            {
                purchaseOrder
            }, AddPurchaseOrderOperationCompleted, userState);
        }

        private void OnAddPurchaseOrderOperationCompleted(object arg)
        {
            if ((AddPurchaseOrderCompleted != null))
            {
                InvokeCompletedEventArgs invokeArgs = ((InvokeCompletedEventArgs) (arg));
                AddPurchaseOrderCompleted(this,
                    new AddPurchaseOrderCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [SoapHeader("AuthenticationHeaderValue")]
        [SoapHeader("DebugHeaderValue", Direction = SoapHeaderDirection.InOut)]
        [SoapDocumentMethod("http://sma-promail/SaveExpectedArrival", RequestNamespace = "http://sma-promail/",
            ResponseNamespace = "http://sma-promail/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public SaveExpectedArrivalResult SaveExpectedArrival(ExpectedArrival expectedArrival)
        {
            object[] results = Invoke("SaveExpectedArrival", new object[]
            {
                expectedArrival
            });
            return ((SaveExpectedArrivalResult) (results[0]));
        }

        /// <remarks/>
        public IAsyncResult BeginSaveExpectedArrival(ExpectedArrival expectedArrival, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("SaveExpectedArrival", new object[]
            {
                expectedArrival
            }, callback, asyncState);
        }

        /// <remarks/>
        public SaveExpectedArrivalResult EndSaveExpectedArrival(IAsyncResult asyncResult)
        {
            object[] results = EndInvoke(asyncResult);
            return ((SaveExpectedArrivalResult) (results[0]));
        }

        /// <remarks/>
        public void SaveExpectedArrivalAsync(ExpectedArrival expectedArrival)
        {
            SaveExpectedArrivalAsync(expectedArrival, null);
        }

        /// <remarks/>
        public void SaveExpectedArrivalAsync(ExpectedArrival expectedArrival, object userState)
        {
            if ((SaveExpectedArrivalOperationCompleted == null))
            {
                SaveExpectedArrivalOperationCompleted = new SendOrPostCallback(OnSaveExpectedArrivalOperationCompleted);
            }
            InvokeAsync("SaveExpectedArrival", new object[]
            {
                expectedArrival
            }, SaveExpectedArrivalOperationCompleted, userState);
        }

        private void OnSaveExpectedArrivalOperationCompleted(object arg)
        {
            if ((SaveExpectedArrivalCompleted != null))
            {
                InvokeCompletedEventArgs invokeArgs = ((InvokeCompletedEventArgs) (arg));
                SaveExpectedArrivalCompleted(this,
                    new SaveExpectedArrivalCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled,
                        invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [SoapHeader("AuthenticationHeaderValue")]
        [SoapHeader("DebugHeaderValue", Direction = SoapHeaderDirection.InOut)]
        [SoapDocumentMethod("http://sma-promail/CancelExpectedArrival", RequestNamespace = "http://sma-promail/",
            ResponseNamespace = "http://sma-promail/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public CancelExpectedArrivalResult CancelExpectedArrival(int expectedArrivalSeqID)
        {
            object[] results = Invoke("CancelExpectedArrival", new object[]
            {
                expectedArrivalSeqID
            });
            return ((CancelExpectedArrivalResult) (results[0]));
        }

        /// <remarks/>
        public IAsyncResult BeginCancelExpectedArrival(int expectedArrivalSeqID, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("CancelExpectedArrival", new object[]
            {
                expectedArrivalSeqID
            }, callback, asyncState);
        }

        /// <remarks/>
        public CancelExpectedArrivalResult EndCancelExpectedArrival(IAsyncResult asyncResult)
        {
            object[] results = EndInvoke(asyncResult);
            return ((CancelExpectedArrivalResult) (results[0]));
        }

        /// <remarks/>
        public void CancelExpectedArrivalAsync(int expectedArrivalSeqID)
        {
            CancelExpectedArrivalAsync(expectedArrivalSeqID, null);
        }

        /// <remarks/>
        public void CancelExpectedArrivalAsync(int expectedArrivalSeqID, object userState)
        {
            if ((CancelExpectedArrivalOperationCompleted == null))
            {
                CancelExpectedArrivalOperationCompleted = new SendOrPostCallback(OnCancelExpectedArrivalOperationCompleted);
            }
            InvokeAsync("CancelExpectedArrival", new object[]
            {
                expectedArrivalSeqID
            }, CancelExpectedArrivalOperationCompleted, userState);
        }

        private void OnCancelExpectedArrivalOperationCompleted(object arg)
        {
            if ((CancelExpectedArrivalCompleted != null))
            {
                InvokeCompletedEventArgs invokeArgs = ((InvokeCompletedEventArgs) (arg));
                CancelExpectedArrivalCompleted(this,
                    new CancelExpectedArrivalCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled,
                        invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [SoapHeader("AuthenticationHeaderValue")]
        [SoapHeader("DebugHeaderValue", Direction = SoapHeaderDirection.InOut)]
        [SoapDocumentMethod("http://sma-promail/CancelExpectedArrivalComponent", RequestNamespace = "http://sma-promail/",
            ResponseNamespace = "http://sma-promail/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public CancelExpectedArrivalResult CancelExpectedArrivalComponent(int expectedArrivalComponent)
        {
            object[] results = Invoke("CancelExpectedArrivalComponent", new object[]
            {
                expectedArrivalComponent
            });
            return ((CancelExpectedArrivalResult) (results[0]));
        }

        /// <remarks/>
        public IAsyncResult BeginCancelExpectedArrivalComponent(int expectedArrivalComponent, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("CancelExpectedArrivalComponent", new object[]
            {
                expectedArrivalComponent
            }, callback, asyncState);
        }

        /// <remarks/>
        public CancelExpectedArrivalResult EndCancelExpectedArrivalComponent(IAsyncResult asyncResult)
        {
            object[] results = EndInvoke(asyncResult);
            return ((CancelExpectedArrivalResult) (results[0]));
        }

        /// <remarks/>
        public void CancelExpectedArrivalComponentAsync(int expectedArrivalComponent)
        {
            CancelExpectedArrivalComponentAsync(expectedArrivalComponent, null);
        }

        /// <remarks/>
        public void CancelExpectedArrivalComponentAsync(int expectedArrivalComponent, object userState)
        {
            if ((CancelExpectedArrivalComponentOperationCompleted == null))
            {
                CancelExpectedArrivalComponentOperationCompleted = new SendOrPostCallback(OnCancelExpectedArrivalComponentOperationCompleted);
            }
            InvokeAsync("CancelExpectedArrivalComponent", new object[]
            {
                expectedArrivalComponent
            }, CancelExpectedArrivalComponentOperationCompleted, userState);
        }

        private void OnCancelExpectedArrivalComponentOperationCompleted(object arg)
        {
            if ((CancelExpectedArrivalComponentCompleted != null))
            {
                InvokeCompletedEventArgs invokeArgs = ((InvokeCompletedEventArgs) (arg));
                CancelExpectedArrivalComponentCompleted(this,
                    new CancelExpectedArrivalComponentCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled,
                        invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [SoapHeader("AuthenticationHeaderValue")]
        [SoapHeader("DebugHeaderValue", Direction = SoapHeaderDirection.InOut)]
        [SoapDocumentMethod("http://sma-promail/GetOrderInfo", RequestNamespace = "http://sma-promail/",
            ResponseNamespace = "http://sma-promail/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public OrderInqRecord GetOrderInfo(string orderId)
        {
            object[] results = Invoke("GetOrderInfo", new object[]
            {
                orderId
            });
            return ((OrderInqRecord) (results[0]));
        }

        /// <remarks/>
        public IAsyncResult BeginGetOrderInfo(string orderId, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("GetOrderInfo", new object[]
            {
                orderId
            }, callback, asyncState);
        }

        /// <remarks/>
        public OrderInqRecord EndGetOrderInfo(IAsyncResult asyncResult)
        {
            object[] results = EndInvoke(asyncResult);
            return ((OrderInqRecord) (results[0]));
        }

        /// <remarks/>
        public void GetOrderInfoAsync(string orderId)
        {
            GetOrderInfoAsync(orderId, null);
        }

        /// <remarks/>
        public void GetOrderInfoAsync(string orderId, object userState)
        {
            if ((GetOrderInfoOperationCompleted == null))
            {
                GetOrderInfoOperationCompleted = new SendOrPostCallback(OnGetOrderInfoOperationCompleted);
            }
            InvokeAsync("GetOrderInfo", new object[]
            {
                orderId
            }, GetOrderInfoOperationCompleted, userState);
        }

        private void OnGetOrderInfoOperationCompleted(object arg)
        {
            if ((GetOrderInfoCompleted != null))
            {
                InvokeCompletedEventArgs invokeArgs = ((InvokeCompletedEventArgs) (arg));
                GetOrderInfoCompleted(this,
                    new GetOrderInfoCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [SoapHeader("AuthenticationHeaderValue")]
        [SoapHeader("DebugHeaderValue", Direction = SoapHeaderDirection.InOut)]
        [SoapDocumentMethod("http://sma-promail/GetOffers", RequestNamespace = "http://sma-promail/",
            ResponseNamespace = "http://sma-promail/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public GetOfferResult[] GetOffers(OfferSortGroup[] sortGroups, string categoryGroupDescription, CustomCategory[] customCategories,
            string mailerUID, string searchString, bool searchID, bool searchDescription, string priceClassDescription)
        {
            object[] results = Invoke("GetOffers", new object[]
            {
                sortGroups,
                categoryGroupDescription,
                customCategories,
                mailerUID,
                searchString,
                searchID,
                searchDescription,
                priceClassDescription
            });
            return ((GetOfferResult[]) (results[0]));
        }

        /// <remarks/>
        public IAsyncResult BeginGetOffers(OfferSortGroup[] sortGroups, string categoryGroupDescription, CustomCategory[] customCategories,
            string mailerUID, string searchString, bool searchID, bool searchDescription, string priceClassDescription,
            AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("GetOffers", new object[]
            {
                sortGroups,
                categoryGroupDescription,
                customCategories,
                mailerUID,
                searchString,
                searchID,
                searchDescription,
                priceClassDescription
            }, callback, asyncState);
        }

        /// <remarks/>
        public GetOfferResult[] EndGetOffers(IAsyncResult asyncResult)
        {
            object[] results = EndInvoke(asyncResult);
            return ((GetOfferResult[]) (results[0]));
        }

        /// <remarks/>
        public void GetOffersAsync(OfferSortGroup[] sortGroups, string categoryGroupDescription, CustomCategory[] customCategories,
            string mailerUID, string searchString, bool searchID, bool searchDescription, string priceClassDescription)
        {
            GetOffersAsync(sortGroups, categoryGroupDescription, customCategories, mailerUID, searchString, searchID, searchDescription,
                priceClassDescription, null);
        }

        /// <remarks/>
        public void GetOffersAsync(OfferSortGroup[] sortGroups, string categoryGroupDescription, CustomCategory[] customCategories,
            string mailerUID, string searchString, bool searchID, bool searchDescription, string priceClassDescription, object userState)
        {
            if ((GetOffersOperationCompleted == null))
            {
                GetOffersOperationCompleted = new SendOrPostCallback(OnGetOffersOperationCompleted);
            }
            InvokeAsync("GetOffers", new object[]
            {
                sortGroups,
                categoryGroupDescription,
                customCategories,
                mailerUID,
                searchString,
                searchID,
                searchDescription,
                priceClassDescription
            }, GetOffersOperationCompleted, userState);
        }

        private void OnGetOffersOperationCompleted(object arg)
        {
            if ((GetOffersCompleted != null))
            {
                InvokeCompletedEventArgs invokeArgs = ((InvokeCompletedEventArgs) (arg));
                GetOffersCompleted(this,
                    new GetOffersCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [SoapHeader("AuthenticationHeaderValue")]
        [SoapHeader("DebugHeaderValue", Direction = SoapHeaderDirection.InOut)]
        [SoapDocumentMethod("http://sma-promail/AddMailer", RequestNamespace = "http://sma-promail/",
            ResponseNamespace = "http://sma-promail/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public AddMailerResult AddMailer(Person person)
        {
            object[] results = Invoke("AddMailer", new object[]
            {
                person
            });
            return ((AddMailerResult) (results[0]));
        }

        /// <remarks/>
        public IAsyncResult BeginAddMailer(Person person, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("AddMailer", new object[]
            {
                person
            }, callback, asyncState);
        }

        /// <remarks/>
        public AddMailerResult EndAddMailer(IAsyncResult asyncResult)
        {
            object[] results = EndInvoke(asyncResult);
            return ((AddMailerResult) (results[0]));
        }

        /// <remarks/>
        public void AddMailerAsync(Person person)
        {
            AddMailerAsync(person, null);
        }

        /// <remarks/>
        public void AddMailerAsync(Person person, object userState)
        {
            if ((AddMailerOperationCompleted == null))
            {
                AddMailerOperationCompleted = new SendOrPostCallback(OnAddMailerOperationCompleted);
            }
            InvokeAsync("AddMailer", new object[]
            {
                person
            }, AddMailerOperationCompleted, userState);
        }

        private void OnAddMailerOperationCompleted(object arg)
        {
            if ((AddMailerCompleted != null))
            {
                InvokeCompletedEventArgs invokeArgs = ((InvokeCompletedEventArgs) (arg));
                AddMailerCompleted(this,
                    new AddMailerCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [SoapHeader("AuthenticationHeaderValue")]
        [SoapHeader("DebugHeaderValue", Direction = SoapHeaderDirection.InOut)]
        [SoapDocumentMethod("http://sma-promail/AddPreRegisteredUser", RequestNamespace = "http://sma-promail/",
            ResponseNamespace = "http://sma-promail/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public AddPreRegUserResult AddPreRegisteredUser(UserPreRegistered user)
        {
            object[] results = Invoke("AddPreRegisteredUser", new object[]
            {
                user
            });
            return ((AddPreRegUserResult) (results[0]));
        }

        /// <remarks/>
        public IAsyncResult BeginAddPreRegisteredUser(UserPreRegistered user, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("AddPreRegisteredUser", new object[]
            {
                user
            }, callback, asyncState);
        }

        /// <remarks/>
        public AddPreRegUserResult EndAddPreRegisteredUser(IAsyncResult asyncResult)
        {
            object[] results = EndInvoke(asyncResult);
            return ((AddPreRegUserResult) (results[0]));
        }

        /// <remarks/>
        public void AddPreRegisteredUserAsync(UserPreRegistered user)
        {
            AddPreRegisteredUserAsync(user, null);
        }

        /// <remarks/>
        public void AddPreRegisteredUserAsync(UserPreRegistered user, object userState)
        {
            if ((AddPreRegisteredUserOperationCompleted == null))
            {
                AddPreRegisteredUserOperationCompleted = new SendOrPostCallback(OnAddPreRegisteredUserOperationCompleted);
            }
            InvokeAsync("AddPreRegisteredUser", new object[]
            {
                user
            }, AddPreRegisteredUserOperationCompleted, userState);
        }

        private void OnAddPreRegisteredUserOperationCompleted(object arg)
        {
            if ((AddPreRegisteredUserCompleted != null))
            {
                InvokeCompletedEventArgs invokeArgs = ((InvokeCompletedEventArgs) (arg));
                AddPreRegisteredUserCompleted(this,
                    new AddPreRegisteredUserCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled,
                        invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [SoapHeader("AuthenticationHeaderValue")]
        [SoapHeader("DebugHeaderValue", Direction = SoapHeaderDirection.InOut)]
        [SoapDocumentMethod("http://sma-promail/GetDetailedBilling", RequestNamespace = "http://sma-promail/",
            ResponseNamespace = "http://sma-promail/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public BillingDetail[] GetDetailedBilling(DateTime StartDate, DateTime EndDate)
        {
            object[] results = Invoke("GetDetailedBilling", new object[]
            {
                StartDate,
                EndDate
            });
            return ((BillingDetail[]) (results[0]));
        }

        /// <remarks/>
        public IAsyncResult BeginGetDetailedBilling(DateTime StartDate, DateTime EndDate, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("GetDetailedBilling", new object[]
            {
                StartDate,
                EndDate
            }, callback, asyncState);
        }

        /// <remarks/>
        public BillingDetail[] EndGetDetailedBilling(IAsyncResult asyncResult)
        {
            object[] results = EndInvoke(asyncResult);
            return ((BillingDetail[]) (results[0]));
        }

        /// <remarks/>
        public void GetDetailedBillingAsync(DateTime StartDate, DateTime EndDate)
        {
            GetDetailedBillingAsync(StartDate, EndDate, null);
        }

        /// <remarks/>
        public void GetDetailedBillingAsync(DateTime StartDate, DateTime EndDate, object userState)
        {
            if ((GetDetailedBillingOperationCompleted == null))
            {
                GetDetailedBillingOperationCompleted = new SendOrPostCallback(OnGetDetailedBillingOperationCompleted);
            }
            InvokeAsync("GetDetailedBilling", new object[]
            {
                StartDate,
                EndDate
            }, GetDetailedBillingOperationCompleted, userState);
        }

        private void OnGetDetailedBillingOperationCompleted(object arg)
        {
            if ((GetDetailedBillingCompleted != null))
            {
                InvokeCompletedEventArgs invokeArgs = ((InvokeCompletedEventArgs) (arg));
                GetDetailedBillingCompleted(this,
                    new GetDetailedBillingCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled,
                        invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [SoapHeader("AuthenticationHeaderValue")]
        [SoapHeader("DebugHeaderValue", Direction = SoapHeaderDirection.InOut)]
        [SoapDocumentMethod("http://sma-promail/GetProductShipmentAllProds", RequestNamespace = "http://sma-promail/",
            ResponseNamespace = "http://sma-promail/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public ProductShipment[] GetProductShipmentAllProds(DateTime StartDate, DateTime EndDate)
        {
            object[] results = Invoke("GetProductShipmentAllProds", new object[]
            {
                StartDate,
                EndDate
            });
            return ((ProductShipment[]) (results[0]));
        }

        /// <remarks/>
        public IAsyncResult BeginGetProductShipmentAllProds(DateTime StartDate, DateTime EndDate, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("GetProductShipmentAllProds", new object[]
            {
                StartDate,
                EndDate
            }, callback, asyncState);
        }

        /// <remarks/>
        public ProductShipment[] EndGetProductShipmentAllProds(IAsyncResult asyncResult)
        {
            object[] results = EndInvoke(asyncResult);
            return ((ProductShipment[]) (results[0]));
        }

        /// <remarks/>
        public void GetProductShipmentAllProdsAsync(DateTime StartDate, DateTime EndDate)
        {
            GetProductShipmentAllProdsAsync(StartDate, EndDate, null);
        }

        /// <remarks/>
        public void GetProductShipmentAllProdsAsync(DateTime StartDate, DateTime EndDate, object userState)
        {
            if ((GetProductShipmentAllProdsOperationCompleted == null))
            {
                GetProductShipmentAllProdsOperationCompleted = new SendOrPostCallback(OnGetProductShipmentAllProdsOperationCompleted);
            }
            InvokeAsync("GetProductShipmentAllProds", new object[]
            {
                StartDate,
                EndDate
            }, GetProductShipmentAllProdsOperationCompleted, userState);
        }

        private void OnGetProductShipmentAllProdsOperationCompleted(object arg)
        {
            if ((GetProductShipmentAllProdsCompleted != null))
            {
                InvokeCompletedEventArgs invokeArgs = ((InvokeCompletedEventArgs) (arg));
                GetProductShipmentAllProdsCompleted(this,
                    new GetProductShipmentAllProdsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled,
                        invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [SoapHeader("AuthenticationHeaderValue")]
        [SoapHeader("DebugHeaderValue", Direction = SoapHeaderDirection.InOut)]
        [SoapDocumentMethod("http://sma-promail/GetProductShipmentProdList", RequestNamespace = "http://sma-promail/",
            ResponseNamespace = "http://sma-promail/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public ProductShipment[] GetProductShipmentProdList(DateTime StartDate, DateTime EndDate, string[] ProductIds, string owner)
        {
            object[] results = Invoke("GetProductShipmentProdList", new object[]
            {
                StartDate,
                EndDate,
                ProductIds,
                owner
            });
            return ((ProductShipment[]) (results[0]));
        }

        /// <remarks/>
        public IAsyncResult BeginGetProductShipmentProdList(DateTime StartDate, DateTime EndDate, string[] ProductIds, string owner,
            AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("GetProductShipmentProdList", new object[]
            {
                StartDate,
                EndDate,
                ProductIds,
                owner
            }, callback, asyncState);
        }

        /// <remarks/>
        public ProductShipment[] EndGetProductShipmentProdList(IAsyncResult asyncResult)
        {
            object[] results = EndInvoke(asyncResult);
            return ((ProductShipment[]) (results[0]));
        }

        /// <remarks/>
        public void GetProductShipmentProdListAsync(DateTime StartDate, DateTime EndDate, string[] ProductIds, string owner)
        {
            GetProductShipmentProdListAsync(StartDate, EndDate, ProductIds, owner, null);
        }

        /// <remarks/>
        public void GetProductShipmentProdListAsync(DateTime StartDate, DateTime EndDate, string[] ProductIds, string owner,
            object userState)
        {
            if ((GetProductShipmentProdListOperationCompleted == null))
            {
                GetProductShipmentProdListOperationCompleted = new SendOrPostCallback(OnGetProductShipmentProdListOperationCompleted);
            }
            InvokeAsync("GetProductShipmentProdList", new object[]
            {
                StartDate,
                EndDate,
                ProductIds,
                owner
            }, GetProductShipmentProdListOperationCompleted, userState);
        }

        private void OnGetProductShipmentProdListOperationCompleted(object arg)
        {
            if ((GetProductShipmentProdListCompleted != null))
            {
                InvokeCompletedEventArgs invokeArgs = ((InvokeCompletedEventArgs) (arg));
                GetProductShipmentProdListCompleted(this,
                    new GetProductShipmentProdListCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled,
                        invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [SoapHeader("AuthenticationHeaderValue")]
        [SoapHeader("DebugHeaderValue", Direction = SoapHeaderDirection.InOut)]
        [SoapDocumentMethod("http://sma-promail/GetShippingActivity", RequestNamespace = "http://sma-promail/",
            ResponseNamespace = "http://sma-promail/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public ShippingActivity[] GetShippingActivity(DateTime StartDate, DateTime EndDate)
        {
            object[] results = Invoke("GetShippingActivity", new object[]
            {
                StartDate,
                EndDate
            });
            return ((ShippingActivity[]) (results[0]));
        }

        /// <remarks/>
        public IAsyncResult BeginGetShippingActivity(DateTime StartDate, DateTime EndDate, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("GetShippingActivity", new object[]
            {
                StartDate,
                EndDate
            }, callback, asyncState);
        }

        /// <remarks/>
        public ShippingActivity[] EndGetShippingActivity(IAsyncResult asyncResult)
        {
            object[] results = EndInvoke(asyncResult);
            return ((ShippingActivity[]) (results[0]));
        }

        /// <remarks/>
        public void GetShippingActivityAsync(DateTime StartDate, DateTime EndDate)
        {
            GetShippingActivityAsync(StartDate, EndDate, null);
        }

        /// <remarks/>
        public void GetShippingActivityAsync(DateTime StartDate, DateTime EndDate, object userState)
        {
            if ((GetShippingActivityOperationCompleted == null))
            {
                GetShippingActivityOperationCompleted = new SendOrPostCallback(OnGetShippingActivityOperationCompleted);
            }
            InvokeAsync("GetShippingActivity", new object[]
            {
                StartDate,
                EndDate
            }, GetShippingActivityOperationCompleted, userState);
        }

        private void OnGetShippingActivityOperationCompleted(object arg)
        {
            if ((GetShippingActivityCompleted != null))
            {
                InvokeCompletedEventArgs invokeArgs = ((InvokeCompletedEventArgs) (arg));
                GetShippingActivityCompleted(this,
                    new GetShippingActivityCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled,
                        invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [SoapHeader("AuthenticationHeaderValue")]
        [SoapHeader("DebugHeaderValue", Direction = SoapHeaderDirection.InOut)]
        [SoapDocumentMethod("http://sma-promail/GetProductReturns", RequestNamespace = "http://sma-promail/",
            ResponseNamespace = "http://sma-promail/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public ProductReturns[] GetProductReturns(DateTime StartDate, DateTime EndDate)
        {
            object[] results = Invoke("GetProductReturns", new object[]
            {
                StartDate,
                EndDate
            });
            return ((ProductReturns[]) (results[0]));
        }

        /// <remarks/>
        public IAsyncResult BeginGetProductReturns(DateTime StartDate, DateTime EndDate, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("GetProductReturns", new object[]
            {
                StartDate,
                EndDate
            }, callback, asyncState);
        }

        /// <remarks/>
        public ProductReturns[] EndGetProductReturns(IAsyncResult asyncResult)
        {
            object[] results = EndInvoke(asyncResult);
            return ((ProductReturns[]) (results[0]));
        }

        /// <remarks/>
        public void GetProductReturnsAsync(DateTime StartDate, DateTime EndDate)
        {
            GetProductReturnsAsync(StartDate, EndDate, null);
        }

        /// <remarks/>
        public void GetProductReturnsAsync(DateTime StartDate, DateTime EndDate, object userState)
        {
            if ((GetProductReturnsOperationCompleted == null))
            {
                GetProductReturnsOperationCompleted = new SendOrPostCallback(OnGetProductReturnsOperationCompleted);
            }
            InvokeAsync("GetProductReturns", new object[]
            {
                StartDate,
                EndDate
            }, GetProductReturnsOperationCompleted, userState);
        }

        private void OnGetProductReturnsOperationCompleted(object arg)
        {
            if ((GetProductReturnsCompleted != null))
            {
                InvokeCompletedEventArgs invokeArgs = ((InvokeCompletedEventArgs) (arg));
                GetProductReturnsCompleted(this,
                    new GetProductReturnsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [SoapHeader("AuthenticationHeaderValue")]
        [SoapHeader("DebugHeaderValue", Direction = SoapHeaderDirection.InOut)]
        [SoapDocumentMethod("http://sma-promail/GetShippingCharge", RequestNamespace = "http://sma-promail/",
            ResponseNamespace = "http://sma-promail/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public ShippingCharges[] GetShippingCharge(DateTime StartDate, DateTime EndDate)
        {
            object[] results = Invoke("GetShippingCharge", new object[]
            {
                StartDate,
                EndDate
            });
            return ((ShippingCharges[]) (results[0]));
        }

        /// <remarks/>
        public IAsyncResult BeginGetShippingCharge(DateTime StartDate, DateTime EndDate, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("GetShippingCharge", new object[]
            {
                StartDate,
                EndDate
            }, callback, asyncState);
        }

        /// <remarks/>
        public ShippingCharges[] EndGetShippingCharge(IAsyncResult asyncResult)
        {
            object[] results = EndInvoke(asyncResult);
            return ((ShippingCharges[]) (results[0]));
        }

        /// <remarks/>
        public void GetShippingChargeAsync(DateTime StartDate, DateTime EndDate)
        {
            GetShippingChargeAsync(StartDate, EndDate, null);
        }

        /// <remarks/>
        public void GetShippingChargeAsync(DateTime StartDate, DateTime EndDate, object userState)
        {
            if ((GetShippingChargeOperationCompleted == null))
            {
                GetShippingChargeOperationCompleted = new SendOrPostCallback(OnGetShippingChargeOperationCompleted);
            }
            InvokeAsync("GetShippingCharge", new object[]
            {
                StartDate,
                EndDate
            }, GetShippingChargeOperationCompleted, userState);
        }

        private void OnGetShippingChargeOperationCompleted(object arg)
        {
            if ((GetShippingChargeCompleted != null))
            {
                InvokeCompletedEventArgs invokeArgs = ((InvokeCompletedEventArgs) (arg));
                GetShippingChargeCompleted(this,
                    new GetShippingChargeCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [SoapHeader("AuthenticationHeaderValue")]
        [SoapHeader("DebugHeaderValue", Direction = SoapHeaderDirection.InOut)]
        [SoapDocumentMethod("http://sma-promail/AddOrder", RequestNamespace = "http://sma-promail/",
            ResponseNamespace = "http://sma-promail/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public AddOrderResult AddOrder(VeraCoreExportOrder order)
        {
            object[] results = Invoke("AddOrder", new object[]
            {
                order
            });
            return ((AddOrderResult) (results[0]));
        }

        /// <remarks/>
        public IAsyncResult BeginAddOrder(VeraCoreExportOrder order, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("AddOrder", new object[]
            {
                order
            }, callback, asyncState);
        }

        /// <remarks/>
        public AddOrderResult EndAddOrder(IAsyncResult asyncResult)
        {
            object[] results = EndInvoke(asyncResult);
            return ((AddOrderResult) (results[0]));
        }

        /// <remarks/>
        public void AddOrderAsync(VeraCoreExportOrder order)
        {
            AddOrderAsync(order, null);
        }

        /// <remarks/>
        public void AddOrderAsync(VeraCoreExportOrder order, object userState)
        {
            if ((AddOrderOperationCompleted == null))
            {
                AddOrderOperationCompleted = new SendOrPostCallback(OnAddOrderOperationCompleted);
            }
            InvokeAsync("AddOrder", new object[]
            {
                order
            }, AddOrderOperationCompleted, userState);
        }

        private void OnAddOrderOperationCompleted(object arg)
        {
            if ((AddOrderCompleted != null))
            {
                InvokeCompletedEventArgs invokeArgs = ((InvokeCompletedEventArgs) (arg));
                AddOrderCompleted(this,
                    new AddOrderCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [SoapHeader("AuthenticationHeaderValue")]
        [SoapHeader("DebugHeaderValue", Direction = SoapHeaderDirection.InOut)]
        [SoapDocumentMethod("http://sma-promail/AddSizeColorCluster", RequestNamespace = "http://sma-promail/",
            ResponseNamespace = "http://sma-promail/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public AddSizeColorClusterResult AddSizeColorCluster(SizeColorCluster cluster)
        {
            object[] results = Invoke("AddSizeColorCluster", new object[]
            {
                cluster
            });
            return ((AddSizeColorClusterResult) (results[0]));
        }

        /// <remarks/>
        public IAsyncResult BeginAddSizeColorCluster(SizeColorCluster cluster, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("AddSizeColorCluster", new object[]
            {
                cluster
            }, callback, asyncState);
        }

        /// <remarks/>
        public AddSizeColorClusterResult EndAddSizeColorCluster(IAsyncResult asyncResult)
        {
            object[] results = EndInvoke(asyncResult);
            return ((AddSizeColorClusterResult) (results[0]));
        }

        /// <remarks/>
        public void AddSizeColorClusterAsync(SizeColorCluster cluster)
        {
            AddSizeColorClusterAsync(cluster, null);
        }

        /// <remarks/>
        public void AddSizeColorClusterAsync(SizeColorCluster cluster, object userState)
        {
            if ((AddSizeColorClusterOperationCompleted == null))
            {
                AddSizeColorClusterOperationCompleted = new SendOrPostCallback(OnAddSizeColorClusterOperationCompleted);
            }
            InvokeAsync("AddSizeColorCluster", new object[]
            {
                cluster
            }, AddSizeColorClusterOperationCompleted, userState);
        }

        private void OnAddSizeColorClusterOperationCompleted(object arg)
        {
            if ((AddSizeColorClusterCompleted != null))
            {
                InvokeCompletedEventArgs invokeArgs = ((InvokeCompletedEventArgs) (arg));
                AddSizeColorClusterCompleted(this,
                    new AddSizeColorClusterCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled,
                        invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [SoapHeader("AuthenticationHeaderValue")]
        [SoapHeader("DebugHeaderValue", Direction = SoapHeaderDirection.InOut)]
        [SoapDocumentMethod("http://sma-promail/AddProductListCluster", RequestNamespace = "http://sma-promail/",
            ResponseNamespace = "http://sma-promail/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public AddProductListClusterResult AddProductListCluster(ProductListCluster cluster)
        {
            object[] results = Invoke("AddProductListCluster", new object[]
            {
                cluster
            });
            return ((AddProductListClusterResult) (results[0]));
        }

        /// <remarks/>
        public IAsyncResult BeginAddProductListCluster(ProductListCluster cluster, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("AddProductListCluster", new object[]
            {
                cluster
            }, callback, asyncState);
        }

        /// <remarks/>
        public AddProductListClusterResult EndAddProductListCluster(IAsyncResult asyncResult)
        {
            object[] results = EndInvoke(asyncResult);
            return ((AddProductListClusterResult) (results[0]));
        }

        /// <remarks/>
        public void AddProductListClusterAsync(ProductListCluster cluster)
        {
            AddProductListClusterAsync(cluster, null);
        }

        /// <remarks/>
        public void AddProductListClusterAsync(ProductListCluster cluster, object userState)
        {
            if ((AddProductListClusterOperationCompleted == null))
            {
                AddProductListClusterOperationCompleted = new SendOrPostCallback(OnAddProductListClusterOperationCompleted);
            }
            InvokeAsync("AddProductListCluster", new object[]
            {
                cluster
            }, AddProductListClusterOperationCompleted, userState);
        }

        private void OnAddProductListClusterOperationCompleted(object arg)
        {
            if ((AddProductListClusterCompleted != null))
            {
                InvokeCompletedEventArgs invokeArgs = ((InvokeCompletedEventArgs) (arg));
                AddProductListClusterCompleted(this,
                    new AddProductListClusterCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled,
                        invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [SoapHeader("AuthenticationHeaderValue")]
        [SoapDocumentMethod("http://sma-promail/CreateProductFromJSON", RequestNamespace = "http://sma-promail/",
            ResponseNamespace = "http://sma-promail/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public Product CreateProductFromJSON(string jsonProduct)
        {
            object[] results = Invoke("CreateProductFromJSON", new object[]
            {
                jsonProduct
            });
            return ((Product) (results[0]));
        }

        /// <remarks/>
        public IAsyncResult BeginCreateProductFromJSON(string jsonProduct, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("CreateProductFromJSON", new object[]
            {
                jsonProduct
            }, callback, asyncState);
        }

        /// <remarks/>
        public Product EndCreateProductFromJSON(IAsyncResult asyncResult)
        {
            object[] results = EndInvoke(asyncResult);
            return ((Product) (results[0]));
        }

        /// <remarks/>
        public void CreateProductFromJSONAsync(string jsonProduct)
        {
            CreateProductFromJSONAsync(jsonProduct, null);
        }

        /// <remarks/>
        public void CreateProductFromJSONAsync(string jsonProduct, object userState)
        {
            if ((CreateProductFromJSONOperationCompleted == null))
            {
                CreateProductFromJSONOperationCompleted = new SendOrPostCallback(OnCreateProductFromJSONOperationCompleted);
            }
            InvokeAsync("CreateProductFromJSON", new object[]
            {
                jsonProduct
            }, CreateProductFromJSONOperationCompleted, userState);
        }

        private void OnCreateProductFromJSONOperationCompleted(object arg)
        {
            if ((CreateProductFromJSONCompleted != null))
            {
                InvokeCompletedEventArgs invokeArgs = ((InvokeCompletedEventArgs) (arg));
                CreateProductFromJSONCompleted(this,
                    new CreateProductFromJSONCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled,
                        invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [SoapHeader("AuthenticationHeaderValue")]
        [SoapDocumentMethod("http://sma-promail/CreateOfferFromJSON", RequestNamespace = "http://sma-promail/",
            ResponseNamespace = "http://sma-promail/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public Offer CreateOfferFromJSON(string jsonOffer)
        {
            object[] results = Invoke("CreateOfferFromJSON", new object[]
            {
                jsonOffer
            });
            return ((Offer) (results[0]));
        }

        /// <remarks/>
        public IAsyncResult BeginCreateOfferFromJSON(string jsonOffer, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("CreateOfferFromJSON", new object[]
            {
                jsonOffer
            }, callback, asyncState);
        }

        /// <remarks/>
        public Offer EndCreateOfferFromJSON(IAsyncResult asyncResult)
        {
            object[] results = EndInvoke(asyncResult);
            return ((Offer) (results[0]));
        }

        /// <remarks/>
        public void CreateOfferFromJSONAsync(string jsonOffer)
        {
            CreateOfferFromJSONAsync(jsonOffer, null);
        }

        /// <remarks/>
        public void CreateOfferFromJSONAsync(string jsonOffer, object userState)
        {
            if ((CreateOfferFromJSONOperationCompleted == null))
            {
                CreateOfferFromJSONOperationCompleted = new SendOrPostCallback(OnCreateOfferFromJSONOperationCompleted);
            }
            InvokeAsync("CreateOfferFromJSON", new object[]
            {
                jsonOffer
            }, CreateOfferFromJSONOperationCompleted, userState);
        }

        private void OnCreateOfferFromJSONOperationCompleted(object arg)
        {
            if ((CreateOfferFromJSONCompleted != null))
            {
                InvokeCompletedEventArgs invokeArgs = ((InvokeCompletedEventArgs) (arg));
                CreateOfferFromJSONCompleted(this,
                    new CreateOfferFromJSONCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled,
                        invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [SoapHeader("AuthenticationHeaderValue")]
        [SoapDocumentMethod("http://sma-promail/CreateExpectedArrivalFromJSON", RequestNamespace = "http://sma-promail/",
            ResponseNamespace = "http://sma-promail/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public ExpectedArrival CreateExpectedArrivalFromJSON(string jsonExpectedArrival)
        {
            object[] results = Invoke("CreateExpectedArrivalFromJSON", new object[]
            {
                jsonExpectedArrival
            });
            return ((ExpectedArrival) (results[0]));
        }

        /// <remarks/>
        public IAsyncResult BeginCreateExpectedArrivalFromJSON(string jsonExpectedArrival, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("CreateExpectedArrivalFromJSON", new object[]
            {
                jsonExpectedArrival
            }, callback, asyncState);
        }

        /// <remarks/>
        public ExpectedArrival EndCreateExpectedArrivalFromJSON(IAsyncResult asyncResult)
        {
            object[] results = EndInvoke(asyncResult);
            return ((ExpectedArrival) (results[0]));
        }

        /// <remarks/>
        public void CreateExpectedArrivalFromJSONAsync(string jsonExpectedArrival)
        {
            CreateExpectedArrivalFromJSONAsync(jsonExpectedArrival, null);
        }

        /// <remarks/>
        public void CreateExpectedArrivalFromJSONAsync(string jsonExpectedArrival, object userState)
        {
            if ((CreateExpectedArrivalFromJSONOperationCompleted == null))
            {
                CreateExpectedArrivalFromJSONOperationCompleted = new SendOrPostCallback(OnCreateExpectedArrivalFromJSONOperationCompleted);
            }
            InvokeAsync("CreateExpectedArrivalFromJSON", new object[]
            {
                jsonExpectedArrival
            }, CreateExpectedArrivalFromJSONOperationCompleted, userState);
        }

        private void OnCreateExpectedArrivalFromJSONOperationCompleted(object arg)
        {
            if ((CreateExpectedArrivalFromJSONCompleted != null))
            {
                InvokeCompletedEventArgs invokeArgs = ((InvokeCompletedEventArgs) (arg));
                CreateExpectedArrivalFromJSONCompleted(this,
                    new CreateExpectedArrivalFromJSONCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled,
                        invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [SoapHeader("AuthenticationHeaderValue")]
        [SoapDocumentMethod("http://sma-promail/CreatePurchaseOrderFromJSON", RequestNamespace = "http://sma-promail/",
            ResponseNamespace = "http://sma-promail/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public PurchaseOrder CreatePurchaseOrderFromJSON(string jsonPurchaseOrder)
        {
            object[] results = Invoke("CreatePurchaseOrderFromJSON", new object[]
            {
                jsonPurchaseOrder
            });
            return ((PurchaseOrder) (results[0]));
        }

        /// <remarks/>
        public IAsyncResult BeginCreatePurchaseOrderFromJSON(string jsonPurchaseOrder, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("CreatePurchaseOrderFromJSON", new object[]
            {
                jsonPurchaseOrder
            }, callback, asyncState);
        }

        /// <remarks/>
        public PurchaseOrder EndCreatePurchaseOrderFromJSON(IAsyncResult asyncResult)
        {
            object[] results = EndInvoke(asyncResult);
            return ((PurchaseOrder) (results[0]));
        }

        /// <remarks/>
        public void CreatePurchaseOrderFromJSONAsync(string jsonPurchaseOrder)
        {
            CreatePurchaseOrderFromJSONAsync(jsonPurchaseOrder, null);
        }

        /// <remarks/>
        public void CreatePurchaseOrderFromJSONAsync(string jsonPurchaseOrder, object userState)
        {
            if ((CreatePurchaseOrderFromJSONOperationCompleted == null))
            {
                CreatePurchaseOrderFromJSONOperationCompleted = new SendOrPostCallback(OnCreatePurchaseOrderFromJSONOperationCompleted);
            }
            InvokeAsync("CreatePurchaseOrderFromJSON", new object[]
            {
                jsonPurchaseOrder
            }, CreatePurchaseOrderFromJSONOperationCompleted, userState);
        }

        private void OnCreatePurchaseOrderFromJSONOperationCompleted(object arg)
        {
            if ((CreatePurchaseOrderFromJSONCompleted != null))
            {
                InvokeCompletedEventArgs invokeArgs = ((InvokeCompletedEventArgs) (arg));
                CreatePurchaseOrderFromJSONCompleted(this,
                    new CreatePurchaseOrderFromJSONCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled,
                        invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [SoapHeader("AuthenticationHeaderValue")]
        [SoapDocumentMethod("http://sma-promail/CreateUserPreRegisteredFromJSON", RequestNamespace = "http://sma-promail/",
            ResponseNamespace = "http://sma-promail/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public UserPreRegistered CreateUserPreRegisteredFromJSON(string json)
        {
            object[] results = Invoke("CreateUserPreRegisteredFromJSON", new object[]
            {
                json
            });
            return ((UserPreRegistered) (results[0]));
        }

        /// <remarks/>
        public IAsyncResult BeginCreateUserPreRegisteredFromJSON(string json, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("CreateUserPreRegisteredFromJSON", new object[]
            {
                json
            }, callback, asyncState);
        }

        /// <remarks/>
        public UserPreRegistered EndCreateUserPreRegisteredFromJSON(IAsyncResult asyncResult)
        {
            object[] results = EndInvoke(asyncResult);
            return ((UserPreRegistered) (results[0]));
        }

        /// <remarks/>
        public void CreateUserPreRegisteredFromJSONAsync(string json)
        {
            CreateUserPreRegisteredFromJSONAsync(json, null);
        }

        /// <remarks/>
        public void CreateUserPreRegisteredFromJSONAsync(string json, object userState)
        {
            if ((CreateUserPreRegisteredFromJSONOperationCompleted == null))
            {
                CreateUserPreRegisteredFromJSONOperationCompleted =
                    new SendOrPostCallback(OnCreateUserPreRegisteredFromJSONOperationCompleted);
            }
            InvokeAsync("CreateUserPreRegisteredFromJSON", new object[]
            {
                json
            }, CreateUserPreRegisteredFromJSONOperationCompleted, userState);
        }

        private void OnCreateUserPreRegisteredFromJSONOperationCompleted(object arg)
        {
            if ((CreateUserPreRegisteredFromJSONCompleted != null))
            {
                InvokeCompletedEventArgs invokeArgs = ((InvokeCompletedEventArgs) (arg));
                CreateUserPreRegisteredFromJSONCompleted(this,
                    new CreateUserPreRegisteredFromJSONCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled,
                        invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [SoapHeader("AuthenticationHeaderValue")]
        [SoapDocumentMethod("http://sma-promail/CreateSizeColorClusterFromJSON", RequestNamespace = "http://sma-promail/",
            ResponseNamespace = "http://sma-promail/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public SizeColorCluster CreateSizeColorClusterFromJSON(string json)
        {
            object[] results = Invoke("CreateSizeColorClusterFromJSON", new object[]
            {
                json
            });
            return ((SizeColorCluster) (results[0]));
        }

        /// <remarks/>
        public IAsyncResult BeginCreateSizeColorClusterFromJSON(string json, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("CreateSizeColorClusterFromJSON", new object[]
            {
                json
            }, callback, asyncState);
        }

        /// <remarks/>
        public SizeColorCluster EndCreateSizeColorClusterFromJSON(IAsyncResult asyncResult)
        {
            object[] results = EndInvoke(asyncResult);
            return ((SizeColorCluster) (results[0]));
        }

        /// <remarks/>
        public void CreateSizeColorClusterFromJSONAsync(string json)
        {
            CreateSizeColorClusterFromJSONAsync(json, null);
        }

        /// <remarks/>
        public void CreateSizeColorClusterFromJSONAsync(string json, object userState)
        {
            if ((CreateSizeColorClusterFromJSONOperationCompleted == null))
            {
                CreateSizeColorClusterFromJSONOperationCompleted = new SendOrPostCallback(OnCreateSizeColorClusterFromJSONOperationCompleted);
            }
            InvokeAsync("CreateSizeColorClusterFromJSON", new object[]
            {
                json
            }, CreateSizeColorClusterFromJSONOperationCompleted, userState);
        }

        private void OnCreateSizeColorClusterFromJSONOperationCompleted(object arg)
        {
            if ((CreateSizeColorClusterFromJSONCompleted != null))
            {
                InvokeCompletedEventArgs invokeArgs = ((InvokeCompletedEventArgs) (arg));
                CreateSizeColorClusterFromJSONCompleted(this,
                    new CreateSizeColorClusterFromJSONCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled,
                        invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [SoapHeader("AuthenticationHeaderValue")]
        [SoapDocumentMethod("http://sma-promail/CreateProductListClusterFromJSON", RequestNamespace = "http://sma-promail/",
            ResponseNamespace = "http://sma-promail/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public ProductListCluster CreateProductListClusterFromJSON(string json)
        {
            object[] results = Invoke("CreateProductListClusterFromJSON", new object[]
            {
                json
            });
            return ((ProductListCluster) (results[0]));
        }

        /// <remarks/>
        public IAsyncResult BeginCreateProductListClusterFromJSON(string json, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("CreateProductListClusterFromJSON", new object[]
            {
                json
            }, callback, asyncState);
        }

        /// <remarks/>
        public ProductListCluster EndCreateProductListClusterFromJSON(IAsyncResult asyncResult)
        {
            object[] results = EndInvoke(asyncResult);
            return ((ProductListCluster) (results[0]));
        }

        /// <remarks/>
        public void CreateProductListClusterFromJSONAsync(string json)
        {
            CreateProductListClusterFromJSONAsync(json, null);
        }

        /// <remarks/>
        public void CreateProductListClusterFromJSONAsync(string json, object userState)
        {
            if ((CreateProductListClusterFromJSONOperationCompleted == null))
            {
                CreateProductListClusterFromJSONOperationCompleted =
                    new SendOrPostCallback(OnCreateProductListClusterFromJSONOperationCompleted);
            }
            InvokeAsync("CreateProductListClusterFromJSON", new object[]
            {
                json
            }, CreateProductListClusterFromJSONOperationCompleted, userState);
        }

        private void OnCreateProductListClusterFromJSONOperationCompleted(object arg)
        {
            if ((CreateProductListClusterFromJSONCompleted != null))
            {
                InvokeCompletedEventArgs invokeArgs = ((InvokeCompletedEventArgs) (arg));
                CreateProductListClusterFromJSONCompleted(this,
                    new CreateProductListClusterFromJSONCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled,
                        invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [SoapHeader("AuthenticationHeaderValue")]
        [SoapHeader("DebugHeaderValue", Direction = SoapHeaderDirection.InOut)]
        [SoapDocumentMethod("http://sma-promail/GetProductAvailabilities", RequestNamespace = "http://sma-promail/",
            ResponseNamespace = "http://sma-promail/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public ProductAvailabilities[] GetProductAvailabilities(string partNumber, string owner, string sizeCode, string colorCode,
            string sizeDescription, string colorDescription)
        {
            object[] results = Invoke("GetProductAvailabilities", new object[]
            {
                partNumber,
                owner,
                sizeCode,
                colorCode,
                sizeDescription,
                colorDescription
            });
            return ((ProductAvailabilities[]) (results[0]));
        }

        /// <remarks/>
        public IAsyncResult BeginGetProductAvailabilities(string partNumber, string owner, string sizeCode, string colorCode,
            string sizeDescription, string colorDescription, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("GetProductAvailabilities", new object[]
            {
                partNumber,
                owner,
                sizeCode,
                colorCode,
                sizeDescription,
                colorDescription
            }, callback, asyncState);
        }

        /// <remarks/>
        public ProductAvailabilities[] EndGetProductAvailabilities(IAsyncResult asyncResult)
        {
            object[] results = EndInvoke(asyncResult);
            return ((ProductAvailabilities[]) (results[0]));
        }

        /// <remarks/>
        public void GetProductAvailabilitiesAsync(string partNumber, string owner, string sizeCode, string colorCode, string sizeDescription,
            string colorDescription)
        {
            GetProductAvailabilitiesAsync(partNumber, owner, sizeCode, colorCode, sizeDescription, colorDescription, null);
        }

        /// <remarks/>
        public void GetProductAvailabilitiesAsync(string partNumber, string owner, string sizeCode, string colorCode, string sizeDescription,
            string colorDescription, object userState)
        {
            if ((GetProductAvailabilitiesOperationCompleted == null))
            {
                GetProductAvailabilitiesOperationCompleted = new SendOrPostCallback(OnGetProductAvailabilitiesOperationCompleted);
            }
            InvokeAsync("GetProductAvailabilities", new object[]
            {
                partNumber,
                owner,
                sizeCode,
                colorCode,
                sizeDescription,
                colorDescription
            }, GetProductAvailabilitiesOperationCompleted, userState);
        }

        private void OnGetProductAvailabilitiesOperationCompleted(object arg)
        {
            if ((GetProductAvailabilitiesCompleted != null))
            {
                InvokeCompletedEventArgs invokeArgs = ((InvokeCompletedEventArgs) (arg));
                GetProductAvailabilitiesCompleted(this,
                    new GetProductAvailabilitiesCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled,
                        invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [SoapHeader("AuthenticationHeaderValue")]
        [SoapHeader("DebugHeaderValue", Direction = SoapHeaderDirection.InOut)]
        [SoapDocumentMethod("http://sma-promail/GetProduct", RequestNamespace = "http://sma-promail/",
            ResponseNamespace = "http://sma-promail/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public GetProductResult GetProduct(string OwnerID, string ProductID)
        {
            object[] results = Invoke("GetProduct", new object[]
            {
                OwnerID,
                ProductID
            });
            return ((GetProductResult) (results[0]));
        }

        /// <remarks/>
        public IAsyncResult BeginGetProduct(string OwnerID, string ProductID, AsyncCallback callback, object asyncState)
        {
            return BeginInvoke("GetProduct", new object[]
            {
                OwnerID,
                ProductID
            }, callback, asyncState);
        }

        /// <remarks/>
        public GetProductResult EndGetProduct(IAsyncResult asyncResult)
        {
            object[] results = EndInvoke(asyncResult);
            return ((GetProductResult) (results[0]));
        }

        /// <remarks/>
        public void GetProductAsync(string OwnerID, string ProductID)
        {
            GetProductAsync(OwnerID, ProductID, null);
        }

        /// <remarks/>
        public void GetProductAsync(string OwnerID, string ProductID, object userState)
        {
            if ((GetProductOperationCompleted == null))
            {
                GetProductOperationCompleted = new SendOrPostCallback(OnGetProductOperationCompleted);
            }
            InvokeAsync("GetProduct", new object[]
            {
                OwnerID,
                ProductID
            }, GetProductOperationCompleted, userState);
        }

        private void OnGetProductOperationCompleted(object arg)
        {
            if ((GetProductCompleted != null))
            {
                InvokeCompletedEventArgs invokeArgs = ((InvokeCompletedEventArgs) (arg));
                GetProductCompleted(this,
                    new GetProductCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled, invokeArgs.UserState));
            }
        }

        /// <remarks/>
        [SoapHeader("AuthenticationHeaderValue")]
        [SoapHeader("DebugHeaderValue", Direction = SoapHeaderDirection.InOut)]
        [SoapDocumentMethod("http://sma-promail/GetExpectedArrivals", RequestNamespace = "http://sma-promail/",
            ResponseNamespace = "http://sma-promail/", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Wrapped)]
        public ExpectedArrivals[] GetExpectedArrivals(DateTime StartDate, DateTime EndDate, string dataSelection)
        {
            object[] results = Invoke("GetExpectedArrivals", new object[]
            {
                StartDate,
                EndDate,
                dataSelection
            });
            return ((ExpectedArrivals[]) (results[0]));
        }

        /// <remarks/>
        public IAsyncResult BeginGetExpectedArrivals(DateTime StartDate, DateTime EndDate, string dataSelection, AsyncCallback callback,
            object asyncState)
        {
            return BeginInvoke("GetExpectedArrivals", new object[]
            {
                StartDate,
                EndDate,
                dataSelection
            }, callback, asyncState);
        }

        /// <remarks/>
        public ExpectedArrivals[] EndGetExpectedArrivals(IAsyncResult asyncResult)
        {
            object[] results = EndInvoke(asyncResult);
            return ((ExpectedArrivals[]) (results[0]));
        }

        /// <remarks/>
        public void GetExpectedArrivalsAsync(DateTime StartDate, DateTime EndDate, string dataSelection)
        {
            GetExpectedArrivalsAsync(StartDate, EndDate, dataSelection, null);
        }

        /// <remarks/>
        public void GetExpectedArrivalsAsync(DateTime StartDate, DateTime EndDate, string dataSelection, object userState)
        {
            if ((GetExpectedArrivalsOperationCompleted == null))
            {
                GetExpectedArrivalsOperationCompleted = new SendOrPostCallback(OnGetExpectedArrivalsOperationCompleted);
            }
            InvokeAsync("GetExpectedArrivals", new object[]
            {
                StartDate,
                EndDate,
                dataSelection
            }, GetExpectedArrivalsOperationCompleted, userState);
        }

        private void OnGetExpectedArrivalsOperationCompleted(object arg)
        {
            if ((GetExpectedArrivalsCompleted != null))
            {
                InvokeCompletedEventArgs invokeArgs = ((InvokeCompletedEventArgs) (arg));
                GetExpectedArrivalsCompleted(this,
                    new GetExpectedArrivalsCompletedEventArgs(invokeArgs.Results, invokeArgs.Error, invokeArgs.Cancelled,
                        invokeArgs.UserState));
            }
        }

        /// <remarks/>
        public new void CancelAsync(object userState)
        {
            base.CancelAsync(userState);
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    [XmlRoot(Namespace = "http://sma-promail/", IsNullable = false)]
    public partial class AuthenticationHeader : SoapHeader
    {

        private string usernameField;

        private string passwordField;

        private System.Xml.XmlAttribute[] anyAttrField;

        /// <remarks/>
        public string Username
        {
            get { return usernameField; }
            set { usernameField = value; }
        }

        /// <remarks/>
        public string Password
        {
            get { return passwordField; }
            set { passwordField = value; }
        }

        /// <remarks/>
        [XmlAnyAttribute()]
        public System.Xml.XmlAttribute[] AnyAttr
        {
            get { return anyAttrField; }
            set { anyAttrField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class AddProductListClusterResult
    {

        private bool newClusterField;

        private int clusterSeqIDField;

        private int? offerSeqIDField;

        /// <remarks/>
        public bool NewCluster
        {
            get { return newClusterField; }
            set { newClusterField = value; }
        }

        /// <remarks/>
        public int ClusterSeqID
        {
            get { return clusterSeqIDField; }
            set { clusterSeqIDField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? OfferSeqID
        {
            get { return offerSeqIDField; }
            set { offerSeqIDField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ProductListCluster
    {

        private ProductCluster clusterField;

        private Offer offerField;

        private ProductID[] productIDsField;

        private ProductActivation[] activationField;

        /// <remarks/>
        public ProductCluster Cluster
        {
            get { return clusterField; }
            set { clusterField = value; }
        }

        /// <remarks/>
        public Offer Offer
        {
            get { return offerField; }
            set { offerField = value; }
        }

        /// <remarks/>
        public ProductID[] ProductIDs
        {
            get { return productIDsField; }
            set { productIDsField = value; }
        }

        /// <remarks/>
        public ProductActivation[] Activation
        {
            get { return activationField; }
            set { activationField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ProductCluster : PMObject
    {

        private int? seqIDField;

        private string partNumberField;

        private string descriptionField;

        private bool? isProductListField;

        private EntryMode? orderingModeField;

        private string dropDownTextField;

        private string productIDHeaderField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string PartNumber
        {
            get { return partNumberField; }
            set { partNumberField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? IsProductList
        {
            get { return isProductListField; }
            set { isProductListField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public EntryMode? OrderingMode
        {
            get { return orderingModeField; }
            set { orderingModeField = value; }
        }

        /// <remarks/>
        public string DropDownText
        {
            get { return dropDownTextField; }
            set { dropDownTextField = value; }
        }

        /// <remarks/>
        public string ProductIDHeader
        {
            get { return productIDHeaderField; }
            set { productIDHeaderField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum EntryMode
    {

        /// <remarks/>
        DropDown,

        /// <remarks/>
        TableListing,

        /// <remarks/>
        IDAndDescriptionTableListing,
    }

    /// <remarks/>
    [XmlInclude(typeof(ExpectedArrivals))]
    [XmlInclude(typeof(GetProductResult))]
    [XmlInclude(typeof(WarehouseLevels))]
    [XmlInclude(typeof(ProductAvailabilities))]
    [XmlInclude(typeof(OrderBudget))]
    [XmlInclude(typeof(OrderRecurrenceSchedule))]
    [XmlInclude(typeof(OfferVariable))]
    [XmlInclude(typeof(OfferOrdered))]
    [XmlInclude(typeof(FreightCode))]
    [XmlInclude(typeof(PickPack))]
    [XmlInclude(typeof(ORDOBYID))]
    [XmlInclude(typeof(OrderMailer))]
    [XmlInclude(typeof(OrderShipTo))]
    [XmlInclude(typeof(OrderBillTo))]
    [XmlInclude(typeof(OrderedBy))]
    [XmlInclude(typeof(OrderVariable))]
    [XmlInclude(typeof(PaymentType))]
    [XmlInclude(typeof(OrderPayment))]
    [XmlInclude(typeof(GiftCertificate))]
    [XmlInclude(typeof(Coupon))]
    [XmlInclude(typeof(DiscountCode))]
    [XmlInclude(typeof(NoChargeType))]
    [XmlInclude(typeof(OrderMoney))]
    [XmlInclude(typeof(FreightAccount))]
    [XmlInclude(typeof(FreightService))]
    [XmlInclude(typeof(FreightCarrier))]
    [XmlInclude(typeof(OrderShipping))]
    [XmlInclude(typeof(CustomerProject))]
    [XmlInclude(typeof(SourceDetail))]
    [XmlInclude(typeof(Source))]
    [XmlInclude(typeof(ResponseMedia))]
    [XmlInclude(typeof(OrderClassification))]
    [XmlInclude(typeof(OrderHeader))]
    [XmlInclude(typeof(VeraCoreExportOrder))]
    [XmlInclude(typeof(ShippingCharges))]
    [XmlInclude(typeof(ProductReturns))]
    [XmlInclude(typeof(ShippingActivity))]
    [XmlInclude(typeof(ProductShipment))]
    [XmlInclude(typeof(BillingDetail))]
    [XmlInclude(typeof(PMSystemCategory))]
    [XmlInclude(typeof(Profile))]
    [XmlInclude(typeof(CategoryGroupDetail))]
    [XmlInclude(typeof(CategoryGroup))]
    [XmlInclude(typeof(OrderEntryView))]
    [XmlInclude(typeof(RestrictedOwner))]
    [XmlInclude(typeof(UserCategoryRestriction))]
    [XmlInclude(typeof(ApprovalGroup))]
    [XmlInclude(typeof(ApprovalGroupUser))]
    [XmlInclude(typeof(ORDOBY))]
    [XmlInclude(typeof(UserPreRegistered))]
    [XmlInclude(typeof(PersonVariable))]
    [XmlInclude(typeof(MailerClass))]
    [XmlInclude(typeof(PersonBilling))]
    [XmlInclude(typeof(PersonContact))]
    [XmlInclude(typeof(PersonAddress))]
    [XmlInclude(typeof(PersonCompany))]
    [XmlInclude(typeof(PersonName))]
    [XmlInclude(typeof(Person))]
    [XmlInclude(typeof(PriceClassStructure))]
    [XmlInclude(typeof(RangePrice))]
    [XmlInclude(typeof(GetOfferResult))]
    [XmlInclude(typeof(ExpectedArrivalComponent))]
    [XmlInclude(typeof(ExpectedArrival))]
    [XmlInclude(typeof(CustomAssemblyGroup))]
    [XmlInclude(typeof(OfferComponent))]
    [XmlInclude(typeof(RestrictionType))]
    [XmlInclude(typeof(OfferRestriction))]
    [XmlInclude(typeof(EDelivery))]
    [XmlInclude(typeof(OfferDropShip))]
    [XmlInclude(typeof(CustomCategoryDef))]
    [XmlInclude(typeof(CustomCategory))]
    [XmlInclude(typeof(OfferCategory))]
    [XmlInclude(typeof(OfferSortGroupXRef))]
    [XmlInclude(typeof(OfferSortLevel))]
    [XmlInclude(typeof(OfferSortGroup))]
    [XmlInclude(typeof(OfferSort))]
    [XmlInclude(typeof(OfferCategorization))]
    [XmlInclude(typeof(OfferImage))]
    [XmlInclude(typeof(OfferImages))]
    [XmlInclude(typeof(OfferLinks))]
    [XmlInclude(typeof(CustomizationProfile))]
    [XmlInclude(typeof(RemoteSystem))]
    [XmlInclude(typeof(CustomizationCode))]
    [XmlInclude(typeof(OfferCustomization))]
    [XmlInclude(typeof(OfferRecurrence))]
    [XmlInclude(typeof(PriceClass))]
    [XmlInclude(typeof(PricingDetail))]
    [XmlInclude(typeof(PricingStructure))]
    [XmlInclude(typeof(PriceBreaks))]
    [XmlInclude(typeof(PriceFamily))]
    [XmlInclude(typeof(OfferPricing))]
    [XmlInclude(typeof(OfferStrings))]
    [XmlInclude(typeof(ShippingCategory))]
    [XmlInclude(typeof(AllowedOfferQuantity))]
    [XmlInclude(typeof(UnitOfMeasure))]
    [XmlInclude(typeof(OfferSettings))]
    [XmlInclude(typeof(OfferStatus))]
    [XmlInclude(typeof(GLCode))]
    [XmlInclude(typeof(RevenueCenter))]
    [XmlInclude(typeof(OfferInfo))]
    [XmlInclude(typeof(OfferHeader))]
    [XmlInclude(typeof(Offer))]
    [XmlInclude(typeof(ClusterSurcharge))]
    [XmlInclude(typeof(SupplierPart))]
    [XmlInclude(typeof(DropShipSupplier))]
    [XmlInclude(typeof(Upsell))]
    [XmlInclude(typeof(ShippingOption))]
    [XmlInclude(typeof(RecurrenceScheduleShippingOption))]
    [XmlInclude(typeof(RecurrenceSchedules))]
    [XmlInclude(typeof(OfferScheduleParameters))]
    [XmlInclude(typeof(OfferIDHeader))]
    [XmlInclude(typeof(OfferID))]
    [XmlInclude(typeof(OfferCycleBillOfMaterials))]
    [XmlInclude(typeof(ProductBillingContainers))]
    [XmlInclude(typeof(UniversalProductCode))]
    [XmlInclude(typeof(PurchaseOrderProduct))]
    [XmlInclude(typeof(Vendor))]
    [XmlInclude(typeof(PurchaseOrder))]
    [XmlInclude(typeof(OnOrder))]
    [XmlInclude(typeof(RawMaterial))]
    [XmlInclude(typeof(ProductCluster))]
    [XmlInclude(typeof(OMSSystem))]
    [XmlInclude(typeof(ProductActivation))]
    [XmlInclude(typeof(ExtensionUpload))]
    [XmlInclude(typeof(UserDefinedChoice))]
    [XmlInclude(typeof(VariableField))]
    [XmlInclude(typeof(ProductVariable))]
    [XmlInclude(typeof(KitComponent))]
    [XmlInclude(typeof(ProductKit))]
    [XmlInclude(typeof(VersionDisposition))]
    [XmlInclude(typeof(VersionStatus))]
    [XmlInclude(typeof(ProductIDSort))]
    [XmlInclude(typeof(ProductIDHeader))]
    [XmlInclude(typeof(ProductVersion))]
    [XmlInclude(typeof(SizeClass))]
    [XmlInclude(typeof(Size))]
    [XmlInclude(typeof(ColorClass))]
    [XmlInclude(typeof(Color))]
    [XmlInclude(typeof(ProductCharacteristics))]
    [XmlInclude(typeof(ProductSortLevel))]
    [XmlInclude(typeof(ProductSortGroup))]
    [XmlInclude(typeof(ProductType))]
    [XmlInclude(typeof(ProductSort))]
    [XmlInclude(typeof(Job))]
    [XmlInclude(typeof(CostCenter))]
    [XmlInclude(typeof(BillingCode))]
    [XmlInclude(typeof(SortGroupType))]
    [XmlInclude(typeof(Owner))]
    [XmlInclude(typeof(ProductHeader))]
    [XmlInclude(typeof(ProductBillFactor))]
    [XmlInclude(typeof(SignatureRequired))]
    [XmlInclude(typeof(ProductOptional))]
    [XmlInclude(typeof(ProductSN))]
    [XmlInclude(typeof(ProductWMSSystem))]
    [XmlInclude(typeof(ProductVersionTrack))]
    [XmlInclude(typeof(ProductGiftCertificate))]
    [XmlInclude(typeof(ProductAcquisition))]
    [XmlInclude(typeof(ProductValuation))]
    [XmlInclude(typeof(PMSystem))]
    [XmlInclude(typeof(Product))]
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class PMObject : PersistentObject
    {
    }

    /// <remarks/>
    [XmlInclude(typeof(PMObject))]
    [XmlInclude(typeof(ExpectedArrivals))]
    [XmlInclude(typeof(GetProductResult))]
    [XmlInclude(typeof(WarehouseLevels))]
    [XmlInclude(typeof(ProductAvailabilities))]
    [XmlInclude(typeof(OrderBudget))]
    [XmlInclude(typeof(OrderRecurrenceSchedule))]
    [XmlInclude(typeof(OfferVariable))]
    [XmlInclude(typeof(OfferOrdered))]
    [XmlInclude(typeof(FreightCode))]
    [XmlInclude(typeof(PickPack))]
    [XmlInclude(typeof(ORDOBYID))]
    [XmlInclude(typeof(OrderMailer))]
    [XmlInclude(typeof(OrderShipTo))]
    [XmlInclude(typeof(OrderBillTo))]
    [XmlInclude(typeof(OrderedBy))]
    [XmlInclude(typeof(OrderVariable))]
    [XmlInclude(typeof(PaymentType))]
    [XmlInclude(typeof(OrderPayment))]
    [XmlInclude(typeof(GiftCertificate))]
    [XmlInclude(typeof(Coupon))]
    [XmlInclude(typeof(DiscountCode))]
    [XmlInclude(typeof(NoChargeType))]
    [XmlInclude(typeof(OrderMoney))]
    [XmlInclude(typeof(FreightAccount))]
    [XmlInclude(typeof(FreightService))]
    [XmlInclude(typeof(FreightCarrier))]
    [XmlInclude(typeof(OrderShipping))]
    [XmlInclude(typeof(CustomerProject))]
    [XmlInclude(typeof(SourceDetail))]
    [XmlInclude(typeof(Source))]
    [XmlInclude(typeof(ResponseMedia))]
    [XmlInclude(typeof(OrderClassification))]
    [XmlInclude(typeof(OrderHeader))]
    [XmlInclude(typeof(VeraCoreExportOrder))]
    [XmlInclude(typeof(ShippingCharges))]
    [XmlInclude(typeof(ProductReturns))]
    [XmlInclude(typeof(ShippingActivity))]
    [XmlInclude(typeof(ProductShipment))]
    [XmlInclude(typeof(BillingDetail))]
    [XmlInclude(typeof(PMSystemCategory))]
    [XmlInclude(typeof(Profile))]
    [XmlInclude(typeof(CategoryGroupDetail))]
    [XmlInclude(typeof(CategoryGroup))]
    [XmlInclude(typeof(OrderEntryView))]
    [XmlInclude(typeof(RestrictedOwner))]
    [XmlInclude(typeof(UserCategoryRestriction))]
    [XmlInclude(typeof(ApprovalGroup))]
    [XmlInclude(typeof(ApprovalGroupUser))]
    [XmlInclude(typeof(ORDOBY))]
    [XmlInclude(typeof(UserPreRegistered))]
    [XmlInclude(typeof(PersonVariable))]
    [XmlInclude(typeof(MailerClass))]
    [XmlInclude(typeof(PersonBilling))]
    [XmlInclude(typeof(PersonContact))]
    [XmlInclude(typeof(PersonAddress))]
    [XmlInclude(typeof(PersonCompany))]
    [XmlInclude(typeof(PersonName))]
    [XmlInclude(typeof(Person))]
    [XmlInclude(typeof(PriceClassStructure))]
    [XmlInclude(typeof(RangePrice))]
    [XmlInclude(typeof(GetOfferResult))]
    [XmlInclude(typeof(ExpectedArrivalComponent))]
    [XmlInclude(typeof(ExpectedArrival))]
    [XmlInclude(typeof(CustomAssemblyGroup))]
    [XmlInclude(typeof(OfferComponent))]
    [XmlInclude(typeof(RestrictionType))]
    [XmlInclude(typeof(OfferRestriction))]
    [XmlInclude(typeof(EDelivery))]
    [XmlInclude(typeof(OfferDropShip))]
    [XmlInclude(typeof(CustomCategoryDef))]
    [XmlInclude(typeof(CustomCategory))]
    [XmlInclude(typeof(OfferCategory))]
    [XmlInclude(typeof(OfferSortGroupXRef))]
    [XmlInclude(typeof(OfferSortLevel))]
    [XmlInclude(typeof(OfferSortGroup))]
    [XmlInclude(typeof(OfferSort))]
    [XmlInclude(typeof(OfferCategorization))]
    [XmlInclude(typeof(OfferImage))]
    [XmlInclude(typeof(OfferImages))]
    [XmlInclude(typeof(OfferLinks))]
    [XmlInclude(typeof(CustomizationProfile))]
    [XmlInclude(typeof(RemoteSystem))]
    [XmlInclude(typeof(CustomizationCode))]
    [XmlInclude(typeof(OfferCustomization))]
    [XmlInclude(typeof(OfferRecurrence))]
    [XmlInclude(typeof(PriceClass))]
    [XmlInclude(typeof(PricingDetail))]
    [XmlInclude(typeof(PricingStructure))]
    [XmlInclude(typeof(PriceBreaks))]
    [XmlInclude(typeof(PriceFamily))]
    [XmlInclude(typeof(OfferPricing))]
    [XmlInclude(typeof(OfferStrings))]
    [XmlInclude(typeof(ShippingCategory))]
    [XmlInclude(typeof(AllowedOfferQuantity))]
    [XmlInclude(typeof(UnitOfMeasure))]
    [XmlInclude(typeof(OfferSettings))]
    [XmlInclude(typeof(OfferStatus))]
    [XmlInclude(typeof(GLCode))]
    [XmlInclude(typeof(RevenueCenter))]
    [XmlInclude(typeof(OfferInfo))]
    [XmlInclude(typeof(OfferHeader))]
    [XmlInclude(typeof(Offer))]
    [XmlInclude(typeof(ClusterSurcharge))]
    [XmlInclude(typeof(SupplierPart))]
    [XmlInclude(typeof(DropShipSupplier))]
    [XmlInclude(typeof(Upsell))]
    [XmlInclude(typeof(ShippingOption))]
    [XmlInclude(typeof(RecurrenceScheduleShippingOption))]
    [XmlInclude(typeof(RecurrenceSchedules))]
    [XmlInclude(typeof(OfferScheduleParameters))]
    [XmlInclude(typeof(OfferIDHeader))]
    [XmlInclude(typeof(OfferID))]
    [XmlInclude(typeof(OfferCycleBillOfMaterials))]
    [XmlInclude(typeof(ProductBillingContainers))]
    [XmlInclude(typeof(UniversalProductCode))]
    [XmlInclude(typeof(PurchaseOrderProduct))]
    [XmlInclude(typeof(Vendor))]
    [XmlInclude(typeof(PurchaseOrder))]
    [XmlInclude(typeof(OnOrder))]
    [XmlInclude(typeof(RawMaterial))]
    [XmlInclude(typeof(ProductCluster))]
    [XmlInclude(typeof(OMSSystem))]
    [XmlInclude(typeof(ProductActivation))]
    [XmlInclude(typeof(ExtensionUpload))]
    [XmlInclude(typeof(UserDefinedChoice))]
    [XmlInclude(typeof(VariableField))]
    [XmlInclude(typeof(ProductVariable))]
    [XmlInclude(typeof(KitComponent))]
    [XmlInclude(typeof(ProductKit))]
    [XmlInclude(typeof(VersionDisposition))]
    [XmlInclude(typeof(VersionStatus))]
    [XmlInclude(typeof(ProductIDSort))]
    [XmlInclude(typeof(ProductIDHeader))]
    [XmlInclude(typeof(ProductVersion))]
    [XmlInclude(typeof(SizeClass))]
    [XmlInclude(typeof(Size))]
    [XmlInclude(typeof(ColorClass))]
    [XmlInclude(typeof(Color))]
    [XmlInclude(typeof(ProductCharacteristics))]
    [XmlInclude(typeof(ProductSortLevel))]
    [XmlInclude(typeof(ProductSortGroup))]
    [XmlInclude(typeof(ProductType))]
    [XmlInclude(typeof(ProductSort))]
    [XmlInclude(typeof(Job))]
    [XmlInclude(typeof(CostCenter))]
    [XmlInclude(typeof(BillingCode))]
    [XmlInclude(typeof(SortGroupType))]
    [XmlInclude(typeof(Owner))]
    [XmlInclude(typeof(ProductHeader))]
    [XmlInclude(typeof(ProductBillFactor))]
    [XmlInclude(typeof(SignatureRequired))]
    [XmlInclude(typeof(ProductOptional))]
    [XmlInclude(typeof(ProductSN))]
    [XmlInclude(typeof(ProductWMSSystem))]
    [XmlInclude(typeof(ProductVersionTrack))]
    [XmlInclude(typeof(ProductGiftCertificate))]
    [XmlInclude(typeof(ProductAcquisition))]
    [XmlInclude(typeof(ProductValuation))]
    [XmlInclude(typeof(PMSystem))]
    [XmlInclude(typeof(Product))]
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public abstract partial class PersistentObject
    {
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ExpectedArrivals : PMObject
    {

        private int expectedArrivalIDField;

        private DateTime? dateTimeEnteredField;

        private string warehouseField;

        private string ownerIDField;

        private string ownerNameField;

        private string productIDField;

        private string productDescriptionField;

        private string versionField;

        private DateTime? expectedArrivalDateField;

        private int? qTYRequestedField;

        private int qTYReceivedtoDateField;

        private short completeField;

        private string ourPOField;

        private string clientPOField;

        private string shippingFromField;

        private string shippingMethodField;

        private string commentsField;

        /// <remarks/>
        public int ExpectedArrivalID
        {
            get { return expectedArrivalIDField; }
            set { expectedArrivalIDField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? DateTimeEntered
        {
            get { return dateTimeEnteredField; }
            set { dateTimeEnteredField = value; }
        }

        /// <remarks/>
        public string Warehouse
        {
            get { return warehouseField; }
            set { warehouseField = value; }
        }

        /// <remarks/>
        public string OwnerID
        {
            get { return ownerIDField; }
            set { ownerIDField = value; }
        }

        /// <remarks/>
        public string OwnerName
        {
            get { return ownerNameField; }
            set { ownerNameField = value; }
        }

        /// <remarks/>
        public string ProductID
        {
            get { return productIDField; }
            set { productIDField = value; }
        }

        /// <remarks/>
        public string ProductDescription
        {
            get { return productDescriptionField; }
            set { productDescriptionField = value; }
        }

        /// <remarks/>
        public string Version
        {
            get { return versionField; }
            set { versionField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? ExpectedArrivalDate
        {
            get { return expectedArrivalDateField; }
            set { expectedArrivalDateField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? QTYRequested
        {
            get { return qTYRequestedField; }
            set { qTYRequestedField = value; }
        }

        /// <remarks/>
        public int QTYReceivedtoDate
        {
            get { return qTYReceivedtoDateField; }
            set { qTYReceivedtoDateField = value; }
        }

        /// <remarks/>
        public short Complete
        {
            get { return completeField; }
            set { completeField = value; }
        }

        /// <remarks/>
        public string OurPO
        {
            get { return ourPOField; }
            set { ourPOField = value; }
        }

        /// <remarks/>
        public string ClientPO
        {
            get { return clientPOField; }
            set { clientPOField = value; }
        }

        /// <remarks/>
        public string ShippingFrom
        {
            get { return shippingFromField; }
            set { shippingFromField = value; }
        }

        /// <remarks/>
        public string ShippingMethod
        {
            get { return shippingMethodField; }
            set { shippingMethodField = value; }
        }

        /// <remarks/>
        public string Comments
        {
            get { return commentsField; }
            set { commentsField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class GetProductResult : PMObject
    {

        private Product productField;

        private Offer offerField;

        /// <remarks/>
        public Product product
        {
            get { return productField; }
            set { productField = value; }
        }

        /// <remarks/>
        public Offer offer
        {
            get { return offerField; }
            set { offerField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class Product : PMObject
    {

        private PMSystem parentSystemField;

        private ProductValuation valuationField;

        private ProductAcquisition acquisitionField;

        private ProductGiftCertificate giftCertificateField;

        private ProductVersionTrack versionTrackField;

        private ProductWMSSystem[] warehouseSystemsField;

        private ProductSN serialNumberField;

        private ProductOptional optionalInfoField;

        private ProductBillFactor[] billFactorsField;

        private ProductHeader headerField;

        private ProductSort sortField;

        private ProductCharacteristics characteristicsField;

        private ProductVersion defaultVersionField;

        private ProductKit defaultKitField;

        private ProductVersion[] versionsField;

        private string mainSystemIDField;

        private ProductKit[] kitCompositionsField;

        private ProductVariable[] variablesField;

        private ProductActivation[] activationField;

        private RawMaterial[] rawMaterialsField;

        private OnOrder[] onOrderField;

        private UniversalProductCode[] universalProductCodesField;

        private ProductBillingContainers productBillingContainersField;

        /// <remarks/>
        public PMSystem ParentSystem
        {
            get { return parentSystemField; }
            set { parentSystemField = value; }
        }

        /// <remarks/>
        public ProductValuation Valuation
        {
            get { return valuationField; }
            set { valuationField = value; }
        }

        /// <remarks/>
        public ProductAcquisition Acquisition
        {
            get { return acquisitionField; }
            set { acquisitionField = value; }
        }

        /// <remarks/>
        public ProductGiftCertificate GiftCertificate
        {
            get { return giftCertificateField; }
            set { giftCertificateField = value; }
        }

        /// <remarks/>
        public ProductVersionTrack VersionTrack
        {
            get { return versionTrackField; }
            set { versionTrackField = value; }
        }

        /// <remarks/>
        public ProductWMSSystem[] WarehouseSystems
        {
            get { return warehouseSystemsField; }
            set { warehouseSystemsField = value; }
        }

        /// <remarks/>
        public ProductSN SerialNumber
        {
            get { return serialNumberField; }
            set { serialNumberField = value; }
        }

        /// <remarks/>
        public ProductOptional OptionalInfo
        {
            get { return optionalInfoField; }
            set { optionalInfoField = value; }
        }

        /// <remarks/>
        public ProductBillFactor[] BillFactors
        {
            get { return billFactorsField; }
            set { billFactorsField = value; }
        }

        /// <remarks/>
        public ProductHeader Header
        {
            get { return headerField; }
            set { headerField = value; }
        }

        /// <remarks/>
        public ProductSort Sort
        {
            get { return sortField; }
            set { sortField = value; }
        }

        /// <remarks/>
        public ProductCharacteristics Characteristics
        {
            get { return characteristicsField; }
            set { characteristicsField = value; }
        }

        /// <remarks/>
        public ProductVersion DefaultVersion
        {
            get { return defaultVersionField; }
            set { defaultVersionField = value; }
        }

        /// <remarks/>
        public ProductKit DefaultKit
        {
            get { return defaultKitField; }
            set { defaultKitField = value; }
        }

        /// <remarks/>
        public ProductVersion[] Versions
        {
            get { return versionsField; }
            set { versionsField = value; }
        }

        /// <remarks/>
        public string MainSystemID
        {
            get { return mainSystemIDField; }
            set { mainSystemIDField = value; }
        }

        /// <remarks/>
        public ProductKit[] KitCompositions
        {
            get { return kitCompositionsField; }
            set { kitCompositionsField = value; }
        }

        /// <remarks/>
        public ProductVariable[] Variables
        {
            get { return variablesField; }
            set { variablesField = value; }
        }

        /// <remarks/>
        public ProductActivation[] Activation
        {
            get { return activationField; }
            set { activationField = value; }
        }

        /// <remarks/>
        public RawMaterial[] RawMaterials
        {
            get { return rawMaterialsField; }
            set { rawMaterialsField = value; }
        }

        /// <remarks/>
        public OnOrder[] OnOrder
        {
            get { return onOrderField; }
            set { onOrderField = value; }
        }

        /// <remarks/>
        public UniversalProductCode[] UniversalProductCodes
        {
            get { return universalProductCodesField; }
            set { universalProductCodesField = value; }
        }

        /// <remarks/>
        public ProductBillingContainers ProductBillingContainers
        {
            get { return productBillingContainersField; }
            set { productBillingContainersField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class PMSystem : PMObject
    {

        private string idField;

        private bool overrideAllowHistoricalConnectionField;

        /// <remarks/>
        public string ID
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        public bool OverrideAllowHistoricalConnection
        {
            get { return overrideAllowHistoricalConnectionField; }
            set { overrideAllowHistoricalConnectionField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ProductValuation : PMObject
    {

        private bool? valuedField;

        private ReceiptValution receiptValutionField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? Valued
        {
            get { return valuedField; }
            set { valuedField = value; }
        }

        /// <remarks/>
        public ReceiptValution ReceiptValution
        {
            get { return receiptValutionField; }
            set { receiptValutionField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum ReceiptValution
    {

        /// <remarks/>
        Manual,

        /// <remarks/>
        Automatic,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ProductAcquisition : PMObject
    {

        private AcquisitionType acquisitionTypeField;

        private DateTime? acquisitionDateField;

        private string acquisitionFromField;

        private string acquisitionCommentsField;

        /// <remarks/>
        public AcquisitionType AcquisitionType
        {
            get { return acquisitionTypeField; }
            set { acquisitionTypeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? AcquisitionDate
        {
            get { return acquisitionDateField; }
            set { acquisitionDateField = value; }
        }

        /// <remarks/>
        public string AcquisitionFrom
        {
            get { return acquisitionFromField; }
            set { acquisitionFromField = value; }
        }

        /// <remarks/>
        public string AcquisitionComments
        {
            get { return acquisitionCommentsField; }
            set { acquisitionCommentsField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum AcquisitionType
    {

        /// <remarks/>
        Unknown,

        /// <remarks/>
        Make,

        /// <remarks/>
        Buy,

        /// <remarks/>
        Receive,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ProductGiftCertificate : PMObject
    {

        private bool? isGiftCertificateField;

        private System.Nullable<decimal> amountField;

        private int? expirationDaysField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? IsGiftCertificate
        {
            get { return isGiftCertificateField; }
            set { isGiftCertificateField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> Amount
        {
            get { return amountField; }
            set { amountField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? ExpirationDays
        {
            get { return expirationDaysField; }
            set { expirationDaysField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ProductVersionTrack : PMObject
    {

        private bool? versionTrackField;

        private System.Nullable<VersionSequence> versionSequenceField;

        private int? inactiveVersionDaysField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? VersionTrack
        {
            get { return versionTrackField; }
            set { versionTrackField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<VersionSequence> VersionSequence
        {
            get { return versionSequenceField; }
            set { versionSequenceField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? InactiveVersionDays
        {
            get { return inactiveVersionDaysField; }
            set { inactiveVersionDaysField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum VersionSequence
    {

        /// <remarks/>
        FirstAvailable,

        /// <remarks/>
        Manual,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ProductWMSSystem : PMObject
    {

        private int? seqIDField;

        private PMSystem systemField;

        private int? reorderPointField;

        private int? reorderAmountField;

        private string reorderCommentsField;

        private System.Nullable<CountFrequency> countFrequencyField;

        private int reservedField;

        private bool? activeField;

        private bool? deletedField;

        private bool isDefaultField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public PMSystem System
        {
            get { return systemField; }
            set { systemField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? ReorderPoint
        {
            get { return reorderPointField; }
            set { reorderPointField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? ReorderAmount
        {
            get { return reorderAmountField; }
            set { reorderAmountField = value; }
        }

        /// <remarks/>
        public string ReorderComments
        {
            get { return reorderCommentsField; }
            set { reorderCommentsField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<CountFrequency> CountFrequency
        {
            get { return countFrequencyField; }
            set { countFrequencyField = value; }
        }

        /// <remarks/>
        public int Reserved
        {
            get { return reservedField; }
            set { reservedField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? Active
        {
            get { return activeField; }
            set { activeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? Deleted
        {
            get { return deletedField; }
            set { deletedField = value; }
        }

        /// <remarks/>
        public bool IsDefault
        {
            get { return isDefaultField; }
            set { isDefaultField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum CountFrequency
    {

        /// <remarks/>
        Never,

        /// <remarks/>
        OnDemand,

        /// <remarks/>
        Daily,

        /// <remarks/>
        Weekly,

        /// <remarks/>
        BiWeekly,

        /// <remarks/>
        Monthly,

        /// <remarks/>
        BiMonthly,

        /// <remarks/>
        Quarterly,

        /// <remarks/>
        SemiAnnually,

        /// <remarks/>
        Annually,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ProductSN : PMObject
    {

        private SerialNumbers serialNumbersField;

        private bool? sNCasePacksField;

        private int? casePackQuantityField;

        /// <remarks/>
        public SerialNumbers SerialNumbers
        {
            get { return serialNumbersField; }
            set { serialNumbersField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? SNCasePacks
        {
            get { return sNCasePacksField; }
            set { sNCasePacksField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? CasePackQuantity
        {
            get { return casePackQuantityField; }
            set { casePackQuantityField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum SerialNumbers
    {

        /// <remarks/>
        NoSerialNumbers,

        /// <remarks/>
        ShippingTime,

        /// <remarks/>
        TrackedInWMS,

        /// <remarks/>
        TableValidationShipTime,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ProductOptional : PMObject
    {

        private string countryOriginField;

        private string tariffCodeField;

        private System.Nullable<decimal> customsValueField;

        private System.Nullable<decimal> valueField;

        private System.Nullable<decimal> insuranceValueField;

        private System.Nullable<decimal> defaultPriceField;

        private PriceType defaultPriceTypeField;

        private ReturnTreatment returnTreatmentField;

        private SignatureRequired signatureRequiredField;

        private string commodityDescriptionField;

        private string nMFCNoField;

        private string freightClassField;

        /// <remarks/>
        public string CountryOrigin
        {
            get { return countryOriginField; }
            set { countryOriginField = value; }
        }

        /// <remarks/>
        public string TariffCode
        {
            get { return tariffCodeField; }
            set { tariffCodeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> CustomsValue
        {
            get { return customsValueField; }
            set { customsValueField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> Value
        {
            get { return valueField; }
            set { valueField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> InsuranceValue
        {
            get { return insuranceValueField; }
            set { insuranceValueField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> DefaultPrice
        {
            get { return defaultPriceField; }
            set { defaultPriceField = value; }
        }

        /// <remarks/>
        public PriceType DefaultPriceType
        {
            get { return defaultPriceTypeField; }
            set { defaultPriceTypeField = value; }
        }

        /// <remarks/>
        public ReturnTreatment ReturnTreatment
        {
            get { return returnTreatmentField; }
            set { returnTreatmentField = value; }
        }

        /// <remarks/>
        public SignatureRequired SignatureRequired
        {
            get { return signatureRequiredField; }
            set { signatureRequiredField = value; }
        }

        /// <remarks/>
        public string CommodityDescription
        {
            get { return commodityDescriptionField; }
            set { commodityDescriptionField = value; }
        }

        /// <remarks/>
        public string NMFCNo
        {
            get { return nMFCNoField; }
            set { nMFCNoField = value; }
        }

        /// <remarks/>
        public string FreightClass
        {
            get { return freightClassField; }
            set { freightClassField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum PriceType
    {

        /// <remarks/>
        Each,

        /// <remarks/>
        PerPack,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum ReturnTreatment
    {

        /// <remarks/>
        CaseByCase,

        /// <remarks/>
        NotEligibleForReuse,

        /// <remarks/>
        MustBeEvaluated,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class SignatureRequired : PMObject
    {

        private int? seqIDField;

        private string descriptionField;

        private string codeField;

        private int? typeField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        public string Code
        {
            get { return codeField; }
            set { codeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Type
        {
            get { return typeField; }
            set { typeField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ProductBillFactor : PMObject
    {

        private int? seqIDField;

        private int? fromQuantityField;

        private int? toQuantityField;

        private System.Nullable<double> billFactorField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? FromQuantity
        {
            get { return fromQuantityField; }
            set { fromQuantityField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? ToQuantity
        {
            get { return toQuantityField; }
            set { toQuantityField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<double> BillFactor
        {
            get { return billFactorField; }
            set { billFactorField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ProductHeader : PMObject
    {

        private int? seqIDField;

        private string partNumberField;

        private string descriptionField;

        private string commentsField;

        private int? leadDaysField;

        private BuildType buildTypeField;

        private UsageCode usageCodeField;

        private bool? offerFlagField;

        private DateTime? userDateField;

        private Owner ownerField;

        private CostCenter costCenterField;

        private Job jobTemplateField;

        private System.Nullable<ProductHistoryDetailLevels> productHistoryDetailLevelField;

        private System.Nullable<CreatedBy> createdByUserTypeField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string PartNumber
        {
            get { return partNumberField; }
            set { partNumberField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        public string Comments
        {
            get { return commentsField; }
            set { commentsField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? LeadDays
        {
            get { return leadDaysField; }
            set { leadDaysField = value; }
        }

        /// <remarks/>
        public BuildType BuildType
        {
            get { return buildTypeField; }
            set { buildTypeField = value; }
        }

        /// <remarks/>
        public UsageCode UsageCode
        {
            get { return usageCodeField; }
            set { usageCodeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? OfferFlag
        {
            get { return offerFlagField; }
            set { offerFlagField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? UserDate
        {
            get { return userDateField; }
            set { userDateField = value; }
        }

        /// <remarks/>
        public Owner Owner
        {
            get { return ownerField; }
            set { ownerField = value; }
        }

        /// <remarks/>
        public CostCenter CostCenter
        {
            get { return costCenterField; }
            set { costCenterField = value; }
        }

        /// <remarks/>
        public Job JobTemplate
        {
            get { return jobTemplateField; }
            set { jobTemplateField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<ProductHistoryDetailLevels> ProductHistoryDetailLevel
        {
            get { return productHistoryDetailLevelField; }
            set { productHistoryDetailLevelField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<CreatedBy> CreatedByUserType
        {
            get { return createdByUserTypeField; }
            set { createdByUserTypeField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum BuildType
    {

        /// <remarks/>
        Product,

        /// <remarks/>
        Kit,

        /// <remarks/>
        POD,

        /// <remarks/>
        MOD,

        /// <remarks/>
        EPOD,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum UsageCode
    {

        /// <remarks/>
        ExclusiveToOwner,

        /// <remarks/>
        AllLocationsForClient,

        /// <remarks/>
        AnyClient,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class Owner : PMObject
    {

        private int? seqIDField;

        private SortGroupType sortGroupTypeField;

        private string idField;

        private string companyNameField;

        private BillingCode billCodeField;

        private System.Nullable<ProductHistoryTransactionTypes> productHistoryTransactionTypeField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public SortGroupType SortGroupType
        {
            get { return sortGroupTypeField; }
            set { sortGroupTypeField = value; }
        }

        /// <remarks/>
        public string ID
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        public string CompanyName
        {
            get { return companyNameField; }
            set { companyNameField = value; }
        }

        /// <remarks/>
        public BillingCode BillCode
        {
            get { return billCodeField; }
            set { billCodeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<ProductHistoryTransactionTypes> ProductHistoryTransactionType
        {
            get { return productHistoryTransactionTypeField; }
            set { productHistoryTransactionTypeField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class SortGroupType : PMObject
    {

        private string descriptionField;

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class BillingCode : PMObject
    {

        private int? seqIDField;

        private string descriptionField;

        private DateTime? closingDateField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? ClosingDate
        {
            get { return closingDateField; }
            set { closingDateField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum ProductHistoryTransactionTypes
    {

        /// <remarks/>
        OnePerTransaction,

        /// <remarks/>
        OnePerDay,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class CostCenter : PMObject
    {

        private int? seqIDField;

        private string idField;

        private string descriptionField;

        private int? sequenceField;

        private bool removableField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string ID
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Sequence
        {
            get { return sequenceField; }
            set { sequenceField = value; }
        }

        /// <remarks/>
        public bool Removable
        {
            get { return removableField; }
            set { removableField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class Job : PMObject
    {

        private int? seqIDField;

        private int? pROJCT_SeqidField;

        private string descriptionField;

        private string instructionsField;

        private DateTime? orderDateField;

        private DateTime? readyDateField;

        private System.Nullable<short> releaseSoonerField;

        private System.Nullable<short> readyStatusField;

        private string jobStatusField;

        private string workStatusField;

        private DateTime? mailDateField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? PROJCT_Seqid
        {
            get { return pROJCT_SeqidField; }
            set { pROJCT_SeqidField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        public string Instructions
        {
            get { return instructionsField; }
            set { instructionsField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? OrderDate
        {
            get { return orderDateField; }
            set { orderDateField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? ReadyDate
        {
            get { return readyDateField; }
            set { readyDateField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<short> ReleaseSooner
        {
            get { return releaseSoonerField; }
            set { releaseSoonerField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<short> ReadyStatus
        {
            get { return readyStatusField; }
            set { readyStatusField = value; }
        }

        /// <remarks/>
        public string JobStatus
        {
            get { return jobStatusField; }
            set { jobStatusField = value; }
        }

        /// <remarks/>
        public string WorkStatus
        {
            get { return workStatusField; }
            set { workStatusField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? MailDate
        {
            get { return mailDateField; }
            set { mailDateField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum ProductHistoryDetailLevels
    {

        /// <remarks/>
        OnePerTransaction,

        /// <remarks/>
        OnePerTransactionTypePerDay,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum CreatedBy
    {

        /// <remarks/>
        WebService,

        /// <remarks/>
        Client,

        /// <remarks/>
        NonClient,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ProductSort : PMObject
    {

        private ProductType productTypeField;

        private ProductSortGroup sortGroup1Field;

        private ProductSortGroup sortGroup2Field;

        private ProductSortGroup sortGroup3Field;

        private ProductSortGroup sortGroup4Field;

        /// <remarks/>
        public ProductType ProductType
        {
            get { return productTypeField; }
            set { productTypeField = value; }
        }

        /// <remarks/>
        public ProductSortGroup SortGroup1
        {
            get { return sortGroup1Field; }
            set { sortGroup1Field = value; }
        }

        /// <remarks/>
        public ProductSortGroup SortGroup2
        {
            get { return sortGroup2Field; }
            set { sortGroup2Field = value; }
        }

        /// <remarks/>
        public ProductSortGroup SortGroup3
        {
            get { return sortGroup3Field; }
            set { sortGroup3Field = value; }
        }

        /// <remarks/>
        public ProductSortGroup SortGroup4
        {
            get { return sortGroup4Field; }
            set { sortGroup4Field = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ProductType : PMObject
    {

        private int? seqIDField;

        private string descriptionField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ProductSortGroup : PMObject
    {

        private int? seqIDField;

        private string descriptionField;

        private ProductSortLevel sortLevelField;

        private string sortKeyField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        public ProductSortLevel SortLevel
        {
            get { return sortLevelField; }
            set { sortLevelField = value; }
        }

        /// <remarks/>
        public string SortKey
        {
            get { return sortKeyField; }
            set { sortKeyField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ProductSortLevel : PMObject
    {

        private int? seqIDField;

        private string descriptionField;

        private int? orderField;

        private SortGroupType sortGroupTypeField;

        private bool? isActiveField;

        private bool? isEnabledField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Order
        {
            get { return orderField; }
            set { orderField = value; }
        }

        /// <remarks/>
        public SortGroupType SortGroupType
        {
            get { return sortGroupTypeField; }
            set { sortGroupTypeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? IsActive
        {
            get { return isActiveField; }
            set { isActiveField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? IsEnabled
        {
            get { return isEnabledField; }
            set { isEnabledField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ProductCharacteristics : PMObject
    {

        private System.Nullable<double> heightField;

        private System.Nullable<double> widthField;

        private System.Nullable<double> lengthField;

        private System.Nullable<double> defaultWeightField;

        private System.Nullable<WeightType> defaultWeightTypeField;

        private string packDescriptionField;

        private Color colorField;

        private Size sizeField;

        private bool? prePackField;

        private int? packQuantityField;

        private System.Nullable<PackTrack> packTrackField;

        private bool? shipSeparatePackagesField;

        private bool? imageLocalField;

        private string imageDirectoryField;

        private string imagePathThumbnailField;

        private string imagePathFullField;

        private int? clusterSequenceField;

        private System.Nullable<double> pRDUCT_SpoilagePercentField;

        private int? pRDUCT_SpoilageRoundingQuantityField;

        private System.Nullable<double> pPCubicFootField;

        private System.Nullable<double> cubicFeetPPField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<double> Height
        {
            get { return heightField; }
            set { heightField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<double> Width
        {
            get { return widthField; }
            set { widthField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<double> Length
        {
            get { return lengthField; }
            set { lengthField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<double> DefaultWeight
        {
            get { return defaultWeightField; }
            set { defaultWeightField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<WeightType> DefaultWeightType
        {
            get { return defaultWeightTypeField; }
            set { defaultWeightTypeField = value; }
        }

        /// <remarks/>
        public string PackDescription
        {
            get { return packDescriptionField; }
            set { packDescriptionField = value; }
        }

        /// <remarks/>
        public Color Color
        {
            get { return colorField; }
            set { colorField = value; }
        }

        /// <remarks/>
        public Size Size
        {
            get { return sizeField; }
            set { sizeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? PrePack
        {
            get { return prePackField; }
            set { prePackField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? PackQuantity
        {
            get { return packQuantityField; }
            set { packQuantityField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<PackTrack> PackTrack
        {
            get { return packTrackField; }
            set { packTrackField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? ShipSeparatePackages
        {
            get { return shipSeparatePackagesField; }
            set { shipSeparatePackagesField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? ImageLocal
        {
            get { return imageLocalField; }
            set { imageLocalField = value; }
        }

        /// <remarks/>
        public string ImageDirectory
        {
            get { return imageDirectoryField; }
            set { imageDirectoryField = value; }
        }

        /// <remarks/>
        public string ImagePathThumbnail
        {
            get { return imagePathThumbnailField; }
            set { imagePathThumbnailField = value; }
        }

        /// <remarks/>
        public string ImagePathFull
        {
            get { return imagePathFullField; }
            set { imagePathFullField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? ClusterSequence
        {
            get { return clusterSequenceField; }
            set { clusterSequenceField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<double> PRDUCT_SpoilagePercent
        {
            get { return pRDUCT_SpoilagePercentField; }
            set { pRDUCT_SpoilagePercentField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? PRDUCT_SpoilageRoundingQuantity
        {
            get { return pRDUCT_SpoilageRoundingQuantityField; }
            set { pRDUCT_SpoilageRoundingQuantityField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<double> PPCubicFoot
        {
            get { return pPCubicFootField; }
            set { pPCubicFootField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<double> CubicFeetPP
        {
            get { return cubicFeetPPField; }
            set { cubicFeetPPField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum WeightType
    {

        /// <remarks/>
        Oz,

        /// <remarks/>
        Lbs,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class Color : PMObject
    {

        private int? seqIDField;

        private ColorClass colorClassField;

        private string codeField;

        private string descriptionField;

        private string imagePathField;

        private bool? imageLocalField;

        private int? sequenceField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public ColorClass ColorClass
        {
            get { return colorClassField; }
            set { colorClassField = value; }
        }

        /// <remarks/>
        public string Code
        {
            get { return codeField; }
            set { codeField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        public string ImagePath
        {
            get { return imagePathField; }
            set { imagePathField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? ImageLocal
        {
            get { return imageLocalField; }
            set { imageLocalField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Sequence
        {
            get { return sequenceField; }
            set { sequenceField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ColorClass : PMObject
    {

        private int? seqIDField;

        private string descriptionField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class Size : PMObject
    {

        private int? seqIDField;

        private string descriptionField;

        private string codeField;

        private SizeClass sizeClassField;

        private int? sequenceField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        public string Code
        {
            get { return codeField; }
            set { codeField = value; }
        }

        /// <remarks/>
        public SizeClass SizeClass
        {
            get { return sizeClassField; }
            set { sizeClassField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Sequence
        {
            get { return sequenceField; }
            set { sequenceField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class SizeClass : PMObject
    {

        private int? seqIDField;

        private string descriptionField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum PackTrack
    {

        /// <remarks/>
        Each,

        /// <remarks/>
        Containers,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ProductVersion : PMObject
    {

        private int? seqIDField;

        private ProductID productField;

        private string uIDField;

        private string dateField;

        private string commentsField;

        private DateTime? startDateField;

        private DateTime? endDateField;

        private System.Nullable<double> weightField;

        private System.Nullable<WeightType> weightTypeField;

        private string versionField;

        private VersionStatus statusField;

        private VersionDisposition dispositionField;

        private int? orderField;

        private bool hasWarehouseTransactionsField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public ProductID Product
        {
            get { return productField; }
            set { productField = value; }
        }

        /// <remarks/>
        public string UID
        {
            get { return uIDField; }
            set { uIDField = value; }
        }

        /// <remarks/>
        public string Date
        {
            get { return dateField; }
            set { dateField = value; }
        }

        /// <remarks/>
        public string Comments
        {
            get { return commentsField; }
            set { commentsField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? StartDate
        {
            get { return startDateField; }
            set { startDateField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? EndDate
        {
            get { return endDateField; }
            set { endDateField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<double> Weight
        {
            get { return weightField; }
            set { weightField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<WeightType> WeightType
        {
            get { return weightTypeField; }
            set { weightTypeField = value; }
        }

        /// <remarks/>
        public string Version
        {
            get { return versionField; }
            set { versionField = value; }
        }

        /// <remarks/>
        public VersionStatus Status
        {
            get { return statusField; }
            set { statusField = value; }
        }

        /// <remarks/>
        public VersionDisposition Disposition
        {
            get { return dispositionField; }
            set { dispositionField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Order
        {
            get { return orderField; }
            set { orderField = value; }
        }

        /// <remarks/>
        public bool HasWarehouseTransactions
        {
            get { return hasWarehouseTransactionsField; }
            set { hasWarehouseTransactionsField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ProductID
    {

        private ProductIDHeader headerField;

        private ProductIDSort sortField;

        /// <remarks/>
        public ProductIDHeader Header
        {
            get { return headerField; }
            set { headerField = value; }
        }

        /// <remarks/>
        public ProductIDSort Sort
        {
            get { return sortField; }
            set { sortField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ProductIDHeader : PMObject
    {

        private int? seqIDField;

        private string partNumberField;

        private Owner ownerField;

        private string descriptionField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string PartNumber
        {
            get { return partNumberField; }
            set { partNumberField = value; }
        }

        /// <remarks/>
        public Owner Owner
        {
            get { return ownerField; }
            set { ownerField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ProductIDSort : PMObject
    {

        private ProductType productTypeField;

        /// <remarks/>
        public ProductType ProductType
        {
            get { return productTypeField; }
            set { productTypeField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class VersionStatus : PMObject
    {

        private int? seqIDField;

        private string descriptionField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class VersionDisposition : PMObject
    {

        private int? seqIDField;

        private string descriptionField;

        private bool? permanentField;

        private int? sequenceField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? Permanent
        {
            get { return permanentField; }
            set { permanentField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Sequence
        {
            get { return sequenceField; }
            set { sequenceField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ProductKit : PMObject
    {

        private string compositionField;

        private DateTime? startDateField;

        private DateTime? endDateField;

        private System.Nullable<double> weightField;

        private System.Nullable<WeightType> weightTypeField;

        private VersionStatus statusField;

        private bool? currentField;

        private KitComponent[] componentsField;

        private bool? usedField;

        /// <remarks/>
        public string Composition
        {
            get { return compositionField; }
            set { compositionField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? StartDate
        {
            get { return startDateField; }
            set { startDateField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? EndDate
        {
            get { return endDateField; }
            set { endDateField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<double> Weight
        {
            get { return weightField; }
            set { weightField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<WeightType> WeightType
        {
            get { return weightTypeField; }
            set { weightTypeField = value; }
        }

        /// <remarks/>
        public VersionStatus Status
        {
            get { return statusField; }
            set { statusField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? Current
        {
            get { return currentField; }
            set { currentField = value; }
        }

        /// <remarks/>
        public KitComponent[] Components
        {
            get { return componentsField; }
            set { componentsField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? Used
        {
            get { return usedField; }
            set { usedField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class KitComponent : PMObject
    {

        private Product productField;

        private int? quantityField;

        private string instructionsField;

        private int? sequenceField;

        private ComponentTreatment importanceField;

        /// <remarks/>
        public Product Product
        {
            get { return productField; }
            set { productField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Quantity
        {
            get { return quantityField; }
            set { quantityField = value; }
        }

        /// <remarks/>
        public string Instructions
        {
            get { return instructionsField; }
            set { instructionsField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Sequence
        {
            get { return sequenceField; }
            set { sequenceField = value; }
        }

        /// <remarks/>
        public ComponentTreatment Importance
        {
            get { return importanceField; }
            set { importanceField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum ComponentTreatment
    {

        /// <remarks/>
        Essential,

        /// <remarks/>
        NonEssential,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ProductVariable : PMObject
    {

        private int? seqIDField;

        private VariableField variableFieldField;

        private string valueField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public VariableField VariableField
        {
            get { return variableFieldField; }
            set { variableFieldField = value; }
        }

        /// <remarks/>
        public string Value
        {
            get { return valueField; }
            set { valueField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class VariableField : PMObject
    {

        private int? seqIDField;

        private string fieldNameField;

        private UserDefinedChoice userDefinedChoiceField;

        private string userDefinedChoiceNameField;

        private ExtensionUpload[] extensionUploadsField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string FieldName
        {
            get { return fieldNameField; }
            set { fieldNameField = value; }
        }

        /// <remarks/>
        public UserDefinedChoice UserDefinedChoice
        {
            get { return userDefinedChoiceField; }
            set { userDefinedChoiceField = value; }
        }

        /// <remarks/>
        public string UserDefinedChoiceName
        {
            get { return userDefinedChoiceNameField; }
            set { userDefinedChoiceNameField = value; }
        }

        /// <remarks/>
        public ExtensionUpload[] ExtensionUploads
        {
            get { return extensionUploadsField; }
            set { extensionUploadsField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class UserDefinedChoice : PMObject
    {

        private string nameField;

        /// <remarks/>
        public string Name
        {
            get { return nameField; }
            set { nameField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ExtensionUpload : PMObject
    {

        private int? seqIDField;

        private int? variableFieldField;

        private string fileExtensionField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? VariableField
        {
            get { return variableFieldField; }
            set { variableFieldField = value; }
        }

        /// <remarks/>
        public string FileExtension
        {
            get { return fileExtensionField; }
            set { fileExtensionField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ProductActivation : PMObject
    {

        private int? seqIDField;

        private OMSSystem oMSSystemField;

        private ProductCluster clusterField;

        private bool? activeField;

        private bool? discontinuedField;

        private Product replacementProductField;

        private bool? recallSOField;

        private bool? offerTreatField;

        private bool? kitTreatField;

        private int? bundleQuantityField;

        private System.Nullable<decimal> packChargeField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public OMSSystem OMSSystem
        {
            get { return oMSSystemField; }
            set { oMSSystemField = value; }
        }

        /// <remarks/>
        public ProductCluster Cluster
        {
            get { return clusterField; }
            set { clusterField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? Active
        {
            get { return activeField; }
            set { activeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? Discontinued
        {
            get { return discontinuedField; }
            set { discontinuedField = value; }
        }

        /// <remarks/>
        public Product ReplacementProduct
        {
            get { return replacementProductField; }
            set { replacementProductField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? RecallSO
        {
            get { return recallSOField; }
            set { recallSOField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? OfferTreat
        {
            get { return offerTreatField; }
            set { offerTreatField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? KitTreat
        {
            get { return kitTreatField; }
            set { kitTreatField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? BundleQuantity
        {
            get { return bundleQuantityField; }
            set { bundleQuantityField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> PackCharge
        {
            get { return packChargeField; }
            set { packChargeField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OMSSystem : PMObject
    {

        private int? seqIDField;

        private PMSystem systemField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public PMSystem System
        {
            get { return systemField; }
            set { systemField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class RawMaterial : PMObject
    {

        private int? seqIDField;

        private ProductID componentField;

        private int quantityField;

        private TreatmentType treatmentField;

        private string instructionsField;

        private int? sequenceField;

        private bool wasModifiedField;

        private bool needsRemovalField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public ProductID Component
        {
            get { return componentField; }
            set { componentField = value; }
        }

        /// <remarks/>
        public int Quantity
        {
            get { return quantityField; }
            set { quantityField = value; }
        }

        /// <remarks/>
        public TreatmentType Treatment
        {
            get { return treatmentField; }
            set { treatmentField = value; }
        }

        /// <remarks/>
        public string Instructions
        {
            get { return instructionsField; }
            set { instructionsField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Sequence
        {
            get { return sequenceField; }
            set { sequenceField = value; }
        }

        /// <remarks/>
        public bool WasModified
        {
            get { return wasModifiedField; }
            set { wasModifiedField = value; }
        }

        /// <remarks/>
        public bool NeedsRemoval
        {
            get { return needsRemovalField; }
            set { needsRemovalField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum TreatmentType
    {

        /// <remarks/>
        Essential,

        /// <remarks/>
        NonEssential,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OnOrder : PMObject
    {

        private int? onOrderSeqIDField;

        private PurchaseOrder purchaseOrderField;

        private int? onOrderProductSeqIDField;

        private PurchaseOrderProduct purchaseOrderProductField;

        private string referenceNumberField;

        private string purchaseOrderNumberField;

        private DateTime? dateRecordedField;

        private DateTime? approximateReceiveDateField;

        private string fromWhereField;

        private string commentsField;

        private System.Nullable<OfferPriceType> priceTypeField;

        private int quantityField;

        private decimal unitPriceField;

        private decimal extendedPriceField;

        private int quantityReceivedField;

        private DateTime? uTCDateRecordedField;

        private bool toDeleteField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? OnOrderSeqID
        {
            get { return onOrderSeqIDField; }
            set { onOrderSeqIDField = value; }
        }

        /// <remarks/>
        public PurchaseOrder PurchaseOrder
        {
            get { return purchaseOrderField; }
            set { purchaseOrderField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? OnOrderProductSeqID
        {
            get { return onOrderProductSeqIDField; }
            set { onOrderProductSeqIDField = value; }
        }

        /// <remarks/>
        public PurchaseOrderProduct PurchaseOrderProduct
        {
            get { return purchaseOrderProductField; }
            set { purchaseOrderProductField = value; }
        }

        /// <remarks/>
        public string ReferenceNumber
        {
            get { return referenceNumberField; }
            set { referenceNumberField = value; }
        }

        /// <remarks/>
        public string PurchaseOrderNumber
        {
            get { return purchaseOrderNumberField; }
            set { purchaseOrderNumberField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? DateRecorded
        {
            get { return dateRecordedField; }
            set { dateRecordedField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? ApproximateReceiveDate
        {
            get { return approximateReceiveDateField; }
            set { approximateReceiveDateField = value; }
        }

        /// <remarks/>
        public string FromWhere
        {
            get { return fromWhereField; }
            set { fromWhereField = value; }
        }

        /// <remarks/>
        public string Comments
        {
            get { return commentsField; }
            set { commentsField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<OfferPriceType> PriceType
        {
            get { return priceTypeField; }
            set { priceTypeField = value; }
        }

        /// <remarks/>
        public int Quantity
        {
            get { return quantityField; }
            set { quantityField = value; }
        }

        /// <remarks/>
        public decimal UnitPrice
        {
            get { return unitPriceField; }
            set { unitPriceField = value; }
        }

        /// <remarks/>
        public decimal ExtendedPrice
        {
            get { return extendedPriceField; }
            set { extendedPriceField = value; }
        }

        /// <remarks/>
        public int QuantityReceived
        {
            get { return quantityReceivedField; }
            set { quantityReceivedField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? UTCDateRecorded
        {
            get { return uTCDateRecordedField; }
            set { uTCDateRecordedField = value; }
        }

        /// <remarks/>
        public bool ToDelete
        {
            get { return toDeleteField; }
            set { toDeleteField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class PurchaseOrder : PMObject
    {

        private int? seqIDField;

        private string referenceNumberField;

        private Owner ownerField;

        private string pONumberField;

        private DateTime? dateRecordedField;

        private DateTime? approximateArrivalDateField;

        private string fromWhereField;

        private string commentsField;

        private int? jobField;

        private int? projectField;

        private int? jobPackageField;

        private Vendor vendorField;

        private PMSystem systemField;

        private string shipToField;

        private string address1Field;

        private string address2Field;

        private string address3Field;

        private string cityField;

        private string stateField;

        private string postalCodeField;

        private string countryField;

        private DateTime? uTCDateRecordedField;

        private PurchaseOrderProduct[] productsField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string ReferenceNumber
        {
            get { return referenceNumberField; }
            set { referenceNumberField = value; }
        }

        /// <remarks/>
        public Owner Owner
        {
            get { return ownerField; }
            set { ownerField = value; }
        }

        /// <remarks/>
        public string PONumber
        {
            get { return pONumberField; }
            set { pONumberField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? DateRecorded
        {
            get { return dateRecordedField; }
            set { dateRecordedField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? ApproximateArrivalDate
        {
            get { return approximateArrivalDateField; }
            set { approximateArrivalDateField = value; }
        }

        /// <remarks/>
        public string FromWhere
        {
            get { return fromWhereField; }
            set { fromWhereField = value; }
        }

        /// <remarks/>
        public string Comments
        {
            get { return commentsField; }
            set { commentsField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Job
        {
            get { return jobField; }
            set { jobField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Project
        {
            get { return projectField; }
            set { projectField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? JobPackage
        {
            get { return jobPackageField; }
            set { jobPackageField = value; }
        }

        /// <remarks/>
        public Vendor Vendor
        {
            get { return vendorField; }
            set { vendorField = value; }
        }

        /// <remarks/>
        public PMSystem System
        {
            get { return systemField; }
            set { systemField = value; }
        }

        /// <remarks/>
        public string ShipTo
        {
            get { return shipToField; }
            set { shipToField = value; }
        }

        /// <remarks/>
        public string Address1
        {
            get { return address1Field; }
            set { address1Field = value; }
        }

        /// <remarks/>
        public string Address2
        {
            get { return address2Field; }
            set { address2Field = value; }
        }

        /// <remarks/>
        public string Address3
        {
            get { return address3Field; }
            set { address3Field = value; }
        }

        /// <remarks/>
        public string City
        {
            get { return cityField; }
            set { cityField = value; }
        }

        /// <remarks/>
        public string State
        {
            get { return stateField; }
            set { stateField = value; }
        }

        /// <remarks/>
        public string PostalCode
        {
            get { return postalCodeField; }
            set { postalCodeField = value; }
        }

        /// <remarks/>
        public string Country
        {
            get { return countryField; }
            set { countryField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? UTCDateRecorded
        {
            get { return uTCDateRecordedField; }
            set { uTCDateRecordedField = value; }
        }

        /// <remarks/>
        public PurchaseOrderProduct[] Products
        {
            get { return productsField; }
            set { productsField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class Vendor : PMObject
    {

        private int? seqIDField;

        private string idField;

        private string nameField;

        private string contactField;

        private string address1Field;

        private string address2Field;

        private string address3Field;

        private string cityField;

        private string stateField;

        private string postalCodeField;

        private string countryField;

        private string emailField;

        private System.Nullable<VendorStatus> statusField;

        private bool? holdField;

        private string commentsField;

        private OMSSystem oMSSystemField;

        private string phoneField;

        private string faxField;

        private System.Nullable<decimal> minimumPurchaseAmountField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string ID
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        public string Name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks/>
        public string Contact
        {
            get { return contactField; }
            set { contactField = value; }
        }

        /// <remarks/>
        public string Address1
        {
            get { return address1Field; }
            set { address1Field = value; }
        }

        /// <remarks/>
        public string Address2
        {
            get { return address2Field; }
            set { address2Field = value; }
        }

        /// <remarks/>
        public string Address3
        {
            get { return address3Field; }
            set { address3Field = value; }
        }

        /// <remarks/>
        public string City
        {
            get { return cityField; }
            set { cityField = value; }
        }

        /// <remarks/>
        public string State
        {
            get { return stateField; }
            set { stateField = value; }
        }

        /// <remarks/>
        public string PostalCode
        {
            get { return postalCodeField; }
            set { postalCodeField = value; }
        }

        /// <remarks/>
        public string Country
        {
            get { return countryField; }
            set { countryField = value; }
        }

        /// <remarks/>
        public string Email
        {
            get { return emailField; }
            set { emailField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<VendorStatus> Status
        {
            get { return statusField; }
            set { statusField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? Hold
        {
            get { return holdField; }
            set { holdField = value; }
        }

        /// <remarks/>
        public string Comments
        {
            get { return commentsField; }
            set { commentsField = value; }
        }

        /// <remarks/>
        public OMSSystem OMSSystem
        {
            get { return oMSSystemField; }
            set { oMSSystemField = value; }
        }

        /// <remarks/>
        public string Phone
        {
            get { return phoneField; }
            set { phoneField = value; }
        }

        /// <remarks/>
        public string Fax
        {
            get { return faxField; }
            set { faxField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> MinimumPurchaseAmount
        {
            get { return minimumPurchaseAmountField; }
            set { minimumPurchaseAmountField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum VendorStatus
    {

        /// <remarks/>
        Active,

        /// <remarks/>
        Inactive,

        /// <remarks/>
        Historical,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class PurchaseOrderProduct : PMObject
    {

        private int? seqIDField;

        private ProductID productField;

        private int? quantityField;

        private OfferPriceType priceTypeField;

        private System.Nullable<decimal> valuationPriceField;

        private System.Nullable<decimal> extendedValuationPriceField;

        private string pOLineNumberField;

        private int? quantityReceivedField;

        private int? completeField;

        private int? purchaseOrderDetailsField;

        private System.Nullable<decimal> unitPriceField;

        private System.Nullable<decimal> extendedPriceField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public ProductID Product
        {
            get { return productField; }
            set { productField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Quantity
        {
            get { return quantityField; }
            set { quantityField = value; }
        }

        /// <remarks/>
        public OfferPriceType PriceType
        {
            get { return priceTypeField; }
            set { priceTypeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> ValuationPrice
        {
            get { return valuationPriceField; }
            set { valuationPriceField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> ExtendedValuationPrice
        {
            get { return extendedValuationPriceField; }
            set { extendedValuationPriceField = value; }
        }

        /// <remarks/>
        public string POLineNumber
        {
            get { return pOLineNumberField; }
            set { pOLineNumberField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? QuantityReceived
        {
            get { return quantityReceivedField; }
            set { quantityReceivedField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Complete
        {
            get { return completeField; }
            set { completeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? PurchaseOrderDetails
        {
            get { return purchaseOrderDetailsField; }
            set { purchaseOrderDetailsField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> UnitPrice
        {
            get { return unitPriceField; }
            set { unitPriceField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> ExtendedPrice
        {
            get { return extendedPriceField; }
            set { extendedPriceField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum OfferPriceType
    {

        /// <remarks/>
        Each,

        /// <remarks/>
        PerThousand,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class UniversalProductCode : PMObject
    {

        private int? seqIDField;

        private Product productField;

        private string codeField;

        private int? unitQuantityField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public Product Product
        {
            get { return productField; }
            set { productField = value; }
        }

        /// <remarks/>
        public string Code
        {
            get { return codeField; }
            set { codeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? UnitQuantity
        {
            get { return unitQuantityField; }
            set { unitQuantityField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ProductBillingContainers : PMObject
    {

        private int? skidQuantityField;

        private int? masterCartonQuantityField;

        private int? innerCartonQuantityField;

        private int? outerSleeveQuantityField;

        private int? innerSleeveQuantityField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SkidQuantity
        {
            get { return skidQuantityField; }
            set { skidQuantityField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? MasterCartonQuantity
        {
            get { return masterCartonQuantityField; }
            set { masterCartonQuantityField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? InnerCartonQuantity
        {
            get { return innerCartonQuantityField; }
            set { innerCartonQuantityField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? OuterSleeveQuantity
        {
            get { return outerSleeveQuantityField; }
            set { outerSleeveQuantityField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? InnerSleeveQuantity
        {
            get { return innerSleeveQuantityField; }
            set { innerSleeveQuantityField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class Offer : PMObject
    {

        private OfferHeader headerField;

        private OfferInfo infoField;

        private OfferStatus statusField;

        private OfferSettings settingsField;

        private OfferStrings stringsField;

        private OfferPricing pricingField;

        private OfferRecurrence recurrenceField;

        private OfferCustomization customizationField;

        private OfferLinks linksField;

        private OfferImages imagesField;

        private OfferCategorization categorizationField;

        private OfferDropShip dropShipField;

        private EDelivery eDeliveryField;

        private OfferRestriction[] restrictionsField;

        private OfferComponent[] componentsField;

        private SupplierPart[] dropShipPartsField;

        private OfferCategory[] categoriesField;

        private Upsell[] upsellsField;

        private ClusterSurcharge[] clusterSurchargesField;

        private OfferScheduleParameters[] scheduleParametersField;

        private OfferCycleBillOfMaterials[] cycleBillOfMaterialsField;

        private ClusterSurcharge[] customAssemblySurchargesField;

        /// <remarks/>
        public OfferHeader Header
        {
            get { return headerField; }
            set { headerField = value; }
        }

        /// <remarks/>
        public OfferInfo Info
        {
            get { return infoField; }
            set { infoField = value; }
        }

        /// <remarks/>
        public OfferStatus Status
        {
            get { return statusField; }
            set { statusField = value; }
        }

        /// <remarks/>
        public OfferSettings Settings
        {
            get { return settingsField; }
            set { settingsField = value; }
        }

        /// <remarks/>
        public OfferStrings Strings
        {
            get { return stringsField; }
            set { stringsField = value; }
        }

        /// <remarks/>
        public OfferPricing Pricing
        {
            get { return pricingField; }
            set { pricingField = value; }
        }

        /// <remarks/>
        public OfferRecurrence Recurrence
        {
            get { return recurrenceField; }
            set { recurrenceField = value; }
        }

        /// <remarks/>
        public OfferCustomization Customization
        {
            get { return customizationField; }
            set { customizationField = value; }
        }

        /// <remarks/>
        public OfferLinks Links
        {
            get { return linksField; }
            set { linksField = value; }
        }

        /// <remarks/>
        public OfferImages Images
        {
            get { return imagesField; }
            set { imagesField = value; }
        }

        /// <remarks/>
        public OfferCategorization Categorization
        {
            get { return categorizationField; }
            set { categorizationField = value; }
        }

        /// <remarks/>
        public OfferDropShip DropShip
        {
            get { return dropShipField; }
            set { dropShipField = value; }
        }

        /// <remarks/>
        public EDelivery EDelivery
        {
            get { return eDeliveryField; }
            set { eDeliveryField = value; }
        }

        /// <remarks/>
        public OfferRestriction[] Restrictions
        {
            get { return restrictionsField; }
            set { restrictionsField = value; }
        }

        /// <remarks/>
        public OfferComponent[] Components
        {
            get { return componentsField; }
            set { componentsField = value; }
        }

        /// <remarks/>
        public SupplierPart[] DropShipParts
        {
            get { return dropShipPartsField; }
            set { dropShipPartsField = value; }
        }

        /// <remarks/>
        public OfferCategory[] Categories
        {
            get { return categoriesField; }
            set { categoriesField = value; }
        }

        /// <remarks/>
        public Upsell[] Upsells
        {
            get { return upsellsField; }
            set { upsellsField = value; }
        }

        /// <remarks/>
        public ClusterSurcharge[] ClusterSurcharges
        {
            get { return clusterSurchargesField; }
            set { clusterSurchargesField = value; }
        }

        /// <remarks/>
        public OfferScheduleParameters[] ScheduleParameters
        {
            get { return scheduleParametersField; }
            set { scheduleParametersField = value; }
        }

        /// <remarks/>
        public OfferCycleBillOfMaterials[] CycleBillOfMaterials
        {
            get { return cycleBillOfMaterialsField; }
            set { cycleBillOfMaterialsField = value; }
        }

        /// <remarks/>
        public ClusterSurcharge[] CustomAssemblySurcharges
        {
            get { return customAssemblySurchargesField; }
            set { customAssemblySurchargesField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OfferHeader : PMObject
    {

        private int? seqIDField;

        private string idField;

        private string descriptionField;

        private string sortKeyField;

        private DateTime? deletedDateTimeField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string ID
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        public string SortKey
        {
            get { return sortKeyField; }
            set { sortKeyField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? DeletedDateTime
        {
            get { return deletedDateTimeField; }
            set { deletedDateTimeField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OfferInfo : PMObject
    {

        private System.Nullable<BillOfMaterials> billOfMaterialsField;

        private bool? customAssemblyField;

        private System.Nullable<DropShipType> dropShipField;

        private RevenueCenter revenueCenterField;

        private bool? productImagesField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<BillOfMaterials> BillOfMaterials
        {
            get { return billOfMaterialsField; }
            set { billOfMaterialsField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? CustomAssembly
        {
            get { return customAssemblyField; }
            set { customAssemblyField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<DropShipType> DropShip
        {
            get { return dropShipField; }
            set { dropShipField = value; }
        }

        /// <remarks/>
        public RevenueCenter RevenueCenter
        {
            get { return revenueCenterField; }
            set { revenueCenterField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? ProductImages
        {
            get { return productImagesField; }
            set { productImagesField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum BillOfMaterials
    {

        /// <remarks/>
        ProductList,

        /// <remarks/>
        CustomAssembly,

        /// <remarks/>
        DropShip,

        /// <remarks/>
        EDelivery,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum DropShipType
    {

        /// <remarks/>
        DropShip,

        /// <remarks/>
        EDelivery,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class RevenueCenter : PMObject
    {

        private int? seqIDField;

        private string idField;

        private string descriptionField;

        private GLCode gLCodeField;

        private bool? isDefaultField;

        private int? sequenceField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string ID
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        public GLCode GLCode
        {
            get { return gLCodeField; }
            set { gLCodeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? IsDefault
        {
            get { return isDefaultField; }
            set { isDefaultField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Sequence
        {
            get { return sequenceField; }
            set { sequenceField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class GLCode : PMObject
    {

        private int? seqIDField;

        private string idField;

        private string descriptionField;

        private GLCodeType typeField;

        private bool removableField;

        private bool needsRemovalField;

        private bool wasModifiedField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string ID
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        public GLCodeType Type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        public bool Removable
        {
            get { return removableField; }
            set { removableField = value; }
        }

        /// <remarks/>
        public bool NeedsRemoval
        {
            get { return needsRemovalField; }
            set { needsRemovalField = value; }
        }

        /// <remarks/>
        public bool WasModified
        {
            get { return wasModifiedField; }
            set { wasModifiedField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum GLCodeType
    {

        /// <remarks/>
        Sales,

        /// <remarks/>
        Cash,

        /// <remarks/>
        OrderDiscounts,

        /// <remarks/>
        PaymentDiscounts,

        /// <remarks/>
        AR,

        /// <remarks/>
        Tax,

        /// <remarks/>
        Credits,

        /// <remarks/>
        ShippingHandling,

        /// <remarks/>
        GiftCertificatesRedeemed,

        /// <remarks/>
        RushCharge,

        /// <remarks/>
        OfferShippingHandling,

        /// <remarks/>
        PackageCharge,

        /// <remarks/>
        BillBack,

        /// <remarks/>
        SpecialHandling,

        /// <remarks/>
        NoChargeDiscounts,

        /// <remarks/>
        OverUnderPayments,

        /// <remarks/>
        OrderReturns,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OfferStatus : PMObject
    {

        private bool? inactiveField;

        private DateTime? startDateField;

        private DateTime? endDateField;

        private string inactiveTextField;

        private DateTime? displayUntilField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? Inactive
        {
            get { return inactiveField; }
            set { inactiveField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? StartDate
        {
            get { return startDateField; }
            set { startDateField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? EndDate
        {
            get { return endDateField; }
            set { endDateField = value; }
        }

        /// <remarks/>
        public string InactiveText
        {
            get { return inactiveTextField; }
            set { inactiveTextField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? DisplayUntil
        {
            get { return displayUntilField; }
            set { displayUntilField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OfferSettings : PMObject
    {

        private int? limitField;

        private int? minimumField;

        private UnitOfMeasure unitOfMeasureField;

        private bool? shipSeperatelyField;

        private bool? captureCommentsField;

        private System.Nullable<DisplayAvailable> displayAvailableField;

        private System.Nullable<OrderUnavailableAction> orderUnavailableActionField;

        private bool? onlyAllowedQuantitiesField;

        private AllowedOfferQuantity[] allowedQuantitiesField;

        private bool? noShChargesField;

        private bool? noShFieldsField;

        private ShippingCategory shipsSeparatelyCategoryField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Limit
        {
            get { return limitField; }
            set { limitField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Minimum
        {
            get { return minimumField; }
            set { minimumField = value; }
        }

        /// <remarks/>
        public UnitOfMeasure UnitOfMeasure
        {
            get { return unitOfMeasureField; }
            set { unitOfMeasureField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? ShipSeperately
        {
            get { return shipSeperatelyField; }
            set { shipSeperatelyField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? CaptureComments
        {
            get { return captureCommentsField; }
            set { captureCommentsField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<DisplayAvailable> DisplayAvailable
        {
            get { return displayAvailableField; }
            set { displayAvailableField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<OrderUnavailableAction> OrderUnavailableAction
        {
            get { return orderUnavailableActionField; }
            set { orderUnavailableActionField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? OnlyAllowedQuantities
        {
            get { return onlyAllowedQuantitiesField; }
            set { onlyAllowedQuantitiesField = value; }
        }

        /// <remarks/>
        public AllowedOfferQuantity[] AllowedQuantities
        {
            get { return allowedQuantitiesField; }
            set { allowedQuantitiesField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? NoShCharges
        {
            get { return noShChargesField; }
            set { noShChargesField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? NoShFields
        {
            get { return noShFieldsField; }
            set { noShFieldsField = value; }
        }

        /// <remarks/>
        public ShippingCategory ShipsSeparatelyCategory
        {
            get { return shipsSeparatelyCategoryField; }
            set { shipsSeparatelyCategoryField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class UnitOfMeasure : PMObject
    {

        private int? seqIDField;

        private string descriptionField;

        private string idField;

        private int? sequenceField;

        private bool? removableField;

        private bool needsRemovalField;

        private bool wasModifiedField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        public string ID
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Sequence
        {
            get { return sequenceField; }
            set { sequenceField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? Removable
        {
            get { return removableField; }
            set { removableField = value; }
        }

        /// <remarks/>
        public bool NeedsRemoval
        {
            get { return needsRemovalField; }
            set { needsRemovalField = value; }
        }

        /// <remarks/>
        public bool WasModified
        {
            get { return wasModifiedField; }
            set { wasModifiedField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum DisplayAvailable
    {

        /// <remarks/>
        Suppress,

        /// <remarks/>
        AvailableBalance,

        /// <remarks/>
        InStockOutOfStockOnly,

        /// <remarks/>
        AvailableBalanceOutOfStock,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum OrderUnavailableAction
    {

        /// <remarks/>
        AllowOrdering,

        /// <remarks/>
        DisplayOfferNoOrdering,

        /// <remarks/>
        SuppressWhenOutOfStock,

        /// <remarks/>
        YesMaximumOrderQtyAsAvailableQty,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class AllowedOfferQuantity : PMObject
    {

        private int? seqIDField;

        private int? quantityField;

        private System.Nullable<byte> sequenceField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Quantity
        {
            get { return quantityField; }
            set { quantityField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<byte> Sequence
        {
            get { return sequenceField; }
            set { sequenceField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ShippingCategory : PMObject
    {

        private int? seqIDField;

        private string descriptionField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OfferStrings : PMObject
    {

        private string inStockField;

        private string outOfStockField;

        private string commentsField;

        private string additionalSearchTextField;

        private string commentsTextField;

        private string versionField;

        /// <remarks/>
        public string InStock
        {
            get { return inStockField; }
            set { inStockField = value; }
        }

        /// <remarks/>
        public string OutOfStock
        {
            get { return outOfStockField; }
            set { outOfStockField = value; }
        }

        /// <remarks/>
        public string Comments
        {
            get { return commentsField; }
            set { commentsField = value; }
        }

        /// <remarks/>
        public string AdditionalSearchText
        {
            get { return additionalSearchTextField; }
            set { additionalSearchTextField = value; }
        }

        /// <remarks/>
        public string CommentsText
        {
            get { return commentsTextField; }
            set { commentsTextField = value; }
        }

        /// <remarks/>
        public string Version
        {
            get { return versionField; }
            set { versionField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OfferPricing : PMObject
    {

        private System.Nullable<decimal> defaultPriceField;

        private System.Nullable<OfferPriceType> priceTypeField;

        private System.Nullable<decimal> shippingAndHandlingChargeField;

        private System.Nullable<ShippingAndHandlingChargeType> shippingAndHandlingChargeTypeField;

        private System.Nullable<decimal> chargePerPdfDownloadField;

        private System.Nullable<decimal> chargePerPptDownloadField;

        private bool? taxableField;

        private bool? clusterSurchargeField;

        private PriceFamily priceFamilyField;

        private PriceBreaks priceBreaksField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> DefaultPrice
        {
            get { return defaultPriceField; }
            set { defaultPriceField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<OfferPriceType> PriceType
        {
            get { return priceTypeField; }
            set { priceTypeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> ShippingAndHandlingCharge
        {
            get { return shippingAndHandlingChargeField; }
            set { shippingAndHandlingChargeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<ShippingAndHandlingChargeType> ShippingAndHandlingChargeType
        {
            get { return shippingAndHandlingChargeTypeField; }
            set { shippingAndHandlingChargeTypeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> ChargePerPdfDownload
        {
            get { return chargePerPdfDownloadField; }
            set { chargePerPdfDownloadField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> ChargePerPptDownload
        {
            get { return chargePerPptDownloadField; }
            set { chargePerPptDownloadField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? Taxable
        {
            get { return taxableField; }
            set { taxableField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? ClusterSurcharge
        {
            get { return clusterSurchargeField; }
            set { clusterSurchargeField = value; }
        }

        /// <remarks/>
        public PriceFamily PriceFamily
        {
            get { return priceFamilyField; }
            set { priceFamilyField = value; }
        }

        /// <remarks/>
        public PriceBreaks PriceBreaks
        {
            get { return priceBreaksField; }
            set { priceBreaksField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum ShippingAndHandlingChargeType
    {

        /// <remarks/>
        Each,

        /// <remarks/>
        PerLine,

        /// <remarks/>
        PerThousand,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class PriceFamily : PMObject
    {

        private int? seqIDField;

        private string idField;

        private string descriptionField;

        private System.Nullable<decimal> alertPercentField;

        private string text1Field;

        private System.Nullable<PriceFamilyData> dataSelect1Field;

        private string text2Field;

        private System.Nullable<PriceFamilyData> dataSelect2Field;

        private string text3Field;

        private System.Nullable<PriceFamilyData> dataSelect3Field;

        private int? sequenceField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string ID
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> AlertPercent
        {
            get { return alertPercentField; }
            set { alertPercentField = value; }
        }

        /// <remarks/>
        public string Text1
        {
            get { return text1Field; }
            set { text1Field = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<PriceFamilyData> DataSelect1
        {
            get { return dataSelect1Field; }
            set { dataSelect1Field = value; }
        }

        /// <remarks/>
        public string Text2
        {
            get { return text2Field; }
            set { text2Field = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<PriceFamilyData> DataSelect2
        {
            get { return dataSelect2Field; }
            set { dataSelect2Field = value; }
        }

        /// <remarks/>
        public string Text3
        {
            get { return text3Field; }
            set { text3Field = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<PriceFamilyData> DataSelect3
        {
            get { return dataSelect3Field; }
            set { dataSelect3Field = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Sequence
        {
            get { return sequenceField; }
            set { sequenceField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum PriceFamilyData
    {

        /// <remarks/>
        NotUsed,

        /// <remarks/>
        QuantityNeededForNextBreak,

        /// <remarks/>
        UnitPriceAtNextBreak,

        /// <remarks/>
        UnitPriceSavingsAtNextBreak,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class PriceBreaks : PMObject
    {

        private PricingStructure pricingStructureField;

        private System.Nullable<QuantityBreakType> quantityBreakTypeField;

        private PricingDetail[] breaksField;

        /// <remarks/>
        public PricingStructure PricingStructure
        {
            get { return pricingStructureField; }
            set { pricingStructureField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<QuantityBreakType> QuantityBreakType
        {
            get { return quantityBreakTypeField; }
            set { quantityBreakTypeField = value; }
        }

        /// <remarks/>
        public PricingDetail[] Breaks
        {
            get { return breaksField; }
            set { breaksField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class PricingStructure : PMObject
    {

        private int? seqIDField;

        private string descriptionField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum QuantityBreakType
    {

        /// <remarks/>
        PerOffer,

        /// <remarks/>
        PerLineItem,

        /// <remarks/>
        PriceFamily,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class PricingDetail : PMObject
    {

        private PricingStructure pricingStructureField;

        private PriceClass priceClassField;

        private int? startField;

        private string endField;

        private System.Nullable<decimal> priceField;

        /// <remarks/>
        public PricingStructure PricingStructure
        {
            get { return pricingStructureField; }
            set { pricingStructureField = value; }
        }

        /// <remarks/>
        public PriceClass PriceClass
        {
            get { return priceClassField; }
            set { priceClassField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Start
        {
            get { return startField; }
            set { startField = value; }
        }

        /// <remarks/>
        public string End
        {
            get { return endField; }
            set { endField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> Price
        {
            get { return priceField; }
            set { priceField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class PriceClass : PMObject
    {

        private int? seqIDField;

        private string descriptionField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OfferRecurrence : PMObject
    {

        private System.Nullable<RecurrenceType> recurrenceField;

        private RecurrenceSchedules recurrenceSchedulesField;

        private int? numberOfRecurrencesField;

        private System.Nullable<BillOfMaterialsType> recurrenceBillOfMaterialsTypeField;

        private DateTime? recurrenceStartDateField;

        private string recurrenceNoneField;

        private bool? allowSuspensionField;

        private bool? massReleaseField;

        private bool? hasOrdersField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<RecurrenceType> Recurrence
        {
            get { return recurrenceField; }
            set { recurrenceField = value; }
        }

        /// <remarks/>
        public RecurrenceSchedules RecurrenceSchedules
        {
            get { return recurrenceSchedulesField; }
            set { recurrenceSchedulesField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? NumberOfRecurrences
        {
            get { return numberOfRecurrencesField; }
            set { numberOfRecurrencesField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<BillOfMaterialsType> RecurrenceBillOfMaterialsType
        {
            get { return recurrenceBillOfMaterialsTypeField; }
            set { recurrenceBillOfMaterialsTypeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? RecurrenceStartDate
        {
            get { return recurrenceStartDateField; }
            set { recurrenceStartDateField = value; }
        }

        /// <remarks/>
        public string RecurrenceNone
        {
            get { return recurrenceNoneField; }
            set { recurrenceNoneField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? AllowSuspension
        {
            get { return allowSuspensionField; }
            set { allowSuspensionField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? MassRelease
        {
            get { return massReleaseField; }
            set { massReleaseField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? HasOrders
        {
            get { return hasOrdersField; }
            set { hasOrdersField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum RecurrenceType
    {

        /// <remarks/>
        None,

        /// <remarks/>
        Automatic,

        /// <remarks/>
        ListOfSchedules,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class RecurrenceSchedules : PMObject
    {

        private int? seqIDField;

        private string idField;

        private string descriptionField;

        private string definitionField;

        private string definitionDetailField;

        private System.Nullable<ScheduleType> scheduleField;

        private int? scheduleCountField;

        private System.Nullable<CycleType> cycleField;

        private DateTime? startCycleDateField;

        private int? daysToJoinField;

        private System.Nullable<MonthlyRecurrenceTypes> monthlyRecurrenceTypeField;

        private int? minimumRecurrencesField;

        private int? maximumRecurrencesField;

        private int? recurrenceIncrementField;

        private bool? allowInfiniteRecurrenceField;

        private string infiniteTagField;

        private string durationTagField;

        private DateTime? expirationDateField;

        private DateTime? nextCycleDateField;

        private bool? hasValidCycleDateField;

        private bool? anyOfferField;

        private RecurrenceScheduleShippingOption[] recurrenceScheduleShippingOptionsField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string ID
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        public string Definition
        {
            get { return definitionField; }
            set { definitionField = value; }
        }

        /// <remarks/>
        public string DefinitionDetail
        {
            get { return definitionDetailField; }
            set { definitionDetailField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<ScheduleType> Schedule
        {
            get { return scheduleField; }
            set { scheduleField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? ScheduleCount
        {
            get { return scheduleCountField; }
            set { scheduleCountField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<CycleType> Cycle
        {
            get { return cycleField; }
            set { cycleField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? StartCycleDate
        {
            get { return startCycleDateField; }
            set { startCycleDateField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? DaysToJoin
        {
            get { return daysToJoinField; }
            set { daysToJoinField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<MonthlyRecurrenceTypes> MonthlyRecurrenceType
        {
            get { return monthlyRecurrenceTypeField; }
            set { monthlyRecurrenceTypeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? MinimumRecurrences
        {
            get { return minimumRecurrencesField; }
            set { minimumRecurrencesField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? MaximumRecurrences
        {
            get { return maximumRecurrencesField; }
            set { maximumRecurrencesField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? RecurrenceIncrement
        {
            get { return recurrenceIncrementField; }
            set { recurrenceIncrementField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? AllowInfiniteRecurrence
        {
            get { return allowInfiniteRecurrenceField; }
            set { allowInfiniteRecurrenceField = value; }
        }

        /// <remarks/>
        public string InfiniteTag
        {
            get { return infiniteTagField; }
            set { infiniteTagField = value; }
        }

        /// <remarks/>
        public string DurationTag
        {
            get { return durationTagField; }
            set { durationTagField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? ExpirationDate
        {
            get { return expirationDateField; }
            set { expirationDateField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? NextCycleDate
        {
            get { return nextCycleDateField; }
            set { nextCycleDateField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? HasValidCycleDate
        {
            get { return hasValidCycleDateField; }
            set { hasValidCycleDateField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? AnyOffer
        {
            get { return anyOfferField; }
            set { anyOfferField = value; }
        }

        /// <remarks/>
        public RecurrenceScheduleShippingOption[] RecurrenceScheduleShippingOptions
        {
            get { return recurrenceScheduleShippingOptionsField; }
            set { recurrenceScheduleShippingOptionsField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum ScheduleType
    {

        /// <remarks/>
        EveryXDays,

        /// <remarks/>
        EveryXWeeks,

        /// <remarks/>
        EveryXMonths,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum CycleType
    {

        /// <remarks/>
        Rolling,

        /// <remarks/>
        Custom,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum MonthlyRecurrenceTypes
    {

        /// <remarks/>
        DayOfMonth,

        /// <remarks/>
        WeekOfMonthAndDayOfWeek,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class RecurrenceScheduleShippingOption : PMObject
    {

        private int? seqIDField;

        private RecurrenceSchedules recurrenceSchedulesField;

        private ShippingOption shippingOptionField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public RecurrenceSchedules RecurrenceSchedules
        {
            get { return recurrenceSchedulesField; }
            set { recurrenceSchedulesField = value; }
        }

        /// <remarks/>
        public ShippingOption ShippingOption
        {
            get { return shippingOptionField; }
            set { shippingOptionField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ShippingOption : PMObject
    {

        private string descriptionField;

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum BillOfMaterialsType
    {

        /// <remarks/>
        HardBOM,

        /// <remarks/>
        RollingBOM,

        /// <remarks/>
        BOMPerCycle,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OfferCustomization : PMObject
    {

        private bool? remoteOnlyField;

        private string linkToRemoteField;

        private string orderButtonTextField;

        private bool? allowOrderQuantityChangeField;

        private System.Nullable<RemotePriceTreatment> remotePriceTreatmentField;

        private CustomizationCode customizationCodeField;

        private bool? previewButtonField;

        private string previewTextField;

        private string previewLinkField;

        private bool? pFAllowReorderField;

        private int? pFReorderExpDaysField;

        private string reorderBtnTextField;

        private string reorderLinkTextField;

        private string prevOrdersTextField;

        private string remoteSystemParameter1Field;

        private string remoteSystemParameter2Field;

        private string remoteSystemParameter3Field;

        private RemoteSystem remoteSystemField;

        private string routingField;

        private CustomizationProfile customizationProfileField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? RemoteOnly
        {
            get { return remoteOnlyField; }
            set { remoteOnlyField = value; }
        }

        /// <remarks/>
        public string LinkToRemote
        {
            get { return linkToRemoteField; }
            set { linkToRemoteField = value; }
        }

        /// <remarks/>
        public string OrderButtonText
        {
            get { return orderButtonTextField; }
            set { orderButtonTextField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? AllowOrderQuantityChange
        {
            get { return allowOrderQuantityChangeField; }
            set { allowOrderQuantityChangeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<RemotePriceTreatment> RemotePriceTreatment
        {
            get { return remotePriceTreatmentField; }
            set { remotePriceTreatmentField = value; }
        }

        /// <remarks/>
        public CustomizationCode CustomizationCode
        {
            get { return customizationCodeField; }
            set { customizationCodeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? PreviewButton
        {
            get { return previewButtonField; }
            set { previewButtonField = value; }
        }

        /// <remarks/>
        public string PreviewText
        {
            get { return previewTextField; }
            set { previewTextField = value; }
        }

        /// <remarks/>
        public string PreviewLink
        {
            get { return previewLinkField; }
            set { previewLinkField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? PFAllowReorder
        {
            get { return pFAllowReorderField; }
            set { pFAllowReorderField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? PFReorderExpDays
        {
            get { return pFReorderExpDaysField; }
            set { pFReorderExpDaysField = value; }
        }

        /// <remarks/>
        public string ReorderBtnText
        {
            get { return reorderBtnTextField; }
            set { reorderBtnTextField = value; }
        }

        /// <remarks/>
        public string ReorderLinkText
        {
            get { return reorderLinkTextField; }
            set { reorderLinkTextField = value; }
        }

        /// <remarks/>
        public string PrevOrdersText
        {
            get { return prevOrdersTextField; }
            set { prevOrdersTextField = value; }
        }

        /// <remarks/>
        public string RemoteSystemParameter1
        {
            get { return remoteSystemParameter1Field; }
            set { remoteSystemParameter1Field = value; }
        }

        /// <remarks/>
        public string RemoteSystemParameter2
        {
            get { return remoteSystemParameter2Field; }
            set { remoteSystemParameter2Field = value; }
        }

        /// <remarks/>
        public string RemoteSystemParameter3
        {
            get { return remoteSystemParameter3Field; }
            set { remoteSystemParameter3Field = value; }
        }

        /// <remarks/>
        public RemoteSystem RemoteSystem
        {
            get { return remoteSystemField; }
            set { remoteSystemField = value; }
        }

        /// <remarks/>
        public string Routing
        {
            get { return routingField; }
            set { routingField = value; }
        }

        /// <remarks/>
        public CustomizationProfile CustomizationProfile
        {
            get { return customizationProfileField; }
            set { customizationProfileField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum RemotePriceTreatment
    {

        /// <remarks/>
        OfferPricing,

        /// <remarks/>
        RemotePriceAsUnitPrice,

        /// <remarks/>
        RemotePriceAsFlatPrice,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class CustomizationCode : PMObject
    {

        private int? seqIDField;

        private string idField;

        private string descriptionField;

        private string commentsField;

        private VariableField[] variableFieldsField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string ID
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        public string Comments
        {
            get { return commentsField; }
            set { commentsField = value; }
        }

        /// <remarks/>
        public VariableField[] VariableFields
        {
            get { return variableFieldsField; }
            set { variableFieldsField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class RemoteSystem : PMObject
    {

        private int? seqIDField;

        private string descriptionField;

        private string nDUrlField;

        private string nDReturnParameterField;

        private string nDParameter1Field;

        private string nDParameter1DescriptionField;

        private string nDParameter2Field;

        private string nDParameter2DescriptionField;

        private string nDParameter3Field;

        private string nDParameter3DescriptionField;

        private string returnDocParameterField;

        private string returnQuantityParameterField;

        private string editUrlField;

        private string editReturnParameterField;

        private string editUIDParameterField;

        private string editDocIDParameterField;

        private string proofUrlField;

        private string proofDocIDParameterField;

        private string proofOfferIDParameterField;

        private string finalUrlField;

        private string finalDocIdParameterField;

        private string finalOfferIDParameterField;

        private bool? needsTicketField;

        private string ticketRSPField;

        private string enterDocUrlField;

        private string enterDocParameterField;

        private string priceParameterField;

        private bool? passUserInfoField;

        private string orderLinkField;

        private string orderDocParameterField;

        private string orderUserParameterField;

        private string finalTicketUrlField;

        private string reorderUrlField;

        private string reorderReturnParameterField;

        private string reorderUIDParameterField;

        private string reorderDocIDParameterField;

        private string orderServiceField;

        private string orderActionField;

        private int? pMKeyField;

        private string userIDField;

        private string passwordField;

        private string additionalParameterField;

        private string commParameterField;

        private string updateUrlField;

        private string queryUrlField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        public string NDUrl
        {
            get { return nDUrlField; }
            set { nDUrlField = value; }
        }

        /// <remarks/>
        public string NDReturnParameter
        {
            get { return nDReturnParameterField; }
            set { nDReturnParameterField = value; }
        }

        /// <remarks/>
        public string NDParameter1
        {
            get { return nDParameter1Field; }
            set { nDParameter1Field = value; }
        }

        /// <remarks/>
        public string NDParameter1Description
        {
            get { return nDParameter1DescriptionField; }
            set { nDParameter1DescriptionField = value; }
        }

        /// <remarks/>
        public string NDParameter2
        {
            get { return nDParameter2Field; }
            set { nDParameter2Field = value; }
        }

        /// <remarks/>
        public string NDParameter2Description
        {
            get { return nDParameter2DescriptionField; }
            set { nDParameter2DescriptionField = value; }
        }

        /// <remarks/>
        public string NDParameter3
        {
            get { return nDParameter3Field; }
            set { nDParameter3Field = value; }
        }

        /// <remarks/>
        public string NDParameter3Description
        {
            get { return nDParameter3DescriptionField; }
            set { nDParameter3DescriptionField = value; }
        }

        /// <remarks/>
        public string ReturnDocParameter
        {
            get { return returnDocParameterField; }
            set { returnDocParameterField = value; }
        }

        /// <remarks/>
        public string ReturnQuantityParameter
        {
            get { return returnQuantityParameterField; }
            set { returnQuantityParameterField = value; }
        }

        /// <remarks/>
        public string EditUrl
        {
            get { return editUrlField; }
            set { editUrlField = value; }
        }

        /// <remarks/>
        public string EditReturnParameter
        {
            get { return editReturnParameterField; }
            set { editReturnParameterField = value; }
        }

        /// <remarks/>
        public string EditUIDParameter
        {
            get { return editUIDParameterField; }
            set { editUIDParameterField = value; }
        }

        /// <remarks/>
        public string EditDocIDParameter
        {
            get { return editDocIDParameterField; }
            set { editDocIDParameterField = value; }
        }

        /// <remarks/>
        public string ProofUrl
        {
            get { return proofUrlField; }
            set { proofUrlField = value; }
        }

        /// <remarks/>
        public string ProofDocIDParameter
        {
            get { return proofDocIDParameterField; }
            set { proofDocIDParameterField = value; }
        }

        /// <remarks/>
        public string ProofOfferIDParameter
        {
            get { return proofOfferIDParameterField; }
            set { proofOfferIDParameterField = value; }
        }

        /// <remarks/>
        public string FinalUrl
        {
            get { return finalUrlField; }
            set { finalUrlField = value; }
        }

        /// <remarks/>
        public string FinalDocIdParameter
        {
            get { return finalDocIdParameterField; }
            set { finalDocIdParameterField = value; }
        }

        /// <remarks/>
        public string FinalOfferIDParameter
        {
            get { return finalOfferIDParameterField; }
            set { finalOfferIDParameterField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? NeedsTicket
        {
            get { return needsTicketField; }
            set { needsTicketField = value; }
        }

        /// <remarks/>
        public string TicketRSP
        {
            get { return ticketRSPField; }
            set { ticketRSPField = value; }
        }

        /// <remarks/>
        public string EnterDocUrl
        {
            get { return enterDocUrlField; }
            set { enterDocUrlField = value; }
        }

        /// <remarks/>
        public string EnterDocParameter
        {
            get { return enterDocParameterField; }
            set { enterDocParameterField = value; }
        }

        /// <remarks/>
        public string PriceParameter
        {
            get { return priceParameterField; }
            set { priceParameterField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? PassUserInfo
        {
            get { return passUserInfoField; }
            set { passUserInfoField = value; }
        }

        /// <remarks/>
        public string OrderLink
        {
            get { return orderLinkField; }
            set { orderLinkField = value; }
        }

        /// <remarks/>
        public string OrderDocParameter
        {
            get { return orderDocParameterField; }
            set { orderDocParameterField = value; }
        }

        /// <remarks/>
        public string OrderUserParameter
        {
            get { return orderUserParameterField; }
            set { orderUserParameterField = value; }
        }

        /// <remarks/>
        public string FinalTicketUrl
        {
            get { return finalTicketUrlField; }
            set { finalTicketUrlField = value; }
        }

        /// <remarks/>
        public string ReorderUrl
        {
            get { return reorderUrlField; }
            set { reorderUrlField = value; }
        }

        /// <remarks/>
        public string ReorderReturnParameter
        {
            get { return reorderReturnParameterField; }
            set { reorderReturnParameterField = value; }
        }

        /// <remarks/>
        public string ReorderUIDParameter
        {
            get { return reorderUIDParameterField; }
            set { reorderUIDParameterField = value; }
        }

        /// <remarks/>
        public string ReorderDocIDParameter
        {
            get { return reorderDocIDParameterField; }
            set { reorderDocIDParameterField = value; }
        }

        /// <remarks/>
        public string OrderService
        {
            get { return orderServiceField; }
            set { orderServiceField = value; }
        }

        /// <remarks/>
        public string OrderAction
        {
            get { return orderActionField; }
            set { orderActionField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? PMKey
        {
            get { return pMKeyField; }
            set { pMKeyField = value; }
        }

        /// <remarks/>
        public string UserID
        {
            get { return userIDField; }
            set { userIDField = value; }
        }

        /// <remarks/>
        public string Password
        {
            get { return passwordField; }
            set { passwordField = value; }
        }

        /// <remarks/>
        public string AdditionalParameter
        {
            get { return additionalParameterField; }
            set { additionalParameterField = value; }
        }

        /// <remarks/>
        public string CommParameter
        {
            get { return commParameterField; }
            set { commParameterField = value; }
        }

        /// <remarks/>
        public string UpdateUrl
        {
            get { return updateUrlField; }
            set { updateUrlField = value; }
        }

        /// <remarks/>
        public string QueryUrl
        {
            get { return queryUrlField; }
            set { queryUrlField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class CustomizationProfile : PMObject
    {

        private int? seqIDField;

        private bool? remoteOnlyField;

        private string idField;

        private RemoteSystem remoteSystemField;

        private string rCFld1Field;

        private string rCFld2Field;

        private string rCFld3Field;

        private string routingField;

        private string orderButtonTextField;

        private RemotePriceTreatment remoteSystemPricingField;

        private CustomizationCode customizationCodeField;

        private bool? previewButtonField;

        private string previewTextField;

        private string previewLinkField;

        private bool? allowOrderQuantityChangeField;

        private string remoteLinkField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? RemoteOnly
        {
            get { return remoteOnlyField; }
            set { remoteOnlyField = value; }
        }

        /// <remarks/>
        public string ID
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        public RemoteSystem RemoteSystem
        {
            get { return remoteSystemField; }
            set { remoteSystemField = value; }
        }

        /// <remarks/>
        public string RCFld1
        {
            get { return rCFld1Field; }
            set { rCFld1Field = value; }
        }

        /// <remarks/>
        public string RCFld2
        {
            get { return rCFld2Field; }
            set { rCFld2Field = value; }
        }

        /// <remarks/>
        public string RCFld3
        {
            get { return rCFld3Field; }
            set { rCFld3Field = value; }
        }

        /// <remarks/>
        public string Routing
        {
            get { return routingField; }
            set { routingField = value; }
        }

        /// <remarks/>
        public string OrderButtonText
        {
            get { return orderButtonTextField; }
            set { orderButtonTextField = value; }
        }

        /// <remarks/>
        public RemotePriceTreatment RemoteSystemPricing
        {
            get { return remoteSystemPricingField; }
            set { remoteSystemPricingField = value; }
        }

        /// <remarks/>
        public CustomizationCode CustomizationCode
        {
            get { return customizationCodeField; }
            set { customizationCodeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? PreviewButton
        {
            get { return previewButtonField; }
            set { previewButtonField = value; }
        }

        /// <remarks/>
        public string PreviewText
        {
            get { return previewTextField; }
            set { previewTextField = value; }
        }

        /// <remarks/>
        public string PreviewLink
        {
            get { return previewLinkField; }
            set { previewLinkField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? AllowOrderQuantityChange
        {
            get { return allowOrderQuantityChangeField; }
            set { allowOrderQuantityChangeField = value; }
        }

        /// <remarks/>
        public string RemoteLink
        {
            get { return remoteLinkField; }
            set { remoteLinkField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OfferLinks : PMObject
    {

        private string htmlLinkField;

        private string pdfLinkField;

        private string pdfTextField;

        private string pptLinkField;

        private string pptTextField;

        private string otherLink1Field;

        private string otherText1Field;

        private string otherLink2Field;

        private string otherText2Field;

        private string otherLink3Field;

        private string otherText3Field;

        /// <remarks/>
        public string HtmlLink
        {
            get { return htmlLinkField; }
            set { htmlLinkField = value; }
        }

        /// <remarks/>
        public string PdfLink
        {
            get { return pdfLinkField; }
            set { pdfLinkField = value; }
        }

        /// <remarks/>
        public string PdfText
        {
            get { return pdfTextField; }
            set { pdfTextField = value; }
        }

        /// <remarks/>
        public string PptLink
        {
            get { return pptLinkField; }
            set { pptLinkField = value; }
        }

        /// <remarks/>
        public string PptText
        {
            get { return pptTextField; }
            set { pptTextField = value; }
        }

        /// <remarks/>
        public string OtherLink1
        {
            get { return otherLink1Field; }
            set { otherLink1Field = value; }
        }

        /// <remarks/>
        public string OtherText1
        {
            get { return otherText1Field; }
            set { otherText1Field = value; }
        }

        /// <remarks/>
        public string OtherLink2
        {
            get { return otherLink2Field; }
            set { otherLink2Field = value; }
        }

        /// <remarks/>
        public string OtherText2
        {
            get { return otherText2Field; }
            set { otherText2Field = value; }
        }

        /// <remarks/>
        public string OtherLink3
        {
            get { return otherLink3Field; }
            set { otherLink3Field = value; }
        }

        /// <remarks/>
        public string OtherText3
        {
            get { return otherText3Field; }
            set { otherText3Field = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OfferImages : PMObject
    {

        private bool? localImagesField;

        private string fullImageTextField;

        private string directoryField;

        private string thumbnailFilenameField;

        private string fullImageFilenameField;

        private OfferImage[] microImagesField;

        private OfferImage[] removedImagesField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? LocalImages
        {
            get { return localImagesField; }
            set { localImagesField = value; }
        }

        /// <remarks/>
        public string FullImageText
        {
            get { return fullImageTextField; }
            set { fullImageTextField = value; }
        }

        /// <remarks/>
        public string Directory
        {
            get { return directoryField; }
            set { directoryField = value; }
        }

        /// <remarks/>
        public string ThumbnailFilename
        {
            get { return thumbnailFilenameField; }
            set { thumbnailFilenameField = value; }
        }

        /// <remarks/>
        public string FullImageFilename
        {
            get { return fullImageFilenameField; }
            set { fullImageFilenameField = value; }
        }

        /// <remarks/>
        public OfferImage[] MicroImages
        {
            get { return microImagesField; }
            set { microImagesField = value; }
        }

        /// <remarks/>
        public OfferImage[] RemovedImages
        {
            get { return removedImagesField; }
            set { removedImagesField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OfferImage : PMObject
    {

        private int? seqIDField;

        private string descriptionField;

        private string microImagePathField;

        private string thumbnailImagePathField;

        private string fullImagePathField;

        private int? sequenceField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        public string MicroImagePath
        {
            get { return microImagePathField; }
            set { microImagePathField = value; }
        }

        /// <remarks/>
        public string ThumbnailImagePath
        {
            get { return thumbnailImagePathField; }
            set { thumbnailImagePathField = value; }
        }

        /// <remarks/>
        public string FullImagePath
        {
            get { return fullImagePathField; }
            set { fullImagePathField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Sequence
        {
            get { return sequenceField; }
            set { sequenceField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OfferCategorization : PMObject
    {

        private bool? featuredOnMainField;

        private DateTime? mainFeatureEndDateField;

        private OfferSort[][] sortGroupingsField;

        private OfferCategory[] customCategoriesField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? FeaturedOnMain
        {
            get { return featuredOnMainField; }
            set { featuredOnMainField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? MainFeatureEndDate
        {
            get { return mainFeatureEndDateField; }
            set { mainFeatureEndDateField = value; }
        }

        /// <remarks/>
        [System.Xml.Serialization.XmlArrayItemAttribute("ArrayOfOfferSort")]
        [System.Xml.Serialization.XmlArrayItemAttribute(NestingLevel = 1)]
        public OfferSort[][] SortGroupings
        {
            get { return sortGroupingsField; }
            set { sortGroupingsField = value; }
        }

        /// <remarks/>
        public OfferCategory[] CustomCategories
        {
            get { return customCategoriesField; }
            set { customCategoriesField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OfferSort : PMObject
    {

        private int? branchCountField;

        private OfferSortGroup sortGroupField;

        private OfferSortGroupXRef sortGroupXRefField;

        private bool? featureField;

        private DateTime? featureEndDateField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? BranchCount
        {
            get { return branchCountField; }
            set { branchCountField = value; }
        }

        /// <remarks/>
        public OfferSortGroup SortGroup
        {
            get { return sortGroupField; }
            set { sortGroupField = value; }
        }

        /// <remarks/>
        public OfferSortGroupXRef SortGroupXRef
        {
            get { return sortGroupXRefField; }
            set { sortGroupXRefField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? Feature
        {
            get { return featureField; }
            set { featureField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? FeatureEndDate
        {
            get { return featureEndDateField; }
            set { featureEndDateField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OfferSortGroup : PMObject
    {

        private string descriptionField;

        private OfferSortLevel levelField;

        private string sortKeyField;

        private string imagePathField;

        private bool? imageLocalField;

        private string headerPathField;

        private bool? headerLocalField;

        private string headerTextField;

        private string footerPathField;

        private bool? footerLocalField;

        private string footerTextField;

        private bool? suppressImgTextField;

        private string prefixHTMLField;

        private string suffixHTMLField;

        private int? classicViewListTypeField;

        private int? shoppingCartViewListTypeField;

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        public OfferSortLevel Level
        {
            get { return levelField; }
            set { levelField = value; }
        }

        /// <remarks/>
        public string SortKey
        {
            get { return sortKeyField; }
            set { sortKeyField = value; }
        }

        /// <remarks/>
        public string ImagePath
        {
            get { return imagePathField; }
            set { imagePathField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? ImageLocal
        {
            get { return imageLocalField; }
            set { imageLocalField = value; }
        }

        /// <remarks/>
        public string HeaderPath
        {
            get { return headerPathField; }
            set { headerPathField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? HeaderLocal
        {
            get { return headerLocalField; }
            set { headerLocalField = value; }
        }

        /// <remarks/>
        public string HeaderText
        {
            get { return headerTextField; }
            set { headerTextField = value; }
        }

        /// <remarks/>
        public string FooterPath
        {
            get { return footerPathField; }
            set { footerPathField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? FooterLocal
        {
            get { return footerLocalField; }
            set { footerLocalField = value; }
        }

        /// <remarks/>
        public string FooterText
        {
            get { return footerTextField; }
            set { footerTextField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? SuppressImgText
        {
            get { return suppressImgTextField; }
            set { suppressImgTextField = value; }
        }

        /// <remarks/>
        public string PrefixHTML
        {
            get { return prefixHTMLField; }
            set { prefixHTMLField = value; }
        }

        /// <remarks/>
        public string SuffixHTML
        {
            get { return suffixHTMLField; }
            set { suffixHTMLField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? ClassicViewListType
        {
            get { return classicViewListTypeField; }
            set { classicViewListTypeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? ShoppingCartViewListType
        {
            get { return shoppingCartViewListTypeField; }
            set { shoppingCartViewListTypeField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OfferSortLevel : PMObject
    {

        private string descriptionField;

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OfferSortGroupXRef : PMObject
    {

        private int? seqIDField;

        private OfferSortGroup sortGroupField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public OfferSortGroup SortGroup
        {
            get { return sortGroupField; }
            set { sortGroupField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OfferCategory : PMObject
    {

        private int? seqIDField;

        private CustomCategory categoryField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public CustomCategory Category
        {
            get { return categoryField; }
            set { categoryField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class CustomCategory : PMObject
    {

        private int? seqIDField;

        private CustomCategoryDef categoryDefField;

        private string descriptionField;

        private int? sequenceField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public CustomCategoryDef CategoryDef
        {
            get { return categoryDefField; }
            set { categoryDefField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Sequence
        {
            get { return sequenceField; }
            set { sequenceField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class CustomCategoryDef : PMObject
    {

        private int? seqIDField;

        private string descriptionField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OfferDropShip : PMObject
    {

        private string checkOutTextField;

        private System.Nullable<double> weightField;

        private System.Nullable<WeightType> weightTypeField;

        /// <remarks/>
        public string CheckOutText
        {
            get { return checkOutTextField; }
            set { checkOutTextField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<double> Weight
        {
            get { return weightField; }
            set { weightField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<WeightType> WeightType
        {
            get { return weightTypeField; }
            set { weightTypeField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class EDelivery : PMObject
    {

        private bool? uploadAttemptedField;

        private bool? uploadSuccessField;

        private string directoryField;

        private string filenameField;

        private int? expirationDaysField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? UploadAttempted
        {
            get { return uploadAttemptedField; }
            set { uploadAttemptedField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? UploadSuccess
        {
            get { return uploadSuccessField; }
            set { uploadSuccessField = value; }
        }

        /// <remarks/>
        public string Directory
        {
            get { return directoryField; }
            set { directoryField = value; }
        }

        /// <remarks/>
        public string Filename
        {
            get { return filenameField; }
            set { filenameField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? ExpirationDays
        {
            get { return expirationDaysField; }
            set { expirationDaysField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OfferRestriction : PMObject
    {

        private RestrictionType restrictionTypeField;

        private int? limitField;

        private SuppressOffer suppressField;

        private bool allowOrderingField;

        private bool suppressDisplayField;

        private bool viewOnlyField;

        private bool? paymentAlwaysRequiredField;

        private string commentsField;

        /// <remarks/>
        public RestrictionType RestrictionType
        {
            get { return restrictionTypeField; }
            set { restrictionTypeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Limit
        {
            get { return limitField; }
            set { limitField = value; }
        }

        /// <remarks/>
        public SuppressOffer Suppress
        {
            get { return suppressField; }
            set { suppressField = value; }
        }

        /// <remarks/>
        public bool AllowOrdering
        {
            get { return allowOrderingField; }
            set { allowOrderingField = value; }
        }

        /// <remarks/>
        public bool SuppressDisplay
        {
            get { return suppressDisplayField; }
            set { suppressDisplayField = value; }
        }

        /// <remarks/>
        public bool ViewOnly
        {
            get { return viewOnlyField; }
            set { viewOnlyField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? PaymentAlwaysRequired
        {
            get { return paymentAlwaysRequiredField; }
            set { paymentAlwaysRequiredField = value; }
        }

        /// <remarks/>
        public string Comments
        {
            get { return commentsField; }
            set { commentsField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class RestrictionType : PMObject
    {

        private int? seqIDField;

        private string descriptionField;

        private bool? budgetTaxField;

        private bool? budgetSHField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? BudgetTax
        {
            get { return budgetTaxField; }
            set { budgetTaxField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? BudgetSH
        {
            get { return budgetSHField; }
            set { budgetSHField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum SuppressOffer
    {

        /// <remarks/>
        No,

        /// <remarks/>
        Yes,

        /// <remarks/>
        ViewOnly,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OfferComponent : PMObject
    {

        private int? seqIDField;

        private ProductID productField;

        private ProductCluster clusterField;

        private int? pTASKSField;

        private int? quantityField;

        private string instructionsField;

        private System.Nullable<BackorderTreatment> backorderTreatmentField;

        private CustomAssemblyGroup customAssemblyGroupField;

        private bool? requiredField;

        private bool? isPrimaryRevenueField;

        private System.Nullable<decimal> revenueAmountField;

        private RevenueCenter revenueCenterField;

        private bool? userSpecifiedQuantitiesField;

        private bool? excludeFromAvailabilityField;

        private bool? doNotShipAloneField;

        private bool? cADefaultField;

        private int? sequenceField;

        private CustomizationProfile customizationProfileField;

        private ClusterSurcharge surchargeField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public ProductID Product
        {
            get { return productField; }
            set { productField = value; }
        }

        /// <remarks/>
        public ProductCluster Cluster
        {
            get { return clusterField; }
            set { clusterField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? PTASKS
        {
            get { return pTASKSField; }
            set { pTASKSField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Quantity
        {
            get { return quantityField; }
            set { quantityField = value; }
        }

        /// <remarks/>
        public string Instructions
        {
            get { return instructionsField; }
            set { instructionsField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<BackorderTreatment> BackorderTreatment
        {
            get { return backorderTreatmentField; }
            set { backorderTreatmentField = value; }
        }

        /// <remarks/>
        public CustomAssemblyGroup CustomAssemblyGroup
        {
            get { return customAssemblyGroupField; }
            set { customAssemblyGroupField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? Required
        {
            get { return requiredField; }
            set { requiredField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? IsPrimaryRevenue
        {
            get { return isPrimaryRevenueField; }
            set { isPrimaryRevenueField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> RevenueAmount
        {
            get { return revenueAmountField; }
            set { revenueAmountField = value; }
        }

        /// <remarks/>
        public RevenueCenter RevenueCenter
        {
            get { return revenueCenterField; }
            set { revenueCenterField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? UserSpecifiedQuantities
        {
            get { return userSpecifiedQuantitiesField; }
            set { userSpecifiedQuantitiesField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? ExcludeFromAvailability
        {
            get { return excludeFromAvailabilityField; }
            set { excludeFromAvailabilityField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? DoNotShipAlone
        {
            get { return doNotShipAloneField; }
            set { doNotShipAloneField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? CADefault
        {
            get { return cADefaultField; }
            set { cADefaultField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Sequence
        {
            get { return sequenceField; }
            set { sequenceField = value; }
        }

        /// <remarks/>
        public CustomizationProfile CustomizationProfile
        {
            get { return customizationProfileField; }
            set { customizationProfileField = value; }
        }

        /// <remarks/>
        public ClusterSurcharge Surcharge
        {
            get { return surchargeField; }
            set { surchargeField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum BackorderTreatment
    {

        /// <remarks/>
        SystemDefault,

        /// <remarks/>
        ShipInStockBackorderRest,

        /// <remarks/>
        BackorderWholeOrder,

        /// <remarks/>
        ShipInStockCancelRest,

        /// <remarks/>
        BackorderEntireOfferLine,

        /// <remarks/>
        ShipCompleteReserveAvailable,

        /// <remarks/>
        ShipCompleteOffersReserveAvailable,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class CustomAssemblyGroup : PMObject
    {

        private int? seqIDField;

        private string headingField;

        private int? minimumQuantityField;

        private int? maximumQuantityField;

        private int? orderField;

        private int? groupTypeField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string Heading
        {
            get { return headingField; }
            set { headingField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? MinimumQuantity
        {
            get { return minimumQuantityField; }
            set { minimumQuantityField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? MaximumQuantity
        {
            get { return maximumQuantityField; }
            set { maximumQuantityField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Order
        {
            get { return orderField; }
            set { orderField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? GroupType
        {
            get { return groupTypeField; }
            set { groupTypeField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ClusterSurcharge : PMObject
    {

        private int? seqIDField;

        private Offer offerField;

        private Product productField;

        private System.Nullable<decimal> surchargeField;

        private string surchargeTextField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public Offer Offer
        {
            get { return offerField; }
            set { offerField = value; }
        }

        /// <remarks/>
        public Product Product
        {
            get { return productField; }
            set { productField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> Surcharge
        {
            get { return surchargeField; }
            set { surchargeField = value; }
        }

        /// <remarks/>
        public string SurchargeText
        {
            get { return surchargeTextField; }
            set { surchargeTextField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class SupplierPart : PMObject
    {

        private int? seqIDField;

        private DropShipSupplier supplierField;

        private DateTime? startDateField;

        private DateTime? endDateField;

        private string partNumberField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public DropShipSupplier Supplier
        {
            get { return supplierField; }
            set { supplierField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? StartDate
        {
            get { return startDateField; }
            set { startDateField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? EndDate
        {
            get { return endDateField; }
            set { endDateField = value; }
        }

        /// <remarks/>
        public string PartNumber
        {
            get { return partNumberField; }
            set { partNumberField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class DropShipSupplier : PMObject
    {

        private int? seqIDField;

        private string nameField;

        private string accountNoField;

        private string referenceField;

        private int? communicationMethodField;

        private int? shippingMethodField;

        private string xMLLinkField;

        private string directoryNameField;

        private string physicalDirectoryField;

        private string emailAddressField;

        private string cCEmailAddressField;

        private string bCCEmailAddressField;

        private string address1Field;

        private string address2Field;

        private string address3Field;

        private string cityField;

        private string stateField;

        private string postalCodeField;

        private string countryCodeField;

        private string phoneField;

        private string upsShipperNoField;

        private string upsPickUpCodeField;

        private int? uPSTareWeightField;

        private string uPSPackageTypeField;

        private System.Nullable<byte> uPSRateTypeField;

        private string fEDEXAccountNumberField;

        private string fEDEXMeterNumberField;

        private string fEDEXServiceTypeField;

        private string fEDEXPackagingTypeField;

        private int? fEDEXTareWeightField;

        private int? timeZoneField;

        private string defaultDropShipImportDirField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string Name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks/>
        public string AccountNo
        {
            get { return accountNoField; }
            set { accountNoField = value; }
        }

        /// <remarks/>
        public string Reference
        {
            get { return referenceField; }
            set { referenceField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? CommunicationMethod
        {
            get { return communicationMethodField; }
            set { communicationMethodField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? ShippingMethod
        {
            get { return shippingMethodField; }
            set { shippingMethodField = value; }
        }

        /// <remarks/>
        public string XMLLink
        {
            get { return xMLLinkField; }
            set { xMLLinkField = value; }
        }

        /// <remarks/>
        public string DirectoryName
        {
            get { return directoryNameField; }
            set { directoryNameField = value; }
        }

        /// <remarks/>
        public string PhysicalDirectory
        {
            get { return physicalDirectoryField; }
            set { physicalDirectoryField = value; }
        }

        /// <remarks/>
        public string EmailAddress
        {
            get { return emailAddressField; }
            set { emailAddressField = value; }
        }

        /// <remarks/>
        public string CCEmailAddress
        {
            get { return cCEmailAddressField; }
            set { cCEmailAddressField = value; }
        }

        /// <remarks/>
        public string BCCEmailAddress
        {
            get { return bCCEmailAddressField; }
            set { bCCEmailAddressField = value; }
        }

        /// <remarks/>
        public string Address1
        {
            get { return address1Field; }
            set { address1Field = value; }
        }

        /// <remarks/>
        public string Address2
        {
            get { return address2Field; }
            set { address2Field = value; }
        }

        /// <remarks/>
        public string Address3
        {
            get { return address3Field; }
            set { address3Field = value; }
        }

        /// <remarks/>
        public string City
        {
            get { return cityField; }
            set { cityField = value; }
        }

        /// <remarks/>
        public string State
        {
            get { return stateField; }
            set { stateField = value; }
        }

        /// <remarks/>
        public string PostalCode
        {
            get { return postalCodeField; }
            set { postalCodeField = value; }
        }

        /// <remarks/>
        public string CountryCode
        {
            get { return countryCodeField; }
            set { countryCodeField = value; }
        }

        /// <remarks/>
        public string Phone
        {
            get { return phoneField; }
            set { phoneField = value; }
        }

        /// <remarks/>
        public string UpsShipperNo
        {
            get { return upsShipperNoField; }
            set { upsShipperNoField = value; }
        }

        /// <remarks/>
        public string UpsPickUpCode
        {
            get { return upsPickUpCodeField; }
            set { upsPickUpCodeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? UPSTareWeight
        {
            get { return uPSTareWeightField; }
            set { uPSTareWeightField = value; }
        }

        /// <remarks/>
        public string UPSPackageType
        {
            get { return uPSPackageTypeField; }
            set { uPSPackageTypeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<byte> UPSRateType
        {
            get { return uPSRateTypeField; }
            set { uPSRateTypeField = value; }
        }

        /// <remarks/>
        public string FEDEXAccountNumber
        {
            get { return fEDEXAccountNumberField; }
            set { fEDEXAccountNumberField = value; }
        }

        /// <remarks/>
        public string FEDEXMeterNumber
        {
            get { return fEDEXMeterNumberField; }
            set { fEDEXMeterNumberField = value; }
        }

        /// <remarks/>
        public string FEDEXServiceType
        {
            get { return fEDEXServiceTypeField; }
            set { fEDEXServiceTypeField = value; }
        }

        /// <remarks/>
        public string FEDEXPackagingType
        {
            get { return fEDEXPackagingTypeField; }
            set { fEDEXPackagingTypeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? FEDEXTareWeight
        {
            get { return fEDEXTareWeightField; }
            set { fEDEXTareWeightField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? TimeZone
        {
            get { return timeZoneField; }
            set { timeZoneField = value; }
        }

        /// <remarks/>
        public string DefaultDropShipImportDir
        {
            get { return defaultDropShipImportDirField; }
            set { defaultDropShipImportDirField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class Upsell : PMObject
    {

        private int? seqIDField;

        private OfferID upsellOfferField;

        private bool? showOnOfferDetailField;

        private string offerDetailCommentsField;

        private bool? showOnSplashField;

        private string splashCommentsField;

        private bool needsRemovalField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public OfferID UpsellOffer
        {
            get { return upsellOfferField; }
            set { upsellOfferField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? ShowOnOfferDetail
        {
            get { return showOnOfferDetailField; }
            set { showOnOfferDetailField = value; }
        }

        /// <remarks/>
        public string OfferDetailComments
        {
            get { return offerDetailCommentsField; }
            set { offerDetailCommentsField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? ShowOnSplash
        {
            get { return showOnSplashField; }
            set { showOnSplashField = value; }
        }

        /// <remarks/>
        public string SplashComments
        {
            get { return splashCommentsField; }
            set { splashCommentsField = value; }
        }

        /// <remarks/>
        public bool NeedsRemoval
        {
            get { return needsRemovalField; }
            set { needsRemovalField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OfferID : PMObject
    {

        private OfferIDHeader headerField;

        /// <remarks/>
        public OfferIDHeader Header
        {
            get { return headerField; }
            set { headerField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OfferIDHeader : PMObject
    {

        private int? seqIDField;

        private string idField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string ID
        {
            get { return idField; }
            set { idField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OfferScheduleParameters : PMObject
    {

        private int? seqIDField;

        private RecurrenceSchedules recurrenceSchedulesField;

        private string overrideDescriptionField;

        private System.Nullable<RecurrenceDurationType> durationTypeField;

        private int? minimumRecurrencesField;

        private int? maximumRecurrencesField;

        private int? recurrenceIncrementField;

        private DateTime? lastCycleDateField;

        private System.Nullable<decimal> discountPercentageField;

        private string discountTextField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public RecurrenceSchedules RecurrenceSchedules
        {
            get { return recurrenceSchedulesField; }
            set { recurrenceSchedulesField = value; }
        }

        /// <remarks/>
        public string OverrideDescription
        {
            get { return overrideDescriptionField; }
            set { overrideDescriptionField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<RecurrenceDurationType> DurationType
        {
            get { return durationTypeField; }
            set { durationTypeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? MinimumRecurrences
        {
            get { return minimumRecurrencesField; }
            set { minimumRecurrencesField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? MaximumRecurrences
        {
            get { return maximumRecurrencesField; }
            set { maximumRecurrencesField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? RecurrenceIncrement
        {
            get { return recurrenceIncrementField; }
            set { recurrenceIncrementField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? LastCycleDate
        {
            get { return lastCycleDateField; }
            set { lastCycleDateField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> DiscountPercentage
        {
            get { return discountPercentageField; }
            set { discountPercentageField = value; }
        }

        /// <remarks/>
        public string DiscountText
        {
            get { return discountTextField; }
            set { discountTextField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum RecurrenceDurationType
    {

        /// <remarks/>
        Infinitely,

        /// <remarks/>
        NumberOfCycles,

        /// <remarks/>
        EndDate,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OfferCycleBillOfMaterials : PMObject
    {

        private int? seqIDField;

        private int? sequenceField;

        private DateTime? cycleDateField;

        private OfferID cBOfferField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Sequence
        {
            get { return sequenceField; }
            set { sequenceField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? CycleDate
        {
            get { return cycleDateField; }
            set { cycleDateField = value; }
        }

        /// <remarks/>
        public OfferID CBOffer
        {
            get { return cBOfferField; }
            set { cBOfferField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class WarehouseLevels : PMObject
    {

        private PMSystem warehouseField;

        private string partNumberField;

        private string partDescriptionField;

        private string sizeField;

        private string colorField;

        private BuildType productTypeField;

        private int onHandField;

        private int reservedField;

        private int markedField;

        private int backorderedField;

        private int neededField;

        private int unavailableField;

        private int expectedField;

        private int onOrderField;

        private int availableField;

        private int totalReservedField;

        /// <remarks/>
        public PMSystem Warehouse
        {
            get { return warehouseField; }
            set { warehouseField = value; }
        }

        /// <remarks/>
        public string PartNumber
        {
            get { return partNumberField; }
            set { partNumberField = value; }
        }

        /// <remarks/>
        public string PartDescription
        {
            get { return partDescriptionField; }
            set { partDescriptionField = value; }
        }

        /// <remarks/>
        public string Size
        {
            get { return sizeField; }
            set { sizeField = value; }
        }

        /// <remarks/>
        public string Color
        {
            get { return colorField; }
            set { colorField = value; }
        }

        /// <remarks/>
        public BuildType ProductType
        {
            get { return productTypeField; }
            set { productTypeField = value; }
        }

        /// <remarks/>
        public int OnHand
        {
            get { return onHandField; }
            set { onHandField = value; }
        }

        /// <remarks/>
        public int Reserved
        {
            get { return reservedField; }
            set { reservedField = value; }
        }

        /// <remarks/>
        public int Marked
        {
            get { return markedField; }
            set { markedField = value; }
        }

        /// <remarks/>
        public int Backordered
        {
            get { return backorderedField; }
            set { backorderedField = value; }
        }

        /// <remarks/>
        public int Needed
        {
            get { return neededField; }
            set { neededField = value; }
        }

        /// <remarks/>
        public int Unavailable
        {
            get { return unavailableField; }
            set { unavailableField = value; }
        }

        /// <remarks/>
        public int Expected
        {
            get { return expectedField; }
            set { expectedField = value; }
        }

        /// <remarks/>
        public int OnOrder
        {
            get { return onOrderField; }
            set { onOrderField = value; }
        }

        /// <remarks/>
        public int Available
        {
            get { return availableField; }
            set { availableField = value; }
        }

        /// <remarks/>
        public int TotalReserved
        {
            get { return totalReservedField; }
            set { totalReservedField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ProductAvailabilities : PMObject
    {

        private WarehouseLevels[] warehousesField;

        /// <remarks/>
        public WarehouseLevels[] Warehouses
        {
            get { return warehousesField; }
            set { warehousesField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OrderBudget : PMObject
    {

        private Person personField;

        /// <remarks/>
        public Person Person
        {
            get { return personField; }
            set { personField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class Person : PMObject
    {

        private PersonName nameField;

        private PersonCompany companyInfoField;

        private PersonAddress addressField;

        private PersonContact contactInfoField;

        private PersonBilling billingInfoField;

        private PersonVariable[] variablesField;

        /// <remarks/>
        public PersonName Name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks/>
        public PersonCompany CompanyInfo
        {
            get { return companyInfoField; }
            set { companyInfoField = value; }
        }

        /// <remarks/>
        public PersonAddress Address
        {
            get { return addressField; }
            set { addressField = value; }
        }

        /// <remarks/>
        public PersonContact ContactInfo
        {
            get { return contactInfoField; }
            set { contactInfoField = value; }
        }

        /// <remarks/>
        public PersonBilling BillingInfo
        {
            get { return billingInfoField; }
            set { billingInfoField = value; }
        }

        /// <remarks/>
        public PersonVariable[] Variables
        {
            get { return variablesField; }
            set { variablesField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class PersonName : PMObject
    {

        private string uIDField;

        private string prefixField;

        private string firstNameField;

        private string middleNameField;

        private string lastNameField;

        private string suffixField;

        private string fullNameField;

        /// <remarks/>
        public string UID
        {
            get { return uIDField; }
            set { uIDField = value; }
        }

        /// <remarks/>
        public string Prefix
        {
            get { return prefixField; }
            set { prefixField = value; }
        }

        /// <remarks/>
        public string FirstName
        {
            get { return firstNameField; }
            set { firstNameField = value; }
        }

        /// <remarks/>
        public string MiddleName
        {
            get { return middleNameField; }
            set { middleNameField = value; }
        }

        /// <remarks/>
        public string LastName
        {
            get { return lastNameField; }
            set { lastNameField = value; }
        }

        /// <remarks/>
        public string Suffix
        {
            get { return suffixField; }
            set { suffixField = value; }
        }

        /// <remarks/>
        public string FullName
        {
            get { return fullNameField; }
            set { fullNameField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class PersonCompany : PMObject
    {

        private string nameField;

        private string titleField;

        private int? storeField;

        private ShippingOption defaultShippingOptionField;

        /// <remarks/>
        public string Name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks/>
        public string Title
        {
            get { return titleField; }
            set { titleField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Store
        {
            get { return storeField; }
            set { storeField = value; }
        }

        /// <remarks/>
        public ShippingOption DefaultShippingOption
        {
            get { return defaultShippingOptionField; }
            set { defaultShippingOptionField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class PersonAddress : PMObject
    {

        private string address1Field;

        private string address2Field;

        private string address3Field;

        private string cityField;

        private string stateField;

        private string postalCodeField;

        private string countryField;

        private bool? commercialField;

        private string cityStateZipField;

        private string cityStateZipCountryField;

        private string compoundAddressField;

        /// <remarks/>
        public string Address1
        {
            get { return address1Field; }
            set { address1Field = value; }
        }

        /// <remarks/>
        public string Address2
        {
            get { return address2Field; }
            set { address2Field = value; }
        }

        /// <remarks/>
        public string Address3
        {
            get { return address3Field; }
            set { address3Field = value; }
        }

        /// <remarks/>
        public string City
        {
            get { return cityField; }
            set { cityField = value; }
        }

        /// <remarks/>
        public string State
        {
            get { return stateField; }
            set { stateField = value; }
        }

        /// <remarks/>
        public string PostalCode
        {
            get { return postalCodeField; }
            set { postalCodeField = value; }
        }

        /// <remarks/>
        public string Country
        {
            get { return countryField; }
            set { countryField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? Commercial
        {
            get { return commercialField; }
            set { commercialField = value; }
        }

        /// <remarks/>
        public string CityStateZip
        {
            get { return cityStateZipField; }
            set { cityStateZipField = value; }
        }

        /// <remarks/>
        public string CityStateZipCountry
        {
            get { return cityStateZipCountryField; }
            set { cityStateZipCountryField = value; }
        }

        /// <remarks/>
        public string CompoundAddress
        {
            get { return compoundAddressField; }
            set { compoundAddressField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class PersonContact : PMObject
    {

        private string phoneField;

        private string faxField;

        private string emailField;

        /// <remarks/>
        public string Phone
        {
            get { return phoneField; }
            set { phoneField = value; }
        }

        /// <remarks/>
        public string Fax
        {
            get { return faxField; }
            set { faxField = value; }
        }

        /// <remarks/>
        public string Email
        {
            get { return emailField; }
            set { emailField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class PersonBilling : PMObject
    {

        private bool? taxExemptField;

        private string taxExemptIDField;

        private bool? taxExemptApprovedField;

        private RestrictionType restrictionType1Field;

        private RestrictionType restrictionType2Field;

        private PriceClass priceClassField;

        private MailerClass mailerClassField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? TaxExempt
        {
            get { return taxExemptField; }
            set { taxExemptField = value; }
        }

        /// <remarks/>
        public string TaxExemptID
        {
            get { return taxExemptIDField; }
            set { taxExemptIDField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? TaxExemptApproved
        {
            get { return taxExemptApprovedField; }
            set { taxExemptApprovedField = value; }
        }

        /// <remarks/>
        public RestrictionType RestrictionType1
        {
            get { return restrictionType1Field; }
            set { restrictionType1Field = value; }
        }

        /// <remarks/>
        public RestrictionType RestrictionType2
        {
            get { return restrictionType2Field; }
            set { restrictionType2Field = value; }
        }

        /// <remarks/>
        public PriceClass PriceClass
        {
            get { return priceClassField; }
            set { priceClassField = value; }
        }

        /// <remarks/>
        public MailerClass MailerClass
        {
            get { return mailerClassField; }
            set { mailerClassField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class MailerClass : PMObject
    {

        private int? seqIDField;

        private string descriptionField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class PersonVariable : PMObject
    {

        private VariableField variableFieldField;

        private string valueField;

        private string valueDescriptionField;

        /// <remarks/>
        public VariableField VariableField
        {
            get { return variableFieldField; }
            set { variableFieldField = value; }
        }

        /// <remarks/>
        public string Value
        {
            get { return valueField; }
            set { valueField = value; }
        }

        /// <remarks/>
        public string ValueDescription
        {
            get { return valueDescriptionField; }
            set { valueDescriptionField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OrderRecurrenceSchedule : PMObject
    {

        private RecurrenceSchedules recurrenceSchedulesField;

        private bool? recurrenceOfferFlagField;

        private ShippingOption recurrenceShippingOptionField;

        private System.Nullable<decimal> recurrenceSpecialHandlingChargeField;

        private int? numberRecurrenceField;

        /// <remarks/>
        public RecurrenceSchedules RecurrenceSchedules
        {
            get { return recurrenceSchedulesField; }
            set { recurrenceSchedulesField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? RecurrenceOfferFlag
        {
            get { return recurrenceOfferFlagField; }
            set { recurrenceOfferFlagField = value; }
        }

        /// <remarks/>
        public ShippingOption RecurrenceShippingOption
        {
            get { return recurrenceShippingOptionField; }
            set { recurrenceShippingOptionField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> RecurrenceSpecialHandlingCharge
        {
            get { return recurrenceSpecialHandlingChargeField; }
            set { recurrenceSpecialHandlingChargeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? NumberRecurrence
        {
            get { return numberRecurrenceField; }
            set { numberRecurrenceField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OfferVariable : PMObject
    {

        private int? seqIDField;

        private VariableField variableFieldField;

        private string valueField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public VariableField VariableField
        {
            get { return variableFieldField; }
            set { variableFieldField = value; }
        }

        /// <remarks/>
        public string Value
        {
            get { return valueField; }
            set { valueField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OfferOrdered : PMObject
    {

        private OfferID offerField;

        private int? quantityField;

        private int? canceledQuantityField;

        private OrderShipToKey orderShipToField;

        private OrderShipToKey orderShipToKeyField;

        private System.Nullable<byte> priceTypeField;

        private System.Nullable<decimal> unitPriceField;

        private System.Nullable<short> shipTypeField;

        private System.Nullable<decimal> shippingHandlingField;

        private System.Nullable<decimal> discountsField;

        private int? discountPercentField;

        private string documentIDField;

        private int? seqIDField;

        private int? cloneLineField;

        private bool? unapprovedField;

        private string shipToKeyField;

        private string fgnOrderField;

        private string commentsField;

        private string rCOrderKeyField;

        private bool? recurringField;

        private System.Nullable<long> lineNumberField;

        private OfferVariable[] variablesField;

        private OrderProductDetail[] productDetailsField;

        private System.Nullable<decimal> lineTaxPercentField;

        private System.Nullable<decimal> shippingHandlingTaxPercentField;

        private System.Nullable<decimal> lineTaxAmountField;

        /// <remarks/>
        public OfferID Offer
        {
            get { return offerField; }
            set { offerField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Quantity
        {
            get { return quantityField; }
            set { quantityField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? CanceledQuantity
        {
            get { return canceledQuantityField; }
            set { canceledQuantityField = value; }
        }

        /// <remarks/>
        public OrderShipToKey OrderShipTo
        {
            get { return orderShipToField; }
            set { orderShipToField = value; }
        }

        /// <remarks/>
        public OrderShipToKey OrderShipToKey
        {
            get { return orderShipToKeyField; }
            set { orderShipToKeyField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<byte> PriceType
        {
            get { return priceTypeField; }
            set { priceTypeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> UnitPrice
        {
            get { return unitPriceField; }
            set { unitPriceField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<short> ShipType
        {
            get { return shipTypeField; }
            set { shipTypeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> ShippingHandling
        {
            get { return shippingHandlingField; }
            set { shippingHandlingField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> Discounts
        {
            get { return discountsField; }
            set { discountsField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? DiscountPercent
        {
            get { return discountPercentField; }
            set { discountPercentField = value; }
        }

        /// <remarks/>
        public string DocumentID
        {
            get { return documentIDField; }
            set { documentIDField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? CloneLine
        {
            get { return cloneLineField; }
            set { cloneLineField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? Unapproved
        {
            get { return unapprovedField; }
            set { unapprovedField = value; }
        }

        /// <remarks/>
        public string ShipToKey
        {
            get { return shipToKeyField; }
            set { shipToKeyField = value; }
        }

        /// <remarks/>
        public string FgnOrder
        {
            get { return fgnOrderField; }
            set { fgnOrderField = value; }
        }

        /// <remarks/>
        public string Comments
        {
            get { return commentsField; }
            set { commentsField = value; }
        }

        /// <remarks/>
        public string RCOrderKey
        {
            get { return rCOrderKeyField; }
            set { rCOrderKeyField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? Recurring
        {
            get { return recurringField; }
            set { recurringField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<long> LineNumber
        {
            get { return lineNumberField; }
            set { lineNumberField = value; }
        }

        /// <remarks/>
        public OfferVariable[] Variables
        {
            get { return variablesField; }
            set { variablesField = value; }
        }

        /// <remarks/>
        public OrderProductDetail[] ProductDetails
        {
            get { return productDetailsField; }
            set { productDetailsField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> LineTaxPercent
        {
            get { return lineTaxPercentField; }
            set { lineTaxPercentField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> ShippingHandlingTaxPercent
        {
            get { return shippingHandlingTaxPercentField; }
            set { shippingHandlingTaxPercentField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> LineTaxAmount
        {
            get { return lineTaxAmountField; }
            set { lineTaxAmountField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OrderShipToKey
    {

        private string keyField;

        /// <remarks/>
        public string Key
        {
            get { return keyField; }
            set { keyField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OrderProductDetail
    {

        private string clusterPartNumberField;

        private string partNumberField;

        private string sizeField;

        private string colorField;

        private int? quantityFactorField;

        private int? quantityField;

        /// <remarks/>
        public string ClusterPartNumber
        {
            get { return clusterPartNumberField; }
            set { clusterPartNumberField = value; }
        }

        /// <remarks/>
        public string PartNumber
        {
            get { return partNumberField; }
            set { partNumberField = value; }
        }

        /// <remarks/>
        public string Size
        {
            get { return sizeField; }
            set { sizeField = value; }
        }

        /// <remarks/>
        public string Color
        {
            get { return colorField; }
            set { colorField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? QuantityFactor
        {
            get { return quantityFactorField; }
            set { quantityFactorField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Quantity
        {
            get { return quantityField; }
            set { quantityField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class FreightCode : PMObject
    {

        private int? seqIDField;

        private string carrierCodeField;

        private string descriptionField;

        private FreightCarrier freightCarrierField;

        private FreightService freightServiceField;

        private System.Nullable<short> defaultField;

        private string sCACCodeField;

        private bool? continentalUSField;

        private bool? canadaField;

        private bool? mexicoField;

        private bool? alaskaField;

        private bool? hawaiiField;

        private bool? puertoRicoField;

        private bool? internationalField;

        private int? deliveryDaysField;

        private DateTime? guaranteedDeliveryTimeField;

        private System.Nullable<WeightType> weightTypeField;

        private System.Nullable<double> minimumWeightField;

        private System.Nullable<double> maximumWeightField;

        private System.Nullable<double> maxDimensionalWeightField;

        private bool? metricField;

        private System.Nullable<double> maximumWidthField;

        private System.Nullable<double> maximumLengthField;

        private System.Nullable<double> maximumHeightField;

        private System.Nullable<double> maximumLengthGirthField;

        private bool? commercialField;

        private bool? residentialField;

        private bool? ruralRoutesField;

        private bool? postOfficeBoxesField;

        private bool? saturdayDeliveryField;

        private bool? activeField;

        private bool? promailDistributedField;

        private string systemIDField;

        private string xmlServiceTypeField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string CarrierCode
        {
            get { return carrierCodeField; }
            set { carrierCodeField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        public FreightCarrier FreightCarrier
        {
            get { return freightCarrierField; }
            set { freightCarrierField = value; }
        }

        /// <remarks/>
        public FreightService FreightService
        {
            get { return freightServiceField; }
            set { freightServiceField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<short> Default
        {
            get { return defaultField; }
            set { defaultField = value; }
        }

        /// <remarks/>
        public string SCACCode
        {
            get { return sCACCodeField; }
            set { sCACCodeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? ContinentalUS
        {
            get { return continentalUSField; }
            set { continentalUSField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? Canada
        {
            get { return canadaField; }
            set { canadaField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? Mexico
        {
            get { return mexicoField; }
            set { mexicoField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? Alaska
        {
            get { return alaskaField; }
            set { alaskaField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? Hawaii
        {
            get { return hawaiiField; }
            set { hawaiiField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? PuertoRico
        {
            get { return puertoRicoField; }
            set { puertoRicoField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? International
        {
            get { return internationalField; }
            set { internationalField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? DeliveryDays
        {
            get { return deliveryDaysField; }
            set { deliveryDaysField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? GuaranteedDeliveryTime
        {
            get { return guaranteedDeliveryTimeField; }
            set { guaranteedDeliveryTimeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<WeightType> WeightType
        {
            get { return weightTypeField; }
            set { weightTypeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<double> MinimumWeight
        {
            get { return minimumWeightField; }
            set { minimumWeightField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<double> MaximumWeight
        {
            get { return maximumWeightField; }
            set { maximumWeightField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<double> MaxDimensionalWeight
        {
            get { return maxDimensionalWeightField; }
            set { maxDimensionalWeightField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? Metric
        {
            get { return metricField; }
            set { metricField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<double> MaximumWidth
        {
            get { return maximumWidthField; }
            set { maximumWidthField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<double> MaximumLength
        {
            get { return maximumLengthField; }
            set { maximumLengthField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<double> MaximumHeight
        {
            get { return maximumHeightField; }
            set { maximumHeightField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<double> MaximumLengthGirth
        {
            get { return maximumLengthGirthField; }
            set { maximumLengthGirthField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? Commercial
        {
            get { return commercialField; }
            set { commercialField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? Residential
        {
            get { return residentialField; }
            set { residentialField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? RuralRoutes
        {
            get { return ruralRoutesField; }
            set { ruralRoutesField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? PostOfficeBoxes
        {
            get { return postOfficeBoxesField; }
            set { postOfficeBoxesField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? SaturdayDelivery
        {
            get { return saturdayDeliveryField; }
            set { saturdayDeliveryField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? Active
        {
            get { return activeField; }
            set { activeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? PromailDistributed
        {
            get { return promailDistributedField; }
            set { promailDistributedField = value; }
        }

        /// <remarks/>
        public string SystemID
        {
            get { return systemIDField; }
            set { systemIDField = value; }
        }

        /// <remarks/>
        public string XmlServiceType
        {
            get { return xmlServiceTypeField; }
            set { xmlServiceTypeField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class FreightCarrier : PMObject
    {

        private string nameField;

        /// <remarks/>
        public string Name
        {
            get { return nameField; }
            set { nameField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class FreightService : PMObject
    {

        private string descriptionField;

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class PickPack : PMObject
    {

        private int? seqIDField;

        private OrderShipTo orderShipToField;

        private int? pPSBAT_SeqidField;

        private DateTime? pickPackageDateTimeField;

        private string systemIDField;

        private string statusField;

        private DateTime? messagedDateField;

        private string shipOrderIDField;

        private DateTime? pickedDateField;

        private System.Nullable<decimal> merAmtField;

        private System.Nullable<decimal> shipHandField;

        private System.Nullable<decimal> taxAmtField;

        private System.Nullable<decimal> pICPAK_DiscAmtField;

        private bool? holdField;

        private int? cREDBT_SeqidField;

        private System.Nullable<decimal> specHandField;

        private System.Nullable<decimal> addlChargeField;

        private int? sUPPLR_SeqidField;

        private bool? shpConfEmailField;

        private bool? needsInvoiceField;

        private string sOTYPEField;

        private System.Nullable<decimal> creditAmountField;

        private System.Nullable<decimal> gCAmtField;

        private string pICPAK_TaxTranIDField;

        private bool? chargeCompleteField;

        private System.Nullable<decimal> offerShipHandField;

        private System.Nullable<decimal> rushHandField;

        private System.Nullable<decimal> packChargesField;

        private System.Nullable<decimal> shipChargesField;

        private System.Nullable<decimal> nCAmountField;

        private System.Nullable<decimal> nCUsedField;

        private System.Nullable<decimal> budgetAmountField;

        private FreightCode freightCodeField;

        private string interceptCommentsField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public OrderShipTo OrderShipTo
        {
            get { return orderShipToField; }
            set { orderShipToField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? PPSBAT_Seqid
        {
            get { return pPSBAT_SeqidField; }
            set { pPSBAT_SeqidField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? PickPackageDateTime
        {
            get { return pickPackageDateTimeField; }
            set { pickPackageDateTimeField = value; }
        }

        /// <remarks/>
        public string SystemID
        {
            get { return systemIDField; }
            set { systemIDField = value; }
        }

        /// <remarks/>
        public string Status
        {
            get { return statusField; }
            set { statusField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? MessagedDate
        {
            get { return messagedDateField; }
            set { messagedDateField = value; }
        }

        /// <remarks/>
        public string ShipOrderID
        {
            get { return shipOrderIDField; }
            set { shipOrderIDField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? PickedDate
        {
            get { return pickedDateField; }
            set { pickedDateField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> MerAmt
        {
            get { return merAmtField; }
            set { merAmtField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> ShipHand
        {
            get { return shipHandField; }
            set { shipHandField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> TaxAmt
        {
            get { return taxAmtField; }
            set { taxAmtField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> PICPAK_DiscAmt
        {
            get { return pICPAK_DiscAmtField; }
            set { pICPAK_DiscAmtField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? Hold
        {
            get { return holdField; }
            set { holdField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? CREDBT_Seqid
        {
            get { return cREDBT_SeqidField; }
            set { cREDBT_SeqidField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> SpecHand
        {
            get { return specHandField; }
            set { specHandField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> AddlCharge
        {
            get { return addlChargeField; }
            set { addlChargeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SUPPLR_Seqid
        {
            get { return sUPPLR_SeqidField; }
            set { sUPPLR_SeqidField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? ShpConfEmail
        {
            get { return shpConfEmailField; }
            set { shpConfEmailField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? NeedsInvoice
        {
            get { return needsInvoiceField; }
            set { needsInvoiceField = value; }
        }

        /// <remarks/>
        public string SOTYPE
        {
            get { return sOTYPEField; }
            set { sOTYPEField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> CreditAmount
        {
            get { return creditAmountField; }
            set { creditAmountField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> GCAmt
        {
            get { return gCAmtField; }
            set { gCAmtField = value; }
        }

        /// <remarks/>
        public string PICPAK_TaxTranID
        {
            get { return pICPAK_TaxTranIDField; }
            set { pICPAK_TaxTranIDField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? ChargeComplete
        {
            get { return chargeCompleteField; }
            set { chargeCompleteField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> OfferShipHand
        {
            get { return offerShipHandField; }
            set { offerShipHandField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> RushHand
        {
            get { return rushHandField; }
            set { rushHandField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> PackCharges
        {
            get { return packChargesField; }
            set { packChargesField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> ShipCharges
        {
            get { return shipChargesField; }
            set { shipChargesField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> NCAmount
        {
            get { return nCAmountField; }
            set { nCAmountField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> NCUsed
        {
            get { return nCUsedField; }
            set { nCUsedField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> BudgetAmount
        {
            get { return budgetAmountField; }
            set { budgetAmountField = value; }
        }

        /// <remarks/>
        public FreightCode FreightCode
        {
            get { return freightCodeField; }
            set { freightCodeField = value; }
        }

        /// <remarks/>
        public string InterceptComments
        {
            get { return interceptCommentsField; }
            set { interceptCommentsField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OrderShipTo : OrderMailer
    {

        private int? seqIDField;

        private ShipToFlag flagField;

        private string keyField;

        private string neededByField;

        private DateTime? releaseDateField;

        private bool? rushField;

        private System.Nullable<decimal> rushHandlingField;

        private string commentsField;

        private FreightCarrier freightCarrierField;

        private FreightService freightServiceField;

        private int? thirdPartyTypeField;

        private string thirdPartyAccountNumberField;

        private string freightCodeField;

        private string freightCodeDescriptionField;

        private ShippingOption specialHandlingField;

        private System.Nullable<decimal> specialHandlingChargeField;

        private System.Nullable<decimal> shippingHandlingChargeField;

        private string fullNameField;

        private string fullNameWithSuffixField;

        private string cityStateZipField;

        private string cityStateZipCountryField;

        private string compoundAddressField;

        private PickPack[] pickPacksField;

        private System.Nullable<decimal> shippingHandlingTaxAmountField;

        private System.Nullable<decimal> shippingHandlingTaxPercentField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public ShipToFlag Flag
        {
            get { return flagField; }
            set { flagField = value; }
        }

        /// <remarks/>
        public string Key
        {
            get { return keyField; }
            set { keyField = value; }
        }

        /// <remarks/>
        public string NeededBy
        {
            get { return neededByField; }
            set { neededByField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? ReleaseDate
        {
            get { return releaseDateField; }
            set { releaseDateField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? Rush
        {
            get { return rushField; }
            set { rushField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> RushHandling
        {
            get { return rushHandlingField; }
            set { rushHandlingField = value; }
        }

        /// <remarks/>
        [Map("DeliveryInstructions")]
        public string Comments
        {
            get { return commentsField; }
            set { commentsField = value; }
        }

        /// <remarks/>
        public FreightCarrier FreightCarrier
        {
            get { return freightCarrierField; }
            set { freightCarrierField = value; }
        }

        /// <remarks/>
        public FreightService FreightService
        {
            get { return freightServiceField; }
            set { freightServiceField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? ThirdPartyType
        {
            get { return thirdPartyTypeField; }
            set { thirdPartyTypeField = value; }
        }

        /// <remarks/>
        public string ThirdPartyAccountNumber
        {
            get { return thirdPartyAccountNumberField; }
            set { thirdPartyAccountNumberField = value; }
        }

        /// <remarks/>
        public string FreightCode
        {
            get { return freightCodeField; }
            set { freightCodeField = value; }
        }

        /// <remarks/>
        public string FreightCodeDescription
        {
            get { return freightCodeDescriptionField; }
            set { freightCodeDescriptionField = value; }
        }

        /// <remarks/>
        public ShippingOption SpecialHandling
        {
            get { return specialHandlingField; }
            set { specialHandlingField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> SpecialHandlingCharge
        {
            get { return specialHandlingChargeField; }
            set { specialHandlingChargeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> ShippingHandlingCharge
        {
            get { return shippingHandlingChargeField; }
            set { shippingHandlingChargeField = value; }
        }

        /// <remarks/>
        public string FullName
        {
            get { return fullNameField; }
            set { fullNameField = value; }
        }

        /// <remarks/>
        public string FullNameWithSuffix
        {
            get { return fullNameWithSuffixField; }
            set { fullNameWithSuffixField = value; }
        }

        /// <remarks/>
        public string CityStateZip
        {
            get { return cityStateZipField; }
            set { cityStateZipField = value; }
        }

        /// <remarks/>
        public string CityStateZipCountry
        {
            get { return cityStateZipCountryField; }
            set { cityStateZipCountryField = value; }
        }

        /// <remarks/>
        public string CompoundAddress
        {
            get { return compoundAddressField; }
            set { compoundAddressField = value; }
        }

        /// <remarks/>
        public PickPack[] PickPacks
        {
            get { return pickPacksField; }
            set { pickPacksField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> ShippingHandlingTaxAmount
        {
            get { return shippingHandlingTaxAmountField; }
            set { shippingHandlingTaxAmountField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> ShippingHandlingTaxPercent
        {
            get { return shippingHandlingTaxPercentField; }
            set { shippingHandlingTaxPercentField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum ShipToFlag
    {

        /// <remarks/>
        Other,

        /// <remarks/>
        OrderedBy,
    }

    /// <remarks/>
    [XmlInclude(typeof(OrderShipTo))]
    [XmlInclude(typeof(OrderBillTo))]
    [XmlInclude(typeof(OrderedBy))]
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OrderMailer : PMObject
    {

        private string prefixField;

        private string firstNameField;

        private string middleInitialField;

        private string lastNameField;

        private string suffixField;

        private string companyNameField;

        private string titleField;

        private string address1Field;

        private string address2Field;

        private string address3Field;

        private string cityField;

        private string stateField;

        private string postalCodeField;

        private string countryField;

        private string phoneField;

        private string faxField;

        private string emailField;

        private string uIDField;

        private bool? taxExemptField;

        private string taxExemptIDField;

        private bool? taxExemptApprovedField;

        private bool? commercialField;

        private PersonVariable[] variablesField;

        /// <remarks/>
        public string Prefix
        {
            get { return prefixField; }
            set { prefixField = value; }
        }

        /// <remarks/>
        public string FirstName
        {
            get { return firstNameField; }
            set { firstNameField = value; }
        }

        /// <remarks/>
        public string MiddleInitial
        {
            get { return middleInitialField; }
            set { middleInitialField = value; }
        }

        /// <remarks/>
        public string LastName
        {
            get { return lastNameField; }
            set { lastNameField = value; }
        }

        /// <remarks/>
        public string Suffix
        {
            get { return suffixField; }
            set { suffixField = value; }
        }

        /// <remarks/>
        [Map("Company")]
        public string CompanyName
        {
            get { return companyNameField; }
            set { companyNameField = value; }
        }

        /// <remarks/>
        public string Title
        {
            get { return titleField; }
            set { titleField = value; }
        }

        /// <remarks/>
        [Map]
        public string Address1
        {
            get { return address1Field; }
            set { address1Field = value; }
        }

        /// <remarks/>
        [Map]
        public string Address2
        {
            get { return address2Field; }
            set { address2Field = value; }
        }

        /// <remarks/>
        public string Address3
        {
            get { return address3Field; }
            set { address3Field = value; }
        }

        /// <remarks/>
        [Map]
        public string City
        {
            get { return cityField; }
            set { cityField = value; }
        }

        /// <remarks/>
        public string State
        {
            get { return stateField; }
            set { stateField = value; }
        }

        /// <remarks/>
        [Map("Zip")]
        public string PostalCode
        {
            get { return postalCodeField; }
            set { postalCodeField = value; }
        }

        /// <remarks/>
        public string Country
        {
            get { return countryField; }
            set { countryField = value; }
        }

        /// <remarks/>
        [Map]
        public string Phone
        {
            get { return phoneField; }
            set { phoneField = value; }
        }

        /// <remarks/>
        [Map]
        public string Fax
        {
            get { return faxField; }
            set { faxField = value; }
        }

        /// <remarks/>
        [Map]
        public string Email
        {
            get { return emailField; }
            set { emailField = value; }
        }

        /// <remarks/>
        public string UID
        {
            get { return uIDField; }
            set { uIDField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? TaxExempt
        {
            get { return taxExemptField; }
            set { taxExemptField = value; }
        }

        /// <remarks/>
        public string TaxExemptID
        {
            get { return taxExemptIDField; }
            set { taxExemptIDField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? TaxExemptApproved
        {
            get { return taxExemptApprovedField; }
            set { taxExemptApprovedField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? Commercial
        {
            get { return commercialField; }
            set { commercialField = value; }
        }

        /// <remarks/>
        public PersonVariable[] Variables
        {
            get { return variablesField; }
            set { variablesField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OrderBillTo : OrderMailer
    {

        private BillToFlag flagField;

        private string fullNameField;

        private string fullNameWithSuffixField;

        private string cityStateZipField;

        private string cityStateZipCountryField;

        private string compoundAddressField;

        /// <remarks/>
        public BillToFlag Flag
        {
            get { return flagField; }
            set { flagField = value; }
        }

        /// <remarks/>
        public string FullName
        {
            get { return fullNameField; }
            set { fullNameField = value; }
        }

        /// <remarks/>
        public string FullNameWithSuffix
        {
            get { return fullNameWithSuffixField; }
            set { fullNameWithSuffixField = value; }
        }

        /// <remarks/>
        public string CityStateZip
        {
            get { return cityStateZipField; }
            set { cityStateZipField = value; }
        }

        /// <remarks/>
        public string CityStateZipCountry
        {
            get { return cityStateZipCountryField; }
            set { cityStateZipCountryField = value; }
        }

        /// <remarks/>
        public string CompoundAddress
        {
            get { return compoundAddressField; }
            set { compoundAddressField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum BillToFlag
    {

        /// <remarks/>
        Other,

        /// <remarks/>
        OrderedBy,

        /// <remarks/>
        DoNotUse,

        /// <remarks/>
        ShipTo,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OrderedBy : OrderMailer
    {

        private ORDOBYID oRDOBYField;

        private string fullNameField;

        private string fullNameWithSuffixField;

        private string cityStateZipField;

        private string cityStateZipCountryField;

        private string compoundAddressField;

        /// <remarks/>
        public ORDOBYID ORDOBY
        {
            get { return oRDOBYField; }
            set { oRDOBYField = value; }
        }

        /// <remarks/>
        public string FullName
        {
            get { return fullNameField; }
            set { fullNameField = value; }
        }

        /// <remarks/>
        public string FullNameWithSuffix
        {
            get { return fullNameWithSuffixField; }
            set { fullNameWithSuffixField = value; }
        }

        /// <remarks/>
        public string CityStateZip
        {
            get { return cityStateZipField; }
            set { cityStateZipField = value; }
        }

        /// <remarks/>
        public string CityStateZipCountry
        {
            get { return cityStateZipCountryField; }
            set { cityStateZipCountryField = value; }
        }

        /// <remarks/>
        public string CompoundAddress
        {
            get { return compoundAddressField; }
            set { compoundAddressField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ORDOBYID : PMObject
    {

        private int? seqIDField;

        private string customerIDField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string CustomerID
        {
            get { return customerIDField; }
            set { customerIDField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OrderVariable : PMObject
    {

        private int? seqIDField;

        private VariableField variableFieldField;

        private string valueField;

        private string valueDescriptionField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public VariableField VariableField
        {
            get { return variableFieldField; }
            set { variableFieldField = value; }
        }

        /// <remarks/>
        public string Value
        {
            get { return valueField; }
            set { valueField = value; }
        }

        /// <remarks/>
        public string ValueDescription
        {
            get { return valueDescriptionField; }
            set { valueDescriptionField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class PaymentType : PMObject
    {

        private string descriptionField;

        private int? sequenceField;

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Sequence
        {
            get { return sequenceField; }
            set { sequenceField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OrderPayment : PMObject
    {

        private PaymentType paymentTypeField;

        private System.Nullable<decimal> paymentAmountField;

        private string cCNumberField;

        private string cCExpirationDateField;

        private string cSCField;

        private string aRReferenceField;

        private string tokenField;

        private string transactionIDField;

        private string authorizationCodeField;

        private System.Nullable<decimal> authorizationAmountField;

        private DateTime? authorizationDateField;

        /// <remarks/>
        public PaymentType PaymentType
        {
            get { return paymentTypeField; }
            set { paymentTypeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> PaymentAmount
        {
            get { return paymentAmountField; }
            set { paymentAmountField = value; }
        }

        /// <remarks/>
        public string CCNumber
        {
            get { return cCNumberField; }
            set { cCNumberField = value; }
        }

        /// <remarks/>
        public string CCExpirationDate
        {
            get { return cCExpirationDateField; }
            set { cCExpirationDateField = value; }
        }

        /// <remarks/>
        public string CSC
        {
            get { return cSCField; }
            set { cSCField = value; }
        }

        /// <remarks/>
        public string ARReference
        {
            get { return aRReferenceField; }
            set { aRReferenceField = value; }
        }

        /// <remarks/>
        public string Token
        {
            get { return tokenField; }
            set { tokenField = value; }
        }

        /// <remarks/>
        public string TransactionID
        {
            get { return transactionIDField; }
            set { transactionIDField = value; }
        }

        /// <remarks/>
        public string AuthorizationCode
        {
            get { return authorizationCodeField; }
            set { authorizationCodeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> AuthorizationAmount
        {
            get { return authorizationAmountField; }
            set { authorizationAmountField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? AuthorizationDate
        {
            get { return authorizationDateField; }
            set { authorizationDateField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class GiftCertificate : PMObject
    {

        private string uIDField;

        private decimal amountField;

        /// <remarks/>
        public string UID
        {
            get { return uIDField; }
            set { uIDField = value; }
        }

        /// <remarks/>
        public decimal Amount
        {
            get { return amountField; }
            set { amountField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class Coupon : PMObject
    {

        private string codeField;

        /// <remarks/>
        public string Code
        {
            get { return codeField; }
            set { codeField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class DiscountCode : PMObject
    {

        private string idField;

        private string descriptionField;

        /// <remarks/>
        public string ID
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class NoChargeType : PMObject
    {

        private string descriptionField;

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OrderMoney : PMObject
    {

        private PriceClass priceClassField;

        private System.Nullable<decimal> shippingHandlingChargeField;

        private System.Nullable<decimal> rushHandlingChargeField;

        private NoChargeType noChargeTypeField;

        private System.Nullable<decimal> discountAmountField;

        private System.Nullable<float> discountPercentField;

        private DiscountCode discountCodeField;

        private Coupon couponField;

        private System.Nullable<decimal> specialHandlingChargeField;

        private System.Nullable<decimal> creditAmountField;

        private GiftCertificate giftCertificateField;

        private System.Nullable<decimal> giftCertificateAmountField;

        private System.Nullable<decimal> taxPercentField;

        private System.Nullable<decimal> noChargeAmountField;

        private System.Nullable<decimal> taxAmountField;

        /// <remarks/>
        public PriceClass PriceClass
        {
            get { return priceClassField; }
            set { priceClassField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> ShippingHandlingCharge
        {
            get { return shippingHandlingChargeField; }
            set { shippingHandlingChargeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> RushHandlingCharge
        {
            get { return rushHandlingChargeField; }
            set { rushHandlingChargeField = value; }
        }

        /// <remarks/>
        public NoChargeType NoChargeType
        {
            get { return noChargeTypeField; }
            set { noChargeTypeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> DiscountAmount
        {
            get { return discountAmountField; }
            set { discountAmountField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<float> DiscountPercent
        {
            get { return discountPercentField; }
            set { discountPercentField = value; }
        }

        /// <remarks/>
        public DiscountCode DiscountCode
        {
            get { return discountCodeField; }
            set { discountCodeField = value; }
        }

        /// <remarks/>
        public Coupon Coupon
        {
            get { return couponField; }
            set { couponField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> SpecialHandlingCharge
        {
            get { return specialHandlingChargeField; }
            set { specialHandlingChargeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> CreditAmount
        {
            get { return creditAmountField; }
            set { creditAmountField = value; }
        }

        /// <remarks/>
        public GiftCertificate GiftCertificate
        {
            get { return giftCertificateField; }
            set { giftCertificateField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> GiftCertificateAmount
        {
            get { return giftCertificateAmountField; }
            set { giftCertificateAmountField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> TaxPercent
        {
            get { return taxPercentField; }
            set { taxPercentField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> NoChargeAmount
        {
            get { return noChargeAmountField; }
            set { noChargeAmountField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> TaxAmount
        {
            get { return taxAmountField; }
            set { taxAmountField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class FreightAccount : PMObject
    {

        private int? seqIDField;

        private string descriptionField;

        private FreightCarrier freightCarrierField;

        private Person personField;

        private string thirdAcctNoField;

        private bool? nonResidentField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        public FreightCarrier FreightCarrier
        {
            get { return freightCarrierField; }
            set { freightCarrierField = value; }
        }

        /// <remarks/>
        public Person Person
        {
            get { return personField; }
            set { personField = value; }
        }

        /// <remarks/>
        public string ThirdAcctNo
        {
            get { return thirdAcctNoField; }
            set { thirdAcctNoField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? NonResident
        {
            get { return nonResidentField; }
            set { nonResidentField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OrderShipping : PMObject
    {

        private FreightCarrier freightCarrierField;

        private FreightService freightServiceField;

        private FreightAccount freightAccountField;

        private ShippingOption shippingOptionField;

        private string freightCodeField;

        private string freightCodeDescriptionField;

        private string shipCommentsField;

        private string neededByField;

        private bool? rushField;

        private DateTime? releaseDateField;

        private int? thirdPartyTypeField;

        private string thirdAccountNumberField;

        private bool? nCShipField;

        private bool? nCPackField;

        private bool? nCOffersField;

        private bool? nCHandlingField;

        private bool? nCOffShipHandlingField;

        private bool? nCSpecialHandlingField;

        private bool? nCRushField;

        /// <remarks/>
        public FreightCarrier FreightCarrier
        {
            get { return freightCarrierField; }
            set { freightCarrierField = value; }
        }

        /// <remarks/>
        public FreightService FreightService
        {
            get { return freightServiceField; }
            set { freightServiceField = value; }
        }

        /// <remarks/>
        public FreightAccount FreightAccount
        {
            get { return freightAccountField; }
            set { freightAccountField = value; }
        }

        /// <remarks/>
        public ShippingOption ShippingOption
        {
            get { return shippingOptionField; }
            set { shippingOptionField = value; }
        }

        /// <remarks/>
        public string FreightCode
        {
            get { return freightCodeField; }
            set { freightCodeField = value; }
        }

        /// <remarks/>
        public string FreightCodeDescription
        {
            get { return freightCodeDescriptionField; }
            set { freightCodeDescriptionField = value; }
        }

        /// <remarks/>
        public string ShipComments
        {
            get { return shipCommentsField; }
            set { shipCommentsField = value; }
        }

        /// <remarks/>
        public string NeededBy
        {
            get { return neededByField; }
            set { neededByField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? Rush
        {
            get { return rushField; }
            set { rushField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? ReleaseDate
        {
            get { return releaseDateField; }
            set { releaseDateField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? ThirdPartyType
        {
            get { return thirdPartyTypeField; }
            set { thirdPartyTypeField = value; }
        }

        /// <remarks/>
        public string ThirdAccountNumber
        {
            get { return thirdAccountNumberField; }
            set { thirdAccountNumberField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? NCShip
        {
            get { return nCShipField; }
            set { nCShipField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? NCPack
        {
            get { return nCPackField; }
            set { nCPackField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? NCOffers
        {
            get { return nCOffersField; }
            set { nCOffersField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? NCHandling
        {
            get { return nCHandlingField; }
            set { nCHandlingField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? NCOffShipHandling
        {
            get { return nCOffShipHandlingField; }
            set { nCOffShipHandlingField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? NCSpecialHandling
        {
            get { return nCSpecialHandlingField; }
            set { nCSpecialHandlingField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? NCRush
        {
            get { return nCRushField; }
            set { nCRushField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class CustomerProject : PMObject
    {

        private string idField;

        /// <remarks/>
        public string ID
        {
            get { return idField; }
            set { idField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class SourceDetail : PMObject
    {

        private string issueField;

        /// <remarks/>
        public string Issue
        {
            get { return issueField; }
            set { issueField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class Source : PMObject
    {

        private string descriptionField;

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ResponseMedia : PMObject
    {

        private string descriptionField;

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OrderClassification : PMObject
    {

        private string campaignIDField;

        private ResponseMedia responseMediaField;

        private Source sourceField;

        private SourceDetail issueField;

        private CustomerProject customerProjectField;

        private string customerCodeField;

        private string storeField;

        private string departmentField;

        private string distributionCenterField;

        private string vendorField;

        /// <remarks/>
        public string CampaignID
        {
            get { return campaignIDField; }
            set { campaignIDField = value; }
        }

        /// <remarks/>
        public ResponseMedia ResponseMedia
        {
            get { return responseMediaField; }
            set { responseMediaField = value; }
        }

        /// <remarks/>
        public Source Source
        {
            get { return sourceField; }
            set { sourceField = value; }
        }

        /// <remarks/>
        public SourceDetail Issue
        {
            get { return issueField; }
            set { issueField = value; }
        }

        /// <remarks/>
        public CustomerProject CustomerProject
        {
            get { return customerProjectField; }
            set { customerProjectField = value; }
        }

        /// <remarks/>
        public string CustomerCode
        {
            get { return customerCodeField; }
            set { customerCodeField = value; }
        }

        /// <remarks/>
        public string Store
        {
            get { return storeField; }
            set { storeField = value; }
        }

        /// <remarks/>
        public string Department
        {
            get { return departmentField; }
            set { departmentField = value; }
        }

        /// <remarks/>
        public string DistributionCenter
        {
            get { return distributionCenterField; }
            set { distributionCenterField = value; }
        }

        /// <remarks/>
        public string Vendor
        {
            get { return vendorField; }
            set { vendorField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OrderHeader : PMObject
    {

        private string idField;

        private DateTime? entryDateField;

        private OrderEntryView orderEntryViewField;

        private string referenceNumberField;

        private string pONumberField;

        private string commentsField;

        private string ipAddressField;

        private string approvalCommentField;

        private DateTime? insertDateField;

        private DateTime? uTCEntryDateTimeField;

        /// <remarks/>
        public string ID
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? EntryDate
        {
            get { return entryDateField; }
            set { entryDateField = value; }
        }

        /// <remarks/>
        public OrderEntryView OrderEntryView
        {
            get { return orderEntryViewField; }
            set { orderEntryViewField = value; }
        }

        /// <remarks/>
        public string ReferenceNumber
        {
            get { return referenceNumberField; }
            set { referenceNumberField = value; }
        }

        /// <remarks/>
        public string PONumber
        {
            get { return pONumberField; }
            set { pONumberField = value; }
        }

        /// <remarks/>
        public string Comments
        {
            get { return commentsField; }
            set { commentsField = value; }
        }

        /// <remarks/>
        public string IpAddress
        {
            get { return ipAddressField; }
            set { ipAddressField = value; }
        }

        /// <remarks/>
        public string ApprovalComment
        {
            get { return approvalCommentField; }
            set { approvalCommentField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? InsertDate
        {
            get { return insertDateField; }
            set { insertDateField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? UTCEntryDateTime
        {
            get { return uTCEntryDateTimeField; }
            set { uTCEntryDateTimeField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OrderEntryView : PMObject
    {

        private int? seqIDField;

        private string descriptionField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(TypeName = "Order", Namespace = "http://sma-promail/")]
    public partial class VeraCoreExportOrder : PMObject
    {

        private OrderHeader headerField;

        private OrderClassification classificationField;

        private OrderShipping shippingField;

        private OrderMoney moneyField;

        private OrderPayment paymentField;

        private OrderVariable[] orderVariablesField;

        private OrderedBy orderedByField;

        private OrderShipTo[] shipToField;

        private OrderBillTo billToField;

        private OfferOrdered[] offersField;

        private OrderRecurrenceSchedule orderRecurrenceScheduleField;

        private OrderBudget orderBudgetField;

        /// <remarks/>
        public OrderHeader Header
        {
            get { return headerField; }
            set { headerField = value; }
        }

        /// <remarks/>
        public OrderClassification Classification
        {
            get { return classificationField; }
            set { classificationField = value; }
        }

        /// <remarks/>
        public OrderShipping Shipping
        {
            get { return shippingField; }
            set { shippingField = value; }
        }

        /// <remarks/>
        public OrderMoney Money
        {
            get { return moneyField; }
            set { moneyField = value; }
        }

        /// <remarks/>
        public OrderPayment Payment
        {
            get { return paymentField; }
            set { paymentField = value; }
        }

        /// <remarks/>
        public OrderVariable[] OrderVariables
        {
            get { return orderVariablesField; }
            set { orderVariablesField = value; }
        }

        /// <remarks/>
        public OrderedBy OrderedBy
        {
            get { return orderedByField; }
            set { orderedByField = value; }
        }

        /// <remarks/>
        public OrderShipTo[] ShipTo
        {
            get { return shipToField; }
            set { shipToField = value; }
        }

        /// <remarks/>
        public OrderBillTo BillTo
        {
            get { return billToField; }
            set { billToField = value; }
        }

        /// <remarks/>
        public OfferOrdered[] Offers
        {
            get { return offersField; }
            set { offersField = value; }
        }

        /// <remarks/>
        public OrderRecurrenceSchedule OrderRecurrenceSchedule
        {
            get { return orderRecurrenceScheduleField; }
            set { orderRecurrenceScheduleField = value; }
        }

        /// <remarks/>
        public OrderBudget OrderBudget
        {
            get { return orderBudgetField; }
            set { orderBudgetField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ShippingCharges : PMObject
    {

        private string pPSlipField;

        private string orderIdField;

        private string level1NameField;

        private string level1CompanyField;

        private string level2NameField;

        private string level2CompanyField;

        private string level3NameField;

        private string level3CompanyField;

        private string level4NameField;

        private string level4CompanyField;

        private string level5NameField;

        private string level5CompanyField;

        private DateTime? dateShippedField;

        private string freightCarrierField;

        private string freightDescriptionField;

        private string trackingIDField;

        private string packageTypeField;

        private System.Nullable<float> packageWeightField;

        private System.Nullable<decimal> freightCostField;

        private string pickupNumberField;

        private string orderedBYFirstNameField;

        private string orderedBYLastNameField;

        private string orderedBYCompanyField;

        private string orderedBYAddress1Field;

        private string orderedBYAddress2Field;

        private string orderedBYAddress3Field;

        private string orderedBYCityField;

        private string orderedBYStateField;

        private string orderedBYZipCodeField;

        private string orderedBYCountryField;

        private string orderedBYPhoneField;

        private string orderedBYFaxField;

        private string orderedBYEmailField;

        private string orderedbyUIDField;

        private string shippedToFirstNameField;

        private string shippedToLastNameField;

        private string shippedToCompanyField;

        private string shippedToAddress1Field;

        private string shippedToAddress2Field;

        private string shippedToAddress3Field;

        private string shippedToCityField;

        private string shippedToStateField;

        private string shippedToZipCodeField;

        private string shippedToCountryField;

        private string shippedToPhoneField;

        private string shippedToFaxField;

        private string shippedToEmailField;

        private string shippedToUIDField;

        private string refField;

        private string poField;

        private int? sourceField;

        private string sourceDescriptionField;

        private int? rspnmdNumberField;

        private string rspnmdField;

        private int? cstprjIDField;

        private string cstprjField;

        private string catAccessField;

        private System.Nullable<decimal> packCostField;

        private string oBYMailerClassField;

        private string sTOMailerClassField;

        private string shipHyperlinkField;

        private string shippingOrderTypeField;

        private string carrierCodeField;

        private string freightServiceField;

        /// <remarks/>
        public string PPSlip
        {
            get { return pPSlipField; }
            set { pPSlipField = value; }
        }

        /// <remarks/>
        public string OrderId
        {
            get { return orderIdField; }
            set { orderIdField = value; }
        }

        /// <remarks/>
        public string level1Name
        {
            get { return level1NameField; }
            set { level1NameField = value; }
        }

        /// <remarks/>
        public string level1Company
        {
            get { return level1CompanyField; }
            set { level1CompanyField = value; }
        }

        /// <remarks/>
        public string level2Name
        {
            get { return level2NameField; }
            set { level2NameField = value; }
        }

        /// <remarks/>
        public string level2Company
        {
            get { return level2CompanyField; }
            set { level2CompanyField = value; }
        }

        /// <remarks/>
        public string level3Name
        {
            get { return level3NameField; }
            set { level3NameField = value; }
        }

        /// <remarks/>
        public string level3Company
        {
            get { return level3CompanyField; }
            set { level3CompanyField = value; }
        }

        /// <remarks/>
        public string level4Name
        {
            get { return level4NameField; }
            set { level4NameField = value; }
        }

        /// <remarks/>
        public string level4Company
        {
            get { return level4CompanyField; }
            set { level4CompanyField = value; }
        }

        /// <remarks/>
        public string level5Name
        {
            get { return level5NameField; }
            set { level5NameField = value; }
        }

        /// <remarks/>
        public string level5Company
        {
            get { return level5CompanyField; }
            set { level5CompanyField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? DateShipped
        {
            get { return dateShippedField; }
            set { dateShippedField = value; }
        }

        /// <remarks/>
        public string FreightCarrier
        {
            get { return freightCarrierField; }
            set { freightCarrierField = value; }
        }

        /// <remarks/>
        public string FreightDescription
        {
            get { return freightDescriptionField; }
            set { freightDescriptionField = value; }
        }

        /// <remarks/>
        public string TrackingID
        {
            get { return trackingIDField; }
            set { trackingIDField = value; }
        }

        /// <remarks/>
        public string PackageType
        {
            get { return packageTypeField; }
            set { packageTypeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<float> PackageWeight
        {
            get { return packageWeightField; }
            set { packageWeightField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> FreightCost
        {
            get { return freightCostField; }
            set { freightCostField = value; }
        }

        /// <remarks/>
        public string PickupNumber
        {
            get { return pickupNumberField; }
            set { pickupNumberField = value; }
        }

        /// <remarks/>
        public string OrderedBYFirstName
        {
            get { return orderedBYFirstNameField; }
            set { orderedBYFirstNameField = value; }
        }

        /// <remarks/>
        public string OrderedBYLastName
        {
            get { return orderedBYLastNameField; }
            set { orderedBYLastNameField = value; }
        }

        /// <remarks/>
        public string OrderedBYCompany
        {
            get { return orderedBYCompanyField; }
            set { orderedBYCompanyField = value; }
        }

        /// <remarks/>
        public string OrderedBYAddress1
        {
            get { return orderedBYAddress1Field; }
            set { orderedBYAddress1Field = value; }
        }

        /// <remarks/>
        public string OrderedBYAddress2
        {
            get { return orderedBYAddress2Field; }
            set { orderedBYAddress2Field = value; }
        }

        /// <remarks/>
        public string OrderedBYAddress3
        {
            get { return orderedBYAddress3Field; }
            set { orderedBYAddress3Field = value; }
        }

        /// <remarks/>
        public string OrderedBYCity
        {
            get { return orderedBYCityField; }
            set { orderedBYCityField = value; }
        }

        /// <remarks/>
        public string OrderedBYState
        {
            get { return orderedBYStateField; }
            set { orderedBYStateField = value; }
        }

        /// <remarks/>
        public string OrderedBYZipCode
        {
            get { return orderedBYZipCodeField; }
            set { orderedBYZipCodeField = value; }
        }

        /// <remarks/>
        public string OrderedBYCountry
        {
            get { return orderedBYCountryField; }
            set { orderedBYCountryField = value; }
        }

        /// <remarks/>
        public string OrderedBYPhone
        {
            get { return orderedBYPhoneField; }
            set { orderedBYPhoneField = value; }
        }

        /// <remarks/>
        public string OrderedBYFax
        {
            get { return orderedBYFaxField; }
            set { orderedBYFaxField = value; }
        }

        /// <remarks/>
        public string OrderedBYEmail
        {
            get { return orderedBYEmailField; }
            set { orderedBYEmailField = value; }
        }

        /// <remarks/>
        public string OrderedbyUID
        {
            get { return orderedbyUIDField; }
            set { orderedbyUIDField = value; }
        }

        /// <remarks/>
        public string ShippedToFirstName
        {
            get { return shippedToFirstNameField; }
            set { shippedToFirstNameField = value; }
        }

        /// <remarks/>
        public string ShippedToLastName
        {
            get { return shippedToLastNameField; }
            set { shippedToLastNameField = value; }
        }

        /// <remarks/>
        public string ShippedToCompany
        {
            get { return shippedToCompanyField; }
            set { shippedToCompanyField = value; }
        }

        /// <remarks/>
        public string ShippedToAddress1
        {
            get { return shippedToAddress1Field; }
            set { shippedToAddress1Field = value; }
        }

        /// <remarks/>
        public string ShippedToAddress2
        {
            get { return shippedToAddress2Field; }
            set { shippedToAddress2Field = value; }
        }

        /// <remarks/>
        public string ShippedToAddress3
        {
            get { return shippedToAddress3Field; }
            set { shippedToAddress3Field = value; }
        }

        /// <remarks/>
        public string ShippedToCity
        {
            get { return shippedToCityField; }
            set { shippedToCityField = value; }
        }

        /// <remarks/>
        public string ShippedToState
        {
            get { return shippedToStateField; }
            set { shippedToStateField = value; }
        }

        /// <remarks/>
        public string ShippedToZipCode
        {
            get { return shippedToZipCodeField; }
            set { shippedToZipCodeField = value; }
        }

        /// <remarks/>
        public string ShippedToCountry
        {
            get { return shippedToCountryField; }
            set { shippedToCountryField = value; }
        }

        /// <remarks/>
        public string ShippedToPhone
        {
            get { return shippedToPhoneField; }
            set { shippedToPhoneField = value; }
        }

        /// <remarks/>
        public string ShippedToFax
        {
            get { return shippedToFaxField; }
            set { shippedToFaxField = value; }
        }

        /// <remarks/>
        public string ShippedToEmail
        {
            get { return shippedToEmailField; }
            set { shippedToEmailField = value; }
        }

        /// <remarks/>
        public string ShippedToUID
        {
            get { return shippedToUIDField; }
            set { shippedToUIDField = value; }
        }

        /// <remarks/>
        public string Ref
        {
            get { return refField; }
            set { refField = value; }
        }

        /// <remarks/>
        public string Po
        {
            get { return poField; }
            set { poField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Source
        {
            get { return sourceField; }
            set { sourceField = value; }
        }

        /// <remarks/>
        public string SourceDescription
        {
            get { return sourceDescriptionField; }
            set { sourceDescriptionField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? RspnmdNumber
        {
            get { return rspnmdNumberField; }
            set { rspnmdNumberField = value; }
        }

        /// <remarks/>
        public string Rspnmd
        {
            get { return rspnmdField; }
            set { rspnmdField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? CstprjID
        {
            get { return cstprjIDField; }
            set { cstprjIDField = value; }
        }

        /// <remarks/>
        public string Cstprj
        {
            get { return cstprjField; }
            set { cstprjField = value; }
        }

        /// <remarks/>
        public string CatAccess
        {
            get { return catAccessField; }
            set { catAccessField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> PackCost
        {
            get { return packCostField; }
            set { packCostField = value; }
        }

        /// <remarks/>
        public string OBYMailerClass
        {
            get { return oBYMailerClassField; }
            set { oBYMailerClassField = value; }
        }

        /// <remarks/>
        public string STOMailerClass
        {
            get { return sTOMailerClassField; }
            set { sTOMailerClassField = value; }
        }

        /// <remarks/>
        public string ShipHyperlink
        {
            get { return shipHyperlinkField; }
            set { shipHyperlinkField = value; }
        }

        /// <remarks/>
        public string ShippingOrderType
        {
            get { return shippingOrderTypeField; }
            set { shippingOrderTypeField = value; }
        }

        /// <remarks/>
        public string CarrierCode
        {
            get { return carrierCodeField; }
            set { carrierCodeField = value; }
        }

        /// <remarks/>
        public string FreightService
        {
            get { return freightServiceField; }
            set { freightServiceField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ProductReturns : PMObject
    {

        private string orderIdField;

        private string pickSlipField;

        private DateTime? dateOrderedField;

        private DateTime? firstShipField;

        private DateTime? dateReturnedField;

        private string productIdField;

        private string descriptionField;

        private string productVersionField;

        private string sizeField;

        private string colorField;

        private string orderReasonCodeField;

        private string productReasonCodeField;

        private int? shipQtyField;

        private int? returnQtyField;

        private string productReturnCommentField;

        private string orderReturnCommentField;

        private string receivedAsField;

        private int? packQuantityField;

        private string packDescriptionField;

        private string orderedByCompanyField;

        private string orderedByNameField;

        private string orderedByTitleField;

        private string orderedByAddress1Field;

        private string orderedByAddress2Field;

        private string orderedByAddress3Field;

        private string orderedByCityField;

        private string orderedByStateField;

        private string orderedByZipcodeField;

        private string orderedByCountryField;

        private string shipToCompanyField;

        private string shipToNameField;

        private string shipToAddress1Field;

        private string shipToAddress2Field;

        private string shipToAddress3Field;

        private string shipToCityField;

        private string shipToStateField;

        private string shipToZipcodeField;

        private string shipToCountryField;

        private int? ownerIDField;

        private string referenceNumberField;

        private string dispositionField;

        private DateTime? evaluationDateField;

        private string evaluationCommentsField;

        /// <remarks/>
        public string OrderId
        {
            get { return orderIdField; }
            set { orderIdField = value; }
        }

        /// <remarks/>
        public string PickSlip
        {
            get { return pickSlipField; }
            set { pickSlipField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? DateOrdered
        {
            get { return dateOrderedField; }
            set { dateOrderedField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? FirstShip
        {
            get { return firstShipField; }
            set { firstShipField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? DateReturned
        {
            get { return dateReturnedField; }
            set { dateReturnedField = value; }
        }

        /// <remarks/>
        public string ProductId
        {
            get { return productIdField; }
            set { productIdField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        public string ProductVersion
        {
            get { return productVersionField; }
            set { productVersionField = value; }
        }

        /// <remarks/>
        public string Size
        {
            get { return sizeField; }
            set { sizeField = value; }
        }

        /// <remarks/>
        public string Color
        {
            get { return colorField; }
            set { colorField = value; }
        }

        /// <remarks/>
        public string OrderReasonCode
        {
            get { return orderReasonCodeField; }
            set { orderReasonCodeField = value; }
        }

        /// <remarks/>
        public string ProductReasonCode
        {
            get { return productReasonCodeField; }
            set { productReasonCodeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? ShipQty
        {
            get { return shipQtyField; }
            set { shipQtyField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? ReturnQty
        {
            get { return returnQtyField; }
            set { returnQtyField = value; }
        }

        /// <remarks/>
        public string ProductReturnComment
        {
            get { return productReturnCommentField; }
            set { productReturnCommentField = value; }
        }

        /// <remarks/>
        public string OrderReturnComment
        {
            get { return orderReturnCommentField; }
            set { orderReturnCommentField = value; }
        }

        /// <remarks/>
        public string ReceivedAs
        {
            get { return receivedAsField; }
            set { receivedAsField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? PackQuantity
        {
            get { return packQuantityField; }
            set { packQuantityField = value; }
        }

        /// <remarks/>
        public string PackDescription
        {
            get { return packDescriptionField; }
            set { packDescriptionField = value; }
        }

        /// <remarks/>
        public string OrderedByCompany
        {
            get { return orderedByCompanyField; }
            set { orderedByCompanyField = value; }
        }

        /// <remarks/>
        public string OrderedByName
        {
            get { return orderedByNameField; }
            set { orderedByNameField = value; }
        }

        /// <remarks/>
        public string OrderedByTitle
        {
            get { return orderedByTitleField; }
            set { orderedByTitleField = value; }
        }

        /// <remarks/>
        public string OrderedByAddress1
        {
            get { return orderedByAddress1Field; }
            set { orderedByAddress1Field = value; }
        }

        /// <remarks/>
        public string OrderedByAddress2
        {
            get { return orderedByAddress2Field; }
            set { orderedByAddress2Field = value; }
        }

        /// <remarks/>
        public string OrderedByAddress3
        {
            get { return orderedByAddress3Field; }
            set { orderedByAddress3Field = value; }
        }

        /// <remarks/>
        public string OrderedByCity
        {
            get { return orderedByCityField; }
            set { orderedByCityField = value; }
        }

        /// <remarks/>
        public string OrderedByState
        {
            get { return orderedByStateField; }
            set { orderedByStateField = value; }
        }

        /// <remarks/>
        public string OrderedByZipcode
        {
            get { return orderedByZipcodeField; }
            set { orderedByZipcodeField = value; }
        }

        /// <remarks/>
        public string OrderedByCountry
        {
            get { return orderedByCountryField; }
            set { orderedByCountryField = value; }
        }

        /// <remarks/>
        public string ShipToCompany
        {
            get { return shipToCompanyField; }
            set { shipToCompanyField = value; }
        }

        /// <remarks/>
        public string ShipToName
        {
            get { return shipToNameField; }
            set { shipToNameField = value; }
        }

        /// <remarks/>
        public string ShipToAddress1
        {
            get { return shipToAddress1Field; }
            set { shipToAddress1Field = value; }
        }

        /// <remarks/>
        public string ShipToAddress2
        {
            get { return shipToAddress2Field; }
            set { shipToAddress2Field = value; }
        }

        /// <remarks/>
        public string ShipToAddress3
        {
            get { return shipToAddress3Field; }
            set { shipToAddress3Field = value; }
        }

        /// <remarks/>
        public string ShipToCity
        {
            get { return shipToCityField; }
            set { shipToCityField = value; }
        }

        /// <remarks/>
        public string ShipToState
        {
            get { return shipToStateField; }
            set { shipToStateField = value; }
        }

        /// <remarks/>
        public string ShipToZipcode
        {
            get { return shipToZipcodeField; }
            set { shipToZipcodeField = value; }
        }

        /// <remarks/>
        public string ShipToCountry
        {
            get { return shipToCountryField; }
            set { shipToCountryField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? OwnerID
        {
            get { return ownerIDField; }
            set { ownerIDField = value; }
        }

        /// <remarks/>
        public string ReferenceNumber
        {
            get { return referenceNumberField; }
            set { referenceNumberField = value; }
        }

        /// <remarks/>
        public string Disposition
        {
            get { return dispositionField; }
            set { dispositionField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? EvaluationDate
        {
            get { return evaluationDateField; }
            set { evaluationDateField = value; }
        }

        /// <remarks/>
        public string EvaluationComments
        {
            get { return evaluationCommentsField; }
            set { evaluationCommentsField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ShippingActivity : PMObject
    {

        private string shipDateField;

        private string orderIdField;

        private string pickPackIdField;

        private string typeField;

        private int? linesShippedField;

        private int? piecesShippedField;

        private int? numberofPackagesField;

        private System.Nullable<double> totalWeightField;

        private System.Nullable<decimal> publishedFreightField;

        private System.Nullable<decimal> actualFreightField;

        private System.Nullable<decimal> markedUpFreightField;

        private string shippingOrderTypeField;

        /// <remarks/>
        public string ShipDate
        {
            get { return shipDateField; }
            set { shipDateField = value; }
        }

        /// <remarks/>
        public string OrderId
        {
            get { return orderIdField; }
            set { orderIdField = value; }
        }

        /// <remarks/>
        public string PickPackId
        {
            get { return pickPackIdField; }
            set { pickPackIdField = value; }
        }

        /// <remarks/>
        public string Type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? LinesShipped
        {
            get { return linesShippedField; }
            set { linesShippedField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? PiecesShipped
        {
            get { return piecesShippedField; }
            set { piecesShippedField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? NumberofPackages
        {
            get { return numberofPackagesField; }
            set { numberofPackagesField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<double> TotalWeight
        {
            get { return totalWeightField; }
            set { totalWeightField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> PublishedFreight
        {
            get { return publishedFreightField; }
            set { publishedFreightField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> ActualFreight
        {
            get { return actualFreightField; }
            set { actualFreightField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> MarkedUpFreight
        {
            get { return markedUpFreightField; }
            set { markedUpFreightField = value; }
        }

        /// <remarks/>
        public string ShippingOrderType
        {
            get { return shippingOrderTypeField; }
            set { shippingOrderTypeField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ProductShipment : PMObject
    {

        private string ownerField;

        private string orderIdField;

        private DateTime? orderDateField;

        private DateTime? shipDateField;

        private string orderedByCoField;

        private string orderedByNameField;

        private string address1Field;

        private string address2Field;

        private string address3Field;

        private string cityField;

        private string stateField;

        private string zipField;

        private string countryField;

        private string phoneField;

        private string faxField;

        private string emailField;

        private string uIDField;

        private string shipToCoField;

        private string shipToNameField;

        private string sTOAddress1Field;

        private string sTOAddress2Field;

        private string sTOAddress3Field;

        private string sTOCityField;

        private string sTOStateField;

        private string asSTOZipField;

        private string sTOCountryField;

        private string sTOPhoneField;

        private string sTOFaxField;

        private string sTOEmailField;

        private string sTOUIDField;

        private string sourceField;

        private int orderQtyField;

        private int shipQtyField;

        private int? onHandQtyField;

        private int? reOrderPointField;

        private string partNumField;

        private string versionField;

        private string descriptionField;

        private string sizeField;

        private string colorField;

        private string sortLvl1Field;

        private string sortLvl2Field;

        private string sortLvl3Field;

        private string sortLv4Field;

        private System.Nullable<decimal> fifoValueField;

        private System.Nullable<decimal> defaultValueField;

        private System.Nullable<decimal> extendedFifoValueField;

        private System.Nullable<decimal> extendedDefaultValueField;

        private string ordVarFld1Field;

        private string ordVarValue1Field;

        private string ordVarFld2Field;

        private string ordVarValue2Field;

        private string ordVarFld3Field;

        private string ordVarValue3Field;

        private string ordVarFld4Field;

        private string ordVarValue4Field;

        private string ordVarFld5Field;

        private string ordVarValue5Field;

        private string ordVarFld6Field;

        private string ordVarValue6Field;

        private string ordVarFld7Field;

        private string ordVarValue7Field;

        private string ordVarFld8Field;

        private string ordVarValue8Field;

        private string ordVarFld9Field;

        private string ordVarValue9Field;

        private string ordVarFld10Field;

        private string ordVarValue10Field;

        private string prodVarFld1Field;

        private string prodVarValue1Field;

        private string prodVarFld2Field;

        private string prodVarValue2Field;

        private string prodVarFld3Field;

        private string prodVarValue3Field;

        private string prodVarFld4Field;

        private string prodVarValue4Field;

        private string prodVarFld5Field;

        private string prodVarValue5Field;

        private string prodVarFld6Field;

        private string prodVarValue6Field;

        private string prodVarFld7Field;

        private string prodVarValue7Field;

        private string prodVarFld8Field;

        private string prodVarValue8Field;

        private string prodVarFld9Field;

        private string prodVarValue9Field;

        private string prodVarFld10Field;

        private string prodVarValue10Field;

        private string prodVarFld11Field;

        private string prodVarValue11Field;

        private string prodVarFld12Field;

        private string prodVarValue12Field;

        private string prodVarFld13Field;

        private string prodVarValue13Field;

        private string prodVarFld14Field;

        private string prodVarValue14Field;

        private string prodVarFld15Field;

        private string prodVarValue15Field;

        private string prodVarFld16Field;

        private string prodVarValue16Field;

        private string prodVarFld17Field;

        private string prodVarValue17Field;

        private string prodVarFld18Field;

        private string prodVarValue18Field;

        private string prodVarFld19Field;

        private string prodVarValue19Field;

        private string prodVarFld20Field;

        private string prodVarValue20Field;

        private string rep1NameField;

        private string rep1CompanyField;

        private string rep1TitleField;

        private string rep2NameField;

        private string rep2CompanyField;

        private string rep2TitleField;

        private string rep3NameField;

        private string rep3CompanyField;

        private string rep3TitleField;

        private string rep4NameField;

        private string rep4CompanyField;

        private string rep4TitleField;

        private string rep5NameField;

        private string rep5CompanyField;

        private string rep5TitleField;

        private string rep6NameField;

        private string rep6CompanyField;

        private string rep6TitleField;

        private string rep7NameField;

        private string rep7CompanyField;

        private string rep7TitleField;

        private string rep8NameField;

        private string rep8CompanyField;

        private string rep8TitleField;

        private string rep9NameField;

        private string rep9CompanyField;

        private string rep9TitleField;

        private string rep10NameField;

        private string rep10CompanyField;

        private string rep10TitleField;

        private string referenceNoField;

        private string poField;

        private string oBYMailerClassField;

        private string sTOMailerClassField;

        private string pickSlipIdField;

        private int? productOwnerField;

        private string projectIdField;

        private string projectDescField;

        /// <remarks/>
        public string Owner
        {
            get { return ownerField; }
            set { ownerField = value; }
        }

        /// <remarks/>
        public string OrderId
        {
            get { return orderIdField; }
            set { orderIdField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? OrderDate
        {
            get { return orderDateField; }
            set { orderDateField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? ShipDate
        {
            get { return shipDateField; }
            set { shipDateField = value; }
        }

        /// <remarks/>
        public string OrderedByCo
        {
            get { return orderedByCoField; }
            set { orderedByCoField = value; }
        }

        /// <remarks/>
        public string OrderedByName
        {
            get { return orderedByNameField; }
            set { orderedByNameField = value; }
        }

        /// <remarks/>
        public string Address1
        {
            get { return address1Field; }
            set { address1Field = value; }
        }

        /// <remarks/>
        public string Address2
        {
            get { return address2Field; }
            set { address2Field = value; }
        }

        /// <remarks/>
        public string Address3
        {
            get { return address3Field; }
            set { address3Field = value; }
        }

        /// <remarks/>
        public string City
        {
            get { return cityField; }
            set { cityField = value; }
        }

        /// <remarks/>
        public string State
        {
            get { return stateField; }
            set { stateField = value; }
        }

        /// <remarks/>
        public string Zip
        {
            get { return zipField; }
            set { zipField = value; }
        }

        /// <remarks/>
        public string Country
        {
            get { return countryField; }
            set { countryField = value; }
        }

        /// <remarks/>
        public string Phone
        {
            get { return phoneField; }
            set { phoneField = value; }
        }

        /// <remarks/>
        public string Fax
        {
            get { return faxField; }
            set { faxField = value; }
        }

        /// <remarks/>
        public string Email
        {
            get { return emailField; }
            set { emailField = value; }
        }

        /// <remarks/>
        public string UID
        {
            get { return uIDField; }
            set { uIDField = value; }
        }

        /// <remarks/>
        public string ShipToCo
        {
            get { return shipToCoField; }
            set { shipToCoField = value; }
        }

        /// <remarks/>
        public string ShipToName
        {
            get { return shipToNameField; }
            set { shipToNameField = value; }
        }

        /// <remarks/>
        public string STOAddress1
        {
            get { return sTOAddress1Field; }
            set { sTOAddress1Field = value; }
        }

        /// <remarks/>
        public string STOAddress2
        {
            get { return sTOAddress2Field; }
            set { sTOAddress2Field = value; }
        }

        /// <remarks/>
        public string STOAddress3
        {
            get { return sTOAddress3Field; }
            set { sTOAddress3Field = value; }
        }

        /// <remarks/>
        public string STOCity
        {
            get { return sTOCityField; }
            set { sTOCityField = value; }
        }

        /// <remarks/>
        public string STOState
        {
            get { return sTOStateField; }
            set { sTOStateField = value; }
        }

        /// <remarks/>
        public string asSTOZip
        {
            get { return asSTOZipField; }
            set { asSTOZipField = value; }
        }

        /// <remarks/>
        public string STOCountry
        {
            get { return sTOCountryField; }
            set { sTOCountryField = value; }
        }

        /// <remarks/>
        public string STOPhone
        {
            get { return sTOPhoneField; }
            set { sTOPhoneField = value; }
        }

        /// <remarks/>
        public string STOFax
        {
            get { return sTOFaxField; }
            set { sTOFaxField = value; }
        }

        /// <remarks/>
        public string STOEmail
        {
            get { return sTOEmailField; }
            set { sTOEmailField = value; }
        }

        /// <remarks/>
        public string STOUID
        {
            get { return sTOUIDField; }
            set { sTOUIDField = value; }
        }

        /// <remarks/>
        public string Source
        {
            get { return sourceField; }
            set { sourceField = value; }
        }

        /// <remarks/>
        public int OrderQty
        {
            get { return orderQtyField; }
            set { orderQtyField = value; }
        }

        /// <remarks/>
        public int ShipQty
        {
            get { return shipQtyField; }
            set { shipQtyField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? OnHandQty
        {
            get { return onHandQtyField; }
            set { onHandQtyField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? ReOrderPoint
        {
            get { return reOrderPointField; }
            set { reOrderPointField = value; }
        }

        /// <remarks/>
        public string PartNum
        {
            get { return partNumField; }
            set { partNumField = value; }
        }

        /// <remarks/>
        public string Version
        {
            get { return versionField; }
            set { versionField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        public string Size
        {
            get { return sizeField; }
            set { sizeField = value; }
        }

        /// <remarks/>
        public string Color
        {
            get { return colorField; }
            set { colorField = value; }
        }

        /// <remarks/>
        public string SortLvl1
        {
            get { return sortLvl1Field; }
            set { sortLvl1Field = value; }
        }

        /// <remarks/>
        public string SortLvl2
        {
            get { return sortLvl2Field; }
            set { sortLvl2Field = value; }
        }

        /// <remarks/>
        public string SortLvl3
        {
            get { return sortLvl3Field; }
            set { sortLvl3Field = value; }
        }

        /// <remarks/>
        public string SortLv4
        {
            get { return sortLv4Field; }
            set { sortLv4Field = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> FifoValue
        {
            get { return fifoValueField; }
            set { fifoValueField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> DefaultValue
        {
            get { return defaultValueField; }
            set { defaultValueField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> ExtendedFifoValue
        {
            get { return extendedFifoValueField; }
            set { extendedFifoValueField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> ExtendedDefaultValue
        {
            get { return extendedDefaultValueField; }
            set { extendedDefaultValueField = value; }
        }

        /// <remarks/>
        public string OrdVarFld1
        {
            get { return ordVarFld1Field; }
            set { ordVarFld1Field = value; }
        }

        /// <remarks/>
        public string OrdVarValue1
        {
            get { return ordVarValue1Field; }
            set { ordVarValue1Field = value; }
        }

        /// <remarks/>
        public string OrdVarFld2
        {
            get { return ordVarFld2Field; }
            set { ordVarFld2Field = value; }
        }

        /// <remarks/>
        public string OrdVarValue2
        {
            get { return ordVarValue2Field; }
            set { ordVarValue2Field = value; }
        }

        /// <remarks/>
        public string OrdVarFld3
        {
            get { return ordVarFld3Field; }
            set { ordVarFld3Field = value; }
        }

        /// <remarks/>
        public string OrdVarValue3
        {
            get { return ordVarValue3Field; }
            set { ordVarValue3Field = value; }
        }

        /// <remarks/>
        public string OrdVarFld4
        {
            get { return ordVarFld4Field; }
            set { ordVarFld4Field = value; }
        }

        /// <remarks/>
        public string OrdVarValue4
        {
            get { return ordVarValue4Field; }
            set { ordVarValue4Field = value; }
        }

        /// <remarks/>
        public string OrdVarFld5
        {
            get { return ordVarFld5Field; }
            set { ordVarFld5Field = value; }
        }

        /// <remarks/>
        public string OrdVarValue5
        {
            get { return ordVarValue5Field; }
            set { ordVarValue5Field = value; }
        }

        /// <remarks/>
        public string OrdVarFld6
        {
            get { return ordVarFld6Field; }
            set { ordVarFld6Field = value; }
        }

        /// <remarks/>
        public string OrdVarValue6
        {
            get { return ordVarValue6Field; }
            set { ordVarValue6Field = value; }
        }

        /// <remarks/>
        public string OrdVarFld7
        {
            get { return ordVarFld7Field; }
            set { ordVarFld7Field = value; }
        }

        /// <remarks/>
        public string OrdVarValue7
        {
            get { return ordVarValue7Field; }
            set { ordVarValue7Field = value; }
        }

        /// <remarks/>
        public string OrdVarFld8
        {
            get { return ordVarFld8Field; }
            set { ordVarFld8Field = value; }
        }

        /// <remarks/>
        public string OrdVarValue8
        {
            get { return ordVarValue8Field; }
            set { ordVarValue8Field = value; }
        }

        /// <remarks/>
        public string OrdVarFld9
        {
            get { return ordVarFld9Field; }
            set { ordVarFld9Field = value; }
        }

        /// <remarks/>
        public string OrdVarValue9
        {
            get { return ordVarValue9Field; }
            set { ordVarValue9Field = value; }
        }

        /// <remarks/>
        public string OrdVarFld10
        {
            get { return ordVarFld10Field; }
            set { ordVarFld10Field = value; }
        }

        /// <remarks/>
        public string OrdVarValue10
        {
            get { return ordVarValue10Field; }
            set { ordVarValue10Field = value; }
        }

        /// <remarks/>
        public string ProdVarFld1
        {
            get { return prodVarFld1Field; }
            set { prodVarFld1Field = value; }
        }

        /// <remarks/>
        public string ProdVarValue1
        {
            get { return prodVarValue1Field; }
            set { prodVarValue1Field = value; }
        }

        /// <remarks/>
        public string ProdVarFld2
        {
            get { return prodVarFld2Field; }
            set { prodVarFld2Field = value; }
        }

        /// <remarks/>
        public string ProdVarValue2
        {
            get { return prodVarValue2Field; }
            set { prodVarValue2Field = value; }
        }

        /// <remarks/>
        public string ProdVarFld3
        {
            get { return prodVarFld3Field; }
            set { prodVarFld3Field = value; }
        }

        /// <remarks/>
        public string ProdVarValue3
        {
            get { return prodVarValue3Field; }
            set { prodVarValue3Field = value; }
        }

        /// <remarks/>
        public string ProdVarFld4
        {
            get { return prodVarFld4Field; }
            set { prodVarFld4Field = value; }
        }

        /// <remarks/>
        public string ProdVarValue4
        {
            get { return prodVarValue4Field; }
            set { prodVarValue4Field = value; }
        }

        /// <remarks/>
        public string ProdVarFld5
        {
            get { return prodVarFld5Field; }
            set { prodVarFld5Field = value; }
        }

        /// <remarks/>
        public string ProdVarValue5
        {
            get { return prodVarValue5Field; }
            set { prodVarValue5Field = value; }
        }

        /// <remarks/>
        public string ProdVarFld6
        {
            get { return prodVarFld6Field; }
            set { prodVarFld6Field = value; }
        }

        /// <remarks/>
        public string ProdVarValue6
        {
            get { return prodVarValue6Field; }
            set { prodVarValue6Field = value; }
        }

        /// <remarks/>
        public string ProdVarFld7
        {
            get { return prodVarFld7Field; }
            set { prodVarFld7Field = value; }
        }

        /// <remarks/>
        public string ProdVarValue7
        {
            get { return prodVarValue7Field; }
            set { prodVarValue7Field = value; }
        }

        /// <remarks/>
        public string ProdVarFld8
        {
            get { return prodVarFld8Field; }
            set { prodVarFld8Field = value; }
        }

        /// <remarks/>
        public string ProdVarValue8
        {
            get { return prodVarValue8Field; }
            set { prodVarValue8Field = value; }
        }

        /// <remarks/>
        public string ProdVarFld9
        {
            get { return prodVarFld9Field; }
            set { prodVarFld9Field = value; }
        }

        /// <remarks/>
        public string ProdVarValue9
        {
            get { return prodVarValue9Field; }
            set { prodVarValue9Field = value; }
        }

        /// <remarks/>
        public string ProdVarFld10
        {
            get { return prodVarFld10Field; }
            set { prodVarFld10Field = value; }
        }

        /// <remarks/>
        public string ProdVarValue10
        {
            get { return prodVarValue10Field; }
            set { prodVarValue10Field = value; }
        }

        /// <remarks/>
        public string ProdVarFld11
        {
            get { return prodVarFld11Field; }
            set { prodVarFld11Field = value; }
        }

        /// <remarks/>
        public string ProdVarValue11
        {
            get { return prodVarValue11Field; }
            set { prodVarValue11Field = value; }
        }

        /// <remarks/>
        public string ProdVarFld12
        {
            get { return prodVarFld12Field; }
            set { prodVarFld12Field = value; }
        }

        /// <remarks/>
        public string ProdVarValue12
        {
            get { return prodVarValue12Field; }
            set { prodVarValue12Field = value; }
        }

        /// <remarks/>
        public string ProdVarFld13
        {
            get { return prodVarFld13Field; }
            set { prodVarFld13Field = value; }
        }

        /// <remarks/>
        public string ProdVarValue13
        {
            get { return prodVarValue13Field; }
            set { prodVarValue13Field = value; }
        }

        /// <remarks/>
        public string ProdVarFld14
        {
            get { return prodVarFld14Field; }
            set { prodVarFld14Field = value; }
        }

        /// <remarks/>
        public string ProdVarValue14
        {
            get { return prodVarValue14Field; }
            set { prodVarValue14Field = value; }
        }

        /// <remarks/>
        public string ProdVarFld15
        {
            get { return prodVarFld15Field; }
            set { prodVarFld15Field = value; }
        }

        /// <remarks/>
        public string ProdVarValue15
        {
            get { return prodVarValue15Field; }
            set { prodVarValue15Field = value; }
        }

        /// <remarks/>
        public string ProdVarFld16
        {
            get { return prodVarFld16Field; }
            set { prodVarFld16Field = value; }
        }

        /// <remarks/>
        public string ProdVarValue16
        {
            get { return prodVarValue16Field; }
            set { prodVarValue16Field = value; }
        }

        /// <remarks/>
        public string ProdVarFld17
        {
            get { return prodVarFld17Field; }
            set { prodVarFld17Field = value; }
        }

        /// <remarks/>
        public string ProdVarValue17
        {
            get { return prodVarValue17Field; }
            set { prodVarValue17Field = value; }
        }

        /// <remarks/>
        public string ProdVarFld18
        {
            get { return prodVarFld18Field; }
            set { prodVarFld18Field = value; }
        }

        /// <remarks/>
        public string ProdVarValue18
        {
            get { return prodVarValue18Field; }
            set { prodVarValue18Field = value; }
        }

        /// <remarks/>
        public string ProdVarFld19
        {
            get { return prodVarFld19Field; }
            set { prodVarFld19Field = value; }
        }

        /// <remarks/>
        public string ProdVarValue19
        {
            get { return prodVarValue19Field; }
            set { prodVarValue19Field = value; }
        }

        /// <remarks/>
        public string ProdVarFld20
        {
            get { return prodVarFld20Field; }
            set { prodVarFld20Field = value; }
        }

        /// <remarks/>
        public string ProdVarValue20
        {
            get { return prodVarValue20Field; }
            set { prodVarValue20Field = value; }
        }

        /// <remarks/>
        public string Rep1Name
        {
            get { return rep1NameField; }
            set { rep1NameField = value; }
        }

        /// <remarks/>
        public string Rep1Company
        {
            get { return rep1CompanyField; }
            set { rep1CompanyField = value; }
        }

        /// <remarks/>
        public string Rep1Title
        {
            get { return rep1TitleField; }
            set { rep1TitleField = value; }
        }

        /// <remarks/>
        public string Rep2Name
        {
            get { return rep2NameField; }
            set { rep2NameField = value; }
        }

        /// <remarks/>
        public string Rep2Company
        {
            get { return rep2CompanyField; }
            set { rep2CompanyField = value; }
        }

        /// <remarks/>
        public string Rep2Title
        {
            get { return rep2TitleField; }
            set { rep2TitleField = value; }
        }

        /// <remarks/>
        public string Rep3Name
        {
            get { return rep3NameField; }
            set { rep3NameField = value; }
        }

        /// <remarks/>
        public string Rep3Company
        {
            get { return rep3CompanyField; }
            set { rep3CompanyField = value; }
        }

        /// <remarks/>
        public string Rep3Title
        {
            get { return rep3TitleField; }
            set { rep3TitleField = value; }
        }

        /// <remarks/>
        public string Rep4Name
        {
            get { return rep4NameField; }
            set { rep4NameField = value; }
        }

        /// <remarks/>
        public string Rep4Company
        {
            get { return rep4CompanyField; }
            set { rep4CompanyField = value; }
        }

        /// <remarks/>
        public string Rep4Title
        {
            get { return rep4TitleField; }
            set { rep4TitleField = value; }
        }

        /// <remarks/>
        public string Rep5Name
        {
            get { return rep5NameField; }
            set { rep5NameField = value; }
        }

        /// <remarks/>
        public string Rep5Company
        {
            get { return rep5CompanyField; }
            set { rep5CompanyField = value; }
        }

        /// <remarks/>
        public string Rep5Title
        {
            get { return rep5TitleField; }
            set { rep5TitleField = value; }
        }

        /// <remarks/>
        public string Rep6Name
        {
            get { return rep6NameField; }
            set { rep6NameField = value; }
        }

        /// <remarks/>
        public string Rep6Company
        {
            get { return rep6CompanyField; }
            set { rep6CompanyField = value; }
        }

        /// <remarks/>
        public string Rep6Title
        {
            get { return rep6TitleField; }
            set { rep6TitleField = value; }
        }

        /// <remarks/>
        public string Rep7Name
        {
            get { return rep7NameField; }
            set { rep7NameField = value; }
        }

        /// <remarks/>
        public string Rep7Company
        {
            get { return rep7CompanyField; }
            set { rep7CompanyField = value; }
        }

        /// <remarks/>
        public string Rep7Title
        {
            get { return rep7TitleField; }
            set { rep7TitleField = value; }
        }

        /// <remarks/>
        public string Rep8Name
        {
            get { return rep8NameField; }
            set { rep8NameField = value; }
        }

        /// <remarks/>
        public string Rep8Company
        {
            get { return rep8CompanyField; }
            set { rep8CompanyField = value; }
        }

        /// <remarks/>
        public string Rep8Title
        {
            get { return rep8TitleField; }
            set { rep8TitleField = value; }
        }

        /// <remarks/>
        public string Rep9Name
        {
            get { return rep9NameField; }
            set { rep9NameField = value; }
        }

        /// <remarks/>
        public string Rep9Company
        {
            get { return rep9CompanyField; }
            set { rep9CompanyField = value; }
        }

        /// <remarks/>
        public string Rep9Title
        {
            get { return rep9TitleField; }
            set { rep9TitleField = value; }
        }

        /// <remarks/>
        public string Rep10Name
        {
            get { return rep10NameField; }
            set { rep10NameField = value; }
        }

        /// <remarks/>
        public string Rep10Company
        {
            get { return rep10CompanyField; }
            set { rep10CompanyField = value; }
        }

        /// <remarks/>
        public string Rep10Title
        {
            get { return rep10TitleField; }
            set { rep10TitleField = value; }
        }

        /// <remarks/>
        public string ReferenceNo
        {
            get { return referenceNoField; }
            set { referenceNoField = value; }
        }

        /// <remarks/>
        public string PO
        {
            get { return poField; }
            set { poField = value; }
        }

        /// <remarks/>
        public string OBYMailerClass
        {
            get { return oBYMailerClassField; }
            set { oBYMailerClassField = value; }
        }

        /// <remarks/>
        public string STOMailerClass
        {
            get { return sTOMailerClassField; }
            set { sTOMailerClassField = value; }
        }

        /// <remarks/>
        public string PickSlipId
        {
            get { return pickSlipIdField; }
            set { pickSlipIdField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? ProductOwner
        {
            get { return productOwnerField; }
            set { productOwnerField = value; }
        }

        /// <remarks/>
        public string ProjectId
        {
            get { return projectIdField; }
            set { projectIdField = value; }
        }

        /// <remarks/>
        public string ProjectDesc
        {
            get { return projectDescField; }
            set { projectDescField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class BillingDetail : PMObject
    {

        private DateTime? shippingDateField;

        private string orderIDField;

        private int shippingOrderIDField;

        private string pickPackIDField;

        private int? lineItemQtyField;

        private System.Nullable<decimal> chargePerLineField;

        private int? packageQtyField;

        private System.Nullable<decimal> packageCostField;

        private int? creditCardTransQtyField;

        private System.Nullable<decimal> creditCardTransCostField;

        private int? billingUnitsQtyField;

        private System.Nullable<decimal> billingUnitsCostField;

        private System.Nullable<decimal> bundleCostField;

        private System.Nullable<decimal> publishedFreightField;

        private System.Nullable<decimal> actualFreightField;

        private System.Nullable<decimal> markedUpFreightField;

        private System.Nullable<decimal> chargePerShipmentField;

        private decimal totalMerchandiseChargeField;

        private string projectIDField;

        private string projectDescriptionField;

        private string orderReferenceNumberField;

        private string ordersPONumberField;

        private string oBYNameField;

        private string oBYCOMPANYField;

        private string oBYADDR1Field;

        private string oBYADDR2Field;

        private string oBYADDR3Field;

        private string oBYCITYField;

        private string oBYSTField;

        private string oBYZIPField;

        private string oBYCOUNTRYField;

        private string sTONameField;

        private string sTOCOMPANYField;

        private string sTOADDR1Field;

        private string sTOADDR2Field;

        private string sTOADDR3Field;

        private string sTOCITYField;

        private string sTOSTField;

        private string sTOZIPField;

        private string sTOCOUNTRYField;

        private string bTONameField;

        private string bTOCOMPANYField;

        private string bTOADDR1Field;

        private string bTOADDR2Field;

        private string bTOADDR3Field;

        private string bTOCITYField;

        private string bTOSTField;

        private string bTOZIPField;

        private string bTOCOUNTRYField;

        private string sOURCEField;

        private string orderStreamField;

        private System.Nullable<decimal> shippingHandlingChargesField;

        private string varFld1Field;

        private string varVal1Field;

        private string varFld2Field;

        private string varVal2Field;

        private string varFld3Field;

        private string varVal3Field;

        private string varFld4Field;

        private string varVal4Field;

        private string varFld5Field;

        private string varVal5Field;

        private string varFld6Field;

        private string varVal6Field;

        private string varFld7Field;

        private string varVal7Field;

        private string varFld8Field;

        private string varVal8Field;

        private string varFld9Field;

        private string varVal9Field;

        private string varFld10Field;

        private string varVal10Field;

        private string firstShipCarrierField;

        private string firstShipSvcField;

        private int bUKeyField;

        private string shipmentOrderTypeField;

        private string rushOrderField;

        private System.Nullable<decimal> rushSurchargeField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? ShippingDate
        {
            get { return shippingDateField; }
            set { shippingDateField = value; }
        }

        /// <remarks/>
        public string OrderID
        {
            get { return orderIDField; }
            set { orderIDField = value; }
        }

        /// <remarks/>
        public int ShippingOrderID
        {
            get { return shippingOrderIDField; }
            set { shippingOrderIDField = value; }
        }

        /// <remarks/>
        public string PickPackID
        {
            get { return pickPackIDField; }
            set { pickPackIDField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? LineItemQty
        {
            get { return lineItemQtyField; }
            set { lineItemQtyField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> ChargePerLine
        {
            get { return chargePerLineField; }
            set { chargePerLineField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? PackageQty
        {
            get { return packageQtyField; }
            set { packageQtyField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> PackageCost
        {
            get { return packageCostField; }
            set { packageCostField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? CreditCardTransQty
        {
            get { return creditCardTransQtyField; }
            set { creditCardTransQtyField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> CreditCardTransCost
        {
            get { return creditCardTransCostField; }
            set { creditCardTransCostField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? BillingUnitsQty
        {
            get { return billingUnitsQtyField; }
            set { billingUnitsQtyField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> BillingUnitsCost
        {
            get { return billingUnitsCostField; }
            set { billingUnitsCostField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> BundleCost
        {
            get { return bundleCostField; }
            set { bundleCostField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> PublishedFreight
        {
            get { return publishedFreightField; }
            set { publishedFreightField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> ActualFreight
        {
            get { return actualFreightField; }
            set { actualFreightField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> MarkedUpFreight
        {
            get { return markedUpFreightField; }
            set { markedUpFreightField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> ChargePerShipment
        {
            get { return chargePerShipmentField; }
            set { chargePerShipmentField = value; }
        }

        /// <remarks/>
        public decimal TotalMerchandiseCharge
        {
            get { return totalMerchandiseChargeField; }
            set { totalMerchandiseChargeField = value; }
        }

        /// <remarks/>
        public string ProjectID
        {
            get { return projectIDField; }
            set { projectIDField = value; }
        }

        /// <remarks/>
        public string ProjectDescription
        {
            get { return projectDescriptionField; }
            set { projectDescriptionField = value; }
        }

        /// <remarks/>
        public string OrderReferenceNumber
        {
            get { return orderReferenceNumberField; }
            set { orderReferenceNumberField = value; }
        }

        /// <remarks/>
        public string OrdersPONumber
        {
            get { return ordersPONumberField; }
            set { ordersPONumberField = value; }
        }

        /// <remarks/>
        public string OBYName
        {
            get { return oBYNameField; }
            set { oBYNameField = value; }
        }

        /// <remarks/>
        public string OBYCOMPANY
        {
            get { return oBYCOMPANYField; }
            set { oBYCOMPANYField = value; }
        }

        /// <remarks/>
        public string OBYADDR1
        {
            get { return oBYADDR1Field; }
            set { oBYADDR1Field = value; }
        }

        /// <remarks/>
        public string OBYADDR2
        {
            get { return oBYADDR2Field; }
            set { oBYADDR2Field = value; }
        }

        /// <remarks/>
        public string OBYADDR3
        {
            get { return oBYADDR3Field; }
            set { oBYADDR3Field = value; }
        }

        /// <remarks/>
        public string OBYCITY
        {
            get { return oBYCITYField; }
            set { oBYCITYField = value; }
        }

        /// <remarks/>
        public string OBYST
        {
            get { return oBYSTField; }
            set { oBYSTField = value; }
        }

        /// <remarks/>
        public string OBYZIP
        {
            get { return oBYZIPField; }
            set { oBYZIPField = value; }
        }

        /// <remarks/>
        public string OBYCOUNTRY
        {
            get { return oBYCOUNTRYField; }
            set { oBYCOUNTRYField = value; }
        }

        /// <remarks/>
        public string STOName
        {
            get { return sTONameField; }
            set { sTONameField = value; }
        }

        /// <remarks/>
        public string STOCOMPANY
        {
            get { return sTOCOMPANYField; }
            set { sTOCOMPANYField = value; }
        }

        /// <remarks/>
        public string STOADDR1
        {
            get { return sTOADDR1Field; }
            set { sTOADDR1Field = value; }
        }

        /// <remarks/>
        public string STOADDR2
        {
            get { return sTOADDR2Field; }
            set { sTOADDR2Field = value; }
        }

        /// <remarks/>
        public string STOADDR3
        {
            get { return sTOADDR3Field; }
            set { sTOADDR3Field = value; }
        }

        /// <remarks/>
        public string STOCITY
        {
            get { return sTOCITYField; }
            set { sTOCITYField = value; }
        }

        /// <remarks/>
        public string STOST
        {
            get { return sTOSTField; }
            set { sTOSTField = value; }
        }

        /// <remarks/>
        public string STOZIP
        {
            get { return sTOZIPField; }
            set { sTOZIPField = value; }
        }

        /// <remarks/>
        public string STOCOUNTRY
        {
            get { return sTOCOUNTRYField; }
            set { sTOCOUNTRYField = value; }
        }

        /// <remarks/>
        public string BTOName
        {
            get { return bTONameField; }
            set { bTONameField = value; }
        }

        /// <remarks/>
        public string BTOCOMPANY
        {
            get { return bTOCOMPANYField; }
            set { bTOCOMPANYField = value; }
        }

        /// <remarks/>
        public string BTOADDR1
        {
            get { return bTOADDR1Field; }
            set { bTOADDR1Field = value; }
        }

        /// <remarks/>
        public string BTOADDR2
        {
            get { return bTOADDR2Field; }
            set { bTOADDR2Field = value; }
        }

        /// <remarks/>
        public string BTOADDR3
        {
            get { return bTOADDR3Field; }
            set { bTOADDR3Field = value; }
        }

        /// <remarks/>
        public string BTOCITY
        {
            get { return bTOCITYField; }
            set { bTOCITYField = value; }
        }

        /// <remarks/>
        public string BTOST
        {
            get { return bTOSTField; }
            set { bTOSTField = value; }
        }

        /// <remarks/>
        public string BTOZIP
        {
            get { return bTOZIPField; }
            set { bTOZIPField = value; }
        }

        /// <remarks/>
        public string BTOCOUNTRY
        {
            get { return bTOCOUNTRYField; }
            set { bTOCOUNTRYField = value; }
        }

        /// <remarks/>
        public string SOURCE
        {
            get { return sOURCEField; }
            set { sOURCEField = value; }
        }

        /// <remarks/>
        public string OrderStream
        {
            get { return orderStreamField; }
            set { orderStreamField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> ShippingHandlingCharges
        {
            get { return shippingHandlingChargesField; }
            set { shippingHandlingChargesField = value; }
        }

        /// <remarks/>
        public string VarFld1
        {
            get { return varFld1Field; }
            set { varFld1Field = value; }
        }

        /// <remarks/>
        public string VarVal1
        {
            get { return varVal1Field; }
            set { varVal1Field = value; }
        }

        /// <remarks/>
        public string VarFld2
        {
            get { return varFld2Field; }
            set { varFld2Field = value; }
        }

        /// <remarks/>
        public string VarVal2
        {
            get { return varVal2Field; }
            set { varVal2Field = value; }
        }

        /// <remarks/>
        public string VarFld3
        {
            get { return varFld3Field; }
            set { varFld3Field = value; }
        }

        /// <remarks/>
        public string VarVal3
        {
            get { return varVal3Field; }
            set { varVal3Field = value; }
        }

        /// <remarks/>
        public string VarFld4
        {
            get { return varFld4Field; }
            set { varFld4Field = value; }
        }

        /// <remarks/>
        public string VarVal4
        {
            get { return varVal4Field; }
            set { varVal4Field = value; }
        }

        /// <remarks/>
        public string VarFld5
        {
            get { return varFld5Field; }
            set { varFld5Field = value; }
        }

        /// <remarks/>
        public string VarVal5
        {
            get { return varVal5Field; }
            set { varVal5Field = value; }
        }

        /// <remarks/>
        public string VarFld6
        {
            get { return varFld6Field; }
            set { varFld6Field = value; }
        }

        /// <remarks/>
        public string VarVal6
        {
            get { return varVal6Field; }
            set { varVal6Field = value; }
        }

        /// <remarks/>
        public string VarFld7
        {
            get { return varFld7Field; }
            set { varFld7Field = value; }
        }

        /// <remarks/>
        public string VarVal7
        {
            get { return varVal7Field; }
            set { varVal7Field = value; }
        }

        /// <remarks/>
        public string VarFld8
        {
            get { return varFld8Field; }
            set { varFld8Field = value; }
        }

        /// <remarks/>
        public string VarVal8
        {
            get { return varVal8Field; }
            set { varVal8Field = value; }
        }

        /// <remarks/>
        public string VarFld9
        {
            get { return varFld9Field; }
            set { varFld9Field = value; }
        }

        /// <remarks/>
        public string VarVal9
        {
            get { return varVal9Field; }
            set { varVal9Field = value; }
        }

        /// <remarks/>
        public string VarFld10
        {
            get { return varFld10Field; }
            set { varFld10Field = value; }
        }

        /// <remarks/>
        public string VarVal10
        {
            get { return varVal10Field; }
            set { varVal10Field = value; }
        }

        /// <remarks/>
        public string FirstShipCarrier
        {
            get { return firstShipCarrierField; }
            set { firstShipCarrierField = value; }
        }

        /// <remarks/>
        public string FirstShipSvc
        {
            get { return firstShipSvcField; }
            set { firstShipSvcField = value; }
        }

        /// <remarks/>
        public int BUKey
        {
            get { return bUKeyField; }
            set { bUKeyField = value; }
        }

        /// <remarks/>
        public string ShipmentOrderType
        {
            get { return shipmentOrderTypeField; }
            set { shipmentOrderTypeField = value; }
        }

        /// <remarks/>
        public string RushOrder
        {
            get { return rushOrderField; }
            set { rushOrderField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> RushSurcharge
        {
            get { return rushSurchargeField; }
            set { rushSurchargeField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class PMSystemCategory : PMObject
    {

        private int? seqIDField;

        private string descriptionField;

        private int? sequenceField;

        private string abbreviationField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Sequence
        {
            get { return sequenceField; }
            set { sequenceField = value; }
        }

        /// <remarks/>
        public string Abbreviation
        {
            get { return abbreviationField; }
            set { abbreviationField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class Profile : PMObject
    {

        private int? seqIDField;

        private string descriptionField;

        private PMSystemCategory systemCategoryField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        public PMSystemCategory SystemCategory
        {
            get { return systemCategoryField; }
            set { systemCategoryField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class CategoryGroupDetail : PMObject
    {

        private int? seqIDField;

        private CategoryGroup categoryGroupField;

        private OfferSortGroupXRef sortGroupField;

        private System.Nullable<Access> accessFlagField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public CategoryGroup CategoryGroup
        {
            get { return categoryGroupField; }
            set { categoryGroupField = value; }
        }

        /// <remarks/>
        public OfferSortGroupXRef SortGroup
        {
            get { return sortGroupField; }
            set { sortGroupField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<Access> AccessFlag
        {
            get { return accessFlagField; }
            set { accessFlagField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class CategoryGroup : PMObject
    {

        private int? seqIDField;

        private string descriptionField;

        private CategoryGroupDetail[] detailsField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        public CategoryGroupDetail[] Details
        {
            get { return detailsField; }
            set { detailsField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum Access
    {

        /// <remarks/>
        No,

        /// <remarks/>
        Yes,

        /// <remarks/>
        Restricted,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class RestrictedOwner : PMObject
    {

        private int? seqIDField;

        private Owner ownerField;

        private bool? accessField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public Owner Owner
        {
            get { return ownerField; }
            set { ownerField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? Access
        {
            get { return accessField; }
            set { accessField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class UserCategoryRestriction : PMObject
    {

        private int? seqIDField;

        private CustomCategory customCategoryField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public CustomCategory CustomCategory
        {
            get { return customCategoryField; }
            set { customCategoryField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ApprovalGroup : PMObject
    {

        private int? seqIDField;

        private string descriptionField;

        private bool? isFinalApprovalField;

        private int? sequenceField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? IsFinalApproval
        {
            get { return isFinalApprovalField; }
            set { isFinalApprovalField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Sequence
        {
            get { return sequenceField; }
            set { sequenceField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ApprovalGroupUser : PMObject
    {

        private int? seqIDField;

        private ApprovalGroup approvalGroupField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public ApprovalGroup ApprovalGroup
        {
            get { return approvalGroupField; }
            set { approvalGroupField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ORDOBY : PMObject
    {

        private ApprovalGroupUser[] approvalGroupUsersField;

        private UserCategoryRestriction[] customCategoryRestrictionsField;

        private RestrictedOwner[] restrictedOwnersField;

        private int? seqIDField;

        private string customerIDField;

        private string passwordField;

        private OrderEntryView orderEntryViewField;

        private Person mailerField;

        private string cCEmailField;

        private string bCCEmailField;

        private bool? clientField;

        private bool? noOfferQuantityDropDownField;

        private bool? requirePasswordNextLoginField;

        private CategoryGroup categoryAccessGroupField;

        private Profile dashboardProfileField;

        private bool? masterApproverStatusField;

        private bool? isInactiveField;

        /// <remarks/>
        public ApprovalGroupUser[] ApprovalGroupUsers
        {
            get { return approvalGroupUsersField; }
            set { approvalGroupUsersField = value; }
        }

        /// <remarks/>
        public UserCategoryRestriction[] CustomCategoryRestrictions
        {
            get { return customCategoryRestrictionsField; }
            set { customCategoryRestrictionsField = value; }
        }

        /// <remarks/>
        public RestrictedOwner[] RestrictedOwners
        {
            get { return restrictedOwnersField; }
            set { restrictedOwnersField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string CustomerID
        {
            get { return customerIDField; }
            set { customerIDField = value; }
        }

        /// <remarks/>
        public string Password
        {
            get { return passwordField; }
            set { passwordField = value; }
        }

        /// <remarks/>
        public OrderEntryView OrderEntryView
        {
            get { return orderEntryViewField; }
            set { orderEntryViewField = value; }
        }

        /// <remarks/>
        public Person Mailer
        {
            get { return mailerField; }
            set { mailerField = value; }
        }

        /// <remarks/>
        public string CCEmail
        {
            get { return cCEmailField; }
            set { cCEmailField = value; }
        }

        /// <remarks/>
        public string BCCEmail
        {
            get { return bCCEmailField; }
            set { bCCEmailField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? Client
        {
            get { return clientField; }
            set { clientField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? NoOfferQuantityDropDown
        {
            get { return noOfferQuantityDropDownField; }
            set { noOfferQuantityDropDownField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? RequirePasswordNextLogin
        {
            get { return requirePasswordNextLoginField; }
            set { requirePasswordNextLoginField = value; }
        }

        /// <remarks/>
        public CategoryGroup CategoryAccessGroup
        {
            get { return categoryAccessGroupField; }
            set { categoryAccessGroupField = value; }
        }

        /// <remarks/>
        public Profile DashboardProfile
        {
            get { return dashboardProfileField; }
            set { dashboardProfileField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? MasterApproverStatus
        {
            get { return masterApproverStatusField; }
            set { masterApproverStatusField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? IsInactive
        {
            get { return isInactiveField; }
            set { isInactiveField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class UserPreRegistered : PMObject
    {

        private int? seqIDField;

        private ORDOBY oRDOBYField;

        private string utilitiesCustomMenuTitleField;

        private string utilitiesCustomMenuLinkField;

        private string reportsCustomMenuTitleField;

        private string reportsCustomMenuLinkField;

        private int? peopFlagField;

        private bool? welcomeDefaultsField;

        private string welcomeHeadingField;

        private bool? displayGraphsField;

        private System.Nullable<StartingGraphType> startingGraphsField;

        private string welcomeTextField;

        private int? contactFlagField;

        private System.Nullable<short> graph1Field;

        private System.Nullable<short> graph2Field;

        private System.Nullable<short> graph3Field;

        private System.Nullable<short> graph4Field;

        private System.Nullable<ClassicGraphType> graph1TypeField;

        private System.Nullable<ClassicGraphType> graph2TypeField;

        private System.Nullable<ClassicGraphType> graph3TypeField;

        private System.Nullable<ClassicGraphType> graph4TypeField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public ORDOBY ORDOBY
        {
            get { return oRDOBYField; }
            set { oRDOBYField = value; }
        }

        /// <remarks/>
        public string UtilitiesCustomMenuTitle
        {
            get { return utilitiesCustomMenuTitleField; }
            set { utilitiesCustomMenuTitleField = value; }
        }

        /// <remarks/>
        public string UtilitiesCustomMenuLink
        {
            get { return utilitiesCustomMenuLinkField; }
            set { utilitiesCustomMenuLinkField = value; }
        }

        /// <remarks/>
        public string ReportsCustomMenuTitle
        {
            get { return reportsCustomMenuTitleField; }
            set { reportsCustomMenuTitleField = value; }
        }

        /// <remarks/>
        public string ReportsCustomMenuLink
        {
            get { return reportsCustomMenuLinkField; }
            set { reportsCustomMenuLinkField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? PeopFlag
        {
            get { return peopFlagField; }
            set { peopFlagField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? WelcomeDefaults
        {
            get { return welcomeDefaultsField; }
            set { welcomeDefaultsField = value; }
        }

        /// <remarks/>
        public string WelcomeHeading
        {
            get { return welcomeHeadingField; }
            set { welcomeHeadingField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? DisplayGraphs
        {
            get { return displayGraphsField; }
            set { displayGraphsField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<StartingGraphType> StartingGraphs
        {
            get { return startingGraphsField; }
            set { startingGraphsField = value; }
        }

        /// <remarks/>
        public string WelcomeText
        {
            get { return welcomeTextField; }
            set { welcomeTextField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? ContactFlag
        {
            get { return contactFlagField; }
            set { contactFlagField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<short> Graph1
        {
            get { return graph1Field; }
            set { graph1Field = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<short> Graph2
        {
            get { return graph2Field; }
            set { graph2Field = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<short> Graph3
        {
            get { return graph3Field; }
            set { graph3Field = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<short> Graph4
        {
            get { return graph4Field; }
            set { graph4Field = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<ClassicGraphType> Graph1Type
        {
            get { return graph1TypeField; }
            set { graph1TypeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<ClassicGraphType> Graph2Type
        {
            get { return graph2TypeField; }
            set { graph2TypeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<ClassicGraphType> Graph3Type
        {
            get { return graph3TypeField; }
            set { graph3TypeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<ClassicGraphType> Graph4Type
        {
            get { return graph4TypeField; }
            set { graph4TypeField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum StartingGraphType
    {

        /// <remarks/>
        None,

        /// <remarks/>
        Classic,

        /// <remarks/>
        Dashboard,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [XmlType(Namespace = "http://sma-promail/")]
    public enum ClassicGraphType
    {

        /// <remarks/>
        None,

        /// <remarks/>
        TopOffers,

        /// <remarks/>
        OrderVolume5Weeks,

        /// <remarks/>
        TopBackorderd,

        /// <remarks/>
        OrderVolume13Months,

        /// <remarks/>
        UnapprovedOrders,

        /// <remarks/>
        InactivatedOffers,
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class PriceClassStructure : PMObject
    {

        private string descriptionField;

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class RangePrice : PMObject
    {

        private int? startField;

        private int? endField;

        private System.Nullable<decimal> priceField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Start
        {
            get { return startField; }
            set { startField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? End
        {
            get { return endField; }
            set { endField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> Price
        {
            get { return priceField; }
            set { priceField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class GetOfferResult : PMObject
    {

        private RestrictionInfo restriction1Field;

        private RestrictionInfo restriction2Field;

        private PriceInfo priceInfoField;

        private int? seqIDField;

        private string idField;

        private string descriptionField;

        private string sortKeyField;

        private int? orderMinimumField;

        private int? orderLimitField;

        private string imagePathField;

        private bool imageLocalField;

        private string fullImagePathField;

        private string pPLinkField;

        private string pdfLinkField;

        private DateTime? startDateField;

        private DateTime? endDateField;

        private string inactiveField;

        private string inactiveTextField;

        private string commentsField;

        private int? customAssemField;

        private System.Nullable<decimal> defaultPriceField;

        private PriceClassStructure priceClassStructureField;

        private int? availableField;

        private int? clusterCountField;

        private bool? taxableField;

        private int? oFFCSTSeqIDField;

        private int? prodCountField;

        private int? kitCountField;

        private int? pODCountField;

        private bool? dropShipField;

        private bool? custAssemField;

        private string otherLink1Field;

        private string otherLink2Field;

        private string otherLink3Field;

        private string fullImageTextField;

        private string pdfTextField;

        private string pPTTextField;

        private string other1TextField;

        private string other2TextField;

        private string other3TextField;

        private int? displayAvailableField;

        private int? orderUnavailableField;

        private string inStockTextField;

        private string outOfStockTextField;

        private int? otherOrderFlagField;

        private string orderLinkField;

        private string rCFld1Field;

        private string rCFld2Field;

        private string rCFld3Field;

        private int? oFFRCS_SeqIDField;

        private string oFFRCS_NDURLField;

        private string oFFRCS_NDRTNPARMField;

        private string oFFRCS_NDparm1Field;

        private string oFFRCS_NDParm2Field;

        private string oFFRCS_NDParm3Field;

        private string oFFRCS_EDITURLField;

        private string oFFRCS_EDITRTNPARMField;

        private string oFFRCS_EDITUIDPARMField;

        private string oFFRCS_EDITDOCIDPARMField;

        private string buttonTextField;

        private bool? allowQuantityChangeField;

        private bool? selQuantityOnlyField;

        private bool? oFFRCS_NEEDSTICKETField;

        private string oFFRCS_TicketRSPField;

        private string offrcs_enterdocurlField;

        private string offrcs_enterdocparmField;

        private bool? offrcs_passuserinfoField;

        private string addlSearchTextField;

        private bool? pFAllowReorderField;

        private int? pFReorderExpDaysField;

        private string oFFRCS_ReorderUrlField;

        private string oFFRCS_ReorderRtnParmField;

        private string oFFRCS_ReorderUIDParmField;

        private string oFFRCS_ReorderDocIDParmField;

        private OfferPriceType priceTypeField;

        private int? mainFeatureField;

        private string reorderButtonTextField;

        private string reorderLinkTextField;

        private string previousOrdersTextField;

        private string unitomDescriptionField;

        private DateTime? mainFeatureEndDateField;

        /// <remarks/>
        public RestrictionInfo Restriction1
        {
            get { return restriction1Field; }
            set { restriction1Field = value; }
        }

        /// <remarks/>
        public RestrictionInfo Restriction2
        {
            get { return restriction2Field; }
            set { restriction2Field = value; }
        }

        /// <remarks/>
        public PriceInfo PriceInfo
        {
            get { return priceInfoField; }
            set { priceInfoField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public string ID
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        public string Description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        public string SortKey
        {
            get { return sortKeyField; }
            set { sortKeyField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? OrderMinimum
        {
            get { return orderMinimumField; }
            set { orderMinimumField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? OrderLimit
        {
            get { return orderLimitField; }
            set { orderLimitField = value; }
        }

        /// <remarks/>
        public string ImagePath
        {
            get { return imagePathField; }
            set { imagePathField = value; }
        }

        /// <remarks/>
        public bool ImageLocal
        {
            get { return imageLocalField; }
            set { imageLocalField = value; }
        }

        /// <remarks/>
        public string FullImagePath
        {
            get { return fullImagePathField; }
            set { fullImagePathField = value; }
        }

        /// <remarks/>
        public string PPLink
        {
            get { return pPLinkField; }
            set { pPLinkField = value; }
        }

        /// <remarks/>
        public string PdfLink
        {
            get { return pdfLinkField; }
            set { pdfLinkField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? StartDate
        {
            get { return startDateField; }
            set { startDateField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? EndDate
        {
            get { return endDateField; }
            set { endDateField = value; }
        }

        /// <remarks/>
        public string Inactive
        {
            get { return inactiveField; }
            set { inactiveField = value; }
        }

        /// <remarks/>
        public string InactiveText
        {
            get { return inactiveTextField; }
            set { inactiveTextField = value; }
        }

        /// <remarks/>
        public string Comments
        {
            get { return commentsField; }
            set { commentsField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? CustomAssem
        {
            get { return customAssemField; }
            set { customAssemField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> DefaultPrice
        {
            get { return defaultPriceField; }
            set { defaultPriceField = value; }
        }

        /// <remarks/>
        public PriceClassStructure PriceClassStructure
        {
            get { return priceClassStructureField; }
            set { priceClassStructureField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Available
        {
            get { return availableField; }
            set { availableField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? ClusterCount
        {
            get { return clusterCountField; }
            set { clusterCountField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? Taxable
        {
            get { return taxableField; }
            set { taxableField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? OFFCSTSeqID
        {
            get { return oFFCSTSeqIDField; }
            set { oFFCSTSeqIDField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? ProdCount
        {
            get { return prodCountField; }
            set { prodCountField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? KitCount
        {
            get { return kitCountField; }
            set { kitCountField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? PODCount
        {
            get { return pODCountField; }
            set { pODCountField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? DropShip
        {
            get { return dropShipField; }
            set { dropShipField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? CustAssem
        {
            get { return custAssemField; }
            set { custAssemField = value; }
        }

        /// <remarks/>
        public string OtherLink1
        {
            get { return otherLink1Field; }
            set { otherLink1Field = value; }
        }

        /// <remarks/>
        public string OtherLink2
        {
            get { return otherLink2Field; }
            set { otherLink2Field = value; }
        }

        /// <remarks/>
        public string OtherLink3
        {
            get { return otherLink3Field; }
            set { otherLink3Field = value; }
        }

        /// <remarks/>
        public string FullImageText
        {
            get { return fullImageTextField; }
            set { fullImageTextField = value; }
        }

        /// <remarks/>
        public string PdfText
        {
            get { return pdfTextField; }
            set { pdfTextField = value; }
        }

        /// <remarks/>
        public string PPTText
        {
            get { return pPTTextField; }
            set { pPTTextField = value; }
        }

        /// <remarks/>
        public string Other1Text
        {
            get { return other1TextField; }
            set { other1TextField = value; }
        }

        /// <remarks/>
        public string Other2Text
        {
            get { return other2TextField; }
            set { other2TextField = value; }
        }

        /// <remarks/>
        public string Other3Text
        {
            get { return other3TextField; }
            set { other3TextField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? DisplayAvailable
        {
            get { return displayAvailableField; }
            set { displayAvailableField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? OrderUnavailable
        {
            get { return orderUnavailableField; }
            set { orderUnavailableField = value; }
        }

        /// <remarks/>
        public string InStockText
        {
            get { return inStockTextField; }
            set { inStockTextField = value; }
        }

        /// <remarks/>
        public string OutOfStockText
        {
            get { return outOfStockTextField; }
            set { outOfStockTextField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? OtherOrderFlag
        {
            get { return otherOrderFlagField; }
            set { otherOrderFlagField = value; }
        }

        /// <remarks/>
        public string OrderLink
        {
            get { return orderLinkField; }
            set { orderLinkField = value; }
        }

        /// <remarks/>
        public string RCFld1
        {
            get { return rCFld1Field; }
            set { rCFld1Field = value; }
        }

        /// <remarks/>
        public string RCFld2
        {
            get { return rCFld2Field; }
            set { rCFld2Field = value; }
        }

        /// <remarks/>
        public string RCFld3
        {
            get { return rCFld3Field; }
            set { rCFld3Field = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? OFFRCS_SeqID
        {
            get { return oFFRCS_SeqIDField; }
            set { oFFRCS_SeqIDField = value; }
        }

        /// <remarks/>
        public string OFFRCS_NDURL
        {
            get { return oFFRCS_NDURLField; }
            set { oFFRCS_NDURLField = value; }
        }

        /// <remarks/>
        public string OFFRCS_NDRTNPARM
        {
            get { return oFFRCS_NDRTNPARMField; }
            set { oFFRCS_NDRTNPARMField = value; }
        }

        /// <remarks/>
        public string OFFRCS_NDparm1
        {
            get { return oFFRCS_NDparm1Field; }
            set { oFFRCS_NDparm1Field = value; }
        }

        /// <remarks/>
        public string OFFRCS_NDParm2
        {
            get { return oFFRCS_NDParm2Field; }
            set { oFFRCS_NDParm2Field = value; }
        }

        /// <remarks/>
        public string OFFRCS_NDParm3
        {
            get { return oFFRCS_NDParm3Field; }
            set { oFFRCS_NDParm3Field = value; }
        }

        /// <remarks/>
        public string OFFRCS_EDITURL
        {
            get { return oFFRCS_EDITURLField; }
            set { oFFRCS_EDITURLField = value; }
        }

        /// <remarks/>
        public string OFFRCS_EDITRTNPARM
        {
            get { return oFFRCS_EDITRTNPARMField; }
            set { oFFRCS_EDITRTNPARMField = value; }
        }

        /// <remarks/>
        public string OFFRCS_EDITUIDPARM
        {
            get { return oFFRCS_EDITUIDPARMField; }
            set { oFFRCS_EDITUIDPARMField = value; }
        }

        /// <remarks/>
        public string OFFRCS_EDITDOCIDPARM
        {
            get { return oFFRCS_EDITDOCIDPARMField; }
            set { oFFRCS_EDITDOCIDPARMField = value; }
        }

        /// <remarks/>
        public string ButtonText
        {
            get { return buttonTextField; }
            set { buttonTextField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? AllowQuantityChange
        {
            get { return allowQuantityChangeField; }
            set { allowQuantityChangeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? SelQuantityOnly
        {
            get { return selQuantityOnlyField; }
            set { selQuantityOnlyField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? OFFRCS_NEEDSTICKET
        {
            get { return oFFRCS_NEEDSTICKETField; }
            set { oFFRCS_NEEDSTICKETField = value; }
        }

        /// <remarks/>
        public string OFFRCS_TicketRSP
        {
            get { return oFFRCS_TicketRSPField; }
            set { oFFRCS_TicketRSPField = value; }
        }

        /// <remarks/>
        public string offrcs_enterdocurl
        {
            get { return offrcs_enterdocurlField; }
            set { offrcs_enterdocurlField = value; }
        }

        /// <remarks/>
        public string offrcs_enterdocparm
        {
            get { return offrcs_enterdocparmField; }
            set { offrcs_enterdocparmField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? offrcs_passuserinfo
        {
            get { return offrcs_passuserinfoField; }
            set { offrcs_passuserinfoField = value; }
        }

        /// <remarks/>
        public string AddlSearchText
        {
            get { return addlSearchTextField; }
            set { addlSearchTextField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? PFAllowReorder
        {
            get { return pFAllowReorderField; }
            set { pFAllowReorderField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? PFReorderExpDays
        {
            get { return pFReorderExpDaysField; }
            set { pFReorderExpDaysField = value; }
        }

        /// <remarks/>
        public string OFFRCS_ReorderUrl
        {
            get { return oFFRCS_ReorderUrlField; }
            set { oFFRCS_ReorderUrlField = value; }
        }

        /// <remarks/>
        public string OFFRCS_ReorderRtnParm
        {
            get { return oFFRCS_ReorderRtnParmField; }
            set { oFFRCS_ReorderRtnParmField = value; }
        }

        /// <remarks/>
        public string OFFRCS_ReorderUIDParm
        {
            get { return oFFRCS_ReorderUIDParmField; }
            set { oFFRCS_ReorderUIDParmField = value; }
        }

        /// <remarks/>
        public string OFFRCS_ReorderDocIDParm
        {
            get { return oFFRCS_ReorderDocIDParmField; }
            set { oFFRCS_ReorderDocIDParmField = value; }
        }

        /// <remarks/>
        public OfferPriceType PriceType
        {
            get { return priceTypeField; }
            set { priceTypeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? MainFeature
        {
            get { return mainFeatureField; }
            set { mainFeatureField = value; }
        }

        /// <remarks/>
        public string ReorderButtonText
        {
            get { return reorderButtonTextField; }
            set { reorderButtonTextField = value; }
        }

        /// <remarks/>
        public string ReorderLinkText
        {
            get { return reorderLinkTextField; }
            set { reorderLinkTextField = value; }
        }

        /// <remarks/>
        public string PreviousOrdersText
        {
            get { return previousOrdersTextField; }
            set { previousOrdersTextField = value; }
        }

        /// <remarks/>
        public string UnitomDescription
        {
            get { return unitomDescriptionField; }
            set { unitomDescriptionField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? MainFeatureEndDate
        {
            get { return mainFeatureEndDateField; }
            set { mainFeatureEndDateField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class RestrictionInfo
    {

        private RestrictionType restrictionTypeField;

        private int quantityRemainingField;

        private int quantityOrderedField;

        /// <remarks/>
        public RestrictionType RestrictionType
        {
            get { return restrictionTypeField; }
            set { restrictionTypeField = value; }
        }

        /// <remarks/>
        public int QuantityRemaining
        {
            get { return quantityRemainingField; }
            set { quantityRemainingField = value; }
        }

        /// <remarks/>
        public int QuantityOrdered
        {
            get { return quantityOrderedField; }
            set { quantityOrderedField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class PriceInfo
    {

        private System.Nullable<decimal> defaultPriceField;

        private OfferPriceType unitsField;

        private RangePrice[] priceDetailsField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public System.Nullable<decimal> DefaultPrice
        {
            get { return defaultPriceField; }
            set { defaultPriceField = value; }
        }

        /// <remarks/>
        public OfferPriceType Units
        {
            get { return unitsField; }
            set { unitsField = value; }
        }

        /// <remarks/>
        public RangePrice[] PriceDetails
        {
            get { return priceDetailsField; }
            set { priceDetailsField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ExpectedArrivalComponent : PMObject
    {

        private int? seqIDField;

        private ExpectedArrival expectedArrivalField;

        private ProductVersion productVersionField;

        private int? quantityField;

        private DateTime? receiptDateField;

        private int? receiptQuantityField;

        private bool? isCompleteField;

        private int? aSNHDRSeqIDField;

        private bool? isCancelField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public ExpectedArrival ExpectedArrival
        {
            get { return expectedArrivalField; }
            set { expectedArrivalField = value; }
        }

        /// <remarks/>
        public ProductVersion ProductVersion
        {
            get { return productVersionField; }
            set { productVersionField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Quantity
        {
            get { return quantityField; }
            set { quantityField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? ReceiptDate
        {
            get { return receiptDateField; }
            set { receiptDateField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? ReceiptQuantity
        {
            get { return receiptQuantityField; }
            set { receiptQuantityField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? IsComplete
        {
            get { return isCompleteField; }
            set { isCompleteField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? ASNHDRSeqID
        {
            get { return aSNHDRSeqIDField; }
            set { aSNHDRSeqIDField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? IsCancel
        {
            get { return isCancelField; }
            set { isCancelField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ExpectedArrival : PMObject
    {

        private int? seqIDField;

        private PMSystem systemField;

        private DateTime? enteredAtField;

        private int? jobField;

        private Owner ownerField;

        private string ourPurchaseOrderField;

        private string customerPurchaseOrderField;

        private DateTime? arrivalTimeField;

        private string arrivalTimeAsStringField;

        private string shippingFromField;

        private string shippingMethodField;

        private string commentsField;

        private bool? isAssemblyField;

        private string userField;

        private DateTime? uTCArrivalDateTimeField;

        private DateTime? uTCEnteredAtField;

        private ExpectedArrivalComponent[] componentsField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }

        /// <remarks/>
        public PMSystem System
        {
            get { return systemField; }
            set { systemField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? EnteredAt
        {
            get { return enteredAtField; }
            set { enteredAtField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? Job
        {
            get { return jobField; }
            set { jobField = value; }
        }

        /// <remarks/>
        public Owner Owner
        {
            get { return ownerField; }
            set { ownerField = value; }
        }

        /// <remarks/>
        public string OurPurchaseOrder
        {
            get { return ourPurchaseOrderField; }
            set { ourPurchaseOrderField = value; }
        }

        /// <remarks/>
        public string CustomerPurchaseOrder
        {
            get { return customerPurchaseOrderField; }
            set { customerPurchaseOrderField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? ArrivalTime
        {
            get { return arrivalTimeField; }
            set { arrivalTimeField = value; }
        }

        /// <remarks/>
        public string ArrivalTimeAsString
        {
            get { return arrivalTimeAsStringField; }
            set { arrivalTimeAsStringField = value; }
        }

        /// <remarks/>
        public string ShippingFrom
        {
            get { return shippingFromField; }
            set { shippingFromField = value; }
        }

        /// <remarks/>
        public string ShippingMethod
        {
            get { return shippingMethodField; }
            set { shippingMethodField = value; }
        }

        /// <remarks/>
        public string Comments
        {
            get { return commentsField; }
            set { commentsField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public bool? IsAssembly
        {
            get { return isAssemblyField; }
            set { isAssemblyField = value; }
        }

        /// <remarks/>
        public string User
        {
            get { return userField; }
            set { userField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? UTCArrivalDateTime
        {
            get { return uTCArrivalDateTimeField; }
            set { uTCArrivalDateTimeField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public DateTime? UTCEnteredAt
        {
            get { return uTCEnteredAtField; }
            set { uTCEnteredAtField = value; }
        }

        /// <remarks/>
        public ExpectedArrivalComponent[] Components
        {
            get { return componentsField; }
            set { componentsField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class SizeColorClusterProductInfo
    {

        private int? productSeqIDField;

        private Size sizeField;

        private Color colorField;

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? ProductSeqID
        {
            get { return productSeqIDField; }
            set { productSeqIDField = value; }
        }

        /// <remarks/>
        public Size Size
        {
            get { return sizeField; }
            set { sizeField = value; }
        }

        /// <remarks/>
        public Color Color
        {
            get { return colorField; }
            set { colorField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class AddSizeColorClusterResult
    {

        private bool newClusterField;

        private int clusterSeqIDField;

        private int? offerSeqIDField;

        private SizeColorClusterProductInfo[] productsField;

        /// <remarks/>
        public bool NewCluster
        {
            get { return newClusterField; }
            set { newClusterField = value; }
        }

        /// <remarks/>
        public int ClusterSeqID
        {
            get { return clusterSeqIDField; }
            set { clusterSeqIDField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? OfferSeqID
        {
            get { return offerSeqIDField; }
            set { offerSeqIDField = value; }
        }

        /// <remarks/>
        public SizeColorClusterProductInfo[] Products
        {
            get { return productsField; }
            set { productsField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ProductTemplateHeader
    {

        private Owner ownerField;

        private string commentsField;

        private int? leadDaysField;

        /// <remarks/>
        public Owner Owner
        {
            get { return ownerField; }
            set { ownerField = value; }
        }

        /// <remarks/>
        public string Comments
        {
            get { return commentsField; }
            set { commentsField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? LeadDays
        {
            get { return leadDaysField; }
            set { leadDaysField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class ProductTemplate
    {

        private ProductTemplateHeader headerField;

        private ProductCharacteristics characteristicsField;

        private ProductValuation valuationField;

        private ProductWMSSystem[] warehouseSystemsField;

        private ProductOptional optionalInfoField;

        private ProductBillFactor[] billFactorsField;

        private ProductSort sortField;

        private ProductActivation[] activationField;

        private ProductBillingContainers productBillingContainersField;

        /// <remarks/>
        public ProductTemplateHeader Header
        {
            get { return headerField; }
            set { headerField = value; }
        }

        /// <remarks/>
        public ProductCharacteristics Characteristics
        {
            get { return characteristicsField; }
            set { characteristicsField = value; }
        }

        /// <remarks/>
        public ProductValuation Valuation
        {
            get { return valuationField; }
            set { valuationField = value; }
        }

        /// <remarks/>
        public ProductWMSSystem[] WarehouseSystems
        {
            get { return warehouseSystemsField; }
            set { warehouseSystemsField = value; }
        }

        /// <remarks/>
        public ProductOptional OptionalInfo
        {
            get { return optionalInfoField; }
            set { optionalInfoField = value; }
        }

        /// <remarks/>
        public ProductBillFactor[] BillFactors
        {
            get { return billFactorsField; }
            set { billFactorsField = value; }
        }

        /// <remarks/>
        public ProductSort Sort
        {
            get { return sortField; }
            set { sortField = value; }
        }

        /// <remarks/>
        public ProductActivation[] Activation
        {
            get { return activationField; }
            set { activationField = value; }
        }

        /// <remarks/>
        public ProductBillingContainers ProductBillingContainers
        {
            get { return productBillingContainersField; }
            set { productBillingContainersField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class SizeColorPair
    {

        private Color colorField;

        private Size sizeField;

        /// <remarks/>
        public Color Color
        {
            get { return colorField; }
            set { colorField = value; }
        }

        /// <remarks/>
        public Size Size
        {
            get { return sizeField; }
            set { sizeField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class SizeColorCluster
    {

        private ProductCluster clusterField;

        private Offer offerField;

        private SizeColorPair[] sizeColorPairsField;

        private SizeClass sizeClassField;

        private ColorClass colorClassField;

        private ProductTemplate productTemplateField;

        /// <remarks/>
        public ProductCluster Cluster
        {
            get { return clusterField; }
            set { clusterField = value; }
        }

        /// <remarks/>
        public Offer Offer
        {
            get { return offerField; }
            set { offerField = value; }
        }

        /// <remarks/>
        public SizeColorPair[] SizeColorPairs
        {
            get { return sizeColorPairsField; }
            set { sizeColorPairsField = value; }
        }

        /// <remarks/>
        public SizeClass SizeClass
        {
            get { return sizeClassField; }
            set { sizeClassField = value; }
        }

        /// <remarks/>
        public ColorClass ColorClass
        {
            get { return colorClassField; }
            set { colorClassField = value; }
        }

        /// <remarks/>
        public ProductTemplate ProductTemplate
        {
            get { return productTemplateField; }
            set { productTemplateField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class AddOrderResult
    {

        private int orderSeqIDField;

        private string orderIDField;

        /// <remarks/>
        public int OrderSeqID
        {
            get { return orderSeqIDField; }
            set { orderSeqIDField = value; }
        }

        /// <remarks/>
        public string OrderID
        {
            get { return orderIDField; }
            set { orderIDField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class AddPreRegUserResult
    {

        private bool newUserField;

        private int? preRegUserSeqIDField;

        /// <remarks/>
        public bool NewUser
        {
            get { return newUserField; }
            set { newUserField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? PreRegUserSeqID
        {
            get { return preRegUserSeqIDField; }
            set { preRegUserSeqIDField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class AddMailerResult
    {

        private bool newMailerField;

        private int? mailerSeqIDField;

        /// <remarks/>
        public bool NewMailer
        {
            get { return newMailerField; }
            set { newMailerField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? MailerSeqID
        {
            get { return mailerSeqIDField; }
            set { mailerSeqIDField = value; }
        }
    }

    /// <remarks/>
    [XmlInclude(typeof(PackagesType))]
    [XmlInclude(typeof(PickPackProductType))]
    [XmlInclude(typeof(PickPackType))]
    [XmlInclude(typeof(BOMProductType))]
    [XmlInclude(typeof(OfferType))]
    [XmlInclude(typeof(AddressType))]
    [XmlInclude(typeof(OrderStatusType))]
    [XmlInclude(typeof(OrderHeadType))]
    [XmlInclude(typeof(OrderInqRecord))]
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public abstract partial class MarshalByRefObject
    {
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class PackagesType : MarshalByRefObject
    {

        private string packageTypeField;

        private DateTime datePackedField;

        private DateTime dateShippedField;

        private DateTime dateVoidedField;

        private string carrierField;

        private string serviceField;

        private string actualServiceField;

        private double weightField;

        private double actualFreightField;

        private double actualAdditionalChgField;

        private string trackingIdField;

        private string pickUpIdField;

        private double insuredValueField;

        /// <remarks/>
        public string PackageType
        {
            get { return packageTypeField; }
            set { packageTypeField = value; }
        }

        /// <remarks/>
        public DateTime DatePacked
        {
            get { return datePackedField; }
            set { datePackedField = value; }
        }

        /// <remarks/>
        public DateTime DateShipped
        {
            get { return dateShippedField; }
            set { dateShippedField = value; }
        }

        /// <remarks/>
        public DateTime DateVoided
        {
            get { return dateVoidedField; }
            set { dateVoidedField = value; }
        }

        /// <remarks/>
        public string Carrier
        {
            get { return carrierField; }
            set { carrierField = value; }
        }

        /// <remarks/>
        public string Service
        {
            get { return serviceField; }
            set { serviceField = value; }
        }

        /// <remarks/>
        public string ActualService
        {
            get { return actualServiceField; }
            set { actualServiceField = value; }
        }

        /// <remarks/>
        public double Weight
        {
            get { return weightField; }
            set { weightField = value; }
        }

        /// <remarks/>
        public double ActualFreight
        {
            get { return actualFreightField; }
            set { actualFreightField = value; }
        }

        /// <remarks/>
        public double ActualAdditionalChg
        {
            get { return actualAdditionalChgField; }
            set { actualAdditionalChgField = value; }
        }

        /// <remarks/>
        public string TrackingId
        {
            get { return trackingIdField; }
            set { trackingIdField = value; }
        }

        /// <remarks/>
        public string PickUpId
        {
            get { return pickUpIdField; }
            set { pickUpIdField = value; }
        }

        /// <remarks/>
        public double InsuredValue
        {
            get { return insuredValueField; }
            set { insuredValueField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class PickPackProductType : MarshalByRefObject
    {

        private string productIdField;

        private string productDescField;

        private string revisionField;

        private int toShipQtyField;

        /// <remarks/>
        public string ProductId
        {
            get { return productIdField; }
            set { productIdField = value; }
        }

        /// <remarks/>
        public string ProductDesc
        {
            get { return productDescField; }
            set { productDescField = value; }
        }

        /// <remarks/>
        public string Revision
        {
            get { return revisionField; }
            set { revisionField = value; }
        }

        /// <remarks/>
        public int ToShipQty
        {
            get { return toShipQtyField; }
            set { toShipQtyField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class PickPackType : MarshalByRefObject
    {

        private string pickSlipIDField;

        private string pickPackIdField;

        private string statusField;

        private DateTime datePickedField;

        private double merchAmtField;

        private double shipHandAmtField;

        private double taxAmtField;

        private double discountAmtField;

        private string holdFlagField;

        private double specialHandlingField;

        private double additionalChargeField;

        private PickPackProductType[] pickPackProductsField;

        private PackagesType[] packagesField;

        /// <remarks/>
        public string PickSlipID
        {
            get { return pickSlipIDField; }
            set { pickSlipIDField = value; }
        }

        /// <remarks/>
        public string PickPackId
        {
            get { return pickPackIdField; }
            set { pickPackIdField = value; }
        }

        /// <remarks/>
        public string Status
        {
            get { return statusField; }
            set { statusField = value; }
        }

        /// <remarks/>
        public DateTime DatePicked
        {
            get { return datePickedField; }
            set { datePickedField = value; }
        }

        /// <remarks/>
        public double MerchAmt
        {
            get { return merchAmtField; }
            set { merchAmtField = value; }
        }

        /// <remarks/>
        public double ShipHandAmt
        {
            get { return shipHandAmtField; }
            set { shipHandAmtField = value; }
        }

        /// <remarks/>
        public double TaxAmt
        {
            get { return taxAmtField; }
            set { taxAmtField = value; }
        }

        /// <remarks/>
        public double DiscountAmt
        {
            get { return discountAmtField; }
            set { discountAmtField = value; }
        }

        /// <remarks/>
        public string HoldFlag
        {
            get { return holdFlagField; }
            set { holdFlagField = value; }
        }

        /// <remarks/>
        public double SpecialHandling
        {
            get { return specialHandlingField; }
            set { specialHandlingField = value; }
        }

        /// <remarks/>
        public double AdditionalCharge
        {
            get { return additionalChargeField; }
            set { additionalChargeField = value; }
        }

        /// <remarks/>
        public PickPackProductType[] PickPackProducts
        {
            get { return pickPackProductsField; }
            set { pickPackProductsField = value; }
        }

        /// <remarks/>
        public PackagesType[] Packages
        {
            get { return packagesField; }
            set { packagesField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class BOMProductType : MarshalByRefObject
    {

        private string productIdField;

        private string productDescField;

        private int orderQtyField;

        private int reservedQtyField;

        private int markedQtyField;

        private int pulledQtyField;

        private int canceledQtyField;

        private int backorderQtyField;

        /// <remarks/>
        public string ProductId
        {
            get { return productIdField; }
            set { productIdField = value; }
        }

        /// <remarks/>
        public string ProductDesc
        {
            get { return productDescField; }
            set { productDescField = value; }
        }

        /// <remarks/>
        public int OrderQty
        {
            get { return orderQtyField; }
            set { orderQtyField = value; }
        }

        /// <remarks/>
        public int ReservedQty
        {
            get { return reservedQtyField; }
            set { reservedQtyField = value; }
        }

        /// <remarks/>
        public int MarkedQty
        {
            get { return markedQtyField; }
            set { markedQtyField = value; }
        }

        /// <remarks/>
        public int PulledQty
        {
            get { return pulledQtyField; }
            set { pulledQtyField = value; }
        }

        /// <remarks/>
        public int CanceledQty
        {
            get { return canceledQtyField; }
            set { canceledQtyField = value; }
        }

        /// <remarks/>
        public int BackorderQty
        {
            get { return backorderQtyField; }
            set { backorderQtyField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OfferType : MarshalByRefObject
    {

        private string offerIdField;

        private string offerDescField;

        private int orderQtyField;

        private double unitPriceField;

        /// <remarks/>
        public string OfferId
        {
            get { return offerIdField; }
            set { offerIdField = value; }
        }

        /// <remarks/>
        public string OfferDesc
        {
            get { return offerDescField; }
            set { offerDescField = value; }
        }

        /// <remarks/>
        public int OrderQty
        {
            get { return orderQtyField; }
            set { orderQtyField = value; }
        }

        /// <remarks/>
        public double UnitPrice
        {
            get { return unitPriceField; }
            set { unitPriceField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class AddressType : MarshalByRefObject
    {

        private string uniqueIdField;

        private string contactField;

        private string companyField;

        private string address1Field;

        private string address2Field;

        private string address3Field;

        private string cityField;

        private string stateField;

        private string postalCodeField;

        private string countryField;

        private string phoneField;

        private string faxField;

        private string emailField;

        /// <remarks/>
        public string UniqueId
        {
            get { return uniqueIdField; }
            set { uniqueIdField = value; }
        }

        /// <remarks/>
        public string Contact
        {
            get { return contactField; }
            set { contactField = value; }
        }

        /// <remarks/>
        public string Company
        {
            get { return companyField; }
            set { companyField = value; }
        }

        /// <remarks/>
        public string Address1
        {
            get { return address1Field; }
            set { address1Field = value; }
        }

        /// <remarks/>
        public string Address2
        {
            get { return address2Field; }
            set { address2Field = value; }
        }

        /// <remarks/>
        public string Address3
        {
            get { return address3Field; }
            set { address3Field = value; }
        }

        /// <remarks/>
        public string City
        {
            get { return cityField; }
            set { cityField = value; }
        }

        /// <remarks/>
        public string State
        {
            get { return stateField; }
            set { stateField = value; }
        }

        /// <remarks/>
        public string PostalCode
        {
            get { return postalCodeField; }
            set { postalCodeField = value; }
        }

        /// <remarks/>
        public string Country
        {
            get { return countryField; }
            set { countryField = value; }
        }

        /// <remarks/>
        public string Phone
        {
            get { return phoneField; }
            set { phoneField = value; }
        }

        /// <remarks/>
        public string Fax
        {
            get { return faxField; }
            set { faxField = value; }
        }

        /// <remarks/>
        public string Email
        {
            get { return emailField; }
            set { emailField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OrderStatusType : MarshalByRefObject
    {

        private bool unapprovedField;

        private bool unprocessedField;

        private bool acceptedField;

        private bool holdField;

        private bool deniedCreditField;

        private bool processedField;

        private bool pendingField;

        private bool pickedField;

        private bool backorderedField;

        private bool shippedField;

        private bool canceledField;

        private bool completeField;

        /// <remarks/>
        public bool Unapproved
        {
            get { return unapprovedField; }
            set { unapprovedField = value; }
        }

        /// <remarks/>
        public bool Unprocessed
        {
            get { return unprocessedField; }
            set { unprocessedField = value; }
        }

        /// <remarks/>
        public bool Accepted
        {
            get { return acceptedField; }
            set { acceptedField = value; }
        }

        /// <remarks/>
        public bool Hold
        {
            get { return holdField; }
            set { holdField = value; }
        }

        /// <remarks/>
        public bool DeniedCredit
        {
            get { return deniedCreditField; }
            set { deniedCreditField = value; }
        }

        /// <remarks/>
        public bool Processed
        {
            get { return processedField; }
            set { processedField = value; }
        }

        /// <remarks/>
        public bool Pending
        {
            get { return pendingField; }
            set { pendingField = value; }
        }

        /// <remarks/>
        public bool Picked
        {
            get { return pickedField; }
            set { pickedField = value; }
        }

        /// <remarks/>
        public bool Backordered
        {
            get { return backorderedField; }
            set { backorderedField = value; }
        }

        /// <remarks/>
        public bool Shipped
        {
            get { return shippedField; }
            set { shippedField = value; }
        }

        /// <remarks/>
        public bool Canceled
        {
            get { return canceledField; }
            set { canceledField = value; }
        }

        /// <remarks/>
        public bool Complete
        {
            get { return completeField; }
            set { completeField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OrderHeadType : MarshalByRefObject
    {

        private string orderIdField;

        private DateTime entryDateField;

        private DateTime uploadDateField;

        private string viewField;

        private string streamField;

        private string referenceNoField;

        private string campaignIdField;

        private string poNumField;

        private string responseMethodField;

        private string sourceField;

        private string sourceDetailField;

        private string orderCommentField;

        private string carrierField;

        private string serviceField;

        private string shipCommentField;

        private string needByField;

        private string rushFlagField;

        private OrderStatusType statusField;

        private string paymentTypeField;

        private string priceClassField;

        private string cCNumberField;

        private string expirationDateField;

        private double shipHandChargeField;

        private double additionalChargeField;

        private double discountPctField;

        private double taxPctField;

        private double specialHandChargeField;

        private string shippingOptionField;

        private double shippingOptionChargeField;

        /// <remarks/>
        public string OrderId
        {
            get { return orderIdField; }
            set { orderIdField = value; }
        }

        /// <remarks/>
        public DateTime EntryDate
        {
            get { return entryDateField; }
            set { entryDateField = value; }
        }

        /// <remarks/>
        public DateTime UploadDate
        {
            get { return uploadDateField; }
            set { uploadDateField = value; }
        }

        /// <remarks/>
        public string View
        {
            get { return viewField; }
            set { viewField = value; }
        }

        /// <remarks/>
        public string Stream
        {
            get { return streamField; }
            set { streamField = value; }
        }

        /// <remarks/>
        public string ReferenceNo
        {
            get { return referenceNoField; }
            set { referenceNoField = value; }
        }

        /// <remarks/>
        public string CampaignId
        {
            get { return campaignIdField; }
            set { campaignIdField = value; }
        }

        /// <remarks/>
        public string PoNum
        {
            get { return poNumField; }
            set { poNumField = value; }
        }

        /// <remarks/>
        public string ResponseMethod
        {
            get { return responseMethodField; }
            set { responseMethodField = value; }
        }

        /// <remarks/>
        public string Source
        {
            get { return sourceField; }
            set { sourceField = value; }
        }

        /// <remarks/>
        public string SourceDetail
        {
            get { return sourceDetailField; }
            set { sourceDetailField = value; }
        }

        /// <remarks/>
        public string OrderComment
        {
            get { return orderCommentField; }
            set { orderCommentField = value; }
        }

        /// <remarks/>
        public string Carrier
        {
            get { return carrierField; }
            set { carrierField = value; }
        }

        /// <remarks/>
        public string Service
        {
            get { return serviceField; }
            set { serviceField = value; }
        }

        /// <remarks/>
        public string ShipComment
        {
            get { return shipCommentField; }
            set { shipCommentField = value; }
        }

        /// <remarks/>
        public string NeedBy
        {
            get { return needByField; }
            set { needByField = value; }
        }

        /// <remarks/>
        public string RushFlag
        {
            get { return rushFlagField; }
            set { rushFlagField = value; }
        }

        /// <remarks/>
        public OrderStatusType Status
        {
            get { return statusField; }
            set { statusField = value; }
        }

        /// <remarks/>
        public string PaymentType
        {
            get { return paymentTypeField; }
            set { paymentTypeField = value; }
        }

        /// <remarks/>
        public string PriceClass
        {
            get { return priceClassField; }
            set { priceClassField = value; }
        }

        /// <remarks/>
        public string CCNumber
        {
            get { return cCNumberField; }
            set { cCNumberField = value; }
        }

        /// <remarks/>
        public string ExpirationDate
        {
            get { return expirationDateField; }
            set { expirationDateField = value; }
        }

        /// <remarks/>
        public double ShipHandCharge
        {
            get { return shipHandChargeField; }
            set { shipHandChargeField = value; }
        }

        /// <remarks/>
        public double AdditionalCharge
        {
            get { return additionalChargeField; }
            set { additionalChargeField = value; }
        }

        /// <remarks/>
        public double DiscountPct
        {
            get { return discountPctField; }
            set { discountPctField = value; }
        }

        /// <remarks/>
        public double TaxPct
        {
            get { return taxPctField; }
            set { taxPctField = value; }
        }

        /// <remarks/>
        public double SpecialHandCharge
        {
            get { return specialHandChargeField; }
            set { specialHandChargeField = value; }
        }

        /// <remarks/>
        public string ShippingOption
        {
            get { return shippingOptionField; }
            set { shippingOptionField = value; }
        }

        /// <remarks/>
        public double ShippingOptionCharge
        {
            get { return shippingOptionChargeField; }
            set { shippingOptionChargeField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class OrderInqRecord : MarshalByRefObject
    {

        private OrderHeadType ordHeadField;

        private AddressType shipToInfoField;

        private AddressType orderedByField;

        private AddressType billToInfoField;

        private OfferType[] offerInfoField;

        private BOMProductType[] billOfMaterialsField;

        private PickPackType[] shippingOrdersField;

        /// <remarks/>
        public OrderHeadType OrdHead
        {
            get { return ordHeadField; }
            set { ordHeadField = value; }
        }

        /// <remarks/>
        public AddressType ShipToInfo
        {
            get { return shipToInfoField; }
            set { shipToInfoField = value; }
        }

        /// <remarks/>
        public AddressType OrderedBy
        {
            get { return orderedByField; }
            set { orderedByField = value; }
        }

        /// <remarks/>
        public AddressType BillToInfo
        {
            get { return billToInfoField; }
            set { billToInfoField = value; }
        }

        /// <remarks/>
        public OfferType[] OfferInfo
        {
            get { return offerInfoField; }
            set { offerInfoField = value; }
        }

        /// <remarks/>
        public BOMProductType[] BillOfMaterials
        {
            get { return billOfMaterialsField; }
            set { billOfMaterialsField = value; }
        }

        /// <remarks/>
        public PickPackType[] ShippingOrders
        {
            get { return shippingOrdersField; }
            set { shippingOrdersField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class CancelExpectedArrivalResult
    {
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class SaveExpectedArrivalResult
    {

        private bool isNewField;

        private int? seqIDField;

        /// <remarks/>
        public bool IsNew
        {
            get { return isNewField; }
            set { isNewField = value; }
        }

        /// <remarks/>
        [XmlElement(IsNullable = true)]
        public int? SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class AddPurchaseOrderResult
    {

        private bool isNewField;

        private int seqIDField;

        /// <remarks/>
        public bool IsNew
        {
            get { return isNewField; }
            set { isNewField = value; }
        }

        /// <remarks/>
        public int SeqID
        {
            get { return seqIDField; }
            set { seqIDField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class SaveOfferResult
    {

        private bool newOfferField;

        private int offerSeqIDField;

        /// <remarks/>
        public bool NewOffer
        {
            get { return newOfferField; }
            set { newOfferField = value; }
        }

        /// <remarks/>
        public int OfferSeqID
        {
            get { return offerSeqIDField; }
            set { offerSeqIDField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    public partial class AddProductResult
    {

        private bool newProductField;

        private int productSeqIDField;

        /// <remarks/>
        public bool NewProduct
        {
            get { return newProductField; }
            set { newProductField = value; }
        }

        /// <remarks/>
        public int ProductSeqID
        {
            get { return productSeqIDField; }
            set { productSeqIDField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [Serializable()]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    [XmlType(Namespace = "http://sma-promail/")]
    [XmlRoot(Namespace = "http://sma-promail/", IsNullable = false)]
    public partial class DebugHeader : SoapHeader
    {

        private bool debugField;

        private string requestField;

        private System.Xml.XmlAttribute[] anyAttrField;

        /// <remarks/>
        public bool Debug
        {
            get { return debugField; }
            set { debugField = value; }
        }

        /// <remarks/>
        public string Request
        {
            get { return requestField; }
            set { requestField = value; }
        }

        /// <remarks/>
        [XmlAnyAttribute()]
        public System.Xml.XmlAttribute[] AnyAttr
        {
            get { return anyAttrField; }
            set { anyAttrField = value; }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    public delegate void AddProductCompletedEventHandler(object sender, AddProductCompletedEventArgs e);

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class AddProductCompletedEventArgs : AsyncCompletedEventArgs
    {

        private object[] results;

        internal AddProductCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public AddProductResult Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return ((AddProductResult) (results[0]));
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    public delegate void SaveOfferCompletedEventHandler(object sender, SaveOfferCompletedEventArgs e);

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class SaveOfferCompletedEventArgs : AsyncCompletedEventArgs
    {

        private object[] results;

        internal SaveOfferCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public SaveOfferResult Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return ((SaveOfferResult) (results[0]));
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    public delegate void AddPurchaseOrderCompletedEventHandler(object sender, AddPurchaseOrderCompletedEventArgs e);

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class AddPurchaseOrderCompletedEventArgs : AsyncCompletedEventArgs
    {

        private object[] results;

        internal AddPurchaseOrderCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public AddPurchaseOrderResult Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return ((AddPurchaseOrderResult) (results[0]));
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    public delegate void SaveExpectedArrivalCompletedEventHandler(object sender, SaveExpectedArrivalCompletedEventArgs e);

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class SaveExpectedArrivalCompletedEventArgs : AsyncCompletedEventArgs
    {

        private object[] results;

        internal SaveExpectedArrivalCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public SaveExpectedArrivalResult Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return ((SaveExpectedArrivalResult) (results[0]));
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    public delegate void CancelExpectedArrivalCompletedEventHandler(object sender, CancelExpectedArrivalCompletedEventArgs e);

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class CancelExpectedArrivalCompletedEventArgs : AsyncCompletedEventArgs
    {

        private object[] results;

        internal CancelExpectedArrivalCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public CancelExpectedArrivalResult Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return ((CancelExpectedArrivalResult) (results[0]));
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    public delegate void CancelExpectedArrivalComponentCompletedEventHandler(
        object sender, CancelExpectedArrivalComponentCompletedEventArgs e);

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class CancelExpectedArrivalComponentCompletedEventArgs : AsyncCompletedEventArgs
    {

        private object[] results;

        internal CancelExpectedArrivalComponentCompletedEventArgs(object[] results, System.Exception exception, bool cancelled,
            object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public CancelExpectedArrivalResult Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return ((CancelExpectedArrivalResult) (results[0]));
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    public delegate void GetOrderInfoCompletedEventHandler(object sender, GetOrderInfoCompletedEventArgs e);

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class GetOrderInfoCompletedEventArgs : AsyncCompletedEventArgs
    {

        private object[] results;

        internal GetOrderInfoCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public OrderInqRecord Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return ((OrderInqRecord) (results[0]));
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    public delegate void GetOffersCompletedEventHandler(object sender, GetOffersCompletedEventArgs e);

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class GetOffersCompletedEventArgs : AsyncCompletedEventArgs
    {

        private object[] results;

        internal GetOffersCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public GetOfferResult[] Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return ((GetOfferResult[]) (results[0]));
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    public delegate void AddMailerCompletedEventHandler(object sender, AddMailerCompletedEventArgs e);

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class AddMailerCompletedEventArgs : AsyncCompletedEventArgs
    {

        private object[] results;

        internal AddMailerCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public AddMailerResult Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return ((AddMailerResult) (results[0]));
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    public delegate void AddPreRegisteredUserCompletedEventHandler(object sender, AddPreRegisteredUserCompletedEventArgs e);

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class AddPreRegisteredUserCompletedEventArgs : AsyncCompletedEventArgs
    {

        private object[] results;

        internal AddPreRegisteredUserCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public AddPreRegUserResult Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return ((AddPreRegUserResult) (results[0]));
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    public delegate void GetDetailedBillingCompletedEventHandler(object sender, GetDetailedBillingCompletedEventArgs e);

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class GetDetailedBillingCompletedEventArgs : AsyncCompletedEventArgs
    {

        private object[] results;

        internal GetDetailedBillingCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public BillingDetail[] Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return ((BillingDetail[]) (results[0]));
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    public delegate void GetProductShipmentAllProdsCompletedEventHandler(object sender, GetProductShipmentAllProdsCompletedEventArgs e);

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class GetProductShipmentAllProdsCompletedEventArgs : AsyncCompletedEventArgs
    {

        private object[] results;

        internal GetProductShipmentAllProdsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState)
            :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public ProductShipment[] Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return ((ProductShipment[]) (results[0]));
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    public delegate void GetProductShipmentProdListCompletedEventHandler(object sender, GetProductShipmentProdListCompletedEventArgs e);

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class GetProductShipmentProdListCompletedEventArgs : AsyncCompletedEventArgs
    {

        private object[] results;

        internal GetProductShipmentProdListCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState)
            :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public ProductShipment[] Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return ((ProductShipment[]) (results[0]));
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    public delegate void GetShippingActivityCompletedEventHandler(object sender, GetShippingActivityCompletedEventArgs e);

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class GetShippingActivityCompletedEventArgs : AsyncCompletedEventArgs
    {

        private object[] results;

        internal GetShippingActivityCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public ShippingActivity[] Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return ((ShippingActivity[]) (results[0]));
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    public delegate void GetProductReturnsCompletedEventHandler(object sender, GetProductReturnsCompletedEventArgs e);

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class GetProductReturnsCompletedEventArgs : AsyncCompletedEventArgs
    {

        private object[] results;

        internal GetProductReturnsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public ProductReturns[] Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return ((ProductReturns[]) (results[0]));
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    public delegate void GetShippingChargeCompletedEventHandler(object sender, GetShippingChargeCompletedEventArgs e);

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class GetShippingChargeCompletedEventArgs : AsyncCompletedEventArgs
    {

        private object[] results;

        internal GetShippingChargeCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public ShippingCharges[] Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return ((ShippingCharges[]) (results[0]));
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    public delegate void AddOrderCompletedEventHandler(object sender, AddOrderCompletedEventArgs e);

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class AddOrderCompletedEventArgs : AsyncCompletedEventArgs
    {

        private object[] results;

        internal AddOrderCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public AddOrderResult Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return ((AddOrderResult) (results[0]));
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    public delegate void AddSizeColorClusterCompletedEventHandler(object sender, AddSizeColorClusterCompletedEventArgs e);

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class AddSizeColorClusterCompletedEventArgs : AsyncCompletedEventArgs
    {

        private object[] results;

        internal AddSizeColorClusterCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public AddSizeColorClusterResult Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return ((AddSizeColorClusterResult) (results[0]));
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    public delegate void AddProductListClusterCompletedEventHandler(object sender, AddProductListClusterCompletedEventArgs e);

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class AddProductListClusterCompletedEventArgs : AsyncCompletedEventArgs
    {

        private object[] results;

        internal AddProductListClusterCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public AddProductListClusterResult Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return ((AddProductListClusterResult) (results[0]));
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    public delegate void CreateProductFromJSONCompletedEventHandler(object sender, CreateProductFromJSONCompletedEventArgs e);

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class CreateProductFromJSONCompletedEventArgs : AsyncCompletedEventArgs
    {

        private object[] results;

        internal CreateProductFromJSONCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public Product Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return ((Product) (results[0]));
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    public delegate void CreateOfferFromJSONCompletedEventHandler(object sender, CreateOfferFromJSONCompletedEventArgs e);

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class CreateOfferFromJSONCompletedEventArgs : AsyncCompletedEventArgs
    {

        private object[] results;

        internal CreateOfferFromJSONCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public Offer Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return ((Offer) (results[0]));
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    public delegate void CreateExpectedArrivalFromJSONCompletedEventHandler(object sender, CreateExpectedArrivalFromJSONCompletedEventArgs e
        );

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class CreateExpectedArrivalFromJSONCompletedEventArgs : AsyncCompletedEventArgs
    {

        private object[] results;

        internal CreateExpectedArrivalFromJSONCompletedEventArgs(object[] results, System.Exception exception, bool cancelled,
            object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public ExpectedArrival Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return ((ExpectedArrival) (results[0]));
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    public delegate void CreatePurchaseOrderFromJSONCompletedEventHandler(object sender, CreatePurchaseOrderFromJSONCompletedEventArgs e);

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class CreatePurchaseOrderFromJSONCompletedEventArgs : AsyncCompletedEventArgs
    {

        private object[] results;

        internal CreatePurchaseOrderFromJSONCompletedEventArgs(object[] results, System.Exception exception, bool cancelled,
            object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public PurchaseOrder Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return ((PurchaseOrder) (results[0]));
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    public delegate void CreateUserPreRegisteredFromJSONCompletedEventHandler(
        object sender, CreateUserPreRegisteredFromJSONCompletedEventArgs e);

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class CreateUserPreRegisteredFromJSONCompletedEventArgs : AsyncCompletedEventArgs
    {

        private object[] results;

        internal CreateUserPreRegisteredFromJSONCompletedEventArgs(object[] results, System.Exception exception, bool cancelled,
            object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public UserPreRegistered Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return ((UserPreRegistered) (results[0]));
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    public delegate void CreateSizeColorClusterFromJSONCompletedEventHandler(
        object sender, CreateSizeColorClusterFromJSONCompletedEventArgs e);

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class CreateSizeColorClusterFromJSONCompletedEventArgs : AsyncCompletedEventArgs
    {

        private object[] results;

        internal CreateSizeColorClusterFromJSONCompletedEventArgs(object[] results, System.Exception exception, bool cancelled,
            object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public SizeColorCluster Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return ((SizeColorCluster) (results[0]));
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    public delegate void CreateProductListClusterFromJSONCompletedEventHandler(
        object sender, CreateProductListClusterFromJSONCompletedEventArgs e);

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class CreateProductListClusterFromJSONCompletedEventArgs : AsyncCompletedEventArgs
    {

        private object[] results;

        internal CreateProductListClusterFromJSONCompletedEventArgs(object[] results, System.Exception exception, bool cancelled,
            object userState) :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public ProductListCluster Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return ((ProductListCluster) (results[0]));
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    public delegate void GetProductAvailabilitiesCompletedEventHandler(object sender, GetProductAvailabilitiesCompletedEventArgs e);

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class GetProductAvailabilitiesCompletedEventArgs : AsyncCompletedEventArgs
    {

        private object[] results;

        internal GetProductAvailabilitiesCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState)
            :
                base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public ProductAvailabilities[] Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return ((ProductAvailabilities[]) (results[0]));
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    public delegate void GetProductCompletedEventHandler(object sender, GetProductCompletedEventArgs e);

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class GetProductCompletedEventArgs : AsyncCompletedEventArgs
    {

        private object[] results;

        internal GetProductCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public GetProductResult Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return ((GetProductResult) (results[0]));
            }
        }
    }

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    public delegate void GetExpectedArrivalsCompletedEventHandler(object sender, GetExpectedArrivalsCompletedEventArgs e);

    /// <remarks/>
    [GeneratedCode("wsdl", "4.6.1055.0")]
    [DebuggerStepThrough()]
    [DesignerCategory("code")]
    public partial class GetExpectedArrivalsCompletedEventArgs : AsyncCompletedEventArgs
    {

        private object[] results;

        internal GetExpectedArrivalsCompletedEventArgs(object[] results, System.Exception exception, bool cancelled, object userState) :
            base(exception, cancelled, userState)
        {
            this.results = results;
        }

        /// <remarks/>
        public ExpectedArrivals[] Result
        {
            get
            {
                RaiseExceptionIfNecessary();
                return ((ExpectedArrivals[]) (results[0]));
            }
        }
    }
}