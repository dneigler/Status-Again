using System.Collections.Generic;
using Status.BLL;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Status.Model;

namespace Status.BLL.Tests
{
    
    
    /// <summary>
    ///This is a test class for DefaultRollStatusProcessorTest and is intended
    ///to contain all DefaultRollStatusProcessorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class DefaultRollStatusProcessorTest
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
        ///A test for MapStatusItem
        ///</summary>
        [TestMethod()]
        public void MapStatusItemTestCurrentToLastWeek()
        {
            var target = new DefaultRollStatusProcessor();
            var sourceStatusItem = new StatusItem
                                              {
                                                  Caption = "Test",
                                                  Milestone = new Milestone()
                                                                  {
                                                                      ConfidenceLevel = MilestoneConfidenceLevels.High, 
                                                                      Date = new DateTime(2011,1,1), 
                                                                      Type = MilestoneTypes.Milestone
                                                                  },
                                                  Notes = new List<Note>(),
                                                  Topic = new Topic() { Caption = "Test Topic" }
                                              };
            var statusReportDate = new DateTime(2011, 1, 3);
            var actual = target.MapStatusItem(sourceStatusItem, statusReportDate);
            Assert.AreEqual(sourceStatusItem.Caption, actual.Caption);
            Assert.AreEqual(sourceStatusItem.Notes.Count, actual.Notes.Count);
            Assert.AreEqual(sourceStatusItem.Topic.Caption, actual.Topic.Caption);
            Assert.AreEqual(MilestoneTypes.LastWeek, actual.Milestone.Type);
            Assert.AreEqual(sourceStatusItem.Milestone.ConfidenceLevel, actual.Milestone.ConfidenceLevel);
            Assert.AreEqual(sourceStatusItem.Milestone.Date, actual.Milestone.Date);
        }


        /// <summary>
        ///A test for MapStatusItem
        ///</summary>
        [TestMethod()]
        public void MapStatusItemTestLastWeekToNull()
        {
            var target = new DefaultRollStatusProcessor();
            var sourceStatusItem = new StatusItem
            {
                Caption = "Test",
                Milestone = new Milestone()
                                {
                                    ConfidenceLevel = MilestoneConfidenceLevels.High, 
                                    Date = new DateTime(2011, 1, 1), 
                                    Type = MilestoneTypes.Milestone
                                },
                Notes = new List<Note>(),
                Topic = new Topic() { Caption = "Test Topic" }
            };
            var statusReportDate = sourceStatusItem.Milestone.Date.AddDays(8);
            var actual = target.MapStatusItem(sourceStatusItem, statusReportDate);
            Assert.IsNull(actual);
        }

        /// <summary>
        ///A test for MapStatusItem
        ///</summary>
        [TestMethod()]
        public void MapStatusItemTestThisWeek()
        {
            var target = new DefaultRollStatusProcessor();
            var sourceStatusItem = new StatusItem
            {
                Caption = "Test",
                Milestone = new Milestone()
                                {
                                    ConfidenceLevel = MilestoneConfidenceLevels.High, 
                                    Date = new DateTime(2011, 1, 1), 
                                    Type = MilestoneTypes.Milestone
                                },
                Notes = new List<Note>(),
                Topic = new Topic() { Caption = "Test Topic" }
            };
            var statusReportDate = sourceStatusItem.Milestone.Date.AddDays(-3);
            var actual = target.MapStatusItem(sourceStatusItem, statusReportDate);
            Assert.AreEqual(MilestoneTypes.ThisWeek, actual.Milestone.Type);
        }

        /// <summary>
        ///A test for MapStatusItem
        ///</summary>
        [TestMethod()]
        public void MapStatusItemTestMilestone()
        {
            var target = new DefaultRollStatusProcessor();
            var sourceStatusItem = new StatusItem
            {
                Caption = "Test",
                Milestone = new Milestone()
                                {
                                    ConfidenceLevel = MilestoneConfidenceLevels.High, 
                                    Date = new DateTime(2011, 1, 1), 
                                    Type = MilestoneTypes.Milestone
                                },
                Notes = new List<Note>(),
                Topic = new Topic() { Caption = "Test Topic" }
            };
            var statusReportDate = sourceStatusItem.Milestone.Date.AddDays(-10);
            var actual = target.MapStatusItem(sourceStatusItem, statusReportDate);
            Assert.AreEqual(sourceStatusItem.Milestone.Type, actual.Milestone.Type);
        }

        /// <summary>
        ///A test for MapStatusItem
        ///</summary>
        [TestMethod()]
        public void MapStatusItemTestOpenItem()
        {
            var target = new DefaultRollStatusProcessor();
            var sourceStatusItem = new StatusItem
            {
                Caption = "Test",
                Milestone = new Milestone() { 
                    ConfidenceLevel = MilestoneConfidenceLevels.High, 
                    Date = new DateTime(2011, 1, 1), 
                    Type = MilestoneTypes.OpenItem },
                Notes = new List<Note>(),
                Topic = new Topic() { Caption = "Test Topic" }
            };
            var statusReportDate = sourceStatusItem.Milestone.Date.AddDays(-10);
            var actual = target.MapStatusItem(sourceStatusItem, statusReportDate);
            Assert.AreEqual(sourceStatusItem.Milestone.Type, actual.Milestone.Type);
        }

        /// <summary>
        ///A test for MapStatusItem
        ///</summary>
        [TestMethod()]
        public void MapStatusItemTestElse()
        {
            var target = new DefaultRollStatusProcessor();
            var sourceStatusItem = new StatusItem
            {
                Caption = "Test",
                Milestone = new Milestone()
                {
                    ConfidenceLevel = MilestoneConfidenceLevels.High,
                    Date = new DateTime(2011, 1, 1),
                    Type = MilestoneTypes.LastWeek
                },
                Notes = new List<Note>(),
                Topic = new Topic() { Caption = "Test Topic" }
            };
            var statusReportDate = sourceStatusItem.Milestone.Date.AddDays(1);
            var actual = target.MapStatusItem(sourceStatusItem, statusReportDate);
            Assert.AreEqual(sourceStatusItem.Milestone.Type, actual.Milestone.Type);
        }
    }
}
