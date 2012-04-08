using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DartSharp.Commands;
using DartSharp.Expressions;
using DartSharp.Methods;

namespace DartSharp.Tests.Commands
{
    [TestClass]
    public class DefineFunctionCommandTests
    {
        [TestMethod]
        public void CreateAndExecuteDefineFunctionCommand()
        {
            IEnumerable<ICommand> commands = new ICommand[] {
                new SetVariableCommand("a", new ConstantExpression(1)),
                new SetVariableCommand("b", new ConstantExpression(2))
            };

            CompositeCommand body = new CompositeCommand(commands);
            DefineFunctionCommand command = new DefineFunctionCommand("foo", null, body);

            Context context = new Context();
            Assert.IsNull(command.Execute(context));
            Assert.AreEqual("foo", command.Name);
            Assert.IsNull(command.ArgumentNames);
            Assert.AreEqual(body, command.Command);

            var result = context.GetValue("foo");

            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(DefinedFunction));

            DefinedFunction dfunc = (DefinedFunction)result;

            Assert.AreEqual(2, dfunc.Call(context, null));
        }
    }
}
