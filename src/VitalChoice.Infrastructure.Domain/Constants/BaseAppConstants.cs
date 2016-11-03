namespace VitalChoice.Infrastructure.Domain.Constants
{
    public class BaseAppConstants
    {
        public const int CREDIT_CARD_MAX_LENGTH = 16;
        public const int CREDIT_CARD_AM_EXPRESS_MAX_LENGTH = 15;
        public const int DEFAULT_LIST_TAKE_COUNT = 100;
        public const int DEFAULT_AUTO_COMPLETE_TAKE_COUNT = 20;
        public const int DEFAULT_TEXT_FIELD_MAX_SIZE = 100;
        public const int ZIP_MAX_SIZE = 10;
        public const int DEFAULT_TEXTAREA_FIELD_MAX_SIZE = 250;
        public const int DELIVERY_INSTRUCTIONS_MAX_SIZE = 60;
        public const int DEFAULT_BIG_TEXT_FIELD_MAX_SIZE = 1000;
        public const int DEFAULT_EMAIL_FIELD_MAX_SIZE = 1000;
        public const int STATE_COUNTRY_CODE_MAX_SIZE = 3;
        public const int DEFAULT_MAX_ALLOWED_PARAMS_SQL = 100;
        public const string DEFAULT_FORM_FIELD_NAME = "Field";
        public const string FAKE_CUSTOMER_EMAIL= "";
        public const int DEFAULT_TOKEN_EXPIRED_MINUTES = 60;
        public const string ORDER_INVOICE_PAGE_URL_TEMPLATE = "https://{0}/api/orderinvoice/pdf/{1}";
        public const string PDF_URL_GENERATE_ORDER_INVOICE_TEMPLATE = "{0}?license={1}url={2}&page_size=Letter&orientation=portrait&filename=Vital+Choice+Seafood+Invoice-{3}.pdf";
        public const string BASE_PHONE_FORMAT = "(___) ___-____? x_____";
        public const string ADMIN_EDIT_LOCK_AREAS = "index.oneCol.productDetail,index.oneCol.customerDetail,index.oneCol.orderDetail,index.oneCol.orderReshipDetail,index.oneCol.orderRefundDetail";
    }
}