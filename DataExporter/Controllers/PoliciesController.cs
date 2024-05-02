using DataExporter.Dtos;
using DataExporter.Model;
using DataExporter.Services;
using Microsoft.AspNetCore.Mvc;

namespace DataExporter.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PoliciesController : ControllerBase
    {
        private PolicyService _policyService;

        public PoliciesController(PolicyService policyService) 
        { 
            _policyService = policyService;
        }

        [HttpPost]
        public async Task<IActionResult> PostPolicies([FromBody]CreatePolicyDto createPolicyDto)
        {         
            var response = await _policyService.CreatePolicyAsync(createPolicyDto);
            if (response == null)
            {
                return StatusCode(500);
            }

            return Ok(response);
        }


        [HttpGet]
        public async Task<IActionResult> GetPolicies()
        {
            var response = await _policyService.ReadPoliciesAsync();
           
            return Ok(response);
        }

        [HttpGet("{policyId}")]
        public async Task<ActionResult<Policy>> GetPolicy(int policyId)
        {
            var response = await _policyService.ReadPolicyAsync(policyId);
            if (response == null)
            {
                return NotFound();
            }

            return Ok(response);
        }


        [HttpPost("export")]
        public async Task<IActionResult> ExportData([FromQuery]DateTime startDate, [FromQuery] DateTime endDate)
        {
            var exportData = await _policyService.ExportDataAsync(startDate, endDate);

            return Ok(exportData);
        }
    }
}
