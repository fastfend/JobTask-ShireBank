using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ShireBank.Shared.Utils.Sqlite;

public class SqliteTimestampConverter : ValueConverter<byte[], string>
{
    public SqliteTimestampConverter() : base(
        v => v == null ? null : ToDb(v),
        v => v == null ? null : FromDb(v))
    { }

    private static byte[] FromDb(string v)
    {
        return v.Select(c => (byte)c).ToArray();
    }

    private static string ToDb(IEnumerable<byte> v)
    {
        return new string(v.Select(b => (char)b).ToArray());
    }
}