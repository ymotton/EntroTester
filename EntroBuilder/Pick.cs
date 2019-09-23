using EntroBuilder.Generators;

namespace EntroBuilder
{
    public static class Pick
    {
        public static IGenerator<T> ValueIn<T>(params T[] possibleValues)
        {
            var generator = new BagGenerator<T>(possibleValues);
            return generator;
        }
    }
}
