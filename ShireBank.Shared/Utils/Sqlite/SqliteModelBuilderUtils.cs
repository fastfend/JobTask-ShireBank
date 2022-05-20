using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace ShireBank.Shared.Utils.Sqlite
{
    internal static class SqliteModelBuilderUtils
    {
        /// <summary>
        /// Fixes issues with DbUpdateConcurrencyException when using SQLite
        /// <see href="https://stackoverflow.com/questions/52684458/updating-entity-in-ef-core-application-with-sqlite-gives-dbupdateconcurrencyexce">See more</see>
        /// </summary>
        /// <param name="modelBuilder"></param>
        /// <param name="context"></param>
        public static void AddSqliteCompatibility(this ModelBuilder modelBuilder, DbContext context)
        {
            if (!context.Database.IsSqlite()) return;

            var timestampProperties = modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(t => t.GetProperties())
                .Where(p => p.ClrType == typeof(byte[])
                            && p.ValueGenerated == ValueGenerated.OnAddOrUpdate
                            && p.IsConcurrencyToken);

            foreach (var property in timestampProperties)
            {
                property.SetColumnType("BLOB");
                property.SetDefaultValueSql("CURRENT_TIMESTAMP");
                property.SetValueConverter(new SqliteTimestampConverter());
                property.SetValueComparer(new ValueComparer<byte[]>(
                    (c1, c2) => c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToArray()));
            }
        }
    }
}
