using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VeriGenX.Domain.Entites;

namespace VeriGenX.Domain.Repository
{
    public interface IProjectRepository
    {
        Task CreateAsync(Project project);
        Task UpdateAsync(Project project);
        Task DeleteAsync(ProjectId projectId);
        Task<Project> GetByIdAsync(ProjectId projectId);
        Task<IEnumerable<Project>> GetAllAsync(UserId userId);
    }
}
