using Microsoft.AspNetCore.Mvc;
using PortfolioAPI.Models;
using PortfolioAPI.Services;

namespace PortfolioAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly PortfolioService _service;
        private readonly IEmailService _emailService;
        private readonly ILogger<ContactsController> _logger;

        public ContactsController(
            PortfolioService service,
            IEmailService emailService,
            ILogger<ContactsController> logger)
        {
            _service = service;
            _emailService = emailService;
            _logger = logger;
        }

        [HttpGet]
        public async Task<ActionResult<List<Contact>>> GetAll()
        {
            try
            {
                var contacts = await _service.GetContactsAsync();
                _logger.LogInformation("Retrieved {Count} contacts", contacts.Count);
                return Ok(contacts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contacts");
                return StatusCode(500, "An error occurred while retrieving contacts.");
            }
        }

        [HttpPost]
        public async Task<ActionResult<Contact>> SubmitContactForm(Contact contact)
        {
            try
            {
                // Validate model state (includes email validation via [EmailAddress] attribute)
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for contact form submission");
                    return BadRequest(ModelState);
                }

                // Save contact to database
                await _service.CreateContactAsync(contact);

                // Send emails
                await _emailService.SendContactFormEmailAsync(
                    contact.Name,
                    contact.Email,
                    contact.Subject,
                    contact.Message,
                    contact.Phone);

                _logger.LogInformation("Contact form submitted by {Email}", contact.Email);

                return CreatedAtAction(nameof(GetAll), new { id = contact.Id }, contact);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing contact form from {Email}", contact?.Email);
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpPut("{id}/mark-read")]
        public async Task<IActionResult> MarkAsRead(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("Contact ID is required.");

                var contact = await _service.GetContactByIdAsync(id);
                if (contact == null)
                {
                    _logger.LogWarning("Contact with ID {Id} not found for mark-read", id);
                    return NotFound();
                }

                contact.IsRead = true;
                await _service.UpdateContactAsync(id, contact);
                _logger.LogInformation("Marked contact with ID {Id} as read", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error marking contact with ID {Id} as read", id);
                return StatusCode(500, "An error occurred while updating the contact.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return BadRequest("Contact ID is required.");

                var contact = await _service.GetContactByIdAsync(id);
                if (contact == null)
                {
                    _logger.LogWarning("Contact with ID {Id} not found for deletion", id);
                    return NotFound();
                }

                await _service.DeleteContactAsync(id);
                _logger.LogInformation("Deleted contact with ID {Id}", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting contact with ID {Id}", id);
                return StatusCode(500, "An error occurred while deleting the contact.");
            }
        }
    }
}
