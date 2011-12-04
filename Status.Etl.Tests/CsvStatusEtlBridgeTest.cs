using System.IO;
using Ninject;
using Status.ETL.Csv;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Status.Repository;
using Status.Etl.Csv;
using System.Collections.Generic;

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
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            _kernel = new StandardKernel(new DefaultEtlNinjectModule());
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
            }
        }

        /// <summary>
        ///A test for ProjectRepository
        ///</summary>
        [TestMethod()]
        public void ProjectRepositoryTest()
        {
            IStatusReportRepository statusReportRepository = null; // TODO: Initialize to an appropriate value
            IProjectRepository projectRepository = null; // TODO: Initialize to an appropriate value
            ITopicRepository topicRepository = null; // TODO: Initialize to an appropriate value
            CsvStatusEtlBridge target = new CsvStatusEtlBridge(statusReportRepository, projectRepository, topicRepository); // TODO: Initialize to an appropriate value
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
            IStatusReportRepository statusReportRepository = null; // TODO: Initialize to an appropriate value
            IProjectRepository projectRepository = null; // TODO: Initialize to an appropriate value
            ITopicRepository topicRepository = null; // TODO: Initialize to an appropriate value
            CsvStatusEtlBridge target = new CsvStatusEtlBridge(statusReportRepository, projectRepository, topicRepository); // TODO: Initialize to an appropriate value
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
            IStatusReportRepository statusReportRepository = null; // TODO: Initialize to an appropriate value
            IProjectRepository projectRepository = null; // TODO: Initialize to an appropriate value
            ITopicRepository topicRepository = null; // TODO: Initialize to an appropriate value
            CsvStatusEtlBridge target = new CsvStatusEtlBridge(statusReportRepository, projectRepository, topicRepository); // TODO: Initialize to an appropriate value
            ITopicRepository expected = null; // TODO: Initialize to an appropriate value
            ITopicRepository actual;
            target.TopicRepository = expected;
            actual = target.TopicRepository;
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
