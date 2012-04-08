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
    public class CompositeCommandTests
    {
        [TestMethod]
        public void CreateAndExecuteCompositeCommand()
        {
            IEnumerable<ICommand> commands = new ICommand[] {
                new SetVariableCommand("a", new ConstantExpression(1)),
                new SetVariableCommand("b", new ConstantExpression(2))
            };

            CompositeCommand command = new CompositeCommand(commands);

            Context context = new Context();

            Assert.IsNotNull(command.Commands);
            Assert.AreEqual(2, command.Commands.Count());
            Assert.AreEqual(2, command.Execute(context));
            Assert.AreEqual(1, context.GetValue("a"));
            Assert.AreEqual(2, context.GetValue("b"));
        }

        [TestMethod]
        public void CreateAndExecuteCompositeCommandWithReturn()
        {
            IEnumerable<ICommand> commands = new ICommand[] {
                new SetVariableCommand("a", new ConstantExpression(1)),
                new ReturnCommand(new VariableExpression("a")),
                new SetVariableCommand("b", new ConstantExpression(2))
            };

            CompositeCommand command = new CompositeCommand(commands);

            Context context = new Context();

            Assert.IsNotNull(command.Commands);
            Assert.AreEqual(3, command.Commands.Count());
            Assert.AreEqual(1, command.Execute(context));
            Assert.IsNotNull(context.ReturnValue);
            Assert.AreEqual(1, context.ReturnValue.Value);
            Assert.AreEqual(1, context.GetValue("a"));
            Assert.IsNull(context.GetValue("b"));
        }
    }
}
