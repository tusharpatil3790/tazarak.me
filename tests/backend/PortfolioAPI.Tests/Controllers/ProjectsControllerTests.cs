using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PortfolioAPI.Controllers;
using PortfolioAPI.Models;
using PortfolioAPI.Services;
using Xunit;

namespace PortfolioAPI.Tests.Controllers;

public class ProjectsControllerTests
{
    private readonly Mock<PortfolioService> _mockService;
    private readonly Mock<ILogger<ProjectsController>> _mockLogger;
    private readonly ProjectsController _controller;

    public ProjectsControllerTests()
    {
        _mockService = new Mock<PortfolioService>();
        _mockLogger = new Mock<ILogger<ProjectsController>>();
        _controller = new ProjectsController(_mockService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithListOfProjects()
    {
        // Arrange
        var projects = new List<Project>
        {
            new Project { Id = "1", Title = "Portfolio Website", Description = "Personal portfolio" },
            new Project { Id = "2", Title = "E-commerce App", Description = "Online shopping platform" }
        };
        _mockService.Setup(s => s.GetProjectsAsync()).ReturnsAsync(projects);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProjects = okResult.Value.Should().BeAssignableTo<List<Project>>().Subject;
        returnedProjects.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetById_ReturnsOkResult_WithProject()
    {
        // Arrange
        var project = new Project { Id = "1", Title = "Portfolio Website", Description = "Personal portfolio" };
        _mockService.Setup(s => s.GetProjectByIdAsync("1")).ReturnsAsync(project);

        // Act
        var result = await _controller.GetById("1");

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedProject = okResult.Value.Should().BeOfType<Project>().Subject;
        returnedProject.Id.Should().Be("1");
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenProjectDoesNotExist()
    {
        // Arrange
        _mockService.Setup(s => s.GetProjectByIdAsync("999")).ReturnsAsync((Project?)null);

        // Act
        var result = await _controller.GetById("999");

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WithNewProject()
    {
        // Arrange
        var newProject = new Project { Title = "Portfolio Website", Description = "Personal portfolio" };
        var createdProject = new Project { Id = "1", Title = "Portfolio Website", Description = "Personal portfolio" };
        _mockService.Setup(s => s.CreateProjectAsync(newProject)).ReturnsAsync(createdProject);

        // Act
        var result = await _controller.Create(newProject);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be("GetById");
        createdResult.RouteValues!["id"].Should().Be("1");
    }

    [Fact]
    public async Task Update_ReturnsNoContent_WhenUpdateSuccessful()
    {
        // Arrange
        var project = new Project { Id = "1", Title = "Updated Portfolio", Description = "Updated description" };
        var existingProject = new Project { Id = "1", Title = "Portfolio Website", Description = "Personal portfolio" };
        _mockService.Setup(s => s.GetProjectByIdAsync("1")).ReturnsAsync(existingProject);

        // Act
        var result = await _controller.Update("1", project);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mockService.Verify(s => s.UpdateProjectAsync("1", project), Times.Once);
    }
}