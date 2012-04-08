namespace DartSharp.Tests
{
    using System;
    using System.Text;
    using System.Collections.Generic;
    using System.Linq;

    using DartSharp;
    using DartSharp.Language;

    using Microsoft.VisualStudio.TestTools.UnitTesting;

    [TestClass]
    public class ObjectUtilitiesTests
    {
        [TestMethod]
        public void GetPropertyFromString()
        {
            Assert.AreEqual(3, ObjectUtilities.GetValue("foo", "Length"));
        }

        [TestMethod]
        public void GetValueUsingCall()
        {
            Assert.AreEqual("oo", ObjectUtilities.GetValue("foo", "Substring", new object[] { 1 }));
        }

        // TODO DynamicObject support?
        //[TestMethod]
        //public void GetValueFromDynamicObject()
        //{
        //    DynamicObject dynobj = new DynamicObject();
        //    dynobj.SetValue("FirstName", "Adam");

        //    Assert.AreEqual("Adam", ObjectUtilities.GetValue(dynobj, "FirstName"));
        //}

        [TestMethod]
        public void IsNumber()
        {
            Assert.IsTrue(ObjectUtilities.IsNumber((byte) 1));
            Assert.IsTrue(ObjectUtilities.IsNumber((short) 2));
            Assert.IsTrue(ObjectUtilities.IsNumber((int) 3));
            Assert.IsTrue(ObjectUtilities.IsNumber((long) 4));
            Assert.IsTrue(ObjectUtilities.IsNumber((float) 1.2));
            Assert.IsTrue(ObjectUtilities.IsNumber((double) 2.3));

            Assert.IsFalse(ObjectUtilities.IsNumber(null));
            Assert.IsFalse(ObjectUtilities.IsNumber("foo"));
            Assert.IsFalse(ObjectUtilities.IsNumber('a'));
            Assert.IsFalse(ObjectUtilities.IsNumber(this));
        }

        [TestMethod]
        public void GetIndexedValuesFromArrays()
        {
            Assert.AreEqual(2, ObjectUtilities.GetIndexedValue(new int[] { 1, 2, 3 }, new object[] { 1 }));
            Assert.AreEqual(3, ObjectUtilities.GetIndexedValue(new int[,] { {1,2}, {2,3} }, new object[] { 1, 1 }));
        }

        [TestMethod]
        public void GetIndexedValuesFromList()
        {
            List<int> list = new List<int>();

            list.Add(1);
            list.Add(2);
            list.Add(3);

            Assert.AreEqual(1, ObjectUtilities.GetIndexedValue(list, new object[] { 0 }));
            Assert.AreEqual(2, ObjectUtilities.GetIndexedValue(list, new object[] { 1 }));
            Assert.AreEqual(3, ObjectUtilities.GetIndexedValue(list, new object[] { 2 }));
        }

        [TestMethod]
        public void GetIndexedValuesFromDictionary()
        {
            Dictionary<string, int> numbers = new Dictionary<string, int>();

            numbers["one"] = 1;
            numbers["two"] = 2;
            numbers["three"] = 3;

            Assert.AreEqual(1, ObjectUtilities.GetIndexedValue(numbers, new object[] { "one" }));
            Assert.AreEqual(2, ObjectUtilities.GetIndexedValue(numbers, new object[] { "two" }));
            Assert.AreEqual(3, ObjectUtilities.GetIndexedValue(numbers, new object[] { "three" }));
        }

        // TODO DynamicObject support?
        //[TestMethod]
        //public void GetIndexedValuesFromDynamicObject()
        //{
        //    DynamicObject obj = new DynamicObject();
        //    obj.SetValue("Name", "Adam");
        //    obj.SetValue("GetAge", new Function(new string[] { "n" }, null));

        //    Assert.AreEqual("Adam", ObjectUtilities.GetIndexedValue(obj, new object[] { "Name" }));
            
        //    object f = ObjectUtilities.GetIndexedValue(obj, new object[] { "GetAge" });
        //    Assert.IsNotNull(f);
        //    Assert.IsInstanceOfType(f, typeof(Function));
        //}

        [TestMethod]
        public void SetIndexedValuesInArrays()
        {
            int[] array = new int[2];

            ObjectUtilities.SetIndexedValue(array, new object[] { 0 }, 1);
            ObjectUtilities.SetIndexedValue(array, new object[] { 1 }, 2);

            Assert.AreEqual(1, array[0]);
            Assert.AreEqual(2, array[1]);
        }

        [TestMethod]
        public void SetIndexedValuesInList()
        {
            List<int> list = new List<int>();

            ObjectUtilities.SetIndexedValue(list, new object[] { 0 }, 1);
            ObjectUtilities.SetIndexedValue(list, new object[] { 1 }, 2);
            ObjectUtilities.SetIndexedValue(list, new object[] { 2 }, 3);

            Assert.AreEqual(3, list.Count);
            Assert.AreEqual(1, list[0]);
            Assert.AreEqual(2, list[1]);
            Assert.AreEqual(3, list[2]);
        }

        [TestMethod]
        public void SetIndexedValuesInDictionary()
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();

            ObjectUtilities.SetIndexedValue(dictionary, new object[] { "one" }, 1);
            ObjectUtilities.SetIndexedValue(dictionary, new object[] { "two" }, 2);
            ObjectUtilities.SetIndexedValue(dictionary, new object[] { "three" }, 3);

            Assert.AreEqual(1, dictionary["one"]);
            Assert.AreEqual(2, dictionary["two"]);
            Assert.AreEqual(3, dictionary["three"]);
        }
    }
}
