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
using Status.Model;
using NHibernate.Linq;
using FluentNHibernate.Automapping;

namespace Status.Persistence.Tests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class UnitTest1
    {
        const string connString = "server=.\\SQLExpress;" +
            "database=StatusAgain;" +
            "Integrated Security=SSPI;";

        public UnitTest1()
        {
            //
            // TODO: Add constructor logic here
            //
        }

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
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext) {
                              // FluentMappings.AddFromAssemblyOf<ProjectMap>()
            //var cfg = new StoreConfiguration();
            //Fluently.Configure()
            //  .Database(MsSqlConfiguration
            //    .MsSql2008
            //    .ConnectionString(connString))
            //  .Mappings(m => 
            //      m.FluentMappings.Conventions.AddFromAssemblyOf<NoUnderscoreForeignKeyConvention>())
            //      .Mappings(m => m.AutoMappings.Add(AutoMap.AssemblyOf<Project>(cfg)
            //          .UseOverridesFromAssemblyOf<ProjectAutoMap>())
            //  )
            //  .ExposeConfiguration(CreateSchema)
            //  .BuildConfiguration();

            Fluently.Configure()
              .Database(MsSqlConfiguration
                .MsSql2008
                .ConnectionString(connString))
              .Mappings(m => m.FluentMappings.AddFromAssemblyOf<ProjectMap>()
              .Conventions.AddFromAssemblyOf<NoUnderscoreForeignKeyConvention>())
              .ExposeConfiguration(CreateSchema)
              .BuildConfiguration();
        }

        private static void CreateSchema(Configuration cfg)
        {
            var schemaExport = new SchemaExport(cfg);
            schemaExport.Drop(false, true);
            schemaExport.Create(false, true);
        }

        //
        // Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup() { 
            
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

        [TestMethod]
        public void TestMethod1()
        {
            //
            // TODO: Add test logic here
            //
            Assert.IsTrue(true, "RanTestMethod1");
            var factory = CreateSessionFactory();
            using (var session = factory.OpenSession())
            {
                var employee = new Employee {
                    FirstName = "Dave",
                    LastName = "Neigler",
                    EmailAddress = "test@test.com"
                };
                var team = new Team {
                    Lead = employee,
                    Name = "Test Team"};
                
                var department = new Department {
                    Name = "Operations IT"
                };

                var project = new Project
                {
                    Name = "Test Project 1",
                    StartDate = DateTime.Parse("01/01/2012"),
                    EndDate = DateTime.Parse("07/01/2012"),
                    Description = "Test project description",
                    JiraProject = "TESTPROJ",
                    Team = team,
                    Type = ProjectType.Grow,
                    Department = department
                };
                var project2 = new Project
                {
                    Name = "Test Project 2",
                    StartDate = DateTime.Parse("01/01/2012"),
                    EndDate = DateTime.Parse("07/01/2012"),
                    Description = "Test project 2 description",
                    JiraProject = "TESTPROJ2",
                    Team = team,
                    Type = ProjectType.Grow,
                    Department = department
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
                var status2 = new StatusItem {
                    Topic = topic2
                };
                var status3 = new StatusItem
                {
                    Topic = topic3
                };
                session.Save(employee);
                session.Save(team);
                session.Save(department);
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
                Topic topicRetVal3 = session.Get<Topic>(topic3.Id);
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

        private ISessionFactory CreateSessionFactory()
        {
            return Fluently.Configure()
              .Database(MsSqlConfiguration
                .MsSql2008
                .ConnectionString(connString))
              .Mappings(m => m.FluentMappings
                .AddFromAssemblyOf<ProjectMap>()
                .Conventions.AddFromAssemblyOf<NoUnderscoreForeignKeyConvention>())
              .BuildSessionFactory();
        }
    }
}
