using MongoDB.Driver;
using PortfolioAPI.Models;

namespace PortfolioAPI.Services
{
    public class PortfolioService
    {
        private readonly IMongoDatabase _database;

    public PortfolioService(IMongoClient client, IConfiguration configuration)
    {
        try
        {
            var databaseName = configuration.GetValue<string>("MongoDb:DatabaseName") ?? "portfolio_db";
            Console.WriteLine($"Attempting to get MongoDB database: {databaseName}");
            _database = client.GetDatabase(databaseName);
            Console.WriteLine($"Successfully connected to MongoDB database: {databaseName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR in PortfolioService constructor: {ex.Message}");
            Console.WriteLine($"StackTrace: {ex.StackTrace}");
            throw;
        }
    }

        // Experience Methods
        public async Task<List<Experience>> GetExperiencesAsync()
        {
            var collection = _database.GetCollection<Experience>("experiences");
            return await collection.Find(_ => true).ToListAsync();
        }

        public async Task<Experience?> GetExperienceByIdAsync(string id)
        {
            var collection = _database.GetCollection<Experience>("experiences");
            return await collection.Find(e => e.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateExperienceAsync(Experience experience)
        {
            var collection = _database.GetCollection<Experience>("experiences");
            await collection.InsertOneAsync(experience);
        }

        public async Task UpdateExperienceAsync(string id, Experience experience)
        {
            var collection = _database.GetCollection<Experience>("experiences");
            experience.UpdatedAt = DateTime.UtcNow;
            await collection.ReplaceOneAsync(e => e.Id == id, experience);
        }

        public async Task DeleteExperienceAsync(string id)
        {
            var collection = _database.GetCollection<Experience>("experiences");
            await collection.DeleteOneAsync(e => e.Id == id);
        }

        // Skill Methods
        public async Task<List<Skill>> GetSkillsAsync()
        {
            var collection = _database.GetCollection<Skill>("skills");
            return await collection.Find(_ => true).ToListAsync();
        }

        public async Task<Skill?> GetSkillByIdAsync(string id)
        {
            var collection = _database.GetCollection<Skill>("skills");
            return await collection.Find(s => s.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateSkillAsync(Skill skill)
        {
            var collection = _database.GetCollection<Skill>("skills");
            await collection.InsertOneAsync(skill);
        }

        public async Task UpdateSkillAsync(string id, Skill skill)
        {
            var collection = _database.GetCollection<Skill>("skills");
            skill.UpdatedAt = DateTime.UtcNow;
            await collection.ReplaceOneAsync(s => s.Id == id, skill);
        }

        public async Task DeleteSkillAsync(string id)
        {
            var collection = _database.GetCollection<Skill>("skills");
            await collection.DeleteOneAsync(s => s.Id == id);
        }

        // Project Methods
        public async Task<List<Project>> GetProjectsAsync()
        {
            var collection = _database.GetCollection<Project>("projects");
            return await collection.Find(_ => true).ToListAsync();
        }

        public async Task<Project?> GetProjectByIdAsync(string id)
        {
            var collection = _database.GetCollection<Project>("projects");
            return await collection.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateProjectAsync(Project project)
        {
            var collection = _database.GetCollection<Project>("projects");
            await collection.InsertOneAsync(project);
        }

        public async Task UpdateProjectAsync(string id, Project project)
        {
            var collection = _database.GetCollection<Project>("projects");
            project.UpdatedAt = DateTime.UtcNow;
            await collection.ReplaceOneAsync(p => p.Id == id, project);
        }

        public async Task DeleteProjectAsync(string id)
        {
            var collection = _database.GetCollection<Project>("projects");
            await collection.DeleteOneAsync(p => p.Id == id);
        }

        // Contact Methods
        public async Task<List<Contact>> GetContactsAsync()
        {
            var collection = _database.GetCollection<Contact>("contacts");
            return await collection.Find(_ => true).SortByDescending(c => c.CreatedAt).ToListAsync();
        }

        public async Task<Contact?> GetContactByIdAsync(string id)
        {
            var collection = _database.GetCollection<Contact>("contacts");
            return await collection.Find(c => c.Id == id).FirstOrDefaultAsync();
        }

        public async Task CreateContactAsync(Contact contact)
        {
            var collection = _database.GetCollection<Contact>("contacts");
            await collection.InsertOneAsync(contact);
        }

        public async Task UpdateContactAsync(string id, Contact contact)
        {
            var collection = _database.GetCollection<Contact>("contacts");
            await collection.ReplaceOneAsync(c => c.Id == id, contact);
        }

        public async Task DeleteContactAsync(string id)
        {
            var collection = _database.GetCollection<Contact>("contacts");
            await collection.DeleteOneAsync(c => c.Id == id);
        }
    }
}
