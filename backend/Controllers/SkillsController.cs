using Microsoft.AspNetCore.Mvc;
using PortfolioAPI.Models;
using PortfolioAPI.Services;

namespace PortfolioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SkillsController : ControllerBase
    {
        private readonly PortfolioService _service;
        private readonly ILogger<SkillsController> _logger;

        public SkillsController(PortfolioService service, ILogger<SkillsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<Skill>>> GetAll()
        {
            try
            {
                var skills = await _service.GetSkillsAsync();
                _logger.LogInformation("Retrieved {Count} skills", skills.Count);
                return Ok(skills);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving skills");
                return StatusCode(500, "An error occurred while retrieving skills.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Skill>> GetById(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("Skill ID is required.");

                var skill = await _service.GetSkillByIdAsync(id);
                if (skill == null)
                {
                    _logger.LogWarning("Skill with ID {Id} not found", id);
                    return NotFound();
                }

                return Ok(skill);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving skill with ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the skill.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Skill>> Create(Skill skill)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for skill creation");
                    return BadRequest(ModelState);
                }

                await _service.CreateSkillAsync(skill);
                _logger.LogInformation("Created skill with ID {Id}", skill.Id);
                return CreatedAtAction(nameof(GetById), new { id = skill.Id }, skill);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating skill");
                return StatusCode(500, "An error occurred while creating the skill.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Skill skill)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("Skill ID is required.");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for skill update");
                    return BadRequest(ModelState);
                }

                var existing = await _service.GetSkillByIdAsync(id);
                if (existing == null)
                {
                    _logger.LogWarning("Skill with ID {Id} not found for update", id);
                    return NotFound();
                }

                skill.Id = id;
                await _service.UpdateSkillAsync(id, skill);
                _logger.LogInformation("Updated skill with ID {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating skill with ID {Id}", id);
                return StatusCode(500, "An error occurred while updating the skill.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("Skill ID is required.");

                var skill = await _service.GetSkillByIdAsync(id);
                if (skill == null)
                {
                    _logger.LogWarning("Skill with ID {Id} not found for deletion", id);
                    return NotFound();
                }

                await _service.DeleteSkillAsync(id);
                _logger.LogInformation("Deleted skill with ID {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting skill with ID {Id}", id);
                return StatusCode(500, "An error occurred while deleting the skill.");
            }
        }
    }
}
