using VeriGenX.Domain.Errors;
using VeriGenX.Domain.Shared;

namespace VeriGenX.Domain.ValueObjects
{
    public class TestResult : ValueObject
    {
        public bool Passed { get;private set; }
        public HashSet<int>? CodeErrorLines { get; private set; }
        public HashSet<int>? TestErrorLines { get; private set; }
        public DateTime TestTime { get;private  set; }
        public List<string>? Failures { get;private  set; }

        private TestResult(bool passed, HashSet<int>? codeErrorLines, HashSet<int>? testErrorLines, List<string>? failures)
        {
            Passed = passed;
            CodeErrorLines = codeErrorLines;
            TestErrorLines = testErrorLines;
            Failures = failures;
            TestTime = DateTime.Now;

        }
        public static Result<TestResult> Create(bool passed, HashSet<int>? codeErrorLines = null, HashSet<int>? TestErrorLines = null, List<string>? failures = null)
        {
            // Validate failure cases
            if (passed)
            {
                if ( failures != null && failures.Count > 0)
                {
                    return Result.Failure<TestResult>(DomainErrors.TestResult.InvalidSuccessState);
                }
            }
            else
            {
                if ( failures == null || failures.Count == 0)
                {
                    return Result.Failure<TestResult>(DomainErrors.TestResult.InvalidFailureState);
                }
            }

            return Result.Success(new TestResult(passed, codeErrorLines, TestErrorLines, failures));
        }


        public override IEnumerable<object> GetAtomicValues()
        {
            yield return Passed;
            yield return CodeErrorLines;
            yield return TestErrorLines ;
            yield return TestTime;

            foreach (var failure in Failures)
            {
                yield return failure;
            }
        }
    }
}
