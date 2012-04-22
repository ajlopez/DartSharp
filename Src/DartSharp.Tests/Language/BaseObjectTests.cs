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
        [TestMethod]
        public void GetValueAsNull()
        {
            IObject obj = new BaseObject(null);
            Assert.IsNull(obj.GetValue("name"));
        }

        [TestMethod]
        public void SetAndGetValue()
        {
            IObject obj = new BaseObject(null);
            obj.SetValue("name", "Adam");
            Assert.AreEqual("Adam", obj.GetValue("name"));
        }
    }
}
