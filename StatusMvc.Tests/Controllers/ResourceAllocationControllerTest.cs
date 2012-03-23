using StatusMvc.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using Status.Repository;
using Status.BLL;
using System.Collections.Generic;
using Ninject;
using Status.Persistence.Tests;
using Status.BLL.Tests;
using System.Web.Mvc;
using System.Linq;

namespace StatusMvc.Tests.Controllers
{
    
    
    /// <summary>
    ///This is a test class for ResourceAllocationControllerTest and is intended
    ///to contain all ResourceAllocationControllerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ResourceAllocationControllerTest
    {
        private static IKernel _kernel = null;
        private readonly static string _connString = "server=.\\SQLExpress;" +
            "database=StatusAgain;" +
            "Integrated Security=SSPI;";
        private readonly static NHibernateUnitTestConfiguration _config = new NHibernateUnitTestConfiguration(_connString);

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
            _kernel = new StandardKernel(new DefaultStatusNinjectModule(_connString));
            _kernel.Bind<ResourceAllocationController>().ToSelf();
            
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
        ///A test for GetMonthsFromRange
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void GetMonthsFromRangeTest()
        {
            var target = _kernel.Get<ResourceAllocationController>();
            var from = new DateTime(2011, 1, 1);
            var to = new DateTime(2011, 3, 1);
            var actual = target.GetMonthsFromRange(from, to);
            Assert.AreEqual(3, actual.Count);
            Assert.AreEqual(to, actual[2]);
            Assert.AreEqual(new DateTime(2011, 2, 1), actual[1]);
            Assert.AreEqual(from, actual[0]);
        }

        [TestMethod()]
        public void GetMonthsFromRangeOffDateTest()
        {
            var target = _kernel.Get<ResourceAllocationController>();
            var from = new DateTime(2011, 1, 27);
            var to = new DateTime(2011, 3, 1);
            var actual = target.GetMonthsFromRange(from, to);
            Assert.AreEqual(3, actual.Count);
            Assert.AreEqual(to, actual[2]);
            Assert.AreEqual(new DateTime(2011, 2, 1), actual[1]);
            Assert.AreEqual(from, actual[0]);
        }

        /// <summary>
        ///A test for GetResourceAllocations
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        //[HostType("ASP.NET")]
        //[AspNetDevelopmentServerHost("C:\\Development\\Home\\StatusAgain\\StatusMvc", "/")]
        //[UrlToTest("http://localhost:57254/")]
        public void GetResourceAllocationsTest()
        {
            var target = _kernel.Get<ResourceAllocationController>();
            var from = new DateTime(2011, 1, 1);
            var to = new DateTime(2012, 3, 1);
            var actual = target.GetResourceAllocationsVM(from, to);
            // check months count
            Assert.AreEqual(15, actual.Months.Count);
            actual.Teams.ToList().ForEach(t => {
                t.Members.ToList().ForEach(m =>
                {
                    m.Projects.ToList().ForEach(p =>
                    {
                        Assert.AreEqual(actual.Months.Count, p.MonthlyAllocations.Count);
                    });
                });
            });
        }
    }
}
