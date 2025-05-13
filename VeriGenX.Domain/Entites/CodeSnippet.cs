using VeriGenX.Domain.Errors;
using VeriGenX.Domain.Shared;
using VeriGenX.Domain.ValueObjects;


namespace VeriGenX.Domain.Entites
{
    public record SnippetId(Guid userId);
    public class CodeSnippet
    {
        public SnippetId SnippetId { get;private  set; }
        public string Title { get;private  set; }
        public string Description { get;private  set; }
        public string VerilogCode { get;private  set; }
        public string TestBench { get;private  set; }
        public DateTime CreatedDate { get;private  set; } 
        public DateTime LastModified { get; private set; } 
        public TestResult? TestResult { get; private set; }
        public WaveformData? Waveform { get; private  set; }

        private CodeSnippet(SnippetId Id, string title, string description, string code, string testBench)
        {
            SnippetId =Id ;
            VerilogCode = code;
            TestBench = testBench;
            Description = description;
            Title = title;
            CreatedDate = DateTime.UtcNow ;
        }
        public static Result<CodeSnippet> Create(
            SnippetId id,
            string title,
            string description,
            string verilogCode,
            string testBench)
        {
            if (string.IsNullOrWhiteSpace(title))
                return Result.Failure<CodeSnippet>(DomainErrors.CodeSnippet.EmptyTitle);

            if (string.IsNullOrWhiteSpace(verilogCode))
                return Result.Failure<CodeSnippet>(DomainErrors.CodeSnippet.EmptyCode);

            if (string.IsNullOrWhiteSpace(testBench))
                return Result.Failure<CodeSnippet>(DomainErrors.CodeSnippet.EmptyTestBench);

            return Result.Success(new CodeSnippet(
                id,
                title.Trim(),
                description?.Trim() ?? string.Empty,
                verilogCode,
                testBench));
        }

        public Result UpdateCode(string newCode)
        {
            if (string.IsNullOrWhiteSpace(newCode))
                return Result.Failure(DomainErrors.CodeSnippet.EmptyCode);

            VerilogCode = newCode;
            LastModified = DateTime.UtcNow;
            return Result.Success();
        }

        public Result UpdateTestBench(string newTestBench)
        {
            if (string.IsNullOrWhiteSpace(newTestBench))
                return Result.Failure(DomainErrors.CodeSnippet.EmptyTestBench);

            TestBench = newTestBench;
            LastModified = DateTime.UtcNow;
            return Result.Success();
        }

        public Result SetTestResult(TestResult result)
        {
            TestResult = result ?? throw new ArgumentNullException(nameof(result));
            LastModified = DateTime.UtcNow;
            return Result.Success();
        }

        public Result SetWaveform(WaveformData waveform)
        {
            Waveform = waveform ?? throw new ArgumentNullException(nameof(waveform));
            LastModified = DateTime.UtcNow;
            return Result.Success();
        }

    }
}
