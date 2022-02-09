// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;

namespace TEGS
{
    public static class RandomExtensions
    {
        /// <summary>
        /// Generates a uniformly distributed random number.
        /// See: https://en.wikipedia.org/wiki/Continuous_uniform_distribution
        /// </summary>
        /// <param name="random">The Random.</param>
        /// <param name="a">The minimum of the distribution.</param>
        /// <param name="b">The maximum of the distribution.</param>
        /// <returns>A uniformly distributed random number.</returns>
        public static double UniformVariate(this Random random, double a, double b)
        {
            if (b <= a)
            {
                throw new ArgumentOutOfRangeException(nameof(b));
            }

            return a + (b - a) * random.NextDouble();
        }

        /// <summary>
        /// Generates an exponentially distributed random number.
        /// See: https://en.wikipedia.org/wiki/Exponential_distribution
        /// </summary>
        /// <param name="random">The Random.</param>
        /// <param name="lambda">The rate of the distribution.</param>
        /// <returns>An exponentially distributed random number.</returns>
        public static double ExponentialVariate(this Random random, double lambda)
        {
            if (lambda <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(lambda));
            }

            return -Math.Log(1.0 - random.NextDouble()) / lambda;
        }

        /// <summary>
        /// Generates a normally distributed random number.
        /// See: https://en.wikipedia.org/wiki/Normal_distribution
        /// </summary>
        /// <param name="random">The Random.</param>
        /// <param name="mu">The mean of the distribution.</param>
        /// <param name="sigma">The standard deviation of the distribution.</param>
        /// <returns>A normally distributed random number.</returns>
        public static double NormalVariate(this Random random, double mu, double sigma)
        {
            if (sigma < 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(sigma));
            }

            double z;
            while (true)
            {
                double u1 = random.NextDouble();
                double u2 = 1.0 - random.NextDouble();

                z = (4 * Math.Exp(-0.5) / Math.Sqrt(2.0)) * (u1 - 0.5) / u2;

                if ((z * z / 4.0) <= -Math.Log(u2))
                {
                    break;
                }
            }

            return mu + z * sigma;
        }

        /// <summary>
        /// Generates a log-normally distributed random number.
        /// See: https://en.wikipedia.org/wiki/Log-normal_distribution
        /// </summary>
        /// <param name="random">The Random.</param>
        /// <param name="mu">The mean of the distribution.</param>
        /// <param name="sigma">The standard deviation of the distribution.</param>
        /// <returns>A log-normally distributed random number.</returns>
        public static double LogNormalVariate(this Random random, double mu, double sigma)
        {
            if (sigma < 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(sigma));
            }

            return Math.Exp(random.NormalVariate(mu, sigma));
        }

        /// <summary>
        /// Generates a triangularly distributed random number.
        /// See: https://en.wikipedia.org/wiki/Triangular_distribution
        /// </summary>
        /// <param name="random">The Random.</param>
        /// <param name="a">The minimum of the distribution.</param>
        /// <param name="b">The maximum of the distribution.</param>
        /// <param name="c">The mode of the distribution.</param>
        /// <returns>A triangularly distributed random number.</returns>
        public static double TriangularVariate(this Random random, double a, double b, double c)
        {
            if (b <= a)
            {
                throw new ArgumentOutOfRangeException(nameof(b));
            }

            if (c < a || c > b)
            {
                throw new ArgumentOutOfRangeException(nameof(c));
            }

            double u = random.NextDouble();
            double fc = (c - a) / (b - a);

            if (u < fc)
            {
                return a + Math.Sqrt(u * (b - a) * (c - a));
            }
            else
            {
                return b - Math.Sqrt((1.0 - u) * (b - a) * (b - c));
            }
        }

        /// <summary>
        /// Generates a gamma distributed random number.
        /// See: https://en.wikipedia.org/wiki/Gamma_distribution
        /// </summary>
        /// <param name="random">The Random.</param>
        /// <param name="k">The shape of the distribution.</param>
        /// <param name="sigma">The scale of the distribution.</param>
        /// <returns>A gamma distributed random number.</returns>
        public static double GammaVariate(this Random random, double k, double sigma)
        {
            if (k <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(k));
            }

            if (sigma <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(sigma));
            }

            if (k > 1.0)
            {
                double ainv = Math.Sqrt(2.0 * k - 1.0);
                double bbb = k - Math.Log(4.0);
                double ccc = k + ainv;

                while (true)
                {
                    double u1 = random.NextDouble();
                    if (!(1E-7 < u1 && u1 < 0.9999999))
                    {
                        continue;
                    }

                    double u2 = 1.0 - random.NextDouble();
                    double v = Math.Log(u1 / (1.0 - u1)) / ainv;
                    double x = k * Math.Exp(v);
                    double z = u1 * u1 * u2;
                    double r = bbb + ccc * v - x;

                    if ((r + (1.0 + Math.Log(4.5)) - 4.5 * z >= 0.0) || r >= Math.Log(z))
                    {
                        return x * sigma;
                    }
                }
            }
            else if (k == 1.0)
            {
                return -Math.Log(1.0 - random.NextDouble()) * sigma;
            }
            else
            {
                double x;
                while (true)
                {
                    double u = random.NextDouble();
                    double b = (Math.E + k) / Math.E;
                    double p = b * u;

                    if (p <= 1.0)
                    {
                        x = Math.Pow(p, 1.0 / k);
                    }
                    else
                    {
                        x = -Math.Log((b - p) / k);
                    }

                    double u1 = random.NextDouble();
                    if (p > 1.0)
                    {
                        if (u1 <= Math.Pow(x, k - 1.0))
                        {
                            break;
                        }
                        else if (u1 <= Math.Exp(-x))
                        {
                            break;
                        }
                    }
                }

                return x * sigma;
            }
        }

        /// <summary>
        /// Generates a beta distributed random number.
        /// See: https://en.wikipedia.org/wiki/Beta_distribution
        /// </summary>
        /// <param name="random">The Random.</param>
        /// <param name="alpha">The alpha shape of the distribution.</param>
        /// <param name="beta">The beta shape of the distribution.</param>
        /// <returns>A beta distributed random number.</returns>
        public static double BetaVariate(this Random random, double alpha, double beta)
        {
            if (alpha <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(alpha));
            }

            if (beta <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(beta));
            }

            double y = random.GammaVariate(alpha, 1.0);
            if (y != 0.0)
            {
                return y / (y + random.GammaVariate(beta, 1.0));
            }

            return 0.0;
        }

        /// <summary>
        /// Generates an Erlang distributed random number.
        /// See: https://en.wikipedia.org/wiki/Erlang_distribution
        /// </summary>
        /// <param name="random">The Random.</param>
        /// <param name="k">The shape of the distribution.</param>
        /// <param name="lambda">The rate of the distribution.</param>
        /// <returns>An Erlang distributed random number.</returns>
        public static double ErlangVariate(this Random random, int k, double lambda)
        {
            if (k < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(k));
            }

            if (lambda <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(lambda));
            }

            double t = 1.0;
            for (int i = 0; i < k; i++)
            {
                t *= 1.0 - random.NextDouble();
            }

            return -Math.Log(t) / lambda;
        }
    }
}
