using System.IO;
using System.Linq;
using Ninject;
using Status.ETL.Csv;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Status.Model;
using Status.Persistence.Tests;
using Status.Repository;
using System.Collections.Generic;

namespace Status.Etl.Tests
{
    
    
    /// <summary>
    ///This is a test class for CsvResourceAllocationBridgeTest and is intended
    ///to contain all CsvResourceAllocationBridgeTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CsvResourceAllocationBridgeTest
    {


        private TestContext testContextInstance;
        private static StandardKernel _kernel;

        public static StandardKernel Kernel
        {
            get { return CsvResourceAllocationBridgeTest._kernel; }
            set { CsvResourceAllocationBridgeTest._kernel = value; }
        }

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
            // following call clears the database, commented for now as easier to test
            // with the CsvStatusItem import run first
            //_config.Configure();
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
        ///A test for CsvResourceAllocationBridge Constructor
        ///</summary>
        [TestMethod()]
        public void CsvResourceAllocationBridgeConstructorTest()
        {
            IResourceAllocationRepository resourceAllocationRepository = null; // TODO: Initialize to an appropriate value
            IProjectRepository projectRepository = null; // TODO: Initialize to an appropriate value
            IResourceRepository resourceRepository = null; // TODO: Initialize to an appropriate value
            ITeamRepository teamRepository = null; // TODO: Initialize to an appropriate value
            IDepartmentRepository departmentRepository = null; // TODO: Initialize to an appropriate value
            CsvResourceAllocationBridge target = new CsvResourceAllocationBridge(resourceAllocationRepository, projectRepository, resourceRepository, teamRepository, departmentRepository);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for UpsertResourceAllocations
        ///</summary>
        [TestMethod()]
        public void UpsertResourceAllocationsTest()
        {
            var target = _kernel.Get<ICsvResourceAllocationBridge>();
            var etl = _kernel.Get<CsvResourceAllocationEtl>();

            using (TextReader file = new StreamReader(@"..\..\..\..\ResourceAllocationSample.csv"))
            {
                var items = etl.ImportAllocations(file);
                target.UpsertResourceAllocations(items);
                // ensure proper status items come back
                var srRepo = _kernel.Get<IResourceAllocationRepository>();
                IList<ResourceAllocation> allocations = srRepo.GetAll();

                Assert.AreEqual(993, allocations.Count);
                
            }
        }
    }
}
