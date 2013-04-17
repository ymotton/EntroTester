namespace EntroTester
{
    public static class Is
    {
        public static SequenceGenerator<T> Value<T>(T value)
        {
            return new SequenceGenerator<T>(new[] { value });
        }
    }
}