using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using NHibernate.Cfg;
using NHibernate.Tool.hbm2ddl;
using NHibernate;
using Ninject;
using Status.Model;
using NHibernate.Linq;
using FluentNHibernate.Automapping;
using Status.Repository;

namespace Status.Persistence.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        const string ConnString = "server=.\\SQLExpress;" +
            "database=StatusAgain;" +
            "Integrated Security=SSPI;";
        private static StandardKernel _kernel;
        private static NHibernateUnitTestConfiguration _config = new NHibernateUnitTestConfiguration(ConnString);
        private IStatusReportRepository _statusReportRepository;


        public UnitTest1()
        {
            IStatusReportRepository statusReportRepository = _kernel.Get<IStatusReportRepository>();
            StatusReportRepository = statusReportRepository;
        }

        private TestContext _testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return _testContextInstance;
            }
            set
            {
                _testContextInstance = value;
            }
        }

        public IStatusReportRepository StatusReportRepository
        {
            get { return _statusReportRepository; }
            set { _statusReportRepository = value; }
        }

        #region Additional test attributes

        private static Employee _employee;
        private static Team _team;
        private static Department _department;

        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            _config.Configure();
            _kernel = new StandardKernel(new DefaultEtlNinjectModule(ConnString));
            var factory = _config.CreateSessionFactory();
            using (var session = factory.OpenSession())
            {
                _employee = new Employee
                                   {
                                       FirstName = "Dave",
                                       LastName = "Neigler",
                                       EmailAddress = "test@test.com"
                                   };
                _team = new Team
                               {
                                   Lead = _employee,
                                   Name = "Test Team"
                               };

                _department = new Department
                                     {
                                         Name = "Operations IT"
                                     };
                session.Save(_employee);
                session.Save(_team);
                session.Save(_department);
            }

        }

        //
        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {

        }

        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [Description("Basic persistence test creates database."), TestMethod]
        public void BasicPersistenceTest()
        {
            Assert.IsTrue(true, "RanTestMethod1");
            var factory = _config.CreateSessionFactory();
            using (var session = factory.OpenSession())
            {
                var project = new Project
                {
                    Name = "Test Project 1",
                    StartDate = DateTime.Parse("01/01/2012"),
                    EndDate = DateTime.Parse("07/01/2012"),
                    Description = "Test project description",
                    JiraProject = "TESTPROJ",
                    Team = _team,
                    Type = ProjectType.Grow,
                    Department = _department
                };
                var project2 = new Project
                {
                    Name = "Test Project 2",
                    StartDate = DateTime.Parse("01/01/2012"),
                    EndDate = DateTime.Parse("07/01/2012"),
                    Description = "Test project 2 description",
                    JiraProject = "TESTPROJ2",
                    Team = _team,
                    Type = ProjectType.Grow,
                    Department = _department
                };

                var topic1 = new JiraIssueTopic
                {
                    JiraId = "BOTEST-1",
                    Caption = "This is the caption"
                };
                var topic2 = new JiraProjectTopic
                {
                    JiraProjectId = "PROJ-1",
                    Caption = "Test project"
                };
                var topic3 = new Topic
                {
                    Caption = "Standard topic"
                };
                var status1 = new StatusItem
                {
                    Caption = "Status",
                    Topic = topic1
                };
                var status2 = new StatusItem
                {
                    Topic = topic2
                };
                var status3 = new StatusItem
                {
                    Topic = topic3
                };
                session.Save(project);
                session.Save(project2);
                session.Save(topic1);
                session.Save(topic2);
                session.Save(topic3);
                session.Save(status1);
                session.Save(status2);
                session.Save(status3);
                Assert.AreNotEqual(topic1.Caption, status1.Caption);
                Assert.AreEqual(topic2.Caption, status2.Caption);
                Assert.AreEqual(topic3.Caption, status3.Caption);
                // delete status shouldn't remove topic
                session.Delete(status3);
                var topicRetVal3 = session.Get<Topic>(topic3.Id);
                Assert.IsNotNull(topicRetVal3);
            }

            using (var session2 = factory.OpenSession())
            {
                var project = session2.Get<Project>(1);
                Assert.IsNotNull(project);
                Assert.AreEqual<string>(project.Department.Name, "Operations IT");

                var projects = session2.Query<Project>()
                    .OrderBy(c => c.Name)
                    .ToList();
                Assert.AreEqual(2, projects.Count);

            }
        }

        [Description("Cascade insertion of items into status report Items collection"), TestMethod]
        public void StatusReportCascadingPersistenceTest()
        {
            const string caption1 = "Status Item StatusReportCascadingPersistenceTest";
            const string caption2 = "2-Status Item StatusReportCascadingPersistenceTest";

            var factory = _config.CreateSessionFactory();
            int srId = 0;
            Project project = null;
            Topic topic2 = null;
            using (var session = factory.OpenSession())
            {
                using (var txn = session.BeginTransaction())
                {
                    var employee = new Employee
                                       {
                                           FirstName = "Dave",
                                           LastName = "Neigler",
                                           EmailAddress = "test@test.com"
                                       };
                    session.SaveOrUpdate(employee);

                    var team = new Team
                                   {
                                       Lead = employee,
                                       Name = "Test Team"
                                   };
                    session.SaveOrUpdate(team);

                    var department = new Department
                                         {
                                             Name = "Operations IT"
                                         };
                    session.SaveOrUpdate(department);

                    project = new Project
                                      {
                                          Name = "Test Project StatusReportCascadingPersistenceTest",
                                          StartDate = DateTime.Parse("01/01/2012"),
                                          EndDate = DateTime.Parse("07/01/2012"),
                                          Description = "Test project description",
                                          JiraProject = "TESTPROJ",
                                          Team = team,
                                          Type = ProjectType.Grow,
                                          Department = department
                                      };
                    session.SaveOrUpdate(project);

                    var topic1 = new JiraIssueTopic
                                     {
                                         JiraId = "BOTEST-StatusReportCascadingPersistenceTest",
                                         Caption = "This is the caption"
                                     };

                    
                    session.SaveOrUpdate(topic1);
                    topic2 = new JiraIssueTopic
                    {
                        JiraId = "2-BOTEST-StatusReportCascadingPersistenceTest",
                        Caption = "This is the second caption"
                    }; 
                    session.SaveOrUpdate(topic2);
                    var sr = new StatusReport()
                                 {
                                     Caption = "Test Status Report 1",
                                     PeriodStart = new DateTime(2012, 1, 1),
                                     PeriodEnd = new DateTime(2012, 1, 7)
                                 };
                    
                    sr.Items.Add(new StatusItem
                                     {
                                         Caption = caption1,
                                         Milestone =
                                             new Milestone()
                                                 {
                                                     ConfidenceLevel = MilestoneConfidenceLevels.Proposed,
                                                     Type = MilestoneTypes.OpenItem
                                                 },
                                         Topic = topic1,
                                         Project = project
                                     });
                    session.SaveOrUpdate(sr);
                    srId = sr.Id;
                    Assert.AreNotEqual(0, srId);
                    txn.Commit();
                }

                using (var txn = session.BeginTransaction())
                {
                    var sr = (from r in session.Query<StatusReport>()
                              where r.Id == srId
                              select r).FirstOrDefault();

                    var statusItem = (from si in session.Query<StatusItem>()
                                      where si.Caption.Equals(caption1)
                                      select si).FirstOrDefault();
                    Assert.IsNotNull(statusItem);

                    Assert.IsNotNull(statusItem.Project);

                    Assert.IsNotNull(statusItem.Topic);

                    // now we add more items and see if it updates properly
                    sr.Items.Add(new StatusItem
                                     {
                                         Caption = caption2,
                                         Milestone =
                                             new Milestone()
                                                 {
                                                     ConfidenceLevel = MilestoneConfidenceLevels.Proposed,
                                                     Type = MilestoneTypes.OpenItem
                                                 },
                                         Topic = topic2,
                                         Project = project
                                     });

                    session.SaveOrUpdate(sr);

                    var statusItem2 = (from si in session.Query<StatusItem>()
                                       where si.Caption.Equals(caption2)
                                       select si).FirstOrDefault();
                    Assert.IsNotNull(statusItem2);

                    Assert.IsNotNull(statusItem2.Project);

                    Assert.IsNotNull(statusItem2.Topic);
                    txn.Commit();
                }
            }
        }
    }
}
