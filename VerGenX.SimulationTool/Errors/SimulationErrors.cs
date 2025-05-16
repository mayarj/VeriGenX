

using VeriGenX.Domain.Shared;

namespace VeriGenX.SimulationTool.Errors
{
    public static class SimulationErrors
    {
        public static class Docker
        {
            public static readonly Error NotResponding = new(
                "Docker.NotResponding",
                "Docker container is not responding");

            public static readonly Error OperationFailed = new(
                "Docker.OperationFailed",
                "Docker operation failed");
        }

        public static class File
        {
            public static readonly Error CreationFailed = new(
                "File.CreationFailed",
                "Failed to create Verilog file");

            public static readonly Error ReadFailed = new(
                "File.ReadFailed",
                "Failed to read file content");
        }

        public static class Simulation
        {
            public static readonly Error Timeout = new(
                "Simulation.Timeout",
                "Simulation timed out");

            public static readonly Error ExecutionFailed = new(
                "Simulation.ExecutionFailed",
                "Simulation execution failed");
        }

        public static class Validation
        {
            public static readonly Error SyntaxCheckFailed = new(
                "Validation.SyntaxCheckFailed",
                "Verilog syntax validation failed");

            public static readonly Error TestAssertionsFailed = new(
                "Validation.TestAssertionsFailed",
                "Test assertions failed");
        }

        public static class Compilation
        {
            public static readonly Error Failed = new(
                "Compilation.Failed",
                "Verilog code compilation failed");
        }

        public static class Parsing
        {
            public static readonly Error VcdParsingFailed = new(
                "Parsing.VcdFailed",
                "Failed to parse VCD file");

            public static readonly Error OutputParsingFailed = new(
                "Parsing.OutputFailed",
                "Failed to parse tool output");
        }
    }

    public class ContainerError : Error
    {
        public ContainerError(string details)
            : base("Docker.ContainerError", $"Container error: {details}")
        {
        }
    }

    public class SyntaxValidationError : Error
    {
        public SyntaxValidationError(string details)
            : base("Validation.SyntaxError", $"Syntax  validation error: {details}")
        {
        }
    }

    public class CompilationError : Error
    {
        public CompilationError(string details)
            : base("Compilation.Error", $"Compilation error: {details}")
        {
        }
    }

    public class TestAssertionError : Error
    {
        public TestAssertionError(string details)
            : base("Validation.TestAssertionError", $"Test assertion error: {details}")
        {
        }
    }

    public class VcdParsingError : Error
    {
        public VcdParsingError(string details)
            : base("Parsing.VcdError", $"VCD parsing error: {details}")
        {
        }
    }
}