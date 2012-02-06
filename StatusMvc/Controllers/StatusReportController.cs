using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using Status.BLL;
using Status.Model;
using Status.Repository;
using StatusMvc.Models;

namespace StatusMvc.Controllers
{
    public class StatusReportController : Controller
    {
        private IStatusReportRepository _repository;
        private ITopicRepository _topicRepository;
        private IProjectRepository _projectRepository;
        private IResourceRepository _resourceRepository;
        private ITagRepository _tagRepository;
        private IStatusReportManager _statusReportManager;

        public StatusReportController(IStatusReportRepository repository, ITopicRepository topicRepository, IProjectRepository projectRepository, IResourceRepository resourceRepository, IStatusReportManager statusReportManager, ITagRepository tagRepository)
        {
            _repository = repository;
            _topicRepository = topicRepository;
            _projectRepository = projectRepository;
            _resourceRepository = resourceRepository;
            _tagRepository = tagRepository;
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
            //.ForMember(m => m.StatusReportId, opt => opt.M);
            //.ForMember(dest => dest.ProjectLeadFullName, opt => opt.MapFrom(src => src.Project.Lead.FullName))
            //.ForMember(dest => dest.ProjectTeamLeadFullName, opt => opt.MapFrom(src => src.Project.Team.Lead.FullName));

        }

        public IStatusReportRepository StatusReportRepository
        {
            get { return _repository; }
        }

        public ITopicRepository TopicRepository
        {
            get { return _topicRepository; }
        }

        public IProjectRepository ProjectRepository
        {
            get { return _projectRepository; }
        }

        public IResourceRepository ResourceRepository
        {
            get { return _resourceRepository; }
        }

        public IStatusReportManager StatusReportManager
        {
            get { return _statusReportManager; }
        }

        public ITagRepository TagRepository
        {
            get { return _tagRepository; }
        }

        //
        // GET: /StatusReport/

        public ActionResult Index()
        {
            var statusDate = GetStatusDateFromQueryString();
            ViewBag.Message = String.Format("Status Report for {0:MM/dd/yyyy}",
                                            statusDate);
            return View();
        }

        private DateTime GetStatusDateFromQueryString()
        {
            DateTime statusDate;
            bool validDate = DateTime.TryParse(Request.QueryString["statusDate"], out statusDate);
            if (!validDate)
                statusDate = this.StatusReportRepository.GetActiveStatusReportDate();
            return statusDate;
        }

        public JsonResult GetAllStatusReports()
        {
            var data = StatusReportRepository.GetAllStatusReports();
            var vm = Mapper.Map<IList<StatusReport>, IList<StatusReportViewModel>>(data);

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetStatusReport(DateTime? statusDate)
        {
            var data = statusDate.HasValue ? StatusReportRepository.GetStatusReport(statusDate.Value) : StatusReportRepository.GetActiveStatusReport();
            // trim data
            //var lst = data.Items.ToList();
            //int count = 3;
            //lst.RemoveRange(count, data.Items.Count - count);
            //data.Items = lst;

            var vm = GetStatusReportViewModel(data);
            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        private StatusReportViewModel GetStatusReportViewModel(StatusReport data)
        {
            var statusReportDate = data.PeriodStart;
            var vm = Mapper.Map<StatusReport, StatusReportViewModel>(data);
            // populate the recent dates
            if (vm != null)
                vm.StatusReportDates = this.StatusReportRepository.GetAllStatusReportDates();
            // determine whether we can roll status
            this.StatusReportManager.RollStatusDateProcessor.GetStatusReportDate(statusReportDate);
            DateTime statusRollDate;
            vm.CanRollStatus = this.StatusReportManager.CanRollStatusReport(data, out statusRollDate);
            if (vm.CanRollStatus) vm.RollStatusDate = statusRollDate;

            // get project names
            IList<Project> projects = this.ProjectRepository.GetAllProjects();
            vm.Projects = Mapper.Map<IList<Project>, IList<ProjectViewModel>>(projects);
            IList<Tag> tags = this.TagRepository.GetAllTags();
            vm.Tags = Mapper.Map<IList<Tag>, IList<TagViewModel>>(tags);
            //vm.Projects = this.ProjectRepository.GetAllProjects().Select(p => new {
            //    Name = p.Name, Id = p.Id, TeamLeadId = p.Team.Lead.Id }).ToList();
            return vm;
        }

        ////
        //// GET: /StatusReport/Details/5

        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        [HttpPost]
        public JsonResult RollStatus(StatusReportViewModel report)
        {
            // the following causes a problem - by reloading the status report from the database, we can't overwrite with 
            // objects being posted back by the client.  Either client provides all details of statusreport, or we go 
            // more manual on mapping back to actual objects.
            StatusReport sr = this.StatusReportRepository.Get(report.Id);
            this.StatusReportManager.StatusReportRepository = this.StatusReportRepository;
            var rolledReport = this.StatusReportManager.RollStatusReport(sr, GetAuditInfo());
            var vm = GetStatusReportViewModel(rolledReport);
            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Save(StatusReportViewModel report)
        {
            // the following causes a problem - by reloading the status report from the database, we can't overwrite with 
            // objects being posted back by the client.  Either client provides all details of statusreport, or we go 
            // more manual on mapping back to actual objects.
            StatusReport sr = this.StatusReportRepository.Get(report.Id);

            using (var txn = this.StatusReportRepository.BeginTransaction())
            {
                if (report.Items != null)
                {
                    report.Items.ToList().ForEach(r =>
                                                      {
                                                          // need to map this back to status report
                                                          StatusItem sri = null;

                                                          // using Id won't work with multiple new entries
                                                          if (r.Id > 0)
                                                          { // is new
                                                              var srSource =
                                                                  (from srSourceItem in sr.Items
                                                                   where srSourceItem.Id == r.Id
                                                                   select srSourceItem).FirstOrDefault();
                                                              // map over the new item
                                                              sri = Mapper.Map<StatusReportItemViewModel, StatusItem>(r, srSource);
                                                          }
                                                          else
                                                          {
                                                              sri = Mapper.Map<StatusReportItemViewModel, StatusItem>(r);
                                                          }
                                                          sri.Milestone.Type = r.MilestoneType;

                                                          sri.AuditInfo =
                                                              GetAuditInfo();
                                                          // if topic doesn't exist yet, we should create
                                                          if (string.IsNullOrEmpty(sri.Caption))
                                                              throw new ArgumentNullException("Caption cannot be null!");
                                                          Topic topic = null;
                                                          if (r.TopicId != 0)
                                                              topic = this.TopicRepository.Get(r.TopicId);
                                                          else
                                                              topic =
                                                                  this.TopicRepository.GetOrAddTopicByCaption(
                                                                      sri.Caption);
                                                          Project project = this.ProjectRepository.Get(r.ProjectId);
                                                          sri.Topic = topic;
                                                          sri.Project = project;
                                                          sri.Milestone.Date = r.MilestoneDate;

                                                          sr.AddStatusItem(sri);
                                                          sri.StatusReport = sr;

                                                          // update the tags
                                                          if (String.IsNullOrEmpty(r.TagsString))
                                                          {
                                                              sri.Tags.Clear();
                                                          }
                                                          else
                                                          {
                                                              var tagsList = r.TagsString.Split(',');
                                                              var tagsL = tagsList.ToList();
                                                              tagsL.ForEach(tag =>
                                                              {
                                                                  if (sri.Tags.FirstOrDefault(tagS => { return tagS.Name == tag; }) == null)
                                                                  {
                                                                      var t = this.TagRepository.GetOrAddTagByName(tag);
                                                                      sri.Tags.Add(t);
                                                                  }
                                                              });
                                                              // delete tags not found
                                                              sri.Tags.ToList().ForEach(tag =>
                                                              {
                                                                  if (tagsL.FirstOrDefault(tagName =>
                                                                  {
                                                                      return tagName == tag.Name;
                                                                  }) == null)
                                                                      sri.Tags.Remove(tag);
                                                              });
                                                          }
                                                          //this.StatusReportRepository.Update(sr);// each item any better

                                                      });
                }
                if (report.ItemsToRemove != null)   
                {
                    report.ItemsToRemove.ToList().ForEach(r =>
                                                              {
                                                                  var sri =
                                                                      Mapper.Map<StatusReportItemViewModel, StatusItem>(
                                                                          r);
                                                                  var sriDeleteItem = (from sriD in sr.Items
                                                                                       where sriD.Id == sri.Id
                                                                                       select sriD).First();
                                                                  sr.Items.Remove(sriDeleteItem);
                                                              });
                }
                this.StatusReportRepository.Update(sr); //.UpsertStatusReportItem(sri);
                txn.Commit();
            }
            return Json(report, JsonRequestBehavior.AllowGet);
        }

        private AuditInfo GetAuditInfo()
        {
            return new AuditInfo(
                this.ResourceRepository.GetOrCreateResourceByIIdentity
                    (User.Identity));
        }

        ////
        //// GET: /StatusReport/Create
        [HttpPost]
        public JsonResult Create(StatusReportViewModel vm)
        {
            StatusReport sr = null;
            Mapper.Map(vm, sr);
            if (sr == null)
                throw new NullReferenceException(
                    "Unable to convert StatusReportViewModel to StatusReport - invalid data?");
            // ReSharper disable HeuristicUnreachableCode
            // this is reachable as Mapper.Map will fill in sr
            if (sr.Id == 0)
                // create
                this.StatusReportRepository.Add(sr);
            else
                this.StatusReportRepository.Update(sr);
            //do the persistence logic here
            var message = "Status Report: " + vm.PeriodStart + " Saved";
            return Json(message);
            // ReSharper restore HeuristicUnreachableCode

        }

        ////
        //// POST: /StatusReport/Create

        //[HttpPost]
        //public ActionResult Create(FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        //
        // GET: /StatusReport/Edit/5

        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        ////
        //// POST: /StatusReport/Edit/5

        //[HttpPost]
        //public ActionResult Edit(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}F

        ////
        //// GET: /StatusReport/Delete/5

        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        ////
        //// POST: /StatusReport/Delete/5

        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
