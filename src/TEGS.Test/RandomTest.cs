﻿// Copyright (c) Jon Thysell <http://jonthysell.com>
// Licensed under the MIT License.

using System;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TEGS.Test
{
    [TestClass]
    public class RandomTest
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Random_UniformVariate_BLessThanALambdaTest()
        {
            new Random(12345).UniformVariate(1.0, 0.0);
        }

        [TestMethod]
        public void Random_UniformVariate_IsDeterminateTest()
        {
            VariateIsDeterminateTest(r => r.UniformVariate(0.0, 1.0));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Random_ExponentialVariate_ZeroLambdaTest()
        {
            new Random(12345).ExponentialVariate(0.0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Random_ExponentialVariate_NegativeLambdaTest()
        {
            new Random(12345).ExponentialVariate(-1.0);
        }

        [TestMethod]
        public void Random_ExponentialVariate_IsDeterminateTest()
        {
            VariateIsDeterminateTest(r => r.ExponentialVariate(5.0));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Random_NormalVariate_NegativeSigmaTest()
        {
            new Random(12345).NormalVariate(0.0, -1.0);
        }

        [TestMethod]
        public void Random_NormalVariate_IsDeterminateTest()
        {
            VariateIsDeterminateTest(r => r.NormalVariate(0.0, 1.0));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Random_LogNormalVariate_NegativeSigmaTest()
        {
            new Random(12345).LogNormalVariate(0.0, -1.0);
        }

        [TestMethod]
        public void Random_LogNormalVariate_IsDeterminateTest()
        {
            VariateIsDeterminateTest(r => r.LogNormalVariate(0.0, 1.0));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Random_TriangulaVariate_BLessThanALambdaTest()
        {
            new Random(12345).TriangularVariate(0.0, -1.0, -0.5);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Random_TriangulaVariate_CLessThanALambdaTest()
        {
            new Random(12345).TriangularVariate(0.0, 1.0, -1.0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Random_TriangulaVariate_CGreaterThanBLambdaTest()
        {
            new Random(12345).TriangularVariate(0.0, 1.0, 2.0);
        }

        [TestMethod]
        public void Random_TriangularVariate_IsDeterminateTest()
        {
            VariateIsDeterminateTest(r => r.TriangularVariate(0.0, 1.0, 0.5));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Random_GammaVariate_ZeroKTest()
        {
            new Random(12345).GammaVariate(0.0, 2.0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Random_GammaVariate_NegativeKTest()
        {
            new Random(12345).GammaVariate(-1.0, 2.0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Random_GammaVariate_ZeroSigmaTest()
        {
            new Random(12345).GammaVariate(1.0, 0.0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Random_GammaVariate_NegativeSigmaTest()
        {
            new Random(12345).GammaVariate(1.0, -1.0);
        }

        [TestMethod]
        public void Random_GammaVariate_IsDeterminateTest()
        {
            VariateIsDeterminateTest(r => r.GammaVariate(1.0, 2.0));
            VariateIsDeterminateTest(r => r.GammaVariate(2.0, 2.0));
            VariateIsDeterminateTest(r => r.GammaVariate(0.5, 2.0));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Random_BetaVariate_ZeroAlphaTest()
        {
            new Random(12345).BetaVariate(0.0, 1.0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Random_BetaVariate_NegativeAlphaTest()
        {
            new Random(12345).BetaVariate(-1.0, 1.0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Random_BetaVariate_ZeroBetaTest()
        {
            new Random(12345).BetaVariate(1.0, 0.0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Random_BetaVariate_NegativeBetaTest()
        {
            new Random(12345).BetaVariate(1.0, -1.0);
        }

        [TestMethod]
        public void Random_BetaVariate_IsDeterminateTest()
        {
            VariateIsDeterminateTest(r => r.BetaVariate(1.0, 1.0));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Random_ErlangVariate_ZeroKTest()
        {
            new Random(12345).ErlangVariate(0, 5.0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Random_ErlangVariate_NegativeKTest()
        {
            new Random(12345).ErlangVariate(-1, 5.0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Random_ErlangVariate_ZeroLambdaTest()
        {
            new Random(12345).ErlangVariate(5, 0.0);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void Random_ErlangVariate_NegativeLambdaTest()
        {
            new Random(12345).ErlangVariate(5, -1.0);
        }

        [TestMethod]
        public void Random_ErlangVariate_IsDeterminateTest()
        {
            VariateIsDeterminateTest(r => r.ErlangVariate(5, 5.0));
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
