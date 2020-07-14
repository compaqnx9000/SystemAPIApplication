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
    public class NuclearPulseController : ControllerBase
    {
        private readonly IGeometryAnalysisService _geometryAnalysisService;
        private readonly IMongoService _mongoService;

        public NuclearPulseController(IGeometryAnalysisService geometryAnalysisService)
        {
            _geometryAnalysisService = geometryAnalysisService ??
                throw new ArgumentNullException(nameof(geometryAnalysisService));
        }
        [HttpGet("nuclearpulse/merge")]
        public IActionResult NuclearpulseMerge()
        {
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = _geometryAnalysisService.NuclearpulseMerge()
            });
        }
        [HttpPost("nuclearpulse/merge")]
        public IActionResult NuclearpulseMerge([FromBody] string[] bo)
        {
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = _geometryAnalysisService.NuclearpulseMerge(bo)
            });
        }
        [HttpGet("nuclearpulse/multi")]
        public IActionResult NuclearpulseMulti()
        {
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = _geometryAnalysisService.NuclearpulseMulti()
            });
        }
        [HttpPost("nuclearpulse/multi")]
        public IActionResult NuclearpulseMulti([FromBody] string[] bo)
        {
            return new JsonResult(new
            {
                return_status = 0,
                return_msg = "",
                return_data = _geometryAnalysisService.NuclearpulseMulti(bo)
            });
        }

    }
}
