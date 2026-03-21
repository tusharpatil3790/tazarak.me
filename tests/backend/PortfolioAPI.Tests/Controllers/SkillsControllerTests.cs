using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using PortfolioAPI.Controllers;
using PortfolioAPI.Models;
using PortfolioAPI.Services;
using Xunit;

namespace PortfolioAPI.Tests.Controllers;

public class SkillsControllerTests
{
    private readonly Mock<PortfolioService> _mockService;
    private readonly Mock<ILogger<SkillsController>> _mockLogger;
    private readonly SkillsController _controller;

    public SkillsControllerTests()
    {
        _mockService = new Mock<PortfolioService>();
        _mockLogger = new Mock<ILogger<SkillsController>>();
        _controller = new SkillsController(_mockService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetAll_ReturnsOkResult_WithListOfSkills()
    {
        // Arrange
        var skills = new List<Skill>
        {
            new Skill { Id = "1", Name = "C#", Proficiency = 90 },
            new Skill { Id = "2", Name = "JavaScript", Proficiency = 85 }
        };
        _mockService.Setup(s => s.GetSkillsAsync()).ReturnsAsync(skills);

        // Act
        var result = await _controller.GetAll();

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedSkills = okResult.Value.Should().BeAssignableTo<List<Skill>>().Subject;
        returnedSkills.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetById_ReturnsOkResult_WithSkill()
    {
        // Arrange
        var skill = new Skill { Id = "1", Name = "C#", Proficiency = 90 };
        _mockService.Setup(s => s.GetSkillByIdAsync("1")).ReturnsAsync(skill);

        // Act
        var result = await _controller.GetById("1");

        // Assert
        var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedSkill = okResult.Value.Should().BeOfType<Skill>().Subject;
        returnedSkill.Id.Should().Be("1");
    }

    [Fact]
    public async Task GetById_ReturnsNotFound_WhenSkillDoesNotExist()
    {
        // Arrange
        _mockService.Setup(s => s.GetSkillByIdAsync("999")).ReturnsAsync((Skill?)null);

        // Act
        var result = await _controller.GetById("999");

        // Assert
        result.Result.Should().BeOfType<NotFoundResult>();
    }

    [Fact]
    public async Task Create_ReturnsCreatedAtAction_WithNewSkill()
    {
        // Arrange
        var newSkill = new Skill { Name = "C#", Proficiency = 90 };
        var createdSkill = new Skill { Id = "1", Name = "C#", Proficiency = 90 };
        _mockService.Setup(s => s.CreateSkillAsync(newSkill)).ReturnsAsync(createdSkill);

        // Act
        var result = await _controller.Create(newSkill);

        // Assert
        var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be("GetById");
        createdResult.RouteValues!["id"].Should().Be("1");
    }

    [Fact]
    public async Task Update_ReturnsNoContent_WhenUpdateSuccessful()
    {
        // Arrange
        var skill = new Skill { Id = "1", Name = "C#", Proficiency = 95 };
        var existingSkill = new Skill { Id = "1", Name = "C#", Proficiency = 90 };
        _mockService.Setup(s => s.GetSkillByIdAsync("1")).ReturnsAsync(existingSkill);

        // Act
        var result = await _controller.Update("1", skill);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mockService.Verify(s => s.UpdateSkillAsync("1", skill), Times.Once);
    }

    [Fact]
    public async Task Delete_ReturnsNoContent_WhenDeleteSuccessful()
    {
        // Arrange
        var existingSkill = new Skill { Id = "1", Name = "C#", Proficiency = 90 };
        _mockService.Setup(s => s.GetSkillByIdAsync("1")).ReturnsAsync(existingSkill);

        // Act
        var result = await _controller.Delete("1");

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mockService.Verify(s => s.DeleteSkillAsync("1"), Times.Once);
    }
}