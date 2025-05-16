using AutoMapper;
using MongoDB.Driver;

using NUnit.Framework;
using System;
using System.Collections.Generic;

using System.Linq;

using System.Threading.Tasks;
using VeriGenX.Domain.Entites;
using VeriGenX.Domain.Repository;
using VeriGenX.Infrastructure.Context;
using VeriGenX.Infrastructure.DAO;
using VeriGenX.Infrastructure.Profiles;
using VeriGenX.Infrastructure.Repository;



namespace VeriGenX.Tests.Infrastructure
{
    [TestFixture]
    public class CodeSnippetRepositoryTests
    {
        //private MongoDbRunner _runner;
        private MongoDbContext _context;
        private CodeSnippetRepository _repository;
        private IMapper _mapper;
        private MapperConfiguration _config;

        [SetUp]
        public void Setup()
        {
            _config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = _config.CreateMapper();
            // Start MongoDB in-memory instance
           // _runner = MongoDbRunner.Start();

            // Configure AutoMapper
        
            // Create context
            var client = new MongoClient();
            _context = new MongoDbContext("mongodb://localhost:27017/", "TestDB");

            // Initialize repository
            _repository = new CodeSnippetRepository(_context, _mapper);
        }

        [TearDown]
        public void TearDown()
        {
          _context.Dispose();
        }

        [Test]
        public async Task CreateAsync_ShouldInsertSnippet()
        {
            var id = new SnippetId(Guid.NewGuid());
            // Arrange
            var snippet =CodeSnippet.Create
            (
                id,
                new ProjectId(Guid.NewGuid()),
                "sss" , 
                "test code",
                "C#",
                "ssss"
            ).Value;

            Assert.IsNotNull(snippet);
            // Act
            await _repository.CreateAsync(snippet);

            // Assert
            var result = await _repository.GetByIdAsync(id);
            Assert.IsNotNull(result);
            Assert.AreEqual(snippet.SnippetId, result.SnippetId);

        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnSnippet()
        {
            // Arrange
            var snippetId = Guid.NewGuid();
            var snippet = new CodeSnippetDocument
            {
                SnippetId = snippetId.ToString(),
                ProjectId = Guid.NewGuid().ToString(),
                VerilogCode = "test code",
                Title = "test code",
                TestBench = "test ",
                CreatedDate = DateTime.Now,
                LastModified = DateTime.Now,
            };
            await _context.GetCollection<CodeSnippetDocument>("CodeSnippets").InsertOneAsync(snippet);

            // Act
            var result = await _repository.GetByIdAsync(new SnippetId(snippetId));

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(snippet.SnippetId, result.SnippetId.snippetId.ToString());
            Assert.AreEqual(snippet.VerilogCode, result.VerilogCode);
        }

        [Test]
        public async Task GetByProjectIdAsync_ShouldReturnSnippets()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var snippets = new List<CodeSnippetDocument>
            {
                new CodeSnippetDocument { SnippetId = Guid.NewGuid().ToString(), ProjectId = projectId.ToString(),  VerilogCode = "code1" , Description = "sss" , Title = "sss" , TestBench = "sss"  },
                new CodeSnippetDocument { SnippetId = Guid.NewGuid().ToString(), ProjectId = projectId.ToString(),  VerilogCode = "code2" , Description = "sss" , Title = "sss" , TestBench = "sss" },
                new CodeSnippetDocument { SnippetId = Guid.NewGuid().ToString(), ProjectId = Guid.NewGuid().ToString() ,  VerilogCode = "code3" , Description = "sss" , Title = "sss" , TestBench = "sss" }
            };
            await _context.GetCollection<CodeSnippetDocument>("CodeSnippets").InsertManyAsync(snippets);

            // Act
            var result = await _repository.GetByProjectIdAsync(new ProjectId(projectId));

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task UpdateAsync_ShouldModifySnippet()
        {
            // Arrange
            var snippetId = Guid.NewGuid();
            var originalSnippet = new CodeSnippetDocument
            {
                SnippetId = snippetId.ToString(),
                ProjectId = Guid.NewGuid().ToString(),
                VerilogCode = "test code",
                Title = "test code",
                TestBench = "test ",
                CreatedDate = DateTime.Now,
                LastModified = DateTime.Now,

            };
            await _context.GetCollection<CodeSnippetDocument>("CodeSnippets").InsertOneAsync(originalSnippet);

            var updatedSnippet = CodeSnippet.Create
            (
              new SnippetId( snippetId),
                new ProjectId(Guid.NewGuid()),
                "sss",
                "test code",
                "updated code",
                "ssss"
            ).Value;

            // Act
            await _repository.UpdateAsync(updatedSnippet);

            // Assert
            var result = await _repository.GetByIdAsync(new SnippetId(snippetId));
            Assert.AreEqual("updated code", result.VerilogCode);
        }

        [Test]
        public async Task DeleteAsync_ShouldRemoveSnippet()
        {
            // Arrange
            var snippetId = Guid.NewGuid();
            var snippet = new CodeSnippetDocument
            {
                SnippetId = snippetId.ToString(),
                ProjectId = Guid.NewGuid().ToString(),
                VerilogCode = "test code",
                Title = "test code",
                TestBench = "test ",
                CreatedDate = DateTime.Now,
                LastModified = DateTime.Now,
            };
            await _context.GetCollection<CodeSnippetDocument>("CodeSnippets").InsertOneAsync(snippet);

            // Act
            await _repository.DeleteAsync(new SnippetId(snippetId));

            // Assert
            var result = await _repository.GetByIdAsync(new SnippetId(snippetId));
            Assert.IsNull(result);
        }
    }

    [TestFixture]
    public class ProjectRepositoryTests
    {
       
        private MongoDbContext _context;
        private ProjectRepository _repository;
        private IMapper _mapper;
        private MapperConfiguration _config;
        [SetUp]
        public void Setup()
        {
            _config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = _config.CreateMapper();
            // Start MongoDB in-memory instance
            // _runner = MongoDbRunner.Start();

            // Configure AutoMapper

            // Create context
            var client = new MongoClient();
            _context = new MongoDbContext("mongodb://localhost:27017/", "TestDB");

            // Initialize repository
            _repository = new ProjectRepository(_context, _mapper);
        }

        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }

        [Test]
        public async Task CreateAsync_ShouldInsertProject()
        {
            // Arrange
            var project = Project.Create(
            
               new ProjectId( Guid.NewGuid()),
               new UserId( Guid.NewGuid()),
              "Test Project",
              "Test Description").Value;

            // Act
            await _repository.CreateAsync(project);

            // Assert
            var result = await _repository.GetByIdAsync((project.ProjectId));
            Assert.IsNotNull(result);
            Assert.AreEqual(project.Name, result.Name);
        }

        [Test]
        public async Task GetByIdAsync_ShouldReturnProject()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var project = new ProjectDocument
            {
                ProjectId = projectId.ToString(),
                Name = "Test Project",
                Description = "Test Description",
                UserId = Guid.NewGuid().ToString()
            };
            await _context.GetCollection<ProjectDocument>("Projects").InsertOneAsync(project);

            // Act
            var result = await _repository.GetByIdAsync(new ProjectId(projectId));

            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(project.ProjectId, result.ProjectId.projectId.ToString());
        }

        [Test]
        public async Task GetAllAsync_ShouldReturnUserProjects()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var projects = new List<ProjectDocument>
            {
                new ProjectDocument { ProjectId = Guid.NewGuid().ToString(), Name = "Project 1", UserId = userId.ToString() },
                new ProjectDocument { ProjectId = Guid.NewGuid().ToString(), Name = "Project 2", UserId = userId.ToString() },
                new ProjectDocument { ProjectId = Guid.NewGuid().ToString(), Name = "Other User Project", UserId = Guid.NewGuid().ToString() }
            };
            await _context.GetCollection<ProjectDocument>("Projects").InsertManyAsync(projects);

            // Act
            var result = await _repository.GetAllAsync(new UserId(userId));

            // Assert
            Assert.AreEqual(2, result.Count());
        }

        [Test]
        public async Task UpdateAsync_ShouldModifyProject()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var originalProject = new ProjectDocument
            {
                ProjectId = projectId.ToString(),
                Name = "Original Name",
                Description = "Original Description",
                UserId = Guid.NewGuid().ToString()
            };
            await _context.GetCollection<ProjectDocument>("Projects").InsertOneAsync(originalProject);

            var updatedProject =Project.Create
            (
               new ProjectId (projectId),
              new UserId (Guid.Parse(originalProject.UserId)), 
               "Updated Name",
               "Updated Description"

            ).Value;

            // Act
            await _repository.UpdateAsync(updatedProject);

            // Assert
            var result = await _repository.GetByIdAsync(new ProjectId(projectId));
            Assert.AreEqual("Updated Name", result.Name);
            Assert.AreEqual("Updated Description", result.Description);
        }

        [Test]
        public async Task DeleteAsync_ShouldRemoveProject()
        {
            // Arrange
            var projectId = Guid.NewGuid();
            var project = new ProjectDocument
            {
                ProjectId = projectId.ToString(),
                Name = "Test Project",
                UserId = Guid.NewGuid().ToString(),
            };
            await _context.GetCollection<ProjectDocument>("Projects").InsertOneAsync(project);

            // Act
            await _repository.DeleteAsync(new ProjectId(projectId));

            // Assert
            var result = await _repository.GetByIdAsync(new ProjectId(projectId));
            Assert.IsNull(result);
        }
    }

    // Simple implementation of MongoDbContext for testing
  
    
}
