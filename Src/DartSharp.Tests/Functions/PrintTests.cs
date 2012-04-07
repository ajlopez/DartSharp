using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DartSharp.Methods;
using System.IO;

namespace DartSharp.Tests.Methods
{
    [TestClass]
    public class PrintTests
    {
        [TestMethod]
        public void WriteLine()
        {
            StringWriter writer = new StringWriter();
            Print print = new Print(writer);
            var result = print.Call(null, new object[] { "hello" });
            Assert.IsNull(result);
            writer.Close();
            Assert.AreEqual("hello\r\n", writer.ToString());
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RaiseIfTwoArguments()
        {
            StringWriter writer = new StringWriter();
            Print print = new Print(writer);
            var result = print.Call(null, new object[] { "hello", "world" });
            Assert.IsNull(result);
            writer.Close();
            Assert.AreEqual("hello\r\nworld\r\n", writer.ToString());
        }

        [TestMethod]
        public void WriteEmptyLine()
        {
            StringWriter writer = new StringWriter();
            Print print = new Print(writer);
            var result = print.Call(null, null);
            Assert.IsNull(result);
            writer.Close();
            Assert.AreEqual("\r\n", writer.ToString());
        }
    }
}
