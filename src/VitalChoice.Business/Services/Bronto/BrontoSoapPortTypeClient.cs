using System.ServiceModel.Channels;
using System.ServiceModel;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Xml;

namespace VitalChoice.Business.Services.Bronto
{
    [DebuggerStepThrough]

    [XmlSchemaProvider("ExportSchema")]
    [XmlRoot(IsNullable = false)]
    public partial class BrontoApiException : object, IXmlSerializable
    {

        private XmlNode[] nodesField;

        private static XmlQualifiedName typeName = new XmlQualifiedName("ApiException", "http://api.bronto.com/v4");

        public XmlNode[] Nodes
        {
            get { return nodesField; }
            set { nodesField = value; }
        }

        public void ReadXml(XmlReader reader)
        {
            nodesField = XmlSerializableServices.ReadNodes(reader);
        }

        public void WriteXml(XmlWriter writer)
        {
            XmlSerializableServices.WriteNodes(writer, Nodes);
        }

        public XmlSchema GetSchema()
        {
            return null;
        }
    }

    [ServiceContract(Namespace = "http://api.bronto.com/v4", ConfigurationName = "BrontoSoapPortType")]
    public interface BrontoSoapPortType
    {

        // CODEGEN: Generating message contract since the operation readLogins is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        readLoginsResponse readLogins(readLogins1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<readLoginsResponse> readLoginsAsync(readLogins1 request);

        // CODEGEN: Generating message contract since the operation deleteLogins is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        deleteLoginsResponse deleteLogins(deleteLogins request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<deleteLoginsResponse> deleteLoginsAsync(deleteLogins request);

        // CODEGEN: Generating message contract since the operation deleteDeliveryGroup is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        deleteDeliveryGroupResponse deleteDeliveryGroup(deleteDeliveryGroup request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<deleteDeliveryGroupResponse> deleteDeliveryGroupAsync(deleteDeliveryGroup request);

        // CODEGEN: Generating message contract since the operation addContactsToWorkflow is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        addContactsToWorkflowResponse addContactsToWorkflow(addContactsToWorkflow1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<addContactsToWorkflowResponse> addContactsToWorkflowAsync(addContactsToWorkflow1 request);

        // CODEGEN: Generating message contract since the operation readApiTokens is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        readApiTokensResponse readApiTokens(readApiTokens1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<readApiTokensResponse> readApiTokensAsync(readApiTokens1 request);

        // CODEGEN: Generating message contract since the operation updateMessageRules is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        updateMessageRulesResponse updateMessageRules(updateMessageRules request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<updateMessageRulesResponse> updateMessageRulesAsync(updateMessageRules request);

        // CODEGEN: Generating message contract since the operation deleteMessageRules is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        deleteMessageRulesResponse deleteMessageRules(deleteMessageRules request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<deleteMessageRulesResponse> deleteMessageRulesAsync(deleteMessageRules request);

        // CODEGEN: Generating message contract since the operation readLists is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        readListsResponse readLists(readLists1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<readListsResponse> readListsAsync(readLists1 request);

        // CODEGEN: Generating message contract since the operation deleteMessages is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        deleteMessagesResponse deleteMessages(deleteMessages request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<deleteMessagesResponse> deleteMessagesAsync(deleteMessages request);

        // CODEGEN: Generating message contract since the operation updateSMSDeliveries is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        updateSMSDeliveriesResponse updateSMSDeliveries(updateSMSDeliveries request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<updateSMSDeliveriesResponse> updateSMSDeliveriesAsync(updateSMSDeliveries request);

        // CODEGEN: Generating message contract since the operation readMessageFolders is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        readMessageFoldersResponse readMessageFolders(readMessageFolders1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<readMessageFoldersResponse> readMessageFoldersAsync(readMessageFolders1 request);

        // CODEGEN: Generating message contract since the operation addUpdateOrder is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        addUpdateOrderResponse addUpdateOrder(addUpdateOrder request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<addUpdateOrderResponse> addUpdateOrderAsync(addUpdateOrder request);

        // CODEGEN: Generating message contract since the operation updateDeliveryGroup is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        updateDeliveryGroupResponse updateDeliveryGroup(updateDeliveryGroup request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<updateDeliveryGroupResponse> updateDeliveryGroupAsync(updateDeliveryGroup request);

        // CODEGEN: Generating message contract since the operation readHeaderFooters is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        readHeaderFootersResponse readHeaderFooters(readHeaderFooters1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<readHeaderFootersResponse> readHeaderFootersAsync(readHeaderFooters1 request);

        // CODEGEN: Generating message contract since the operation deleteApiTokens is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        deleteApiTokensResponse deleteApiTokens(deleteApiTokens request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<deleteApiTokensResponse> deleteApiTokensAsync(deleteApiTokens request);

        // CODEGEN: Generating message contract since the operation addFields is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        addFieldsResponse addFields(addFields request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<addFieldsResponse> addFieldsAsync(addFields request);

        // CODEGEN: Generating message contract since the operation deleteHeaderFooters is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        deleteHeaderFootersResponse deleteHeaderFooters(deleteHeaderFooters request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<deleteHeaderFootersResponse> deleteHeaderFootersAsync(deleteHeaderFooters request);

        // CODEGEN: Generating message contract since the operation deleteWorkflows is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        deleteWorkflowsResponse deleteWorkflows(deleteWorkflows request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<deleteWorkflowsResponse> deleteWorkflowsAsync(deleteWorkflows request);

        // CODEGEN: Generating message contract since the operation addToList is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        addToListResponse addToList(addToList1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<addToListResponse> addToListAsync(addToList1 request);

        // CODEGEN: Generating message contract since the operation updateContentTags is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        updateContentTagsResponse updateContentTags(updateContentTags request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<updateContentTagsResponse> updateContentTagsAsync(updateContentTags request);

        // CODEGEN: Generating message contract since the operation readActivities is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        readActivitiesResponse readActivities(readActivities1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<readActivitiesResponse> readActivitiesAsync(readActivities1 request);

        // CODEGEN: Generating message contract since the operation addSMSMessages is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        addSMSMessagesResponse addSMSMessages(addSMSMessages request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<addSMSMessagesResponse> addSMSMessagesAsync(addSMSMessages request);

        // CODEGEN: Generating message contract since the operation readConversions is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        readConversionsResponse readConversions(readConversions1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<readConversionsResponse> readConversionsAsync(readConversions1 request);

        // CODEGEN: Generating message contract since the operation deleteContacts is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        deleteContactsResponse deleteContacts(deleteContacts request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<deleteContactsResponse> deleteContactsAsync(deleteContacts request);

        // CODEGEN: Generating message contract since the operation addDeliveryGroup is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        addDeliveryGroupResponse addDeliveryGroup(addDeliveryGroup request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<addDeliveryGroupResponse> addDeliveryGroupAsync(addDeliveryGroup request);

        // CODEGEN: Generating message contract since the operation updateSMSKeywords is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        updateSMSKeywordsResponse updateSMSKeywords(updateSMSKeywords request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<updateSMSKeywordsResponse> updateSMSKeywordsAsync(updateSMSKeywords request);

        // CODEGEN: Generating message contract since the operation updateMessages is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        updateMessagesResponse updateMessages(updateMessages request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<updateMessagesResponse> updateMessagesAsync(updateMessages request);

        // CODEGEN: Generating message contract since the operation readUnsubscribes is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        readUnsubscribesResponse readUnsubscribes(readUnsubscribes1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<readUnsubscribesResponse> readUnsubscribesAsync(readUnsubscribes1 request);

        // CODEGEN: Generating message contract since the operation readContacts is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        readContactsResponse readContacts(readContacts1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<readContactsResponse> readContactsAsync(readContacts1 request);

        // CODEGEN: Generating message contract since the operation readRecentOutboundActivities is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        readRecentOutboundActivitiesResponse readRecentOutboundActivities(readRecentOutboundActivities1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<readRecentOutboundActivitiesResponse> readRecentOutboundActivitiesAsync(readRecentOutboundActivities1 request);

        // CODEGEN: Generating message contract since the operation addContentTags is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        addContentTagsResponse addContentTags(addContentTags request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<addContentTagsResponse> addContentTagsAsync(addContentTags request);

        // CODEGEN: Generating message contract since the operation updateDeliveries is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        updateDeliveriesResponse updateDeliveries(updateDeliveries request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<updateDeliveriesResponse> updateDeliveriesAsync(updateDeliveries request);

        // CODEGEN: Generating message contract since the operation deleteSMSMessages is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        deleteSMSMessagesResponse deleteSMSMessages(deleteSMSMessages request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<deleteSMSMessagesResponse> deleteSMSMessagesAsync(deleteSMSMessages request);

        // CODEGEN: Generating message contract since the operation addSMSKeywords is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        addSMSKeywordsResponse addSMSKeywords(addSMSKeywords request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<addSMSKeywordsResponse> addSMSKeywordsAsync(addSMSKeywords request);

        // CODEGEN: Generating message contract since the operation readWorkflows is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        readWorkflowsResponse readWorkflows(readWorkflows1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<readWorkflowsResponse> readWorkflowsAsync(readWorkflows1 request);

        // CODEGEN: Generating message contract since the operation updateApiTokens is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        updateApiTokensResponse updateApiTokens(updateApiTokens request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<updateApiTokensResponse> updateApiTokensAsync(updateApiTokens request);

        // CODEGEN: Generating message contract since the operation readAccounts is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        readAccountsResponse readAccounts(readAccounts1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<readAccountsResponse> readAccountsAsync(readAccounts1 request);

        // CODEGEN: Generating message contract since the operation addToSMSKeyword is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        addToSMSKeywordResponse addToSMSKeyword(addToSMSKeyword1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<addToSMSKeywordResponse> addToSMSKeywordAsync(addToSMSKeyword1 request);

        // CODEGEN: Generating message contract since the operation removeFromList is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        removeFromListResponse removeFromList(removeFromList1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<removeFromListResponse> removeFromListAsync(removeFromList1 request);

        // CODEGEN: Generating message contract since the operation readDeliveryRecipients is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        readDeliveryRecipientsResponse readDeliveryRecipients(readDeliveryRecipients1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<readDeliveryRecipientsResponse> readDeliveryRecipientsAsync(readDeliveryRecipients1 request);

        // CODEGEN: Generating message contract since the operation addLists is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        addListsResponse addLists(addLists request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<addListsResponse> addListsAsync(addLists request);

        // CODEGEN: Generating message contract since the operation readSegments is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        readSegmentsResponse readSegments(readSegments1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<readSegmentsResponse> readSegmentsAsync(readSegments1 request);

        // CODEGEN: Generating message contract since the operation readSMSKeywords is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        readSMSKeywordsResponse readSMSKeywords(readSMSKeywords1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<readSMSKeywordsResponse> readSMSKeywordsAsync(readSMSKeywords1 request);

        // CODEGEN: Generating message contract since the operation readRecentInboundActivities is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        readRecentInboundActivitiesResponse readRecentInboundActivities(readRecentInboundActivities1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<readRecentInboundActivitiesResponse> readRecentInboundActivitiesAsync(readRecentInboundActivities1 request);

        // CODEGEN: Generating message contract since the operation addDeliveries is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        addDeliveriesResponse addDeliveries(addDeliveries request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<addDeliveriesResponse> addDeliveriesAsync(addDeliveries request);

        // CODEGEN: Generating message contract since the operation addContacts is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        addContactsResponse addContacts(addContacts request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<addContactsResponse> addContactsAsync(addContacts request);

        // CODEGEN: Generating message contract since the operation addContactEvent is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        addContactEventResponse addContactEvent(addContactEvent1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<addContactEventResponse> addContactEventAsync(addContactEvent1 request);

        // CODEGEN: Generating message contract since the operation deleteDeliveries is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        deleteDeliveriesResponse deleteDeliveries(deleteDeliveries request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<deleteDeliveriesResponse> deleteDeliveriesAsync(deleteDeliveries request);

        // CODEGEN: Parameter 'return' requires additional schema information that cannot be captured using the parameter mode. The specific attribute is 'XmlElementAttribute'.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        [return: MessageParameterAttribute(Name = "return")]
        loginResponse login(login request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<loginResponse> loginAsync(login request);

        // CODEGEN: Generating message contract since the operation deleteOrders is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        deleteOrdersResponse deleteOrders(deleteOrders request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<deleteOrdersResponse> deleteOrdersAsync(deleteOrders request);

        // CODEGEN: Generating message contract since the operation addOrUpdateDeliveryGroup is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        addOrUpdateDeliveryGroupResponse addOrUpdateDeliveryGroup(addOrUpdateDeliveryGroup request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<addOrUpdateDeliveryGroupResponse> addOrUpdateDeliveryGroupAsync(addOrUpdateDeliveryGroup request);

        // CODEGEN: Generating message contract since the operation updateMessageFolders is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        updateMessageFoldersResponse updateMessageFolders(updateMessageFolders request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<updateMessageFoldersResponse> updateMessageFoldersAsync(updateMessageFolders request);

        // CODEGEN: Generating message contract since the operation addOrUpdateOrders is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        addOrUpdateOrdersResponse addOrUpdateOrders(addOrUpdateOrders request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<addOrUpdateOrdersResponse> addOrUpdateOrdersAsync(addOrUpdateOrders request);

        // CODEGEN: Generating message contract since the operation addOrUpdateContacts is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        addOrUpdateContactsResponse addOrUpdateContacts(addOrUpdateContacts request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<addOrUpdateContactsResponse> addOrUpdateContactsAsync(addOrUpdateContacts request);

        // CODEGEN: Generating message contract since the operation readDeliveries is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        readDeliveriesResponse readDeliveries(readDeliveries1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<readDeliveriesResponse> readDeliveriesAsync(readDeliveries1 request);

        // CODEGEN: Generating message contract since the operation readSMSDeliveries is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        readSMSDeliveriesResponse readSMSDeliveries(readSMSDeliveries1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<readSMSDeliveriesResponse> readSMSDeliveriesAsync(readSMSDeliveries1 request);

        // CODEGEN: Generating message contract since the operation updateLists is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        updateListsResponse updateLists(updateLists request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<updateListsResponse> updateListsAsync(updateLists request);

        // CODEGEN: Generating message contract since the operation readContentTags is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        readContentTagsResponse readContentTags(readContentTags1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<readContentTagsResponse> readContentTagsAsync(readContentTags1 request);

        // CODEGEN: Generating message contract since the operation addAccounts is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        addAccountsResponse addAccounts(addAccounts request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<addAccountsResponse> addAccountsAsync(addAccounts request);

        // CODEGEN: Generating message contract since the operation deleteLists is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        deleteListsResponse deleteLists(deleteLists request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<deleteListsResponse> deleteListsAsync(deleteLists request);

        // CODEGEN: Generating message contract since the operation deleteContentTags is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        deleteContentTagsResponse deleteContentTags(deleteContentTags request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<deleteContentTagsResponse> deleteContentTagsAsync(deleteContentTags request);

        // CODEGEN: Generating message contract since the operation removeFromSMSKeyword is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        removeFromSMSKeywordResponse removeFromSMSKeyword(removeFromSMSKeyword1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<removeFromSMSKeywordResponse> removeFromSMSKeywordAsync(removeFromSMSKeyword1 request);

        // CODEGEN: Generating message contract since the operation addMessages is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        addMessagesResponse addMessages(addMessages request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<addMessagesResponse> addMessagesAsync(addMessages request);

        // CODEGEN: Generating message contract since the operation readFields is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        readFieldsResponse readFields(readFields1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<readFieldsResponse> readFieldsAsync(readFields1 request);

        // CODEGEN: Generating message contract since the operation addHeaderFooters is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        addHeaderFootersResponse addHeaderFooters(addHeaderFooters request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<addHeaderFootersResponse> addHeaderFootersAsync(addHeaderFooters request);

        // CODEGEN: Generating message contract since the operation updateFields is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        updateFieldsResponse updateFields(updateFields request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<updateFieldsResponse> updateFieldsAsync(updateFields request);

        // CODEGEN: Generating message contract since the operation deleteFromDeliveryGroup is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        deleteFromDeliveryGroupResponse deleteFromDeliveryGroup(deleteFromDeliveryGroup1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<deleteFromDeliveryGroupResponse> deleteFromDeliveryGroupAsync(deleteFromDeliveryGroup1 request);

        // CODEGEN: Generating message contract since the operation clearLists is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        clearListsResponse clearLists(clearLists request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<clearListsResponse> clearListsAsync(clearLists request);

        // CODEGEN: Generating message contract since the operation addMessageRules is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        addMessageRulesResponse addMessageRules(addMessageRules request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<addMessageRulesResponse> addMessageRulesAsync(addMessageRules request);

        // CODEGEN: Generating message contract since the operation addMessageFolders is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        addMessageFoldersResponse addMessageFolders(addMessageFolders request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<addMessageFoldersResponse> addMessageFoldersAsync(addMessageFolders request);

        // CODEGEN: Generating message contract since the operation readMessages is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        readMessagesResponse readMessages(readMessages1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<readMessagesResponse> readMessagesAsync(readMessages1 request);

        // CODEGEN: Generating message contract since the operation deleteAccounts is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        deleteAccountsResponse deleteAccounts(deleteAccounts request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<deleteAccountsResponse> deleteAccountsAsync(deleteAccounts request);

        // CODEGEN: Generating message contract since the operation updateSMSMessages is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        updateSMSMessagesResponse updateSMSMessages(updateSMSMessages request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<updateSMSMessagesResponse> updateSMSMessagesAsync(updateSMSMessages request);

        // CODEGEN: Generating message contract since the operation readMessageRules is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        readMessageRulesResponse readMessageRules(readMessageRules1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<readMessageRulesResponse> readMessageRulesAsync(readMessageRules1 request);

        // CODEGEN: Generating message contract since the operation addWorkflows is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        addWorkflowsResponse addWorkflows(addWorkflows request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<addWorkflowsResponse> addWorkflowsAsync(addWorkflows request);

        // CODEGEN: Generating message contract since the operation deleteSMSKeywords is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        deleteSMSKeywordsResponse deleteSMSKeywords(deleteSMSKeywords request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<deleteSMSKeywordsResponse> deleteSMSKeywordsAsync(deleteSMSKeywords request);

        // CODEGEN: Generating message contract since the operation updateWorkflows is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        updateWorkflowsResponse updateWorkflows(updateWorkflows request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<updateWorkflowsResponse> updateWorkflowsAsync(updateWorkflows request);

        // CODEGEN: Generating message contract since the operation addConversion is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        addConversionResponse addConversion(addConversion request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<addConversionResponse> addConversionAsync(addConversion request);

        // CODEGEN: Generating message contract since the operation updateAccounts is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        updateAccountsResponse updateAccounts(updateAccounts request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<updateAccountsResponse> updateAccountsAsync(updateAccounts request);

        // CODEGEN: Generating message contract since the operation readBounces is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        readBouncesResponse readBounces(readBounces1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<readBouncesResponse> readBouncesAsync(readBounces1 request);

        // CODEGEN: Generating message contract since the operation updateHeaderFooters is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        updateHeaderFootersResponse updateHeaderFooters(updateHeaderFooters request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<updateHeaderFootersResponse> updateHeaderFootersAsync(updateHeaderFooters request);

        // CODEGEN: Generating message contract since the operation deleteMessageFolders is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        deleteMessageFoldersResponse deleteMessageFolders(deleteMessageFolders request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<deleteMessageFoldersResponse> deleteMessageFoldersAsync(deleteMessageFolders request);

        // CODEGEN: Generating message contract since the operation addLogins is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        addLoginsResponse addLogins(addLogins request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<addLoginsResponse> addLoginsAsync(addLogins request);

        // CODEGEN: Generating message contract since the operation updateContacts is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        updateContactsResponse updateContacts(updateContacts request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<updateContactsResponse> updateContactsAsync(updateContacts request);

        // CODEGEN: Generating message contract since the operation readDeliveryGroups is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        readDeliveryGroupsResponse readDeliveryGroups(readDeliveryGroups1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<readDeliveryGroupsResponse> readDeliveryGroupsAsync(readDeliveryGroups1 request);

        // CODEGEN: Generating message contract since the operation addToDeliveryGroup is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        addToDeliveryGroupResponse addToDeliveryGroup(addToDeliveryGroup1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<addToDeliveryGroupResponse> addToDeliveryGroupAsync(addToDeliveryGroup1 request);

        // CODEGEN: Generating message contract since the operation addSMSDeliveries is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        addSMSDeliveriesResponse addSMSDeliveries(addSMSDeliveries request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<addSMSDeliveriesResponse> addSMSDeliveriesAsync(addSMSDeliveries request);

        // CODEGEN: Generating message contract since the operation deleteSMSDeliveries is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        deleteSMSDeliveriesResponse deleteSMSDeliveries(deleteSMSDeliveries request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<deleteSMSDeliveriesResponse> deleteSMSDeliveriesAsync(deleteSMSDeliveries request);

        // CODEGEN: Generating message contract since the operation deleteFields is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        deleteFieldsResponse deleteFields(deleteFields request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<deleteFieldsResponse> deleteFieldsAsync(deleteFields request);

        // CODEGEN: Generating message contract since the operation readSMSMessages is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        readSMSMessagesResponse readSMSMessages(readSMSMessages1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<readSMSMessagesResponse> readSMSMessagesAsync(readSMSMessages1 request);

        // CODEGEN: Generating message contract since the operation addApiTokens is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        addApiTokensResponse addApiTokens(addApiTokens request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<addApiTokensResponse> addApiTokensAsync(addApiTokens request);

        // CODEGEN: Generating message contract since the operation updateLogins is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        updateLoginsResponse updateLogins(updateLogins request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<updateLoginsResponse> updateLoginsAsync(updateLogins request);

        // CODEGEN: Generating message contract since the operation readWebforms is neither RPC nor document wrapped.
        [OperationContract(Action = "", ReplyAction = "*")]
        [FaultContract(typeof(BrontoApiException), Action = "", Name = "ApiException")]
        [XmlSerializerFormat]
        [ServiceKnownType(typeof(recentActivitySearchRequest))]
        readWebformsResponse readWebforms(readWebforms1 request);

        [OperationContract(Action = "", ReplyAction = "*")]
        Task<readWebformsResponse> readWebformsAsync(readWebforms1 request);
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class sessionHeader
    {

        private string sessionIdField;

        private bool disableHistoryField;

        private bool disableHistoryFieldSpecified;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string sessionId
        {
            get { return sessionIdField; }
            set { sessionIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public bool disableHistory
        {
            get { return disableHistoryField; }
            set { disableHistoryField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool disableHistorySpecified
        {
            get { return disableHistoryFieldSpecified; }
            set { disableHistoryFieldSpecified = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class webformObject
    {

        private string idField;

        private string nameField;

        private string typeField;

        private bool isDefaultField;

        private bool isDefaultFieldSpecified;

        private System.DateTime modifiedField;

        private bool modifiedFieldSpecified;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public bool isDefault
        {
            get { return isDefaultField; }
            set { isDefaultField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool isDefaultSpecified
        {
            get { return isDefaultFieldSpecified; }
            set { isDefaultFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public System.DateTime modified
        {
            get { return modifiedField; }
            set { modifiedField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool modifiedSpecified
        {
            get { return modifiedFieldSpecified; }
            set { modifiedFieldSpecified = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class webformFilter
    {

        private filterType typeField;

        private bool typeFieldSpecified;

        private string[] idField;

        private stringValue[] nameField;

        private string[] webformTypeField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public filterType type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool typeSpecified
        {
            get { return typeFieldSpecified; }
            set { typeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("id", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 1)]
        public string[] id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement("name", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 2)]
        public stringValue[] name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks/>
        [XmlElement("webformType", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 3)]
        public string[] webformType
        {
            get { return webformTypeField; }
            set { webformTypeField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public enum filterType
    {

        /// <remarks/>
        AND,

        /// <remarks/>
        OR,
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class stringValue
    {

        private filterOperator operatorField;

        private bool operatorFieldSpecified;

        private string valueField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public filterOperator @operator
        {
            get { return operatorField; }
            set { operatorField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool operatorSpecified
        {
            get { return operatorFieldSpecified; }
            set { operatorFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string value
        {
            get { return valueField; }
            set { valueField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public enum filterOperator
    {

        /// <remarks/>
        EqualTo,

        /// <remarks/>
        NotEqualTo,

        /// <remarks/>
        StartsWith,

        /// <remarks/>
        EndsWith,

        /// <remarks/>
        DoesNotStartWith,

        /// <remarks/>
        DoesNotEndWith,

        /// <remarks/>
        GreaterThan,

        /// <remarks/>
        LessThan,

        /// <remarks/>
        GreaterThanEqualTo,

        /// <remarks/>
        LessThanEqualTo,

        /// <remarks/>
        Contains,

        /// <remarks/>
        DoesNotContain,

        /// <remarks/>
        SameYear,

        /// <remarks/>
        NotSameYear,

        /// <remarks/>
        SameDay,

        /// <remarks/>
        NotSameDay,

        /// <remarks/>
        Before,

        /// <remarks/>
        After,

        /// <remarks/>
        BeforeOrSameDay,

        /// <remarks/>
        AfterOrSameDay,
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class readWebforms
    {

        private webformFilter filterField;

        private int pageNumberField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public webformFilter filter
        {
            get { return filterField; }
            set { filterField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public int pageNumber
        {
            get { return pageNumberField; }
            set { pageNumberField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class readSMSMessages
    {

        private messageFilter filterField;

        private bool includeContentField;

        private int pageNumberField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public messageFilter filter
        {
            get { return filterField; }
            set { filterField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public bool includeContent
        {
            get { return includeContentField; }
            set { includeContentField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public int pageNumber
        {
            get { return pageNumberField; }
            set { pageNumberField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class messageFilter
    {

        private filterType typeField;

        private bool typeFieldSpecified;

        private string[] idField;

        private stringValue[] nameField;

        private string[] statusField;

        private string[] messageFolderIdField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public filterType type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool typeSpecified
        {
            get { return typeFieldSpecified; }
            set { typeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("id", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 1)]
        public string[] id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement("name", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 2)]
        public stringValue[] name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks/>
        [XmlElement("status", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 3)]
        public string[] status
        {
            get { return statusField; }
            set { statusField = value; }
        }

        /// <remarks/>
        [XmlElement("messageFolderId", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 4)]
        public string[] messageFolderId
        {
            get { return messageFolderIdField; }
            set { messageFolderIdField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class addToDeliveryGroup
    {

        private deliveryGroupObject deliveryGroupField;

        private string[] deliveryIdsField;

        private string[] messageIdsField;

        private string[] messageRuleIdsField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public deliveryGroupObject deliveryGroup
        {
            get { return deliveryGroupField; }
            set { deliveryGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("deliveryIds", Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string[] deliveryIds
        {
            get { return deliveryIdsField; }
            set { deliveryIdsField = value; }
        }

        /// <remarks/>
        [XmlElement("messageIds", Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string[] messageIds
        {
            get { return messageIdsField; }
            set { messageIdsField = value; }
        }

        /// <remarks/>
        [XmlElement("messageRuleIds", Form = XmlSchemaForm.Unqualified, Order = 3)]
        public string[] messageRuleIds
        {
            get { return messageRuleIdsField; }
            set { messageRuleIdsField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class deliveryGroupObject
    {

        private string idField;

        private string nameField;

        private string visibilityField;

        private long deliveryCountField;

        private bool deliveryCountFieldSpecified;

        private System.DateTime createdDateField;

        private bool createdDateFieldSpecified;

        private string[] deliveryIdsField;

        private string[] messageRuleIdsField;

        private string[] messageIdsField;

        private long numSendsField;

        private bool numSendsFieldSpecified;

        private long numDeliveriesField;

        private bool numDeliveriesFieldSpecified;

        private long numHardBadEmailField;

        private bool numHardBadEmailFieldSpecified;

        private long numHardDestUnreachField;

        private bool numHardDestUnreachFieldSpecified;

        private long numHardMessageContentField;

        private bool numHardMessageContentFieldSpecified;

        private long numHardBouncesField;

        private bool numHardBouncesFieldSpecified;

        private long numSoftBadEmailField;

        private bool numSoftBadEmailFieldSpecified;

        private long numSoftDestUnreachField;

        private bool numSoftDestUnreachFieldSpecified;

        private long numSoftMessageContentField;

        private bool numSoftMessageContentFieldSpecified;

        private long numSoftBouncesField;

        private bool numSoftBouncesFieldSpecified;

        private long numOtherBouncesField;

        private bool numOtherBouncesFieldSpecified;

        private long numBouncesField;

        private bool numBouncesFieldSpecified;

        private long uniqOpensField;

        private bool uniqOpensFieldSpecified;

        private long numOpensField;

        private bool numOpensFieldSpecified;

        private double avgOpensField;

        private bool avgOpensFieldSpecified;

        private long uniqClicksField;

        private bool uniqClicksFieldSpecified;

        private long numClicksField;

        private bool numClicksFieldSpecified;

        private double avgClicksField;

        private bool avgClicksFieldSpecified;

        private long uniqConversionsField;

        private bool uniqConversionsFieldSpecified;

        private long numConversionsField;

        private bool numConversionsFieldSpecified;

        private double avgConversionsField;

        private bool avgConversionsFieldSpecified;

        private decimal revenueField;

        private bool revenueFieldSpecified;

        private long numSurveyResponsesField;

        private bool numSurveyResponsesFieldSpecified;

        private long numFriendForwardsField;

        private bool numFriendForwardsFieldSpecified;

        private long numContactUpdatesField;

        private bool numContactUpdatesFieldSpecified;

        private long numUnsubscribesByPrefsField;

        private bool numUnsubscribesByPrefsFieldSpecified;

        private long numUnsubscribesByComplaintField;

        private bool numUnsubscribesByComplaintFieldSpecified;

        private long numContactLossBouncesField;

        private bool numContactLossBouncesFieldSpecified;

        private long numContactLossField;

        private bool numContactLossFieldSpecified;

        private double deliveryRateField;

        private bool deliveryRateFieldSpecified;

        private double openRateField;

        private bool openRateFieldSpecified;

        private double clickRateField;

        private bool clickRateFieldSpecified;

        private double clickThroughRateField;

        private bool clickThroughRateFieldSpecified;

        private double conversionRateField;

        private bool conversionRateFieldSpecified;

        private double bounceRateField;

        private bool bounceRateFieldSpecified;

        private double complaintRateField;

        private bool complaintRateFieldSpecified;

        private double contactLossRateField;

        private bool contactLossRateFieldSpecified;

        private long numSocialSharesField;

        private bool numSocialSharesFieldSpecified;

        private long numSharesFacebookField;

        private bool numSharesFacebookFieldSpecified;

        private long numSharesTwitterField;

        private bool numSharesTwitterFieldSpecified;

        private long numSharesLinkedInField;

        private bool numSharesLinkedInFieldSpecified;

        private long numSharesDiggField;

        private bool numSharesDiggFieldSpecified;

        private long numSharesMySpaceField;

        private bool numSharesMySpaceFieldSpecified;

        private long numSocialViewsField;

        private bool numSocialViewsFieldSpecified;

        private long numViewsFacebookField;

        private bool numViewsFacebookFieldSpecified;

        private long numViewsTwitterField;

        private bool numViewsTwitterFieldSpecified;

        private long numViewsLinkedInField;

        private bool numViewsLinkedInFieldSpecified;

        private long numViewsDiggField;

        private bool numViewsDiggFieldSpecified;

        private long numViewsMySpaceField;

        private bool numViewsMySpaceFieldSpecified;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string visibility
        {
            get { return visibilityField; }
            set { visibilityField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public long deliveryCount
        {
            get { return deliveryCountField; }
            set { deliveryCountField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool deliveryCountSpecified
        {
            get { return deliveryCountFieldSpecified; }
            set { deliveryCountFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public System.DateTime createdDate
        {
            get { return createdDateField; }
            set { createdDateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool createdDateSpecified
        {
            get { return createdDateFieldSpecified; }
            set { createdDateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("deliveryIds", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 5)]
        public string[] deliveryIds
        {
            get { return deliveryIdsField; }
            set { deliveryIdsField = value; }
        }

        /// <remarks/>
        [XmlElement("messageRuleIds", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 6)]
        public string[] messageRuleIds
        {
            get { return messageRuleIdsField; }
            set { messageRuleIdsField = value; }
        }

        /// <remarks/>
        [XmlElement("messageIds", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 7)]
        public string[] messageIds
        {
            get { return messageIdsField; }
            set { messageIdsField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 8)]
        public long numSends
        {
            get { return numSendsField; }
            set { numSendsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSendsSpecified
        {
            get { return numSendsFieldSpecified; }
            set { numSendsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 9)]
        public long numDeliveries
        {
            get { return numDeliveriesField; }
            set { numDeliveriesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numDeliveriesSpecified
        {
            get { return numDeliveriesFieldSpecified; }
            set { numDeliveriesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 10)]
        public long numHardBadEmail
        {
            get { return numHardBadEmailField; }
            set { numHardBadEmailField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numHardBadEmailSpecified
        {
            get { return numHardBadEmailFieldSpecified; }
            set { numHardBadEmailFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 11)]
        public long numHardDestUnreach
        {
            get { return numHardDestUnreachField; }
            set { numHardDestUnreachField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numHardDestUnreachSpecified
        {
            get { return numHardDestUnreachFieldSpecified; }
            set { numHardDestUnreachFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 12)]
        public long numHardMessageContent
        {
            get { return numHardMessageContentField; }
            set { numHardMessageContentField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numHardMessageContentSpecified
        {
            get { return numHardMessageContentFieldSpecified; }
            set { numHardMessageContentFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 13)]
        public long numHardBounces
        {
            get { return numHardBouncesField; }
            set { numHardBouncesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numHardBouncesSpecified
        {
            get { return numHardBouncesFieldSpecified; }
            set { numHardBouncesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 14)]
        public long numSoftBadEmail
        {
            get { return numSoftBadEmailField; }
            set { numSoftBadEmailField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSoftBadEmailSpecified
        {
            get { return numSoftBadEmailFieldSpecified; }
            set { numSoftBadEmailFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 15)]
        public long numSoftDestUnreach
        {
            get { return numSoftDestUnreachField; }
            set { numSoftDestUnreachField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSoftDestUnreachSpecified
        {
            get { return numSoftDestUnreachFieldSpecified; }
            set { numSoftDestUnreachFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 16)]
        public long numSoftMessageContent
        {
            get { return numSoftMessageContentField; }
            set { numSoftMessageContentField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSoftMessageContentSpecified
        {
            get { return numSoftMessageContentFieldSpecified; }
            set { numSoftMessageContentFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 17)]
        public long numSoftBounces
        {
            get { return numSoftBouncesField; }
            set { numSoftBouncesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSoftBouncesSpecified
        {
            get { return numSoftBouncesFieldSpecified; }
            set { numSoftBouncesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 18)]
        public long numOtherBounces
        {
            get { return numOtherBouncesField; }
            set { numOtherBouncesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numOtherBouncesSpecified
        {
            get { return numOtherBouncesFieldSpecified; }
            set { numOtherBouncesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 19)]
        public long numBounces
        {
            get { return numBouncesField; }
            set { numBouncesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numBouncesSpecified
        {
            get { return numBouncesFieldSpecified; }
            set { numBouncesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 20)]
        public long uniqOpens
        {
            get { return uniqOpensField; }
            set { uniqOpensField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool uniqOpensSpecified
        {
            get { return uniqOpensFieldSpecified; }
            set { uniqOpensFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 21)]
        public long numOpens
        {
            get { return numOpensField; }
            set { numOpensField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numOpensSpecified
        {
            get { return numOpensFieldSpecified; }
            set { numOpensFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 22)]
        public double avgOpens
        {
            get { return avgOpensField; }
            set { avgOpensField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool avgOpensSpecified
        {
            get { return avgOpensFieldSpecified; }
            set { avgOpensFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 23)]
        public long uniqClicks
        {
            get { return uniqClicksField; }
            set { uniqClicksField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool uniqClicksSpecified
        {
            get { return uniqClicksFieldSpecified; }
            set { uniqClicksFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 24)]
        public long numClicks
        {
            get { return numClicksField; }
            set { numClicksField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numClicksSpecified
        {
            get { return numClicksFieldSpecified; }
            set { numClicksFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 25)]
        public double avgClicks
        {
            get { return avgClicksField; }
            set { avgClicksField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool avgClicksSpecified
        {
            get { return avgClicksFieldSpecified; }
            set { avgClicksFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 26)]
        public long uniqConversions
        {
            get { return uniqConversionsField; }
            set { uniqConversionsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool uniqConversionsSpecified
        {
            get { return uniqConversionsFieldSpecified; }
            set { uniqConversionsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 27)]
        public long numConversions
        {
            get { return numConversionsField; }
            set { numConversionsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numConversionsSpecified
        {
            get { return numConversionsFieldSpecified; }
            set { numConversionsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 28)]
        public double avgConversions
        {
            get { return avgConversionsField; }
            set { avgConversionsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool avgConversionsSpecified
        {
            get { return avgConversionsFieldSpecified; }
            set { avgConversionsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 29)]
        public decimal revenue
        {
            get { return revenueField; }
            set { revenueField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool revenueSpecified
        {
            get { return revenueFieldSpecified; }
            set { revenueFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 30)]
        public long numSurveyResponses
        {
            get { return numSurveyResponsesField; }
            set { numSurveyResponsesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSurveyResponsesSpecified
        {
            get { return numSurveyResponsesFieldSpecified; }
            set { numSurveyResponsesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 31)]
        public long numFriendForwards
        {
            get { return numFriendForwardsField; }
            set { numFriendForwardsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numFriendForwardsSpecified
        {
            get { return numFriendForwardsFieldSpecified; }
            set { numFriendForwardsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 32)]
        public long numContactUpdates
        {
            get { return numContactUpdatesField; }
            set { numContactUpdatesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numContactUpdatesSpecified
        {
            get { return numContactUpdatesFieldSpecified; }
            set { numContactUpdatesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 33)]
        public long numUnsubscribesByPrefs
        {
            get { return numUnsubscribesByPrefsField; }
            set { numUnsubscribesByPrefsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numUnsubscribesByPrefsSpecified
        {
            get { return numUnsubscribesByPrefsFieldSpecified; }
            set { numUnsubscribesByPrefsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 34)]
        public long numUnsubscribesByComplaint
        {
            get { return numUnsubscribesByComplaintField; }
            set { numUnsubscribesByComplaintField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numUnsubscribesByComplaintSpecified
        {
            get { return numUnsubscribesByComplaintFieldSpecified; }
            set { numUnsubscribesByComplaintFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 35)]
        public long numContactLossBounces
        {
            get { return numContactLossBouncesField; }
            set { numContactLossBouncesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numContactLossBouncesSpecified
        {
            get { return numContactLossBouncesFieldSpecified; }
            set { numContactLossBouncesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 36)]
        public long numContactLoss
        {
            get { return numContactLossField; }
            set { numContactLossField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numContactLossSpecified
        {
            get { return numContactLossFieldSpecified; }
            set { numContactLossFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 37)]
        public double deliveryRate
        {
            get { return deliveryRateField; }
            set { deliveryRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool deliveryRateSpecified
        {
            get { return deliveryRateFieldSpecified; }
            set { deliveryRateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 38)]
        public double openRate
        {
            get { return openRateField; }
            set { openRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool openRateSpecified
        {
            get { return openRateFieldSpecified; }
            set { openRateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 39)]
        public double clickRate
        {
            get { return clickRateField; }
            set { clickRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool clickRateSpecified
        {
            get { return clickRateFieldSpecified; }
            set { clickRateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 40)]
        public double clickThroughRate
        {
            get { return clickThroughRateField; }
            set { clickThroughRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool clickThroughRateSpecified
        {
            get { return clickThroughRateFieldSpecified; }
            set { clickThroughRateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 41)]
        public double conversionRate
        {
            get { return conversionRateField; }
            set { conversionRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool conversionRateSpecified
        {
            get { return conversionRateFieldSpecified; }
            set { conversionRateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 42)]
        public double bounceRate
        {
            get { return bounceRateField; }
            set { bounceRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool bounceRateSpecified
        {
            get { return bounceRateFieldSpecified; }
            set { bounceRateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 43)]
        public double complaintRate
        {
            get { return complaintRateField; }
            set { complaintRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool complaintRateSpecified
        {
            get { return complaintRateFieldSpecified; }
            set { complaintRateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 44)]
        public double contactLossRate
        {
            get { return contactLossRateField; }
            set { contactLossRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool contactLossRateSpecified
        {
            get { return contactLossRateFieldSpecified; }
            set { contactLossRateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 45)]
        public long numSocialShares
        {
            get { return numSocialSharesField; }
            set { numSocialSharesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSocialSharesSpecified
        {
            get { return numSocialSharesFieldSpecified; }
            set { numSocialSharesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 46)]
        public long numSharesFacebook
        {
            get { return numSharesFacebookField; }
            set { numSharesFacebookField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSharesFacebookSpecified
        {
            get { return numSharesFacebookFieldSpecified; }
            set { numSharesFacebookFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 47)]
        public long numSharesTwitter
        {
            get { return numSharesTwitterField; }
            set { numSharesTwitterField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSharesTwitterSpecified
        {
            get { return numSharesTwitterFieldSpecified; }
            set { numSharesTwitterFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 48)]
        public long numSharesLinkedIn
        {
            get { return numSharesLinkedInField; }
            set { numSharesLinkedInField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSharesLinkedInSpecified
        {
            get { return numSharesLinkedInFieldSpecified; }
            set { numSharesLinkedInFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 49)]
        public long numSharesDigg
        {
            get { return numSharesDiggField; }
            set { numSharesDiggField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSharesDiggSpecified
        {
            get { return numSharesDiggFieldSpecified; }
            set { numSharesDiggFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 50)]
        public long numSharesMySpace
        {
            get { return numSharesMySpaceField; }
            set { numSharesMySpaceField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSharesMySpaceSpecified
        {
            get { return numSharesMySpaceFieldSpecified; }
            set { numSharesMySpaceFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 51)]
        public long numSocialViews
        {
            get { return numSocialViewsField; }
            set { numSocialViewsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSocialViewsSpecified
        {
            get { return numSocialViewsFieldSpecified; }
            set { numSocialViewsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 52)]
        public long numViewsFacebook
        {
            get { return numViewsFacebookField; }
            set { numViewsFacebookField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numViewsFacebookSpecified
        {
            get { return numViewsFacebookFieldSpecified; }
            set { numViewsFacebookFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 53)]
        public long numViewsTwitter
        {
            get { return numViewsTwitterField; }
            set { numViewsTwitterField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numViewsTwitterSpecified
        {
            get { return numViewsTwitterFieldSpecified; }
            set { numViewsTwitterFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 54)]
        public long numViewsLinkedIn
        {
            get { return numViewsLinkedInField; }
            set { numViewsLinkedInField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numViewsLinkedInSpecified
        {
            get { return numViewsLinkedInFieldSpecified; }
            set { numViewsLinkedInFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 55)]
        public long numViewsDigg
        {
            get { return numViewsDiggField; }
            set { numViewsDiggField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numViewsDiggSpecified
        {
            get { return numViewsDiggFieldSpecified; }
            set { numViewsDiggFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 56)]
        public long numViewsMySpace
        {
            get { return numViewsMySpaceField; }
            set { numViewsMySpaceField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numViewsMySpaceSpecified
        {
            get { return numViewsMySpaceFieldSpecified; }
            set { numViewsMySpaceFieldSpecified = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class deliveryGroupFilter
    {

        private string[] deliveryGroupIdField;

        private memberType listByTypeField;

        private bool listByTypeFieldSpecified;

        private string[] automatorIdField;

        private string[] messageGroupIdField;

        private string[] deliveryIdField;

        private stringValue[] nameField;

        /// <remarks/>
        [XmlElement("deliveryGroupId", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 0)]
        public string[] deliveryGroupId
        {
            get { return deliveryGroupIdField; }
            set { deliveryGroupIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public memberType listByType
        {
            get { return listByTypeField; }
            set { listByTypeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool listByTypeSpecified
        {
            get { return listByTypeFieldSpecified; }
            set { listByTypeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("automatorId", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 2)]
        public string[] automatorId
        {
            get { return automatorIdField; }
            set { automatorIdField = value; }
        }

        /// <remarks/>
        [XmlElement("messageGroupId", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 3)]
        public string[] messageGroupId
        {
            get { return messageGroupIdField; }
            set { messageGroupIdField = value; }
        }

        /// <remarks/>
        [XmlElement("deliveryId", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 4)]
        public string[] deliveryId
        {
            get { return deliveryIdField; }
            set { deliveryIdField = value; }
        }

        /// <remarks/>
        [XmlElement("name", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 5)]
        public stringValue[] name
        {
            get { return nameField; }
            set { nameField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public enum memberType
    {

        /// <remarks/>
        DELIVERIES,

        /// <remarks/>
        AUTOMATORS,

        /// <remarks/>
        MESSAGEGROUPS,

        /// <remarks/>
        DELIVERYGROUPS,
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class readDeliveryGroups
    {

        private deliveryGroupFilter filterField;

        private int pageNumberField;

        private bool includeStatsField;

        private bool includeStatsFieldSpecified;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public deliveryGroupFilter filter
        {
            get { return filterField; }
            set { filterField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public int pageNumber
        {
            get { return pageNumberField; }
            set { pageNumberField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public bool includeStats
        {
            get { return includeStatsField; }
            set { includeStatsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool includeStatsSpecified
        {
            get { return includeStatsFieldSpecified; }
            set { includeStatsFieldSpecified = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class bounceObject
    {

        private string contactIdField;

        private string deliveryIdField;

        private string typeField;

        private string descriptionField;

        private System.DateTime createdField;

        private bool createdFieldSpecified;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string contactId
        {
            get { return contactIdField; }
            set { contactIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string deliveryId
        {
            get { return deliveryIdField; }
            set { deliveryIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public string description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public System.DateTime created
        {
            get { return createdField; }
            set { createdField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool createdSpecified
        {
            get { return createdFieldSpecified; }
            set { createdFieldSpecified = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class bounceFilter
    {

        private string contactIdField;

        private System.DateTime startField;

        private bool startFieldSpecified;

        private System.DateTime endField;

        private bool endFieldSpecified;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string contactId
        {
            get { return contactIdField; }
            set { contactIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public System.DateTime start
        {
            get { return startField; }
            set { startField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool startSpecified
        {
            get { return startFieldSpecified; }
            set { startFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public System.DateTime end
        {
            get { return endField; }
            set { endField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool endSpecified
        {
            get { return endFieldSpecified; }
            set { endFieldSpecified = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class readBounces
    {

        private bounceFilter filterField;

        private int pageNumberField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public bounceFilter filter
        {
            get { return filterField; }
            set { filterField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public int pageNumber
        {
            get { return pageNumberField; }
            set { pageNumberField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class messageRuleFilter
    {

        private filterType typeField;

        private bool typeFieldSpecified;

        private string[] idField;

        private stringValue[] nameField;

        private string[] ruleTypeField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public filterType type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool typeSpecified
        {
            get { return typeFieldSpecified; }
            set { typeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("id", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 1)]
        public string[] id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement("name", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 2)]
        public stringValue[] name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks/>
        [XmlElement("ruleType", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 3)]
        public string[] ruleType
        {
            get { return ruleTypeField; }
            set { ruleTypeField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class readMessageRules
    {

        private messageRuleFilter filterField;

        private int pageNumberField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public messageRuleFilter filter
        {
            get { return filterField; }
            set { filterField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public int pageNumber
        {
            get { return pageNumberField; }
            set { pageNumberField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class readMessages
    {

        private messageFilter filterField;

        private bool includeContentField;

        private int pageNumberField;

        private int pageSizeField;

        private bool pageSizeFieldSpecified;

        private bool includeStatsField;

        private bool includeStatsFieldSpecified;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public messageFilter filter
        {
            get { return filterField; }
            set { filterField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public bool includeContent
        {
            get { return includeContentField; }
            set { includeContentField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public int pageNumber
        {
            get { return pageNumberField; }
            set { pageNumberField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public int pageSize
        {
            get { return pageSizeField; }
            set { pageSizeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool pageSizeSpecified
        {
            get { return pageSizeFieldSpecified; }
            set { pageSizeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public bool includeStats
        {
            get { return includeStatsField; }
            set { includeStatsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool includeStatsSpecified
        {
            get { return includeStatsFieldSpecified; }
            set { includeStatsFieldSpecified = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class deleteFromDeliveryGroup
    {

        private deliveryGroupObject deliveryGroupField;

        private string[] deliveryIdsField;

        private string[] messageIdsField;

        private string[] messageRuleIdsField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public deliveryGroupObject deliveryGroup
        {
            get { return deliveryGroupField; }
            set { deliveryGroupField = value; }
        }

        /// <remarks/>
        [XmlElement("deliveryIds", Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string[] deliveryIds
        {
            get { return deliveryIdsField; }
            set { deliveryIdsField = value; }
        }

        /// <remarks/>
        [XmlElement("messageIds", Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string[] messageIds
        {
            get { return messageIdsField; }
            set { messageIdsField = value; }
        }

        /// <remarks/>
        [XmlElement("messageRuleIds", Form = XmlSchemaForm.Unqualified, Order = 3)]
        public string[] messageRuleIds
        {
            get { return messageRuleIdsField; }
            set { messageRuleIdsField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class fieldsFilter
    {

        private filterType typeField;

        private bool typeFieldSpecified;

        private string[] idField;

        private stringValue[] nameField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public filterType type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool typeSpecified
        {
            get { return typeFieldSpecified; }
            set { typeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("id", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 1)]
        public string[] id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement("name", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 2)]
        public stringValue[] name
        {
            get { return nameField; }
            set { nameField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class readFields
    {

        private fieldsFilter filterField;

        private int pageNumberField;

        private int pageSizeField;

        private bool pageSizeFieldSpecified;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public fieldsFilter filter
        {
            get { return filterField; }
            set { filterField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public int pageNumber
        {
            get { return pageNumberField; }
            set { pageNumberField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public int pageSize
        {
            get { return pageSizeField; }
            set { pageSizeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool pageSizeSpecified
        {
            get { return pageSizeFieldSpecified; }
            set { pageSizeFieldSpecified = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class removeFromSMSKeyword
    {

        private smsKeywordObject keywordField;

        private contactObject[] contactsField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public smsKeywordObject keyword
        {
            get { return keywordField; }
            set { keywordField = value; }
        }

        /// <remarks/>
        [XmlElement("contacts", Form = XmlSchemaForm.Unqualified, Order = 1)]
        public contactObject[] contacts
        {
            get { return contactsField; }
            set { contactsField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class smsKeywordObject
    {

        private string idField;

        private string nameField;

        private string descriptionField;

        private long subscriberCountField;

        private bool subscriberCountFieldSpecified;

        private long frequencyCapField;

        private bool frequencyCapFieldSpecified;

        private System.DateTime dateCreatedField;

        private bool dateCreatedFieldSpecified;

        private System.DateTime scheduledDeleteDateField;

        private bool scheduledDeleteDateFieldSpecified;

        private string confirmationMessageField;

        private string messageContentField;

        private string keywordTypeField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public long subscriberCount
        {
            get { return subscriberCountField; }
            set { subscriberCountField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool subscriberCountSpecified
        {
            get { return subscriberCountFieldSpecified; }
            set { subscriberCountFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public long frequencyCap
        {
            get { return frequencyCapField; }
            set { frequencyCapField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool frequencyCapSpecified
        {
            get { return frequencyCapFieldSpecified; }
            set { frequencyCapFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 5)]
        public System.DateTime dateCreated
        {
            get { return dateCreatedField; }
            set { dateCreatedField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool dateCreatedSpecified
        {
            get { return dateCreatedFieldSpecified; }
            set { dateCreatedFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 6)]
        public System.DateTime scheduledDeleteDate
        {
            get { return scheduledDeleteDateField; }
            set { scheduledDeleteDateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool scheduledDeleteDateSpecified
        {
            get { return scheduledDeleteDateFieldSpecified; }
            set { scheduledDeleteDateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 7)]
        public string confirmationMessage
        {
            get { return confirmationMessageField; }
            set { confirmationMessageField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 8)]
        public string messageContent
        {
            get { return messageContentField; }
            set { messageContentField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 9)]
        public string keywordType
        {
            get { return keywordTypeField; }
            set { keywordTypeField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class contactObject
    {

        private string idField;

        private string emailField;

        private string mobileNumberField;

        private string statusField;

        private string msgPrefField;

        private string sourceField;

        private string customSourceField;

        private System.DateTime createdField;

        private bool createdFieldSpecified;

        private System.DateTime modifiedField;

        private bool modifiedFieldSpecified;

        private bool deletedField;

        private bool deletedFieldSpecified;

        private string[] listIdsField;

        private string[] segmentIdsField;

        private contactField[] fieldsField;

        private string[] sMSKeywordIDsField;

        private long numSendsField;

        private bool numSendsFieldSpecified;

        private long numBouncesField;

        private bool numBouncesFieldSpecified;

        private long numOpensField;

        private bool numOpensFieldSpecified;

        private long numClicksField;

        private bool numClicksFieldSpecified;

        private long numConversionsField;

        private bool numConversionsFieldSpecified;

        private float conversionAmountField;

        private bool conversionAmountFieldSpecified;

        private readOnlyContactData readOnlyContactDataField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string email
        {
            get { return emailField; }
            set { emailField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string mobileNumber
        {
            get { return mobileNumberField; }
            set { mobileNumberField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public string status
        {
            get { return statusField; }
            set { statusField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public string msgPref
        {
            get { return msgPrefField; }
            set { msgPrefField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 5)]
        public string source
        {
            get { return sourceField; }
            set { sourceField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 6)]
        public string customSource
        {
            get { return customSourceField; }
            set { customSourceField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 7)]
        public System.DateTime created
        {
            get { return createdField; }
            set { createdField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool createdSpecified
        {
            get { return createdFieldSpecified; }
            set { createdFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 8)]
        public System.DateTime modified
        {
            get { return modifiedField; }
            set { modifiedField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool modifiedSpecified
        {
            get { return modifiedFieldSpecified; }
            set { modifiedFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 9)]
        public bool deleted
        {
            get { return deletedField; }
            set { deletedField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool deletedSpecified
        {
            get { return deletedFieldSpecified; }
            set { deletedFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("listIds", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 10)]
        public string[] listIds
        {
            get { return listIdsField; }
            set { listIdsField = value; }
        }

        /// <remarks/>
        [XmlElement("segmentIds", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 11)]
        public string[] segmentIds
        {
            get { return segmentIdsField; }
            set { segmentIdsField = value; }
        }

        /// <remarks/>
        [XmlElement("fields", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 12)]
        public contactField[] fields
        {
            get { return fieldsField; }
            set { fieldsField = value; }
        }

        /// <remarks/>
        [XmlElement("SMSKeywordIDs", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 13)]
        public string[] SMSKeywordIDs
        {
            get { return sMSKeywordIDsField; }
            set { sMSKeywordIDsField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 14)]
        public long numSends
        {
            get { return numSendsField; }
            set { numSendsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSendsSpecified
        {
            get { return numSendsFieldSpecified; }
            set { numSendsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 15)]
        public long numBounces
        {
            get { return numBouncesField; }
            set { numBouncesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numBouncesSpecified
        {
            get { return numBouncesFieldSpecified; }
            set { numBouncesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 16)]
        public long numOpens
        {
            get { return numOpensField; }
            set { numOpensField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numOpensSpecified
        {
            get { return numOpensFieldSpecified; }
            set { numOpensFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 17)]
        public long numClicks
        {
            get { return numClicksField; }
            set { numClicksField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numClicksSpecified
        {
            get { return numClicksFieldSpecified; }
            set { numClicksFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 18)]
        public long numConversions
        {
            get { return numConversionsField; }
            set { numConversionsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numConversionsSpecified
        {
            get { return numConversionsFieldSpecified; }
            set { numConversionsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 19)]
        public float conversionAmount
        {
            get { return conversionAmountField; }
            set { conversionAmountField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool conversionAmountSpecified
        {
            get { return conversionAmountFieldSpecified; }
            set { conversionAmountFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 20)]
        public readOnlyContactData readOnlyContactData
        {
            get { return readOnlyContactDataField; }
            set { readOnlyContactDataField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class contactField
    {

        private string fieldIdField;

        private string contentField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string fieldId
        {
            get { return fieldIdField; }
            set { fieldIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string content
        {
            get { return contentField; }
            set { contentField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class readOnlyContactData
    {

        private string geoIPCityField;

        private string geoIPStateRegionField;

        private string geoIPZipField;

        private string geoIPCountryField;

        private string geoIPCountryCodeField;

        private string primaryBrowserField;

        private string mobileBrowserField;

        private string primaryEmailClientField;

        private string mobileEmailClientField;

        private string operatingSystemField;

        private System.DateTime firstOrderDateField;

        private bool firstOrderDateFieldSpecified;

        private System.DateTime lastOrderDateField;

        private bool lastOrderDateFieldSpecified;

        private decimal lastOrderTotalField;

        private bool lastOrderTotalFieldSpecified;

        private long totalOrdersField;

        private bool totalOrdersFieldSpecified;

        private decimal totalRevenueField;

        private bool totalRevenueFieldSpecified;

        private decimal averageOrderValueField;

        private bool averageOrderValueFieldSpecified;

        private System.DateTime lastDeliveryDateField;

        private bool lastDeliveryDateFieldSpecified;

        private System.DateTime lastOpenDateField;

        private bool lastOpenDateFieldSpecified;

        private System.DateTime lastClickDateField;

        private bool lastClickDateFieldSpecified;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string geoIPCity
        {
            get { return geoIPCityField; }
            set { geoIPCityField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string geoIPStateRegion
        {
            get { return geoIPStateRegionField; }
            set { geoIPStateRegionField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string geoIPZip
        {
            get { return geoIPZipField; }
            set { geoIPZipField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public string geoIPCountry
        {
            get { return geoIPCountryField; }
            set { geoIPCountryField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public string geoIPCountryCode
        {
            get { return geoIPCountryCodeField; }
            set { geoIPCountryCodeField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 5)]
        public string primaryBrowser
        {
            get { return primaryBrowserField; }
            set { primaryBrowserField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 6)]
        public string mobileBrowser
        {
            get { return mobileBrowserField; }
            set { mobileBrowserField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 7)]
        public string primaryEmailClient
        {
            get { return primaryEmailClientField; }
            set { primaryEmailClientField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 8)]
        public string mobileEmailClient
        {
            get { return mobileEmailClientField; }
            set { mobileEmailClientField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 9)]
        public string operatingSystem
        {
            get { return operatingSystemField; }
            set { operatingSystemField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 10)]
        public System.DateTime firstOrderDate
        {
            get { return firstOrderDateField; }
            set { firstOrderDateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool firstOrderDateSpecified
        {
            get { return firstOrderDateFieldSpecified; }
            set { firstOrderDateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 11)]
        public System.DateTime lastOrderDate
        {
            get { return lastOrderDateField; }
            set { lastOrderDateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool lastOrderDateSpecified
        {
            get { return lastOrderDateFieldSpecified; }
            set { lastOrderDateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 12)]
        public decimal lastOrderTotal
        {
            get { return lastOrderTotalField; }
            set { lastOrderTotalField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool lastOrderTotalSpecified
        {
            get { return lastOrderTotalFieldSpecified; }
            set { lastOrderTotalFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 13)]
        public long totalOrders
        {
            get { return totalOrdersField; }
            set { totalOrdersField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool totalOrdersSpecified
        {
            get { return totalOrdersFieldSpecified; }
            set { totalOrdersFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 14)]
        public decimal totalRevenue
        {
            get { return totalRevenueField; }
            set { totalRevenueField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool totalRevenueSpecified
        {
            get { return totalRevenueFieldSpecified; }
            set { totalRevenueFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 15)]
        public decimal averageOrderValue
        {
            get { return averageOrderValueField; }
            set { averageOrderValueField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool averageOrderValueSpecified
        {
            get { return averageOrderValueFieldSpecified; }
            set { averageOrderValueFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 16)]
        public System.DateTime lastDeliveryDate
        {
            get { return lastDeliveryDateField; }
            set { lastDeliveryDateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool lastDeliveryDateSpecified
        {
            get { return lastDeliveryDateFieldSpecified; }
            set { lastDeliveryDateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 17)]
        public System.DateTime lastOpenDate
        {
            get { return lastOpenDateField; }
            set { lastOpenDateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool lastOpenDateSpecified
        {
            get { return lastOpenDateFieldSpecified; }
            set { lastOpenDateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 18)]
        public System.DateTime lastClickDate
        {
            get { return lastClickDateField; }
            set { lastClickDateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool lastClickDateSpecified
        {
            get { return lastClickDateFieldSpecified; }
            set { lastClickDateFieldSpecified = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class contentTagFilter
    {

        private filterType typeField;

        private bool typeFieldSpecified;

        private string[] idField;

        private stringValue[] nameField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public filterType type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool typeSpecified
        {
            get { return typeFieldSpecified; }
            set { typeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("id", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 1)]
        public string[] id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement("name", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 2)]
        public stringValue[] name
        {
            get { return nameField; }
            set { nameField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class readContentTags
    {

        private contentTagFilter filterField;

        private bool includeContentField;

        private int pageNumberField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public contentTagFilter filter
        {
            get { return filterField; }
            set { filterField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public bool includeContent
        {
            get { return includeContentField; }
            set { includeContentField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public int pageNumber
        {
            get { return pageNumberField; }
            set { pageNumberField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class smsDeliveryFilter
    {

        private filterType typeField;

        private bool typeFieldSpecified;

        private string[] idField;

        private string[] messageIdField;

        private dateValue[] startField;

        private string[] statusField;

        private string[] deliveryTypeField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public filterType type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool typeSpecified
        {
            get { return typeFieldSpecified; }
            set { typeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("id", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 1)]
        public string[] id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement("messageId", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 2)]
        public string[] messageId
        {
            get { return messageIdField; }
            set { messageIdField = value; }
        }

        /// <remarks/>
        [XmlElement("start", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 3)]
        public dateValue[] start
        {
            get { return startField; }
            set { startField = value; }
        }

        /// <remarks/>
        [XmlElement("status", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 4)]
        public string[] status
        {
            get { return statusField; }
            set { statusField = value; }
        }

        /// <remarks/>
        [XmlElement("deliveryType", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 5)]
        public string[] deliveryType
        {
            get { return deliveryTypeField; }
            set { deliveryTypeField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class dateValue
    {

        private filterOperator operatorField;

        private bool operatorFieldSpecified;

        private System.DateTime valueField;

        private bool valueFieldSpecified;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public filterOperator @operator
        {
            get { return operatorField; }
            set { operatorField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool operatorSpecified
        {
            get { return operatorFieldSpecified; }
            set { operatorFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public System.DateTime value
        {
            get { return valueField; }
            set { valueField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool valueSpecified
        {
            get { return valueFieldSpecified; }
            set { valueFieldSpecified = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class readSMSDeliveries
    {

        private smsDeliveryFilter filterField;

        private bool includeContentField;

        private bool includeRecipientsField;

        private int pageNumberField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public smsDeliveryFilter filter
        {
            get { return filterField; }
            set { filterField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public bool includeContent
        {
            get { return includeContentField; }
            set { includeContentField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public bool includeRecipients
        {
            get { return includeRecipientsField; }
            set { includeRecipientsField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public int pageNumber
        {
            get { return pageNumberField; }
            set { pageNumberField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class deliveryFilter
    {

        private filterType typeField;

        private bool typeFieldSpecified;

        private string[] idField;

        private string[] messageIdField;

        private dateValue[] startField;

        private string[] statusField;

        private string[] deliveryTypeField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public filterType type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool typeSpecified
        {
            get { return typeFieldSpecified; }
            set { typeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("id", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 1)]
        public string[] id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement("messageId", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 2)]
        public string[] messageId
        {
            get { return messageIdField; }
            set { messageIdField = value; }
        }

        /// <remarks/>
        [XmlElement("start", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 3)]
        public dateValue[] start
        {
            get { return startField; }
            set { startField = value; }
        }

        /// <remarks/>
        [XmlElement("status", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 4)]
        public string[] status
        {
            get { return statusField; }
            set { statusField = value; }
        }

        /// <remarks/>
        [XmlElement("deliveryType", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 5)]
        public string[] deliveryType
        {
            get { return deliveryTypeField; }
            set { deliveryTypeField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class readDeliveries
    {

        private deliveryFilter filterField;

        private bool includeRecipientsField;

        private bool includeContentField;

        private int pageNumberField;

        private bool includeOrderIdsField;

        private bool includeOrderIdsFieldSpecified;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public deliveryFilter filter
        {
            get { return filterField; }
            set { filterField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public bool includeRecipients
        {
            get { return includeRecipientsField; }
            set { includeRecipientsField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public bool includeContent
        {
            get { return includeContentField; }
            set { includeContentField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public int pageNumber
        {
            get { return pageNumberField; }
            set { pageNumberField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public bool includeOrderIds
        {
            get { return includeOrderIdsField; }
            set { includeOrderIdsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool includeOrderIdsSpecified
        {
            get { return includeOrderIdsFieldSpecified; }
            set { includeOrderIdsFieldSpecified = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class addContactEvent
    {

        private string keywordField;

        private contactObject[] contactsField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string keyword
        {
            get { return keywordField; }
            set { keywordField = value; }
        }

        /// <remarks/>
        [XmlElement("contacts", Form = XmlSchemaForm.Unqualified, Order = 1)]
        public contactObject[] contacts
        {
            get { return contactsField; }
            set { contactsField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class readRecentInboundActivities
    {

        private recentInboundActivitySearchRequest filterField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public recentInboundActivitySearchRequest filter
        {
            get { return filterField; }
            set { filterField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class recentInboundActivitySearchRequest : recentActivitySearchRequest
    {

        private string[] typesField;

        /// <remarks/>
        [XmlElement("types", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 0)]
        public string[] types
        {
            get { return typesField; }
            set { typesField = value; }
        }
    }

    /// <remarks/>
    [XmlInclude(typeof(recentInboundActivitySearchRequest))]
    [XmlInclude(typeof(recentOutboundActivitySearchRequest))]

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public abstract partial class recentActivitySearchRequest
    {

        private System.DateTime startField;

        private bool startFieldSpecified;

        private System.DateTime endField;

        private bool endFieldSpecified;

        private string contactIdField;

        private string deliveryIdField;

        private int sizeField;

        private readDirection readDirectionField;

        private bool readDirectionFieldSpecified;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public System.DateTime start
        {
            get { return startField; }
            set { startField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool startSpecified
        {
            get { return startFieldSpecified; }
            set { startFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public System.DateTime end
        {
            get { return endField; }
            set { endField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool endSpecified
        {
            get { return endFieldSpecified; }
            set { endFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string contactId
        {
            get { return contactIdField; }
            set { contactIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public string deliveryId
        {
            get { return deliveryIdField; }
            set { deliveryIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public int size
        {
            get { return sizeField; }
            set { sizeField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 5)]
        public readDirection readDirection
        {
            get { return readDirectionField; }
            set { readDirectionField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool readDirectionSpecified
        {
            get { return readDirectionFieldSpecified; }
            set { readDirectionFieldSpecified = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public enum readDirection
    {

        /// <remarks/>
        NEXT,

        /// <remarks/>
        FIRST,
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class recentOutboundActivitySearchRequest : recentActivitySearchRequest
    {

        private string[] typesField;

        /// <remarks/>
        [XmlElement("types", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 0)]
        public string[] types
        {
            get { return typesField; }
            set { typesField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class smsKeywordFilter
    {

        private filterType typeField;

        private bool typeFieldSpecified;

        private string[] idField;

        private stringValue[] nameField;

        private string keywordTypeField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public filterType type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool typeSpecified
        {
            get { return typeFieldSpecified; }
            set { typeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("id", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 1)]
        public string[] id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement("name", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 2)]
        public stringValue[] name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public string keywordType
        {
            get { return keywordTypeField; }
            set { keywordTypeField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class readSMSKeywords
    {

        private smsKeywordFilter filterField;

        private bool includeDeletedField;

        private int pageNumberField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public smsKeywordFilter filter
        {
            get { return filterField; }
            set { filterField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public bool includeDeleted
        {
            get { return includeDeletedField; }
            set { includeDeletedField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public int pageNumber
        {
            get { return pageNumberField; }
            set { pageNumberField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class segmentCriteriaObject
    {

        private string operatorField;

        private string conditionField;

        private string valueField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string @operator
        {
            get { return operatorField; }
            set { operatorField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string condition
        {
            get { return conditionField; }
            set { conditionField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string value
        {
            get { return valueField; }
            set { valueField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class segmentRuleObject
    {

        private bool canMatchAnyField;

        private bool canMatchAnyFieldSpecified;

        private segmentCriteriaObject[] criteriaField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public bool canMatchAny
        {
            get { return canMatchAnyField; }
            set { canMatchAnyField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool canMatchAnySpecified
        {
            get { return canMatchAnyFieldSpecified; }
            set { canMatchAnyFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("criteria", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 1)]
        public segmentCriteriaObject[] criteria
        {
            get { return criteriaField; }
            set { criteriaField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class segmentObject
    {

        private string idField;

        private string nameField;

        private bool matchAnyRuleField;

        private bool matchAnyRuleFieldSpecified;

        private segmentRuleObject[] rulesField;

        private System.DateTime lastUpdatedField;

        private bool lastUpdatedFieldSpecified;

        private long activeCountField;

        private bool activeCountFieldSpecified;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public bool matchAnyRule
        {
            get { return matchAnyRuleField; }
            set { matchAnyRuleField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool matchAnyRuleSpecified
        {
            get { return matchAnyRuleFieldSpecified; }
            set { matchAnyRuleFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("rules", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 3)]
        public segmentRuleObject[] rules
        {
            get { return rulesField; }
            set { rulesField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public System.DateTime lastUpdated
        {
            get { return lastUpdatedField; }
            set { lastUpdatedField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool lastUpdatedSpecified
        {
            get { return lastUpdatedFieldSpecified; }
            set { lastUpdatedFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 5)]
        public long activeCount
        {
            get { return activeCountField; }
            set { activeCountField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool activeCountSpecified
        {
            get { return activeCountFieldSpecified; }
            set { activeCountFieldSpecified = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class segmentFilter
    {

        private filterType typeField;

        private bool typeFieldSpecified;

        private string[] idField;

        private stringValue[] nameField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public filterType type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool typeSpecified
        {
            get { return typeFieldSpecified; }
            set { typeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("id", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 1)]
        public string[] id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement("name", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 2)]
        public stringValue[] name
        {
            get { return nameField; }
            set { nameField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class readSegments
    {

        private segmentFilter filterField;

        private int pageNumberField;

        private int pageSizeField;

        private bool pageSizeFieldSpecified;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public segmentFilter filter
        {
            get { return filterField; }
            set { filterField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public int pageNumber
        {
            get { return pageNumberField; }
            set { pageNumberField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public int pageSize
        {
            get { return pageSizeField; }
            set { pageSizeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool pageSizeSpecified
        {
            get { return pageSizeFieldSpecified; }
            set { pageSizeFieldSpecified = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class deliveryRecipientStatObject
    {

        private string deliveryIdField;

        private string listIdField;

        private string segmentIdField;

        private string contactIdField;

        private long numSendsField;

        private bool numSendsFieldSpecified;

        private long numDeliveriesField;

        private bool numDeliveriesFieldSpecified;

        private long numHardBadEmailField;

        private bool numHardBadEmailFieldSpecified;

        private long numHardDestUnreachField;

        private bool numHardDestUnreachFieldSpecified;

        private long numHardMessageContentField;

        private bool numHardMessageContentFieldSpecified;

        private long numHardBouncesField;

        private bool numHardBouncesFieldSpecified;

        private long numSoftBadEmailField;

        private bool numSoftBadEmailFieldSpecified;

        private long numSoftDestUnreachField;

        private bool numSoftDestUnreachFieldSpecified;

        private long numSoftMessageContentField;

        private bool numSoftMessageContentFieldSpecified;

        private long numSoftBouncesField;

        private bool numSoftBouncesFieldSpecified;

        private long numOtherBouncesField;

        private bool numOtherBouncesFieldSpecified;

        private long numBouncesField;

        private bool numBouncesFieldSpecified;

        private long uniqOpensField;

        private bool uniqOpensFieldSpecified;

        private long numOpensField;

        private bool numOpensFieldSpecified;

        private double avgOpensField;

        private bool avgOpensFieldSpecified;

        private long uniqClicksField;

        private bool uniqClicksFieldSpecified;

        private long numClicksField;

        private bool numClicksFieldSpecified;

        private double avgClicksField;

        private bool avgClicksFieldSpecified;

        private long uniqConversionsField;

        private bool uniqConversionsFieldSpecified;

        private long numConversionsField;

        private bool numConversionsFieldSpecified;

        private double avgConversionsField;

        private bool avgConversionsFieldSpecified;

        private double revenueField;

        private bool revenueFieldSpecified;

        private long numSurveyResponsesField;

        private bool numSurveyResponsesFieldSpecified;

        private long numFriendForwardsField;

        private bool numFriendForwardsFieldSpecified;

        private long numContactUpdatesField;

        private bool numContactUpdatesFieldSpecified;

        private long numUnsubscribesByPrefsField;

        private bool numUnsubscribesByPrefsFieldSpecified;

        private long numUnsubscribesByComplaintField;

        private bool numUnsubscribesByComplaintFieldSpecified;

        private long numContactLossField;

        private bool numContactLossFieldSpecified;

        private long numContactLossBouncesField;

        private bool numContactLossBouncesFieldSpecified;

        private double deliveryRateField;

        private bool deliveryRateFieldSpecified;

        private double openRateField;

        private bool openRateFieldSpecified;

        private double clickRateField;

        private bool clickRateFieldSpecified;

        private double clickThroughRateField;

        private bool clickThroughRateFieldSpecified;

        private double conversionRateField;

        private bool conversionRateFieldSpecified;

        private double bounceRateField;

        private bool bounceRateFieldSpecified;

        private double complaintRateField;

        private bool complaintRateFieldSpecified;

        private double contactLossRateField;

        private bool contactLossRateFieldSpecified;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string deliveryId
        {
            get { return deliveryIdField; }
            set { deliveryIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string listId
        {
            get { return listIdField; }
            set { listIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string segmentId
        {
            get { return segmentIdField; }
            set { segmentIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public string contactId
        {
            get { return contactIdField; }
            set { contactIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public long numSends
        {
            get { return numSendsField; }
            set { numSendsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSendsSpecified
        {
            get { return numSendsFieldSpecified; }
            set { numSendsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 5)]
        public long numDeliveries
        {
            get { return numDeliveriesField; }
            set { numDeliveriesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numDeliveriesSpecified
        {
            get { return numDeliveriesFieldSpecified; }
            set { numDeliveriesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 6)]
        public long numHardBadEmail
        {
            get { return numHardBadEmailField; }
            set { numHardBadEmailField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numHardBadEmailSpecified
        {
            get { return numHardBadEmailFieldSpecified; }
            set { numHardBadEmailFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 7)]
        public long numHardDestUnreach
        {
            get { return numHardDestUnreachField; }
            set { numHardDestUnreachField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numHardDestUnreachSpecified
        {
            get { return numHardDestUnreachFieldSpecified; }
            set { numHardDestUnreachFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 8)]
        public long numHardMessageContent
        {
            get { return numHardMessageContentField; }
            set { numHardMessageContentField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numHardMessageContentSpecified
        {
            get { return numHardMessageContentFieldSpecified; }
            set { numHardMessageContentFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 9)]
        public long numHardBounces
        {
            get { return numHardBouncesField; }
            set { numHardBouncesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numHardBouncesSpecified
        {
            get { return numHardBouncesFieldSpecified; }
            set { numHardBouncesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 10)]
        public long numSoftBadEmail
        {
            get { return numSoftBadEmailField; }
            set { numSoftBadEmailField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSoftBadEmailSpecified
        {
            get { return numSoftBadEmailFieldSpecified; }
            set { numSoftBadEmailFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 11)]
        public long numSoftDestUnreach
        {
            get { return numSoftDestUnreachField; }
            set { numSoftDestUnreachField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSoftDestUnreachSpecified
        {
            get { return numSoftDestUnreachFieldSpecified; }
            set { numSoftDestUnreachFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 12)]
        public long numSoftMessageContent
        {
            get { return numSoftMessageContentField; }
            set { numSoftMessageContentField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSoftMessageContentSpecified
        {
            get { return numSoftMessageContentFieldSpecified; }
            set { numSoftMessageContentFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 13)]
        public long numSoftBounces
        {
            get { return numSoftBouncesField; }
            set { numSoftBouncesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSoftBouncesSpecified
        {
            get { return numSoftBouncesFieldSpecified; }
            set { numSoftBouncesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 14)]
        public long numOtherBounces
        {
            get { return numOtherBouncesField; }
            set { numOtherBouncesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numOtherBouncesSpecified
        {
            get { return numOtherBouncesFieldSpecified; }
            set { numOtherBouncesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 15)]
        public long numBounces
        {
            get { return numBouncesField; }
            set { numBouncesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numBouncesSpecified
        {
            get { return numBouncesFieldSpecified; }
            set { numBouncesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 16)]
        public long uniqOpens
        {
            get { return uniqOpensField; }
            set { uniqOpensField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool uniqOpensSpecified
        {
            get { return uniqOpensFieldSpecified; }
            set { uniqOpensFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 17)]
        public long numOpens
        {
            get { return numOpensField; }
            set { numOpensField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numOpensSpecified
        {
            get { return numOpensFieldSpecified; }
            set { numOpensFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 18)]
        public double avgOpens
        {
            get { return avgOpensField; }
            set { avgOpensField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool avgOpensSpecified
        {
            get { return avgOpensFieldSpecified; }
            set { avgOpensFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 19)]
        public long uniqClicks
        {
            get { return uniqClicksField; }
            set { uniqClicksField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool uniqClicksSpecified
        {
            get { return uniqClicksFieldSpecified; }
            set { uniqClicksFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 20)]
        public long numClicks
        {
            get { return numClicksField; }
            set { numClicksField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numClicksSpecified
        {
            get { return numClicksFieldSpecified; }
            set { numClicksFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 21)]
        public double avgClicks
        {
            get { return avgClicksField; }
            set { avgClicksField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool avgClicksSpecified
        {
            get { return avgClicksFieldSpecified; }
            set { avgClicksFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 22)]
        public long uniqConversions
        {
            get { return uniqConversionsField; }
            set { uniqConversionsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool uniqConversionsSpecified
        {
            get { return uniqConversionsFieldSpecified; }
            set { uniqConversionsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 23)]
        public long numConversions
        {
            get { return numConversionsField; }
            set { numConversionsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numConversionsSpecified
        {
            get { return numConversionsFieldSpecified; }
            set { numConversionsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 24)]
        public double avgConversions
        {
            get { return avgConversionsField; }
            set { avgConversionsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool avgConversionsSpecified
        {
            get { return avgConversionsFieldSpecified; }
            set { avgConversionsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 25)]
        public double revenue
        {
            get { return revenueField; }
            set { revenueField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool revenueSpecified
        {
            get { return revenueFieldSpecified; }
            set { revenueFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 26)]
        public long numSurveyResponses
        {
            get { return numSurveyResponsesField; }
            set { numSurveyResponsesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSurveyResponsesSpecified
        {
            get { return numSurveyResponsesFieldSpecified; }
            set { numSurveyResponsesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 27)]
        public long numFriendForwards
        {
            get { return numFriendForwardsField; }
            set { numFriendForwardsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numFriendForwardsSpecified
        {
            get { return numFriendForwardsFieldSpecified; }
            set { numFriendForwardsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 28)]
        public long numContactUpdates
        {
            get { return numContactUpdatesField; }
            set { numContactUpdatesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numContactUpdatesSpecified
        {
            get { return numContactUpdatesFieldSpecified; }
            set { numContactUpdatesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 29)]
        public long numUnsubscribesByPrefs
        {
            get { return numUnsubscribesByPrefsField; }
            set { numUnsubscribesByPrefsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numUnsubscribesByPrefsSpecified
        {
            get { return numUnsubscribesByPrefsFieldSpecified; }
            set { numUnsubscribesByPrefsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 30)]
        public long numUnsubscribesByComplaint
        {
            get { return numUnsubscribesByComplaintField; }
            set { numUnsubscribesByComplaintField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numUnsubscribesByComplaintSpecified
        {
            get { return numUnsubscribesByComplaintFieldSpecified; }
            set { numUnsubscribesByComplaintFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 31)]
        public long numContactLoss
        {
            get { return numContactLossField; }
            set { numContactLossField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numContactLossSpecified
        {
            get { return numContactLossFieldSpecified; }
            set { numContactLossFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 32)]
        public long numContactLossBounces
        {
            get { return numContactLossBouncesField; }
            set { numContactLossBouncesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numContactLossBouncesSpecified
        {
            get { return numContactLossBouncesFieldSpecified; }
            set { numContactLossBouncesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 33)]
        public double deliveryRate
        {
            get { return deliveryRateField; }
            set { deliveryRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool deliveryRateSpecified
        {
            get { return deliveryRateFieldSpecified; }
            set { deliveryRateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 34)]
        public double openRate
        {
            get { return openRateField; }
            set { openRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool openRateSpecified
        {
            get { return openRateFieldSpecified; }
            set { openRateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 35)]
        public double clickRate
        {
            get { return clickRateField; }
            set { clickRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool clickRateSpecified
        {
            get { return clickRateFieldSpecified; }
            set { clickRateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 36)]
        public double clickThroughRate
        {
            get { return clickThroughRateField; }
            set { clickThroughRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool clickThroughRateSpecified
        {
            get { return clickThroughRateFieldSpecified; }
            set { clickThroughRateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 37)]
        public double conversionRate
        {
            get { return conversionRateField; }
            set { conversionRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool conversionRateSpecified
        {
            get { return conversionRateFieldSpecified; }
            set { conversionRateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 38)]
        public double bounceRate
        {
            get { return bounceRateField; }
            set { bounceRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool bounceRateSpecified
        {
            get { return bounceRateFieldSpecified; }
            set { bounceRateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 39)]
        public double complaintRate
        {
            get { return complaintRateField; }
            set { complaintRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool complaintRateSpecified
        {
            get { return complaintRateFieldSpecified; }
            set { complaintRateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 40)]
        public double contactLossRate
        {
            get { return contactLossRateField; }
            set { contactLossRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool contactLossRateSpecified
        {
            get { return contactLossRateFieldSpecified; }
            set { contactLossRateFieldSpecified = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class deliveryRecipientFilter
    {

        private filterType typeField;

        private bool typeFieldSpecified;

        private string deliveryIdField;

        private string[] listIdsField;

        private string[] segmentIdsField;

        private string[] contactIdsField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public filterType type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool typeSpecified
        {
            get { return typeFieldSpecified; }
            set { typeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string deliveryId
        {
            get { return deliveryIdField; }
            set { deliveryIdField = value; }
        }

        /// <remarks/>
        [XmlElement("listIds", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 2)]
        public string[] listIds
        {
            get { return listIdsField; }
            set { listIdsField = value; }
        }

        /// <remarks/>
        [XmlElement("segmentIds", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 3)]
        public string[] segmentIds
        {
            get { return segmentIdsField; }
            set { segmentIdsField = value; }
        }

        /// <remarks/>
        [XmlElement("contactIds", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 4)]
        public string[] contactIds
        {
            get { return contactIdsField; }
            set { contactIdsField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class readDeliveryRecipients
    {

        private deliveryRecipientFilter filterField;

        private int pageNumberField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public deliveryRecipientFilter filter
        {
            get { return filterField; }
            set { filterField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public int pageNumber
        {
            get { return pageNumberField; }
            set { pageNumberField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class removeFromList
    {

        private mailListObject listField;

        private contactObject[] contactsField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public mailListObject list
        {
            get { return listField; }
            set { listField = value; }
        }

        /// <remarks/>
        [XmlElement("contacts", Form = XmlSchemaForm.Unqualified, Order = 1)]
        public contactObject[] contacts
        {
            get { return contactsField; }
            set { contactsField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class mailListObject
    {

        private string idField;

        private string nameField;

        private string labelField;

        private long activeCountField;

        private bool activeCountFieldSpecified;

        private string statusField;

        private string visibilityField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string label
        {
            get { return labelField; }
            set { labelField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public long activeCount
        {
            get { return activeCountField; }
            set { activeCountField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool activeCountSpecified
        {
            get { return activeCountFieldSpecified; }
            set { activeCountFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public string status
        {
            get { return statusField; }
            set { statusField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 5)]
        public string visibility
        {
            get { return visibilityField; }
            set { visibilityField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class addToSMSKeyword
    {

        private smsKeywordObject keywordField;

        private contactObject[] contactsField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public smsKeywordObject keyword
        {
            get { return keywordField; }
            set { keywordField = value; }
        }

        /// <remarks/>
        [XmlElement("contacts", Form = XmlSchemaForm.Unqualified, Order = 1)]
        public contactObject[] contacts
        {
            get { return contactsField; }
            set { contactsField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class accountAllocations
    {

        private bool canExceedAllocationField;

        private bool canExceedAllocationFieldSpecified;

        private bool canExceedSmsAllocationField;

        private bool canExceedSmsAllocationFieldSpecified;

        private string emailsField;

        private long contactsField;

        private bool contactsFieldSpecified;

        private long hostingField;

        private bool hostingFieldSpecified;

        private long loginsField;

        private bool loginsFieldSpecified;

        private bool apiField;

        private bool apiFieldSpecified;

        private long fieldsField;

        private bool fieldsFieldSpecified;

        private System.DateTime startDateField;

        private bool startDateFieldSpecified;

        private long periodFrequencyField;

        private bool periodFrequencyFieldSpecified;

        private string bundleField;

        private bool defaultTemplatesField;

        private bool defaultTemplatesFieldSpecified;

        private bool brandingField;

        private bool brandingFieldSpecified;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public bool canExceedAllocation
        {
            get { return canExceedAllocationField; }
            set { canExceedAllocationField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool canExceedAllocationSpecified
        {
            get { return canExceedAllocationFieldSpecified; }
            set { canExceedAllocationFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public bool canExceedSmsAllocation
        {
            get { return canExceedSmsAllocationField; }
            set { canExceedSmsAllocationField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool canExceedSmsAllocationSpecified
        {
            get { return canExceedSmsAllocationFieldSpecified; }
            set { canExceedSmsAllocationFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "integer", Order = 2)]
        public string emails
        {
            get { return emailsField; }
            set { emailsField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public long contacts
        {
            get { return contactsField; }
            set { contactsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool contactsSpecified
        {
            get { return contactsFieldSpecified; }
            set { contactsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public long hosting
        {
            get { return hostingField; }
            set { hostingField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool hostingSpecified
        {
            get { return hostingFieldSpecified; }
            set { hostingFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 5)]
        public long logins
        {
            get { return loginsField; }
            set { loginsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool loginsSpecified
        {
            get { return loginsFieldSpecified; }
            set { loginsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 6)]
        public bool api
        {
            get { return apiField; }
            set { apiField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool apiSpecified
        {
            get { return apiFieldSpecified; }
            set { apiFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 7)]
        public long fields
        {
            get { return fieldsField; }
            set { fieldsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool fieldsSpecified
        {
            get { return fieldsFieldSpecified; }
            set { fieldsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 8)]
        public System.DateTime startDate
        {
            get { return startDateField; }
            set { startDateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool startDateSpecified
        {
            get { return startDateFieldSpecified; }
            set { startDateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 9)]
        public long periodFrequency
        {
            get { return periodFrequencyField; }
            set { periodFrequencyField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool periodFrequencySpecified
        {
            get { return periodFrequencyFieldSpecified; }
            set { periodFrequencyFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 10)]
        public string bundle
        {
            get { return bundleField; }
            set { bundleField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 11)]
        public bool defaultTemplates
        {
            get { return defaultTemplatesField; }
            set { defaultTemplatesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool defaultTemplatesSpecified
        {
            get { return defaultTemplatesFieldSpecified; }
            set { defaultTemplatesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 12)]
        public bool branding
        {
            get { return brandingField; }
            set { brandingField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool brandingSpecified
        {
            get { return brandingFieldSpecified; }
            set { brandingFieldSpecified = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class repliesSettings
    {

        private bool deletedAutomatedRepliesField;

        private bool deletedAutomatedRepliesFieldSpecified;

        private bool deleteSpamField;

        private bool deleteSpamFieldSpecified;

        private bool deleteUnsubscribeRepliesField;

        private bool deleteUnsubscribeRepliesFieldSpecified;

        private bool handleUnsubscribesField;

        private bool handleUnsubscribesFieldSpecified;

        private string unsubscribeKeywordsField;

        private string replyForwardEmailField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public bool deletedAutomatedReplies
        {
            get { return deletedAutomatedRepliesField; }
            set { deletedAutomatedRepliesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool deletedAutomatedRepliesSpecified
        {
            get { return deletedAutomatedRepliesFieldSpecified; }
            set { deletedAutomatedRepliesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public bool deleteSpam
        {
            get { return deleteSpamField; }
            set { deleteSpamField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool deleteSpamSpecified
        {
            get { return deleteSpamFieldSpecified; }
            set { deleteSpamFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public bool deleteUnsubscribeReplies
        {
            get { return deleteUnsubscribeRepliesField; }
            set { deleteUnsubscribeRepliesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool deleteUnsubscribeRepliesSpecified
        {
            get { return deleteUnsubscribeRepliesFieldSpecified; }
            set { deleteUnsubscribeRepliesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public bool handleUnsubscribes
        {
            get { return handleUnsubscribesField; }
            set { handleUnsubscribesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool handleUnsubscribesSpecified
        {
            get { return handleUnsubscribesFieldSpecified; }
            set { handleUnsubscribesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public string unsubscribeKeywords
        {
            get { return unsubscribeKeywordsField; }
            set { unsubscribeKeywordsField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 5)]
        public string replyForwardEmail
        {
            get { return replyForwardEmailField; }
            set { replyForwardEmailField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class brandingSettings
    {

        private string brandingImageField;

        private string brandingImageLinkField;

        private string brandingImageUrlField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string brandingImage
        {
            get { return brandingImageField; }
            set { brandingImageField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string brandingImageLink
        {
            get { return brandingImageLinkField; }
            set { brandingImageLinkField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string brandingImageUrl
        {
            get { return brandingImageUrlField; }
            set { brandingImageUrlField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class formatSettings
    {

        private string timeZoneField;

        private string dateFormatField;

        private string localeField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string timeZone
        {
            get { return timeZoneField; }
            set { timeZoneField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string dateFormat
        {
            get { return dateFormatField; }
            set { dateFormatField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string locale
        {
            get { return localeField; }
            set { localeField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class generalSettings
    {

        private string emergencyEmailField;

        private long dailyFrequencyCapField;

        private bool dailyFrequencyCapFieldSpecified;

        private long weeklyFrequencyCapField;

        private bool weeklyFrequencyCapFieldSpecified;

        private long monthlyFrequencyCapField;

        private bool monthlyFrequencyCapFieldSpecified;

        private bool textDeliveryField;

        private bool textDeliveryFieldSpecified;

        private bool textPreferenceField;

        private bool textPreferenceFieldSpecified;

        private bool useSSLField;

        private bool useSSLFieldSpecified;

        private bool sendUsageAlertsField;

        private bool sendUsageAlertsFieldSpecified;

        private string usageAlertEmailField;

        private long currentContactsField;

        private bool currentContactsFieldSpecified;

        private long maxContactsField;

        private bool maxContactsFieldSpecified;

        private string currentMonthlyEmailsField;

        private long currentHostingSizeField;

        private bool currentHostingSizeFieldSpecified;

        private long maxHostingSizeField;

        private bool maxHostingSizeFieldSpecified;

        private bool agencyTemplateuploadPermField;

        private bool agencyTemplateuploadPermFieldSpecified;

        private bool defaultTemplatesField;

        private bool defaultTemplatesFieldSpecified;

        private bool enableInboxPreviewsField;

        private bool enableInboxPreviewsFieldSpecified;

        private bool allowCustomizedBrandingField;

        private bool allowCustomizedBrandingFieldSpecified;

        private long bounceLimitField;

        private bool bounceLimitFieldSpecified;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string emergencyEmail
        {
            get { return emergencyEmailField; }
            set { emergencyEmailField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public long dailyFrequencyCap
        {
            get { return dailyFrequencyCapField; }
            set { dailyFrequencyCapField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool dailyFrequencyCapSpecified
        {
            get { return dailyFrequencyCapFieldSpecified; }
            set { dailyFrequencyCapFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public long weeklyFrequencyCap
        {
            get { return weeklyFrequencyCapField; }
            set { weeklyFrequencyCapField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool weeklyFrequencyCapSpecified
        {
            get { return weeklyFrequencyCapFieldSpecified; }
            set { weeklyFrequencyCapFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public long monthlyFrequencyCap
        {
            get { return monthlyFrequencyCapField; }
            set { monthlyFrequencyCapField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool monthlyFrequencyCapSpecified
        {
            get { return monthlyFrequencyCapFieldSpecified; }
            set { monthlyFrequencyCapFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public bool textDelivery
        {
            get { return textDeliveryField; }
            set { textDeliveryField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool textDeliverySpecified
        {
            get { return textDeliveryFieldSpecified; }
            set { textDeliveryFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 5)]
        public bool textPreference
        {
            get { return textPreferenceField; }
            set { textPreferenceField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool textPreferenceSpecified
        {
            get { return textPreferenceFieldSpecified; }
            set { textPreferenceFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 6)]
        public bool useSSL
        {
            get { return useSSLField; }
            set { useSSLField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool useSSLSpecified
        {
            get { return useSSLFieldSpecified; }
            set { useSSLFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 7)]
        public bool sendUsageAlerts
        {
            get { return sendUsageAlertsField; }
            set { sendUsageAlertsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool sendUsageAlertsSpecified
        {
            get { return sendUsageAlertsFieldSpecified; }
            set { sendUsageAlertsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 8)]
        public string usageAlertEmail
        {
            get { return usageAlertEmailField; }
            set { usageAlertEmailField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 9)]
        public long currentContacts
        {
            get { return currentContactsField; }
            set { currentContactsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool currentContactsSpecified
        {
            get { return currentContactsFieldSpecified; }
            set { currentContactsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 10)]
        public long maxContacts
        {
            get { return maxContactsField; }
            set { maxContactsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool maxContactsSpecified
        {
            get { return maxContactsFieldSpecified; }
            set { maxContactsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, DataType = "integer", Order = 11)]
        public string currentMonthlyEmails
        {
            get { return currentMonthlyEmailsField; }
            set { currentMonthlyEmailsField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 12)]
        public long currentHostingSize
        {
            get { return currentHostingSizeField; }
            set { currentHostingSizeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool currentHostingSizeSpecified
        {
            get { return currentHostingSizeFieldSpecified; }
            set { currentHostingSizeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 13)]
        public long maxHostingSize
        {
            get { return maxHostingSizeField; }
            set { maxHostingSizeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool maxHostingSizeSpecified
        {
            get { return maxHostingSizeFieldSpecified; }
            set { maxHostingSizeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 14)]
        public bool agencyTemplateuploadPerm
        {
            get { return agencyTemplateuploadPermField; }
            set { agencyTemplateuploadPermField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool agencyTemplateuploadPermSpecified
        {
            get { return agencyTemplateuploadPermFieldSpecified; }
            set { agencyTemplateuploadPermFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 15)]
        public bool defaultTemplates
        {
            get { return defaultTemplatesField; }
            set { defaultTemplatesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool defaultTemplatesSpecified
        {
            get { return defaultTemplatesFieldSpecified; }
            set { defaultTemplatesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 16)]
        public bool enableInboxPreviews
        {
            get { return enableInboxPreviewsField; }
            set { enableInboxPreviewsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool enableInboxPreviewsSpecified
        {
            get { return enableInboxPreviewsFieldSpecified; }
            set { enableInboxPreviewsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 17)]
        public bool allowCustomizedBranding
        {
            get { return allowCustomizedBrandingField; }
            set { allowCustomizedBrandingField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool allowCustomizedBrandingSpecified
        {
            get { return allowCustomizedBrandingFieldSpecified; }
            set { allowCustomizedBrandingFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 18)]
        public long bounceLimit
        {
            get { return bounceLimitField; }
            set { bounceLimitField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool bounceLimitSpecified
        {
            get { return bounceLimitFieldSpecified; }
            set { bounceLimitFieldSpecified = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class accountObject
    {

        private string idField;

        private string nameField;

        private string statusField;

        private generalSettings generalSettingsField;

        private contactInformation contactInformationField;

        private formatSettings formatSettingsField;

        private brandingSettings brandingSettingsField;

        private repliesSettings repliesSettingsField;

        private accountAllocations allocationsField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string status
        {
            get { return statusField; }
            set { statusField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public generalSettings generalSettings
        {
            get { return generalSettingsField; }
            set { generalSettingsField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public contactInformation contactInformation
        {
            get { return contactInformationField; }
            set { contactInformationField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 5)]
        public formatSettings formatSettings
        {
            get { return formatSettingsField; }
            set { formatSettingsField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 6)]
        public brandingSettings brandingSettings
        {
            get { return brandingSettingsField; }
            set { brandingSettingsField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 7)]
        public repliesSettings repliesSettings
        {
            get { return repliesSettingsField; }
            set { repliesSettingsField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 8)]
        public accountAllocations allocations
        {
            get { return allocationsField; }
            set { allocationsField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class contactInformation
    {

        private string organizationField;

        private string firstNameField;

        private string lastNameField;

        private string emailField;

        private string phoneField;

        private string addressField;

        private string address2Field;

        private string cityField;

        private string stateField;

        private string zipField;

        private string countryField;

        private string notesField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string organization
        {
            get { return organizationField; }
            set { organizationField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string firstName
        {
            get { return firstNameField; }
            set { firstNameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string lastName
        {
            get { return lastNameField; }
            set { lastNameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public string email
        {
            get { return emailField; }
            set { emailField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public string phone
        {
            get { return phoneField; }
            set { phoneField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 5)]
        public string address
        {
            get { return addressField; }
            set { addressField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 6)]
        public string address2
        {
            get { return address2Field; }
            set { address2Field = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 7)]
        public string city
        {
            get { return cityField; }
            set { cityField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 8)]
        public string state
        {
            get { return stateField; }
            set { stateField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 9)]
        public string zip
        {
            get { return zipField; }
            set { zipField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 10)]
        public string country
        {
            get { return countryField; }
            set { countryField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 11)]
        public string notes
        {
            get { return notesField; }
            set { notesField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class accountFilter
    {

        private filterType typeField;

        private bool typeFieldSpecified;

        private string[] idField;

        private stringValue[] nameField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public filterType type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool typeSpecified
        {
            get { return typeFieldSpecified; }
            set { typeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("id", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 1)]
        public string[] id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement("name", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 2)]
        public stringValue[] name
        {
            get { return nameField; }
            set { nameField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class readAccounts
    {

        private accountFilter filterField;

        private bool includeInfoField;

        private int pageNumberField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public accountFilter filter
        {
            get { return filterField; }
            set { filterField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public bool includeInfo
        {
            get { return includeInfoField; }
            set { includeInfoField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public int pageNumber
        {
            get { return pageNumberField; }
            set { pageNumberField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class workflowFilter
    {

        private filterType typeField;

        private bool typeFieldSpecified;

        private string[] idField;

        private stringValue[] nameField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public filterType type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool typeSpecified
        {
            get { return typeFieldSpecified; }
            set { typeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("id", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 1)]
        public string[] id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement("name", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 2)]
        public stringValue[] name
        {
            get { return nameField; }
            set { nameField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class readWorkflows
    {

        private workflowFilter filterField;

        private int pageNumberField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public workflowFilter filter
        {
            get { return filterField; }
            set { filterField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public int pageNumber
        {
            get { return pageNumberField; }
            set { pageNumberField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class remailObject
    {

        private int daysField;

        private bool daysFieldSpecified;

        private string timeField;

        private string subjectField;

        private string messageIdField;

        private string activityField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public int days
        {
            get { return daysField; }
            set { daysField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool daysSpecified
        {
            get { return daysFieldSpecified; }
            set { daysFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string time
        {
            get { return timeField; }
            set { timeField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string subject
        {
            get { return subjectField; }
            set { subjectField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public string messageId
        {
            get { return messageIdField; }
            set { messageIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public string activity
        {
            get { return activityField; }
            set { activityField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class deliveryProductObject
    {

        private string placeholderField;

        private string productIdField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string placeholder
        {
            get { return placeholderField; }
            set { placeholderField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string productId
        {
            get { return productIdField; }
            set { productIdField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class messageFieldObject
    {

        private string nameField;

        private string typeField;

        private string contentField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string content
        {
            get { return contentField; }
            set { contentField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class deliveryObject
    {

        private string idField;

        private System.DateTime startField;

        private bool startFieldSpecified;

        private string messageIdField;

        private string statusField;

        private string typeField;

        private string fromEmailField;

        private string fromNameField;

        private string replyEmailField;

        private bool authenticationField;

        private bool authenticationFieldSpecified;

        private bool replyTrackingField;

        private bool replyTrackingFieldSpecified;

        private string messageRuleIdField;

        private bool optinField;

        private bool optinFieldSpecified;

        private long throttleField;

        private bool throttleFieldSpecified;

        private bool fatigueOverrideField;

        private bool fatigueOverrideFieldSpecified;

        private messageContentObject[] contentField;

        private deliveryRecipientObject[] recipientsField;

        private messageFieldObject[] fieldsField;

        private deliveryProductObject[] productsField;

        private remailObject remailField;

        private long numSendsField;

        private bool numSendsFieldSpecified;

        private long numDeliveriesField;

        private bool numDeliveriesFieldSpecified;

        private long numHardBadEmailField;

        private bool numHardBadEmailFieldSpecified;

        private long numHardDestUnreachField;

        private bool numHardDestUnreachFieldSpecified;

        private long numHardMessageContentField;

        private bool numHardMessageContentFieldSpecified;

        private long numHardBouncesField;

        private bool numHardBouncesFieldSpecified;

        private long numSoftBadEmailField;

        private bool numSoftBadEmailFieldSpecified;

        private long numSoftDestUnreachField;

        private bool numSoftDestUnreachFieldSpecified;

        private long numSoftMessageContentField;

        private bool numSoftMessageContentFieldSpecified;

        private long numSoftBouncesField;

        private bool numSoftBouncesFieldSpecified;

        private long numOtherBouncesField;

        private bool numOtherBouncesFieldSpecified;

        private long numBouncesField;

        private bool numBouncesFieldSpecified;

        private long uniqOpensField;

        private bool uniqOpensFieldSpecified;

        private long numOpensField;

        private bool numOpensFieldSpecified;

        private double avgOpensField;

        private bool avgOpensFieldSpecified;

        private long uniqClicksField;

        private bool uniqClicksFieldSpecified;

        private long numClicksField;

        private bool numClicksFieldSpecified;

        private double avgClicksField;

        private bool avgClicksFieldSpecified;

        private long uniqConversionsField;

        private bool uniqConversionsFieldSpecified;

        private long numConversionsField;

        private bool numConversionsFieldSpecified;

        private double avgConversionsField;

        private bool avgConversionsFieldSpecified;

        private decimal revenueField;

        private bool revenueFieldSpecified;

        private long numSurveyResponsesField;

        private bool numSurveyResponsesFieldSpecified;

        private long numFriendForwardsField;

        private bool numFriendForwardsFieldSpecified;

        private long numContactUpdatesField;

        private bool numContactUpdatesFieldSpecified;

        private long numUnsubscribesByPrefsField;

        private bool numUnsubscribesByPrefsFieldSpecified;

        private long numUnsubscribesByComplaintField;

        private bool numUnsubscribesByComplaintFieldSpecified;

        private long numContactLossField;

        private bool numContactLossFieldSpecified;

        private long numContactLossBouncesField;

        private bool numContactLossBouncesFieldSpecified;

        private double deliveryRateField;

        private bool deliveryRateFieldSpecified;

        private double openRateField;

        private bool openRateFieldSpecified;

        private double clickRateField;

        private bool clickRateFieldSpecified;

        private double clickThroughRateField;

        private bool clickThroughRateFieldSpecified;

        private double conversionRateField;

        private bool conversionRateFieldSpecified;

        private double bounceRateField;

        private bool bounceRateFieldSpecified;

        private double complaintRateField;

        private bool complaintRateFieldSpecified;

        private double contactLossRateField;

        private bool contactLossRateFieldSpecified;

        private long numSocialSharesField;

        private bool numSocialSharesFieldSpecified;

        private long numSharesFacebookField;

        private bool numSharesFacebookFieldSpecified;

        private long numSharesTwitterField;

        private bool numSharesTwitterFieldSpecified;

        private long numSharesLinkedInField;

        private bool numSharesLinkedInFieldSpecified;

        private long numSharesDiggField;

        private bool numSharesDiggFieldSpecified;

        private long numSharesMySpaceField;

        private bool numSharesMySpaceFieldSpecified;

        private long numViewsFacebookField;

        private bool numViewsFacebookFieldSpecified;

        private long numViewsTwitterField;

        private bool numViewsTwitterFieldSpecified;

        private long numViewsLinkedInField;

        private bool numViewsLinkedInFieldSpecified;

        private long numViewsDiggField;

        private bool numViewsDiggFieldSpecified;

        private long numViewsMySpaceField;

        private bool numViewsMySpaceFieldSpecified;

        private long numSocialViewsField;

        private bool numSocialViewsFieldSpecified;

        private string cartIdField;

        private string orderIdField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public System.DateTime start
        {
            get { return startField; }
            set { startField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool startSpecified
        {
            get { return startFieldSpecified; }
            set { startFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string messageId
        {
            get { return messageIdField; }
            set { messageIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public string status
        {
            get { return statusField; }
            set { statusField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public string type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 5)]
        public string fromEmail
        {
            get { return fromEmailField; }
            set { fromEmailField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 6)]
        public string fromName
        {
            get { return fromNameField; }
            set { fromNameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 7)]
        public string replyEmail
        {
            get { return replyEmailField; }
            set { replyEmailField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 8)]
        public bool authentication
        {
            get { return authenticationField; }
            set { authenticationField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool authenticationSpecified
        {
            get { return authenticationFieldSpecified; }
            set { authenticationFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 9)]
        public bool replyTracking
        {
            get { return replyTrackingField; }
            set { replyTrackingField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool replyTrackingSpecified
        {
            get { return replyTrackingFieldSpecified; }
            set { replyTrackingFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 10)]
        public string messageRuleId
        {
            get { return messageRuleIdField; }
            set { messageRuleIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 11)]
        public bool optin
        {
            get { return optinField; }
            set { optinField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool optinSpecified
        {
            get { return optinFieldSpecified; }
            set { optinFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 12)]
        public long throttle
        {
            get { return throttleField; }
            set { throttleField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool throttleSpecified
        {
            get { return throttleFieldSpecified; }
            set { throttleFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 13)]
        public bool fatigueOverride
        {
            get { return fatigueOverrideField; }
            set { fatigueOverrideField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool fatigueOverrideSpecified
        {
            get { return fatigueOverrideFieldSpecified; }
            set { fatigueOverrideFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("content", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 14)]
        public messageContentObject[] content
        {
            get { return contentField; }
            set { contentField = value; }
        }

        /// <remarks/>
        [XmlElement("recipients", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 15)]
        public deliveryRecipientObject[] recipients
        {
            get { return recipientsField; }
            set { recipientsField = value; }
        }

        /// <remarks/>
        [XmlElement("fields", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 16)]
        public messageFieldObject[] fields
        {
            get { return fieldsField; }
            set { fieldsField = value; }
        }

        /// <remarks/>
        [XmlElement("products", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 17)]
        public deliveryProductObject[] products
        {
            get { return productsField; }
            set { productsField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 18)]
        public remailObject remail
        {
            get { return remailField; }
            set { remailField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 19)]
        public long numSends
        {
            get { return numSendsField; }
            set { numSendsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSendsSpecified
        {
            get { return numSendsFieldSpecified; }
            set { numSendsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 20)]
        public long numDeliveries
        {
            get { return numDeliveriesField; }
            set { numDeliveriesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numDeliveriesSpecified
        {
            get { return numDeliveriesFieldSpecified; }
            set { numDeliveriesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 21)]
        public long numHardBadEmail
        {
            get { return numHardBadEmailField; }
            set { numHardBadEmailField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numHardBadEmailSpecified
        {
            get { return numHardBadEmailFieldSpecified; }
            set { numHardBadEmailFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 22)]
        public long numHardDestUnreach
        {
            get { return numHardDestUnreachField; }
            set { numHardDestUnreachField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numHardDestUnreachSpecified
        {
            get { return numHardDestUnreachFieldSpecified; }
            set { numHardDestUnreachFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 23)]
        public long numHardMessageContent
        {
            get { return numHardMessageContentField; }
            set { numHardMessageContentField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numHardMessageContentSpecified
        {
            get { return numHardMessageContentFieldSpecified; }
            set { numHardMessageContentFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 24)]
        public long numHardBounces
        {
            get { return numHardBouncesField; }
            set { numHardBouncesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numHardBouncesSpecified
        {
            get { return numHardBouncesFieldSpecified; }
            set { numHardBouncesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 25)]
        public long numSoftBadEmail
        {
            get { return numSoftBadEmailField; }
            set { numSoftBadEmailField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSoftBadEmailSpecified
        {
            get { return numSoftBadEmailFieldSpecified; }
            set { numSoftBadEmailFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 26)]
        public long numSoftDestUnreach
        {
            get { return numSoftDestUnreachField; }
            set { numSoftDestUnreachField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSoftDestUnreachSpecified
        {
            get { return numSoftDestUnreachFieldSpecified; }
            set { numSoftDestUnreachFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 27)]
        public long numSoftMessageContent
        {
            get { return numSoftMessageContentField; }
            set { numSoftMessageContentField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSoftMessageContentSpecified
        {
            get { return numSoftMessageContentFieldSpecified; }
            set { numSoftMessageContentFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 28)]
        public long numSoftBounces
        {
            get { return numSoftBouncesField; }
            set { numSoftBouncesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSoftBouncesSpecified
        {
            get { return numSoftBouncesFieldSpecified; }
            set { numSoftBouncesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 29)]
        public long numOtherBounces
        {
            get { return numOtherBouncesField; }
            set { numOtherBouncesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numOtherBouncesSpecified
        {
            get { return numOtherBouncesFieldSpecified; }
            set { numOtherBouncesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 30)]
        public long numBounces
        {
            get { return numBouncesField; }
            set { numBouncesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numBouncesSpecified
        {
            get { return numBouncesFieldSpecified; }
            set { numBouncesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 31)]
        public long uniqOpens
        {
            get { return uniqOpensField; }
            set { uniqOpensField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool uniqOpensSpecified
        {
            get { return uniqOpensFieldSpecified; }
            set { uniqOpensFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 32)]
        public long numOpens
        {
            get { return numOpensField; }
            set { numOpensField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numOpensSpecified
        {
            get { return numOpensFieldSpecified; }
            set { numOpensFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 33)]
        public double avgOpens
        {
            get { return avgOpensField; }
            set { avgOpensField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool avgOpensSpecified
        {
            get { return avgOpensFieldSpecified; }
            set { avgOpensFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 34)]
        public long uniqClicks
        {
            get { return uniqClicksField; }
            set { uniqClicksField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool uniqClicksSpecified
        {
            get { return uniqClicksFieldSpecified; }
            set { uniqClicksFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 35)]
        public long numClicks
        {
            get { return numClicksField; }
            set { numClicksField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numClicksSpecified
        {
            get { return numClicksFieldSpecified; }
            set { numClicksFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 36)]
        public double avgClicks
        {
            get { return avgClicksField; }
            set { avgClicksField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool avgClicksSpecified
        {
            get { return avgClicksFieldSpecified; }
            set { avgClicksFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 37)]
        public long uniqConversions
        {
            get { return uniqConversionsField; }
            set { uniqConversionsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool uniqConversionsSpecified
        {
            get { return uniqConversionsFieldSpecified; }
            set { uniqConversionsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 38)]
        public long numConversions
        {
            get { return numConversionsField; }
            set { numConversionsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numConversionsSpecified
        {
            get { return numConversionsFieldSpecified; }
            set { numConversionsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 39)]
        public double avgConversions
        {
            get { return avgConversionsField; }
            set { avgConversionsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool avgConversionsSpecified
        {
            get { return avgConversionsFieldSpecified; }
            set { avgConversionsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 40)]
        public decimal revenue
        {
            get { return revenueField; }
            set { revenueField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool revenueSpecified
        {
            get { return revenueFieldSpecified; }
            set { revenueFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 41)]
        public long numSurveyResponses
        {
            get { return numSurveyResponsesField; }
            set { numSurveyResponsesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSurveyResponsesSpecified
        {
            get { return numSurveyResponsesFieldSpecified; }
            set { numSurveyResponsesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 42)]
        public long numFriendForwards
        {
            get { return numFriendForwardsField; }
            set { numFriendForwardsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numFriendForwardsSpecified
        {
            get { return numFriendForwardsFieldSpecified; }
            set { numFriendForwardsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 43)]
        public long numContactUpdates
        {
            get { return numContactUpdatesField; }
            set { numContactUpdatesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numContactUpdatesSpecified
        {
            get { return numContactUpdatesFieldSpecified; }
            set { numContactUpdatesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 44)]
        public long numUnsubscribesByPrefs
        {
            get { return numUnsubscribesByPrefsField; }
            set { numUnsubscribesByPrefsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numUnsubscribesByPrefsSpecified
        {
            get { return numUnsubscribesByPrefsFieldSpecified; }
            set { numUnsubscribesByPrefsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 45)]
        public long numUnsubscribesByComplaint
        {
            get { return numUnsubscribesByComplaintField; }
            set { numUnsubscribesByComplaintField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numUnsubscribesByComplaintSpecified
        {
            get { return numUnsubscribesByComplaintFieldSpecified; }
            set { numUnsubscribesByComplaintFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 46)]
        public long numContactLoss
        {
            get { return numContactLossField; }
            set { numContactLossField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numContactLossSpecified
        {
            get { return numContactLossFieldSpecified; }
            set { numContactLossFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 47)]
        public long numContactLossBounces
        {
            get { return numContactLossBouncesField; }
            set { numContactLossBouncesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numContactLossBouncesSpecified
        {
            get { return numContactLossBouncesFieldSpecified; }
            set { numContactLossBouncesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 48)]
        public double deliveryRate
        {
            get { return deliveryRateField; }
            set { deliveryRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool deliveryRateSpecified
        {
            get { return deliveryRateFieldSpecified; }
            set { deliveryRateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 49)]
        public double openRate
        {
            get { return openRateField; }
            set { openRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool openRateSpecified
        {
            get { return openRateFieldSpecified; }
            set { openRateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 50)]
        public double clickRate
        {
            get { return clickRateField; }
            set { clickRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool clickRateSpecified
        {
            get { return clickRateFieldSpecified; }
            set { clickRateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 51)]
        public double clickThroughRate
        {
            get { return clickThroughRateField; }
            set { clickThroughRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool clickThroughRateSpecified
        {
            get { return clickThroughRateFieldSpecified; }
            set { clickThroughRateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 52)]
        public double conversionRate
        {
            get { return conversionRateField; }
            set { conversionRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool conversionRateSpecified
        {
            get { return conversionRateFieldSpecified; }
            set { conversionRateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 53)]
        public double bounceRate
        {
            get { return bounceRateField; }
            set { bounceRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool bounceRateSpecified
        {
            get { return bounceRateFieldSpecified; }
            set { bounceRateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 54)]
        public double complaintRate
        {
            get { return complaintRateField; }
            set { complaintRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool complaintRateSpecified
        {
            get { return complaintRateFieldSpecified; }
            set { complaintRateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 55)]
        public double contactLossRate
        {
            get { return contactLossRateField; }
            set { contactLossRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool contactLossRateSpecified
        {
            get { return contactLossRateFieldSpecified; }
            set { contactLossRateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 56)]
        public long numSocialShares
        {
            get { return numSocialSharesField; }
            set { numSocialSharesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSocialSharesSpecified
        {
            get { return numSocialSharesFieldSpecified; }
            set { numSocialSharesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 57)]
        public long numSharesFacebook
        {
            get { return numSharesFacebookField; }
            set { numSharesFacebookField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSharesFacebookSpecified
        {
            get { return numSharesFacebookFieldSpecified; }
            set { numSharesFacebookFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 58)]
        public long numSharesTwitter
        {
            get { return numSharesTwitterField; }
            set { numSharesTwitterField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSharesTwitterSpecified
        {
            get { return numSharesTwitterFieldSpecified; }
            set { numSharesTwitterFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 59)]
        public long numSharesLinkedIn
        {
            get { return numSharesLinkedInField; }
            set { numSharesLinkedInField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSharesLinkedInSpecified
        {
            get { return numSharesLinkedInFieldSpecified; }
            set { numSharesLinkedInFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 60)]
        public long numSharesDigg
        {
            get { return numSharesDiggField; }
            set { numSharesDiggField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSharesDiggSpecified
        {
            get { return numSharesDiggFieldSpecified; }
            set { numSharesDiggFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 61)]
        public long numSharesMySpace
        {
            get { return numSharesMySpaceField; }
            set { numSharesMySpaceField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSharesMySpaceSpecified
        {
            get { return numSharesMySpaceFieldSpecified; }
            set { numSharesMySpaceFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 62)]
        public long numViewsFacebook
        {
            get { return numViewsFacebookField; }
            set { numViewsFacebookField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numViewsFacebookSpecified
        {
            get { return numViewsFacebookFieldSpecified; }
            set { numViewsFacebookFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 63)]
        public long numViewsTwitter
        {
            get { return numViewsTwitterField; }
            set { numViewsTwitterField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numViewsTwitterSpecified
        {
            get { return numViewsTwitterFieldSpecified; }
            set { numViewsTwitterFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 64)]
        public long numViewsLinkedIn
        {
            get { return numViewsLinkedInField; }
            set { numViewsLinkedInField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numViewsLinkedInSpecified
        {
            get { return numViewsLinkedInFieldSpecified; }
            set { numViewsLinkedInFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 65)]
        public long numViewsDigg
        {
            get { return numViewsDiggField; }
            set { numViewsDiggField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numViewsDiggSpecified
        {
            get { return numViewsDiggFieldSpecified; }
            set { numViewsDiggFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 66)]
        public long numViewsMySpace
        {
            get { return numViewsMySpaceField; }
            set { numViewsMySpaceField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numViewsMySpaceSpecified
        {
            get { return numViewsMySpaceFieldSpecified; }
            set { numViewsMySpaceFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 67)]
        public long numSocialViews
        {
            get { return numSocialViewsField; }
            set { numSocialViewsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSocialViewsSpecified
        {
            get { return numSocialViewsFieldSpecified; }
            set { numSocialViewsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 68)]
        public string cartId
        {
            get { return cartIdField; }
            set { cartIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 69)]
        public string orderId
        {
            get { return orderIdField; }
            set { orderIdField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class messageContentObject
    {

        private string typeField;

        private string subjectField;

        private string contentField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string subject
        {
            get { return subjectField; }
            set { subjectField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string content
        {
            get { return contentField; }
            set { contentField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class deliveryRecipientObject
    {

        private string deliveryTypeField;

        private string idField;

        private string typeField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string deliveryType
        {
            get { return deliveryTypeField; }
            set { deliveryTypeField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string type
        {
            get { return typeField; }
            set { typeField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class recentActivityObject
    {

        private System.DateTime createdDateField;

        private bool createdDateFieldSpecified;

        private string contactIdField;

        private string listIdField;

        private string segmentIdField;

        private string keywordIdField;

        private string messageIdField;

        private string deliveryIdField;

        private string workflowIdField;

        private string activityTypeField;

        private string emailAddressField;

        private string mobileNumberField;

        private string contactStatusField;

        private string messageNameField;

        private string deliveryTypeField;

        private System.DateTime deliveryStartField;

        private bool deliveryStartFieldSpecified;

        private string workflowNameField;

        private string segmentNameField;

        private string listNameField;

        private string listLabelField;

        private string automatorNameField;

        private string smsKeywordNameField;

        private string bounceTypeField;

        private string bounceReasonField;

        private string skipReasonField;

        private string linkNameField;

        private string linkUrlField;

        private string orderIdField;

        private string unsubscribeMethodField;

        private string ftafEmailsField;

        private string socialNetworkField;

        private string socialActivityField;

        private string webformTypeField;

        private string webformActionField;

        private string webformNameField;

        private string webformIdField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public System.DateTime createdDate
        {
            get { return createdDateField; }
            set { createdDateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool createdDateSpecified
        {
            get { return createdDateFieldSpecified; }
            set { createdDateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string contactId
        {
            get { return contactIdField; }
            set { contactIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string listId
        {
            get { return listIdField; }
            set { listIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public string segmentId
        {
            get { return segmentIdField; }
            set { segmentIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public string keywordId
        {
            get { return keywordIdField; }
            set { keywordIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 5)]
        public string messageId
        {
            get { return messageIdField; }
            set { messageIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 6)]
        public string deliveryId
        {
            get { return deliveryIdField; }
            set { deliveryIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 7)]
        public string workflowId
        {
            get { return workflowIdField; }
            set { workflowIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 8)]
        public string activityType
        {
            get { return activityTypeField; }
            set { activityTypeField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 9)]
        public string emailAddress
        {
            get { return emailAddressField; }
            set { emailAddressField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 10)]
        public string mobileNumber
        {
            get { return mobileNumberField; }
            set { mobileNumberField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 11)]
        public string contactStatus
        {
            get { return contactStatusField; }
            set { contactStatusField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 12)]
        public string messageName
        {
            get { return messageNameField; }
            set { messageNameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 13)]
        public string deliveryType
        {
            get { return deliveryTypeField; }
            set { deliveryTypeField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 14)]
        public System.DateTime deliveryStart
        {
            get { return deliveryStartField; }
            set { deliveryStartField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool deliveryStartSpecified
        {
            get { return deliveryStartFieldSpecified; }
            set { deliveryStartFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 15)]
        public string workflowName
        {
            get { return workflowNameField; }
            set { workflowNameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 16)]
        public string segmentName
        {
            get { return segmentNameField; }
            set { segmentNameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 17)]
        public string listName
        {
            get { return listNameField; }
            set { listNameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 18)]
        public string listLabel
        {
            get { return listLabelField; }
            set { listLabelField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 19)]
        public string automatorName
        {
            get { return automatorNameField; }
            set { automatorNameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 20)]
        public string smsKeywordName
        {
            get { return smsKeywordNameField; }
            set { smsKeywordNameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 21)]
        public string bounceType
        {
            get { return bounceTypeField; }
            set { bounceTypeField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 22)]
        public string bounceReason
        {
            get { return bounceReasonField; }
            set { bounceReasonField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 23)]
        public string skipReason
        {
            get { return skipReasonField; }
            set { skipReasonField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 24)]
        public string linkName
        {
            get { return linkNameField; }
            set { linkNameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 25)]
        public string linkUrl
        {
            get { return linkUrlField; }
            set { linkUrlField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 26)]
        public string orderId
        {
            get { return orderIdField; }
            set { orderIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 27)]
        public string unsubscribeMethod
        {
            get { return unsubscribeMethodField; }
            set { unsubscribeMethodField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 28)]
        public string ftafEmails
        {
            get { return ftafEmailsField; }
            set { ftafEmailsField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 29)]
        public string socialNetwork
        {
            get { return socialNetworkField; }
            set { socialNetworkField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 30)]
        public string socialActivity
        {
            get { return socialActivityField; }
            set { socialActivityField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 31)]
        public string webformType
        {
            get { return webformTypeField; }
            set { webformTypeField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 32)]
        public string webformAction
        {
            get { return webformActionField; }
            set { webformActionField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 33)]
        public string webformName
        {
            get { return webformNameField; }
            set { webformNameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 34)]
        public string webformId
        {
            get { return webformIdField; }
            set { webformIdField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class readRecentOutboundActivities
    {

        private recentOutboundActivitySearchRequest filterField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public recentOutboundActivitySearchRequest filter
        {
            get { return filterField; }
            set { filterField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class contactFilter
    {

        private filterType typeField;

        private bool typeFieldSpecified;

        private string[] idField;

        private stringValue[] emailField;

        private stringValue[] mobileNumberField;

        private string[] statusField;

        private dateValue[] createdField;

        private dateValue[] modifiedField;

        private string[] listIdField;

        private string[] segmentIdField;

        private string[] sMSKeywordIDField;

        private string[] msgPrefField;

        private string[] sourceField;

        private stringValue[] customSourceField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public filterType type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool typeSpecified
        {
            get { return typeFieldSpecified; }
            set { typeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("id", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 1)]
        public string[] id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement("email", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 2)]
        public stringValue[] email
        {
            get { return emailField; }
            set { emailField = value; }
        }

        /// <remarks/>
        [XmlElement("mobileNumber", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 3)]
        public stringValue[] mobileNumber
        {
            get { return mobileNumberField; }
            set { mobileNumberField = value; }
        }

        /// <remarks/>
        [XmlElement("status", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 4)]
        public string[] status
        {
            get { return statusField; }
            set { statusField = value; }
        }

        /// <remarks/>
        [XmlElement("created", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 5)]
        public dateValue[] created
        {
            get { return createdField; }
            set { createdField = value; }
        }

        /// <remarks/>
        [XmlElement("modified", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 6)]
        public dateValue[] modified
        {
            get { return modifiedField; }
            set { modifiedField = value; }
        }

        /// <remarks/>
        [XmlElement("listId", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 7)]
        public string[] listId
        {
            get { return listIdField; }
            set { listIdField = value; }
        }

        /// <remarks/>
        [XmlElement("segmentId", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 8)]
        public string[] segmentId
        {
            get { return segmentIdField; }
            set { segmentIdField = value; }
        }

        /// <remarks/>
        [XmlElement("SMSKeywordID", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 9)]
        public string[] SMSKeywordID
        {
            get { return sMSKeywordIDField; }
            set { sMSKeywordIDField = value; }
        }

        /// <remarks/>
        [XmlElement("msgPref", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 10)]
        public string[] msgPref
        {
            get { return msgPrefField; }
            set { msgPrefField = value; }
        }

        /// <remarks/>
        [XmlElement("source", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 11)]
        public string[] source
        {
            get { return sourceField; }
            set { sourceField = value; }
        }

        /// <remarks/>
        [XmlElement("customSource", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 12)]
        public stringValue[] customSource
        {
            get { return customSourceField; }
            set { customSourceField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class readContacts
    {

        private contactFilter filterField;

        private bool includeListsField;

        private bool includeListsFieldSpecified;

        private string[] fieldsField;

        private int pageNumberField;

        private bool includeSMSKeywordsField;

        private bool includeSMSKeywordsFieldSpecified;

        private bool includeGeoIPDataField;

        private bool includeGeoIPDataFieldSpecified;

        private bool includeTechnologyDataField;

        private bool includeTechnologyDataFieldSpecified;

        private bool includeRFMDataField;

        private bool includeRFMDataFieldSpecified;

        private bool includeEngagementDataField;

        private bool includeEngagementDataFieldSpecified;

        private bool includeSegmentsField;

        private bool includeSegmentsFieldSpecified;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public contactFilter filter
        {
            get { return filterField; }
            set { filterField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public bool includeLists
        {
            get { return includeListsField; }
            set { includeListsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool includeListsSpecified
        {
            get { return includeListsFieldSpecified; }
            set { includeListsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("fields", Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string[] fields
        {
            get { return fieldsField; }
            set { fieldsField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public int pageNumber
        {
            get { return pageNumberField; }
            set { pageNumberField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public bool includeSMSKeywords
        {
            get { return includeSMSKeywordsField; }
            set { includeSMSKeywordsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool includeSMSKeywordsSpecified
        {
            get { return includeSMSKeywordsFieldSpecified; }
            set { includeSMSKeywordsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 5)]
        public bool includeGeoIPData
        {
            get { return includeGeoIPDataField; }
            set { includeGeoIPDataField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool includeGeoIPDataSpecified
        {
            get { return includeGeoIPDataFieldSpecified; }
            set { includeGeoIPDataFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 6)]
        public bool includeTechnologyData
        {
            get { return includeTechnologyDataField; }
            set { includeTechnologyDataField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool includeTechnologyDataSpecified
        {
            get { return includeTechnologyDataFieldSpecified; }
            set { includeTechnologyDataFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 7)]
        public bool includeRFMData
        {
            get { return includeRFMDataField; }
            set { includeRFMDataField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool includeRFMDataSpecified
        {
            get { return includeRFMDataFieldSpecified; }
            set { includeRFMDataFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 8)]
        public bool includeEngagementData
        {
            get { return includeEngagementDataField; }
            set { includeEngagementDataField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool includeEngagementDataSpecified
        {
            get { return includeEngagementDataFieldSpecified; }
            set { includeEngagementDataFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 9)]
        public bool includeSegments
        {
            get { return includeSegmentsField; }
            set { includeSegmentsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool includeSegmentsSpecified
        {
            get { return includeSegmentsFieldSpecified; }
            set { includeSegmentsFieldSpecified = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class unsubscribeObject
    {

        private string contactIdField;

        private string deliveryIdField;

        private string methodField;

        private string complaintField;

        private System.DateTime createdField;

        private bool createdFieldSpecified;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string contactId
        {
            get { return contactIdField; }
            set { contactIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string deliveryId
        {
            get { return deliveryIdField; }
            set { deliveryIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string method
        {
            get { return methodField; }
            set { methodField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public string complaint
        {
            get { return complaintField; }
            set { complaintField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public System.DateTime created
        {
            get { return createdField; }
            set { createdField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool createdSpecified
        {
            get { return createdFieldSpecified; }
            set { createdFieldSpecified = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class unsubscribeFilter
    {

        private string contactIdField;

        private string deliveryIdField;

        private System.DateTime startField;

        private bool startFieldSpecified;

        private System.DateTime endField;

        private bool endFieldSpecified;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string contactId
        {
            get { return contactIdField; }
            set { contactIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string deliveryId
        {
            get { return deliveryIdField; }
            set { deliveryIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public System.DateTime start
        {
            get { return startField; }
            set { startField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool startSpecified
        {
            get { return startFieldSpecified; }
            set { startFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public System.DateTime end
        {
            get { return endField; }
            set { endField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool endSpecified
        {
            get { return endFieldSpecified; }
            set { endFieldSpecified = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class readUnsubscribes
    {

        private unsubscribeFilter filterField;

        private int pageNumberField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public unsubscribeFilter filter
        {
            get { return filterField; }
            set { filterField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public int pageNumber
        {
            get { return pageNumberField; }
            set { pageNumberField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class conversionObject
    {

        private string idField;

        private string contactIdField;

        private string emailField;

        private string orderIdField;

        private string itemField;

        private string descriptionField;

        private int quantityField;

        private bool quantityFieldSpecified;

        private decimal amountField;

        private bool amountFieldSpecified;

        private decimal orderTotalField;

        private bool orderTotalFieldSpecified;

        private System.DateTime createdDateField;

        private bool createdDateFieldSpecified;

        private string deliveryIdField;

        private string messageIdField;

        private string automatorIdField;

        private string listIdField;

        private string segmentIdField;

        private string deliveryTypeField;

        private string tidField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string contactId
        {
            get { return contactIdField; }
            set { contactIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string email
        {
            get { return emailField; }
            set { emailField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public string orderId
        {
            get { return orderIdField; }
            set { orderIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public string item
        {
            get { return itemField; }
            set { itemField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 5)]
        public string description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 6)]
        public int quantity
        {
            get { return quantityField; }
            set { quantityField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool quantitySpecified
        {
            get { return quantityFieldSpecified; }
            set { quantityFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 7)]
        public decimal amount
        {
            get { return amountField; }
            set { amountField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool amountSpecified
        {
            get { return amountFieldSpecified; }
            set { amountFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 8)]
        public decimal orderTotal
        {
            get { return orderTotalField; }
            set { orderTotalField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool orderTotalSpecified
        {
            get { return orderTotalFieldSpecified; }
            set { orderTotalFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 9)]
        public System.DateTime createdDate
        {
            get { return createdDateField; }
            set { createdDateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool createdDateSpecified
        {
            get { return createdDateFieldSpecified; }
            set { createdDateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 10)]
        public string deliveryId
        {
            get { return deliveryIdField; }
            set { deliveryIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 11)]
        public string messageId
        {
            get { return messageIdField; }
            set { messageIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 12)]
        public string automatorId
        {
            get { return automatorIdField; }
            set { automatorIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 13)]
        public string listId
        {
            get { return listIdField; }
            set { listIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 14)]
        public string segmentId
        {
            get { return segmentIdField; }
            set { segmentIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 15)]
        public string deliveryType
        {
            get { return deliveryTypeField; }
            set { deliveryTypeField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 16)]
        public string tid
        {
            get { return tidField; }
            set { tidField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class conversionFilter
    {

        private string[] contactIdField;

        private string[] deliveryIdField;

        private string[] idField;

        private string[] orderIdField;

        /// <remarks/>
        [XmlElement("contactId", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 0)]
        public string[] contactId
        {
            get { return contactIdField; }
            set { contactIdField = value; }
        }

        /// <remarks/>
        [XmlElement("deliveryId", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 1)]
        public string[] deliveryId
        {
            get { return deliveryIdField; }
            set { deliveryIdField = value; }
        }

        /// <remarks/>
        [XmlElement("id", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 2)]
        public string[] id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement("orderId", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 3)]
        public string[] orderId
        {
            get { return orderIdField; }
            set { orderIdField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class readConversions
    {

        private conversionFilter filterField;

        private int pageNumberField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public conversionFilter filter
        {
            get { return filterField; }
            set { filterField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public int pageNumber
        {
            get { return pageNumberField; }
            set { pageNumberField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class smsMessageObject
    {

        private string idField;

        private string nameField;

        private string statusField;

        private string messageFolderIdField;

        private bool shortenUrlsField;

        private bool shortenUrlsFieldSpecified;

        private string contentField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string status
        {
            get { return statusField; }
            set { statusField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public string messageFolderId
        {
            get { return messageFolderIdField; }
            set { messageFolderIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public bool shortenUrls
        {
            get { return shortenUrlsField; }
            set { shortenUrlsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool shortenUrlsSpecified
        {
            get { return shortenUrlsFieldSpecified; }
            set { shortenUrlsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 5)]
        public string content
        {
            get { return contentField; }
            set { contentField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class activityObject
    {

        private System.DateTime activityDateField;

        private bool activityDateFieldSpecified;

        private string contactIdField;

        private string deliveryIdField;

        private string messageIdField;

        private string listIdField;

        private string segmentIdField;

        private string trackingTypeField;

        private string bounceReasonField;

        private string bounceTypeField;

        private string linkNameField;

        private string linkUrlField;

        private string urlField;

        private int quantityField;

        private bool quantityFieldSpecified;

        private string amountField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public System.DateTime activityDate
        {
            get { return activityDateField; }
            set { activityDateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool activityDateSpecified
        {
            get { return activityDateFieldSpecified; }
            set { activityDateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string contactId
        {
            get { return contactIdField; }
            set { contactIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string deliveryId
        {
            get { return deliveryIdField; }
            set { deliveryIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public string messageId
        {
            get { return messageIdField; }
            set { messageIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public string listId
        {
            get { return listIdField; }
            set { listIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 5)]
        public string segmentId
        {
            get { return segmentIdField; }
            set { segmentIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 6)]
        public string trackingType
        {
            get { return trackingTypeField; }
            set { trackingTypeField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 7)]
        public string bounceReason
        {
            get { return bounceReasonField; }
            set { bounceReasonField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 8)]
        public string bounceType
        {
            get { return bounceTypeField; }
            set { bounceTypeField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 9)]
        public string linkName
        {
            get { return linkNameField; }
            set { linkNameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 10)]
        public string linkUrl
        {
            get { return linkUrlField; }
            set { linkUrlField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 11)]
        public string url
        {
            get { return urlField; }
            set { urlField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 12)]
        public int quantity
        {
            get { return quantityField; }
            set { quantityField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool quantitySpecified
        {
            get { return quantityFieldSpecified; }
            set { quantityFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 13)]
        public string amount
        {
            get { return amountField; }
            set { amountField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class activityFilter
    {

        private System.DateTime startField;

        private bool startFieldSpecified;

        private int sizeField;

        private bool sizeFieldSpecified;

        private string[] typesField;

        private readDirection readDirectionField;

        private bool readDirectionFieldSpecified;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public System.DateTime start
        {
            get { return startField; }
            set { startField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool startSpecified
        {
            get { return startFieldSpecified; }
            set { startFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public int size
        {
            get { return sizeField; }
            set { sizeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool sizeSpecified
        {
            get { return sizeFieldSpecified; }
            set { sizeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("types", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 2)]
        public string[] types
        {
            get { return typesField; }
            set { typesField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public readDirection readDirection
        {
            get { return readDirectionField; }
            set { readDirectionField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool readDirectionSpecified
        {
            get { return readDirectionFieldSpecified; }
            set { readDirectionFieldSpecified = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class readActivities
    {

        private activityFilter filterField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public activityFilter filter
        {
            get { return filterField; }
            set { filterField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class contentTagObject
    {

        private string idField;

        private string nameField;

        private string valueField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string value
        {
            get { return valueField; }
            set { valueField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class addToList
    {

        private mailListObject listField;

        private contactObject[] contactsField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public mailListObject list
        {
            get { return listField; }
            set { listField = value; }
        }

        /// <remarks/>
        [XmlElement("contacts", Form = XmlSchemaForm.Unqualified, Order = 1)]
        public contactObject[] contacts
        {
            get { return contactsField; }
            set { contactsField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class fieldOptionObject
    {

        private string labelField;

        private string valueField;

        private bool isDefaultField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string label
        {
            get { return labelField; }
            set { labelField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string value
        {
            get { return valueField; }
            set { valueField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public bool isDefault
        {
            get { return isDefaultField; }
            set { isDefaultField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class fieldObject
    {

        private string idField;

        private string nameField;

        private string labelField;

        private string typeField;

        private string visibilityField;

        private fieldOptionObject[] optionsField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string label
        {
            get { return labelField; }
            set { labelField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public string type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public string visibility
        {
            get { return visibilityField; }
            set { visibilityField = value; }
        }

        /// <remarks/>
        [XmlElement("options", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 5)]
        public fieldOptionObject[] options
        {
            get { return optionsField; }
            set { optionsField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class headerFooterObject
    {

        private string idField;

        private string nameField;

        private string htmlField;

        private string textField;

        private bool isHeaderField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string html
        {
            get { return htmlField; }
            set { htmlField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public string text
        {
            get { return textField; }
            set { textField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public bool isHeader
        {
            get { return isHeaderField; }
            set { isHeaderField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class headerFooterFilter
    {

        private filterType typeField;

        private bool typeFieldSpecified;

        private string[] idField;

        private stringValue[] nameField;

        private string[] positionField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public filterType type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool typeSpecified
        {
            get { return typeFieldSpecified; }
            set { typeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("id", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 1)]
        public string[] id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement("name", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 2)]
        public stringValue[] name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks/>
        [XmlElement("position", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 3)]
        public string[] position
        {
            get { return positionField; }
            set { positionField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class readHeaderFooters
    {

        private headerFooterFilter filterField;

        private bool includeContentField;

        private int pageNumberField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public headerFooterFilter filter
        {
            get { return filterField; }
            set { filterField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public bool includeContent
        {
            get { return includeContentField; }
            set { includeContentField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public int pageNumber
        {
            get { return pageNumberField; }
            set { pageNumberField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class productObject
    {

        private string idField;

        private string skuField;

        private string nameField;

        private string descriptionField;

        private string categoryField;

        private string imageField;

        private string urlField;

        private int quantityField;

        private bool quantityFieldSpecified;

        private decimal priceField;

        private bool priceFieldSpecified;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string sku
        {
            get { return skuField; }
            set { skuField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public string description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public string category
        {
            get { return categoryField; }
            set { categoryField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 5)]
        public string image
        {
            get { return imageField; }
            set { imageField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 6)]
        public string url
        {
            get { return urlField; }
            set { urlField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 7)]
        public int quantity
        {
            get { return quantityField; }
            set { quantityField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool quantitySpecified
        {
            get { return quantityFieldSpecified; }
            set { quantityFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 8)]
        public decimal price
        {
            get { return priceField; }
            set { priceField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool priceSpecified
        {
            get { return priceFieldSpecified; }
            set { priceFieldSpecified = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class orderObject
    {

        private string idField;

        private string contactIdField;

        private string emailField;

        private productObject[] productsField;

        private System.DateTime orderDateField;

        private bool orderDateFieldSpecified;

        private string deliveryIdField;

        private string messageIdField;

        private string automatorIdField;

        private string listIdField;

        private string segmentIdField;

        private string deliveryTypeField;

        private string tidField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string contactId
        {
            get { return contactIdField; }
            set { contactIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string email
        {
            get { return emailField; }
            set { emailField = value; }
        }

        /// <remarks/>
        [XmlElement("products", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 3)]
        public productObject[] products
        {
            get { return productsField; }
            set { productsField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public System.DateTime orderDate
        {
            get { return orderDateField; }
            set { orderDateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool orderDateSpecified
        {
            get { return orderDateFieldSpecified; }
            set { orderDateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 5)]
        public string deliveryId
        {
            get { return deliveryIdField; }
            set { deliveryIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 6)]
        public string messageId
        {
            get { return messageIdField; }
            set { messageIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 7)]
        public string automatorId
        {
            get { return automatorIdField; }
            set { automatorIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 8)]
        public string listId
        {
            get { return listIdField; }
            set { listIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 9)]
        public string segmentId
        {
            get { return segmentIdField; }
            set { segmentIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 10)]
        public string deliveryType
        {
            get { return deliveryTypeField; }
            set { deliveryTypeField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 11)]
        public string tid
        {
            get { return tidField; }
            set { tidField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class messageFolderObject
    {

        private string idField;

        private string nameField;

        private string parentIdField;

        private string parentNameField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string parentId
        {
            get { return parentIdField; }
            set { parentIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public string parentName
        {
            get { return parentNameField; }
            set { parentNameField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class messageFolderFilter
    {

        private filterType typeField;

        private bool typeFieldSpecified;

        private string[] idField;

        private stringValue[] nameField;

        private string[] parentIdField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public filterType type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool typeSpecified
        {
            get { return typeFieldSpecified; }
            set { typeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("id", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 1)]
        public string[] id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement("name", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 2)]
        public stringValue[] name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks/>
        [XmlElement("parentId", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 3)]
        public string[] parentId
        {
            get { return parentIdField; }
            set { parentIdField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class readMessageFolders
    {

        private messageFolderFilter filterField;

        private int pageNumberField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public messageFolderFilter filter
        {
            get { return filterField; }
            set { filterField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public int pageNumber
        {
            get { return pageNumberField; }
            set { pageNumberField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class smsMessageFieldObject
    {

        private string nameField;

        private string contentField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string content
        {
            get { return contentField; }
            set { contentField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class smsDeliveryContactsObject
    {

        private string keywordField;

        private string[] contactIdsField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string keyword
        {
            get { return keywordField; }
            set { keywordField = value; }
        }

        /// <remarks/>
        [XmlElement("contactIds", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 1)]
        public string[] contactIds
        {
            get { return contactIdsField; }
            set { contactIdsField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class smsDeliveryObject
    {

        private string idField;

        private System.DateTime startField;

        private bool startFieldSpecified;

        private string messageIdField;

        private string deliveryTypeField;

        private string statusField;

        private string contentField;

        private deliveryRecipientObject[] recipientsField;

        private smsDeliveryContactsObject contactRecipientsField;

        private string[] keywordsField;

        private smsMessageFieldObject[] fieldsField;

        private long numSendsField;

        private bool numSendsFieldSpecified;

        private long numDeliveriesField;

        private bool numDeliveriesFieldSpecified;

        private long numIncomingField;

        private bool numIncomingFieldSpecified;

        private long numBouncesField;

        private bool numBouncesFieldSpecified;

        private double deliveryRateField;

        private bool deliveryRateFieldSpecified;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public System.DateTime start
        {
            get { return startField; }
            set { startField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool startSpecified
        {
            get { return startFieldSpecified; }
            set { startFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string messageId
        {
            get { return messageIdField; }
            set { messageIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public string deliveryType
        {
            get { return deliveryTypeField; }
            set { deliveryTypeField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public string status
        {
            get { return statusField; }
            set { statusField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 5)]
        public string content
        {
            get { return contentField; }
            set { contentField = value; }
        }

        /// <remarks/>
        [XmlElement("recipients", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 6)]
        public deliveryRecipientObject[] recipients
        {
            get { return recipientsField; }
            set { recipientsField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 7)]
        public smsDeliveryContactsObject contactRecipients
        {
            get { return contactRecipientsField; }
            set { contactRecipientsField = value; }
        }

        /// <remarks/>
        [XmlElement("keywords", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 8)]
        public string[] keywords
        {
            get { return keywordsField; }
            set { keywordsField = value; }
        }

        /// <remarks/>
        [XmlElement("fields", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 9)]
        public smsMessageFieldObject[] fields
        {
            get { return fieldsField; }
            set { fieldsField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 10)]
        public long numSends
        {
            get { return numSendsField; }
            set { numSendsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSendsSpecified
        {
            get { return numSendsFieldSpecified; }
            set { numSendsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 11)]
        public long numDeliveries
        {
            get { return numDeliveriesField; }
            set { numDeliveriesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numDeliveriesSpecified
        {
            get { return numDeliveriesFieldSpecified; }
            set { numDeliveriesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 12)]
        public long numIncoming
        {
            get { return numIncomingField; }
            set { numIncomingField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numIncomingSpecified
        {
            get { return numIncomingFieldSpecified; }
            set { numIncomingFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 13)]
        public long numBounces
        {
            get { return numBouncesField; }
            set { numBouncesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numBouncesSpecified
        {
            get { return numBouncesFieldSpecified; }
            set { numBouncesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 14)]
        public double deliveryRate
        {
            get { return deliveryRateField; }
            set { deliveryRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool deliveryRateSpecified
        {
            get { return deliveryRateFieldSpecified; }
            set { deliveryRateFieldSpecified = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class messageObject
    {

        private string idField;

        private string nameField;

        private string statusField;

        private string messageFolderIdField;

        private messageContentObject[] contentField;

        private long numSendsField;

        private bool numSendsFieldSpecified;

        private long numDeliveriesField;

        private bool numDeliveriesFieldSpecified;

        private long numHardBadEmailField;

        private bool numHardBadEmailFieldSpecified;

        private long numHardDestUnreachField;

        private bool numHardDestUnreachFieldSpecified;

        private long numHardMessageContentField;

        private bool numHardMessageContentFieldSpecified;

        private long numHardBouncesField;

        private bool numHardBouncesFieldSpecified;

        private long numSoftBadEmailField;

        private bool numSoftBadEmailFieldSpecified;

        private long numSoftDestUnreachField;

        private bool numSoftDestUnreachFieldSpecified;

        private long numSoftMessageContentField;

        private bool numSoftMessageContentFieldSpecified;

        private long numSoftBouncesField;

        private bool numSoftBouncesFieldSpecified;

        private long numOtherBouncesField;

        private bool numOtherBouncesFieldSpecified;

        private long numBouncesField;

        private bool numBouncesFieldSpecified;

        private long uniqOpensField;

        private bool uniqOpensFieldSpecified;

        private long numOpensField;

        private bool numOpensFieldSpecified;

        private double avgOpensField;

        private bool avgOpensFieldSpecified;

        private long uniqClicksField;

        private bool uniqClicksFieldSpecified;

        private long numClicksField;

        private bool numClicksFieldSpecified;

        private double avgClicksField;

        private bool avgClicksFieldSpecified;

        private long uniqConversionsField;

        private bool uniqConversionsFieldSpecified;

        private long numConversionsField;

        private bool numConversionsFieldSpecified;

        private double avgConversionsField;

        private bool avgConversionsFieldSpecified;

        private decimal revenueField;

        private bool revenueFieldSpecified;

        private long numSurveyResponsesField;

        private bool numSurveyResponsesFieldSpecified;

        private long numFriendForwardsField;

        private bool numFriendForwardsFieldSpecified;

        private long numContactUpdatesField;

        private bool numContactUpdatesFieldSpecified;

        private long numUnsubscribesByPrefsField;

        private bool numUnsubscribesByPrefsFieldSpecified;

        private long numUnsubscribesByComplaintField;

        private bool numUnsubscribesByComplaintFieldSpecified;

        private long numContactLossBouncesField;

        private bool numContactLossBouncesFieldSpecified;

        private long numContactLossField;

        private bool numContactLossFieldSpecified;

        private double deliveryRateField;

        private bool deliveryRateFieldSpecified;

        private double openRateField;

        private bool openRateFieldSpecified;

        private double clickRateField;

        private bool clickRateFieldSpecified;

        private double clickThroughRateField;

        private bool clickThroughRateFieldSpecified;

        private double conversionRateField;

        private bool conversionRateFieldSpecified;

        private double bounceRateField;

        private bool bounceRateFieldSpecified;

        private double complaintRateField;

        private bool complaintRateFieldSpecified;

        private double contactLossRateField;

        private bool contactLossRateFieldSpecified;

        private long numSocialSharesField;

        private bool numSocialSharesFieldSpecified;

        private long numSharesFacebookField;

        private bool numSharesFacebookFieldSpecified;

        private long numSharesTwitterField;

        private bool numSharesTwitterFieldSpecified;

        private long numSharesLinkedInField;

        private bool numSharesLinkedInFieldSpecified;

        private long numSharesDiggField;

        private bool numSharesDiggFieldSpecified;

        private long numSharesMySpaceField;

        private bool numSharesMySpaceFieldSpecified;

        private long numSocialViewsField;

        private bool numSocialViewsFieldSpecified;

        private long numViewsFacebookField;

        private bool numViewsFacebookFieldSpecified;

        private long numViewsTwitterField;

        private bool numViewsTwitterFieldSpecified;

        private long numViewsLinkedInField;

        private bool numViewsLinkedInFieldSpecified;

        private long numViewsDiggField;

        private bool numViewsDiggFieldSpecified;

        private long numViewsMySpaceField;

        private bool numViewsMySpaceFieldSpecified;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string status
        {
            get { return statusField; }
            set { statusField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public string messageFolderId
        {
            get { return messageFolderIdField; }
            set { messageFolderIdField = value; }
        }

        /// <remarks/>
        [XmlElement("content", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 4)]
        public messageContentObject[] content
        {
            get { return contentField; }
            set { contentField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 5)]
        public long numSends
        {
            get { return numSendsField; }
            set { numSendsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSendsSpecified
        {
            get { return numSendsFieldSpecified; }
            set { numSendsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 6)]
        public long numDeliveries
        {
            get { return numDeliveriesField; }
            set { numDeliveriesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numDeliveriesSpecified
        {
            get { return numDeliveriesFieldSpecified; }
            set { numDeliveriesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 7)]
        public long numHardBadEmail
        {
            get { return numHardBadEmailField; }
            set { numHardBadEmailField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numHardBadEmailSpecified
        {
            get { return numHardBadEmailFieldSpecified; }
            set { numHardBadEmailFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 8)]
        public long numHardDestUnreach
        {
            get { return numHardDestUnreachField; }
            set { numHardDestUnreachField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numHardDestUnreachSpecified
        {
            get { return numHardDestUnreachFieldSpecified; }
            set { numHardDestUnreachFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 9)]
        public long numHardMessageContent
        {
            get { return numHardMessageContentField; }
            set { numHardMessageContentField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numHardMessageContentSpecified
        {
            get { return numHardMessageContentFieldSpecified; }
            set { numHardMessageContentFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 10)]
        public long numHardBounces
        {
            get { return numHardBouncesField; }
            set { numHardBouncesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numHardBouncesSpecified
        {
            get { return numHardBouncesFieldSpecified; }
            set { numHardBouncesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 11)]
        public long numSoftBadEmail
        {
            get { return numSoftBadEmailField; }
            set { numSoftBadEmailField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSoftBadEmailSpecified
        {
            get { return numSoftBadEmailFieldSpecified; }
            set { numSoftBadEmailFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 12)]
        public long numSoftDestUnreach
        {
            get { return numSoftDestUnreachField; }
            set { numSoftDestUnreachField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSoftDestUnreachSpecified
        {
            get { return numSoftDestUnreachFieldSpecified; }
            set { numSoftDestUnreachFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 13)]
        public long numSoftMessageContent
        {
            get { return numSoftMessageContentField; }
            set { numSoftMessageContentField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSoftMessageContentSpecified
        {
            get { return numSoftMessageContentFieldSpecified; }
            set { numSoftMessageContentFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 14)]
        public long numSoftBounces
        {
            get { return numSoftBouncesField; }
            set { numSoftBouncesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSoftBouncesSpecified
        {
            get { return numSoftBouncesFieldSpecified; }
            set { numSoftBouncesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 15)]
        public long numOtherBounces
        {
            get { return numOtherBouncesField; }
            set { numOtherBouncesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numOtherBouncesSpecified
        {
            get { return numOtherBouncesFieldSpecified; }
            set { numOtherBouncesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 16)]
        public long numBounces
        {
            get { return numBouncesField; }
            set { numBouncesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numBouncesSpecified
        {
            get { return numBouncesFieldSpecified; }
            set { numBouncesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 17)]
        public long uniqOpens
        {
            get { return uniqOpensField; }
            set { uniqOpensField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool uniqOpensSpecified
        {
            get { return uniqOpensFieldSpecified; }
            set { uniqOpensFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 18)]
        public long numOpens
        {
            get { return numOpensField; }
            set { numOpensField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numOpensSpecified
        {
            get { return numOpensFieldSpecified; }
            set { numOpensFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 19)]
        public double avgOpens
        {
            get { return avgOpensField; }
            set { avgOpensField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool avgOpensSpecified
        {
            get { return avgOpensFieldSpecified; }
            set { avgOpensFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 20)]
        public long uniqClicks
        {
            get { return uniqClicksField; }
            set { uniqClicksField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool uniqClicksSpecified
        {
            get { return uniqClicksFieldSpecified; }
            set { uniqClicksFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 21)]
        public long numClicks
        {
            get { return numClicksField; }
            set { numClicksField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numClicksSpecified
        {
            get { return numClicksFieldSpecified; }
            set { numClicksFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 22)]
        public double avgClicks
        {
            get { return avgClicksField; }
            set { avgClicksField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool avgClicksSpecified
        {
            get { return avgClicksFieldSpecified; }
            set { avgClicksFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 23)]
        public long uniqConversions
        {
            get { return uniqConversionsField; }
            set { uniqConversionsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool uniqConversionsSpecified
        {
            get { return uniqConversionsFieldSpecified; }
            set { uniqConversionsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 24)]
        public long numConversions
        {
            get { return numConversionsField; }
            set { numConversionsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numConversionsSpecified
        {
            get { return numConversionsFieldSpecified; }
            set { numConversionsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 25)]
        public double avgConversions
        {
            get { return avgConversionsField; }
            set { avgConversionsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool avgConversionsSpecified
        {
            get { return avgConversionsFieldSpecified; }
            set { avgConversionsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 26)]
        public decimal revenue
        {
            get { return revenueField; }
            set { revenueField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool revenueSpecified
        {
            get { return revenueFieldSpecified; }
            set { revenueFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 27)]
        public long numSurveyResponses
        {
            get { return numSurveyResponsesField; }
            set { numSurveyResponsesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSurveyResponsesSpecified
        {
            get { return numSurveyResponsesFieldSpecified; }
            set { numSurveyResponsesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 28)]
        public long numFriendForwards
        {
            get { return numFriendForwardsField; }
            set { numFriendForwardsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numFriendForwardsSpecified
        {
            get { return numFriendForwardsFieldSpecified; }
            set { numFriendForwardsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 29)]
        public long numContactUpdates
        {
            get { return numContactUpdatesField; }
            set { numContactUpdatesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numContactUpdatesSpecified
        {
            get { return numContactUpdatesFieldSpecified; }
            set { numContactUpdatesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 30)]
        public long numUnsubscribesByPrefs
        {
            get { return numUnsubscribesByPrefsField; }
            set { numUnsubscribesByPrefsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numUnsubscribesByPrefsSpecified
        {
            get { return numUnsubscribesByPrefsFieldSpecified; }
            set { numUnsubscribesByPrefsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 31)]
        public long numUnsubscribesByComplaint
        {
            get { return numUnsubscribesByComplaintField; }
            set { numUnsubscribesByComplaintField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numUnsubscribesByComplaintSpecified
        {
            get { return numUnsubscribesByComplaintFieldSpecified; }
            set { numUnsubscribesByComplaintFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 32)]
        public long numContactLossBounces
        {
            get { return numContactLossBouncesField; }
            set { numContactLossBouncesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numContactLossBouncesSpecified
        {
            get { return numContactLossBouncesFieldSpecified; }
            set { numContactLossBouncesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 33)]
        public long numContactLoss
        {
            get { return numContactLossField; }
            set { numContactLossField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numContactLossSpecified
        {
            get { return numContactLossFieldSpecified; }
            set { numContactLossFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 34)]
        public double deliveryRate
        {
            get { return deliveryRateField; }
            set { deliveryRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool deliveryRateSpecified
        {
            get { return deliveryRateFieldSpecified; }
            set { deliveryRateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 35)]
        public double openRate
        {
            get { return openRateField; }
            set { openRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool openRateSpecified
        {
            get { return openRateFieldSpecified; }
            set { openRateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 36)]
        public double clickRate
        {
            get { return clickRateField; }
            set { clickRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool clickRateSpecified
        {
            get { return clickRateFieldSpecified; }
            set { clickRateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 37)]
        public double clickThroughRate
        {
            get { return clickThroughRateField; }
            set { clickThroughRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool clickThroughRateSpecified
        {
            get { return clickThroughRateFieldSpecified; }
            set { clickThroughRateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 38)]
        public double conversionRate
        {
            get { return conversionRateField; }
            set { conversionRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool conversionRateSpecified
        {
            get { return conversionRateFieldSpecified; }
            set { conversionRateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 39)]
        public double bounceRate
        {
            get { return bounceRateField; }
            set { bounceRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool bounceRateSpecified
        {
            get { return bounceRateFieldSpecified; }
            set { bounceRateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 40)]
        public double complaintRate
        {
            get { return complaintRateField; }
            set { complaintRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool complaintRateSpecified
        {
            get { return complaintRateFieldSpecified; }
            set { complaintRateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 41)]
        public double contactLossRate
        {
            get { return contactLossRateField; }
            set { contactLossRateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool contactLossRateSpecified
        {
            get { return contactLossRateFieldSpecified; }
            set { contactLossRateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 42)]
        public long numSocialShares
        {
            get { return numSocialSharesField; }
            set { numSocialSharesField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSocialSharesSpecified
        {
            get { return numSocialSharesFieldSpecified; }
            set { numSocialSharesFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 43)]
        public long numSharesFacebook
        {
            get { return numSharesFacebookField; }
            set { numSharesFacebookField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSharesFacebookSpecified
        {
            get { return numSharesFacebookFieldSpecified; }
            set { numSharesFacebookFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 44)]
        public long numSharesTwitter
        {
            get { return numSharesTwitterField; }
            set { numSharesTwitterField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSharesTwitterSpecified
        {
            get { return numSharesTwitterFieldSpecified; }
            set { numSharesTwitterFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 45)]
        public long numSharesLinkedIn
        {
            get { return numSharesLinkedInField; }
            set { numSharesLinkedInField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSharesLinkedInSpecified
        {
            get { return numSharesLinkedInFieldSpecified; }
            set { numSharesLinkedInFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 46)]
        public long numSharesDigg
        {
            get { return numSharesDiggField; }
            set { numSharesDiggField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSharesDiggSpecified
        {
            get { return numSharesDiggFieldSpecified; }
            set { numSharesDiggFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 47)]
        public long numSharesMySpace
        {
            get { return numSharesMySpaceField; }
            set { numSharesMySpaceField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSharesMySpaceSpecified
        {
            get { return numSharesMySpaceFieldSpecified; }
            set { numSharesMySpaceFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 48)]
        public long numSocialViews
        {
            get { return numSocialViewsField; }
            set { numSocialViewsField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numSocialViewsSpecified
        {
            get { return numSocialViewsFieldSpecified; }
            set { numSocialViewsFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 49)]
        public long numViewsFacebook
        {
            get { return numViewsFacebookField; }
            set { numViewsFacebookField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numViewsFacebookSpecified
        {
            get { return numViewsFacebookFieldSpecified; }
            set { numViewsFacebookFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 50)]
        public long numViewsTwitter
        {
            get { return numViewsTwitterField; }
            set { numViewsTwitterField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numViewsTwitterSpecified
        {
            get { return numViewsTwitterFieldSpecified; }
            set { numViewsTwitterFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 51)]
        public long numViewsLinkedIn
        {
            get { return numViewsLinkedInField; }
            set { numViewsLinkedInField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numViewsLinkedInSpecified
        {
            get { return numViewsLinkedInFieldSpecified; }
            set { numViewsLinkedInFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 52)]
        public long numViewsDigg
        {
            get { return numViewsDiggField; }
            set { numViewsDiggField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numViewsDiggSpecified
        {
            get { return numViewsDiggFieldSpecified; }
            set { numViewsDiggFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 53)]
        public long numViewsMySpace
        {
            get { return numViewsMySpaceField; }
            set { numViewsMySpaceField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool numViewsMySpaceSpecified
        {
            get { return numViewsMySpaceFieldSpecified; }
            set { numViewsMySpaceFieldSpecified = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class mailListFilter
    {

        private filterType typeField;

        private bool typeFieldSpecified;

        private string[] idField;

        private stringValue[] nameField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public filterType type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool typeSpecified
        {
            get { return typeFieldSpecified; }
            set { typeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("id", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 1)]
        public string[] id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement("name", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 2)]
        public stringValue[] name
        {
            get { return nameField; }
            set { nameField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class readLists
    {

        private mailListFilter filterField;

        private int pageNumberField;

        private int pageSizeField;

        private bool pageSizeFieldSpecified;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public mailListFilter filter
        {
            get { return filterField; }
            set { filterField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public int pageNumber
        {
            get { return pageNumberField; }
            set { pageNumberField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public int pageSize
        {
            get { return pageSizeField; }
            set { pageSizeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool pageSizeSpecified
        {
            get { return pageSizeFieldSpecified; }
            set { pageSizeFieldSpecified = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class messageRuleObject
    {

        private string idField;

        private string nameField;

        private string typeField;

        private string messageIdField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public string messageId
        {
            get { return messageIdField; }
            set { messageIdField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class apiTokenObject
    {

        private string idField;

        private string nameField;

        private int permissionsField;

        private bool activeField;

        private System.DateTime createdField;

        private bool createdFieldSpecified;

        private System.DateTime modifiedField;

        private bool modifiedFieldSpecified;

        private string accountIdField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public int permissions
        {
            get { return permissionsField; }
            set { permissionsField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public bool active
        {
            get { return activeField; }
            set { activeField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public System.DateTime created
        {
            get { return createdField; }
            set { createdField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool createdSpecified
        {
            get { return createdFieldSpecified; }
            set { createdFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 5)]
        public System.DateTime modified
        {
            get { return modifiedField; }
            set { modifiedField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool modifiedSpecified
        {
            get { return modifiedFieldSpecified; }
            set { modifiedFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 6)]
        public string accountId
        {
            get { return accountIdField; }
            set { accountIdField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class apiTokenFilter
    {

        private filterType typeField;

        private bool typeFieldSpecified;

        private string[] idField;

        private string[] accountIdField;

        private stringValue[] nameField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public filterType type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool typeSpecified
        {
            get { return typeFieldSpecified; }
            set { typeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("id", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 1)]
        public string[] id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement("accountId", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 2)]
        public string[] accountId
        {
            get { return accountIdField; }
            set { accountIdField = value; }
        }

        /// <remarks/>
        [XmlElement("name", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 3)]
        public stringValue[] name
        {
            get { return nameField; }
            set { nameField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class readApiTokens
    {

        private apiTokenFilter filterField;

        private int pageNumberField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public apiTokenFilter filter
        {
            get { return filterField; }
            set { filterField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public int pageNumber
        {
            get { return pageNumberField; }
            set { pageNumberField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class workflowObject
    {

        private string idField;

        private string siteIdField;

        private string nameField;

        private string descriptionField;

        private string statusField;

        private System.DateTime createdDateField;

        private bool createdDateFieldSpecified;

        private System.DateTime modifiedDateField;

        private bool modifiedDateFieldSpecified;

        private System.DateTime activatedDateField;

        private bool activatedDateFieldSpecified;

        private System.DateTime deActivatedDateField;

        private bool deActivatedDateFieldSpecified;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string siteId
        {
            get { return siteIdField; }
            set { siteIdField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public string name
        {
            get { return nameField; }
            set { nameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public string description
        {
            get { return descriptionField; }
            set { descriptionField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public string status
        {
            get { return statusField; }
            set { statusField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 5)]
        public System.DateTime createdDate
        {
            get { return createdDateField; }
            set { createdDateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool createdDateSpecified
        {
            get { return createdDateFieldSpecified; }
            set { createdDateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 6)]
        public System.DateTime modifiedDate
        {
            get { return modifiedDateField; }
            set { modifiedDateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool modifiedDateSpecified
        {
            get { return modifiedDateFieldSpecified; }
            set { modifiedDateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 7)]
        public System.DateTime activatedDate
        {
            get { return activatedDateField; }
            set { activatedDateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool activatedDateSpecified
        {
            get { return activatedDateFieldSpecified; }
            set { activatedDateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 8)]
        public System.DateTime deActivatedDate
        {
            get { return deActivatedDateField; }
            set { deActivatedDateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool deActivatedDateSpecified
        {
            get { return deActivatedDateFieldSpecified; }
            set { deActivatedDateFieldSpecified = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class addContactsToWorkflow
    {

        private workflowObject workflowField;

        private contactObject[] contactsField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public workflowObject workflow
        {
            get { return workflowField; }
            set { workflowField = value; }
        }

        /// <remarks/>
        [XmlElement("contacts", Form = XmlSchemaForm.Unqualified, Order = 1)]
        public contactObject[] contacts
        {
            get { return contactsField; }
            set { contactsField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class resultItem
    {

        private string idField;

        private bool isNewField;

        private bool isNewFieldSpecified;

        private bool isErrorField;

        private bool isErrorFieldSpecified;

        private int errorCodeField;

        private string errorStringField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string id
        {
            get { return idField; }
            set { idField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public bool isNew
        {
            get { return isNewField; }
            set { isNewField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool isNewSpecified
        {
            get { return isNewFieldSpecified; }
            set { isNewFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public bool isError
        {
            get { return isErrorField; }
            set { isErrorField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool isErrorSpecified
        {
            get { return isErrorFieldSpecified; }
            set { isErrorFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public int errorCode
        {
            get { return errorCodeField; }
            set { errorCodeField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public string errorString
        {
            get { return errorStringField; }
            set { errorStringField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class writeResult
    {

        private System.Nullable<int>[] errorsField;

        private resultItem[] resultsField;

        /// <remarks/>
        [XmlElement("errors", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 0)]
        public System.Nullable<int>[] errors
        {
            get { return errorsField; }
            set { errorsField = value; }
        }

        /// <remarks/>
        [XmlElement("results", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 1)]
        public resultItem[] results
        {
            get { return resultsField; }
            set { resultsField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class loginObject
    {

        private string usernameField;

        private string passwordField;

        private contactInformation contactInformationField;

        private bool permissionAgencyAdminField;

        private bool permissionAgencyAdminFieldSpecified;

        private bool permissionAdminField;

        private bool permissionAdminFieldSpecified;

        private bool permissionApiField;

        private bool permissionApiFieldSpecified;

        private bool permissionUpgradeField;

        private bool permissionUpgradeFieldSpecified;

        private bool permissionFatigueOverrideField;

        private bool permissionFatigueOverrideFieldSpecified;

        private bool permissionMessageComposeField;

        private bool permissionMessageComposeFieldSpecified;

        private bool permissionMessageApproveField;

        private bool permissionMessageApproveFieldSpecified;

        private bool permissionMessageDeleteField;

        private bool permissionMessageDeleteFieldSpecified;

        private bool permissionAutomatorComposeField;

        private bool permissionAutomatorComposeFieldSpecified;

        private bool permissionListCreateSendField;

        private bool permissionListCreateSendFieldSpecified;

        private bool permissionListCreateField;

        private bool permissionListCreateFieldSpecified;

        private bool permissionSegmentCreateField;

        private bool permissionSegmentCreateFieldSpecified;

        private bool permissionFieldCreateField;

        private bool permissionFieldCreateFieldSpecified;

        private bool permissionFieldReorderField;

        private bool permissionFieldReorderFieldSpecified;

        private bool permissionSubscriberCreateField;

        private bool permissionSubscriberCreateFieldSpecified;

        private bool permissionSubscriberViewField;

        private bool permissionSubscriberViewFieldSpecified;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public string username
        {
            get { return usernameField; }
            set { usernameField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public string password
        {
            get { return passwordField; }
            set { passwordField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 2)]
        public contactInformation contactInformation
        {
            get { return contactInformationField; }
            set { contactInformationField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 3)]
        public bool permissionAgencyAdmin
        {
            get { return permissionAgencyAdminField; }
            set { permissionAgencyAdminField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool permissionAgencyAdminSpecified
        {
            get { return permissionAgencyAdminFieldSpecified; }
            set { permissionAgencyAdminFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 4)]
        public bool permissionAdmin
        {
            get { return permissionAdminField; }
            set { permissionAdminField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool permissionAdminSpecified
        {
            get { return permissionAdminFieldSpecified; }
            set { permissionAdminFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 5)]
        public bool permissionApi
        {
            get { return permissionApiField; }
            set { permissionApiField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool permissionApiSpecified
        {
            get { return permissionApiFieldSpecified; }
            set { permissionApiFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 6)]
        public bool permissionUpgrade
        {
            get { return permissionUpgradeField; }
            set { permissionUpgradeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool permissionUpgradeSpecified
        {
            get { return permissionUpgradeFieldSpecified; }
            set { permissionUpgradeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 7)]
        public bool permissionFatigueOverride
        {
            get { return permissionFatigueOverrideField; }
            set { permissionFatigueOverrideField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool permissionFatigueOverrideSpecified
        {
            get { return permissionFatigueOverrideFieldSpecified; }
            set { permissionFatigueOverrideFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 8)]
        public bool permissionMessageCompose
        {
            get { return permissionMessageComposeField; }
            set { permissionMessageComposeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool permissionMessageComposeSpecified
        {
            get { return permissionMessageComposeFieldSpecified; }
            set { permissionMessageComposeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 9)]
        public bool permissionMessageApprove
        {
            get { return permissionMessageApproveField; }
            set { permissionMessageApproveField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool permissionMessageApproveSpecified
        {
            get { return permissionMessageApproveFieldSpecified; }
            set { permissionMessageApproveFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 10)]
        public bool permissionMessageDelete
        {
            get { return permissionMessageDeleteField; }
            set { permissionMessageDeleteField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool permissionMessageDeleteSpecified
        {
            get { return permissionMessageDeleteFieldSpecified; }
            set { permissionMessageDeleteFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 11)]
        public bool permissionAutomatorCompose
        {
            get { return permissionAutomatorComposeField; }
            set { permissionAutomatorComposeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool permissionAutomatorComposeSpecified
        {
            get { return permissionAutomatorComposeFieldSpecified; }
            set { permissionAutomatorComposeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 12)]
        public bool permissionListCreateSend
        {
            get { return permissionListCreateSendField; }
            set { permissionListCreateSendField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool permissionListCreateSendSpecified
        {
            get { return permissionListCreateSendFieldSpecified; }
            set { permissionListCreateSendFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 13)]
        public bool permissionListCreate
        {
            get { return permissionListCreateField; }
            set { permissionListCreateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool permissionListCreateSpecified
        {
            get { return permissionListCreateFieldSpecified; }
            set { permissionListCreateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 14)]
        public bool permissionSegmentCreate
        {
            get { return permissionSegmentCreateField; }
            set { permissionSegmentCreateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool permissionSegmentCreateSpecified
        {
            get { return permissionSegmentCreateFieldSpecified; }
            set { permissionSegmentCreateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 15)]
        public bool permissionFieldCreate
        {
            get { return permissionFieldCreateField; }
            set { permissionFieldCreateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool permissionFieldCreateSpecified
        {
            get { return permissionFieldCreateFieldSpecified; }
            set { permissionFieldCreateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 16)]
        public bool permissionFieldReorder
        {
            get { return permissionFieldReorderField; }
            set { permissionFieldReorderField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool permissionFieldReorderSpecified
        {
            get { return permissionFieldReorderFieldSpecified; }
            set { permissionFieldReorderFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 17)]
        public bool permissionSubscriberCreate
        {
            get { return permissionSubscriberCreateField; }
            set { permissionSubscriberCreateField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool permissionSubscriberCreateSpecified
        {
            get { return permissionSubscriberCreateFieldSpecified; }
            set { permissionSubscriberCreateFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 18)]
        public bool permissionSubscriberView
        {
            get { return permissionSubscriberViewField; }
            set { permissionSubscriberViewField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool permissionSubscriberViewSpecified
        {
            get { return permissionSubscriberViewFieldSpecified; }
            set { permissionSubscriberViewFieldSpecified = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class loginFilter
    {

        private filterType typeField;

        private bool typeFieldSpecified;

        private stringValue[] usernameField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public filterType type
        {
            get { return typeField; }
            set { typeField = value; }
        }

        /// <remarks/>
        [XmlIgnore]
        public bool typeSpecified
        {
            get { return typeFieldSpecified; }
            set { typeFieldSpecified = value; }
        }

        /// <remarks/>
        [XmlElement("username", Form = XmlSchemaForm.Unqualified, IsNullable = true, Order = 1)]
        public stringValue[] username
        {
            get { return usernameField; }
            set { usernameField = value; }
        }
    }

    /// <remarks/>

    //[System.SerializableAttribute]
    [DebuggerStepThrough]
    //[System.ComponentModel.DesignerCategoryAttribute("code")]
    [XmlType(Namespace = "http://api.bronto.com/v4")]
    public partial class readLogins
    {

        private loginFilter filterField;

        private int pageNumberField;

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 0)]
        public loginFilter filter
        {
            get { return filterField; }
            set { filterField = value; }
        }

        /// <remarks/>
        [XmlElement(Form = XmlSchemaForm.Unqualified, Order = 1)]
        public int pageNumber
        {
            get { return pageNumberField; }
            set { pageNumberField = value; }
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class readLogins1
    {
        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public readLogins readLogins;

        public readLogins1()
        {
        }

        public readLogins1(sessionHeader sessionHeader, readLogins readLogins)
        {
            this.sessionHeader = sessionHeader;
            this.readLogins = readLogins;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "readLoginsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class readLoginsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement("return", Form = XmlSchemaForm.Unqualified)] public loginObject[] @return;

        public readLoginsResponse()
        {
        }

        public readLoginsResponse(loginObject[] @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class deleteLogins
    {
        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "deleteLogins", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("accounts", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public loginObject[] deleteLogins1;

        public deleteLogins()
        {
        }

        public deleteLogins(sessionHeader sessionHeader, loginObject[] deleteLogins1)
        {
            this.sessionHeader = sessionHeader;
            this.deleteLogins1 = deleteLogins1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "deleteLoginsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class deleteLoginsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public deleteLoginsResponse()
        {
        }

        public deleteLoginsResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class deleteDeliveryGroup
    {
        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "deleteDeliveryGroup", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("deliveryGroups", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public deliveryGroupObject[]
            deleteDeliveryGroup1;

        public deleteDeliveryGroup()
        {
        }

        public deleteDeliveryGroup(sessionHeader sessionHeader, deliveryGroupObject[] deleteDeliveryGroup1)
        {
            this.sessionHeader = sessionHeader;
            this.deleteDeliveryGroup1 = deleteDeliveryGroup1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "deleteDeliveryGroupResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class deleteDeliveryGroupResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public deleteDeliveryGroupResponse()
        {
        }

        public deleteDeliveryGroupResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class addContactsToWorkflow1
    {
        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public addContactsToWorkflow addContactsToWorkflow;

        public addContactsToWorkflow1()
        {
        }

        public addContactsToWorkflow1(sessionHeader sessionHeader, addContactsToWorkflow addContactsToWorkflow)
        {
            this.sessionHeader = sessionHeader;
            this.addContactsToWorkflow = addContactsToWorkflow;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "addContactsToWorkflowResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class addContactsToWorkflowResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public addContactsToWorkflowResponse()
        {
        }

        public addContactsToWorkflowResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class readApiTokens1
    {
        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public readApiTokens readApiTokens;

        public readApiTokens1()
        {
        }

        public readApiTokens1(sessionHeader sessionHeader, readApiTokens readApiTokens)
        {
            this.sessionHeader = sessionHeader;
            this.readApiTokens = readApiTokens;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "readApiTokensResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class readApiTokensResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement("return", Form = XmlSchemaForm.Unqualified)] public apiTokenObject[] @return;

        public readApiTokensResponse()
        {
        }

        public readApiTokensResponse(apiTokenObject[] @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class updateMessageRules
    {
        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "updateMessageRules", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("messageRules", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public messageRuleObject[] updateMessageRules1;

        public updateMessageRules()
        {
        }

        public updateMessageRules(sessionHeader sessionHeader, messageRuleObject[] updateMessageRules1)
        {
            this.sessionHeader = sessionHeader;
            this.updateMessageRules1 = updateMessageRules1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "updateMessageRulesResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class updateMessageRulesResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public updateMessageRulesResponse()
        {
        }

        public updateMessageRulesResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class deleteMessageRules
    {
        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "deleteMessageRules", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("messageRules", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public messageRuleObject[] deleteMessageRules1;

        public deleteMessageRules()
        {
        }

        public deleteMessageRules(sessionHeader sessionHeader, messageRuleObject[] deleteMessageRules1)
        {
            this.sessionHeader = sessionHeader;
            this.deleteMessageRules1 = deleteMessageRules1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "deleteMessageRulesResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class deleteMessageRulesResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public deleteMessageRulesResponse()
        {
        }

        public deleteMessageRulesResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class readLists1
    {
        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public readLists readLists;

        public readLists1()
        {
        }

        public readLists1(sessionHeader sessionHeader, readLists readLists)
        {
            this.sessionHeader = sessionHeader;
            this.readLists = readLists;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "readListsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class readListsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement("return", Form = XmlSchemaForm.Unqualified)] public mailListObject[] @return;

        public readListsResponse()
        {
        }

        public readListsResponse(mailListObject[] @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class deleteMessages
    {
        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "deleteMessages", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("messages", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public messageObject[] deleteMessages1;

        public deleteMessages()
        {
        }

        public deleteMessages(sessionHeader sessionHeader, messageObject[] deleteMessages1)
        {
            this.sessionHeader = sessionHeader;
            this.deleteMessages1 = deleteMessages1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "deleteMessagesResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class deleteMessagesResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public deleteMessagesResponse()
        {
        }

        public deleteMessagesResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class updateSMSDeliveries
    {
        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "updateSMSDeliveries", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("smsdeliveries", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public smsDeliveryObject[]
            updateSMSDeliveries1;

        public updateSMSDeliveries()
        {
        }

        public updateSMSDeliveries(sessionHeader sessionHeader, smsDeliveryObject[] updateSMSDeliveries1)
        {
            this.sessionHeader = sessionHeader;
            this.updateSMSDeliveries1 = updateSMSDeliveries1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "updateSMSDeliveriesResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class updateSMSDeliveriesResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public updateSMSDeliveriesResponse()
        {
        }

        public updateSMSDeliveriesResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class readMessageFolders1
    {
        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public readMessageFolders readMessageFolders;

        public readMessageFolders1()
        {
        }

        public readMessageFolders1(sessionHeader sessionHeader, readMessageFolders readMessageFolders)
        {
            this.sessionHeader = sessionHeader;
            this.readMessageFolders = readMessageFolders;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "readMessageFoldersResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class readMessageFoldersResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement("return", Form = XmlSchemaForm.Unqualified)] public messageFolderObject[] @return;

        public readMessageFoldersResponse()
        {
        }

        public readMessageFoldersResponse(messageFolderObject[] @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class addUpdateOrder
    {
        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "addUpdateOrder", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("orders", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public orderObject[] addUpdateOrder1;

        public addUpdateOrder()
        {
        }

        public addUpdateOrder(sessionHeader sessionHeader, orderObject[] addUpdateOrder1)
        {
            this.sessionHeader = sessionHeader;
            this.addUpdateOrder1 = addUpdateOrder1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "addUpdateOrderResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class addUpdateOrderResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public addUpdateOrderResponse()
        {
        }

        public addUpdateOrderResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class updateDeliveryGroup
    {
        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "updateDeliveryGroup", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("deliveryGroups", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public deliveryGroupObject[]
            updateDeliveryGroup1;

        public updateDeliveryGroup()
        {
        }

        public updateDeliveryGroup(sessionHeader sessionHeader, deliveryGroupObject[] updateDeliveryGroup1)
        {
            this.sessionHeader = sessionHeader;
            this.updateDeliveryGroup1 = updateDeliveryGroup1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "updateDeliveryGroupResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class updateDeliveryGroupResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public updateDeliveryGroupResponse()
        {
        }

        public updateDeliveryGroupResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class readHeaderFooters1
    {
        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public readHeaderFooters readHeaderFooters;

        public readHeaderFooters1()
        {
        }

        public readHeaderFooters1(sessionHeader sessionHeader, readHeaderFooters readHeaderFooters)
        {
            this.sessionHeader = sessionHeader;
            this.readHeaderFooters = readHeaderFooters;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "readHeaderFootersResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class readHeaderFootersResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement("return", Form = XmlSchemaForm.Unqualified)] public headerFooterObject[] @return;

        public readHeaderFootersResponse()
        {
        }

        public readHeaderFootersResponse(headerFooterObject[] @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class deleteApiTokens
    {
        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "deleteApiTokens", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("tokens", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public apiTokenObject[] deleteApiTokens1;

        public deleteApiTokens()
        {
        }

        public deleteApiTokens(sessionHeader sessionHeader, apiTokenObject[] deleteApiTokens1)
        {
            this.sessionHeader = sessionHeader;
            this.deleteApiTokens1 = deleteApiTokens1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "deleteApiTokensResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class deleteApiTokensResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public deleteApiTokensResponse()
        {
        }

        public deleteApiTokensResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class addFields
    {
        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "addFields", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("fields", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public fieldObject[] addFields1;

        public addFields()
        {
        }

        public addFields(sessionHeader sessionHeader, fieldObject[] addFields1)
        {
            this.sessionHeader = sessionHeader;
            this.addFields1 = addFields1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "addFieldsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class addFieldsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public addFieldsResponse()
        {
        }

        public addFieldsResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class deleteHeaderFooters
    {
        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "deleteHeaderFooters", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("footers", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public headerFooterObject[] deleteHeaderFooters1;

        public deleteHeaderFooters()
        {
        }

        public deleteHeaderFooters(sessionHeader sessionHeader, headerFooterObject[] deleteHeaderFooters1)
        {
            this.sessionHeader = sessionHeader;
            this.deleteHeaderFooters1 = deleteHeaderFooters1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "deleteHeaderFootersResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class deleteHeaderFootersResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public deleteHeaderFootersResponse()
        {
        }

        public deleteHeaderFootersResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class deleteWorkflows
    {
        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "deleteWorkflows", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("workflows", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public workflowObject[] deleteWorkflows1;

        public deleteWorkflows()
        {
        }

        public deleteWorkflows(sessionHeader sessionHeader, workflowObject[] deleteWorkflows1)
        {
            this.sessionHeader = sessionHeader;
            this.deleteWorkflows1 = deleteWorkflows1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "deleteWorkflowsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class deleteWorkflowsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public deleteWorkflowsResponse()
        {
        }

        public deleteWorkflowsResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class addToList1
    {
        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public addToList addToList;

        public addToList1()
        {
        }

        public addToList1(sessionHeader sessionHeader, addToList addToList)
        {
            this.sessionHeader = sessionHeader;
            this.addToList = addToList;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "addToListResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class addToListResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public addToListResponse()
        {
        }

        public addToListResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class updateContentTags
    {
        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "updateContentTags", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("contentTags", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public contentTagObject[] updateContentTags1;

        public updateContentTags()
        {
        }

        public updateContentTags(sessionHeader sessionHeader, contentTagObject[] updateContentTags1)
        {
            this.sessionHeader = sessionHeader;
            this.updateContentTags1 = updateContentTags1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "updateContentTagsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class updateContentTagsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public updateContentTagsResponse()
        {
        }

        public updateContentTagsResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class readActivities1
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public readActivities readActivities;

        public readActivities1()
        {
        }

        public readActivities1(sessionHeader sessionHeader, readActivities readActivities)
        {
            this.sessionHeader = sessionHeader;
            this.readActivities = readActivities;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "readActivitiesResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class readActivitiesResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement("return", Form = XmlSchemaForm.Unqualified)] public activityObject[] @return;

        public readActivitiesResponse()
        {
        }

        public readActivitiesResponse(activityObject[] @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class addSMSMessages
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "addSMSMessages", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("messages", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public smsMessageObject[] addSMSMessages1;

        public addSMSMessages()
        {
        }

        public addSMSMessages(sessionHeader sessionHeader, smsMessageObject[] addSMSMessages1)
        {
            this.sessionHeader = sessionHeader;
            this.addSMSMessages1 = addSMSMessages1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "addSMSMessagesResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class addSMSMessagesResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public addSMSMessagesResponse()
        {
        }

        public addSMSMessagesResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class readConversions1
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public readConversions readConversions;

        public readConversions1()
        {
        }

        public readConversions1(sessionHeader sessionHeader, readConversions readConversions)
        {
            this.sessionHeader = sessionHeader;
            this.readConversions = readConversions;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "readConversionsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class readConversionsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement("return", Form = XmlSchemaForm.Unqualified)] public conversionObject[] @return;

        public readConversionsResponse()
        {
        }

        public readConversionsResponse(conversionObject[] @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class deleteContacts
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "deleteContacts", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("contacts", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public contactObject[] deleteContacts1;

        public deleteContacts()
        {
        }

        public deleteContacts(sessionHeader sessionHeader, contactObject[] deleteContacts1)
        {
            this.sessionHeader = sessionHeader;
            this.deleteContacts1 = deleteContacts1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "deleteContactsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class deleteContactsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public deleteContactsResponse()
        {
        }

        public deleteContactsResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class addDeliveryGroup
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "addDeliveryGroup", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("deliveryGroups", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public deliveryGroupObject[]
            addDeliveryGroup1;

        public addDeliveryGroup()
        {
        }

        public addDeliveryGroup(sessionHeader sessionHeader, deliveryGroupObject[] addDeliveryGroup1)
        {
            this.sessionHeader = sessionHeader;
            this.addDeliveryGroup1 = addDeliveryGroup1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "addDeliveryGroupResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class addDeliveryGroupResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public addDeliveryGroupResponse()
        {
        }

        public addDeliveryGroupResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class updateSMSKeywords
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "updateSMSKeywords", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("keyword", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public smsKeywordObject[] updateSMSKeywords1;

        public updateSMSKeywords()
        {
        }

        public updateSMSKeywords(sessionHeader sessionHeader, smsKeywordObject[] updateSMSKeywords1)
        {
            this.sessionHeader = sessionHeader;
            this.updateSMSKeywords1 = updateSMSKeywords1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "updateSMSKeywordsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class updateSMSKeywordsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public updateSMSKeywordsResponse()
        {
        }

        public updateSMSKeywordsResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class updateMessages
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "updateMessages", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("messages", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public messageObject[] updateMessages1;

        public updateMessages()
        {
        }

        public updateMessages(sessionHeader sessionHeader, messageObject[] updateMessages1)
        {
            this.sessionHeader = sessionHeader;
            this.updateMessages1 = updateMessages1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "updateMessagesResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class updateMessagesResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public updateMessagesResponse()
        {
        }

        public updateMessagesResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class readUnsubscribes1
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public readUnsubscribes readUnsubscribes;

        public readUnsubscribes1()
        {
        }

        public readUnsubscribes1(sessionHeader sessionHeader, readUnsubscribes readUnsubscribes)
        {
            this.sessionHeader = sessionHeader;
            this.readUnsubscribes = readUnsubscribes;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "readUnsubscribesResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class readUnsubscribesResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement("return", Form = XmlSchemaForm.Unqualified)] public unsubscribeObject[] @return;

        public readUnsubscribesResponse()
        {
        }

        public readUnsubscribesResponse(unsubscribeObject[] @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class readContacts1
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public readContacts readContacts;

        public readContacts1()
        {
        }

        public readContacts1(sessionHeader sessionHeader, readContacts readContacts)
        {
            this.sessionHeader = sessionHeader;
            this.readContacts = readContacts;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "readContactsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class readContactsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement("return", Form = XmlSchemaForm.Unqualified)] public contactObject[] @return;

        public readContactsResponse()
        {
        }

        public readContactsResponse(contactObject[] @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class readRecentOutboundActivities1
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public readRecentOutboundActivities
            readRecentOutboundActivities;

        public readRecentOutboundActivities1()
        {
        }

        public readRecentOutboundActivities1(sessionHeader sessionHeader, readRecentOutboundActivities readRecentOutboundActivities)
        {
            this.sessionHeader = sessionHeader;
            this.readRecentOutboundActivities = readRecentOutboundActivities;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "readRecentOutboundActivitiesResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class readRecentOutboundActivitiesResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement("return", Form = XmlSchemaForm.Unqualified)] public recentActivityObject[] @return;

        public readRecentOutboundActivitiesResponse()
        {
        }

        public readRecentOutboundActivitiesResponse(recentActivityObject[] @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class addContentTags
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "addContentTags", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("contentTags", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public contentTagObject[] addContentTags1;

        public addContentTags()
        {
        }

        public addContentTags(sessionHeader sessionHeader, contentTagObject[] addContentTags1)
        {
            this.sessionHeader = sessionHeader;
            this.addContentTags1 = addContentTags1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "addContentTagsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class addContentTagsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public addContentTagsResponse()
        {
        }

        public addContentTagsResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class updateDeliveries
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "updateDeliveries", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("deliveries", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public deliveryObject[] updateDeliveries1;

        public updateDeliveries()
        {
        }

        public updateDeliveries(sessionHeader sessionHeader, deliveryObject[] updateDeliveries1)
        {
            this.sessionHeader = sessionHeader;
            this.updateDeliveries1 = updateDeliveries1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "updateDeliveriesResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class updateDeliveriesResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public updateDeliveriesResponse()
        {
        }

        public updateDeliveriesResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class deleteSMSMessages
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "deleteSMSMessages", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("messages", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public smsMessageObject[] deleteSMSMessages1;

        public deleteSMSMessages()
        {
        }

        public deleteSMSMessages(sessionHeader sessionHeader, smsMessageObject[] deleteSMSMessages1)
        {
            this.sessionHeader = sessionHeader;
            this.deleteSMSMessages1 = deleteSMSMessages1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "deleteSMSMessagesResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class deleteSMSMessagesResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public deleteSMSMessagesResponse()
        {
        }

        public deleteSMSMessagesResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class addSMSKeywords
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "addSMSKeywords", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("keyword", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public smsKeywordObject[] addSMSKeywords1;

        public addSMSKeywords()
        {
        }

        public addSMSKeywords(sessionHeader sessionHeader, smsKeywordObject[] addSMSKeywords1)
        {
            this.sessionHeader = sessionHeader;
            this.addSMSKeywords1 = addSMSKeywords1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "addSMSKeywordsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class addSMSKeywordsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public addSMSKeywordsResponse()
        {
        }

        public addSMSKeywordsResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class readWorkflows1
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public readWorkflows readWorkflows;

        public readWorkflows1()
        {
        }

        public readWorkflows1(sessionHeader sessionHeader, readWorkflows readWorkflows)
        {
            this.sessionHeader = sessionHeader;
            this.readWorkflows = readWorkflows;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "readWorkflowsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class readWorkflowsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement("return", Form = XmlSchemaForm.Unqualified)] public workflowObject[] @return;

        public readWorkflowsResponse()
        {
        }

        public readWorkflowsResponse(workflowObject[] @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class updateApiTokens
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "updateApiTokens", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("tokens", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public apiTokenObject[] updateApiTokens1;

        public updateApiTokens()
        {
        }

        public updateApiTokens(sessionHeader sessionHeader, apiTokenObject[] updateApiTokens1)
        {
            this.sessionHeader = sessionHeader;
            this.updateApiTokens1 = updateApiTokens1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "updateApiTokensResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class updateApiTokensResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public updateApiTokensResponse()
        {
        }

        public updateApiTokensResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class readAccounts1
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public readAccounts readAccounts;

        public readAccounts1()
        {
        }

        public readAccounts1(sessionHeader sessionHeader, readAccounts readAccounts)
        {
            this.sessionHeader = sessionHeader;
            this.readAccounts = readAccounts;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "readAccountsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class readAccountsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement("return", Form = XmlSchemaForm.Unqualified)] public accountObject[] @return;

        public readAccountsResponse()
        {
        }

        public readAccountsResponse(accountObject[] @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class addToSMSKeyword1
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public addToSMSKeyword addToSMSKeyword;

        public addToSMSKeyword1()
        {
        }

        public addToSMSKeyword1(sessionHeader sessionHeader, addToSMSKeyword addToSMSKeyword)
        {
            this.sessionHeader = sessionHeader;
            this.addToSMSKeyword = addToSMSKeyword;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "addToSMSKeywordResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class addToSMSKeywordResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public addToSMSKeywordResponse()
        {
        }

        public addToSMSKeywordResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class removeFromList1
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public removeFromList removeFromList;

        public removeFromList1()
        {
        }

        public removeFromList1(sessionHeader sessionHeader, removeFromList removeFromList)
        {
            this.sessionHeader = sessionHeader;
            this.removeFromList = removeFromList;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "removeFromListResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class removeFromListResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public removeFromListResponse()
        {
        }

        public removeFromListResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class readDeliveryRecipients1
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public readDeliveryRecipients readDeliveryRecipients;

        public readDeliveryRecipients1()
        {
        }

        public readDeliveryRecipients1(sessionHeader sessionHeader, readDeliveryRecipients readDeliveryRecipients)
        {
            this.sessionHeader = sessionHeader;
            this.readDeliveryRecipients = readDeliveryRecipients;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "readDeliveryRecipientsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class readDeliveryRecipientsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement("return", Form = XmlSchemaForm.Unqualified)] public deliveryRecipientStatObject[] @return;

        public readDeliveryRecipientsResponse()
        {
        }

        public readDeliveryRecipientsResponse(deliveryRecipientStatObject[] @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class addLists
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "addLists", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("lists", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public mailListObject[] addLists1;

        public addLists()
        {
        }

        public addLists(sessionHeader sessionHeader, mailListObject[] addLists1)
        {
            this.sessionHeader = sessionHeader;
            this.addLists1 = addLists1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "addListsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class addListsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public addListsResponse()
        {
        }

        public addListsResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class readSegments1
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public readSegments readSegments;

        public readSegments1()
        {
        }

        public readSegments1(sessionHeader sessionHeader, readSegments readSegments)
        {
            this.sessionHeader = sessionHeader;
            this.readSegments = readSegments;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "readSegmentsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class readSegmentsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement("return", Form = XmlSchemaForm.Unqualified)] public segmentObject[] @return;

        public readSegmentsResponse()
        {
        }

        public readSegmentsResponse(segmentObject[] @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class readSMSKeywords1
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public readSMSKeywords readSMSKeywords;

        public readSMSKeywords1()
        {
        }

        public readSMSKeywords1(sessionHeader sessionHeader, readSMSKeywords readSMSKeywords)
        {
            this.sessionHeader = sessionHeader;
            this.readSMSKeywords = readSMSKeywords;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "readSMSKeywordsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class readSMSKeywordsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement("return", Form = XmlSchemaForm.Unqualified)] public smsKeywordObject[] @return;

        public readSMSKeywordsResponse()
        {
        }

        public readSMSKeywordsResponse(smsKeywordObject[] @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class readRecentInboundActivities1
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public readRecentInboundActivities
            readRecentInboundActivities;

        public readRecentInboundActivities1()
        {
        }

        public readRecentInboundActivities1(sessionHeader sessionHeader, readRecentInboundActivities readRecentInboundActivities)
        {
            this.sessionHeader = sessionHeader;
            this.readRecentInboundActivities = readRecentInboundActivities;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "readRecentInboundActivitiesResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class readRecentInboundActivitiesResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement("return", Form = XmlSchemaForm.Unqualified)] public recentActivityObject[] @return;

        public readRecentInboundActivitiesResponse()
        {
        }

        public readRecentInboundActivitiesResponse(recentActivityObject[] @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class addDeliveries
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "addDeliveries", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("deliveries", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public deliveryObject[] addDeliveries1;

        public addDeliveries()
        {
        }

        public addDeliveries(sessionHeader sessionHeader, deliveryObject[] addDeliveries1)
        {
            this.sessionHeader = sessionHeader;
            this.addDeliveries1 = addDeliveries1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "addDeliveriesResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class addDeliveriesResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public addDeliveriesResponse()
        {
        }

        public addDeliveriesResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class addContacts
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "addContacts", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("contacts", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public contactObject[] addContacts1;

        public addContacts()
        {
        }

        public addContacts(sessionHeader sessionHeader, contactObject[] addContacts1)
        {
            this.sessionHeader = sessionHeader;
            this.addContacts1 = addContacts1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "addContactsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class addContactsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public addContactsResponse()
        {
        }

        public addContactsResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class addContactEvent1
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public addContactEvent addContactEvent;

        public addContactEvent1()
        {
        }

        public addContactEvent1(sessionHeader sessionHeader, addContactEvent addContactEvent)
        {
            this.sessionHeader = sessionHeader;
            this.addContactEvent = addContactEvent;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "addContactEventResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class addContactEventResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public addContactEventResponse()
        {
        }

        public addContactEventResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class deleteDeliveries
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "deleteDeliveries", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("deliveries", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public deliveryObject[] deleteDeliveries1;

        public deleteDeliveries()
        {
        }

        public deleteDeliveries(sessionHeader sessionHeader, deliveryObject[] deleteDeliveries1)
        {
            this.sessionHeader = sessionHeader;
            this.deleteDeliveries1 = deleteDeliveries1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "deleteDeliveriesResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class deleteDeliveriesResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public deleteDeliveriesResponse()
        {
        }

        public deleteDeliveriesResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "login", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class login
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public string
            apiToken;

        public login()
        {
        }

        public login(string apiToken)
        {
            this.apiToken = apiToken;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "loginResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class loginResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public string
            @return;

        public loginResponse()
        {
        }

        public loginResponse(string @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class deleteOrders
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "deleteOrders", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("orders", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public orderObject[] deleteOrders1;

        public deleteOrders()
        {
        }

        public deleteOrders(sessionHeader sessionHeader, orderObject[] deleteOrders1)
        {
            this.sessionHeader = sessionHeader;
            this.deleteOrders1 = deleteOrders1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "deleteOrdersResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class deleteOrdersResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public deleteOrdersResponse()
        {
        }

        public deleteOrdersResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class addOrUpdateDeliveryGroup
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "addOrUpdateDeliveryGroup", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("deliveryGroups", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public deliveryGroupObject[]
            addOrUpdateDeliveryGroup1;

        public addOrUpdateDeliveryGroup()
        {
        }

        public addOrUpdateDeliveryGroup(sessionHeader sessionHeader, deliveryGroupObject[] addOrUpdateDeliveryGroup1)
        {
            this.sessionHeader = sessionHeader;
            this.addOrUpdateDeliveryGroup1 = addOrUpdateDeliveryGroup1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "addOrUpdateDeliveryGroupResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class addOrUpdateDeliveryGroupResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public addOrUpdateDeliveryGroupResponse()
        {
        }

        public addOrUpdateDeliveryGroupResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class updateMessageFolders
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "updateMessageFolders", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("messageFolders", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public messageFolderObject[]
            updateMessageFolders1;

        public updateMessageFolders()
        {
        }

        public updateMessageFolders(sessionHeader sessionHeader, messageFolderObject[] updateMessageFolders1)
        {
            this.sessionHeader = sessionHeader;
            this.updateMessageFolders1 = updateMessageFolders1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "updateMessageFoldersResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class updateMessageFoldersResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public updateMessageFoldersResponse()
        {
        }

        public updateMessageFoldersResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class addOrUpdateOrders
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "addOrUpdateOrders", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("orders", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public orderObject[] addOrUpdateOrders1;

        public addOrUpdateOrders()
        {
        }

        public addOrUpdateOrders(sessionHeader sessionHeader, orderObject[] addOrUpdateOrders1)
        {
            this.sessionHeader = sessionHeader;
            this.addOrUpdateOrders1 = addOrUpdateOrders1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "addOrUpdateOrdersResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class addOrUpdateOrdersResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public addOrUpdateOrdersResponse()
        {
        }

        public addOrUpdateOrdersResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class addOrUpdateContacts
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "addOrUpdateContacts", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("contacts", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public contactObject[] addOrUpdateContacts1;

        public addOrUpdateContacts()
        {
        }

        public addOrUpdateContacts(sessionHeader sessionHeader, contactObject[] addOrUpdateContacts1)
        {
            this.sessionHeader = sessionHeader;
            this.addOrUpdateContacts1 = addOrUpdateContacts1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "addOrUpdateContactsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class addOrUpdateContactsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public addOrUpdateContactsResponse()
        {
        }

        public addOrUpdateContactsResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class readDeliveries1
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public readDeliveries readDeliveries;

        public readDeliveries1()
        {
        }

        public readDeliveries1(sessionHeader sessionHeader, readDeliveries readDeliveries)
        {
            this.sessionHeader = sessionHeader;
            this.readDeliveries = readDeliveries;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "readDeliveriesResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class readDeliveriesResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement("return", Form = XmlSchemaForm.Unqualified)] public deliveryObject[] @return;

        public readDeliveriesResponse()
        {
        }

        public readDeliveriesResponse(deliveryObject[] @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class readSMSDeliveries1
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public readSMSDeliveries readSMSDeliveries;

        public readSMSDeliveries1()
        {
        }

        public readSMSDeliveries1(sessionHeader sessionHeader, readSMSDeliveries readSMSDeliveries)
        {
            this.sessionHeader = sessionHeader;
            this.readSMSDeliveries = readSMSDeliveries;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "readSMSDeliveriesResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class readSMSDeliveriesResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement("return", Form = XmlSchemaForm.Unqualified)] public smsDeliveryObject[] @return;

        public readSMSDeliveriesResponse()
        {
        }

        public readSMSDeliveriesResponse(smsDeliveryObject[] @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class updateLists
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "updateLists", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("lists", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public mailListObject[] updateLists1;

        public updateLists()
        {
        }

        public updateLists(sessionHeader sessionHeader, mailListObject[] updateLists1)
        {
            this.sessionHeader = sessionHeader;
            this.updateLists1 = updateLists1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "updateListsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class updateListsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public updateListsResponse()
        {
        }

        public updateListsResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class readContentTags1
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public readContentTags readContentTags;

        public readContentTags1()
        {
        }

        public readContentTags1(sessionHeader sessionHeader, readContentTags readContentTags)
        {
            this.sessionHeader = sessionHeader;
            this.readContentTags = readContentTags;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "readContentTagsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class readContentTagsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement("return", Form = XmlSchemaForm.Unqualified)] public contentTagObject[] @return;

        public readContentTagsResponse()
        {
        }

        public readContentTagsResponse(contentTagObject[] @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class addAccounts
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "addAccounts", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("accounts", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public accountObject[] addAccounts1;

        public addAccounts()
        {
        }

        public addAccounts(sessionHeader sessionHeader, accountObject[] addAccounts1)
        {
            this.sessionHeader = sessionHeader;
            this.addAccounts1 = addAccounts1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "addAccountsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class addAccountsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public addAccountsResponse()
        {
        }

        public addAccountsResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class deleteLists
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "deleteLists", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("lists", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public mailListObject[] deleteLists1;

        public deleteLists()
        {
        }

        public deleteLists(sessionHeader sessionHeader, mailListObject[] deleteLists1)
        {
            this.sessionHeader = sessionHeader;
            this.deleteLists1 = deleteLists1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "deleteListsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class deleteListsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public deleteListsResponse()
        {
        }

        public deleteListsResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class deleteContentTags
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "deleteContentTags", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("contentTags", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public contentTagObject[] deleteContentTags1;

        public deleteContentTags()
        {
        }

        public deleteContentTags(sessionHeader sessionHeader, contentTagObject[] deleteContentTags1)
        {
            this.sessionHeader = sessionHeader;
            this.deleteContentTags1 = deleteContentTags1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "deleteContentTagsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class deleteContentTagsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public deleteContentTagsResponse()
        {
        }

        public deleteContentTagsResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class removeFromSMSKeyword1
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public removeFromSMSKeyword removeFromSMSKeyword;

        public removeFromSMSKeyword1()
        {
        }

        public removeFromSMSKeyword1(sessionHeader sessionHeader, removeFromSMSKeyword removeFromSMSKeyword)
        {
            this.sessionHeader = sessionHeader;
            this.removeFromSMSKeyword = removeFromSMSKeyword;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "removeFromSMSKeywordResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class removeFromSMSKeywordResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public removeFromSMSKeywordResponse()
        {
        }

        public removeFromSMSKeywordResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class addMessages
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "addMessages", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("messages", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public messageObject[] addMessages1;

        public addMessages()
        {
        }

        public addMessages(sessionHeader sessionHeader, messageObject[] addMessages1)
        {
            this.sessionHeader = sessionHeader;
            this.addMessages1 = addMessages1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "addMessagesResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class addMessagesResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public addMessagesResponse()
        {
        }

        public addMessagesResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class readFields1
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public readFields readFields;

        public readFields1()
        {
        }

        public readFields1(sessionHeader sessionHeader, readFields readFields)
        {
            this.sessionHeader = sessionHeader;
            this.readFields = readFields;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "readFieldsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class readFieldsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement("return", Form = XmlSchemaForm.Unqualified)] public fieldObject[] @return;

        public readFieldsResponse()
        {
        }

        public readFieldsResponse(fieldObject[] @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class addHeaderFooters
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "addHeaderFooters", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("footers", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public headerFooterObject[] addHeaderFooters1;

        public addHeaderFooters()
        {
        }

        public addHeaderFooters(sessionHeader sessionHeader, headerFooterObject[] addHeaderFooters1)
        {
            this.sessionHeader = sessionHeader;
            this.addHeaderFooters1 = addHeaderFooters1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "addHeaderFootersResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class addHeaderFootersResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public addHeaderFootersResponse()
        {
        }

        public addHeaderFootersResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class updateFields
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "updateFields", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("fields", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public fieldObject[] updateFields1;

        public updateFields()
        {
        }

        public updateFields(sessionHeader sessionHeader, fieldObject[] updateFields1)
        {
            this.sessionHeader = sessionHeader;
            this.updateFields1 = updateFields1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "updateFieldsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class updateFieldsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public updateFieldsResponse()
        {
        }

        public updateFieldsResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class deleteFromDeliveryGroup1
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public deleteFromDeliveryGroup deleteFromDeliveryGroup;

        public deleteFromDeliveryGroup1()
        {
        }

        public deleteFromDeliveryGroup1(sessionHeader sessionHeader, deleteFromDeliveryGroup deleteFromDeliveryGroup)
        {
            this.sessionHeader = sessionHeader;
            this.deleteFromDeliveryGroup = deleteFromDeliveryGroup;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "deleteFromDeliveryGroupResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class deleteFromDeliveryGroupResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public deleteFromDeliveryGroupResponse()
        {
        }

        public deleteFromDeliveryGroupResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class clearLists
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "clearLists", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("list", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public mailListObject[] clearLists1;

        public clearLists()
        {
        }

        public clearLists(sessionHeader sessionHeader, mailListObject[] clearLists1)
        {
            this.sessionHeader = sessionHeader;
            this.clearLists1 = clearLists1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "clearListsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class clearListsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public clearListsResponse()
        {
        }

        public clearListsResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class addMessageRules
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "addMessageRules", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("messageRules", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public messageRuleObject[] addMessageRules1;

        public addMessageRules()
        {
        }

        public addMessageRules(sessionHeader sessionHeader, messageRuleObject[] addMessageRules1)
        {
            this.sessionHeader = sessionHeader;
            this.addMessageRules1 = addMessageRules1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "addMessageRulesResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class addMessageRulesResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public addMessageRulesResponse()
        {
        }

        public addMessageRulesResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class addMessageFolders
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "addMessageFolders", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("messageFolders", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public messageFolderObject[]
            addMessageFolders1;

        public addMessageFolders()
        {
        }

        public addMessageFolders(sessionHeader sessionHeader, messageFolderObject[] addMessageFolders1)
        {
            this.sessionHeader = sessionHeader;
            this.addMessageFolders1 = addMessageFolders1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "addMessageFoldersResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class addMessageFoldersResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public addMessageFoldersResponse()
        {
        }

        public addMessageFoldersResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class readMessages1
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public readMessages readMessages;

        public readMessages1()
        {
        }

        public readMessages1(sessionHeader sessionHeader, readMessages readMessages)
        {
            this.sessionHeader = sessionHeader;
            this.readMessages = readMessages;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "readMessagesResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class readMessagesResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement("return", Form = XmlSchemaForm.Unqualified)] public messageObject[] @return;

        public readMessagesResponse()
        {
        }

        public readMessagesResponse(messageObject[] @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class deleteAccounts
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "deleteAccounts", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("accounts", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public accountObject[] deleteAccounts1;

        public deleteAccounts()
        {
        }

        public deleteAccounts(sessionHeader sessionHeader, accountObject[] deleteAccounts1)
        {
            this.sessionHeader = sessionHeader;
            this.deleteAccounts1 = deleteAccounts1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "deleteAccountsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class deleteAccountsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public deleteAccountsResponse()
        {
        }

        public deleteAccountsResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class updateSMSMessages
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "updateSMSMessages", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("messages", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public smsMessageObject[] updateSMSMessages1;

        public updateSMSMessages()
        {
        }

        public updateSMSMessages(sessionHeader sessionHeader, smsMessageObject[] updateSMSMessages1)
        {
            this.sessionHeader = sessionHeader;
            this.updateSMSMessages1 = updateSMSMessages1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "updateSMSMessagesResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class updateSMSMessagesResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public updateSMSMessagesResponse()
        {
        }

        public updateSMSMessagesResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class readMessageRules1
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public readMessageRules readMessageRules;

        public readMessageRules1()
        {
        }

        public readMessageRules1(sessionHeader sessionHeader, readMessageRules readMessageRules)
        {
            this.sessionHeader = sessionHeader;
            this.readMessageRules = readMessageRules;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "readMessageRulesResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class readMessageRulesResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement("return", Form = XmlSchemaForm.Unqualified)] public messageRuleObject[] @return;

        public readMessageRulesResponse()
        {
        }

        public readMessageRulesResponse(messageRuleObject[] @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class addWorkflows
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "addWorkflows", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("workflows", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public workflowObject[] addWorkflows1;

        public addWorkflows()
        {
        }

        public addWorkflows(sessionHeader sessionHeader, workflowObject[] addWorkflows1)
        {
            this.sessionHeader = sessionHeader;
            this.addWorkflows1 = addWorkflows1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "addWorkflowsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class addWorkflowsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public addWorkflowsResponse()
        {
        }

        public addWorkflowsResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class deleteSMSKeywords
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "deleteSMSKeywords", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("keyword", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public smsKeywordObject[] deleteSMSKeywords1;

        public deleteSMSKeywords()
        {
        }

        public deleteSMSKeywords(sessionHeader sessionHeader, smsKeywordObject[] deleteSMSKeywords1)
        {
            this.sessionHeader = sessionHeader;
            this.deleteSMSKeywords1 = deleteSMSKeywords1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "deleteSMSKeywordsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class deleteSMSKeywordsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public deleteSMSKeywordsResponse()
        {
        }

        public deleteSMSKeywordsResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class updateWorkflows
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "updateWorkflows", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("workflows", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public workflowObject[] updateWorkflows1;

        public updateWorkflows()
        {
        }

        public updateWorkflows(sessionHeader sessionHeader, workflowObject[] updateWorkflows1)
        {
            this.sessionHeader = sessionHeader;
            this.updateWorkflows1 = updateWorkflows1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "updateWorkflowsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class updateWorkflowsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public updateWorkflowsResponse()
        {
        }

        public updateWorkflowsResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class addConversion
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "addConversion", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("conversions", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public conversionObject[] addConversion1;

        public addConversion()
        {
        }

        public addConversion(sessionHeader sessionHeader, conversionObject[] addConversion1)
        {
            this.sessionHeader = sessionHeader;
            this.addConversion1 = addConversion1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "addConversionResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class addConversionResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public addConversionResponse()
        {
        }

        public addConversionResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class updateAccounts
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "updateAccounts", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("accounts", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public accountObject[] updateAccounts1;

        public updateAccounts()
        {
        }

        public updateAccounts(sessionHeader sessionHeader, accountObject[] updateAccounts1)
        {
            this.sessionHeader = sessionHeader;
            this.updateAccounts1 = updateAccounts1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "updateAccountsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class updateAccountsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public updateAccountsResponse()
        {
        }

        public updateAccountsResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class readBounces1
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public readBounces readBounces;

        public readBounces1()
        {
        }

        public readBounces1(sessionHeader sessionHeader, readBounces readBounces)
        {
            this.sessionHeader = sessionHeader;
            this.readBounces = readBounces;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "readBouncesResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class readBouncesResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement("return", Form = XmlSchemaForm.Unqualified)] public bounceObject[] @return;

        public readBouncesResponse()
        {
        }

        public readBouncesResponse(bounceObject[] @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class updateHeaderFooters
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "updateHeaderFooters", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("footers", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public headerFooterObject[] updateHeaderFooters1;

        public updateHeaderFooters()
        {
        }

        public updateHeaderFooters(sessionHeader sessionHeader, headerFooterObject[] updateHeaderFooters1)
        {
            this.sessionHeader = sessionHeader;
            this.updateHeaderFooters1 = updateHeaderFooters1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "updateHeaderFootersResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class updateHeaderFootersResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public updateHeaderFootersResponse()
        {
        }

        public updateHeaderFootersResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class deleteMessageFolders
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "deleteMessageFolders", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("messageFolders", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public messageFolderObject[]
            deleteMessageFolders1;

        public deleteMessageFolders()
        {
        }

        public deleteMessageFolders(sessionHeader sessionHeader, messageFolderObject[] deleteMessageFolders1)
        {
            this.sessionHeader = sessionHeader;
            this.deleteMessageFolders1 = deleteMessageFolders1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "deleteMessageFoldersResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class deleteMessageFoldersResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public deleteMessageFoldersResponse()
        {
        }

        public deleteMessageFoldersResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class addLogins
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "addLogins", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("accounts", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public loginObject[] addLogins1;

        public addLogins()
        {
        }

        public addLogins(sessionHeader sessionHeader, loginObject[] addLogins1)
        {
            this.sessionHeader = sessionHeader;
            this.addLogins1 = addLogins1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "addLoginsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class addLoginsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public addLoginsResponse()
        {
        }

        public addLoginsResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class updateContacts
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "updateContacts", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("contacts", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public contactObject[] updateContacts1;

        public updateContacts()
        {
        }

        public updateContacts(sessionHeader sessionHeader, contactObject[] updateContacts1)
        {
            this.sessionHeader = sessionHeader;
            this.updateContacts1 = updateContacts1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "updateContactsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class updateContactsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public updateContactsResponse()
        {
        }

        public updateContactsResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class readDeliveryGroups1
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public readDeliveryGroups readDeliveryGroups;

        public readDeliveryGroups1()
        {
        }

        public readDeliveryGroups1(sessionHeader sessionHeader, readDeliveryGroups readDeliveryGroups)
        {
            this.sessionHeader = sessionHeader;
            this.readDeliveryGroups = readDeliveryGroups;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "readDeliveryGroupsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class readDeliveryGroupsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement("return", Form = XmlSchemaForm.Unqualified)] public deliveryGroupObject[] @return;

        public readDeliveryGroupsResponse()
        {
        }

        public readDeliveryGroupsResponse(deliveryGroupObject[] @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class addToDeliveryGroup1
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public addToDeliveryGroup addToDeliveryGroup;

        public addToDeliveryGroup1()
        {
        }

        public addToDeliveryGroup1(sessionHeader sessionHeader, addToDeliveryGroup addToDeliveryGroup)
        {
            this.sessionHeader = sessionHeader;
            this.addToDeliveryGroup = addToDeliveryGroup;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "addToDeliveryGroupResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class addToDeliveryGroupResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public addToDeliveryGroupResponse()
        {
        }

        public addToDeliveryGroupResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class addSMSDeliveries
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "addSMSDeliveries", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("smsdeliveries", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public smsDeliveryObject[] addSMSDeliveries1;

        public addSMSDeliveries()
        {
        }

        public addSMSDeliveries(sessionHeader sessionHeader, smsDeliveryObject[] addSMSDeliveries1)
        {
            this.sessionHeader = sessionHeader;
            this.addSMSDeliveries1 = addSMSDeliveries1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "addSMSDeliveriesResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class addSMSDeliveriesResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public addSMSDeliveriesResponse()
        {
        }

        public addSMSDeliveriesResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class deleteSMSDeliveries
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "deleteSMSDeliveries", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("smsdeliveries", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public smsDeliveryObject[]
            deleteSMSDeliveries1;

        public deleteSMSDeliveries()
        {
        }

        public deleteSMSDeliveries(sessionHeader sessionHeader, smsDeliveryObject[] deleteSMSDeliveries1)
        {
            this.sessionHeader = sessionHeader;
            this.deleteSMSDeliveries1 = deleteSMSDeliveries1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "deleteSMSDeliveriesResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class deleteSMSDeliveriesResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public deleteSMSDeliveriesResponse()
        {
        }

        public deleteSMSDeliveriesResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class deleteFields
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "deleteFields", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("fields", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public fieldObject[] deleteFields1;

        public deleteFields()
        {
        }

        public deleteFields(sessionHeader sessionHeader, fieldObject[] deleteFields1)
        {
            this.sessionHeader = sessionHeader;
            this.deleteFields1 = deleteFields1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "deleteFieldsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class deleteFieldsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public deleteFieldsResponse()
        {
        }

        public deleteFieldsResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class readSMSMessages1
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public readSMSMessages readSMSMessages;

        public readSMSMessages1()
        {
        }

        public readSMSMessages1(sessionHeader sessionHeader, readSMSMessages readSMSMessages)
        {
            this.sessionHeader = sessionHeader;
            this.readSMSMessages = readSMSMessages;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "readSMSMessagesResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class readSMSMessagesResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement("return", Form = XmlSchemaForm.Unqualified)] public smsMessageObject[] @return;

        public readSMSMessagesResponse()
        {
        }

        public readSMSMessagesResponse(smsMessageObject[] @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class addApiTokens
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "addApiTokens", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("tokens", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public apiTokenObject[] addApiTokens1;

        public addApiTokens()
        {
        }

        public addApiTokens(sessionHeader sessionHeader, apiTokenObject[] addApiTokens1)
        {
            this.sessionHeader = sessionHeader;
            this.addApiTokens1 = addApiTokens1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "addApiTokensResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class addApiTokensResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public addApiTokensResponse()
        {
        }

        public addApiTokensResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class updateLogins
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Name = "updateLogins", Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlArrayItem("accounts", Form = XmlSchemaForm.Unqualified, IsNullable = false)] public loginObject[] updateLogins1;

        public updateLogins()
        {
        }

        public updateLogins(sessionHeader sessionHeader, loginObject[] updateLogins1)
        {
            this.sessionHeader = sessionHeader;
            this.updateLogins1 = updateLogins1;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "updateLoginsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class updateLoginsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement(Form = XmlSchemaForm.Unqualified)] public
            writeResult @return;

        public updateLoginsResponse()
        {
        }

        public updateLoginsResponse(writeResult @return)
        {
            this.@return = @return;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(IsWrapped = false)]
    public partial class readWebforms1
    {

        [MessageHeader(Namespace = "http://api.bronto.com/v4")] [XmlElement(IsNullable = true)] public sessionHeader sessionHeader;

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] public readWebforms readWebforms;

        public readWebforms1()
        {
        }

        public readWebforms1(sessionHeader sessionHeader, readWebforms readWebforms)
        {
            this.sessionHeader = sessionHeader;
            this.readWebforms = readWebforms;
        }
    }

    [DebuggerStepThrough]

    [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
    [MessageContract(WrapperName = "readWebformsResponse", WrapperNamespace = "http://api.bronto.com/v4", IsWrapped = true)]
    public partial class readWebformsResponse
    {

        [MessageBodyMember(Namespace = "http://api.bronto.com/v4", Order = 0)] [XmlElement("return", Form = XmlSchemaForm.Unqualified)] public webformObject[] @return;

        public readWebformsResponse()
        {
        }

        public readWebformsResponse(webformObject[] @return)
        {
            this.@return = @return;
        }
    }


    public interface BrontoSoapPortTypeChannel : BrontoSoapPortType, IClientChannel
    {
    }

    [DebuggerStepThrough]

    public partial class BrontoSoapPortTypeClient : ClientBase<BrontoSoapPortType>, BrontoSoapPortType
    {

        public BrontoSoapPortTypeClient()
        {
        }

        public BrontoSoapPortTypeClient(string endpointConfigurationName) :
            base(endpointConfigurationName)
        {
        }

        public BrontoSoapPortTypeClient(string endpointConfigurationName, string remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public BrontoSoapPortTypeClient(string endpointConfigurationName, EndpointAddress remoteAddress) :
            base(endpointConfigurationName, remoteAddress)
        {
        }

        public BrontoSoapPortTypeClient(Binding binding, EndpointAddress remoteAddress) :
            base(binding, remoteAddress)
        {
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        readLoginsResponse BrontoSoapPortType.readLogins(readLogins1 request)
        {
            return base.Channel.readLogins(request);
        }

        public loginObject[] readLogins(sessionHeader sessionHeader, readLogins readLogins1)
        {
            readLogins1 inValue = new readLogins1();
            inValue.sessionHeader = sessionHeader;
            inValue.readLogins = readLogins1;
            readLoginsResponse retVal = ((BrontoSoapPortType) (this)).readLogins(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<readLoginsResponse> BrontoSoapPortType.readLoginsAsync(readLogins1 request)
        {
            return base.Channel.readLoginsAsync(request);
        }

        public Task<readLoginsResponse> readLoginsAsync(sessionHeader sessionHeader, readLogins readLogins)
        {
            readLogins1 inValue = new readLogins1();
            inValue.sessionHeader = sessionHeader;
            inValue.readLogins = readLogins;
            return ((BrontoSoapPortType) (this)).readLoginsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        deleteLoginsResponse BrontoSoapPortType.deleteLogins(deleteLogins request)
        {
            return base.Channel.deleteLogins(request);
        }

        public writeResult deleteLogins(sessionHeader sessionHeader, loginObject[] deleteLogins1)
        {
            deleteLogins inValue = new deleteLogins();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteLogins1 = deleteLogins1;
            deleteLoginsResponse retVal = ((BrontoSoapPortType) (this)).deleteLogins(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<deleteLoginsResponse> BrontoSoapPortType.deleteLoginsAsync(deleteLogins request)
        {
            return base.Channel.deleteLoginsAsync(request);
        }

        public Task<deleteLoginsResponse> deleteLoginsAsync(sessionHeader sessionHeader, loginObject[] deleteLogins1)
        {
            deleteLogins inValue = new deleteLogins();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteLogins1 = deleteLogins1;
            return ((BrontoSoapPortType) (this)).deleteLoginsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        deleteDeliveryGroupResponse BrontoSoapPortType.deleteDeliveryGroup(deleteDeliveryGroup request)
        {
            return base.Channel.deleteDeliveryGroup(request);
        }

        public writeResult deleteDeliveryGroup(sessionHeader sessionHeader, deliveryGroupObject[] deleteDeliveryGroup1)
        {
            deleteDeliveryGroup inValue = new deleteDeliveryGroup();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteDeliveryGroup1 = deleteDeliveryGroup1;
            deleteDeliveryGroupResponse retVal = ((BrontoSoapPortType) (this)).deleteDeliveryGroup(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<deleteDeliveryGroupResponse> BrontoSoapPortType.deleteDeliveryGroupAsync(deleteDeliveryGroup request)
        {
            return base.Channel.deleteDeliveryGroupAsync(request);
        }

        public Task<deleteDeliveryGroupResponse> deleteDeliveryGroupAsync(sessionHeader sessionHeader,
            deliveryGroupObject[] deleteDeliveryGroup1)
        {
            deleteDeliveryGroup inValue = new deleteDeliveryGroup();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteDeliveryGroup1 = deleteDeliveryGroup1;
            return ((BrontoSoapPortType) (this)).deleteDeliveryGroupAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        addContactsToWorkflowResponse BrontoSoapPortType.addContactsToWorkflow(addContactsToWorkflow1 request)
        {
            return base.Channel.addContactsToWorkflow(request);
        }

        public writeResult addContactsToWorkflow(sessionHeader sessionHeader, addContactsToWorkflow addContactsToWorkflow1)
        {
            addContactsToWorkflow1 inValue = new addContactsToWorkflow1();
            inValue.sessionHeader = sessionHeader;
            inValue.addContactsToWorkflow = addContactsToWorkflow1;
            addContactsToWorkflowResponse retVal = ((BrontoSoapPortType) (this)).addContactsToWorkflow(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<addContactsToWorkflowResponse> BrontoSoapPortType.addContactsToWorkflowAsync(addContactsToWorkflow1 request)
        {
            return base.Channel.addContactsToWorkflowAsync(request);
        }

        public Task<addContactsToWorkflowResponse> addContactsToWorkflowAsync(sessionHeader sessionHeader,
            addContactsToWorkflow addContactsToWorkflow)
        {
            addContactsToWorkflow1 inValue = new addContactsToWorkflow1();
            inValue.sessionHeader = sessionHeader;
            inValue.addContactsToWorkflow = addContactsToWorkflow;
            return ((BrontoSoapPortType) (this)).addContactsToWorkflowAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        readApiTokensResponse BrontoSoapPortType.readApiTokens(readApiTokens1 request)
        {
            return base.Channel.readApiTokens(request);
        }

        public apiTokenObject[] readApiTokens(sessionHeader sessionHeader, readApiTokens readApiTokens1)
        {
            readApiTokens1 inValue = new readApiTokens1();
            inValue.sessionHeader = sessionHeader;
            inValue.readApiTokens = readApiTokens1;
            readApiTokensResponse retVal = ((BrontoSoapPortType) (this)).readApiTokens(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<readApiTokensResponse> BrontoSoapPortType.readApiTokensAsync(readApiTokens1 request)
        {
            return base.Channel.readApiTokensAsync(request);
        }

        public Task<readApiTokensResponse> readApiTokensAsync(sessionHeader sessionHeader, readApiTokens readApiTokens)
        {
            readApiTokens1 inValue = new readApiTokens1();
            inValue.sessionHeader = sessionHeader;
            inValue.readApiTokens = readApiTokens;
            return ((BrontoSoapPortType) (this)).readApiTokensAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        updateMessageRulesResponse BrontoSoapPortType.updateMessageRules(updateMessageRules request)
        {
            return base.Channel.updateMessageRules(request);
        }

        public writeResult updateMessageRules(sessionHeader sessionHeader, messageRuleObject[] updateMessageRules1)
        {
            updateMessageRules inValue = new updateMessageRules();
            inValue.sessionHeader = sessionHeader;
            inValue.updateMessageRules1 = updateMessageRules1;
            updateMessageRulesResponse retVal = ((BrontoSoapPortType) (this)).updateMessageRules(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<updateMessageRulesResponse> BrontoSoapPortType.updateMessageRulesAsync(updateMessageRules request)
        {
            return base.Channel.updateMessageRulesAsync(request);
        }

        public Task<updateMessageRulesResponse> updateMessageRulesAsync(sessionHeader sessionHeader, messageRuleObject[] updateMessageRules1)
        {
            updateMessageRules inValue = new updateMessageRules();
            inValue.sessionHeader = sessionHeader;
            inValue.updateMessageRules1 = updateMessageRules1;
            return ((BrontoSoapPortType) (this)).updateMessageRulesAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        deleteMessageRulesResponse BrontoSoapPortType.deleteMessageRules(deleteMessageRules request)
        {
            return base.Channel.deleteMessageRules(request);
        }

        public writeResult deleteMessageRules(sessionHeader sessionHeader, messageRuleObject[] deleteMessageRules1)
        {
            deleteMessageRules inValue = new deleteMessageRules();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteMessageRules1 = deleteMessageRules1;
            deleteMessageRulesResponse retVal = ((BrontoSoapPortType) (this)).deleteMessageRules(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<deleteMessageRulesResponse> BrontoSoapPortType.deleteMessageRulesAsync(deleteMessageRules request)
        {
            return base.Channel.deleteMessageRulesAsync(request);
        }

        public Task<deleteMessageRulesResponse> deleteMessageRulesAsync(sessionHeader sessionHeader, messageRuleObject[] deleteMessageRules1)
        {
            deleteMessageRules inValue = new deleteMessageRules();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteMessageRules1 = deleteMessageRules1;
            return ((BrontoSoapPortType) (this)).deleteMessageRulesAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        readListsResponse BrontoSoapPortType.readLists(readLists1 request)
        {
            return base.Channel.readLists(request);
        }

        public mailListObject[] readLists(sessionHeader sessionHeader, readLists readLists1)
        {
            readLists1 inValue = new readLists1();
            inValue.sessionHeader = sessionHeader;
            inValue.readLists = readLists1;
            readListsResponse retVal = ((BrontoSoapPortType) (this)).readLists(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<readListsResponse> BrontoSoapPortType.readListsAsync(readLists1 request)
        {
            return base.Channel.readListsAsync(request);
        }

        public Task<readListsResponse> readListsAsync(sessionHeader sessionHeader, readLists readLists)
        {
            readLists1 inValue = new readLists1();
            inValue.sessionHeader = sessionHeader;
            inValue.readLists = readLists;
            return ((BrontoSoapPortType) (this)).readListsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        deleteMessagesResponse BrontoSoapPortType.deleteMessages(deleteMessages request)
        {
            return base.Channel.deleteMessages(request);
        }

        public writeResult deleteMessages(sessionHeader sessionHeader, messageObject[] deleteMessages1)
        {
            deleteMessages inValue = new deleteMessages();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteMessages1 = deleteMessages1;
            deleteMessagesResponse retVal = ((BrontoSoapPortType) (this)).deleteMessages(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<deleteMessagesResponse> BrontoSoapPortType.deleteMessagesAsync(deleteMessages request)
        {
            return base.Channel.deleteMessagesAsync(request);
        }

        public Task<deleteMessagesResponse> deleteMessagesAsync(sessionHeader sessionHeader, messageObject[] deleteMessages1)
        {
            deleteMessages inValue = new deleteMessages();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteMessages1 = deleteMessages1;
            return ((BrontoSoapPortType) (this)).deleteMessagesAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        updateSMSDeliveriesResponse BrontoSoapPortType.updateSMSDeliveries(updateSMSDeliveries request)
        {
            return base.Channel.updateSMSDeliveries(request);
        }

        public writeResult updateSMSDeliveries(sessionHeader sessionHeader, smsDeliveryObject[] updateSMSDeliveries1)
        {
            updateSMSDeliveries inValue = new updateSMSDeliveries();
            inValue.sessionHeader = sessionHeader;
            inValue.updateSMSDeliveries1 = updateSMSDeliveries1;
            updateSMSDeliveriesResponse retVal = ((BrontoSoapPortType) (this)).updateSMSDeliveries(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<updateSMSDeliveriesResponse> BrontoSoapPortType.updateSMSDeliveriesAsync(updateSMSDeliveries request)
        {
            return base.Channel.updateSMSDeliveriesAsync(request);
        }

        public Task<updateSMSDeliveriesResponse> updateSMSDeliveriesAsync(sessionHeader sessionHeader,
            smsDeliveryObject[] updateSMSDeliveries1)
        {
            updateSMSDeliveries inValue = new updateSMSDeliveries();
            inValue.sessionHeader = sessionHeader;
            inValue.updateSMSDeliveries1 = updateSMSDeliveries1;
            return ((BrontoSoapPortType) (this)).updateSMSDeliveriesAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        readMessageFoldersResponse BrontoSoapPortType.readMessageFolders(readMessageFolders1 request)
        {
            return base.Channel.readMessageFolders(request);
        }

        public messageFolderObject[] readMessageFolders(sessionHeader sessionHeader, readMessageFolders readMessageFolders1)
        {
            readMessageFolders1 inValue = new readMessageFolders1();
            inValue.sessionHeader = sessionHeader;
            inValue.readMessageFolders = readMessageFolders1;
            readMessageFoldersResponse retVal = ((BrontoSoapPortType) (this)).readMessageFolders(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<readMessageFoldersResponse> BrontoSoapPortType.readMessageFoldersAsync(readMessageFolders1 request)
        {
            return base.Channel.readMessageFoldersAsync(request);
        }

        public Task<readMessageFoldersResponse> readMessageFoldersAsync(sessionHeader sessionHeader, readMessageFolders readMessageFolders)
        {
            readMessageFolders1 inValue = new readMessageFolders1();
            inValue.sessionHeader = sessionHeader;
            inValue.readMessageFolders = readMessageFolders;
            return ((BrontoSoapPortType) (this)).readMessageFoldersAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        addUpdateOrderResponse BrontoSoapPortType.addUpdateOrder(addUpdateOrder request)
        {
            return base.Channel.addUpdateOrder(request);
        }

        public writeResult addUpdateOrder(sessionHeader sessionHeader, orderObject[] addUpdateOrder1)
        {
            addUpdateOrder inValue = new addUpdateOrder();
            inValue.sessionHeader = sessionHeader;
            inValue.addUpdateOrder1 = addUpdateOrder1;
            addUpdateOrderResponse retVal = ((BrontoSoapPortType) (this)).addUpdateOrder(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<addUpdateOrderResponse> BrontoSoapPortType.addUpdateOrderAsync(addUpdateOrder request)
        {
            return base.Channel.addUpdateOrderAsync(request);
        }

        public Task<addUpdateOrderResponse> addUpdateOrderAsync(sessionHeader sessionHeader, orderObject[] addUpdateOrder1)
        {
            addUpdateOrder inValue = new addUpdateOrder();
            inValue.sessionHeader = sessionHeader;
            inValue.addUpdateOrder1 = addUpdateOrder1;
            return ((BrontoSoapPortType) (this)).addUpdateOrderAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        updateDeliveryGroupResponse BrontoSoapPortType.updateDeliveryGroup(updateDeliveryGroup request)
        {
            return base.Channel.updateDeliveryGroup(request);
        }

        public writeResult updateDeliveryGroup(sessionHeader sessionHeader, deliveryGroupObject[] updateDeliveryGroup1)
        {
            updateDeliveryGroup inValue = new updateDeliveryGroup();
            inValue.sessionHeader = sessionHeader;
            inValue.updateDeliveryGroup1 = updateDeliveryGroup1;
            updateDeliveryGroupResponse retVal = ((BrontoSoapPortType) (this)).updateDeliveryGroup(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<updateDeliveryGroupResponse> BrontoSoapPortType.updateDeliveryGroupAsync(updateDeliveryGroup request)
        {
            return base.Channel.updateDeliveryGroupAsync(request);
        }

        public Task<updateDeliveryGroupResponse> updateDeliveryGroupAsync(sessionHeader sessionHeader,
            deliveryGroupObject[] updateDeliveryGroup1)
        {
            updateDeliveryGroup inValue = new updateDeliveryGroup();
            inValue.sessionHeader = sessionHeader;
            inValue.updateDeliveryGroup1 = updateDeliveryGroup1;
            return ((BrontoSoapPortType) (this)).updateDeliveryGroupAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        readHeaderFootersResponse BrontoSoapPortType.readHeaderFooters(readHeaderFooters1 request)
        {
            return base.Channel.readHeaderFooters(request);
        }

        public headerFooterObject[] readHeaderFooters(sessionHeader sessionHeader, readHeaderFooters readHeaderFooters1)
        {
            readHeaderFooters1 inValue = new readHeaderFooters1();
            inValue.sessionHeader = sessionHeader;
            inValue.readHeaderFooters = readHeaderFooters1;
            readHeaderFootersResponse retVal = ((BrontoSoapPortType) (this)).readHeaderFooters(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<readHeaderFootersResponse> BrontoSoapPortType.readHeaderFootersAsync(readHeaderFooters1 request)
        {
            return base.Channel.readHeaderFootersAsync(request);
        }

        public Task<readHeaderFootersResponse> readHeaderFootersAsync(sessionHeader sessionHeader, readHeaderFooters readHeaderFooters)
        {
            readHeaderFooters1 inValue = new readHeaderFooters1();
            inValue.sessionHeader = sessionHeader;
            inValue.readHeaderFooters = readHeaderFooters;
            return ((BrontoSoapPortType) (this)).readHeaderFootersAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        deleteApiTokensResponse BrontoSoapPortType.deleteApiTokens(deleteApiTokens request)
        {
            return base.Channel.deleteApiTokens(request);
        }

        public writeResult deleteApiTokens(sessionHeader sessionHeader, apiTokenObject[] deleteApiTokens1)
        {
            deleteApiTokens inValue = new deleteApiTokens();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteApiTokens1 = deleteApiTokens1;
            deleteApiTokensResponse retVal = ((BrontoSoapPortType) (this)).deleteApiTokens(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<deleteApiTokensResponse> BrontoSoapPortType.deleteApiTokensAsync(deleteApiTokens request)
        {
            return base.Channel.deleteApiTokensAsync(request);
        }

        public Task<deleteApiTokensResponse> deleteApiTokensAsync(sessionHeader sessionHeader, apiTokenObject[] deleteApiTokens1)
        {
            deleteApiTokens inValue = new deleteApiTokens();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteApiTokens1 = deleteApiTokens1;
            return ((BrontoSoapPortType) (this)).deleteApiTokensAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        addFieldsResponse BrontoSoapPortType.addFields(addFields request)
        {
            return base.Channel.addFields(request);
        }

        public writeResult addFields(sessionHeader sessionHeader, fieldObject[] addFields1)
        {
            addFields inValue = new addFields();
            inValue.sessionHeader = sessionHeader;
            inValue.addFields1 = addFields1;
            addFieldsResponse retVal = ((BrontoSoapPortType) (this)).addFields(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<addFieldsResponse> BrontoSoapPortType.addFieldsAsync(addFields request)
        {
            return base.Channel.addFieldsAsync(request);
        }

        public Task<addFieldsResponse> addFieldsAsync(sessionHeader sessionHeader, fieldObject[] addFields1)
        {
            addFields inValue = new addFields();
            inValue.sessionHeader = sessionHeader;
            inValue.addFields1 = addFields1;
            return ((BrontoSoapPortType) (this)).addFieldsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        deleteHeaderFootersResponse BrontoSoapPortType.deleteHeaderFooters(deleteHeaderFooters request)
        {
            return base.Channel.deleteHeaderFooters(request);
        }

        public writeResult deleteHeaderFooters(sessionHeader sessionHeader, headerFooterObject[] deleteHeaderFooters1)
        {
            deleteHeaderFooters inValue = new deleteHeaderFooters();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteHeaderFooters1 = deleteHeaderFooters1;
            deleteHeaderFootersResponse retVal = ((BrontoSoapPortType) (this)).deleteHeaderFooters(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<deleteHeaderFootersResponse> BrontoSoapPortType.deleteHeaderFootersAsync(deleteHeaderFooters request)
        {
            return base.Channel.deleteHeaderFootersAsync(request);
        }

        public Task<deleteHeaderFootersResponse> deleteHeaderFootersAsync(sessionHeader sessionHeader,
            headerFooterObject[] deleteHeaderFooters1)
        {
            deleteHeaderFooters inValue = new deleteHeaderFooters();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteHeaderFooters1 = deleteHeaderFooters1;
            return ((BrontoSoapPortType) (this)).deleteHeaderFootersAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        deleteWorkflowsResponse BrontoSoapPortType.deleteWorkflows(deleteWorkflows request)
        {
            return base.Channel.deleteWorkflows(request);
        }

        public writeResult deleteWorkflows(sessionHeader sessionHeader, workflowObject[] deleteWorkflows1)
        {
            deleteWorkflows inValue = new deleteWorkflows();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteWorkflows1 = deleteWorkflows1;
            deleteWorkflowsResponse retVal = ((BrontoSoapPortType) (this)).deleteWorkflows(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<deleteWorkflowsResponse> BrontoSoapPortType.deleteWorkflowsAsync(deleteWorkflows request)
        {
            return base.Channel.deleteWorkflowsAsync(request);
        }

        public Task<deleteWorkflowsResponse> deleteWorkflowsAsync(sessionHeader sessionHeader, workflowObject[] deleteWorkflows1)
        {
            deleteWorkflows inValue = new deleteWorkflows();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteWorkflows1 = deleteWorkflows1;
            return ((BrontoSoapPortType) (this)).deleteWorkflowsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        addToListResponse BrontoSoapPortType.addToList(addToList1 request)
        {
            return base.Channel.addToList(request);
        }

        public writeResult addToList(sessionHeader sessionHeader, addToList addToList1)
        {
            addToList1 inValue = new addToList1();
            inValue.sessionHeader = sessionHeader;
            inValue.addToList = addToList1;
            addToListResponse retVal = ((BrontoSoapPortType) (this)).addToList(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<addToListResponse> BrontoSoapPortType.addToListAsync(addToList1 request)
        {
            return base.Channel.addToListAsync(request);
        }

        public Task<addToListResponse> addToListAsync(sessionHeader sessionHeader, addToList addToList)
        {
            addToList1 inValue = new addToList1();
            inValue.sessionHeader = sessionHeader;
            inValue.addToList = addToList;
            return ((BrontoSoapPortType) (this)).addToListAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        updateContentTagsResponse BrontoSoapPortType.updateContentTags(updateContentTags request)
        {
            return base.Channel.updateContentTags(request);
        }

        public writeResult updateContentTags(sessionHeader sessionHeader, contentTagObject[] updateContentTags1)
        {
            updateContentTags inValue = new updateContentTags();
            inValue.sessionHeader = sessionHeader;
            inValue.updateContentTags1 = updateContentTags1;
            updateContentTagsResponse retVal = ((BrontoSoapPortType) (this)).updateContentTags(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<updateContentTagsResponse> BrontoSoapPortType.updateContentTagsAsync(updateContentTags request)
        {
            return base.Channel.updateContentTagsAsync(request);
        }

        public Task<updateContentTagsResponse> updateContentTagsAsync(sessionHeader sessionHeader, contentTagObject[] updateContentTags1)
        {
            updateContentTags inValue = new updateContentTags();
            inValue.sessionHeader = sessionHeader;
            inValue.updateContentTags1 = updateContentTags1;
            return ((BrontoSoapPortType) (this)).updateContentTagsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        readActivitiesResponse BrontoSoapPortType.readActivities(readActivities1 request)
        {
            return base.Channel.readActivities(request);
        }

        public activityObject[] readActivities(sessionHeader sessionHeader, readActivities readActivities1)
        {
            readActivities1 inValue = new readActivities1();
            inValue.sessionHeader = sessionHeader;
            inValue.readActivities = readActivities1;
            readActivitiesResponse retVal = ((BrontoSoapPortType) (this)).readActivities(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<readActivitiesResponse> BrontoSoapPortType.readActivitiesAsync(readActivities1 request)
        {
            return base.Channel.readActivitiesAsync(request);
        }

        public Task<readActivitiesResponse> readActivitiesAsync(sessionHeader sessionHeader, readActivities readActivities)
        {
            readActivities1 inValue = new readActivities1();
            inValue.sessionHeader = sessionHeader;
            inValue.readActivities = readActivities;
            return ((BrontoSoapPortType) (this)).readActivitiesAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        addSMSMessagesResponse BrontoSoapPortType.addSMSMessages(addSMSMessages request)
        {
            return base.Channel.addSMSMessages(request);
        }

        public writeResult addSMSMessages(sessionHeader sessionHeader, smsMessageObject[] addSMSMessages1)
        {
            addSMSMessages inValue = new addSMSMessages();
            inValue.sessionHeader = sessionHeader;
            inValue.addSMSMessages1 = addSMSMessages1;
            addSMSMessagesResponse retVal = ((BrontoSoapPortType) (this)).addSMSMessages(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<addSMSMessagesResponse> BrontoSoapPortType.addSMSMessagesAsync(addSMSMessages request)
        {
            return base.Channel.addSMSMessagesAsync(request);
        }

        public Task<addSMSMessagesResponse> addSMSMessagesAsync(sessionHeader sessionHeader, smsMessageObject[] addSMSMessages1)
        {
            addSMSMessages inValue = new addSMSMessages();
            inValue.sessionHeader = sessionHeader;
            inValue.addSMSMessages1 = addSMSMessages1;
            return ((BrontoSoapPortType) (this)).addSMSMessagesAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        readConversionsResponse BrontoSoapPortType.readConversions(readConversions1 request)
        {
            return base.Channel.readConversions(request);
        }

        public conversionObject[] readConversions(sessionHeader sessionHeader, readConversions readConversions1)
        {
            readConversions1 inValue = new readConversions1();
            inValue.sessionHeader = sessionHeader;
            inValue.readConversions = readConversions1;
            readConversionsResponse retVal = ((BrontoSoapPortType) (this)).readConversions(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<readConversionsResponse> BrontoSoapPortType.readConversionsAsync(readConversions1 request)
        {
            return base.Channel.readConversionsAsync(request);
        }

        public Task<readConversionsResponse> readConversionsAsync(sessionHeader sessionHeader, readConversions readConversions)
        {
            readConversions1 inValue = new readConversions1();
            inValue.sessionHeader = sessionHeader;
            inValue.readConversions = readConversions;
            return ((BrontoSoapPortType) (this)).readConversionsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        deleteContactsResponse BrontoSoapPortType.deleteContacts(deleteContacts request)
        {
            return base.Channel.deleteContacts(request);
        }

        public writeResult deleteContacts(sessionHeader sessionHeader, contactObject[] deleteContacts1)
        {
            deleteContacts inValue = new deleteContacts();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteContacts1 = deleteContacts1;
            deleteContactsResponse retVal = ((BrontoSoapPortType) (this)).deleteContacts(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<deleteContactsResponse> BrontoSoapPortType.deleteContactsAsync(deleteContacts request)
        {
            return base.Channel.deleteContactsAsync(request);
        }

        public Task<deleteContactsResponse> deleteContactsAsync(sessionHeader sessionHeader, contactObject[] deleteContacts1)
        {
            deleteContacts inValue = new deleteContacts();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteContacts1 = deleteContacts1;
            return ((BrontoSoapPortType) (this)).deleteContactsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        addDeliveryGroupResponse BrontoSoapPortType.addDeliveryGroup(addDeliveryGroup request)
        {
            return base.Channel.addDeliveryGroup(request);
        }

        public writeResult addDeliveryGroup(sessionHeader sessionHeader, deliveryGroupObject[] addDeliveryGroup1)
        {
            addDeliveryGroup inValue = new addDeliveryGroup();
            inValue.sessionHeader = sessionHeader;
            inValue.addDeliveryGroup1 = addDeliveryGroup1;
            addDeliveryGroupResponse retVal = ((BrontoSoapPortType) (this)).addDeliveryGroup(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<addDeliveryGroupResponse> BrontoSoapPortType.addDeliveryGroupAsync(addDeliveryGroup request)
        {
            return base.Channel.addDeliveryGroupAsync(request);
        }

        public Task<addDeliveryGroupResponse> addDeliveryGroupAsync(sessionHeader sessionHeader, deliveryGroupObject[] addDeliveryGroup1)
        {
            addDeliveryGroup inValue = new addDeliveryGroup();
            inValue.sessionHeader = sessionHeader;
            inValue.addDeliveryGroup1 = addDeliveryGroup1;
            return ((BrontoSoapPortType) (this)).addDeliveryGroupAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        updateSMSKeywordsResponse BrontoSoapPortType.updateSMSKeywords(updateSMSKeywords request)
        {
            return base.Channel.updateSMSKeywords(request);
        }

        public writeResult updateSMSKeywords(sessionHeader sessionHeader, smsKeywordObject[] updateSMSKeywords1)
        {
            updateSMSKeywords inValue = new updateSMSKeywords();
            inValue.sessionHeader = sessionHeader;
            inValue.updateSMSKeywords1 = updateSMSKeywords1;
            updateSMSKeywordsResponse retVal = ((BrontoSoapPortType) (this)).updateSMSKeywords(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<updateSMSKeywordsResponse> BrontoSoapPortType.updateSMSKeywordsAsync(updateSMSKeywords request)
        {
            return base.Channel.updateSMSKeywordsAsync(request);
        }

        public Task<updateSMSKeywordsResponse> updateSMSKeywordsAsync(sessionHeader sessionHeader, smsKeywordObject[] updateSMSKeywords1)
        {
            updateSMSKeywords inValue = new updateSMSKeywords();
            inValue.sessionHeader = sessionHeader;
            inValue.updateSMSKeywords1 = updateSMSKeywords1;
            return ((BrontoSoapPortType) (this)).updateSMSKeywordsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        updateMessagesResponse BrontoSoapPortType.updateMessages(updateMessages request)
        {
            return base.Channel.updateMessages(request);
        }

        public writeResult updateMessages(sessionHeader sessionHeader, messageObject[] updateMessages1)
        {
            updateMessages inValue = new updateMessages();
            inValue.sessionHeader = sessionHeader;
            inValue.updateMessages1 = updateMessages1;
            updateMessagesResponse retVal = ((BrontoSoapPortType) (this)).updateMessages(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<updateMessagesResponse> BrontoSoapPortType.updateMessagesAsync(updateMessages request)
        {
            return base.Channel.updateMessagesAsync(request);
        }

        public Task<updateMessagesResponse> updateMessagesAsync(sessionHeader sessionHeader, messageObject[] updateMessages1)
        {
            updateMessages inValue = new updateMessages();
            inValue.sessionHeader = sessionHeader;
            inValue.updateMessages1 = updateMessages1;
            return ((BrontoSoapPortType) (this)).updateMessagesAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        readUnsubscribesResponse BrontoSoapPortType.readUnsubscribes(readUnsubscribes1 request)
        {
            return base.Channel.readUnsubscribes(request);
        }

        public unsubscribeObject[] readUnsubscribes(sessionHeader sessionHeader, readUnsubscribes readUnsubscribes1)
        {
            readUnsubscribes1 inValue = new readUnsubscribes1();
            inValue.sessionHeader = sessionHeader;
            inValue.readUnsubscribes = readUnsubscribes1;
            readUnsubscribesResponse retVal = ((BrontoSoapPortType) (this)).readUnsubscribes(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<readUnsubscribesResponse> BrontoSoapPortType.readUnsubscribesAsync(readUnsubscribes1 request)
        {
            return base.Channel.readUnsubscribesAsync(request);
        }

        public Task<readUnsubscribesResponse> readUnsubscribesAsync(sessionHeader sessionHeader, readUnsubscribes readUnsubscribes)
        {
            readUnsubscribes1 inValue = new readUnsubscribes1();
            inValue.sessionHeader = sessionHeader;
            inValue.readUnsubscribes = readUnsubscribes;
            return ((BrontoSoapPortType) (this)).readUnsubscribesAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        readContactsResponse BrontoSoapPortType.readContacts(readContacts1 request)
        {
            return base.Channel.readContacts(request);
        }

        public contactObject[] readContacts(sessionHeader sessionHeader, readContacts readContacts1)
        {
            readContacts1 inValue = new readContacts1();
            inValue.sessionHeader = sessionHeader;
            inValue.readContacts = readContacts1;
            readContactsResponse retVal = ((BrontoSoapPortType) (this)).readContacts(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<readContactsResponse> BrontoSoapPortType.readContactsAsync(readContacts1 request)
        {
            return base.Channel.readContactsAsync(request);
        }

        public Task<readContactsResponse> readContactsAsync(sessionHeader sessionHeader, readContacts readContacts)
        {
            readContacts1 inValue = new readContacts1();
            inValue.sessionHeader = sessionHeader;
            inValue.readContacts = readContacts;
            return ((BrontoSoapPortType) (this)).readContactsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        readRecentOutboundActivitiesResponse BrontoSoapPortType.readRecentOutboundActivities(readRecentOutboundActivities1 request)
        {
            return base.Channel.readRecentOutboundActivities(request);
        }

        public recentActivityObject[] readRecentOutboundActivities(sessionHeader sessionHeader,
            readRecentOutboundActivities readRecentOutboundActivities1)
        {
            readRecentOutboundActivities1 inValue = new readRecentOutboundActivities1();
            inValue.sessionHeader = sessionHeader;
            inValue.readRecentOutboundActivities = readRecentOutboundActivities1;
            readRecentOutboundActivitiesResponse retVal = ((BrontoSoapPortType) (this)).readRecentOutboundActivities(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<readRecentOutboundActivitiesResponse> BrontoSoapPortType.readRecentOutboundActivitiesAsync(
            readRecentOutboundActivities1 request)
        {
            return base.Channel.readRecentOutboundActivitiesAsync(request);
        }

        public Task<readRecentOutboundActivitiesResponse> readRecentOutboundActivitiesAsync(sessionHeader sessionHeader,
            readRecentOutboundActivities readRecentOutboundActivities)
        {
            readRecentOutboundActivities1 inValue = new readRecentOutboundActivities1();
            inValue.sessionHeader = sessionHeader;
            inValue.readRecentOutboundActivities = readRecentOutboundActivities;
            return ((BrontoSoapPortType) (this)).readRecentOutboundActivitiesAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        addContentTagsResponse BrontoSoapPortType.addContentTags(addContentTags request)
        {
            return base.Channel.addContentTags(request);
        }

        public writeResult addContentTags(sessionHeader sessionHeader, contentTagObject[] addContentTags1)
        {
            addContentTags inValue = new addContentTags();
            inValue.sessionHeader = sessionHeader;
            inValue.addContentTags1 = addContentTags1;
            addContentTagsResponse retVal = ((BrontoSoapPortType) (this)).addContentTags(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<addContentTagsResponse> BrontoSoapPortType.addContentTagsAsync(addContentTags request)
        {
            return base.Channel.addContentTagsAsync(request);
        }

        public Task<addContentTagsResponse> addContentTagsAsync(sessionHeader sessionHeader, contentTagObject[] addContentTags1)
        {
            addContentTags inValue = new addContentTags();
            inValue.sessionHeader = sessionHeader;
            inValue.addContentTags1 = addContentTags1;
            return ((BrontoSoapPortType) (this)).addContentTagsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        updateDeliveriesResponse BrontoSoapPortType.updateDeliveries(updateDeliveries request)
        {
            return base.Channel.updateDeliveries(request);
        }

        public writeResult updateDeliveries(sessionHeader sessionHeader, deliveryObject[] updateDeliveries1)
        {
            updateDeliveries inValue = new updateDeliveries();
            inValue.sessionHeader = sessionHeader;
            inValue.updateDeliveries1 = updateDeliveries1;
            updateDeliveriesResponse retVal = ((BrontoSoapPortType) (this)).updateDeliveries(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<updateDeliveriesResponse> BrontoSoapPortType.updateDeliveriesAsync(updateDeliveries request)
        {
            return base.Channel.updateDeliveriesAsync(request);
        }

        public Task<updateDeliveriesResponse> updateDeliveriesAsync(sessionHeader sessionHeader, deliveryObject[] updateDeliveries1)
        {
            updateDeliveries inValue = new updateDeliveries();
            inValue.sessionHeader = sessionHeader;
            inValue.updateDeliveries1 = updateDeliveries1;
            return ((BrontoSoapPortType) (this)).updateDeliveriesAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        deleteSMSMessagesResponse BrontoSoapPortType.deleteSMSMessages(deleteSMSMessages request)
        {
            return base.Channel.deleteSMSMessages(request);
        }

        public writeResult deleteSMSMessages(sessionHeader sessionHeader, smsMessageObject[] deleteSMSMessages1)
        {
            deleteSMSMessages inValue = new deleteSMSMessages();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteSMSMessages1 = deleteSMSMessages1;
            deleteSMSMessagesResponse retVal = ((BrontoSoapPortType) (this)).deleteSMSMessages(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<deleteSMSMessagesResponse> BrontoSoapPortType.deleteSMSMessagesAsync(deleteSMSMessages request)
        {
            return base.Channel.deleteSMSMessagesAsync(request);
        }

        public Task<deleteSMSMessagesResponse> deleteSMSMessagesAsync(sessionHeader sessionHeader, smsMessageObject[] deleteSMSMessages1)
        {
            deleteSMSMessages inValue = new deleteSMSMessages();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteSMSMessages1 = deleteSMSMessages1;
            return ((BrontoSoapPortType) (this)).deleteSMSMessagesAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        addSMSKeywordsResponse BrontoSoapPortType.addSMSKeywords(addSMSKeywords request)
        {
            return base.Channel.addSMSKeywords(request);
        }

        public writeResult addSMSKeywords(sessionHeader sessionHeader, smsKeywordObject[] addSMSKeywords1)
        {
            addSMSKeywords inValue = new addSMSKeywords();
            inValue.sessionHeader = sessionHeader;
            inValue.addSMSKeywords1 = addSMSKeywords1;
            addSMSKeywordsResponse retVal = ((BrontoSoapPortType) (this)).addSMSKeywords(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<addSMSKeywordsResponse> BrontoSoapPortType.addSMSKeywordsAsync(addSMSKeywords request)
        {
            return base.Channel.addSMSKeywordsAsync(request);
        }

        public Task<addSMSKeywordsResponse> addSMSKeywordsAsync(sessionHeader sessionHeader, smsKeywordObject[] addSMSKeywords1)
        {
            addSMSKeywords inValue = new addSMSKeywords();
            inValue.sessionHeader = sessionHeader;
            inValue.addSMSKeywords1 = addSMSKeywords1;
            return ((BrontoSoapPortType) (this)).addSMSKeywordsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        readWorkflowsResponse BrontoSoapPortType.readWorkflows(readWorkflows1 request)
        {
            return base.Channel.readWorkflows(request);
        }

        public workflowObject[] readWorkflows(sessionHeader sessionHeader, readWorkflows readWorkflows1)
        {
            readWorkflows1 inValue = new readWorkflows1();
            inValue.sessionHeader = sessionHeader;
            inValue.readWorkflows = readWorkflows1;
            readWorkflowsResponse retVal = ((BrontoSoapPortType) (this)).readWorkflows(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<readWorkflowsResponse> BrontoSoapPortType.readWorkflowsAsync(readWorkflows1 request)
        {
            return base.Channel.readWorkflowsAsync(request);
        }

        public Task<readWorkflowsResponse> readWorkflowsAsync(sessionHeader sessionHeader, readWorkflows readWorkflows)
        {
            readWorkflows1 inValue = new readWorkflows1();
            inValue.sessionHeader = sessionHeader;
            inValue.readWorkflows = readWorkflows;
            return ((BrontoSoapPortType) (this)).readWorkflowsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        updateApiTokensResponse BrontoSoapPortType.updateApiTokens(updateApiTokens request)
        {
            return base.Channel.updateApiTokens(request);
        }

        public writeResult updateApiTokens(sessionHeader sessionHeader, apiTokenObject[] updateApiTokens1)
        {
            updateApiTokens inValue = new updateApiTokens();
            inValue.sessionHeader = sessionHeader;
            inValue.updateApiTokens1 = updateApiTokens1;
            updateApiTokensResponse retVal = ((BrontoSoapPortType) (this)).updateApiTokens(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<updateApiTokensResponse> BrontoSoapPortType.updateApiTokensAsync(updateApiTokens request)
        {
            return base.Channel.updateApiTokensAsync(request);
        }

        public Task<updateApiTokensResponse> updateApiTokensAsync(sessionHeader sessionHeader, apiTokenObject[] updateApiTokens1)
        {
            updateApiTokens inValue = new updateApiTokens();
            inValue.sessionHeader = sessionHeader;
            inValue.updateApiTokens1 = updateApiTokens1;
            return ((BrontoSoapPortType) (this)).updateApiTokensAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        readAccountsResponse BrontoSoapPortType.readAccounts(readAccounts1 request)
        {
            return base.Channel.readAccounts(request);
        }

        public accountObject[] readAccounts(sessionHeader sessionHeader, readAccounts readAccounts1)
        {
            readAccounts1 inValue = new readAccounts1();
            inValue.sessionHeader = sessionHeader;
            inValue.readAccounts = readAccounts1;
            readAccountsResponse retVal = ((BrontoSoapPortType) (this)).readAccounts(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<readAccountsResponse> BrontoSoapPortType.readAccountsAsync(readAccounts1 request)
        {
            return base.Channel.readAccountsAsync(request);
        }

        public Task<readAccountsResponse> readAccountsAsync(sessionHeader sessionHeader, readAccounts readAccounts)
        {
            readAccounts1 inValue = new readAccounts1();
            inValue.sessionHeader = sessionHeader;
            inValue.readAccounts = readAccounts;
            return ((BrontoSoapPortType) (this)).readAccountsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        addToSMSKeywordResponse BrontoSoapPortType.addToSMSKeyword(addToSMSKeyword1 request)
        {
            return base.Channel.addToSMSKeyword(request);
        }

        public writeResult addToSMSKeyword(sessionHeader sessionHeader, addToSMSKeyword addToSMSKeyword1)
        {
            addToSMSKeyword1 inValue = new addToSMSKeyword1();
            inValue.sessionHeader = sessionHeader;
            inValue.addToSMSKeyword = addToSMSKeyword1;
            addToSMSKeywordResponse retVal = ((BrontoSoapPortType) (this)).addToSMSKeyword(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<addToSMSKeywordResponse> BrontoSoapPortType.addToSMSKeywordAsync(addToSMSKeyword1 request)
        {
            return base.Channel.addToSMSKeywordAsync(request);
        }

        public Task<addToSMSKeywordResponse> addToSMSKeywordAsync(sessionHeader sessionHeader, addToSMSKeyword addToSMSKeyword)
        {
            addToSMSKeyword1 inValue = new addToSMSKeyword1();
            inValue.sessionHeader = sessionHeader;
            inValue.addToSMSKeyword = addToSMSKeyword;
            return ((BrontoSoapPortType) (this)).addToSMSKeywordAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        removeFromListResponse BrontoSoapPortType.removeFromList(removeFromList1 request)
        {
            return base.Channel.removeFromList(request);
        }

        public writeResult removeFromList(sessionHeader sessionHeader, removeFromList removeFromList1)
        {
            removeFromList1 inValue = new removeFromList1();
            inValue.sessionHeader = sessionHeader;
            inValue.removeFromList = removeFromList1;
            removeFromListResponse retVal = ((BrontoSoapPortType) (this)).removeFromList(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<removeFromListResponse> BrontoSoapPortType.removeFromListAsync(removeFromList1 request)
        {
            return base.Channel.removeFromListAsync(request);
        }

        public Task<removeFromListResponse> removeFromListAsync(sessionHeader sessionHeader, removeFromList removeFromList)
        {
            removeFromList1 inValue = new removeFromList1();
            inValue.sessionHeader = sessionHeader;
            inValue.removeFromList = removeFromList;
            return ((BrontoSoapPortType) (this)).removeFromListAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        readDeliveryRecipientsResponse BrontoSoapPortType.readDeliveryRecipients(readDeliveryRecipients1 request)
        {
            return base.Channel.readDeliveryRecipients(request);
        }

        public deliveryRecipientStatObject[] readDeliveryRecipients(sessionHeader sessionHeader,
            readDeliveryRecipients readDeliveryRecipients1)
        {
            readDeliveryRecipients1 inValue = new readDeliveryRecipients1();
            inValue.sessionHeader = sessionHeader;
            inValue.readDeliveryRecipients = readDeliveryRecipients1;
            readDeliveryRecipientsResponse retVal = ((BrontoSoapPortType) (this)).readDeliveryRecipients(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<readDeliveryRecipientsResponse> BrontoSoapPortType.readDeliveryRecipientsAsync(readDeliveryRecipients1 request)
        {
            return base.Channel.readDeliveryRecipientsAsync(request);
        }

        public Task<readDeliveryRecipientsResponse> readDeliveryRecipientsAsync(sessionHeader sessionHeader,
            readDeliveryRecipients readDeliveryRecipients)
        {
            readDeliveryRecipients1 inValue = new readDeliveryRecipients1();
            inValue.sessionHeader = sessionHeader;
            inValue.readDeliveryRecipients = readDeliveryRecipients;
            return ((BrontoSoapPortType) (this)).readDeliveryRecipientsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        addListsResponse BrontoSoapPortType.addLists(addLists request)
        {
            return base.Channel.addLists(request);
        }

        public writeResult addLists(sessionHeader sessionHeader, mailListObject[] addLists1)
        {
            addLists inValue = new addLists();
            inValue.sessionHeader = sessionHeader;
            inValue.addLists1 = addLists1;
            addListsResponse retVal = ((BrontoSoapPortType) (this)).addLists(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<addListsResponse> BrontoSoapPortType.addListsAsync(addLists request)
        {
            return base.Channel.addListsAsync(request);
        }

        public Task<addListsResponse> addListsAsync(sessionHeader sessionHeader, mailListObject[] addLists1)
        {
            addLists inValue = new addLists();
            inValue.sessionHeader = sessionHeader;
            inValue.addLists1 = addLists1;
            return ((BrontoSoapPortType) (this)).addListsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        readSegmentsResponse BrontoSoapPortType.readSegments(readSegments1 request)
        {
            return base.Channel.readSegments(request);
        }

        public segmentObject[] readSegments(sessionHeader sessionHeader, readSegments readSegments1)
        {
            readSegments1 inValue = new readSegments1();
            inValue.sessionHeader = sessionHeader;
            inValue.readSegments = readSegments1;
            readSegmentsResponse retVal = ((BrontoSoapPortType) (this)).readSegments(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<readSegmentsResponse> BrontoSoapPortType.readSegmentsAsync(readSegments1 request)
        {
            return base.Channel.readSegmentsAsync(request);
        }

        public Task<readSegmentsResponse> readSegmentsAsync(sessionHeader sessionHeader, readSegments readSegments)
        {
            readSegments1 inValue = new readSegments1();
            inValue.sessionHeader = sessionHeader;
            inValue.readSegments = readSegments;
            return ((BrontoSoapPortType) (this)).readSegmentsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        readSMSKeywordsResponse BrontoSoapPortType.readSMSKeywords(readSMSKeywords1 request)
        {
            return base.Channel.readSMSKeywords(request);
        }

        public smsKeywordObject[] readSMSKeywords(sessionHeader sessionHeader, readSMSKeywords readSMSKeywords1)
        {
            readSMSKeywords1 inValue = new readSMSKeywords1();
            inValue.sessionHeader = sessionHeader;
            inValue.readSMSKeywords = readSMSKeywords1;
            readSMSKeywordsResponse retVal = ((BrontoSoapPortType) (this)).readSMSKeywords(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<readSMSKeywordsResponse> BrontoSoapPortType.readSMSKeywordsAsync(readSMSKeywords1 request)
        {
            return base.Channel.readSMSKeywordsAsync(request);
        }

        public Task<readSMSKeywordsResponse> readSMSKeywordsAsync(sessionHeader sessionHeader, readSMSKeywords readSMSKeywords)
        {
            readSMSKeywords1 inValue = new readSMSKeywords1();
            inValue.sessionHeader = sessionHeader;
            inValue.readSMSKeywords = readSMSKeywords;
            return ((BrontoSoapPortType) (this)).readSMSKeywordsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        readRecentInboundActivitiesResponse BrontoSoapPortType.readRecentInboundActivities(readRecentInboundActivities1 request)
        {
            return base.Channel.readRecentInboundActivities(request);
        }

        public recentActivityObject[] readRecentInboundActivities(sessionHeader sessionHeader,
            readRecentInboundActivities readRecentInboundActivities1)
        {
            readRecentInboundActivities1 inValue = new readRecentInboundActivities1();
            inValue.sessionHeader = sessionHeader;
            inValue.readRecentInboundActivities = readRecentInboundActivities1;
            readRecentInboundActivitiesResponse retVal = ((BrontoSoapPortType) (this)).readRecentInboundActivities(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<readRecentInboundActivitiesResponse> BrontoSoapPortType.readRecentInboundActivitiesAsync(readRecentInboundActivities1 request)
        {
            return base.Channel.readRecentInboundActivitiesAsync(request);
        }

        public Task<readRecentInboundActivitiesResponse> readRecentInboundActivitiesAsync(sessionHeader sessionHeader,
            readRecentInboundActivities readRecentInboundActivities)
        {
            readRecentInboundActivities1 inValue = new readRecentInboundActivities1();
            inValue.sessionHeader = sessionHeader;
            inValue.readRecentInboundActivities = readRecentInboundActivities;
            return ((BrontoSoapPortType) (this)).readRecentInboundActivitiesAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        addDeliveriesResponse BrontoSoapPortType.addDeliveries(addDeliveries request)
        {
            return base.Channel.addDeliveries(request);
        }

        public writeResult addDeliveries(sessionHeader sessionHeader, deliveryObject[] addDeliveries1)
        {
            addDeliveries inValue = new addDeliveries();
            inValue.sessionHeader = sessionHeader;
            inValue.addDeliveries1 = addDeliveries1;
            addDeliveriesResponse retVal = ((BrontoSoapPortType) (this)).addDeliveries(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<addDeliveriesResponse> BrontoSoapPortType.addDeliveriesAsync(addDeliveries request)
        {
            return base.Channel.addDeliveriesAsync(request);
        }

        public Task<addDeliveriesResponse> addDeliveriesAsync(sessionHeader sessionHeader, deliveryObject[] addDeliveries1)
        {
            addDeliveries inValue = new addDeliveries();
            inValue.sessionHeader = sessionHeader;
            inValue.addDeliveries1 = addDeliveries1;
            return ((BrontoSoapPortType) (this)).addDeliveriesAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        addContactsResponse BrontoSoapPortType.addContacts(addContacts request)
        {
            return base.Channel.addContacts(request);
        }

        public writeResult addContacts(sessionHeader sessionHeader, contactObject[] addContacts1)
        {
            addContacts inValue = new addContacts();
            inValue.sessionHeader = sessionHeader;
            inValue.addContacts1 = addContacts1;
            addContactsResponse retVal = ((BrontoSoapPortType) (this)).addContacts(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<addContactsResponse> BrontoSoapPortType.addContactsAsync(addContacts request)
        {
            return base.Channel.addContactsAsync(request);
        }

        public Task<addContactsResponse> addContactsAsync(sessionHeader sessionHeader, contactObject[] addContacts1)
        {
            addContacts inValue = new addContacts();
            inValue.sessionHeader = sessionHeader;
            inValue.addContacts1 = addContacts1;
            return ((BrontoSoapPortType) (this)).addContactsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        addContactEventResponse BrontoSoapPortType.addContactEvent(addContactEvent1 request)
        {
            return base.Channel.addContactEvent(request);
        }

        public writeResult addContactEvent(sessionHeader sessionHeader, addContactEvent addContactEvent1)
        {
            addContactEvent1 inValue = new addContactEvent1();
            inValue.sessionHeader = sessionHeader;
            inValue.addContactEvent = addContactEvent1;
            addContactEventResponse retVal = ((BrontoSoapPortType) (this)).addContactEvent(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<addContactEventResponse> BrontoSoapPortType.addContactEventAsync(addContactEvent1 request)
        {
            return base.Channel.addContactEventAsync(request);
        }

        public Task<addContactEventResponse> addContactEventAsync(sessionHeader sessionHeader, addContactEvent addContactEvent)
        {
            addContactEvent1 inValue = new addContactEvent1();
            inValue.sessionHeader = sessionHeader;
            inValue.addContactEvent = addContactEvent;
            return ((BrontoSoapPortType) (this)).addContactEventAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        deleteDeliveriesResponse BrontoSoapPortType.deleteDeliveries(deleteDeliveries request)
        {
            return base.Channel.deleteDeliveries(request);
        }

        public writeResult deleteDeliveries(sessionHeader sessionHeader, deliveryObject[] deleteDeliveries1)
        {
            deleteDeliveries inValue = new deleteDeliveries();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteDeliveries1 = deleteDeliveries1;
            deleteDeliveriesResponse retVal = ((BrontoSoapPortType) (this)).deleteDeliveries(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<deleteDeliveriesResponse> BrontoSoapPortType.deleteDeliveriesAsync(deleteDeliveries request)
        {
            return base.Channel.deleteDeliveriesAsync(request);
        }

        public Task<deleteDeliveriesResponse> deleteDeliveriesAsync(sessionHeader sessionHeader, deliveryObject[] deleteDeliveries1)
        {
            deleteDeliveries inValue = new deleteDeliveries();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteDeliveries1 = deleteDeliveries1;
            return ((BrontoSoapPortType) (this)).deleteDeliveriesAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        loginResponse BrontoSoapPortType.login(login request)
        {
            return base.Channel.login(request);
        }

        public string login(string apiToken)
        {
            login inValue = new login();
            inValue.apiToken = apiToken;
            loginResponse retVal = ((BrontoSoapPortType) (this)).login(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<loginResponse> BrontoSoapPortType.loginAsync(login request)
        {
            return base.Channel.loginAsync(request);
        }

        public Task<loginResponse> loginAsync(string apiToken)
        {
            login inValue = new login();
            inValue.apiToken = apiToken;
            return ((BrontoSoapPortType) (this)).loginAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        deleteOrdersResponse BrontoSoapPortType.deleteOrders(deleteOrders request)
        {
            return base.Channel.deleteOrders(request);
        }

        public writeResult deleteOrders(sessionHeader sessionHeader, orderObject[] deleteOrders1)
        {
            deleteOrders inValue = new deleteOrders();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteOrders1 = deleteOrders1;
            deleteOrdersResponse retVal = ((BrontoSoapPortType) (this)).deleteOrders(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<deleteOrdersResponse> BrontoSoapPortType.deleteOrdersAsync(deleteOrders request)
        {
            return base.Channel.deleteOrdersAsync(request);
        }

        public Task<deleteOrdersResponse> deleteOrdersAsync(sessionHeader sessionHeader, orderObject[] deleteOrders1)
        {
            deleteOrders inValue = new deleteOrders();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteOrders1 = deleteOrders1;
            return ((BrontoSoapPortType) (this)).deleteOrdersAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        addOrUpdateDeliveryGroupResponse BrontoSoapPortType.addOrUpdateDeliveryGroup(addOrUpdateDeliveryGroup request)
        {
            return base.Channel.addOrUpdateDeliveryGroup(request);
        }

        public writeResult addOrUpdateDeliveryGroup(sessionHeader sessionHeader, deliveryGroupObject[] addOrUpdateDeliveryGroup1)
        {
            addOrUpdateDeliveryGroup inValue = new addOrUpdateDeliveryGroup();
            inValue.sessionHeader = sessionHeader;
            inValue.addOrUpdateDeliveryGroup1 = addOrUpdateDeliveryGroup1;
            addOrUpdateDeliveryGroupResponse retVal = ((BrontoSoapPortType) (this)).addOrUpdateDeliveryGroup(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<addOrUpdateDeliveryGroupResponse> BrontoSoapPortType.addOrUpdateDeliveryGroupAsync(addOrUpdateDeliveryGroup request)
        {
            return base.Channel.addOrUpdateDeliveryGroupAsync(request);
        }

        public Task<addOrUpdateDeliveryGroupResponse> addOrUpdateDeliveryGroupAsync(sessionHeader sessionHeader,
            deliveryGroupObject[] addOrUpdateDeliveryGroup1)
        {
            addOrUpdateDeliveryGroup inValue = new addOrUpdateDeliveryGroup();
            inValue.sessionHeader = sessionHeader;
            inValue.addOrUpdateDeliveryGroup1 = addOrUpdateDeliveryGroup1;
            return ((BrontoSoapPortType) (this)).addOrUpdateDeliveryGroupAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        updateMessageFoldersResponse BrontoSoapPortType.updateMessageFolders(updateMessageFolders request)
        {
            return base.Channel.updateMessageFolders(request);
        }

        public writeResult updateMessageFolders(sessionHeader sessionHeader, messageFolderObject[] updateMessageFolders1)
        {
            updateMessageFolders inValue = new updateMessageFolders();
            inValue.sessionHeader = sessionHeader;
            inValue.updateMessageFolders1 = updateMessageFolders1;
            updateMessageFoldersResponse retVal = ((BrontoSoapPortType) (this)).updateMessageFolders(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<updateMessageFoldersResponse> BrontoSoapPortType.updateMessageFoldersAsync(updateMessageFolders request)
        {
            return base.Channel.updateMessageFoldersAsync(request);
        }

        public Task<updateMessageFoldersResponse> updateMessageFoldersAsync(sessionHeader sessionHeader,
            messageFolderObject[] updateMessageFolders1)
        {
            updateMessageFolders inValue = new updateMessageFolders();
            inValue.sessionHeader = sessionHeader;
            inValue.updateMessageFolders1 = updateMessageFolders1;
            return ((BrontoSoapPortType) (this)).updateMessageFoldersAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        addOrUpdateOrdersResponse BrontoSoapPortType.addOrUpdateOrders(addOrUpdateOrders request)
        {
            return base.Channel.addOrUpdateOrders(request);
        }

        public writeResult addOrUpdateOrders(sessionHeader sessionHeader, orderObject[] addOrUpdateOrders1)
        {
            addOrUpdateOrders inValue = new addOrUpdateOrders();
            inValue.sessionHeader = sessionHeader;
            inValue.addOrUpdateOrders1 = addOrUpdateOrders1;
            addOrUpdateOrdersResponse retVal = ((BrontoSoapPortType) (this)).addOrUpdateOrders(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<addOrUpdateOrdersResponse> BrontoSoapPortType.addOrUpdateOrdersAsync(addOrUpdateOrders request)
        {
            return base.Channel.addOrUpdateOrdersAsync(request);
        }

        public Task<addOrUpdateOrdersResponse> addOrUpdateOrdersAsync(sessionHeader sessionHeader, orderObject[] addOrUpdateOrders1)
        {
            addOrUpdateOrders inValue = new addOrUpdateOrders();
            inValue.sessionHeader = sessionHeader;
            inValue.addOrUpdateOrders1 = addOrUpdateOrders1;
            return ((BrontoSoapPortType) (this)).addOrUpdateOrdersAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        addOrUpdateContactsResponse BrontoSoapPortType.addOrUpdateContacts(addOrUpdateContacts request)
        {
            return base.Channel.addOrUpdateContacts(request);
        }

        public writeResult addOrUpdateContacts(sessionHeader sessionHeader, contactObject[] addOrUpdateContacts1)
        {
            addOrUpdateContacts inValue = new addOrUpdateContacts();
            inValue.sessionHeader = sessionHeader;
            inValue.addOrUpdateContacts1 = addOrUpdateContacts1;
            addOrUpdateContactsResponse retVal = ((BrontoSoapPortType) (this)).addOrUpdateContacts(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<addOrUpdateContactsResponse> BrontoSoapPortType.addOrUpdateContactsAsync(addOrUpdateContacts request)
        {
            return base.Channel.addOrUpdateContactsAsync(request);
        }

        public Task<addOrUpdateContactsResponse> addOrUpdateContactsAsync(sessionHeader sessionHeader, contactObject[] addOrUpdateContacts1)
        {
            addOrUpdateContacts inValue = new addOrUpdateContacts();
            inValue.sessionHeader = sessionHeader;
            inValue.addOrUpdateContacts1 = addOrUpdateContacts1;
            return ((BrontoSoapPortType) (this)).addOrUpdateContactsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        readDeliveriesResponse BrontoSoapPortType.readDeliveries(readDeliveries1 request)
        {
            return base.Channel.readDeliveries(request);
        }

        public deliveryObject[] readDeliveries(sessionHeader sessionHeader, readDeliveries readDeliveries1)
        {
            readDeliveries1 inValue = new readDeliveries1();
            inValue.sessionHeader = sessionHeader;
            inValue.readDeliveries = readDeliveries1;
            readDeliveriesResponse retVal = ((BrontoSoapPortType) (this)).readDeliveries(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<readDeliveriesResponse> BrontoSoapPortType.readDeliveriesAsync(readDeliveries1 request)
        {
            return base.Channel.readDeliveriesAsync(request);
        }

        public Task<readDeliveriesResponse> readDeliveriesAsync(sessionHeader sessionHeader, readDeliveries readDeliveries)
        {
            readDeliveries1 inValue = new readDeliveries1();
            inValue.sessionHeader = sessionHeader;
            inValue.readDeliveries = readDeliveries;
            return ((BrontoSoapPortType) (this)).readDeliveriesAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        readSMSDeliveriesResponse BrontoSoapPortType.readSMSDeliveries(readSMSDeliveries1 request)
        {
            return base.Channel.readSMSDeliveries(request);
        }

        public smsDeliveryObject[] readSMSDeliveries(sessionHeader sessionHeader, readSMSDeliveries readSMSDeliveries1)
        {
            readSMSDeliveries1 inValue = new readSMSDeliveries1();
            inValue.sessionHeader = sessionHeader;
            inValue.readSMSDeliveries = readSMSDeliveries1;
            readSMSDeliveriesResponse retVal = ((BrontoSoapPortType) (this)).readSMSDeliveries(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<readSMSDeliveriesResponse> BrontoSoapPortType.readSMSDeliveriesAsync(readSMSDeliveries1 request)
        {
            return base.Channel.readSMSDeliveriesAsync(request);
        }

        public Task<readSMSDeliveriesResponse> readSMSDeliveriesAsync(sessionHeader sessionHeader, readSMSDeliveries readSMSDeliveries)
        {
            readSMSDeliveries1 inValue = new readSMSDeliveries1();
            inValue.sessionHeader = sessionHeader;
            inValue.readSMSDeliveries = readSMSDeliveries;
            return ((BrontoSoapPortType) (this)).readSMSDeliveriesAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        updateListsResponse BrontoSoapPortType.updateLists(updateLists request)
        {
            return base.Channel.updateLists(request);
        }

        public writeResult updateLists(sessionHeader sessionHeader, mailListObject[] updateLists1)
        {
            updateLists inValue = new updateLists();
            inValue.sessionHeader = sessionHeader;
            inValue.updateLists1 = updateLists1;
            updateListsResponse retVal = ((BrontoSoapPortType) (this)).updateLists(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<updateListsResponse> BrontoSoapPortType.updateListsAsync(updateLists request)
        {
            return base.Channel.updateListsAsync(request);
        }

        public Task<updateListsResponse> updateListsAsync(sessionHeader sessionHeader, mailListObject[] updateLists1)
        {
            updateLists inValue = new updateLists();
            inValue.sessionHeader = sessionHeader;
            inValue.updateLists1 = updateLists1;
            return ((BrontoSoapPortType) (this)).updateListsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        readContentTagsResponse BrontoSoapPortType.readContentTags(readContentTags1 request)
        {
            return base.Channel.readContentTags(request);
        }

        public contentTagObject[] readContentTags(sessionHeader sessionHeader, readContentTags readContentTags1)
        {
            readContentTags1 inValue = new readContentTags1();
            inValue.sessionHeader = sessionHeader;
            inValue.readContentTags = readContentTags1;
            readContentTagsResponse retVal = ((BrontoSoapPortType) (this)).readContentTags(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<readContentTagsResponse> BrontoSoapPortType.readContentTagsAsync(readContentTags1 request)
        {
            return base.Channel.readContentTagsAsync(request);
        }

        public Task<readContentTagsResponse> readContentTagsAsync(sessionHeader sessionHeader, readContentTags readContentTags)
        {
            readContentTags1 inValue = new readContentTags1();
            inValue.sessionHeader = sessionHeader;
            inValue.readContentTags = readContentTags;
            return ((BrontoSoapPortType) (this)).readContentTagsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        addAccountsResponse BrontoSoapPortType.addAccounts(addAccounts request)
        {
            return base.Channel.addAccounts(request);
        }

        public writeResult addAccounts(sessionHeader sessionHeader, accountObject[] addAccounts1)
        {
            addAccounts inValue = new addAccounts();
            inValue.sessionHeader = sessionHeader;
            inValue.addAccounts1 = addAccounts1;
            addAccountsResponse retVal = ((BrontoSoapPortType) (this)).addAccounts(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<addAccountsResponse> BrontoSoapPortType.addAccountsAsync(addAccounts request)
        {
            return base.Channel.addAccountsAsync(request);
        }

        public Task<addAccountsResponse> addAccountsAsync(sessionHeader sessionHeader, accountObject[] addAccounts1)
        {
            addAccounts inValue = new addAccounts();
            inValue.sessionHeader = sessionHeader;
            inValue.addAccounts1 = addAccounts1;
            return ((BrontoSoapPortType) (this)).addAccountsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        deleteListsResponse BrontoSoapPortType.deleteLists(deleteLists request)
        {
            return base.Channel.deleteLists(request);
        }

        public writeResult deleteLists(sessionHeader sessionHeader, mailListObject[] deleteLists1)
        {
            deleteLists inValue = new deleteLists();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteLists1 = deleteLists1;
            deleteListsResponse retVal = ((BrontoSoapPortType) (this)).deleteLists(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<deleteListsResponse> BrontoSoapPortType.deleteListsAsync(deleteLists request)
        {
            return base.Channel.deleteListsAsync(request);
        }

        public Task<deleteListsResponse> deleteListsAsync(sessionHeader sessionHeader, mailListObject[] deleteLists1)
        {
            deleteLists inValue = new deleteLists();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteLists1 = deleteLists1;
            return ((BrontoSoapPortType) (this)).deleteListsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        deleteContentTagsResponse BrontoSoapPortType.deleteContentTags(deleteContentTags request)
        {
            return base.Channel.deleteContentTags(request);
        }

        public writeResult deleteContentTags(sessionHeader sessionHeader, contentTagObject[] deleteContentTags1)
        {
            deleteContentTags inValue = new deleteContentTags();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteContentTags1 = deleteContentTags1;
            deleteContentTagsResponse retVal = ((BrontoSoapPortType) (this)).deleteContentTags(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<deleteContentTagsResponse> BrontoSoapPortType.deleteContentTagsAsync(deleteContentTags request)
        {
            return base.Channel.deleteContentTagsAsync(request);
        }

        public Task<deleteContentTagsResponse> deleteContentTagsAsync(sessionHeader sessionHeader, contentTagObject[] deleteContentTags1)
        {
            deleteContentTags inValue = new deleteContentTags();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteContentTags1 = deleteContentTags1;
            return ((BrontoSoapPortType) (this)).deleteContentTagsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        removeFromSMSKeywordResponse BrontoSoapPortType.removeFromSMSKeyword(removeFromSMSKeyword1 request)
        {
            return base.Channel.removeFromSMSKeyword(request);
        }

        public writeResult removeFromSMSKeyword(sessionHeader sessionHeader, removeFromSMSKeyword removeFromSMSKeyword1)
        {
            removeFromSMSKeyword1 inValue = new removeFromSMSKeyword1();
            inValue.sessionHeader = sessionHeader;
            inValue.removeFromSMSKeyword = removeFromSMSKeyword1;
            removeFromSMSKeywordResponse retVal = ((BrontoSoapPortType) (this)).removeFromSMSKeyword(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<removeFromSMSKeywordResponse> BrontoSoapPortType.removeFromSMSKeywordAsync(removeFromSMSKeyword1 request)
        {
            return base.Channel.removeFromSMSKeywordAsync(request);
        }

        public Task<removeFromSMSKeywordResponse> removeFromSMSKeywordAsync(sessionHeader sessionHeader,
            removeFromSMSKeyword removeFromSMSKeyword)
        {
            removeFromSMSKeyword1 inValue = new removeFromSMSKeyword1();
            inValue.sessionHeader = sessionHeader;
            inValue.removeFromSMSKeyword = removeFromSMSKeyword;
            return ((BrontoSoapPortType) (this)).removeFromSMSKeywordAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        addMessagesResponse BrontoSoapPortType.addMessages(addMessages request)
        {
            return base.Channel.addMessages(request);
        }

        public writeResult addMessages(sessionHeader sessionHeader, messageObject[] addMessages1)
        {
            addMessages inValue = new addMessages();
            inValue.sessionHeader = sessionHeader;
            inValue.addMessages1 = addMessages1;
            addMessagesResponse retVal = ((BrontoSoapPortType) (this)).addMessages(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<addMessagesResponse> BrontoSoapPortType.addMessagesAsync(addMessages request)
        {
            return base.Channel.addMessagesAsync(request);
        }

        public Task<addMessagesResponse> addMessagesAsync(sessionHeader sessionHeader, messageObject[] addMessages1)
        {
            addMessages inValue = new addMessages();
            inValue.sessionHeader = sessionHeader;
            inValue.addMessages1 = addMessages1;
            return ((BrontoSoapPortType) (this)).addMessagesAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        readFieldsResponse BrontoSoapPortType.readFields(readFields1 request)
        {
            return base.Channel.readFields(request);
        }

        public fieldObject[] readFields(sessionHeader sessionHeader, readFields readFields1)
        {
            readFields1 inValue = new readFields1();
            inValue.sessionHeader = sessionHeader;
            inValue.readFields = readFields1;
            readFieldsResponse retVal = ((BrontoSoapPortType) (this)).readFields(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<readFieldsResponse> BrontoSoapPortType.readFieldsAsync(readFields1 request)
        {
            return base.Channel.readFieldsAsync(request);
        }

        public Task<readFieldsResponse> readFieldsAsync(sessionHeader sessionHeader, readFields readFields)
        {
            readFields1 inValue = new readFields1();
            inValue.sessionHeader = sessionHeader;
            inValue.readFields = readFields;
            return ((BrontoSoapPortType) (this)).readFieldsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        addHeaderFootersResponse BrontoSoapPortType.addHeaderFooters(addHeaderFooters request)
        {
            return base.Channel.addHeaderFooters(request);
        }

        public writeResult addHeaderFooters(sessionHeader sessionHeader, headerFooterObject[] addHeaderFooters1)
        {
            addHeaderFooters inValue = new addHeaderFooters();
            inValue.sessionHeader = sessionHeader;
            inValue.addHeaderFooters1 = addHeaderFooters1;
            addHeaderFootersResponse retVal = ((BrontoSoapPortType) (this)).addHeaderFooters(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<addHeaderFootersResponse> BrontoSoapPortType.addHeaderFootersAsync(addHeaderFooters request)
        {
            return base.Channel.addHeaderFootersAsync(request);
        }

        public Task<addHeaderFootersResponse> addHeaderFootersAsync(sessionHeader sessionHeader, headerFooterObject[] addHeaderFooters1)
        {
            addHeaderFooters inValue = new addHeaderFooters();
            inValue.sessionHeader = sessionHeader;
            inValue.addHeaderFooters1 = addHeaderFooters1;
            return ((BrontoSoapPortType) (this)).addHeaderFootersAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        updateFieldsResponse BrontoSoapPortType.updateFields(updateFields request)
        {
            return base.Channel.updateFields(request);
        }

        public writeResult updateFields(sessionHeader sessionHeader, fieldObject[] updateFields1)
        {
            updateFields inValue = new updateFields();
            inValue.sessionHeader = sessionHeader;
            inValue.updateFields1 = updateFields1;
            updateFieldsResponse retVal = ((BrontoSoapPortType) (this)).updateFields(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<updateFieldsResponse> BrontoSoapPortType.updateFieldsAsync(updateFields request)
        {
            return base.Channel.updateFieldsAsync(request);
        }

        public Task<updateFieldsResponse> updateFieldsAsync(sessionHeader sessionHeader, fieldObject[] updateFields1)
        {
            updateFields inValue = new updateFields();
            inValue.sessionHeader = sessionHeader;
            inValue.updateFields1 = updateFields1;
            return ((BrontoSoapPortType) (this)).updateFieldsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        deleteFromDeliveryGroupResponse BrontoSoapPortType.deleteFromDeliveryGroup(deleteFromDeliveryGroup1 request)
        {
            return base.Channel.deleteFromDeliveryGroup(request);
        }

        public writeResult deleteFromDeliveryGroup(sessionHeader sessionHeader, deleteFromDeliveryGroup deleteFromDeliveryGroup1)
        {
            deleteFromDeliveryGroup1 inValue = new deleteFromDeliveryGroup1();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteFromDeliveryGroup = deleteFromDeliveryGroup1;
            deleteFromDeliveryGroupResponse retVal = ((BrontoSoapPortType) (this)).deleteFromDeliveryGroup(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<deleteFromDeliveryGroupResponse> BrontoSoapPortType.deleteFromDeliveryGroupAsync(deleteFromDeliveryGroup1 request)
        {
            return base.Channel.deleteFromDeliveryGroupAsync(request);
        }

        public Task<deleteFromDeliveryGroupResponse> deleteFromDeliveryGroupAsync(sessionHeader sessionHeader,
            deleteFromDeliveryGroup deleteFromDeliveryGroup)
        {
            deleteFromDeliveryGroup1 inValue = new deleteFromDeliveryGroup1();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteFromDeliveryGroup = deleteFromDeliveryGroup;
            return ((BrontoSoapPortType) (this)).deleteFromDeliveryGroupAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        clearListsResponse BrontoSoapPortType.clearLists(clearLists request)
        {
            return base.Channel.clearLists(request);
        }

        public writeResult clearLists(sessionHeader sessionHeader, mailListObject[] clearLists1)
        {
            clearLists inValue = new clearLists();
            inValue.sessionHeader = sessionHeader;
            inValue.clearLists1 = clearLists1;
            clearListsResponse retVal = ((BrontoSoapPortType) (this)).clearLists(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<clearListsResponse> BrontoSoapPortType.clearListsAsync(clearLists request)
        {
            return base.Channel.clearListsAsync(request);
        }

        public Task<clearListsResponse> clearListsAsync(sessionHeader sessionHeader, mailListObject[] clearLists1)
        {
            clearLists inValue = new clearLists();
            inValue.sessionHeader = sessionHeader;
            inValue.clearLists1 = clearLists1;
            return ((BrontoSoapPortType) (this)).clearListsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        addMessageRulesResponse BrontoSoapPortType.addMessageRules(addMessageRules request)
        {
            return base.Channel.addMessageRules(request);
        }

        public writeResult addMessageRules(sessionHeader sessionHeader, messageRuleObject[] addMessageRules1)
        {
            addMessageRules inValue = new addMessageRules();
            inValue.sessionHeader = sessionHeader;
            inValue.addMessageRules1 = addMessageRules1;
            addMessageRulesResponse retVal = ((BrontoSoapPortType) (this)).addMessageRules(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<addMessageRulesResponse> BrontoSoapPortType.addMessageRulesAsync(addMessageRules request)
        {
            return base.Channel.addMessageRulesAsync(request);
        }

        public Task<addMessageRulesResponse> addMessageRulesAsync(sessionHeader sessionHeader, messageRuleObject[] addMessageRules1)
        {
            addMessageRules inValue = new addMessageRules();
            inValue.sessionHeader = sessionHeader;
            inValue.addMessageRules1 = addMessageRules1;
            return ((BrontoSoapPortType) (this)).addMessageRulesAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        addMessageFoldersResponse BrontoSoapPortType.addMessageFolders(addMessageFolders request)
        {
            return base.Channel.addMessageFolders(request);
        }

        public writeResult addMessageFolders(sessionHeader sessionHeader, messageFolderObject[] addMessageFolders1)
        {
            addMessageFolders inValue = new addMessageFolders();
            inValue.sessionHeader = sessionHeader;
            inValue.addMessageFolders1 = addMessageFolders1;
            addMessageFoldersResponse retVal = ((BrontoSoapPortType) (this)).addMessageFolders(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<addMessageFoldersResponse> BrontoSoapPortType.addMessageFoldersAsync(addMessageFolders request)
        {
            return base.Channel.addMessageFoldersAsync(request);
        }

        public Task<addMessageFoldersResponse> addMessageFoldersAsync(sessionHeader sessionHeader, messageFolderObject[] addMessageFolders1)
        {
            addMessageFolders inValue = new addMessageFolders();
            inValue.sessionHeader = sessionHeader;
            inValue.addMessageFolders1 = addMessageFolders1;
            return ((BrontoSoapPortType) (this)).addMessageFoldersAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        readMessagesResponse BrontoSoapPortType.readMessages(readMessages1 request)
        {
            return base.Channel.readMessages(request);
        }

        public messageObject[] readMessages(sessionHeader sessionHeader, readMessages readMessages1)
        {
            readMessages1 inValue = new readMessages1();
            inValue.sessionHeader = sessionHeader;
            inValue.readMessages = readMessages1;
            readMessagesResponse retVal = ((BrontoSoapPortType) (this)).readMessages(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<readMessagesResponse> BrontoSoapPortType.readMessagesAsync(readMessages1 request)
        {
            return base.Channel.readMessagesAsync(request);
        }

        public Task<readMessagesResponse> readMessagesAsync(sessionHeader sessionHeader, readMessages readMessages)
        {
            readMessages1 inValue = new readMessages1();
            inValue.sessionHeader = sessionHeader;
            inValue.readMessages = readMessages;
            return ((BrontoSoapPortType) (this)).readMessagesAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        deleteAccountsResponse BrontoSoapPortType.deleteAccounts(deleteAccounts request)
        {
            return base.Channel.deleteAccounts(request);
        }

        public writeResult deleteAccounts(sessionHeader sessionHeader, accountObject[] deleteAccounts1)
        {
            deleteAccounts inValue = new deleteAccounts();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteAccounts1 = deleteAccounts1;
            deleteAccountsResponse retVal = ((BrontoSoapPortType) (this)).deleteAccounts(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<deleteAccountsResponse> BrontoSoapPortType.deleteAccountsAsync(deleteAccounts request)
        {
            return base.Channel.deleteAccountsAsync(request);
        }

        public Task<deleteAccountsResponse> deleteAccountsAsync(sessionHeader sessionHeader, accountObject[] deleteAccounts1)
        {
            deleteAccounts inValue = new deleteAccounts();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteAccounts1 = deleteAccounts1;
            return ((BrontoSoapPortType) (this)).deleteAccountsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        updateSMSMessagesResponse BrontoSoapPortType.updateSMSMessages(updateSMSMessages request)
        {
            return base.Channel.updateSMSMessages(request);
        }

        public writeResult updateSMSMessages(sessionHeader sessionHeader, smsMessageObject[] updateSMSMessages1)
        {
            updateSMSMessages inValue = new updateSMSMessages();
            inValue.sessionHeader = sessionHeader;
            inValue.updateSMSMessages1 = updateSMSMessages1;
            updateSMSMessagesResponse retVal = ((BrontoSoapPortType) (this)).updateSMSMessages(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<updateSMSMessagesResponse> BrontoSoapPortType.updateSMSMessagesAsync(updateSMSMessages request)
        {
            return base.Channel.updateSMSMessagesAsync(request);
        }

        public Task<updateSMSMessagesResponse> updateSMSMessagesAsync(sessionHeader sessionHeader, smsMessageObject[] updateSMSMessages1)
        {
            updateSMSMessages inValue = new updateSMSMessages();
            inValue.sessionHeader = sessionHeader;
            inValue.updateSMSMessages1 = updateSMSMessages1;
            return ((BrontoSoapPortType) (this)).updateSMSMessagesAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        readMessageRulesResponse BrontoSoapPortType.readMessageRules(readMessageRules1 request)
        {
            return base.Channel.readMessageRules(request);
        }

        public messageRuleObject[] readMessageRules(sessionHeader sessionHeader, readMessageRules readMessageRules1)
        {
            readMessageRules1 inValue = new readMessageRules1();
            inValue.sessionHeader = sessionHeader;
            inValue.readMessageRules = readMessageRules1;
            readMessageRulesResponse retVal = ((BrontoSoapPortType) (this)).readMessageRules(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<readMessageRulesResponse> BrontoSoapPortType.readMessageRulesAsync(readMessageRules1 request)
        {
            return base.Channel.readMessageRulesAsync(request);
        }

        public Task<readMessageRulesResponse> readMessageRulesAsync(sessionHeader sessionHeader, readMessageRules readMessageRules)
        {
            readMessageRules1 inValue = new readMessageRules1();
            inValue.sessionHeader = sessionHeader;
            inValue.readMessageRules = readMessageRules;
            return ((BrontoSoapPortType) (this)).readMessageRulesAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        addWorkflowsResponse BrontoSoapPortType.addWorkflows(addWorkflows request)
        {
            return base.Channel.addWorkflows(request);
        }

        public writeResult addWorkflows(sessionHeader sessionHeader, workflowObject[] addWorkflows1)
        {
            addWorkflows inValue = new addWorkflows();
            inValue.sessionHeader = sessionHeader;
            inValue.addWorkflows1 = addWorkflows1;
            addWorkflowsResponse retVal = ((BrontoSoapPortType) (this)).addWorkflows(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<addWorkflowsResponse> BrontoSoapPortType.addWorkflowsAsync(addWorkflows request)
        {
            return base.Channel.addWorkflowsAsync(request);
        }

        public Task<addWorkflowsResponse> addWorkflowsAsync(sessionHeader sessionHeader, workflowObject[] addWorkflows1)
        {
            addWorkflows inValue = new addWorkflows();
            inValue.sessionHeader = sessionHeader;
            inValue.addWorkflows1 = addWorkflows1;
            return ((BrontoSoapPortType) (this)).addWorkflowsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        deleteSMSKeywordsResponse BrontoSoapPortType.deleteSMSKeywords(deleteSMSKeywords request)
        {
            return base.Channel.deleteSMSKeywords(request);
        }

        public writeResult deleteSMSKeywords(sessionHeader sessionHeader, smsKeywordObject[] deleteSMSKeywords1)
        {
            deleteSMSKeywords inValue = new deleteSMSKeywords();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteSMSKeywords1 = deleteSMSKeywords1;
            deleteSMSKeywordsResponse retVal = ((BrontoSoapPortType) (this)).deleteSMSKeywords(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<deleteSMSKeywordsResponse> BrontoSoapPortType.deleteSMSKeywordsAsync(deleteSMSKeywords request)
        {
            return base.Channel.deleteSMSKeywordsAsync(request);
        }

        public Task<deleteSMSKeywordsResponse> deleteSMSKeywordsAsync(sessionHeader sessionHeader, smsKeywordObject[] deleteSMSKeywords1)
        {
            deleteSMSKeywords inValue = new deleteSMSKeywords();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteSMSKeywords1 = deleteSMSKeywords1;
            return ((BrontoSoapPortType) (this)).deleteSMSKeywordsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        updateWorkflowsResponse BrontoSoapPortType.updateWorkflows(updateWorkflows request)
        {
            return base.Channel.updateWorkflows(request);
        }

        public writeResult updateWorkflows(sessionHeader sessionHeader, workflowObject[] updateWorkflows1)
        {
            updateWorkflows inValue = new updateWorkflows();
            inValue.sessionHeader = sessionHeader;
            inValue.updateWorkflows1 = updateWorkflows1;
            updateWorkflowsResponse retVal = ((BrontoSoapPortType) (this)).updateWorkflows(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<updateWorkflowsResponse> BrontoSoapPortType.updateWorkflowsAsync(updateWorkflows request)
        {
            return base.Channel.updateWorkflowsAsync(request);
        }

        public Task<updateWorkflowsResponse> updateWorkflowsAsync(sessionHeader sessionHeader, workflowObject[] updateWorkflows1)
        {
            updateWorkflows inValue = new updateWorkflows();
            inValue.sessionHeader = sessionHeader;
            inValue.updateWorkflows1 = updateWorkflows1;
            return ((BrontoSoapPortType) (this)).updateWorkflowsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        addConversionResponse BrontoSoapPortType.addConversion(addConversion request)
        {
            return base.Channel.addConversion(request);
        }

        public writeResult addConversion(sessionHeader sessionHeader, conversionObject[] addConversion1)
        {
            addConversion inValue = new addConversion();
            inValue.sessionHeader = sessionHeader;
            inValue.addConversion1 = addConversion1;
            addConversionResponse retVal = ((BrontoSoapPortType) (this)).addConversion(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<addConversionResponse> BrontoSoapPortType.addConversionAsync(addConversion request)
        {
            return base.Channel.addConversionAsync(request);
        }

        public Task<addConversionResponse> addConversionAsync(sessionHeader sessionHeader, conversionObject[] addConversion1)
        {
            addConversion inValue = new addConversion();
            inValue.sessionHeader = sessionHeader;
            inValue.addConversion1 = addConversion1;
            return ((BrontoSoapPortType) (this)).addConversionAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        updateAccountsResponse BrontoSoapPortType.updateAccounts(updateAccounts request)
        {
            return base.Channel.updateAccounts(request);
        }

        public writeResult updateAccounts(sessionHeader sessionHeader, accountObject[] updateAccounts1)
        {
            updateAccounts inValue = new updateAccounts();
            inValue.sessionHeader = sessionHeader;
            inValue.updateAccounts1 = updateAccounts1;
            updateAccountsResponse retVal = ((BrontoSoapPortType) (this)).updateAccounts(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<updateAccountsResponse> BrontoSoapPortType.updateAccountsAsync(updateAccounts request)
        {
            return base.Channel.updateAccountsAsync(request);
        }

        public Task<updateAccountsResponse> updateAccountsAsync(sessionHeader sessionHeader, accountObject[] updateAccounts1)
        {
            updateAccounts inValue = new updateAccounts();
            inValue.sessionHeader = sessionHeader;
            inValue.updateAccounts1 = updateAccounts1;
            return ((BrontoSoapPortType) (this)).updateAccountsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        readBouncesResponse BrontoSoapPortType.readBounces(readBounces1 request)
        {
            return base.Channel.readBounces(request);
        }

        public bounceObject[] readBounces(sessionHeader sessionHeader, readBounces readBounces1)
        {
            readBounces1 inValue = new readBounces1();
            inValue.sessionHeader = sessionHeader;
            inValue.readBounces = readBounces1;
            readBouncesResponse retVal = ((BrontoSoapPortType) (this)).readBounces(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<readBouncesResponse> BrontoSoapPortType.readBouncesAsync(readBounces1 request)
        {
            return base.Channel.readBouncesAsync(request);
        }

        public Task<readBouncesResponse> readBouncesAsync(sessionHeader sessionHeader, readBounces readBounces)
        {
            readBounces1 inValue = new readBounces1();
            inValue.sessionHeader = sessionHeader;
            inValue.readBounces = readBounces;
            return ((BrontoSoapPortType) (this)).readBouncesAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        updateHeaderFootersResponse BrontoSoapPortType.updateHeaderFooters(updateHeaderFooters request)
        {
            return base.Channel.updateHeaderFooters(request);
        }

        public writeResult updateHeaderFooters(sessionHeader sessionHeader, headerFooterObject[] updateHeaderFooters1)
        {
            updateHeaderFooters inValue = new updateHeaderFooters();
            inValue.sessionHeader = sessionHeader;
            inValue.updateHeaderFooters1 = updateHeaderFooters1;
            updateHeaderFootersResponse retVal = ((BrontoSoapPortType) (this)).updateHeaderFooters(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<updateHeaderFootersResponse> BrontoSoapPortType.updateHeaderFootersAsync(updateHeaderFooters request)
        {
            return base.Channel.updateHeaderFootersAsync(request);
        }

        public Task<updateHeaderFootersResponse> updateHeaderFootersAsync(sessionHeader sessionHeader,
            headerFooterObject[] updateHeaderFooters1)
        {
            updateHeaderFooters inValue = new updateHeaderFooters();
            inValue.sessionHeader = sessionHeader;
            inValue.updateHeaderFooters1 = updateHeaderFooters1;
            return ((BrontoSoapPortType) (this)).updateHeaderFootersAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        deleteMessageFoldersResponse BrontoSoapPortType.deleteMessageFolders(deleteMessageFolders request)
        {
            return base.Channel.deleteMessageFolders(request);
        }

        public writeResult deleteMessageFolders(sessionHeader sessionHeader, messageFolderObject[] deleteMessageFolders1)
        {
            deleteMessageFolders inValue = new deleteMessageFolders();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteMessageFolders1 = deleteMessageFolders1;
            deleteMessageFoldersResponse retVal = ((BrontoSoapPortType) (this)).deleteMessageFolders(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<deleteMessageFoldersResponse> BrontoSoapPortType.deleteMessageFoldersAsync(deleteMessageFolders request)
        {
            return base.Channel.deleteMessageFoldersAsync(request);
        }

        public Task<deleteMessageFoldersResponse> deleteMessageFoldersAsync(sessionHeader sessionHeader,
            messageFolderObject[] deleteMessageFolders1)
        {
            deleteMessageFolders inValue = new deleteMessageFolders();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteMessageFolders1 = deleteMessageFolders1;
            return ((BrontoSoapPortType) (this)).deleteMessageFoldersAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        addLoginsResponse BrontoSoapPortType.addLogins(addLogins request)
        {
            return base.Channel.addLogins(request);
        }

        public writeResult addLogins(sessionHeader sessionHeader, loginObject[] addLogins1)
        {
            addLogins inValue = new addLogins();
            inValue.sessionHeader = sessionHeader;
            inValue.addLogins1 = addLogins1;
            addLoginsResponse retVal = ((BrontoSoapPortType) (this)).addLogins(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<addLoginsResponse> BrontoSoapPortType.addLoginsAsync(addLogins request)
        {
            return base.Channel.addLoginsAsync(request);
        }

        public Task<addLoginsResponse> addLoginsAsync(sessionHeader sessionHeader, loginObject[] addLogins1)
        {
            addLogins inValue = new addLogins();
            inValue.sessionHeader = sessionHeader;
            inValue.addLogins1 = addLogins1;
            return ((BrontoSoapPortType) (this)).addLoginsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        updateContactsResponse BrontoSoapPortType.updateContacts(updateContacts request)
        {
            return base.Channel.updateContacts(request);
        }

        public writeResult updateContacts(sessionHeader sessionHeader, contactObject[] updateContacts1)
        {
            updateContacts inValue = new updateContacts();
            inValue.sessionHeader = sessionHeader;
            inValue.updateContacts1 = updateContacts1;
            updateContactsResponse retVal = ((BrontoSoapPortType) (this)).updateContacts(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<updateContactsResponse> BrontoSoapPortType.updateContactsAsync(updateContacts request)
        {
            return base.Channel.updateContactsAsync(request);
        }

        public Task<updateContactsResponse> updateContactsAsync(sessionHeader sessionHeader, contactObject[] updateContacts1)
        {
            updateContacts inValue = new updateContacts();
            inValue.sessionHeader = sessionHeader;
            inValue.updateContacts1 = updateContacts1;
            return ((BrontoSoapPortType) (this)).updateContactsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        readDeliveryGroupsResponse BrontoSoapPortType.readDeliveryGroups(readDeliveryGroups1 request)
        {
            return base.Channel.readDeliveryGroups(request);
        }

        public deliveryGroupObject[] readDeliveryGroups(sessionHeader sessionHeader, readDeliveryGroups readDeliveryGroups1)
        {
            readDeliveryGroups1 inValue = new readDeliveryGroups1();
            inValue.sessionHeader = sessionHeader;
            inValue.readDeliveryGroups = readDeliveryGroups1;
            readDeliveryGroupsResponse retVal = ((BrontoSoapPortType) (this)).readDeliveryGroups(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<readDeliveryGroupsResponse> BrontoSoapPortType.readDeliveryGroupsAsync(readDeliveryGroups1 request)
        {
            return base.Channel.readDeliveryGroupsAsync(request);
        }

        public Task<readDeliveryGroupsResponse> readDeliveryGroupsAsync(sessionHeader sessionHeader, readDeliveryGroups readDeliveryGroups)
        {
            readDeliveryGroups1 inValue = new readDeliveryGroups1();
            inValue.sessionHeader = sessionHeader;
            inValue.readDeliveryGroups = readDeliveryGroups;
            return ((BrontoSoapPortType) (this)).readDeliveryGroupsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        addToDeliveryGroupResponse BrontoSoapPortType.addToDeliveryGroup(addToDeliveryGroup1 request)
        {
            return base.Channel.addToDeliveryGroup(request);
        }

        public writeResult addToDeliveryGroup(sessionHeader sessionHeader, addToDeliveryGroup addToDeliveryGroup1)
        {
            addToDeliveryGroup1 inValue = new addToDeliveryGroup1();
            inValue.sessionHeader = sessionHeader;
            inValue.addToDeliveryGroup = addToDeliveryGroup1;
            addToDeliveryGroupResponse retVal = ((BrontoSoapPortType) (this)).addToDeliveryGroup(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<addToDeliveryGroupResponse> BrontoSoapPortType.addToDeliveryGroupAsync(addToDeliveryGroup1 request)
        {
            return base.Channel.addToDeliveryGroupAsync(request);
        }

        public Task<addToDeliveryGroupResponse> addToDeliveryGroupAsync(sessionHeader sessionHeader, addToDeliveryGroup addToDeliveryGroup)
        {
            addToDeliveryGroup1 inValue = new addToDeliveryGroup1();
            inValue.sessionHeader = sessionHeader;
            inValue.addToDeliveryGroup = addToDeliveryGroup;
            return ((BrontoSoapPortType) (this)).addToDeliveryGroupAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        addSMSDeliveriesResponse BrontoSoapPortType.addSMSDeliveries(addSMSDeliveries request)
        {
            return base.Channel.addSMSDeliveries(request);
        }

        public writeResult addSMSDeliveries(sessionHeader sessionHeader, smsDeliveryObject[] addSMSDeliveries1)
        {
            addSMSDeliveries inValue = new addSMSDeliveries();
            inValue.sessionHeader = sessionHeader;
            inValue.addSMSDeliveries1 = addSMSDeliveries1;
            addSMSDeliveriesResponse retVal = ((BrontoSoapPortType) (this)).addSMSDeliveries(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<addSMSDeliveriesResponse> BrontoSoapPortType.addSMSDeliveriesAsync(addSMSDeliveries request)
        {
            return base.Channel.addSMSDeliveriesAsync(request);
        }

        public Task<addSMSDeliveriesResponse> addSMSDeliveriesAsync(sessionHeader sessionHeader, smsDeliveryObject[] addSMSDeliveries1)
        {
            addSMSDeliveries inValue = new addSMSDeliveries();
            inValue.sessionHeader = sessionHeader;
            inValue.addSMSDeliveries1 = addSMSDeliveries1;
            return ((BrontoSoapPortType) (this)).addSMSDeliveriesAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        deleteSMSDeliveriesResponse BrontoSoapPortType.deleteSMSDeliveries(deleteSMSDeliveries request)
        {
            return base.Channel.deleteSMSDeliveries(request);
        }

        public writeResult deleteSMSDeliveries(sessionHeader sessionHeader, smsDeliveryObject[] deleteSMSDeliveries1)
        {
            deleteSMSDeliveries inValue = new deleteSMSDeliveries();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteSMSDeliveries1 = deleteSMSDeliveries1;
            deleteSMSDeliveriesResponse retVal = ((BrontoSoapPortType) (this)).deleteSMSDeliveries(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<deleteSMSDeliveriesResponse> BrontoSoapPortType.deleteSMSDeliveriesAsync(deleteSMSDeliveries request)
        {
            return base.Channel.deleteSMSDeliveriesAsync(request);
        }

        public Task<deleteSMSDeliveriesResponse> deleteSMSDeliveriesAsync(sessionHeader sessionHeader,
            smsDeliveryObject[] deleteSMSDeliveries1)
        {
            deleteSMSDeliveries inValue = new deleteSMSDeliveries();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteSMSDeliveries1 = deleteSMSDeliveries1;
            return ((BrontoSoapPortType) (this)).deleteSMSDeliveriesAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        deleteFieldsResponse BrontoSoapPortType.deleteFields(deleteFields request)
        {
            return base.Channel.deleteFields(request);
        }

        public writeResult deleteFields(sessionHeader sessionHeader, fieldObject[] deleteFields1)
        {
            deleteFields inValue = new deleteFields();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteFields1 = deleteFields1;
            deleteFieldsResponse retVal = ((BrontoSoapPortType) (this)).deleteFields(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<deleteFieldsResponse> BrontoSoapPortType.deleteFieldsAsync(deleteFields request)
        {
            return base.Channel.deleteFieldsAsync(request);
        }

        public Task<deleteFieldsResponse> deleteFieldsAsync(sessionHeader sessionHeader, fieldObject[] deleteFields1)
        {
            deleteFields inValue = new deleteFields();
            inValue.sessionHeader = sessionHeader;
            inValue.deleteFields1 = deleteFields1;
            return ((BrontoSoapPortType) (this)).deleteFieldsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        readSMSMessagesResponse BrontoSoapPortType.readSMSMessages(readSMSMessages1 request)
        {
            return base.Channel.readSMSMessages(request);
        }

        public smsMessageObject[] readSMSMessages(sessionHeader sessionHeader, readSMSMessages readSMSMessages1)
        {
            readSMSMessages1 inValue = new readSMSMessages1();
            inValue.sessionHeader = sessionHeader;
            inValue.readSMSMessages = readSMSMessages1;
            readSMSMessagesResponse retVal = ((BrontoSoapPortType) (this)).readSMSMessages(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<readSMSMessagesResponse> BrontoSoapPortType.readSMSMessagesAsync(readSMSMessages1 request)
        {
            return base.Channel.readSMSMessagesAsync(request);
        }

        public Task<readSMSMessagesResponse> readSMSMessagesAsync(sessionHeader sessionHeader, readSMSMessages readSMSMessages)
        {
            readSMSMessages1 inValue = new readSMSMessages1();
            inValue.sessionHeader = sessionHeader;
            inValue.readSMSMessages = readSMSMessages;
            return ((BrontoSoapPortType) (this)).readSMSMessagesAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        addApiTokensResponse BrontoSoapPortType.addApiTokens(addApiTokens request)
        {
            return base.Channel.addApiTokens(request);
        }

        public writeResult addApiTokens(sessionHeader sessionHeader, apiTokenObject[] addApiTokens1)
        {
            addApiTokens inValue = new addApiTokens();
            inValue.sessionHeader = sessionHeader;
            inValue.addApiTokens1 = addApiTokens1;
            addApiTokensResponse retVal = ((BrontoSoapPortType) (this)).addApiTokens(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<addApiTokensResponse> BrontoSoapPortType.addApiTokensAsync(addApiTokens request)
        {
            return base.Channel.addApiTokensAsync(request);
        }

        public Task<addApiTokensResponse> addApiTokensAsync(sessionHeader sessionHeader, apiTokenObject[] addApiTokens1)
        {
            addApiTokens inValue = new addApiTokens();
            inValue.sessionHeader = sessionHeader;
            inValue.addApiTokens1 = addApiTokens1;
            return ((BrontoSoapPortType) (this)).addApiTokensAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        updateLoginsResponse BrontoSoapPortType.updateLogins(updateLogins request)
        {
            return base.Channel.updateLogins(request);
        }

        public writeResult updateLogins(sessionHeader sessionHeader, loginObject[] updateLogins1)
        {
            updateLogins inValue = new updateLogins();
            inValue.sessionHeader = sessionHeader;
            inValue.updateLogins1 = updateLogins1;
            updateLoginsResponse retVal = ((BrontoSoapPortType) (this)).updateLogins(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<updateLoginsResponse> BrontoSoapPortType.updateLoginsAsync(updateLogins request)
        {
            return base.Channel.updateLoginsAsync(request);
        }

        public Task<updateLoginsResponse> updateLoginsAsync(sessionHeader sessionHeader, loginObject[] updateLogins1)
        {
            updateLogins inValue = new updateLogins();
            inValue.sessionHeader = sessionHeader;
            inValue.updateLogins1 = updateLogins1;
            return ((BrontoSoapPortType) (this)).updateLoginsAsync(inValue);
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        readWebformsResponse BrontoSoapPortType.readWebforms(readWebforms1 request)
        {
            return base.Channel.readWebforms(request);
        }

        public webformObject[] readWebforms(sessionHeader sessionHeader, readWebforms readWebforms1)
        {
            readWebforms1 inValue = new readWebforms1();
            inValue.sessionHeader = sessionHeader;
            inValue.readWebforms = readWebforms1;
            readWebformsResponse retVal = ((BrontoSoapPortType) (this)).readWebforms(inValue);
            return retVal.@return;
        }

        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Advanced)]
        Task<readWebformsResponse> BrontoSoapPortType.readWebformsAsync(readWebforms1 request)
        {
            return base.Channel.readWebformsAsync(request);
        }

        public Task<readWebformsResponse> readWebformsAsync(sessionHeader sessionHeader, readWebforms readWebforms)
        {
            readWebforms1 inValue = new readWebforms1();
            inValue.sessionHeader = sessionHeader;
            inValue.readWebforms = readWebforms;
            return ((BrontoSoapPortType) (this)).readWebformsAsync(inValue);
        }
    }
}