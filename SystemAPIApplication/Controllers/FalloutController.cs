using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using NetTopologySuite.Geometries;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using SystemAPIApplication.bo;
using SystemAPIApplication.Services;
using SystemAPIApplication.vo;

namespace SystemAPIApplication.Controllers
{
    [ApiController]
    public class FalloutController : ControllerBase
    {
        private readonly IGeometryAnalysisService _geometryAnalysisService;


        public FalloutController(IGeometryAnalysisService geometryAnalysisService)
        {
            _geometryAnalysisService = geometryAnalysisService ??
                throw new ArgumentNullException(nameof(geometryAnalysisService));

        }
        [HttpGet("fallout/merge")]
        public IActionResult FalloutMerge()
        {
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "If the equivalent is less than 1000 tons, it will be skipped",
                return_data = _geometryAnalysisService.FalloutMerge()
            });
        }
        [HttpPost("fallout/merge")]
        public IActionResult FalloutMerge([FromBody] string[] bo)
        {
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "If the equivalent is less than 1000 tons, it will be skipped",
                return_data = _geometryAnalysisService.FalloutMerge(bo)
            });
        }
        [HttpGet("fallout/multi")]
        public IActionResult FalloutMulti()
        {
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "If the equivalent is less than 1000 tons, it will be skipped",
                return_data = _geometryAnalysisService.FalloutMulti()
            });
        }
        [HttpPost("fallout/multi")]
        public IActionResult FalloutMulti([FromBody] string[] bo)
        {
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "If the equivalent is less than 1000 tons, it will be skipped",
                return_data = _geometryAnalysisService.FalloutMulti(bo)
            });
        }
    }
}
