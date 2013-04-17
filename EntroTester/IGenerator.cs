using System;

namespace EntroTester
{
    public interface IGenerator<out T> : IGenerator
    {
        new T Next(Random random);
    }

    public interface IGenerator
    {
        object Next(Random random);
    }
}