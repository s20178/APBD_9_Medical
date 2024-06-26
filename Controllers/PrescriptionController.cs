using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MedicalApp.DTO;
using MedicalApp.Services;

namespace MedicalApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionsController : ControllerBase
    {
        private readonly IPrescriptionService _prescriptionService;

        public PrescriptionsController(IPrescriptionService prescriptionService)
        {
            _prescriptionService = prescriptionService;
        }

        [HttpPost]
        public async Task<IActionResult> AddPrescription([FromBody] PrescriptionDto prescriptionDto)
        {
            try
            {
                var prescription = await _prescriptionService.AddPrescriptionAsync(prescriptionDto);
                return Ok(prescription);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("patient/{patientId}")]
        public async Task<IActionResult> GetPatientData(int patientId)
        {
            try
            {
                var patient = await _prescriptionService.GetPatientDataAsync(patientId);
                return Ok(patient);
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}
