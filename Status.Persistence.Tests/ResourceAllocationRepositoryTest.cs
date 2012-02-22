using Status.Persistence;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using NHibernate;
using Status.Model;
using System.Collections.Generic;
using Ninject;
using Status.Repository;

namespace Status.Persistence.Tests
{
    
    
    /// <summary>
    ///This is a test class for ResourceAllocationRepositoryTest and is intended
    ///to contain all ResourceAllocationRepositoryTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ResourceAllocationRepositoryTest
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
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            _kernel = new StandardKernel(new DefaultEtlNinjectModule(_connString));
            // _config.Configure();
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
        ///A test for GetResourceAllocationsByDateRange
        ///</summary>
        [TestMethod()]
        public void GetResourceAllocationsByDateRangeTest()
        {
            IResourceAllocationRepository target = _kernel.Get<IResourceAllocationRepository>(); // new ResourceAllocationRepository(session); // TODO: Initialize to an appropriate value
            DateTime from = new DateTime(2011, 01, 01); // TODO: Initialize to an appropriate value
            Nullable<DateTime> to = new Nullable<DateTime>(); // TODO: Initialize to an appropriate value
            IList<ResourceAllocation> actual;
            actual = target.GetResourceAllocationsByDateRange(from, to);
            Assert.IsTrue(actual.Count > 0);
        }
    }
}
