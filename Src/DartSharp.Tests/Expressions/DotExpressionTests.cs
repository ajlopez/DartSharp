namespace DartSharp.Tests
{
    using System;
    using System.IO;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    using DartSharp;
    using DartSharp.Commands;
    using DartSharp.Expressions;
    using DartSharp.Language;

    [TestClass]
    public class ExpressionsTests
    {
        [TestMethod]
        public void EvaluateDotExpressionOnInteger()
        {
            IExpression expression = new DotExpression(new ConstantExpression(1), "ToString", new List<IExpression>());

            Assert.AreEqual("1", expression.Evaluate(null));
        }

        [TestMethod]
        public void EvaluateDotExpressionOnString()
        {
            IExpression expression = new DotExpression(new ConstantExpression("foo"), "Length");

            Assert.AreEqual(3, expression.Evaluate(null));
        }

        [TestMethod]
        public void EvaluateDotExpressionAsTypeInvocation()
        {
            DotExpression dot = new DotExpression(new DotExpression(new DotExpression(new VariableExpression("System"), "IO"), "File"), "Exists", new IExpression[] { new ConstantExpression("unknown.txt") });

            Assert.IsFalse((bool) dot.Evaluate(new Context()));
        }
    }
}
