namespace VitalChoice.Infrastructure.Domain.Constants
{
    public class VeraCoreConstants
    {
        public const string ShipPattern = @"^ShipAck[0-9]+.*?\.xml$";
        public const string CancelPattern = @"^CancelAck[0-9]+.*?\.xml$";

        public const int MAX_PARSING_ATTEMPT = 3;
    }
}