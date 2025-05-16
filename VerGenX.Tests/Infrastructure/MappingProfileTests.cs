using AutoMapper;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using VeriGenX.Domain.Entites;
using VeriGenX.Domain.enums;
using VeriGenX.Domain.ValueObjects;
using VeriGenX.Infrastructure.DAO;
using VeriGenX.Infrastructure.Profiles;

namespace VeriGenX.Tests.Infrastructure
{
    [TestFixture]
    public class MappingProfileTests
    {
        private IMapper _mapper;
        private MapperConfiguration _config;

        [SetUp]
        public void Setup()
        {
            _config = new MapperConfiguration(cfg => cfg.AddProfile<MappingProfile>());
            _mapper = _config.CreateMapper();
        }

        [Test]
        public void Configuration_IsValid()
        {
            _config.AssertConfigurationIsValid();
        }

        [Test]
        public void Map_UserDocument_To_User()
        {
            // Arrange
            var userDoc = new UserDocument
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com"
            };

            // Act
            var user = _mapper.Map<User>(userDoc);

            // Assert
            Assert.AreEqual(userDoc.Id, user.Id.userId.ToString());
            Assert.AreEqual(userDoc.FirstName, user.FirstName.Value);
            Assert.AreEqual(userDoc.LastName, user.LastName.Value);
            Assert.AreEqual(userDoc.Email, user.Email.Value);
        }

        [Test]
        public void Map_User_To_UserDocument()
        {

            // Arrange
            var user = User.Create(
                new UserId(Guid.NewGuid()),
                FirstName.Create("John").Value,
                 LastName.Create("Smith").Value,
                 Email.Create("jane.smith@example.com").Value).Value;

            // Act
            var userDoc = _mapper.Map<UserDocument>(user);

            // Assert
            Assert.AreEqual(user.Id.userId.ToString(), userDoc.Id);
            Assert.AreEqual(user.FirstName.Value, userDoc.FirstName);
            Assert.AreEqual(user.LastName.Value, userDoc.LastName);
            Assert.AreEqual(user.Email.Value, userDoc.Email);
        }

        [Test]
        public void Map_SignalDocument_To_Signal()
        {
            SortedList<int, double> sortedList = new SortedList<int, double>();
            sortedList.Add(1, 1.1);
            sortedList.Add(2, 2.2);
            sortedList.Add(3, 3.3);
            var signalDoc = new SignalDocument
            {
                Type = "reg",
                Name = "clock",
                Size = 1,
                _signalValues = sortedList, 
            };

            // Act
            var signal = _mapper.Map<Signal>(signalDoc);

            // Assert
            Assert.AreEqual(VerilogDataType.Reg, signal.Type);
            Assert.AreEqual(signalDoc.Name, signal.Name);
            Assert.AreEqual(signalDoc.Size, signal.Size);
            CollectionAssert.AreEqual(signalDoc._signalValues, signal._signalValues);
        }

        [Test]
        public void Map_Signal_To_SignalDocument()
        {
            // Arrange
            var signal = Signal.Create(
                VerilogDataType.Wire,
                "data",
                8).Value;
            signal.AddValue(1, 1.1);
            signal.AddValue(2, 2.2);
            signal.AddValue(3, 3.3);

            // Act
            var signalDoc = _mapper.Map<SignalDocument>(signal);

            // Assert
            Assert.AreEqual(signal.Type.ToString(), signalDoc.Type);
            Assert.AreEqual(signal.Name, signalDoc.Name);
            Assert.AreEqual(signal.Size, signalDoc.Size);
            CollectionAssert.AreEqual(signal._signalValues, signalDoc._signalValues);
        }

        [Test]
        public void Map_TestResultDocument_To_TestResult()
        {
            List<string> failures = new List<string>();
            failures.Add("sssss");
            failures.Add("asdasd");
            // Arrange
            var testResultDoc = new TestResultDocument
            {
                Passed = true,
                CodeErrorLines = new() { 10, 15 },
                TestErrorLines = new() { 20, 25 },
                TestTime = DateTime.UtcNow,
                Failures = failures,
            };

            // Act
            var testResult = _mapper.Map<TestResult>(testResultDoc);

            // Assert
            Assert.AreEqual(testResultDoc.Passed, testResult.Passed);
            CollectionAssert.AreEqual(testResultDoc.CodeErrorLines, testResult.CodeErrorLines);
            CollectionAssert.AreEqual(testResultDoc.TestErrorLines, testResult.TestErrorLines);
            Assert.AreEqual(testResultDoc.TestTime, testResult.TestTime);
            Assert.AreEqual(testResultDoc.Failures, testResult.Failures);
        }

        [Test]
        public void Map_TestResult_To_TestResultDocument()
        {
            List<string> failures = new List<string>();
            failures.Add("sssss");
            failures.Add("asdasd");
            // Arrange
            var testResult =  TestResult.Create(
                false,
                new() { 5, 10 },
                new() { 15, 20 },
                failures).Value;

            // Act
            var testResultDoc = _mapper.Map<TestResultDocument>(testResult);

            // Assert
            Assert.AreEqual(testResult.Passed, testResultDoc.Passed);
            CollectionAssert.AreEqual(testResult.CodeErrorLines, testResultDoc.CodeErrorLines);
            CollectionAssert.AreEqual(testResult.TestErrorLines, testResultDoc.TestErrorLines);
            Assert.AreEqual(testResult.TestTime, testResultDoc.TestTime);
            Assert.AreEqual(testResult.Failures, testResultDoc.Failures);
        }

        [Test]
        public void Map_CodeSnippetDocument_To_CodeSnippet()
        {
            List<string> failures = new List<string>();
            failures.Add("sssss");
            failures.Add("asdasd");
            // Arrange
            var snippetDoc = new CodeSnippetDocument
            {
                SnippetId = Guid.NewGuid().ToString(),
                ProjectId = Guid.NewGuid().ToString(),
                Title = "Test Snippet",
                Description = "Test Description",
                VerilogCode = "module test; endmodule",
                TestBench = "module tb; endmodule",
                CreatedDate = DateTime.Now,
                LastModified = DateTime.Now,
                TestResultDocument = new TestResultDocument
                {
                    Passed = true,
                    CodeErrorLines = new(),
                    TestErrorLines = new(),
                    TestTime = DateTime.Now,
                    Failures = failures
                },
                WaveformDataDocument = new()
            };

            // Act
            var snippet = _mapper.Map<CodeSnippet>(snippetDoc);

            // Assert
            Assert.AreEqual(snippetDoc.SnippetId, snippet.SnippetId.snippetId.ToString());
            Assert.AreEqual(snippetDoc.Title, snippet.Title);
            Assert.AreEqual(snippetDoc.Description, snippet.Description);
            Assert.AreEqual(snippetDoc.VerilogCode, snippet.VerilogCode);
            Assert.AreEqual(snippetDoc.TestBench, snippet.TestBench);
            Assert.AreEqual(snippetDoc.CreatedDate, snippet.CreatedDate);
            Assert.AreEqual(snippetDoc.LastModified, snippet.LastModified);
            Assert.IsNotNull(snippet.TestResult);
            Assert.IsNotNull(snippet.Waveform);
        }

        [Test]
        public void Map_CodeSnippet_To_CodeSnippetDocument()
        {

            // Arrange
            var snippet = CodeSnippet.Create(
                new SnippetId(Guid.NewGuid()),
                new ProjectId(Guid.NewGuid()),
                "Test Snippet",
                "Test Description",
                "module test; endmodule",
                "module tb; endmodule").Value;



            var result = TestResult.Create(true).Value;
            snippet.SetTestResult(result);
            var _timeScale = TimeUnit.ns;
            var waveform = WaveformData.Create(_timeScale).Value;

            waveform.AddSignal("id1", null);
            snippet.SetWaveform(waveform);
            // Act
            var snippetDoc = _mapper.Map<CodeSnippetDocument>(snippet);

            // Assert
            Assert.AreEqual(snippet.SnippetId.snippetId.ToString(), snippetDoc.SnippetId);
            Assert.AreEqual(snippet.ProjectId.projectId.ToString(), snippetDoc.ProjectId);
            Assert.AreEqual(snippet.Title, snippetDoc.Title);
            Assert.AreEqual(snippet.Description, snippetDoc.Description);
            Assert.AreEqual(snippet.VerilogCode, snippetDoc.VerilogCode);
            Assert.AreEqual(snippet.TestBench, snippetDoc.TestBench);
            Assert.AreEqual(snippet.CreatedDate, snippetDoc.CreatedDate);
            Assert.AreEqual(snippet.LastModified, snippetDoc.LastModified);
            Assert.IsNotNull(snippetDoc.TestResultDocument);
            Assert.IsNotNull(snippetDoc.WaveformDataDocument);
        }

        [Test]
        public void Map_ProjectDocument_To_Project()
        {
            // Arrange
            var projectDoc = new ProjectDocument
            {
                ProjectId = Guid.NewGuid().ToString(),
                UserId = Guid.NewGuid().ToString(),
                Name = "Test Project",
                Description = "Test Description",
                CreatedDate = DateTime.Now,
                LastModified = DateTime.Now
            };

            // Act
            var project = _mapper.Map<Project>(projectDoc);

            // Assert
            Assert.AreEqual(projectDoc.ProjectId, project.ProjectId.projectId.ToString());
            Assert.AreEqual(projectDoc.Name, project.Name);
            Assert.AreEqual(projectDoc.Description, project.Description);
            Assert.AreEqual(projectDoc.CreatedDate, project.CreatedDate);
            Assert.AreEqual(projectDoc.LastModified, project.LastModified);
        }

        [Test]
        public void Map_Project_To_ProjectDocument()
        {
            // Arrange
            var project = Project.Create(
                new ProjectId(Guid.NewGuid()),
                new UserId(Guid.NewGuid()),
                "Test Project",
                "Test Description").Value;

            // Act
            var projectDoc = _mapper.Map<ProjectDocument>(project);

            // Assert
            Assert.AreEqual(project.ProjectId.projectId.ToString(), projectDoc.ProjectId);
            Assert.AreEqual(project.UserId.userId.ToString(), projectDoc.UserId);
            Assert.AreEqual(project.Name, projectDoc.Name);
            Assert.AreEqual(project.Description, projectDoc.Description);
            Assert.AreEqual(project.CreatedDate, projectDoc.CreatedDate);
            Assert.AreEqual(project.LastModified, projectDoc.LastModified);
        }
    }
}
