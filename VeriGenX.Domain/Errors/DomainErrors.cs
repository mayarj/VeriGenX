
using VeriGenX.Domain.Shared;

namespace VeriGenX.Domain.Errors
{
    public static class DomainErrors
    {
        public static class Email
        {
            public static readonly Error Empty = new(
                "Email.Empty",
                "Email is empty.");

            public static readonly Error InvalidFormat = new(
                "Email.InvalidFormat",
                "Email format is invalid.");
        }

        public static class FirstName
        {
            public static readonly Error Empty = new(
                "FirstName.Empty",
                "First name is empty.");

            public static readonly Error TooLong = new(
                "LastName.TooLong",
                "FirstName name is too long.");
        }

        public static class LastName
        {
            public static readonly Error Empty = new(
                "LastName.Empty",
                "Last name is empty.");

            public static readonly Error TooLong = new(
                "LastName.TooLong",
                "Last name is too long.");
        }

        public static class TestResult
        {
            public static readonly Error InvalidSuccessState = new(
                "TestResult.InvalidSuccessState",
                "A passing test cannot have error messages or failures");

            public static readonly Error InvalidFailureState = new(
                "TestResult.InvalidFailureState",
                "A failing test must have either an error message or failures");
        }

        public static class Signal
        {
            public static readonly Error EmptyName = new(
                "Signal.EmptyName",
                "Signal name cannot be empty");

            public static readonly Error InvalidSize = new(
                "Signal.InvalidSize",
                "Signal size must be positive");

            public static readonly Error InvalidTime = new(
                "Signal.InvalidTime",
                "Time value cannot be negative");

            public static readonly Error DuplicateTime = new(
                "Signal.DuplicateTime",
                "Time point already exists in signal");
        }

        public static class WaveformData
        {

            public static readonly Error EmptySignalId = new(
                "WaveformData.EmptySignalId",
                "Signal identifier cannot be empty");

            public static readonly Error NullSignal = new(
                "WaveformData.NullSignal",
                "Signal cannot be null");

            public static readonly Error DuplicateSignalId = new(
                "WaveformData.DuplicateSignalId",
                "Signal with this ID already exists");

            public static readonly Error SignalNotFound = new(
                "WaveformData.SignalNotFound",
                "No signal found with the specified ID");
        }

        public static class CodeSnippet
        {
            public static readonly Error EmptyTitle = new(
                "CodeSnippet.EmptyTitle",
                "Title cannot be empty");

            public static readonly Error EmptyCode = new(
                "CodeSnippet.EmptyCode",
                "Verilog code cannot be empty");

            public static readonly Error EmptyTestBench = new(
                "CodeSnippet.EmptyTestBench",
                "Test bench cannot be empty");
        }

        public static class Project
        {
            public static readonly Error EmptyName = new(
                "Project.EmptyName",
                "Project name cannot be empty");

            public static readonly Error NameTooLong = new(
                "Project.NameTooLong",
                "Project name cannot exceed 100 characters");

            public static readonly Error DescriptionTooLong = new(
                "Project.DescriptionTooLong",
                "Description cannot exceed 500 characters");

            public static readonly Error NullSnippet = new(
                "Project.NullSnippet",
                "Cannot add null code snippet");

            public static readonly Error DuplicateSnippet = new(
                "Project.DuplicateSnippet",
                "Snippet already exists in project");

            public static readonly Error SnippetNotFound = new(
                "Project.SnippetNotFound",
                "Specified snippet not found in project");
        }
    }
}
