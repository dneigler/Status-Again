using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Status.Repository;
using Status.Model;
using NLog;
using Status.BLL;
using AutoMapper;
using StatusMvc.Models;

namespace StatusMvc.Controllers
{
    public class ResourceAllocationController : Controller
    {
        private DateTime startDate, endDate;
        private readonly Logger _logger = NLog.LogManager.GetCurrentClassLogger();

        private IStatusReportRepository _repository;

        public IStatusReportRepository Repository
        {
            get { return _repository; }
            set { _repository = value; }
        }
        private ITopicRepository _topicRepository;

        public ITopicRepository TopicRepository
        {
            get { return _topicRepository; }
            set { _topicRepository = value; }
        }
        private IProjectRepository _projectRepository;

        public IProjectRepository ProjectRepository
        {
            get { return _projectRepository; }
            set { _projectRepository = value; }
        }
        private IResourceRepository _resourceRepository;

        public IResourceRepository ResourceRepository
        {
            get { return _resourceRepository; }
            set { _resourceRepository = value; }
        }
        private ITagRepository _tagRepository;

        public ITagRepository TagRepository
        {
            get { return _tagRepository; }
            set { _tagRepository = value; }
        }
        private IStatusReportManager _statusReportManager;

        public IStatusReportManager StatusReportManager
        {
            get { return _statusReportManager; }
            set { _statusReportManager = value; }
        }

        private ITeamRepository _teamRepository;

        public ITeamRepository TeamRepository
        {
            get { return _teamRepository; }
            set { _teamRepository = value; }
        }

        private IResourceAllocationRepository _resourceAllocationRepository;

        public IResourceAllocationRepository ResourceAllocationRepository
        {
            get { return _resourceAllocationRepository; }
            set { _resourceAllocationRepository = value; }
        }

        
        public ResourceAllocationController(IStatusReportRepository repository, ITopicRepository topicRepository, IProjectRepository projectRepository, IResourceRepository resourceRepository, IStatusReportManager statusReportManager, ITeamRepository teamRepository, ITagRepository tagRepository, IResourceAllocationRepository resourceAllocationRepository)
        {
            
            _repository = repository;
            _topicRepository = topicRepository;
            _projectRepository = projectRepository;
            _resourceRepository = resourceRepository;
            _tagRepository = tagRepository;
            _teamRepository = teamRepository;
            _statusReportManager = statusReportManager;
            _resourceAllocationRepository = resourceAllocationRepository;
            
            Mapper.CreateMap<Project, StatusMvc.Models.ResourceAllocationViewModel.TeamAllocationRAVM.ProjectRAVM>();

            
            Mapper.CreateMap<Team, StatusMvc.Models.ResourceAllocationViewModel.TeamAllocationRAVM>();
        }

        //
        // GET: /ResourceAllocation/
        
        public ActionResult Index()
        {
            
            //var allocs = this.ResourceAllocationRepository.GetAll();
            //// reconstruct the allocations by team
            //// or just use NHibernate to help here

            //// allocation -> user, then work back down?
            //var resourceGroup = allocs.GroupBy(item => item.Resource as Employee);

            //resourceGroup.ToList().ForEach(rg =>
            //{
            //    // each resource group by team?

            //    var emp = rg.Key;

            //});
            return View();
        }

        /// <summary>
        /// Gets all resource allocations
        /// </summary>
        /// <returns></returns>
        public JsonResult GetResourceAllocations()
        {
            if (!DateTime.TryParse(Request.QueryString["startDate"], out startDate))
                startDate = new DateTime(2011, 1, 1);
            if (!DateTime.TryParse(Request.QueryString["endDate"], out endDate))
                endDate = new DateTime(2011, 1, 1);
            
            // in order to load ahead, pull all allocs in date range and pass them to the VM resolver
            var allocs = this.ResourceAllocationRepository.GetResourceAllocationsByDateRange(startDate, endDate);

            //Mapper.CreateMap<Employee, StatusMvc.Models.ResourceAllocationViewModel.TeamAllocationRAVM.UserRAVM>()
            //    .ForMember(userRAVM => userRAVM.Projects,
            //        opt =>
            //        {
            //            opt.ResolveUsing<StatusMvc.Models.ResourceAllocationViewModel.TeamAllocationRAVM.ProjectRAVMResolver>()
            //                .ConstructedBy(() => new StatusMvc.Models.ResourceAllocationViewModel.TeamAllocationRAVM.ProjectRAVMResolver(allocs))
            //                .FromMember(r => r.Id);
            //        });

            var teams = this.TeamRepository.GetAllTeamsDetail();
            var data = Mapper.Map<IList<Team>, IList<StatusMvc.Models.ResourceAllocationViewModel.TeamAllocationRAVM>>(teams);
            // data needs the projecs filled in
            data.ToList().ForEach(team =>
            {
                team.Members.ToList().ForEach(member =>
                {
                    // pull all projects 
                    member.Projects.ToList().ForEach(project =>
                    {
                        // group the allocs by month
                        //project.MonthlyAllocations.Clear();
                        var subAllocs = allocs.Where(alloc => alloc.Project.Id == project.Id && alloc.Resource.Id == member.Id);

                        // no need to group as allocs should only have single
                        subAllocs.OrderBy(sa => sa.Month).ToList().ForEach(mg =>
                        {
                            project.MonthlyAllocations.Add(Mapper.Map<ResourceAllocation, ResourceAllocationViewModel.TeamAllocationRAVM.MonthRAVM>(mg));

                        });
                    });
                });

            });
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        //
        // GET: /ResourceAllocation/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /ResourceAllocation/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /ResourceAllocation/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
        
        //
        // GET: /ResourceAllocation/Edit/5
 
        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /ResourceAllocation/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /ResourceAllocation/Delete/5
 
        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /ResourceAllocation/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here
 
                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
