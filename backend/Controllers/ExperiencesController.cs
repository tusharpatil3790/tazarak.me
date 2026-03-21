using Microsoft.AspNetCore.Mvc;
using PortfolioAPI.Models;
using PortfolioAPI.Services;

namespace PortfolioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExperiencesController : ControllerBase
    {
        private readonly PortfolioService _service;
        private readonly ILogger<ExperiencesController> _logger;

        public ExperiencesController(PortfolioService service, ILogger<ExperiencesController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<Experience>>> GetAll()
        {
            try
            {
                var experiences = await _service.GetExperiencesAsync();
                _logger.LogInformation("Retrieved {Count} experiences", experiences.Count);
                return Ok(experiences);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving experiences");
                return StatusCode(500, "An error occurred while retrieving experiences.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Experience>> GetById(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("Experience ID is required.");

                var experience = await _service.GetExperienceByIdAsync(id);
                if (experience == null)
                {
                    _logger.LogWarning("Experience with ID {Id} not found", id);
                    return NotFound();
                }

                return Ok(experience);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving experience with ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the experience.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Experience>> Create(Experience experience)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for experience creation");
                    return BadRequest(ModelState);
                }

                await _service.CreateExperienceAsync(experience);
                _logger.LogInformation("Created experience with ID {Id}", experience.Id);
                return CreatedAtAction(nameof(GetById), new { id = experience.Id }, experience);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating experience");
                return StatusCode(500, "An error occurred while creating the experience.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Experience experience)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("Experience ID is required.");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for experience update");
                    return BadRequest(ModelState);
                }

                var existing = await _service.GetExperienceByIdAsync(id);
                if (existing == null)
                {
                    _logger.LogWarning("Experience with ID {Id} not found for update", id);
                    return NotFound();
                }

                experience.Id = id;
                await _service.UpdateExperienceAsync(id, experience);
                _logger.LogInformation("Updated experience with ID {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating experience with ID {Id}", id);
                return StatusCode(500, "An error occurred while updating the experience.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("Experience ID is required.");

                var experience = await _service.GetExperienceByIdAsync(id);
                if (experience == null)
                {
                    _logger.LogWarning("Experience with ID {Id} not found for deletion", id);
                    return NotFound();
                }

                await _service.DeleteExperienceAsync(id);
                _logger.LogInformation("Deleted experience with ID {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting experience with ID {Id}", id);
                return StatusCode(500, "An error occurred while deleting the experience.");
            }
        }
    }
}
