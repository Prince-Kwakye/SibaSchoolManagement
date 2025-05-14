using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SibaSchoolManagementApi.DTOs;
using SibaSchoolManagementApi.Services;

namespace SibaSchoolManagementApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimetableController(ITimetableService timetableService) : ControllerBase
    {
        private readonly ITimetableService _timetableService = timetableService ?? throw new ArgumentNullException(nameof(timetableService));

        [Authorize(Policy = "AdminOrStaff")]
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TimetableDto>>> GetTimetableSlots([FromQuery] bool? current)
        {
            var slots = await _timetableService.GetAllTimetableSlotsAsync(current);
            return Ok(slots);
        }

        [Authorize(Policy = "AdminOrStaff")]
        [HttpGet("{id}")]
        public async Task<ActionResult<TimetableDto>> GetTimetableSlot(int id)
        {
            var slot = await _timetableService.GetTimetableSlotByIdAsync(id);
            return slot is null ? NotFound() : Ok(slot);
        }

        [Authorize(Policy = "AdminOrStaff")]
        [HttpPost]
        public async Task<ActionResult<TimetableDto>> CreateTimetableSlot(CreateTimetableDto timetableDto)
        {
            var slot = await _timetableService.CreateTimetableSlotAsync(timetableDto);
            return CreatedAtAction(nameof(GetTimetableSlot), new { id = slot.Id }, slot);
        }

        [Authorize(Policy = "AdminOrStaff")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTimetableSlot(int id, UpdateTimetableDto timetableDto)
        {
            try
            {
                await _timetableService.UpdateTimetableSlotAsync(id, timetableDto);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }

        [Authorize(Policy = "AdminOrStaff")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTimetableSlot(int id)
        {
            try
            {
                await _timetableService.DeleteTimetableSlotAsync(id);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
        }
    }
}