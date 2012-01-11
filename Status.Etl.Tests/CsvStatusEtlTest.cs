using NLog;
using NLog.Config;
using NLog.Targets;
using Status.Etl.Csv;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Collections.Generic;
using Status.Model;

namespace Status.Etl.Tests
{
    
    
    /// <summary>
    ///This is a test class for CsvStatusEtlTest and is intended
    ///to contain all CsvStatusEtlTest Unit Tests
    ///</summary>
    [TestClass()]
    [DeploymentItem("NLog.config")]
    public class CsvStatusEtlTest
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
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            LogManager.Configuration = new XmlLoggingConfiguration(@"NLog.config");
            //LoggingConfiguration();// XmlLoggingConfiguration(@"NLog.config");
            //var ft = new FileTarget {FileName = typeof (CsvStatusEtlTest).Name + "_log.txt"};
            //var ct = new ConsoleTarget();
            //LogManager.Configuration.AddTarget("console", ct);
            //LogManager.Configuration.AddTarget("file", ft);
            //LogManager.Configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, ft));
            //LogManager.Configuration.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, ct));
            
        }
        
        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
        }
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
        ///A test for ImportAllocations
        ///</summary>
        [TestMethod]
        public void ImportStatusTest()
        {
            var target = new CsvStatusEtl();
            using (TextReader file = new StreamReader(@"..\..\..\..\StatusSample.csv"))
            {
                var actual = target.ImportStatus(file);

                // check values in first item
                Assert.AreEqual(new DateTime(2011, 11, 2), actual[0].MilestoneDate);
                Assert.AreEqual("Software Licensing", actual[0].Project);

                Assert.AreEqual(87, actual[actual.Count - 1].ProjectID);
                Assert.AreEqual(new DateTime(2011, 11, 28), actual[actual.Count - 1].StatusDate);
            }
        }

        /// <summary>
        ///A test for ExportAllocations
        ///</summary>
        [TestMethod()]
        public void ExportStatusTest()
        {
            CsvStatusEtl target = new CsvStatusEtl(); // TODO: Initialize to an appropriate value
            TextWriter file = null; // TODO: Initialize to an appropriate value
            IList<StatusReport> reports = null; // TODO: Initialize to an appropriate value
            target.ExportStatus(file, reports);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for ExportAllocations
        ///</summary>
        [TestMethod()]
        public void ExportStatusTest1()
        {
            CsvStatusEtl target = new CsvStatusEtl(); // TODO: Initialize to an appropriate value
            TextWriter file = null; // TODO: Initialize to an appropriate value
            IList<StatusCsvItem> items = null; // TODO: Initialize to an appropriate value
            target.ExportStatus(file, items);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for CsvStatusEtl Constructor
        ///</summary>
        [TestMethod()]
        public void CsvStatusEtlConstructorTest()
        {
            CsvStatusEtl target = new CsvStatusEtl();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }
    }
}
