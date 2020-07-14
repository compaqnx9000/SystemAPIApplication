using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SystemAPIApplication.Services;
using SystemAPIApplication.Utils;
using SystemAPIApplication.vo;

namespace SystemAPIApplication.Controllers
{
    [ApiController]
    public class ThermalRadiationController : ControllerBase
    {
        private readonly IGeometryAnalysisService _geometryAnalysisService;

        public ThermalRadiationController(IGeometryAnalysisService geometryAnalysisService)
        {
            _geometryAnalysisService = geometryAnalysisService ??
                throw new ArgumentNullException(nameof(geometryAnalysisService));
        }
        [HttpGet("thermalradiation/merge")]
        public IActionResult ThermalradiationMerge()
        {
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = _geometryAnalysisService.ThermalradiationMerge()
            });
        }
        [HttpPost("thermalradiation/merge")]
        public IActionResult ThermalradiationMerge([FromBody] string[] bo)
        {
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = _geometryAnalysisService.ThermalradiationMerge(bo)
            });
        }
        [HttpGet("thermalradiation/multi")]
        public IActionResult ThermalradiationMulti()
        {
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = _geometryAnalysisService.ThermalradiationMulti()
            });
        }
        [HttpPost("thermalradiation/multi")]
        public IActionResult ThermalradiationMulti([FromBody] string[] bo)
        {
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = _geometryAnalysisService.ThermalradiationMulti(bo)
            });
        }
    }
}
