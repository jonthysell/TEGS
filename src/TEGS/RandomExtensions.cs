// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;

namespace TEGS
{
    public static class RandomExtensions
    {
        public static double UniformVariate(this Random random, double alpha, double beta)
        {
            return alpha + (beta - alpha) * random.NextDouble();
        }

        public static double ExponentialVariate(this Random random, double lambda)
        {
            if (lambda == 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(lambda));
            }

            return -Math.Log(1.0 - random.NextDouble()) / lambda;
        }

        public static double NormalVariate(this Random random, double mu, double sigma)
        {
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

        public static double LogNormalVariate(this Random random, double mu, double sigma)
        {
            return Math.Exp(random.NormalVariate(mu, sigma));
        }

        public static double TriangularVariate(this Random random, double low, double high, double mode)
        {
            double u = random.NextDouble();
            if (high == low)
            {
                return low;
            }

            double c = (mode - low) / (high - low);
            if (u > c)
            {
                u = 1.0 - u;
                c = 1.0 - c;
                double h = high;
                high = low;
                low = h;
            }

            return low + (high - low) * Math.Sqrt(u * c);
        }

        public static double GammaVariate(this Random random, double alpha, double beta)
        {
            if (alpha <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(alpha));
            }

            if (beta <= 0.0)
            {
                throw new ArgumentOutOfRangeException(nameof(beta));
            }

            if (alpha > 1.0)
            {
                double ainv = Math.Sqrt(2.0 * alpha - 1.0);
                double bbb = alpha - Math.Log(4.0);
                double ccc = alpha + ainv;

                while (true)
                {
                    double u1 = random.NextDouble();
                    if (!(1E-7 < u1 && u1 < 0.9999999))
                    {
                        continue;
                    }

                    double u2 = 1.0 - random.NextDouble();
                    double v = Math.Log(u1 / (1.0 - u1)) / ainv;
                    double x = alpha * Math.Exp(v);
                    double z = u1 * u1 * u2;
                    double r = bbb + ccc * v - x;

                    if ((r + (1.0 + Math.Log(4.5)) - 4.5 * z >= 0.0) || r >= Math.Log(z))
                    {
                        return x * beta;
                    }
                }
            }
            else if (alpha == 1.0)
            {
                return -Math.Log(1.0 - random.NextDouble()) * beta;
            }
            else
            {
                double x;
                while (true)
                {
                    double u = random.NextDouble();
                    double b = (Math.E + alpha) / Math.E;
                    double p = b * u;

                    if (p <= 1.0)
                    {
                        x = Math.Pow(p, 1.0 / alpha);
                    }
                    else
                    {
                        x = -Math.Log((b - p) / alpha);
                    }

                    double u1 = random.NextDouble();
                    if (p > 1.0)
                    {
                        if (u1 <= Math.Pow(x, alpha - 1.0))
                        {
                            break;
                        }
                        else if (u1 <= Math.Exp(-x))
                        {
                            break;
                        }
                    }
                }

                return x * beta;
            }
        }

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
    }
}
