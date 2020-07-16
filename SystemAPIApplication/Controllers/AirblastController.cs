using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemAPIApplication.Services;
using SystemAPIApplication.vo;

namespace SystemAPIApplication.Controllers
{
    [ApiController]
    public class AirblastController : ControllerBase
    {
        private readonly IGeometryAnalysisService _geometryAnalysisService;

        public AirblastController(IGeometryAnalysisService geometryAnalysisService)
        {
            _geometryAnalysisService = geometryAnalysisService ??
                throw new ArgumentNullException(nameof(geometryAnalysisService));
        }
        [HttpGet("airblast/merge")]
        public IActionResult AirblastMerge()
        {
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = _geometryAnalysisService.AirblastMerge()
            });
        }
        [HttpPost("airblast/merge")]
        public IActionResult AirblastMerge([FromBody] string[] bo)
        {
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = _geometryAnalysisService.AirblastMerge(bo)
            });
        }
        [HttpGet("airblast/multi")]
        public IActionResult AirblastMulti()
        {
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = _geometryAnalysisService.AirblastMulti()
            });
        }
        [HttpPost("airblast/multi")]
        public IActionResult AirblastMulti([FromBody] string[] bo)
        {
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = _geometryAnalysisService.AirblastMulti(bo)
            });
        }
    }
}
