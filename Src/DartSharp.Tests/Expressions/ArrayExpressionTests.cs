using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DartSharp.Commands;
using DartSharp.Expressions;
using DartSharp.Methods;
using System.IO;
using System.Collections;

namespace DartSharp.Tests.Expressions
{
    [TestClass]
    public class ArrayExpressionTests
    {
        [TestMethod]
        public void EvaluateArrayExpression()
        {
            ArrayExpression expr = new ArrayExpression(new IExpression[] { new ConstantExpression(1), new ConstantExpression(2), new ConstantExpression(3) });

            Assert.IsNotNull(expr.Expressions);
            Assert.AreEqual(3, expr.Expressions.Count());

            object result = expr.Evaluate(null);

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IList));

            IList list = (IList)result;
            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(3, list[2]);
        }
    }
}
