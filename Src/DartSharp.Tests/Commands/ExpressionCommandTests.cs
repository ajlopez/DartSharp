using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DartSharp.Commands;
using DartSharp.Expressions;

namespace DartSharp.Tests.Commands
{
    [TestClass]
    public class ExpressionCommandTests
    {
        [TestMethod]
        public void ExecuteConstantExpression()
        {
            ConstantExpression expr = new ConstantExpression(1);
            ExpressionCommand command = new ExpressionCommand(expr);
            Assert.AreEqual(1, command.Execute(null));
            Assert.AreEqual(expr, command.Expression);
        }
    }
}
