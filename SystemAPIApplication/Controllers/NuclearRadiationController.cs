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
    public class NuclearRadiationController : ControllerBase
    {
        private readonly IGeometryAnalysisService _geometryAnalysisService;

        public NuclearRadiationController(IGeometryAnalysisService geometryAnalysisService)
        {
            _geometryAnalysisService = geometryAnalysisService ??
                throw new ArgumentNullException(nameof(geometryAnalysisService));
        }
        [HttpGet("nuclearradiation/merge")]
        public IActionResult NuclearradiationMerge()
        {
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = _geometryAnalysisService.NuclearradiationMerge()
            });
        }
        [HttpPost("nuclearradiation/merge")]
        public IActionResult NuclearradiationMerge([FromBody] string[] bo)
        {
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = _geometryAnalysisService.NuclearradiationMerge(bo)
            });;
        }
        [HttpGet("nuclearradiation/multi")]
        public IActionResult NuclearradiationMulti()
        {
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = _geometryAnalysisService.NuclearradiationMulti()
            });
        }
        [HttpPost("nuclearradiation/multi")]
        public IActionResult NuclearradiationMulti([FromBody] string[] bo)
        {
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = _geometryAnalysisService.NuclearradiationMulti(bo)
            });
        }
    }
}
