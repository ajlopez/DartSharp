using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DartSharp.Commands;
using DartSharp.Expressions;
using DartSharp.Methods;
using DartSharp.Language;

namespace DartSharp.Tests.Commands
{
    [TestClass]
    public class IfCommandTests
    {
        [TestMethod]
        public void IfTrueThen()
        {
            var condition = new CompareExpression(ComparisonOperator.Less, new VariableExpression("a"), new ConstantExpression(10));
            var thencommand = new SetVariableCommand("k", new ConstantExpression(1));

            var ifcommand = new IfCommand(condition, thencommand);

            Context context = new Context();
            context.SetValue("a", 1);
            context.SetValue("k", 0);

            var result = ifcommand.Execute(context);

            Assert.IsNull(result);
            Assert.IsNotNull(ifcommand.Condition);
            Assert.IsNotNull(ifcommand.ThenCommand);
            Assert.IsNull(ifcommand.ElseCommand);

            Assert.AreEqual(1, context.GetValue("k"));
        }

        [TestMethod]
        public void IfFalseThen()
        {
            var condition = new CompareExpression(ComparisonOperator.Less, new VariableExpression("a"), new ConstantExpression(10));
            var thencommand = new SetVariableCommand("k", new ConstantExpression(1));

            var ifcommand = new IfCommand(condition, thencommand);

            Context context = new Context();
            context.SetValue("a", 10);
            context.SetValue("k", 0);

            var result = ifcommand.Execute(context);

            Assert.IsNull(result);
            Assert.IsNotNull(ifcommand.Condition);
            Assert.IsNotNull(ifcommand.ThenCommand);
            Assert.IsNull(ifcommand.ElseCommand);

            Assert.AreEqual(0, context.GetValue("k"));
        }

        [TestMethod]
        public void IfTrueThenElse()
        {
            var condition = new CompareExpression(ComparisonOperator.Less, new VariableExpression("a"), new ConstantExpression(10));
            var thencommand = new SetVariableCommand("k", new ConstantExpression(1));
            var elsecommand = new SetVariableCommand("k", new ConstantExpression(2));

            var ifcommand = new IfCommand(condition, thencommand, elsecommand);

            Context context = new Context();
            context.SetValue("a", 1);
            context.SetValue("k", 0);

            var result = ifcommand.Execute(context);

            Assert.IsNull(result);
            Assert.IsNotNull(ifcommand.Condition);
            Assert.IsNotNull(ifcommand.ThenCommand);
            Assert.IsNotNull(ifcommand.ElseCommand);

            Assert.AreEqual(1, context.GetValue("k"));
        }

        [TestMethod]
        public void IfFalseThenElse()
        {
            var condition = new CompareExpression(ComparisonOperator.Less, new VariableExpression("a"), new ConstantExpression(10));
            var thencommand = new SetVariableCommand("k", new ConstantExpression(1));
            var elsecommand = new SetVariableCommand("k", new ConstantExpression(2));

            var ifcommand = new IfCommand(condition, thencommand, elsecommand);

            Context context = new Context();
            context.SetValue("a", 10);
            context.SetValue("k", 0);

            var result = ifcommand.Execute(context);

            Assert.IsNull(result);
            Assert.IsNotNull(ifcommand.Condition);
            Assert.IsNotNull(ifcommand.ThenCommand);
            Assert.IsNotNull(ifcommand.ElseCommand);

            Assert.AreEqual(2, context.GetValue("k"));
        }
    }
}
