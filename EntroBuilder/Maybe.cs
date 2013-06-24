namespace EntroBuilder
{
    internal class Maybe<T>
    {
        public bool HasValue { get; private set; }
        public T Value { get; private set; }
        public Maybe() { HasValue = false; }
        public Maybe(T value)
        {
            Value = value;
            HasValue = true;
        }
    }
}