using ShireBank.Shared.Utils.Sqlite;
using Xunit;

namespace ShireBank.Shared.Tests.Utils.Sqlite
{
    public class SqliteTimestampConverterTests
    {
        [Fact]
        public void FromDbWorks()
        {
            string input = "test";
            byte[] output = {
                116, 101, 115, 116
            };

            var converter = new SqliteTimestampConverter()
                .ConvertFromProviderExpression
                .Compile();

            Assert.Equal(converter.Invoke(input), output);
        }

        [Fact]
        public void ToDbWorks()
        {
            byte[] input = {
                116, 101, 115, 116
            };
            string output = "test";

            var converter = new SqliteTimestampConverter()
                .ConvertToProviderExpression
                .Compile();

            Assert.Equal(converter.Invoke(input), output);
        }
    }
}
