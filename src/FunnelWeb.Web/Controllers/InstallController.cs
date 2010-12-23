﻿using System.Web.Mvc;
using FunnelWeb.DatabaseDeployer;
using FunnelWeb.DatabaseDeployer.Infrastructure;
using FunnelWeb.Settings;
using FunnelWeb.Web.Views.Install;

namespace FunnelWeb.Web.Controllers
{
    [ValidateInput(false)]
    public class InstallController : Controller
    {
        public IApplicationDatabase Database { get; set; }
        public IConnectionStringProvider ConnectionStringProvider { get; set; }

        [Authorize(Roles="Admin")]
        public virtual ActionResult Index()
        {
            var connectionString = ConnectionStringProvider.ConnectionString;

            string error;
            var model = new IndexModel();
            model.CanConnect = Database.TryConnect(connectionString, out error);
            model.ConnectionError = error;
            model.ConnectionString = connectionString;
            if (model.CanConnect)
            {
                model.CurrentVersion = Database.GetCurrentVersion(connectionString);
                model.NewVersion = Database.GetApplicationVersion(connectionString);
            }

            return View("Index", model);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ActionName("test")]
        public virtual ActionResult Test(string connectionString)
        {
            ConnectionStringProvider.ConnectionString = connectionString;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public virtual ActionResult Upgrade()
        {
            var log = new Log();
            var result = Database.PerformUpgrade(ConnectionStringProvider.ConnectionString, log);
            return View("UpgradeReport", new UpgradeModel(result, log));
        }
    }
}