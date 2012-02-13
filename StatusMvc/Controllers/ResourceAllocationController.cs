using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Status.Repository;
using Status.Model;

namespace StatusMvc.Controllers
{
    public class ResourceAllocationController : Controller
    {
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

        public ResourceAllocationController(IStatusReportRepository repository, ITopicRepository topicRepository, IProjectRepository projectRepository, IResourceRepository resourceRepository, IStatusReportManager statusReportManager, ITeamRepository teamRepository, ITagRepository tagRepository)
        {
            _repository = repository;
            _topicRepository = topicRepository;
            _projectRepository = projectRepository;
            _resourceRepository = resourceRepository;
            _tagRepository = tagRepository;
            _teamRepository = teamRepository;
            _statusReportManager = statusReportManager;

            
            Mapper.CreateMap<StatusReport, StatusReportViewModel>()
                .ForMember(m => m.NumberOfStatusItems, opt => opt.ResolveUsing<NumberOfStatusItemsFormatter>());
            Mapper.CreateMap<StatusItem, StatusReportItemViewModel>()
                .ForMember(m => m.TagsString, opt => opt.MapFrom(src =>
                    String.Join(",", (from tag in src.Tags
                                      select tag.Name))));

            Mapper.CreateMap<StatusReportItemViewModel, StatusItem>();

            Mapper.CreateMap<Project, ProjectViewModel>();

            Mapper.CreateMap<Tag, TagViewModel>();
        }

        //
        // GET: /ResourceAllocation/
        
        public ActionResult Index()
        {
            var teams = this.TeamRepository.GetAllTeamsDetail();
            var allocs = this.ResourceAllocationRepository.GetAll();
            // reconstruct the allocations by team
            // or just use NHibernate to help here

            // allocation -> user, then work back down?
            var resourceGroup = allocs.GroupBy(item => item.Resource as Employee);
            
            resourceGroup.ToList().ForEach(rg => {
                // each resource group by team?
                
                var emp = rg.Key;
                emp.Team
            })
            return View();
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
