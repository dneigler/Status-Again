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
            Mapper.CreateMap<StatusReport, StatusReportViewModel>();
            var data = _repository.GetAllStatusReports();
            var vm = Mapper.Map<IList<StatusReport>, IList<StatusReportViewModel>>(data);

            ViewBag.Message = "Welcome to ASP.NET MVC!";

            return View(vm);
        }

        public ActionResult About()
        {
            return View();
        }
    }
}
