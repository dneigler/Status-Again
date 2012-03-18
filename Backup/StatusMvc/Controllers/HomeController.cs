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
    public class HomeController : Controller
    {
        private IStatusReportRepository _repository;

        public HomeController(IStatusReportRepository repository)
        {
            _repository = repository;
        }

        public ActionResult Index()
        {
            Mapper.CreateMap<StatusReport, StatusReportViewModel>()
                .ForMember(m => m.NumberOfStatusItems, opt => opt.ResolveUsing<NumberOfStatusItemsFormatter>())
                .ForMember(m=>m.StatusReportDates, opt => opt.Ignore())
                .ForMember(m => m.ItemsToRemove, opt => opt.Ignore())
                .ForMember(m => m.Projects, opt => opt.Ignore())
                .ForMember(m => m.Tags, opt => opt.Ignore())
                .ForMember(m => m.CanRollStatus, opt => opt.Ignore())
                .ForMember(m => m.RollStatusDate, opt => opt.Ignore());
            Mapper.CreateMap<StatusItem, StatusReportItemViewModel>()
                .ForMember(m => m.TagsString, opt => opt.Ignore())
                ;
                /*.ForMember(m => m.Topic, opt => opt.Ignore())
                .ForMember(m => m.Milestone, opt => opt.Ignore())
                .ForMember(m => m.AuditInfo, opt => opt.Ignore())
                .ForMember(m => m.StatusReport, opt => opt.Ignore())
                .ForMember(m => m.Tags, opt => opt.Ignore())
                .ForMember(m => m.Project, opt => opt.Ignore());*/

            Mapper.CreateMap<Project, ProjectViewModel>()
                .ForMember(m => m.ProjectTeamId, opt => opt.Ignore());

            Mapper.AssertConfigurationIsValid();
            
            /*
             * The following 6 properties on Status.Model.StatusItem are not mapped: 
	Topic
	Milestone
	AuditInfo
	StatusReport
	Tags
	Project
Add a custom mapping expression, ignore, or rename the property on StatusMvc.Models.StatusReportItemViewModel.
             * */

            //.ForMember(dest => dest.ProjectLeadFullName, opt => opt.MapFrom(src => src.Project.Lead.FullName))
                //.ForMember(dest => dest.ProjectTeamLeadFullName, opt => opt.MapFrom(src => src.Project.Team.Lead.FullName));

            var data = _repository.GetAllStatusReports();
            var vm = Mapper.Map<IList<StatusReport>, IList<StatusReportViewModel>>(data);

            ViewBag.Message = "Status Reports";

            return View(vm);
        }

        public ActionResult About()
        {
            return View();
        }
    }

    public class NumberOfStatusItemsFormatter : ValueResolver<StatusReport, int>
    {
        protected override int ResolveCore(StatusReport source)
        {
            return source.Items.Count;
        }
    }
}
