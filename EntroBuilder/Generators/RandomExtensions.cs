using System;

namespace EntroBuilder
{
    public static class RandomExtensions
    {
        public static sbyte NextBetween(this Random random, sbyte min, sbyte max)
        {
            return (sbyte)NextBetween(random, (ulong)min, (ulong)max);
        }
        public static byte NextBetween(this Random random, byte min, byte max)
        {
            return (byte)NextBetween(random, (ulong)min, (ulong)max);
        }

        public static short NextBetween(this Random random, short min, short max)
        {
            return (short)NextBetween(random, (ulong)min, (ulong)max);
        }
        public static ushort NextBetween(this Random random, ushort min, ushort max)
        {
            return (ushort)NextBetween(random, (ulong)min, (ulong)max);
        }

        public static int NextBetween(this Random random, int min, int max)
        {
            return (int)NextBetween(random, (ulong)min, (ulong)max);
        }
        public static uint NextBetween(this Random random, uint min, uint max)
        {
            return (uint)NextBetween(random, (ulong)min, (ulong)max);
        }

        public static long NextBetween(this Random random, long min, long max)
        {
            return (long)NextBetween(random, (ulong)min, (ulong)max);
        }
        public static ulong NextBetween(this Random random, ulong min, ulong max)
        {
            ulong range = (max - min);
            if (range == ulong.MaxValue)
            {
                byte[] buf = new byte[8];
                random.NextBytes(buf);
                return (ulong)BitConverter.ToInt64(buf, 0);
            }

            ulong inclusiveRange = range + 1;
            ulong cutoff, candidate;
            do
            {
                byte[] buf = new byte[8];
                random.NextBytes(buf);
                candidate = (ulong)BitConverter.ToInt64(buf, 0);
                cutoff = ulong.MaxValue - ((ulong.MaxValue % inclusiveRange) + 1) % inclusiveRange;
            } while (candidate > cutoff);

            return (candidate % inclusiveRange) + min;
        }

        public static float NextBetween(this Random random, float min, float max)
        {
            return (float)NextBetween(random, (double)min, (double)max);
        }
        public static double NextBetween(this Random random, double min, double max)
        {
            double range = Math.Abs(max - min);
            double delta = random.NextDouble() * range;
            double result = min + delta;
            return result;
        }

        public static decimal NextBetween(this Random random, decimal min, decimal max)
        {
            decimal upperCutoff, lowerCutoff, candidate;
            do
            {
                candidate = random.NextDecimal();
                upperCutoff = max == 0 ? 0 : decimal.MaxValue - ((decimal.MaxValue % max) + 1) % max;
                lowerCutoff = min == 0 ? 0 : decimal.MinValue - ((decimal.MinValue % min) - 1) % min;
            } while (candidate > upperCutoff || candidate < lowerCutoff);

            if (candidate > min && candidate > 0)
            {
                return (candidate % max);
            }
            return (candidate % min);
        }
        public static int NextInt32(this Random random)
        {
            unchecked
            {
                int firstBits = random.Next(0, 1 << 4) << 28;
                int lastBits = random.Next(0, 1 << 28);
                return firstBits | lastBits;
            }
        }
        public static decimal NextDecimal(this Random random)
        {
            byte scale = (byte)random.Next(29);
            bool sign = random.Next(2) == 1;
            return new decimal(random.NextInt32(),
                               random.NextInt32(),
                               random.NextInt32(),
                               sign,
                               scale);
        }
    }
}
