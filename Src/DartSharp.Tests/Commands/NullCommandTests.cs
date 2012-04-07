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
    public class NullCommandTests
    {
        [TestMethod]
        public void ExecuteNullCommand()
        {
            Assert.IsNull(NullCommand.Instance.Execute(null));
        }
    }
}
