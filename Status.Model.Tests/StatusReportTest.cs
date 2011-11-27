using Status.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Status.Model.Tests
{
    
    
    /// <summary>
    ///This is a test class for StatusReportTest and is intended
    ///to contain all StatusReportTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StatusReportTest
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for StatusReport Constructor
        ///</summary>
        [TestMethod()]
        public void StatusReportConstructorTest()
        {
            StatusReport target = new StatusReport();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for AddStatusItem
        ///</summary>
        [TestMethod()]
        public void AddStatusItemTest()
        {
            StatusReport target = new StatusReport(); // TODO: Initialize to an appropriate value
            Topic statusTopic = new Topic() {Caption = "Test Topic"}; // TODO: Initialize to an appropriate value
            target.AddStatusItem(statusTopic);
            Assert.AreEqual(1, target.Items.Count);

            StatusItem si = target.Items[0];
            Assert.AreEqual(statusTopic.Caption, si.Caption);
        }

        /// <summary>
        ///A test for AddStatusItem
        ///</summary>
        [TestMethod()]
        public void AddStatusItemTest1()
        {
            StatusReport target = new StatusReport(); // TODO: Initialize to an appropriate value
            StatusItem statusItem = new StatusItem() {Caption = "Test Status Item"}; // TODO: Initialize to an appropriate value
            target.AddStatusItem(statusItem);
            Assert.AreEqual(1, target.Items.Count);

            StatusItem si = target.Items[0];
            Assert.AreSame(statusItem, si);
        }

        /// <summary>
        ///A test for Caption
        ///</summary>
        [TestMethod()]
        public void CaptionTest()
        {
            StatusReport target = new StatusReport(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.Caption = expected;
            actual = target.Caption;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Id
        ///</summary>
        [TestMethod()]
        public void IdTest()
        {
            StatusReport target = new StatusReport(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Id = expected;
            actual = target.Id;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Items
        ///</summary>
        [TestMethod()]
        public void ItemsTest()
        {
            StatusReport target = new StatusReport(); // TODO: Initialize to an appropriate value
            Assert.IsNotNull(target.Items);
        }

        /// <summary>
        ///A test for PeriodEnd
        ///</summary>
        [TestMethod()]
        public void PeriodEndTest()
        {
            StatusReport target = new StatusReport(); // TODO: Initialize to an appropriate value
            DateTime expected = new DateTime(); // TODO: Initialize to an appropriate value
            DateTime actual;
            target.PeriodEnd = expected;
            actual = target.PeriodEnd;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for PeriodStart
        ///</summary>
        [TestMethod()]
        public void PeriodStartTest()
        {
            StatusReport target = new StatusReport(); // TODO: Initialize to an appropriate value
            DateTime expected = new DateTime(); // TODO: Initialize to an appropriate value
            DateTime actual;
            target.PeriodStart = expected;
            actual = target.PeriodStart;
            Assert.AreEqual(expected, actual);
        }
    }
}
