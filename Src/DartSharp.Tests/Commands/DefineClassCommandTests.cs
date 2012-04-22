using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DartSharp.Commands;
using DartSharp.Language;

namespace DartSharp.Tests.Commands
{
    [TestClass]
    public class DefineClassCommandTests
    {
        [TestMethod]
        public void CreateDefineClassCommand()
        {
            DefineClassCommand command = new DefineClassCommand("MyClass", null);
            Assert.AreEqual("MyClass", command.Name);
            Assert.IsNull(command.Command);
        }

        [TestMethod]
        public void ExecuteDefineClassCommand()
        {
            Context context = new Context();
            DefineClassCommand command = new DefineClassCommand("MyClass", null);
            command.Execute(context);
            var result = context.GetValue("MyClass");
            Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result, typeof(IClass));

            IClass klass = (IClass)result;
            Assert.AreEqual("MyClass", klass.Name);
        }
    }
}
