using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PortfolioAPI.Controllers;
using PortfolioAPI.Models;
using PortfolioAPI.Services;
using Xunit;

namespace PortfolioAPI.Tests.Controllers;

public class ContactsControllerTests
{
    private readonly Mock<PortfolioService> _mockService;
    private readonly Mock<EmailService> _mockEmailService;
    private readonly Mock<ILogger<ContactsController>> _mockLogger;
    private readonly ContactsController _controller;

    public ContactsControllerTests()
    {
        _mockService = new Mock<PortfolioService>();
        _mockEmailService = new Mock<EmailService>();
        _mockLogger = new Mock<ILogger<ContactsController>>();
        _controller = new ContactsController(_mockService.Object, _mockEmailService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithListOfContacts()
    {
        // Arrange
        var contacts = new List<Contact>
        {
            new Contact { Id = "1", Name = "John Doe", Email = "john@example.com" },
            new Contact { Id = "2", Name = "Jane Smith", Email = "jane@example.com" }
        };
        _mockService.Setup(s => s.GetContactsAsync()).ReturnsAsync(contacts);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedContacts = okResult.Value.Should().BeAssignableTo<List<Contact>>().Subject;
        returnedContacts.Should().HaveCount(2);
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WithNewContact()
    {
        // Arrange
        var newContact = new Contact { Name = "John Doe", Email = "john@example.com", Message = "Hello!" };
        var createdContact = new Contact { Id = "1", Name = "John Doe", Email = "john@example.com", Message = "Hello!" };
        _mockService.Setup(s => s.CreateContactAsync(newContact)).ReturnsAsync(createdContact);
        _mockEmailService.Setup(e => e.SendContactNotificationAsync(createdContact)).Returns(Task.CompletedTask);

        // Act
        var result = await _controller.Create(newContact);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be("GetAll");
        var returnedContact = createdResult.Value.Should().BeOfType<Contact>().Subject;
        returnedContact.Id.Should().Be("1");
        _mockEmailService.Verify(e => e.SendContactNotificationAsync(createdContact), Times.Once);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenModelIsInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("Email", "Required");

        // Act
        var result = await _controller.Create(new Contact());

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenContactIsNull()
    {
        // Act
        var result = await _controller.Create(null!);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Create_HandlesEmailServiceException()
    {
        // Arrange
        var newContact = new Contact { Name = "John Doe", Email = "john@example.com", Message = "Hello!" };
        var createdContact = new Contact { Id = "1", Name = "John Doe", Email = "john@example.com", Message = "Hello!" };
        _mockService.Setup(s => s.CreateContactAsync(newContact)).ReturnsAsync(createdContact);
        _mockEmailService.Setup(e => e.SendContactNotificationAsync(createdContact)).ThrowsAsync(new Exception("Email service failed"));

        // Act
        var result = await _controller.Create(newContact);

        // Assert
        // Should still succeed even if email fails
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.Value.Should().BeOfType<Contact>();
        _mockService.Verify(s => s.CreateContactAsync(newContact), Times.Once);
    }

    [Fact]
    public async Task Create_HandlesServiceException()
    {
        // Arrange
        var newContact = new Contact { Name = "John Doe", Email = "john@example.com", Message = "Hello!" };
        _mockService.Setup(s => s.CreateContactAsync(newContact)).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.Create(newContact);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetAll_HandlesExceptionFromService()
    {
        // Arrange
        _mockService.Setup(s => s.GetContactsAsync()).ThrowsAsync(new Exception("Database connection failed"));

        // Act
        var result = await _controller.GetAll();

        // Assert
        result.Result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(500);
    }
}