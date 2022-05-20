using System.Linq;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ShireBank.Shared.Data;

internal class SqliteTimestampConverter : ValueConverter<byte[], string>
{
    public SqliteTimestampConverter() : base(
        v => v == null ? null : ToDb(v),
        v => v == null ? null : FromDb(v))
    {
    }

    private static byte[] FromDb(string v)
    {
        return v.Select(c => (byte)c).ToArray();
    }

    private static string ToDb(byte[] v)
    {
        return new(v.Select(b => (char)b).ToArray());
    }
}