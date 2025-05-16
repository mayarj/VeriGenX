using AutoMapper;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VeriGenX.Domain.Entites;
using VeriGenX.Domain.Repository;
using VeriGenX.Infrastructure.Context;
using VeriGenX.Infrastructure.DAO;

namespace VeriGenX.Infrastructure.Repository
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly IMongoDbContext _context;
        private readonly IMongoCollection<ProjectDocument> _collection;
        private readonly IMapper _mapper;

        public ProjectRepository(IMongoDbContext context, IMapper mapper)
        {
            _context = context;
            _context.CreateIndexAsync<ProjectDocument>("UserId" ,null ,  "Projects").Wait();
            _collection = _context.GetCollection<ProjectDocument>("Projects");
            _mapper = mapper;
            
        }

        // Create a new Project
        public async Task CreateAsync(Project project)
        {
            var projectDoc = _mapper.Map<ProjectDocument>(project);  
            await _collection.InsertOneAsync(projectDoc);
        }

        // Update an existing Project
        public async Task UpdateAsync(Project project)
        {
            var projectDoc = _mapper.Map<ProjectDocument>(project);
            var filter = Builders<ProjectDocument>.Filter.Eq(p => p.ProjectId, projectDoc.ProjectId.ToString());
            await _collection.ReplaceOneAsync(filter, projectDoc);
        }

        // Delete a Project by ID
        public async Task DeleteAsync(ProjectId projectId)
        {
            var filter = Builders<ProjectDocument>.Filter.Eq(p => p.ProjectId, projectId.projectId.ToString());
            await _collection.DeleteOneAsync(filter);
        }

        // Get a Project by ID   
        public async Task<Project> GetByIdAsync(ProjectId projectId)
        {
            var filter = Builders<ProjectDocument>.Filter.Eq(p => p.ProjectId, projectId.projectId.ToString());
            var project =  await _collection.Find(filter).FirstOrDefaultAsync();
            return _mapper.Map<Project>(project);
        }

        // Get all Projects
        public async Task<IEnumerable<Project>> GetAllAsync(UserId userId)
        {
            var projects = await _collection.Find(s => s.UserId == userId.userId.ToString()).ToListAsync();
            return _mapper.Map<IEnumerable<Project>>(projects);
        }
    }
}
