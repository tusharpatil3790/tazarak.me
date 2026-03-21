using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using MongoDB.Driver;
using PortfolioAPI.Models;
using PortfolioAPI.Services;
using Xunit;

namespace PortfolioAPI.Tests.Services;

public class PortfolioServiceTests
{
    private readonly Mock<IMongoClient> _mockClient;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly PortfolioService _service;

    public PortfolioServiceTests()
    {
        _mockClient = new Mock<IMongoClient>();
        _mockConfiguration = new Mock<IConfiguration>();
        var mockDatabase = new Mock<IMongoDatabase>();

        _mockConfiguration.Setup(c => c.GetValue<string>("MongoDb:DatabaseName")).Returns("test_db");
        _mockClient.Setup(c => c.GetDatabase("test_db", null)).Returns(mockDatabase.Object);

        _service = new PortfolioService(_mockClient.Object, _mockConfiguration.Object);
    }

    [Fact]
    public async Task GetExperiencesAsync_ReturnsAllExperiences()
    {
        // Arrange
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockCollection = new Mock<IMongoCollection<Experience>>();
        var experiences = new List<Experience>
        {
            new Experience { Id = "1", Title = "Developer" },
            new Experience { Id = "2", Title = "Senior Developer" }
        };
        var mockCursor = new Mock<IAsyncCursor<Experience>>();
        mockCursor.Setup(c => c.Current).Returns(experiences);
        mockCursor.SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);
        mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true).ReturnsAsync(false);

        mockCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Experience>>(), It.IsAny<FindOptions<Experience>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockCursor.Object);
        mockDatabase.Setup(d => d.GetCollection<Experience>("experiences", null)).Returns(mockCollection.Object);

        // Use reflection to set the private _database field
        var databaseField = typeof(PortfolioService).GetField("_database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        databaseField?.SetValue(_service, mockDatabase.Object);

        // Act
        var result = await _service.GetExperiencesAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().Title.Should().Be("Developer");
    }

    [Fact]
    public async Task GetExperienceByIdAsync_ReturnsExperience_WhenFound()
    {
        // Arrange
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockCollection = new Mock<IMongoCollection<Experience>>();
        var experience = new Experience { Id = "1", Title = "Developer" };
        var experiences = new List<Experience> { experience };
        var mockCursor = new Mock<IAsyncCursor<Experience>>();
        mockCursor.Setup(c => c.Current).Returns(experiences);
        mockCursor.SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);
        mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true).ReturnsAsync(false);

        mockCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Experience>>(), It.IsAny<FindOptions<Experience>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockCursor.Object);
        mockDatabase.Setup(d => d.GetCollection<Experience>("experiences", null)).Returns(mockCollection.Object);

        var databaseField = typeof(PortfolioService).GetField("_database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        databaseField?.SetValue(_service, mockDatabase.Object);

        // Act
        var result = await _service.GetExperienceByIdAsync("1");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("1");
    }

    [Fact]
    public async Task CreateExperienceAsync_AddsExperienceToDatabase()
    {
        // Arrange
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockCollection = new Mock<IMongoCollection<Experience>>();
        var experience = new Experience { Title = "Developer", Company = "Tech Corp" };

        mockDatabase.Setup(d => d.GetCollection<Experience>("experiences", null)).Returns(mockCollection.Object);

        var databaseField = typeof(PortfolioService).GetField("_database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        databaseField?.SetValue(_service, mockDatabase.Object);

        // Act
        var result = await _service.CreateExperienceAsync(experience);

        // Assert
        mockCollection.Verify(c => c.InsertOneAsync(experience, null, It.IsAny<CancellationToken>()), Times.Once);
        result.Should().Be(experience);
    }

    [Fact]
    public async Task UpdateExperienceAsync_UpdatesExperienceInDatabase()
    {
        // Arrange
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockCollection = new Mock<IMongoCollection<Experience>>();
        var experience = new Experience { Id = "1", Title = "Senior Developer", Company = "Tech Corp" };

        mockDatabase.Setup(d => d.GetCollection<Experience>("experiences", null)).Returns(mockCollection.Object);

        var databaseField = typeof(PortfolioService).GetField("_database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        databaseField?.SetValue(_service, mockDatabase.Object);

        // Act
        await _service.UpdateExperienceAsync("1", experience);

        // Assert
        mockCollection.Verify(c => c.ReplaceOneAsync(It.IsAny<FilterDefinition<Experience>>(), experience, It.IsAny<ReplaceOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteExperienceAsync_DeletesExperienceFromDatabase()
    {
        // Arrange
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockCollection = new Mock<IMongoCollection<Experience>>();

        mockDatabase.Setup(d => d.GetCollection<Experience>("experiences", null)).Returns(mockCollection.Object);

        var databaseField = typeof(PortfolioService).GetField("_database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        databaseField?.SetValue(_service, mockDatabase.Object);

        // Act
        await _service.DeleteExperienceAsync("1");

        // Assert
        mockCollection.Verify(c => c.DeleteOneAsync(It.IsAny<FilterDefinition<Experience>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetSkillsAsync_ReturnsAllSkills()
    {
        // Arrange
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockCollection = new Mock<IMongoCollection<Skill>>();
        var skills = new List<Skill>
        {
            new Skill { Id = "1", Name = "C#", Proficiency = 90 },
            new Skill { Id = "2", Name = "JavaScript", Proficiency = 85 }
        };
        var mockCursor = new Mock<IAsyncCursor<Skill>>();
        mockCursor.Setup(c => c.Current).Returns(skills);
        mockCursor.SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);
        mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true).ReturnsAsync(false);

        mockCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Skill>>(), It.IsAny<FindOptions<Skill>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockCursor.Object);
        mockDatabase.Setup(d => d.GetCollection<Skill>("skills", null)).Returns(mockCollection.Object);

        var databaseField = typeof(PortfolioService).GetField("_database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        databaseField?.SetValue(_service, mockDatabase.Object);

        // Act
        var result = await _service.GetSkillsAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("C#");
    }

    [Fact]
    public async Task GetSkillByIdAsync_ReturnsSkill_WhenFound()
    {
        // Arrange
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockCollection = new Mock<IMongoCollection<Skill>>();
        var skill = new Skill { Id = "1", Name = "C#", Proficiency = 90 };
        var skills = new List<Skill> { skill };
        var mockCursor = new Mock<IAsyncCursor<Skill>>();
        mockCursor.Setup(c => c.Current).Returns(skills);
        mockCursor.SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);
        mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true).ReturnsAsync(false);

        mockCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Skill>>(), It.IsAny<FindOptions<Skill>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockCursor.Object);
        mockDatabase.Setup(d => d.GetCollection<Skill>("skills", null)).Returns(mockCollection.Object);

        var databaseField = typeof(PortfolioService).GetField("_database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        databaseField?.SetValue(_service, mockDatabase.Object);

        // Act
        var result = await _service.GetSkillByIdAsync("1");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("1");
        result.Name.Should().Be("C#");
    }

    [Fact]
    public async Task UpdateSkillAsync_UpdatesSkillInDatabase()
    {
        // Arrange
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockCollection = new Mock<IMongoCollection<Skill>>();
        var skill = new Skill { Id = "1", Name = "C#", Proficiency = 95 };

        mockDatabase.Setup(d => d.GetCollection<Skill>("skills", null)).Returns(mockCollection.Object);

        var databaseField = typeof(PortfolioService).GetField("_database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        databaseField?.SetValue(_service, mockDatabase.Object);

        // Act
        await _service.UpdateSkillAsync("1", skill);

        // Assert
        mockCollection.Verify(c => c.ReplaceOneAsync(It.IsAny<FilterDefinition<Skill>>(), skill, It.IsAny<ReplaceOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteSkillAsync_DeletesSkillFromDatabase()
    {
        // Arrange
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockCollection = new Mock<IMongoCollection<Skill>>();

        mockDatabase.Setup(d => d.GetCollection<Skill>("skills", null)).Returns(mockCollection.Object);

        var databaseField = typeof(PortfolioService).GetField("_database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        databaseField?.SetValue(_service, mockDatabase.Object);

        // Act
        await _service.DeleteSkillAsync("1");

        // Assert
        mockCollection.Verify(c => c.DeleteOneAsync(It.IsAny<FilterDefinition<Skill>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetProjectByIdAsync_ReturnsProject_WhenFound()
    {
        // Arrange
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockCollection = new Mock<IMongoCollection<Project>>();
        var project = new Project { Id = "1", Title = "Portfolio Website", Description = "Personal portfolio" };
        var projects = new List<Project> { project };
        var mockCursor = new Mock<IAsyncCursor<Project>>();
        mockCursor.Setup(c => c.Current).Returns(projects);
        mockCursor.SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);
        mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true).ReturnsAsync(false);

        mockCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Project>>(), It.IsAny<FindOptions<Project>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockCursor.Object);
        mockDatabase.Setup(d => d.GetCollection<Project>("projects", null)).Returns(mockCollection.Object);

        var databaseField = typeof(PortfolioService).GetField("_database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        databaseField?.SetValue(_service, mockDatabase.Object);

        // Act
        var result = await _service.GetProjectByIdAsync("1");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("1");
        result.Title.Should().Be("Portfolio Website");
    }

    [Fact]
    public async Task CreateProjectAsync_AddsProjectToDatabase()
    {
        // Arrange
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockCollection = new Mock<IMongoCollection<Project>>();
        var project = new Project { Title = "E-commerce App", Description = "Online shopping platform" };

        mockDatabase.Setup(d => d.GetCollection<Project>("projects", null)).Returns(mockCollection.Object);

        var databaseField = typeof(PortfolioService).GetField("_database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        databaseField?.SetValue(_service, mockDatabase.Object);

        // Act
        var result = await _service.CreateProjectAsync(project);

        // Assert
        mockCollection.Verify(c => c.InsertOneAsync(project, null, It.IsAny<CancellationToken>()), Times.Once);
        result.Should().Be(project);
    }

    [Fact]
    public async Task UpdateProjectAsync_UpdatesProjectInDatabase()
    {
        // Arrange
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockCollection = new Mock<IMongoCollection<Project>>();
        var project = new Project { Id = "1", Title = "Updated Project", Description = "Updated description" };

        mockDatabase.Setup(d => d.GetCollection<Project>("projects", null)).Returns(mockCollection.Object);

        var databaseField = typeof(PortfolioService).GetField("_database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        databaseField?.SetValue(_service, mockDatabase.Object);

        // Act
        await _service.UpdateProjectAsync("1", project);

        // Assert
        mockCollection.Verify(c => c.ReplaceOneAsync(It.IsAny<FilterDefinition<Project>>(), project, It.IsAny<ReplaceOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteProjectAsync_DeletesProjectFromDatabase()
    {
        // Arrange
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockCollection = new Mock<IMongoCollection<Project>>();

        mockDatabase.Setup(d => d.GetCollection<Project>("projects", null)).Returns(mockCollection.Object);

        var databaseField = typeof(PortfolioService).GetField("_database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        databaseField?.SetValue(_service, mockDatabase.Object);

        // Act
        await _service.DeleteProjectAsync("1");

        // Assert
        mockCollection.Verify(c => c.DeleteOneAsync(It.IsAny<FilterDefinition<Project>>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetContactsAsync_ReturnsAllContacts_SortedByCreatedDate()
    {
        // Arrange
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockCollection = new Mock<IMongoCollection<Contact>>();
        var contacts = new List<Contact>
        {
            new Contact { Id = "1", Name = "John Doe", CreatedAt = DateTime.UtcNow.AddDays(-1) },
            new Contact { Id = "2", Name = "Jane Smith", CreatedAt = DateTime.UtcNow }
        };
        var mockCursor = new Mock<IAsyncCursor<Contact>>();
        mockCursor.Setup(c => c.Current).Returns(contacts);
        mockCursor.SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);
        mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true).ReturnsAsync(false);

        mockCollection.Setup(c => c.Find(It.IsAny<FilterDefinition<Contact>>(), It.IsAny<FindOptions>()))
            .Returns(mockCursor.Object);
        mockDatabase.Setup(d => d.GetCollection<Contact>("contacts", null)).Returns(mockCollection.Object);

        var databaseField = typeof(PortfolioService).GetField("_database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        databaseField?.SetValue(_service, mockDatabase.Object);

        // Act
        var result = await _service.GetContactsAsync();

        // Assert
        result.Should().HaveCount(2);
        result.First().Name.Should().Be("John Doe");
    }

    [Fact]
    public async Task GetContactByIdAsync_ReturnsContact_WhenFound()
    {
        // Arrange
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockCollection = new Mock<IMongoCollection<Contact>>();
        var contact = new Contact { Id = "1", Name = "John Doe", Email = "john@example.com" };
        var contacts = new List<Contact> { contact };
        var mockCursor = new Mock<IAsyncCursor<Contact>>();
        mockCursor.Setup(c => c.Current).Returns(contacts);
        mockCursor.SetupSequence(c => c.MoveNext(It.IsAny<CancellationToken>())).Returns(true).Returns(false);
        mockCursor.SetupSequence(c => c.MoveNextAsync(It.IsAny<CancellationToken>())).ReturnsAsync(true).ReturnsAsync(false);

        mockCollection.Setup(c => c.FindAsync(It.IsAny<FilterDefinition<Contact>>(), It.IsAny<FindOptions<Contact>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(mockCursor.Object);
        mockDatabase.Setup(d => d.GetCollection<Contact>("contacts", null)).Returns(mockCollection.Object);

        var databaseField = typeof(PortfolioService).GetField("_database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        databaseField?.SetValue(_service, mockDatabase.Object);

        // Act
        var result = await _service.GetContactByIdAsync("1");

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be("1");
        result.Name.Should().Be("John Doe");
    }

    [Fact]
    public async Task CreateContactAsync_AddsContactToDatabase()
    {
        // Arrange
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockCollection = new Mock<IMongoCollection<Contact>>();
        var contact = new Contact { Name = "John Doe", Email = "john@example.com", Message = "Hello!" };

        mockDatabase.Setup(d => d.GetCollection<Contact>("contacts", null)).Returns(mockCollection.Object);

        var databaseField = typeof(PortfolioService).GetField("_database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        databaseField?.SetValue(_service, mockDatabase.Object);

        // Act
        await _service.CreateContactAsync(contact);

        // Assert
        mockCollection.Verify(c => c.InsertOneAsync(contact, null, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateContactAsync_UpdatesContactInDatabase()
    {
        // Arrange
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockCollection = new Mock<IMongoCollection<Contact>>();
        var contact = new Contact { Id = "1", Name = "John Doe", Email = "john@example.com", Message = "Updated message" };

        mockDatabase.Setup(d => d.GetCollection<Contact>("contacts", null)).Returns(mockCollection.Object);

        var databaseField = typeof(PortfolioService).GetField("_database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        databaseField?.SetValue(_service, mockDatabase.Object);

        // Act
        await _service.UpdateContactAsync("1", contact);

        // Assert
        mockCollection.Verify(c => c.ReplaceOneAsync(It.IsAny<FilterDefinition<Contact>>(), contact, It.IsAny<ReplaceOptions>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteContactAsync_DeletesContactFromDatabase()
    {
        // Arrange
        var mockDatabase = new Mock<IMongoDatabase>();
        var mockCollection = new Mock<IMongoCollection<Contact>>();

        mockDatabase.Setup(d => d.GetCollection<Contact>("contacts", null)).Returns(mockCollection.Object);

        var databaseField = typeof(PortfolioService).GetField("_database", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        databaseField?.SetValue(_service, mockDatabase.Object);

        // Act
        await _service.DeleteContactAsync("1");

        // Assert
        mockCollection.Verify(c => c.DeleteOneAsync(It.IsAny<FilterDefinition<Contact>>(), It.IsAny<CancellationToken>()), Times.Once);
    }
}