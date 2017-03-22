using System;
using System.Collections.Generic;
using System.Linq;

namespace EntroBuilder.Generators
{
    public class CompoundGenerator<T> : IGenerator<T>
    {
        private readonly double _totalProbability;
        private readonly List<Tuple<IGenerator<T>, double>> _generators;

        public CompoundGenerator(CompoundGenerator<T> compoundGenerator, IGenerator<T> generator) 
            : this(compoundGenerator, Tuple.Create(generator, 1.0)) { }
        public CompoundGenerator(CompoundGenerator<T> compoundGenerator, Tuple<IGenerator<T>, double> generator) 
            : this(compoundGenerator._generators.Concat(new[] { generator })) { }
        public CompoundGenerator(IEnumerable<IGenerator<T>> generators) 
            : this(generators.Select(g => Tuple.Create(g, 1.0))) { }
        public CompoundGenerator(IEnumerable<Tuple<IGenerator<T>, double>> generators)
        {
            _generators = generators.ToList();
            if (_generators.Count == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(generators), "Expecting at least one generator.");
            }
            if (_generators.Select(x => x.Item2).Any(x => x <= 0.0))
            {
                throw new ArgumentOutOfRangeException(nameof(generators), "Expecting probabilities to be strictly positive.");
            }
            _totalProbability = _generators.Select(x => x.Item2).Sum();
        }

        public T Next(Random random)
        {
            double p = random.NextDouble() * _totalProbability;
            double cumulativeP = 0.0;
            for (int i = 0; i < _generators.Count; i++)
            {
                cumulativeP += _generators[i].Item2;
                if (p < cumulativeP)
                {
                    return _generators[i].Item1.Next(random);
                }
            }
            return _generators.Last().Item1.Next(random);
        }

        object IGenerator.Next(Random random)
        {
            return Next(random);
        }
    }
}
