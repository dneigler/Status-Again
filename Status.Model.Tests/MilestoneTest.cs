using Status.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Status.Model.Tests
{
    
    
    /// <summary>
    ///This is a test class for MilestoneTest and is intended
    ///to contain all MilestoneTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MilestoneTest
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
        ///A test for Milestone Constructor
        ///</summary>
        [TestMethod()]
        public void MilestoneConstructorTest()
        {
            Milestone target = new Milestone();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for ConfidenceLevel
        ///</summary>
        [TestMethod()]
        public void ConfidenceLevelTest()
        {
            Milestone target = new Milestone(); // TODO: Initialize to an appropriate value
            MilestoneConfidenceLevels expected = new MilestoneConfidenceLevels(); // TODO: Initialize to an appropriate value
            MilestoneConfidenceLevels actual;
            target.ConfidenceLevel = expected;
            actual = target.ConfidenceLevel;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Date
        ///</summary>
        [TestMethod()]
        public void DateTest()
        {
            Milestone target = new Milestone(); // TODO: Initialize to an appropriate value
            DateTime expected = new DateTime(); // TODO: Initialize to an appropriate value
            DateTime actual;
            target.Date = expected;
            actual = target.Date.Value;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Type
        ///</summary>
        [TestMethod()]
        public void TypeTest()
        {
            Milestone target = new Milestone(); // TODO: Initialize to an appropriate value
            MilestoneTypes expected = new MilestoneTypes(); // TODO: Initialize to an appropriate value
            MilestoneTypes actual;
            target.Type = expected;
            actual = target.Type;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
