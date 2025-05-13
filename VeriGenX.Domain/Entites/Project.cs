using VeriGenX.Domain.Errors;
using VeriGenX.Domain.Shared;

namespace VeriGenX.Domain.Entites
{
    public record ProjectId(Guid id);
    public class Project
    {
        public ProjectId ProjectId { get;private  set; }
        public string Name { get;private  set; }
        public string Description { get;private  set; }
        public DateTime CreatedDate { get;private  set; } 
        public DateTime LastModified { get;private  set; }
        private  List<CodeSnippet> _snippets = new();

        private Project (ProjectId id, string name , string description)
        {
            Name = name;
            Description = description;
            CreatedDate = DateTime.Now;
            LastModified = DateTime.Now;
            ProjectId = id ;
        }

        public static Result<Project> Create(ProjectId id, string name, string description = "")
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure<Project>(DomainErrors.Project.EmptyName);

            if (name.Length > 100)
                return Result.Failure<Project>(DomainErrors.Project.NameTooLong);

            return Result.Success(new Project(
                id,
                name.Trim(),
                description?.Trim() ?? string.Empty));
        }

        public Result UpdateDescription(string newDescription)
        {
            if (newDescription.Length > 500)
                return Result.Failure(DomainErrors.Project.DescriptionTooLong);
            Description = newDescription.Trim();
            LastModified = DateTime.UtcNow;
            return Result.Success();
        }

        public Result AddSnippet(CodeSnippet? snippet)
        {
            if (snippet == null)
                return Result.Failure(DomainErrors.Project.NullSnippet);

            if (_snippets.Exists(s => s.SnippetId == snippet.SnippetId))
                return Result.Failure(DomainErrors.Project.DuplicateSnippet);

            _snippets.Add(snippet);
            LastModified = DateTime.UtcNow;
            return Result.Success();
        }

        public Result RemoveSnippet(SnippetId snippetId)
        {
            var snippet = _snippets.Find(s => s.SnippetId == snippetId);
            if (snippet == null)
                return Result.Failure(DomainErrors.Project.SnippetNotFound);

            _snippets.Remove(snippet);
            LastModified = DateTime.UtcNow;
            return Result.Success();
        }

    }
}
