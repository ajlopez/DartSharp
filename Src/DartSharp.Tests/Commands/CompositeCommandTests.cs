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
    }
}
