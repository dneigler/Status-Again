using Status.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Status.Model.Tests
{
    
    
    /// <summary>
    ///This is a test class for StatusItemTest and is intended
    ///to contain all StatusItemTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StatusItemTest
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
        ///A test for StatusItem Constructor
        ///</summary>
        [TestMethod()]
        public void StatusItemConstructorTest()
        {
            Topic statusTopic = null; // TODO: Initialize to an appropriate value
            StatusItem target = new StatusItem(statusTopic);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for StatusItem Constructor
        ///</summary>
        [TestMethod()]
        public void StatusItemConstructorTest1()
        {
            StatusItem target = new StatusItem();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Caption
        ///</summary>
        [TestMethod()]
        public void CaptionTest()
        {
            StatusItem target = new StatusItem(); // TODO: Initialize to an appropriate value
            string expected = string.Empty; // TODO: Initialize to an appropriate value
            string actual;
            target.Caption = expected;
            actual = target.Caption;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Id
        ///</summary>
        [TestMethod()]
        public void IdTest()
        {
            StatusItem target = new StatusItem(); // TODO: Initialize to an appropriate value
            int expected = 0; // TODO: Initialize to an appropriate value
            int actual;
            target.Id = expected;
            actual = target.Id;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Milestone
        ///</summary>
        [TestMethod()]
        public void MilestoneTest()
        {
            StatusItem target = new StatusItem(); // TODO: Initialize to an appropriate value
            Milestone expected = null; // TODO: Initialize to an appropriate value
            Milestone actual;
            target.Milestone = expected;
            actual = target.Milestone;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Notes
        ///</summary>
        [TestMethod()]
        public void NotesTest()
        {
            StatusItem target = new StatusItem(); // TODO: Initialize to an appropriate value
            List<Note> expected = null; // TODO: Initialize to an appropriate value
            List<Note> actual;
            target.Notes = expected;
            actual = target.Notes;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Topic
        ///</summary>
        [TestMethod()]
        public void TopicTest()
        {
            StatusItem target = new StatusItem(); // TODO: Initialize to an appropriate value
            Topic expected = null; // TODO: Initialize to an appropriate value
            Topic actual;
            target.Topic = expected;
            actual = target.Topic;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
