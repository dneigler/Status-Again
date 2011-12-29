using System.Configuration;
using System.Diagnostics;
using Status.Persistence;
using StatusMvc.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using Status.Repository;
using StatusMvc.Models;
using System.Web.Mvc;

namespace StatusMvc.Tests.Controllers
{
    
    
    /// <summary>
    ///This is a test class for StatusReportControllerTest and is intended
    ///to contain all StatusReportControllerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StatusReportControllerTest
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
        ///A test for StatusReportController Constructor
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Administrator\\Documents\\Visual Studio 2010\\Projects\\StatusAgain\\StatusMvc", "/")]
        [UrlToTest("http://localhost:57254/")]
        public void StatusReportControllerConstructorTest()
        {
            IStatusReportRepository repository = new StatusReportRepository(ConfigurationManager.ConnectionStrings["StatusAgain"].ConnectionString); // TODO: Initialize to an appropriate value
            StatusReportController target = new StatusReportController(repository);
            
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for Create
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Administrator\\Documents\\Visual Studio 2010\\Projects\\StatusAgain\\StatusMvc", "/")]
        [UrlToTest("http://localhost:57254/")]
        public void CreateTest()
        {
            
            IStatusReportRepository repository = null; // TODO: Initialize to an appropriate value
            StatusReportController target = new StatusReportController(repository); // TODO: Initialize to an appropriate value
            StatusReportViewModel vm = null; // TODO: Initialize to an appropriate value
            JsonResult expected = null; // TODO: Initialize to an appropriate value
            JsonResult actual;
            actual = target.Create(vm);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetAllStatusReports
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Administrator\\Documents\\Visual Studio 2010\\Projects\\StatusAgain\\StatusMvc", "/")]
        [UrlToTest("http://localhost:57254/")]
        public void GetAllStatusReportsTest()
        {
            IStatusReportRepository repository = null; // TODO: Initialize to an appropriate value
            StatusReportController target = new StatusReportController(repository); // TODO: Initialize to an appropriate value
            JsonResult expected = null; // TODO: Initialize to an appropriate value
            JsonResult actual;
            actual = target.GetAllStatusReports();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetStatusReport
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Administrator\\Documents\\Visual Studio 2010\\Projects\\StatusAgain\\StatusMvc", "/")]
        [UrlToTest("http://localhost:57254/")]
        public void GetStatusReportTest()
        {
            IStatusReportRepository repository = new StatusReportRepository(ConfigurationManager.ConnectionStrings["StatusAgain"].ConnectionString); // TODO: Initialize to an appropriate value
            StatusReportController target = new StatusReportController(repository);
            Nullable<DateTime> statusDate = null;// new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            JsonResult expected = null; // TODO: Initialize to an appropriate value
            JsonResult actual;
            actual = target.GetStatusReport(statusDate);
            Assert.IsNotNull(actual, "actual != null");
            Console.WriteLine("actual.Data= {0}", actual.Data);
            
        }

        /// <summary>
        ///A test for Index
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Administrator\\Documents\\Visual Studio 2010\\Projects\\StatusAgain\\StatusMvc", "/")]
        [UrlToTest("http://localhost:57254/")]
        public void IndexTest()
        {
            IStatusReportRepository repository = null; // TODO: Initialize to an appropriate value
            StatusReportController target = new StatusReportController(repository); // TODO: Initialize to an appropriate value
            ActionResult expected = null; // TODO: Initialize to an appropriate value
            ActionResult actual;
            actual = target.Index();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for StatusReportRepository
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        [HostType("ASP.NET")]
        [AspNetDevelopmentServerHost("C:\\Users\\Administrator\\Documents\\Visual Studio 2010\\Projects\\StatusAgain\\StatusMvc", "/")]
        [UrlToTest("http://localhost:57254/")]
        public void StatusReportRepositoryTest()
        {
            IStatusReportRepository repository = null; // TODO: Initialize to an appropriate value
            StatusReportController target = new StatusReportController(repository); // TODO: Initialize to an appropriate value
            IStatusReportRepository actual;
            actual = target.StatusReportRepository;
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
