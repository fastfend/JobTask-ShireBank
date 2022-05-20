using ShireBank.Shared.Types;
using Xunit;

namespace ShireBank.Shared.Tests.Types
{
    public class DecimalValueTests
    {
        [Fact]
        public void CastToDecimalType()
        {
            decimal input = 10.25m;
            DecimalValue value = new DecimalValue(10, 250000000);

            decimal output = value;

            Assert.Equal(input, output);
        }

        [Fact]
        public void CastFromDecimalType()
        {
            DecimalValue input = new DecimalValue(10, 250000000);
            decimal value = 10.25m;

            DecimalValue output = value;

            Assert.Equal(input, output);
        }
    }
}
