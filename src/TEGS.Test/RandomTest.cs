// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TEGS.Test
{
    [TestClass]
    public class RandomTest
    {
        [TestMethod]
        public void Random_UniformVariate_IsDeterminateTest()
        {
            VariateIsDeterminateTest(r => r.UniformVariate(0, 1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Random_ExponentialVariate_ZeroLambdaTest()
        {
            new Random(12345).ExponentialVariate(0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Random_ExponentialVariate_NegativeLambdaTest()
        {
            new Random(12345).ExponentialVariate(-1);
        }

        [TestMethod]
        public void Random_ExponentialVariate_IsDeterminateTest()
        {
            VariateIsDeterminateTest(r => r.ExponentialVariate(5));
        }

        [TestMethod]
        public void Random_NormalVariate_IsDeterminateTest()
        {
            VariateIsDeterminateTest(r => r.NormalVariate(5, 1));
        }

        [TestMethod]
        public void Random_LogNormalVariate_IsDeterminateTest()
        {
            VariateIsDeterminateTest(r => r.LogNormalVariate(5, 1));
        }

        [TestMethod]
        public void Random_TriangularVariate_IsDeterminateTest()
        {
            VariateIsDeterminateTest(r => r.TriangularVariate(0, 1, 0.5));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Random_GammaVariate_ZeroAlphaTest()
        {
            new Random(12345).GammaVariate(0, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Random_GammaVariate_ZeroBetaTest()
        {
            new Random(12345).GammaVariate(1, 0);
        }

        [TestMethod]
        public void Random_GammaVariate_IsDeterminateTest()
        {
            VariateIsDeterminateTest(r => r.GammaVariate(1, 1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Random_BetaVariate_ZeroAlphaTest()
        {
            new Random(12345).BetaVariate(0, 1);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Random_BetaVariate_ZeroBetaTest()
        {
            new Random(12345).BetaVariate(1, 0);
        }

        [TestMethod]
        public void Random_BetaVariate_IsDeterminateTest()
        {
            VariateIsDeterminateTest(r => r.BetaVariate(1, 1));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Random_ErlangVariate_ZeroKTest()
        {
            new Random(12345).ErlangVariate(0, 5);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Random_ErlangVariate_NegativeKTest()
        {
            new Random(12345).ErlangVariate(-1, 5);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Random_ErlangVariate_ZeroLambdaTest()
        {
            new Random(12345).ErlangVariate(5, 0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Random_ErlangVariate_NegativeLambdaTest()
        {
            new Random(12345).ErlangVariate(5, -1);
        }

        [TestMethod]
        public void Random_ErlangVariate_IsDeterminateTest()
        {
            VariateIsDeterminateTest(r => r.ErlangVariate(5, 5));
        }

        private static void VariateIsDeterminateTest(Func<Random, double> variateFunc)
        {
            Random r1 = new Random(12345);
            Random r2 = new Random(12345);

            for (int i = 0; i < 1000; i++)
            {
                double d1 = variateFunc(r1);
                double d2 = variateFunc(r2);
                Assert.AreEqual(d1, d2);
            }
        }
    }
}
