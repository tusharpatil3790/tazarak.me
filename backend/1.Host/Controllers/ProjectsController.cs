using Microsoft.AspNetCore.Mvc;
using PortfolioAPI.Models;
using PortfolioAPI.Services;

namespace PortfolioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProjectsController : ControllerBase
    {
        private readonly PortfolioService _service;
        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(PortfolioService service, ILogger<ProjectsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<Project>>> GetAll()
        {
            try
            {
                var projects = await _service.GetProjectsAsync();
                _logger.LogInformation("Retrieved {Count} projects", projects.Count);
                return Ok(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving projects");
                return StatusCode(500, "An error occurred while retrieving projects.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetById(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("Project ID is required.");

                var project = await _service.GetProjectByIdAsync(id);
                if (project == null)
                {
                    _logger.LogWarning("Project with ID {Id} not found", id);
                    return NotFound();
                }

                return Ok(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving project with ID {Id}", id);
                return StatusCode(500, "An error occurred while retrieving the project.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Project>> Create(Project project)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for project creation");
                    return BadRequest(ModelState);
                }

                await _service.CreateProjectAsync(project);
                _logger.LogInformation("Created project with ID {Id}", project.Id);
                return CreatedAtAction(nameof(GetById), new { id = project.Id }, project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating project");
                return StatusCode(500, "An error occurred while creating the project.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Project project)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("Project ID is required.");

                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for project update");
                    return BadRequest(ModelState);
                }

                var existing = await _service.GetProjectByIdAsync(id);
                if (existing == null)
                {
                    _logger.LogWarning("Project with ID {Id} not found for update", id);
                    return NotFound();
                }

                project.Id = id;
                await _service.UpdateProjectAsync(id, project);
                _logger.LogInformation("Updated project with ID {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating project with ID {Id}", id);
                return StatusCode(500, "An error occurred while updating the project.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("Project ID is required.");

                var project = await _service.GetProjectByIdAsync(id);
                if (project == null)
                {
                    _logger.LogWarning("Project with ID {Id} not found for deletion", id);
                    return NotFound();
                }

                await _service.DeleteProjectAsync(id);
                _logger.LogInformation("Deleted project with ID {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting project with ID {Id}", id);
                return StatusCode(500, "An error occurred while deleting the project.");
            }
        }
    }
}
