using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DartSharp.Language;

namespace DartSharp.Tests.Language
{
    [TestClass]
    public class FuncMethodTests
    {
        [TestMethod]
        public void DefineAndCallFuncMethod()
        {
            IMethod method = new FuncMethod(null, (obj, context, args) => ((string)obj).Length);
            Assert.IsNull(method.Type);
            Assert.AreEqual(3, method.Call("foo", null, null));
        }
    }
}
