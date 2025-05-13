using VeriGenX.Domain.Shared;

namespace VeriGenX.Domain.Errors
{
    public static class ExternalErrors
    {
        public static class SimulationTool
        {
            public static readonly Error IcarusNotInstalled = new(
                "External.Icarus.NotInstalled",
                "Icarus Verilog is not installed on the system");

            public static readonly Error IcarusExecutionFailed = new(
                "External.Icarus.ExecutionFailed",
                "Failed to execute Icarus Verilog simulation");

            public static readonly Error CompilationError = new(
                "External.Icarus.CompilationError",
                "Verilog code compilation failed");

            public static readonly Error Timeout = new(
                "External.Icarus.Timeout",
                "Simulation timed out");
        }

        public static class AIApi
        {
            public static readonly Error ConnectionFailed = new(
                "External.AI.ApiConnectionFailed",
                "Could not connect to AI API service");

            public static readonly Error InvalidApiKey = new(
                "External.AI.InvalidApiKey",
                "The provided API key is invalid");

            public static readonly Error RateLimitExceeded = new(
                "External.AI.RateLimitExceeded",
                "API rate limit exceeded");

            public static readonly Error ServiceUnavailable = new(
                "External.AI.ServiceUnavailable",
                "AI service is temporarily unavailable");

            public static readonly Error InvalidResponse = new(
                "External.AI.InvalidResponse",
                "Received malformed response from AI API");
        }

        public static class FileSystem
        {
            public static readonly Error FileNotFound = new(
                "External.FileSystem.NotFound",
                "The specified file was not found");

            public static readonly Error PermissionDenied = new(
                "External.FileSystem.PermissionDenied",
                "Insufficient permissions to access file");

            public static readonly Error DiskFull = new(
                "External.FileSystem.DiskFull",
                "Insufficient disk space available");
        }

        public static class ExternalService
        {
            public static readonly Error NetworkError = new(
                "External.NetworkError",
                "Network connectivity issue detected");

            public static readonly Error DependencyFailure = new(
                "External.DependencyFailure",
                "Required external service failed");
        }
    }
}
