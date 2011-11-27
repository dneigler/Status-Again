using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Status.Model;
using Status.BLL;

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
            Type expected = typeof(DefaultRollStatusProcessor);
            Type actual = target.RollStatusProcessor.GetType();
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for RollStatusReport
        ///</summary>
        [TestMethod]
        public void RollStatusReportTest()
        {
            Mapper.CreateMap<StatusItem, StatusItem>();
            var target = new StatusReportManager();
            List<StatusItem> items = new List<StatusItem>();
            StatusItem si1 = new StatusItem()
                                 {
                                     Caption = "Test1",
                                     Milestone = new Milestone()
                                                     {
                                                         ConfidenceLevel = MilestoneConfidenceLevels.High,
                                                         Date = new DateTime(2011, 1, 1),
                                                         Type = MilestoneTypes.Milestone
                                                     },
                                     Notes = new List<Note>(),
                                     Topic = new Topic() { Caption = "test Topic 1" }
                                 };
            StatusItem si2 = Mapper.Map<StatusItem, StatusItem>(si1);
            si2.Milestone.Date = new DateTime(2011, 1, 10);

            StatusItem si3 = Mapper.Map<StatusItem, StatusItem>(si1);
            si3.Milestone.Date = new DateTime(2011, 2, 1);
            items.Add(si1);
            items.Add(si2);
            items.Add(si3);
            var report = new StatusReport
                             {
                                 Caption = "Test 1",
                                 PeriodStart = new DateTime(2011, 01, 01)
                             };
            items.ForEach(report.AddStatusItem);
            IRollStatusDateProcessor dateProcessor = new DefaultRollStatusDateProcessor();
            StatusReport actual = target.RollStatusReport(report, dateProcessor);
            Assert.AreEqual(report.Caption, actual.Caption);
            Assert.AreEqual(report.Items.Count, actual.Items.Count);
            Assert.AreEqual(report.Items[0], si1);
            Assert.AreEqual(report.Items[1], si2);
            Assert.AreEqual(report.Items[2], si3);
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



        /// <summary>
        ///A test for RollStatusProcessor
        ///</summary>
        [TestMethod()]
        public void RollStatusProcessorTest()
        {
            StatusReportManager target = new StatusReportManager();
            IRollStatusProcessor expected = new DefaultRollStatusProcessor();
            target.RollStatusProcessor = expected;
            IRollStatusProcessor actual = target.RollStatusProcessor;
            Assert.AreEqual(expected, actual);
        }
    }
}