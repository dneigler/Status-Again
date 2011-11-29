﻿using Status.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Status.Model.Tests
{
    
    
    /// <summary>
    ///This is a test class for AuditInfoTest and is intended
    ///to contain all AuditInfoTest Unit Tests
    ///</summary>
    [TestClass()]
    public class AuditInfoTest
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
        ///A test for Author
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Status.Model.dll")]
        public void AuthorTest()
        {
            AuditInfo_Accessor target = new AuditInfo_Accessor(); // TODO: Initialize to an appropriate value
            Resource expected = null; // TODO: Initialize to an appropriate value
            Resource actual;
            target.Author = expected;
            actual = target.Author;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for AuditTime
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Status.Model.dll")]
        public void AuditTimeTest()
        {
            AuditInfo_Accessor target = new AuditInfo_Accessor(); // TODO: Initialize to an appropriate value
            DateTime expected = new DateTime(); // TODO: Initialize to an appropriate value
            DateTime actual;
            target.AuditTime = expected;
            actual = target.AuditTime;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetHashCode
        ///</summary>
        [TestMethod()]
        public void GetHashCodeTest()
        {
            AuditInfo target = new AuditInfo(new Resource() {EmailAddress="test@test.com", FirstName = "Test", LastName = "User"});
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.GetHashCode();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Equals
        ///</summary>
        [TestMethod()]
        public void EqualsTest()
        {
            AuditInfo target = new AuditInfo(new Resource() { EmailAddress = "test@test.com", FirstName = "Test", LastName = "User" });
            AuditInfo obj = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.Equals(obj);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Equals
        ///</summary>
        [TestMethod()]
        public void EqualsTest1()
        {
            AuditInfo target = new AuditInfo(new Resource() {EmailAddress="test@test.com", FirstName = "Test", LastName = "User"});
            object obj = null; // TODO: Initialize to an appropriate value
            bool expected = false; // TODO: Initialize to an appropriate value
            bool actual;
            actual = target.Equals(obj);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for AuditInfo Constructor
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Status.Model.dll")]
        public void AuditInfoConstructorTest()
        {
            AuditInfo_Accessor target = new AuditInfo_Accessor();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for AuditInfo Constructor
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void AuditInfoConstructorTest1()
        {
            Resource author = null; // TODO: Initialize to an appropriate value
            DateTime auditTime = new DateTime(); // TODO: Initialize to an appropriate value
            string machineName = string.Empty; // TODO: Initialize to an appropriate value
            AuditInfo target = new AuditInfo(author, auditTime, machineName);
        }

        /// <summary>
        ///A test for AuditInfo Constructor
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ArgumentException))]
        public void AuditInfoConstructorTest2()
        {
            Resource author = null; // TODO: Initialize to an appropriate value
            AuditInfo target = new AuditInfo(author);
        }

        /// <summary>
        ///A test for MachineName
        ///</summary>
        [TestMethod()]
        [DeploymentItem("Status.Model.dll")]
        public void MachineNameTest()
        {
            AuditInfo_Accessor target = new AuditInfo_Accessor(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.MachineName = expected;
            actual = target.MachineName;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
