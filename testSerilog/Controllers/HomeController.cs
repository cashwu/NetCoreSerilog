﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Serilog;
using testSerilog.Models;

namespace testSerilog.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IDiagnosticContext _diagnosticContext;

        public HomeController(ILogger<HomeController> logger, IDiagnosticContext diagnosticContext)
        {
            _logger = logger;
            _diagnosticContext = diagnosticContext;
        }

        public IActionResult Index()
        {
            var logModel = new LogModel();
            var fruit = new[] { "Apple", "Pear", "Orange" };
            _logger.LogInformation("Hello, world - {fruit} - {@logModel}", fruit, logModel);
            
            _diagnosticContext.Set("CatalogLoadTime", 123123123123);
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult Error()
        {
            throw new NotImplementedException("cash test");
        }
    }

    class LogModel
    {
        public LogModel()
        {
            Name = "cash";
            Id = 123;
            Today = DateTime.Today;
        }
        
        public string Name { get; set; }

        public int Id { get; set; }

        public DateTime Today { get; set; }
    }
}