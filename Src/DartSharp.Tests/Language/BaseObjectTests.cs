using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DartSharp.Language;

namespace DartSharp.Tests.Language
{
    [TestClass]
    public class BaseObjectTests
    {
        private IClass klass;

        [TestInitialize]
        public void Setup()
        {
            IClass type = new BaseClass("String", null);
            this.klass = new BaseClass("MyClass", null);
            this.klass.DefineVariable("name", type);
        }

        [TestMethod]
        public void GetInstanceVariableAsNull()
        {
            IObject obj = new BaseObject(klass);
            Assert.IsNull(obj.GetValue("name"));
        }

        [TestMethod]
        public void SetAndGetInstanceVariable()
        {
            IObject obj = new BaseObject(klass);
            obj.SetValue("name", "Adam");
            Assert.AreEqual("Adam", obj.GetValue("name"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RaiseWhenGetUndefinedVariable()
        {
            IObject obj = new BaseObject(klass);
            obj.GetValue("length");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RaiseWhenSetUndefinedVariable()
        {
            IObject obj = new BaseObject(klass);
            obj.SetValue("length", 100);
        }
    }
}
