using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DartSharp.Expressions;
using DartSharp.Commands;

namespace DartSharp.Tests.Commands
{
    [TestClass]
    public class DefineVariableCommandTests
    {
        [TestMethod]
        public void DefineVariable()
        {
            Context context = new Context();
            DefineVariableCommand expr = new DefineVariableCommand("a");

            Assert.IsNull(expr.Execute(context));
            Assert.IsTrue(context.HasVariable("a"));
            Assert.IsNull(context.GetValue("a"));
            Assert.AreEqual("a", expr.Name);
        }

        [TestMethod]
        public void DefineVariableWithInitialValue()
        {
            Context context = new Context();
            DefineVariableCommand expr = new DefineVariableCommand("a", new ConstantExpression(1));

            Assert.AreEqual(1, expr.Execute(context));
            Assert.IsTrue(context.HasVariable("a"));
            Assert.AreEqual(1, context.GetValue("a"));
            Assert.AreEqual("a", expr.Name);
        }
    }
}
