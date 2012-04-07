using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DartSharp.Expressions;

namespace DartSharp.Tests.Expressions
{
    [TestClass]
    public class DefineVariableExpressionTests
    {
        [TestMethod]
        public void DefineVariable()
        {
            Context context = new Context();
            DefineVariableExpression expr = new DefineVariableExpression("a");

            Assert.IsNull(expr.Evaluate(context));
            Assert.IsTrue(context.HasVariable("a"));
            Assert.IsNull(context.GetValue("a"));
            Assert.AreEqual("a", expr.Name);
        }

        [TestMethod]
        public void DefineVariableWithInitialValue()
        {
            Context context = new Context();
            DefineVariableExpression expr = new DefineVariableExpression("a", new ConstantExpression(1));

            Assert.AreEqual(1, expr.Evaluate(context));
            Assert.IsTrue(context.HasVariable("a"));
            Assert.AreEqual(1, context.GetValue("a"));
            Assert.AreEqual("a", expr.Name);
        }
    }
}
