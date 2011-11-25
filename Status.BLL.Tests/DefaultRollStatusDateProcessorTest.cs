using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Status.BLL;

namespace Status.BLL.Tests
{
    /// <summary>
    ///This is a test class for DefaultRollStatusDateProcessorTest and is intended
    ///to contain all DefaultRollStatusDateProcessorTest Unit Tests
    ///</summary>
    [TestClass]
    public class DefaultRollStatusDateProcessorTest
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
        ///Tests the next Monday logic for default roll status date processor.
        ///</summary>
        [TestMethod]
        public void GetStatusReportDateTestOnSaturday()
        {
            var target = new DefaultRollStatusDateProcessor();
            var sourceDate = new DateTime(2011, 1, 1);
            var expected = new DateTime(2011, 1, 3);
            DateTime actual = target.GetStatusReportDate(sourceDate);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///Tests the next Monday logic for default roll status date processor.
        ///</summary>
        [TestMethod]
        public void GetStatusReportDateTestOnMonday()
        {
            var target = new DefaultRollStatusDateProcessor();
            var sourceDate = new DateTime(2011, 1, 3);
            var expected = new DateTime(2011, 1, 10);
            DateTime actual = target.GetStatusReportDate(sourceDate);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for DefaultRollStatusDateProcessor Constructor
        ///</summary>
        [TestMethod()]
        public void DefaultRollStatusDateProcessorConstructorTest()
        {
            DefaultRollStatusDateProcessor target = new DefaultRollStatusDateProcessor();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for GetStatusReportDate
        ///</summary>
        [TestMethod()]
        public void GetStatusReportDateTest()
        {
            DefaultRollStatusDateProcessor target = new DefaultRollStatusDateProcessor(); // TODO: Initialize to an appropriate value
            DateTime sourceDate = new DateTime(); // TODO: Initialize to an appropriate value
            DateTime expected = new DateTime(); // TODO: Initialize to an appropriate value
            DateTime actual;
            actual = target.GetStatusReportDate(sourceDate);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}