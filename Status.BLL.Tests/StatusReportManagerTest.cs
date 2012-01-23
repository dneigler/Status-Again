using System;
using System.Collections.Generic;
using AutoMapper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Ninject;
using Status.Model;
using Status.BLL;
using Status.Persistence.Tests;
using Status.Repository;

namespace Status.BLL.Tests
{
    /// <summary>
    ///This is a test class for StatusReportManagerTest and is intended
    ///to contain all StatusReportManagerTest Unit Tests
    ///</summary>
    [TestClass]
    public class StatusReportManagerTest
    {
        private static IKernel _kernel = null;
        private readonly static string _connString = "server=.\\SQLExpress;" +
            "database=StatusAgain;" +
            "Integrated Security=SSPI;";
        private readonly static NHibernateUnitTestConfiguration _config = new NHibernateUnitTestConfiguration(_connString);
        private IResourceRepository _resourceRepository;
        private IProjectRepository _projectRepository;
        private IStatusReportRepository _statusReportRepository;

        private AuditInfo _auditInfo = null;

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
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            _kernel = new StandardKernel(new DefaultStatusNinjectModule(_connString));

        }

        ///Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
        }

        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            _resourceRepository = _kernel.Get<IResourceRepository>();
            _projectRepository = _kernel.Get<IProjectRepository>();
            _statusReportRepository = _kernel.Get<IStatusReportRepository>();

            var resource = _resourceRepository.GetResourceByEmail("test@test.com");
            if (resource == null)
            {
                _resourceRepository.AddResource(new Resource()
                {
                    EmailAddress = "test@test.com",
                    FirstName = "Test",
                    LastName = "User"
                });
                resource = _resourceRepository.GetResourceByEmail("test@test.com");

            }
            _auditInfo = new AuditInfo(resource);

        }

        //
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {

            using (var txn = _statusReportRepository.BeginTransaction())
            {
                try
                {
                    _statusReportRepository.DeleteStatusReport(new DateTime(2011, 1, 1));
                }
                catch (Exception)
                {


                }
                try
                {
                    _statusReportRepository.DeleteStatusReport(new DateTime(2011, 1, 3));
                }
                catch (Exception)
                {


                }
                txn.Commit();
            }

        }

        #endregion

        /// <summary>
        ///A test for RollStatusProcessor
        ///</summary>
        [TestMethod]
        public void RollStatusProcessorDefaultTest()
        {
            var target = _kernel.Get<StatusReportManager>();
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
            var target = _kernel.Get<StatusReportManager>();
            var items = new List<StatusItem>();
            target.StatusReportRepository = _statusReportRepository;
            var proj = _projectRepository.GetAllProjects()[0];

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
                                     Topic = new Topic() { Caption = "test Topic 1" },
                                     Project = proj,
                                     AuditInfo = _auditInfo
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

            StatusReport actual = target.RollStatusReport(report, _auditInfo);
            Assert.AreEqual(report.Caption, actual.Caption);
            Assert.AreEqual(report.Items.Count, actual.Items.Count);
            Assert.AreEqual(report.Items[0], si1);
            Assert.AreEqual(report.Items[1], si2);
            Assert.AreEqual(report.Items[2], si3);
            Assert.AreEqual(new DateTime(2011, 01, 03), actual.PeriodStart);
            Assert.AreEqual(0, report.Items[0].Id);
            Assert.AreEqual(0, report.Items[1].Id);
            Assert.AreEqual(0, report.Items[2].Id);
            // cleanup
            _statusReportRepository.DeleteStatusReport(new DateTime(2011, 1, 3));
        }

        /// <summary>
        ///A test for StatusReportManager Constructor
        ///</summary>
        [TestMethod]
        public void StatusReportManagerConstructorTest()
        {
            var target = _kernel.Get<StatusReportManager>();
            Assert.IsTrue(true);
        }



        /// <summary>
        ///A test for RollStatusProcessor
        ///</summary>
        [TestMethod()]
        public void RollStatusProcessorTest()
        {
            var target = _kernel.Get<StatusReportManager>();
            IRollStatusProcessor expected = new DefaultRollStatusProcessor();
            target.RollStatusProcessor = expected;
            IRollStatusProcessor actual = target.RollStatusProcessor;
            Assert.AreEqual(expected, actual);
        }
    }
}