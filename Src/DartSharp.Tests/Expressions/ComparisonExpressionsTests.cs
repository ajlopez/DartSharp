namespace DartSharp.Tests
{
    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using DartSharp.Commands;
    using DartSharp.Expressions;
    using DartSharp.Language;

    [TestClass]
    public class ComparisonExpressionsTests
    {
        [TestMethod]
        public void EvaluateEqualOperator()
        {
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.Equal, null, null));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.Equal, true, true));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.Equal, false, false));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.Equal, 1, 1));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.Equal, 1.2, 1.0 + 0.2));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.Equal, "foo", "foo"));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.Equal, 2, "2"));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.Equal, "foo", "sfoo".Substring(1)));

            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Equal, true, false));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Equal, 2, 1));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Equal, 3.14, 2.12));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Equal, "foo", "bar"));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Equal, "foo", "Foo"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void RaiseIfNotComparableObject()
        {
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Equal, new Context(), new Context()));
        }

        [TestMethod]
        public void EvaluateNotEqualOperator()
        {
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.NotEqual, null, null));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.NotEqual, true, true));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.NotEqual, false, false));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.NotEqual, 1, 1));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.NotEqual, 1.2, 1.0 + 0.2));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.NotEqual, "foo", "foo"));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.NotEqual, 2, "2"));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.NotEqual, "foo", "sfoo".Substring(1)));

            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.NotEqual, true, false));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.NotEqual, 2, 1));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.NotEqual, 3.14, 2.12));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.NotEqual, "foo", "bar"));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.NotEqual, "foo", "Foo"));
        }

        [TestMethod]
        public void EvaluateLessOperator()
        {
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.Less, 1, 2));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.Less, "bar", "foo"));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.Less, 1.2, 3.4));

            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Less, 1, 1));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Less, 2, 1));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Less, null, null));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Less, "foo", "foo"));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Less, "foo", "bar"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidCastException))]
        public void RaiseIfNotComparableObjectInLess()
        {
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Less, new Context(), new Context()));
        }

        [TestMethod]
        public void EvaluateLessEqualOperator()
        {
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.LessEqual, 1, 2));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.LessEqual, "bar", "foo"));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.LessEqual, 1.2, 3.4));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.LessEqual, 1, 1));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.LessEqual, null, null));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.LessEqual, "foo", "foo"));

            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.LessEqual, 2, 1));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.LessEqual, "foo", "bar"));
        }

        [TestMethod]
        public void EvaluateGreaterOperator()
        {
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.Greater, 2, 1));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.Greater, "foo", "bar"));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.Greater, 3.14, 2.12));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.Greater, "3", 2));

            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Greater, 2, 2));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Greater, 1, 2));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Greater, "foo", "foo"));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Greater, "bar", "foo"));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.Greater, 2, "3"));
        }

        [TestMethod]
        public void EvaluateGreaterEqualOperator()
        {
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.GreaterEqual, 2, 1));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.GreaterEqual, "foo", "bar"));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.GreaterEqual, 3.14, 2.12));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.GreaterEqual, "3", 2));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.GreaterEqual, 2, 2));
            Assert.IsTrue(EvaluateComparisonOperator(ComparisonOperator.GreaterEqual, "foo", "foo"));

            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.GreaterEqual, 1, 2));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.GreaterEqual, "bar", "foo"));
            Assert.IsFalse(EvaluateComparisonOperator(ComparisonOperator.GreaterEqual, 2, "3"));
        }

        private static bool EvaluateComparisonOperator(ComparisonOperator operation, object left, object right)
        {
            IExpression expression = new CompareExpression(operation, new ConstantExpression(left), new ConstantExpression(right));

            return (bool) expression.Evaluate(null);
        }
    }
}
