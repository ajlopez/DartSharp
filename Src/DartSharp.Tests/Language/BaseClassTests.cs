using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DartSharp.Language;

namespace DartSharp.Tests.Language
{
    [TestClass]
    public class BaseClassTests
    {
        [TestMethod]
        public void ClassWithoutSuperclass()
        {
            IClass klass = new BaseClass("Object", null);
            Assert.AreEqual("Object", klass.Name);
            Assert.IsNull(klass.Super);
        }

        [TestMethod]
        public void ClassWithSuperclass()
        {
            IClass super = new BaseClass("Object", null);
            IClass klass = new BaseClass("Rectangle", super);
            Assert.AreEqual("Rectangle", klass.Name);
            Assert.AreEqual(super, klass.Super);
        }

        [TestMethod]
        public void UnknowVariableAsNullType()
        {
            IClass klass = new BaseClass("Object", null);
            Assert.IsNull(klass.GetVariableType("a"));
        }

        [TestMethod]
        public void DefineVariable()
        {
            IClass type = new BaseClass("int", null);
            IClass klass = new BaseClass("MyClass", null);
            klass.DefineVariable("age", type);
            var result = klass.GetVariableType("age");
            Assert.IsNotNull(result);
            Assert.AreEqual(type, result);
        }

        [TestMethod]
        public void DefineVariableAndGetVariableFromSuper()
        {
            IClass type = new BaseClass("int", null);
            IClass super = new BaseClass("MySuperclass", null);
            IClass klass = new BaseClass("MyClass", super);
            super.DefineVariable("age", type);
            var result = klass.GetVariableType("age");
            Assert.IsNotNull(result);
            Assert.AreEqual(type, result);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RaiseIfVariableIsAlreadyDefined()
        {
            IClass type = new BaseClass("int", null);
            IClass klass = new BaseClass("MyClass", null);
            klass.DefineVariable("age", type);
            klass.DefineVariable("age", type);
        }

        [TestMethod]
        public void DefineMethod()
        {
            IClass type = new BaseClass("String", null);
            IClass klass = new BaseClass("MyClass", null);
            IMethod getname = new FuncMethod(type, (obj, context, arguments) => ((IObject)obj).GetValue("name"));
            klass.DefineMethod("getName", getname);
            var result = klass.GetMethod("getName");
            Assert.IsNotNull(result);
            Assert.AreEqual(type, result.Type);
        }

        [TestMethod]
        public void DefineAndGetMethodFromSuper()
        {
            IClass type = new BaseClass("String", null);
            IClass super = new BaseClass("MySuperClass", null);
            IClass klass = new BaseClass("MyClass", super);
            IMethod getname = new FuncMethod(type, (obj, context, arguments) => ((IObject)obj).GetValue("name"));
            super.DefineMethod("getName", getname);
            var result = klass.GetMethod("getName");
            Assert.IsNotNull(result);
            Assert.AreEqual(type, result.Type);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void RaiseIfMethodIsAlreadyDefined()
        {
            IClass type = new BaseClass("String", null);
            IClass klass = new BaseClass("MyClass", null);
            IMethod getname = new FuncMethod(type, (obj, context, arguments) => ((IObject)obj).GetValue("name"));
            klass.DefineMethod("getName", getname);
            klass.DefineMethod("getName", getname);
        }
    }
}
