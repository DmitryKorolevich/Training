﻿namespace VitalChoice.Infrastructure.Domain.Constants
{
    public class ProMailConstants
    {
        public const string ShipPattern = @"^ShipAck[0-9]+.*?\.xml$";
        public const string CancelPattern = @"^CancelAck[0-9]+\.xml$";
    }
}