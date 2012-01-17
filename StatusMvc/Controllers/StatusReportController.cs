using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
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

        
        public StatusReportController(IStatusReportRepository repository, ITopicRepository topicRepository, IProjectRepository projectRepository)
        {
            _repository = repository;
            _topicRepository = topicRepository;
            _projectRepository = projectRepository;
            Mapper.CreateMap<StatusReport, StatusReportViewModel>()
                .ForMember(m => m.NumberOfStatusItems, opt => opt.ResolveUsing<NumberOfStatusItemsFormatter>());
            Mapper.CreateMap<StatusItem, StatusReportItemViewModel>();
            Mapper.CreateMap<StatusReportItemViewModel, StatusItem>();
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

        //
        // GET: /StatusReport/

        public ActionResult Index()
        {
            return View();
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
            var vm = Mapper.Map<StatusReport, StatusReportViewModel>(data);

            return Json(vm, JsonRequestBehavior.AllowGet);
        }

        ////
        //// GET: /StatusReport/Details/5

        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        [HttpPost]
        public JsonResult Save(StatusReportViewModel report)
        {
            report.Items.ToList().ForEach(r =>
                                        {
                                            // need to map this back to status report
                                            var sri = Mapper.Map<StatusReportItemViewModel, StatusItem>(r);
                                            // if topic doesn't exist yet, we should create
                                            if (string.IsNullOrEmpty(sri.Caption)) throw new ArgumentNullException("Caption cannot be null!");
                                            Topic topic = null;
                                            if (r.TopicId != 0)
                                                topic = this.TopicRepository.Get(r.TopicId);
                                            else topic = this.TopicRepository.GetOrAddTopicByCaption(sri.Caption);
                                            Project project = this.ProjectRepository.Get(r.ProjectId);
                                            sri.Topic = topic;
                                            sri.Project = project;
                                            this.StatusReportRepository.UpsertStatusReportItem(sri);
                                        });
            return Json(report, JsonRequestBehavior.AllowGet);
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
        //}

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
