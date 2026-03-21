using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PortfolioAPI.Controllers;
using PortfolioAPI.Models;
using PortfolioAPI.Services;
using Xunit;

namespace PortfolioAPI.Tests.Controllers;

public class ExperiencesControllerTests
{
    private readonly Mock<PortfolioService> _mockService;
    private readonly Mock<ILogger<ExperiencesController>> _mockLogger;
    private readonly ExperiencesController _controller;

    public ExperiencesControllerTests()
    {
        _mockService = new Mock<PortfolioService>();
        _mockLogger = new Mock<ILogger<ExperiencesController>>();
        _controller = new ExperiencesController(_mockService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithListOfExperiences()
    {
        // Arrange
        var experiences = new List<Experience>
        {
            new Experience { Id = "1", Title = "Developer", Company = "Tech Corp" },
            new Experience { Id = "2", Title = "Senior Developer", Company = "Big Tech" }
        };
        _mockService.Setup(s => s.GetExperiencesAsync()).ReturnsAsync(experiences);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedExperiences = okResult.Value.Should().BeAssignableTo<List<Experience>>().Subject;
        returnedExperiences.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetAll_Returns500_WhenExceptionOccurs()
    {
        // Arrange
        _mockService.Setup(s => s.GetExperiencesAsync()).ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetAll();

        // Assert
        result.Result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetById_ReturnsOkResult_WithExperience()
    {
        // Arrange
        var experience = new Experience { Id = "1", Title = "Developer", Company = "Tech Corp" };
        _mockService.Setup(s => s.GetExperienceByIdAsync("1")).ReturnsAsync(experience);

        // Act
        var result = await _controller.GetById("1");

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedExperience = okResult.Value.Should().BeOfType<Experience>().Subject;
        returnedExperience.Id.Should().Be("1");
    }

    [Fact]
    public async Task GetById_ReturnsBadRequest_WhenIdIsNullOrEmpty()
    {
        // Act
        var result = await _controller.GetById("");

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenExperienceDoesNotExist()
    {
        // Arrange
        _mockService.Setup(s => s.GetExperienceByIdAsync("999")).ReturnsAsync((Experience?)null);

        // Act
        var result = await _controller.GetById("999");

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WithNewExperience()
    {
        // Arrange
        var newExperience = new Experience { Title = "Developer", Company = "Tech Corp" };
        var createdExperience = new Experience { Id = "1", Title = "Developer", Company = "Tech Corp" };
        _mockService.Setup(s => s.CreateExperienceAsync(newExperience)).ReturnsAsync(createdExperience);

        // Act
        var result = await _controller.Create(newExperience);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be("GetById");
        createdResult.RouteValues!["id"].Should().Be("1");
        var returnedExperience = createdResult.Value.Should().BeOfType<Experience>().Subject;
        returnedExperience.Id.Should().Be("1");
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenModelIsInvalid()
    {
        // Arrange
        _controller.ModelState.AddModelError("Title", "Required");

        // Act
        var result = await _controller.Create(new Experience());

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Update_ReturnsNoContent_WhenUpdateSuccessful()
    {
        // Arrange
        var experience = new Experience { Id = "1", Title = "Senior Developer", Company = "Tech Corp" };
        var existingExperience = new Experience { Id = "1", Title = "Developer", Company = "Tech Corp" };
        _mockService.Setup(s => s.GetExperienceByIdAsync("1")).ReturnsAsync(existingExperience);

        // Act
        var result = await _controller.Update("1", experience);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mockService.Verify(s => s.UpdateExperienceAsync("1", experience), Times.Once);
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenIdIsNullOrEmpty()
    {
        // Act
        var result = await _controller.Update("", new Experience());

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Update_ReturnsNotFound_WhenExperienceDoesNotExist()
    {
        // Arrange
        _mockService.Setup(s => s.GetExperienceByIdAsync("999")).ReturnsAsync((Experience?)null);

        // Act
        var result = await _controller.Update("999", new Experience());

        // Assert
        result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenDeleteSuccessful()
    {
        // Arrange
        var existingExperience = new Experience { Id = "1", Title = "Developer", Company = "Tech Corp" };
        _mockService.Setup(s => s.GetExperienceByIdAsync("1")).ReturnsAsync(existingExperience);

        // Act
        var result = await _controller.Delete("1");

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mockService.Verify(s => s.DeleteExperienceAsync("1"), Times.Once);
    }

    [Fact]
    public async Task Create_ReturnsBadRequest_WhenExperienceIsNull()
    {
        // Act
        var result = await _controller.Create(null!);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Update_ReturnsBadRequest_WhenExperienceIsNull()
    {
        // Act
        var result = await _controller.Update("1", null!);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Create_HandlesExceptionFromService()
    {
        // Arrange
        var experience = new Experience { Title = "Developer", Company = "Tech Corp" };
        _mockService.Setup(s => s.CreateExperienceAsync(experience)).ThrowsAsync(new Exception("Database connection failed"));

        // Act
        var result = await _controller.Create(experience);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task Update_HandlesExceptionFromService()
    {
        // Arrange
        var experience = new Experience { Id = "1", Title = "Senior Developer", Company = "Tech Corp" };
        var existingExperience = new Experience { Id = "1", Title = "Developer", Company = "Tech Corp" };
        _mockService.Setup(s => s.GetExperienceByIdAsync("1")).ReturnsAsync(existingExperience);
        _mockService.Setup(s => s.UpdateExperienceAsync("1", experience)).ThrowsAsync(new Exception("Update failed"));

        // Act
        var result = await _controller.Update("1", experience);

        // Assert
        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task Delete_HandlesExceptionFromService()
    {
        // Arrange
        var existingExperience = new Experience { Id = "1", Title = "Developer", Company = "Tech Corp" };
        _mockService.Setup(s => s.GetExperienceByIdAsync("1")).ReturnsAsync(existingExperience);
        _mockService.Setup(s => s.DeleteExperienceAsync("1")).ThrowsAsync(new Exception("Delete failed"));

        // Act
        var result = await _controller.Delete("1");

        // Assert
        result.Should().BeOfType<ObjectResult>()
            .Which.StatusCode.Should().Be(500);
    }
}