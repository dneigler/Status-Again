using Status.Persistence;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using NHibernate;
using Status.Model;
using System.Collections.Generic;
using Status.Repository;
using System.Linq;
using Ninject;

namespace Status.Persistence.Tests
{
    
    
    /// <summary>
    ///This is a test class for TeamRepositoryTest and is intended
    ///to contain all TeamRepositoryTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TeamRepositoryTest
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
        ///A test for TeamRepository Constructor
        ///</summary>
        [TestMethod()]
        public void TeamRepositoryConstructorTest()
        {
            ITransaction transaction = null; // TODO: Initialize to an appropriate value
            TeamRepository target = new TeamRepository(transaction);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for TeamRepository Constructor
        ///</summary>
        [TestMethod()]
        public void TeamRepositoryConstructorTest1()
        {
            string connectionString = string.Empty; // TODO: Initialize to an appropriate value
            ISession session = null; // TODO: Initialize to an appropriate value
            TeamRepository target = new TeamRepository(connectionString, session);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for TeamRepository Constructor
        ///</summary>
        [TestMethod()]
        public void TeamRepositoryConstructorTest2()
        {
            ISession session = null; // TODO: Initialize to an appropriate value
            TeamRepository target = new TeamRepository(session);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for TeamRepository Constructor
        ///</summary>
        [TestMethod()]
        public void TeamRepositoryConstructorTest3()
        {
            TeamRepository target = new TeamRepository();
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for TeamRepository Constructor
        ///</summary>
        [TestMethod()]
        public void TeamRepositoryConstructorTest4()
        {
            string connectionString = string.Empty; // TODO: Initialize to an appropriate value
            TeamRepository target = new TeamRepository(connectionString);
            Assert.Inconclusive("TODO: Implement code to verify target");
        }

        /// <summary>
        ///A test for AddTeam
        ///</summary>
        [TestMethod()]
        public void AddTeamTest()
        {
            TeamRepository target = new TeamRepository(); // TODO: Initialize to an appropriate value
            Team team = null; // TODO: Initialize to an appropriate value
            target.AddTeam(team);
            Assert.Inconclusive("A method that does not return a value cannot be verified.");
        }

        /// <summary>
        ///A test for GetAllTeams
        ///</summary>
        [TestMethod()]
        public void GetAllTeamsTest()
        {
            TeamRepository target = new TeamRepository(); // TODO: Initialize to an appropriate value
            IList<Team> expected = null; // TODO: Initialize to an appropriate value
            IList<Team> actual;
            actual = target.GetAllTeams();
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetAllTeamsDetail
        ///</summary>
        [TestMethod()]
        public void GetAllTeamsDetailTest()
        {
            ITeamRepository target = _kernel.Get<ITeamRepository>();
            IList<Team> teams = target.GetAllTeamsDetail();
            // we should have the team member details
            teams.ToList().ForEach(t =>
            {
                Assert.IsTrue(t.Members.Count > 0);
            });
            
        }

        /// <summary>
        ///A test for GetTeamByName
        ///</summary>
        [TestMethod()]
        public void GetTeamByNameTest()
        {
            TeamRepository target = new TeamRepository(); // TODO: Initialize to an appropriate value
            string name = string.Empty; // TODO: Initialize to an appropriate value
            Team expected = null; // TODO: Initialize to an appropriate value
            Team actual;
            actual = target.GetTeamByName(name);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for GetTeamsByLead
        ///</summary>
        [TestMethod()]
        public void GetTeamsByLeadTest()
        {
            TeamRepository target = new TeamRepository(); // TODO: Initialize to an appropriate value
            string teamLeadEmail = string.Empty; // TODO: Initialize to an appropriate value
            IList<Team> expected = null; // TODO: Initialize to an appropriate value
            IList<Team> actual;
            actual = target.GetTeamsByLead(teamLeadEmail);
            Assert.AreEqual(expected, actual);
            Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
