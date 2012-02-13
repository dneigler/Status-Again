using System.IO;
using Ninject;
using Status.ETL.Csv;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Status.Repository;
using Status.Etl.Csv;
using System.Collections.Generic;
using Status.Persistence.Tests;
using Status.Model;

namespace Status.Etl.Tests
{
    
    
    /// <summary>
    ///This is a test class for CsvStatusEtlBridgeTest and is intended
    ///to contain all CsvStatusEtlBridgeTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CsvStatusEtlBridgeTest
    {
        private TestContext testContextInstance;
        private static StandardKernel _kernel;
        private readonly static string _connString = "server=.\\SQLExpress;" +
            "database=StatusAgain;" +
            "Integrated Security=SSPI;";
        private readonly static NHibernateUnitTestConfiguration _config = new NHibernateUnitTestConfiguration(_connString);

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
        [AssemblyInitialize()]
        public static void AssemblyInit(TestContext context)
        {
            //System.Environment.CurrentDirectory = @"C:\";
        }

        // 

        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            _kernel = new StandardKernel(new DefaultEtlNinjectModule(_connString));
            _config.Configure();
        }

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
        ///A test for CsvStatusEtlBridge Constructor
        ///</summary>
        [TestMethod()]
        public void CsvStatusEtlBridgeConstructorTest()
        {
            var target = _kernel.Get<ICsvStatusEtlBridge>(); // new CsvStatusEtlBridge(statusReportRepository, projectRepository, topicRepository);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for UpsertStatus
        ///</summary>
        [TestMethod()]
        public void UpsertStatusTest()
        {
            var target = _kernel.Get<ICsvStatusEtlBridge>(); // new CsvStatusEtlBridge(statusReportRepository, projectRepository, topicRepository);
            var etl = new CsvStatusEtl();
            using (TextReader file = new StreamReader(@"..\..\..\..\StatusSample.csv"))
            {
                var items = etl.ImportStatus(file);
                target.UpsertStatus(items);
                // ensure proper status items come back
                var srRepo = _kernel.Get<IStatusReportRepository>();
                IList<StatusReport> reports = srRepo.GetStatusReports(new DateTime(2011, 11, 1), new DateTime(2011, 12, 1));

                var sr = (from r in reports.ToList() where r.PeriodStart == new DateTime(2011, 11, 7) select r).Single();
                Assert.AreEqual(95, sr.Items.Count);

                sr = (from r in reports.ToList() where r.PeriodStart == new DateTime(2011, 11, 14) select r).Single();
                Assert.AreEqual(96, sr.Items.Count);

                sr = (from r in reports.ToList() where r.PeriodStart == new DateTime(2011, 11, 21) select r).Single();
                Assert.AreEqual(93, sr.Items.Count);

                sr = (from r in reports.ToList() where r.PeriodStart == new DateTime(2011, 11, 28) select r).Single();
                Assert.AreEqual(93, sr.Items.Count);
            }
        }

        /// <summary>
        ///A test for ProjectRepository
        ///</summary>
        [TestMethod()]
        public void ProjectRepositoryTest()
        {
            var target = _kernel.Get<CsvStatusEtlBridge>();
            IProjectRepository expected = null; // TODO: Initialize to an appropriate value
            IProjectRepository actual;
            target.ProjectRepository = expected;
            actual = target.ProjectRepository;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for StatusReportRepository
        ///</summary>
        [TestMethod()]
        public void StatusReportRepositoryTest()
        {
            var target = _kernel.Get<CsvStatusEtlBridge>();
            IStatusReportRepository expected = null; // TODO: Initialize to an appropriate value
            IStatusReportRepository actual;
            target.StatusReportRepository = expected;
            actual = target.StatusReportRepository;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for TopicRepository
        ///</summary>
        [TestMethod()]
        public void TopicRepositoryTest()
        {
            var target = _kernel.Get<CsvStatusEtlBridge>();
            ITopicRepository expected = null; // TODO: Initialize to an appropriate value
            ITopicRepository actual;
            target.TopicRepository = expected;
            actual = target.TopicRepository;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
