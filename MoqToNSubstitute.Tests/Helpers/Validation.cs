namespace MoqToNSubstitute.Tests.Helpers
{
    /// <summary>
    /// Unit test validation methods
    /// </summary>
    public static class Validation
    {
        /// <summary>
        /// Takes 2 objects of type T and makes sure the ValueTypes are equal
        /// </summary>
        /// <typeparam name="T">The type of object to compare</typeparam>
        /// <param name="expected">The object with the expected values</param>
        /// <param name="actual">The object with the actual values</param>
        /// <returns>True if the objects are the same, false otherwise</returns>
        public static bool Validate<T>(T expected, T actual) where T : class?
        {
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var expectedValue = property.GetValue(expected);
                var actualValue = property.GetValue(actual);

                if (expectedValue != null && !expectedValue.GetType().IsValueType)
                {
                    Validate(expectedValue, actualValue);
                }
                else
                {
                    Assert.AreEqual(expectedValue, actualValue);
                }
            }

            return true;
        }

    }
}
