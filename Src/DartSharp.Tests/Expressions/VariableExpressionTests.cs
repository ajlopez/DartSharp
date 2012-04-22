using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DartSharp.Expressions;
using DartSharp.Commands;
using DartSharp.Methods;

namespace DartSharp.Tests.Expressions
{
    [TestClass]
    public class VariableExpressionTests
    {
        [TestMethod]
        public void EvaluateUndefinedVariable()
        {
            Context context = new Context();
            VariableExpression expr = new VariableExpression("foo");

            Assert.IsNull(expr.Evaluate(context));
        }

        [TestMethod]
        public void DefineVariableWithName()
        {
            Context context = new Context();
            VariableExpression expr = new VariableExpression("foo");

            Assert.AreEqual("foo", expr.Name);
            Assert.IsNull(expr.TypeExpression);
        }

        [TestMethod]
        public void DefineVariableWithNameAndType()
        {
            Context context = new Context();
            VariableExpression typeexpr = new VariableExpression("List");
            VariableExpression expr = new VariableExpression(typeexpr, "foo");

            Assert.AreEqual("foo", expr.Name);
            Assert.AreEqual(typeexpr, expr.TypeExpression);
        }

        [TestMethod]
        public void EvaluateDefinedVariable()
        {
            Context context = new Context();
            context.SetValue("one", 1);
            VariableExpression expr = new VariableExpression("one");

            Assert.AreEqual(1, expr.Evaluate(context));
            Assert.AreEqual("one", expr.Name);
        }
    }
}
