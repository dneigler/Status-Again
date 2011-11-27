using Status.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Status.Model.Tests
{
    
    
    /// <summary>
    ///This is a test class for EmployeeTest and is intended
    ///to contain all EmployeeTest Unit Tests
    ///</summary>
    [TestClass()]
    public class EmployeeTest
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
        ///A test for Employee Constructor
        ///</summary>
        [TestMethod()]
        public void EmployeeConstructorTest()
        {
            Employee target = new Employee();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for EdsId
        ///</summary>
        [TestMethod()]
        public void EdsIdTest()
        {
            Employee target = new Employee(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.EdsId = expected;
            actual = target.EdsId;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Team
        ///</summary>
        [TestMethod()]
        public void TeamTest()
        {
            Employee target = new Employee(); // TODO: Initialize to an appropriate value
            Team expected = null; // TODO: Initialize to an appropriate value
            Team actual;
            target.Team = expected;
            actual = target.Team;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Title
        ///</summary>
        [TestMethod()]
        public void TitleTest()
        {
            Employee target = new Employee(); // TODO: Initialize to an appropriate value
            Title expected = new Title(); // TODO: Initialize to an appropriate value
            Title actual;
            target.Title = expected;
            actual = target.Title;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
