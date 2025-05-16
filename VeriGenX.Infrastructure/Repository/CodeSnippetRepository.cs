
using AutoMapper;
using MongoDB.Driver;
using VeriGenX.Domain.Entites;
using VeriGenX.Domain.Repository;
using VeriGenX.Infrastructure.Context;
using VeriGenX.Infrastructure.DAO;

namespace VeriGenX.Infrastructure.Repository
{
    public class CodeSnippetRepository : ICodeSnippetRepository
    {
        private readonly IMongoDbContext _context;
        private readonly IMongoCollection<CodeSnippetDocument> _snippets;
        private readonly IMapper _mapper;
        //private readonly ICurrentUserService _currentUserService;

        public CodeSnippetRepository(IMongoDbContext context, IMapper mapper)
        {
            _context = context;
            _context.CreateIndexAsync<CodeSnippetDocument>("ProjectId" , null, "CodeSnippets").Wait();
            _snippets = _context.GetCollection<CodeSnippetDocument>("CodeSnippets");
            //_currentUserService = currentUserService;
            _mapper = mapper;
     
        }

        public async Task CreateAsync(CodeSnippet snippet)
        {
            var CodeSnippetDocument = _mapper.Map<CodeSnippetDocument>(snippet);
            await _snippets.InsertOneAsync(CodeSnippetDocument);
            //return snippet;
        }

        public async Task<CodeSnippet?> GetByIdAsync(SnippetId snippetId)
        {
           var snippet =  await _snippets.Find(s => s.SnippetId == snippetId.snippetId.ToString()).FirstOrDefaultAsync();
        return _mapper.Map<CodeSnippet>(snippet);
        }

        public async Task<IEnumerable<CodeSnippet>> GetByProjectIdAsync(ProjectId projectId)
        {
            var sinppets =  await _snippets.Find(s => s.ProjectId == projectId.projectId.ToString() ).ToListAsync();
            return _mapper.Map<IEnumerable<CodeSnippet>>(sinppets);
        }

        public async Task UpdateAsync(CodeSnippet snippet)
        {
            var snip = _mapper.Map<CodeSnippetDocument>(snippet);
            var filter = Builders<CodeSnippetDocument>.Filter.Eq(s => s.SnippetId, snip.SnippetId);
            await _snippets.ReplaceOneAsync(filter, snip);
       
        }

        public async Task DeleteAsync(SnippetId snippetId)
        {
            var filter = Builders<CodeSnippetDocument>.Filter.Eq(s => s.SnippetId, snippetId.snippetId.ToString());
            await _snippets.DeleteOneAsync(filter);
           
        }
    }

}
