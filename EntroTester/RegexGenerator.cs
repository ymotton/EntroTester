using System;
using Fare;

namespace EntroTester
{
    /// <summary>
    /// See https://github.com/moodmosaic/Fare
    /// </summary>
    public class RegexGenerator : IGenerator<string>
    {
        readonly Xeger _xeger;

        public RegexGenerator(string pattern)
        {
            _xeger = new Xeger(pattern);
        }

        public string Next(Random random)
        {
            string value = _xeger.Generate();
            return value;
        }
        object IGenerator.Next(Random random)
        {
            return Next(random);
        }
    }
}