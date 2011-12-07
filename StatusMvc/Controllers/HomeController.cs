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
                .ForMember(m => m.NumberOfStatusItems, opt => opt.ResolveUsing<NumberOfStatusItemsFormatter>());
            Mapper.CreateMap<StatusItem, StatusReportItemViewModel>();
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
