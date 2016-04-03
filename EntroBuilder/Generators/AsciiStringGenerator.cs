using System;
using System.Linq;

namespace EntroBuilder
{
    public class AsciiStringGenerator : ScalarGenerator<string>
    {
        protected override string NextImpl(Random random)
        {
            var lengthRanges = new[] {8, 64, 256};
            var maxLength = lengthRanges[random.Next(3)];
            var length = random.Next(maxLength);
            var charGenerator = new AsciiCharGenerator();
            return new string(Enumerable.Range(0, length).Select(i => charGenerator.Next(random)).ToArray());
        }
    }
}