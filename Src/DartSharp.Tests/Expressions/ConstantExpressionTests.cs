using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DartSharp.Expressions;

namespace DartSharp.Tests.Expressions
{
    [TestClass]
    public class ConstantExpressionTests
    {
        [TestMethod]
        public void CreateAndEvaluateInteger()
        {
            ConstantExpression expr = new ConstantExpression(1);

            Assert.AreEqual(1, expr.Evaluate(null));
            Assert.AreEqual(1, expr.Value);
        }
    }
}
