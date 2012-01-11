﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate;
using Status.Model;
using Status.Repository;
using NLog;

namespace Status.ETL.Csv
{
    public class CsvResourceAllocationBridge : ICsvResourceAllocationBridge
    {
        private readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public IResourceAllocationRepository ResourceAllocationRepository { get; set; }
        public IProjectRepository ProjectRepository { get; set; }

        public IResourceRepository ResourceRepository { get; set; }

        public ITeamRepository TeamRepository { get; set; }

        public IDepartmentRepository DepartmentRepository { get; set; }

        public CsvResourceAllocationBridge(IResourceAllocationRepository resourceAllocationRepository, IProjectRepository projectRepository, IResourceRepository resourceRepository, ITeamRepository teamRepository, IDepartmentRepository departmentRepository)
        {
            ResourceAllocationRepository = resourceAllocationRepository;
            ProjectRepository = projectRepository;
            ResourceRepository = resourceRepository;
            TeamRepository = teamRepository;
            DepartmentRepository = departmentRepository;
        }

        public void UpsertResourceAllocations(IList<ResourceAllocationCsvItem> items)
        {
            _logger.Info("UpsertStatus called for {0} items", items.Count);

            ITransaction transaction = this.ResourceAllocationRepository.BeginTransaction();
            try
            {
                // clear all entries for each resource / month combination before feeding the database
                var grpResourceMonths = items.GroupBy(item => new {item.Name, item.Month})
                    ;
                //.Select(group => new
                //                     {
                //                         EmployeeID = group.Key.EmployeeID,
                //                         Month = group.Key.Month
                //                     });

                grpResourceMonths.ToList().ForEach(
                    grm =>
                        {
                            var rm = grm.Key;

                            var firstEntry = grm.First();
                            // lookup resource id by external id
                            var resource = this.ResourceRepository.GetResourcesByName(rm.Name).FirstOrDefault();
                            if (resource == null)
                            {
                                this.ResourceRepository.AddResource(new Employee()
                                                                        {
                                                                            FullName = firstEntry.Name,
                                                                            EmailAddress =
                                                                                String.Format("{0}@test.com",
                                                                                              firstEntry.Name.Replace(" ",
                                                                                                                      "."))
                                                                        });
                                resource = this.ResourceRepository.GetResourcesByName(firstEntry.Name).FirstOrDefault();
                            }
                            this.ResourceAllocationRepository.DeleteByResourceMonth(resource, rm.Month);

                            // add allocations now?
                            // loop through
                            grm.ToList().ForEach(item =>
                                                     {
                                                         // look up project first
                                                         var project =
                                                             this.ProjectRepository.GetProjectByName(item.Project);

                                                         if (project == null)
                                                         {
                                                             var pItem = firstEntry;
                                                             // var pTeam = this.TeamRepository.GetTeamByName() // not going to work with this file format as team id is made up
                                                             var pLead = this.ResourceRepository.GetResourcesByName(pItem.ResourceTeamLead)[0];

                                                             // create dummy department
                                                             var department = this.DepartmentRepository.GetByName("Department");
                                                             if (department == null)
                                                             {
                                                                 department = new Department() { Name = "Department", Manager = pLead as Employee };
                                                                 this.DepartmentRepository.Add(department);
                                                                 department = this.DepartmentRepository.GetByName("Department");
                                                             }

                                                             project = new Project()
                                                             {
                                                                 Name = item.Project,
                                                                 Caption = item.ProjectType,
                                                                 Lead = pLead as Employee,
                                                                 Department = department
                                                             };
                                                             this.ProjectRepository.AddProject(project);
                                                             project = this.ProjectRepository.GetProjectByName(item.Project);
                                                         }
                                                         var allocation = new ResourceAllocation
                                                                              {
                                                                                  Month = item.Month,
                                                                                  Project = project,
                                                                                  Resource = resource,
                                                                                  Allocation = item.AllocationPercentage
                                                                              };
                                                         this.ResourceAllocationRepository.Add(allocation);
                                                     });
                        }
                    );
                this.ResourceAllocationRepository.CommitTransaction();
            }
            catch (Exception exc)
            {
                _logger.ErrorException("Unable to import from CsvResourceAllocation file", exc);
                this.ResourceAllocationRepository.RollbackTransaction();
                throw;
            }
            finally
            {
            }
        }
    }
}
