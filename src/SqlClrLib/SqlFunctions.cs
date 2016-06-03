using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace SqlClrLib
{
    public static class SqlFunctions
    {
        private static readonly PasswordHasher Hasher = new PasswordHasher();

        [SqlFunction]
        public static SqlString HashIdentityPassword(SqlString input)
        {
            return Hasher.HashPassword(input.Value);
        }
    }
}