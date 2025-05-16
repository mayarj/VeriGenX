using NUnit.Framework;
using System;
using VeriGenX.Domain.Entites;

using VeriGenX.Domain.enums;
using VeriGenX.Domain.Errors;
using VeriGenX.Domain.Shared;
using VeriGenX.Domain.ValueObjects;


namespace VeriGenX.Tests.Domain.tests;

    [TestFixture]
    public class SignalTests
    {
        [Test]
        public void Create_WithEmptyName_ReturnsFailure()
        {
            var result = Signal.Create(VerilogDataType.Wire, "", 1);
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(DomainErrors.Signal.EmptyName, result.Error);
        }

        [Test]
        public void Create_WithInvalidSize_ReturnsFailure()
        {
            var result = Signal.Create(VerilogDataType.Wire, "valid", 0);
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(DomainErrors.Signal.InvalidSize, result.Error);
        }

        [Test]
        public void Create_WithValidParameters_ReturnsSuccess()
        {
            var result = Signal.Create(VerilogDataType.Wire, "valid", 1);
            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual("valid", result.Value.Name);
        }

        [Test]
        public void AddValue_WithNegativeTime_ReturnsFailure()
        {
            var signal = Signal.Create(VerilogDataType.Wire, "valid", 1).Value;
            var result = signal.AddValue(-1, 0.5);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(DomainErrors.Signal.InvalidTime, result.Error);
        }

        [Test]
        public void AddValue_WithDuplicateTime_ReturnsFailure()
        {
            var signal = Signal.Create(VerilogDataType.Wire, "valid", 1).Value;
            signal.AddValue(1, 0.5);
            var result = signal.AddValue(1, 0.6);

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(DomainErrors.Signal.DuplicateTime, result.Error);
        }

        [Test]
        public void AddValue_WithValidParameters_ReturnsSuccess()
        {
            var signal = Signal.Create(VerilogDataType.Wire, "valid", 1).Value;
            var result = signal.AddValue(1, 0.5);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(1, signal._signalValues.Count);
        }
    }
    [TestFixture]
    public class CodeSnippetTests
    {
        private SnippetId _snippetId;
        private ProjectId _projectId;

        [SetUp]
        public void Setup()
        {
            _snippetId = new SnippetId(Guid.NewGuid());
            _projectId = new ProjectId(Guid.NewGuid()); 

        }

        [Test]
        public void Create_WithEmptyTitle_ReturnsFailure()
        {
            var result = CodeSnippet.Create(_snippetId, _projectId, "", "desc", "code", "test");
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(DomainErrors.CodeSnippet.EmptyTitle, result.Error);
        }

        [Test]
        public void Create_WithEmptyCode_ReturnsFailure()
        {
            var result = CodeSnippet.Create(_snippetId, _projectId, "title", "desc", "", "test");
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(DomainErrors.CodeSnippet.EmptyCode, result.Error);
        }

        [Test]
        public void Create_WithEmptyTestBench_ReturnsFailure()
        {
            var result = CodeSnippet.Create(_snippetId, _projectId, "title", "desc", "code", "");
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(DomainErrors.CodeSnippet.EmptyTestBench, result.Error);
        }

        [Test]
        public void Create_WithValidParameters_ReturnsSuccess()
        {
            var title = "Test Snippet";
            var description = "Description";
            var code = "module test; endmodule";
            var testBench = "test bench code";

            var result = CodeSnippet.Create(_snippetId, _projectId, title, description, code, testBench);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(title, result.Value.Title);
            Assert.AreEqual(description, result.Value.Description);
            Assert.AreEqual(code, result.Value.VerilogCode);
            Assert.AreEqual(testBench, result.Value.TestBench);
        }

        [Test]
        public void UpdateCode_WithEmptyCode_ReturnsFailure()
        {
            var snippet = CodeSnippet.Create(_snippetId, _projectId, "title", "desc", "code", "test").Value;
            var result = snippet.UpdateCode("");

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(DomainErrors.CodeSnippet.EmptyCode, result.Error);
        }

        [Test]
        public void UpdateTestBench_WithEmptyTestBench_ReturnsFailure()
        {
            var snippet = CodeSnippet.Create(_snippetId, _projectId, "title", "desc", "code", "test").Value;
            var result = snippet.UpdateTestBench("");

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(DomainErrors.CodeSnippet.EmptyTestBench, result.Error);
        }

        [Test]
        public void SetTestResult_UpdatesLastModified()
        {
            var snippet = CodeSnippet.Create(_snippetId, _projectId, "title", "desc", "code", "test").Value;
            var testResult = TestResult.Create(true).Value;
            var beforeUpdate = snippet.LastModified;

            snippet.SetTestResult(testResult);

            Assert.Greater(snippet.LastModified, beforeUpdate);
        }
    }

    [TestFixture]
    public class ProjectTests
    {
        private ProjectId _projectId;
        private UserId _userId;

        [SetUp]
        public void Setup()
        {
            _projectId = new ProjectId(Guid.NewGuid());
            _userId = new UserId(Guid.NewGuid());
        }

        [Test]
        public void Create_WithEmptyName_ReturnsFailure()
        {
            var result = Project.Create(_projectId, _userId, "", "desc");
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(DomainErrors.Project.EmptyName, result.Error);
        }

        [Test]
        public void Create_WithTooLongName_ReturnsFailure()
        {
            var longName = new string('a', 101);
            var result = Project.Create(_projectId, _userId, longName, "desc");

            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(DomainErrors.Project.NameTooLong, result.Error);
        }

        [Test]
        public void Create_WithValidParameters_ReturnsSuccess()
        {
            var name = "Test Project";
            var description = "Description";

            var result = Project.Create(_projectId, _userId, name, description);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(name, result.Value.Name);
            Assert.AreEqual(description, result.Value.Description);
        }

        [Test]
        public void UpdateDescription_WithTooLongDescription_ReturnsFailure()
        {
            var project = Project.Create(_projectId, _userId, "name", "desc").Value;
            var longDesc = new string('a', 501);

            var result = project.UpdateDescription(longDesc);
            Assert.IsTrue(result.IsFailure);
            Assert.AreEqual(DomainErrors.Project.DescriptionTooLong, result.Error);
        }

        //[Test]
        //public void AddSnippet_WithNullSnippet_ReturnsFailure()
        //{
        //    var project = Project.Create(_projectId, _userId, "name", "desc").Value;

        //    var result = project.AddSnippet(null);
        //    Assert.IsTrue(result.IsFailure);
        //    Assert.AreEqual(DomainErrors.Project.NullSnippet, result.Error);
        //}

        //[Test]
        //public void AddSnippet_WithDuplicateSnippet_ReturnsFailure()
        //{
        //    var project = Project.Create(_projectId, _userId, "name", "desc").Value;
        //    var snippetId = new SnippetId(Guid.NewGuid());
        //    var snippet = CodeSnippet.Create(snippetId, _projectId, "title", "desc", "code", "test").Value;

        //    project.AddSnippet(snippet);
        //    var result = project.AddSnippet(snippet);

        //    Assert.IsTrue(result.IsFailure);
        //    Assert.AreEqual(DomainErrors.Project.DuplicateSnippet, result.Error);
        //}

        //[Test]
        //public void RemoveSnippet_WithNonExistentSnippet_ReturnsFailure()
        //{
        //    var project = Project.Create(_projectId, _userId, "name", "desc").Value;
        //    var snippetId = new SnippetId(Guid.NewGuid());

        //    var result = project.RemoveSnippet(snippetId);
        //    Assert.IsTrue(result.IsFailure);
        //    Assert.AreEqual(DomainErrors.Project.SnippetNotFound, result.Error);
        //}
    }

    [TestFixture]
    public class UserTests
    {
        private UserId _userId;
        private FirstName _firstName;
        private LastName _lastName;
        private Email _email;

        [SetUp]
        public void Setup()
        {
            _userId = new UserId(Guid.NewGuid());
            _firstName = FirstName.Create("John").Value;
            _lastName = LastName.Create("Doe").Value;
            _email = Email.Create("john.doe@example.com").Value;
        }

        [Test]
        public void Create_ReturnsValidUser()
        {
            var result = User.Create(_userId, _firstName, _lastName, _email);

            Assert.IsTrue(result.IsSuccess);
            Assert.AreEqual(_userId, result.Value.Id);
            Assert.AreEqual(_firstName, result.Value.FirstName);
            Assert.AreEqual(_lastName, result.Value.LastName);
            Assert.AreEqual(_email, result.Value.Email);
        }
    }
