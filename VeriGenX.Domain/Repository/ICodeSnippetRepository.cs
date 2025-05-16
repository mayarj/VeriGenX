using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeriGenX.Domain.Entites;

namespace VeriGenX.Domain.Repository
{
    public interface ICodeSnippetRepository
    {
        Task CreateAsync(CodeSnippet snippet);
        Task<CodeSnippet?> GetByIdAsync(SnippetId snippetId);
        Task<IEnumerable<CodeSnippet>> GetByProjectIdAsync(ProjectId projectId);
        Task UpdateAsync(CodeSnippet snippet);
        Task DeleteAsync(SnippetId snippetId);
    }
}
