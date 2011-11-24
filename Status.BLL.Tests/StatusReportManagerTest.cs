using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Status.Model;

namespace Status.BLL.Tests
{
    /// <summary>
    ///This is a test class for StatusReportManagerTest and is intended
    ///to contain all StatusReportManagerTest Unit Tests
    ///</summary>
    [TestClass]
    public class StatusReportManagerTest
    {
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext { get; set; }

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
        ///A test for RollStatusProcessor
        ///</summary>
        [TestMethod]
        public void RollStatusProcessorDefaultTest()
        {
            var target = new StatusReportManager();
            Type expected = typeof (DefaultRollStatusProcessor);
            Type actual = target.RollStatusProcessor.GetType();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for RollStatusReport
        ///</summary>
        [TestMethod]
        public void RollStatusReportTest()
        {
            var target = new StatusReportManager();
            var report = new StatusReport
                             {
                                 Caption = "Test 1",
                                 Items = new List<StatusItem>(),
                                 PeriodStart = new DateTime(2011, 01, 01)
                             };
            IRollStatusDateProcessor dateProcessor = new DefaultRollStatusDateProcessor();
            StatusReport actual = target.RollStatusReport(report, dateProcessor);
            Assert.AreEqual(report.Caption, actual.Caption);
            Assert.AreEqual(report.Items.Count, actual.Items.Count);
            Assert.AreEqual(new DateTime(2011, 01, 03), actual.PeriodStart);
        }

        /// <summary>
        ///A test for StatusReportManager Constructor
        ///</summary>
        [TestMethod]
        public void StatusReportManagerConstructorTest()
        {
            var target = new StatusReportManager();
            Assert.IsTrue(true);
        }
    }
}