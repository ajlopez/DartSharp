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
        private IClass type;

        [TestInitialize]
        public void Setup()
        {
            IClass type = new BaseClass("String", null);
            this.type = new BaseClass("MyClass", null);
            IMethod getname = new FuncMethod(null, (obj, context, arguments) => ((IObject)obj).GetValue("name"));
            this.type.DefineVariable("name", type);
            this.type.DefineMethod("getName", getname);
        }

        [TestMethod]
        public void GetObjectType()
        {
            IObject obj = new BaseObject(this.type);
            Assert.AreEqual(this.type, obj.Type);
        }

        [TestMethod]
        public void GetInstanceVariableAsNull()
        {
            IObject obj = new BaseObject(type);
            Assert.IsNull(obj.GetValue("name"));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void RaiseIfClassIsNull()
        {
            new BaseObject(null);
        }

        [TestMethod]
        public void SetAndGetInstanceVariable()
        {
            IObject obj = new BaseObject(type);
            obj.SetValue("name", "Adam");
            Assert.AreEqual("Adam", obj.GetValue("name"));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RaiseWhenGetUndefinedVariable()
        {
            IObject obj = new BaseObject(type);
            obj.GetValue("length");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RaiseWhenSetUndefinedVariable()
        {
            IObject obj = new BaseObject(type);
            obj.SetValue("length", 100);
        }

        [TestMethod]
        public void InvokeGetName()
        {
            IObject obj = new BaseObject(type);
            obj.SetValue("name", "Adam");
            Assert.AreEqual("Adam", obj.Invoke("getName", null, null));
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RaiseWhenInvokeUndefinedMethod()
        {
            IObject obj = new BaseObject(type);
            obj.Invoke("getLength", null, null);
        }
    }
}
