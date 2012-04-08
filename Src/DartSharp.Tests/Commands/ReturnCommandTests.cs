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
    public class ReturnCommandTests
    {
        [TestMethod]
        public void ExecuteConstantExpression()
        {
            ConstantExpression expr = new ConstantExpression(1);
            ReturnCommand command = new ReturnCommand(expr);
            Context context = new Context();
            Assert.AreEqual(1, command.Execute(context));
            Assert.IsNotNull(context.ReturnValue);
            Assert.AreEqual(1, context.ReturnValue.Value);
            Assert.AreEqual(expr, command.Expression);
        }
    }
}
