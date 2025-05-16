using NUnit.Framework;
using System.Collections.Generic;
using VeriGenX.Domain.enums;
using VeriGenX.Domain.Errors;
using VeriGenX.Domain.ValueObjects;

namespace VeriGenX.Tests.Domain.tests;


[TestFixture]
public class EmailTests
{
    [Test]
    public void Create_WithEmptyString_ReturnsFailure()
    {
        var result = Email.Create("");
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual(DomainErrors.Email.Empty, result.Error);
    }

    [Test]
    public void Create_WithInvalidFormat_ReturnsFailure()
    {
        var result = Email.Create("invalid-email");
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual(DomainErrors.Email.InvalidFormat, result.Error);
    }

    [Test]
    public void Create_WithValidEmail_ReturnsSuccess()
    {
        var email = "test@example.com";
        var result = Email.Create(email);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(email, result.Value.Value);
    }

    [Test]
    public void Equals_WithSameEmail_ReturnsTrue()
    {
        var email1 = Email.Create("test@example.com").Value;
        var email2 = Email.Create("test@example.com").Value;

        Assert.IsTrue(email1.Equals(email2));
    }
}

[TestFixture]
public class FirstNameTests
{
    [Test]
    public void Create_WithEmptyString_ReturnsFailure()
    {
        var result = FirstName.Create("");
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual(DomainErrors.FirstName.Empty, result.Error);
    }

    [Test]
    public void Create_WithTooLongName_ReturnsFailure()
    {
        var longName = new string('a', FirstName.MaxLength + 1);
        var result = FirstName.Create(longName);

        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual(DomainErrors.FirstName.TooLong, result.Error);
    }

    [Test]
    public void Create_WithValidName_ReturnsSuccess()
    {
        var name = "John";
        var result = FirstName.Create(name);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(name, result.Value.Value);
    }
}

[TestFixture]
public class LastNameTests
{
    [Test]
    public void Create_WithEmptyString_ReturnsFailure()
    {
        var result = LastName.Create("");
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual(DomainErrors.LastName.Empty, result.Error);
    }

    [Test]
    public void Create_WithTooLongName_ReturnsFailure()
    {
        var longName = new string('a', LastName.MaxLength + 1);
        var result = LastName.Create(longName);

        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual(DomainErrors.LastName.TooLong, result.Error);
    }

    [Test]
    public void Create_WithValidName_ReturnsSuccess()
    {
        var name = "Doe";
        var result = LastName.Create(name);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(name, result.Value.Value);
    }
}

[TestFixture]
public class TestResultTests
{
    [Test]
    public void Create_WithPassedTrueAndErrorMessage_ReturnsFailure()
    {
        List<string> errors = new List<string>();
        errors.Add("Error message");
        var result = TestResult.Create(true, null, null ,errors);
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual(DomainErrors.TestResult.InvalidSuccessState, result.Error);
    }

    [Test]
    public void Create_WithPassedFalseAndNoErrorMessage_ReturnsFailure()
    {
        var result = TestResult.Create(false);
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual(DomainErrors.TestResult.InvalidFailureState, result.Error);
    }

    [Test]
    public void Create_WithPassedTrueAndNoErrors_ReturnsSuccess()
    {
        var result = TestResult.Create(true);
        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(result.Value.Passed);
    }

    [Test]
    public void Create_WithPassedFalseAndErrorMessage_ReturnsSuccess()
    {
        var errorMessage = "Test failed";
        List<string> errors = new List<string>();
        errors.Add(errorMessage);
        var result = TestResult.Create(false,null ,null, errors);

        Assert.IsTrue(result.IsSuccess);
        Assert.IsFalse(result.Value.Passed);
        Assert.AreEqual(errors, result.Value.Failures);
    }
}

[TestFixture]
public class WaveformDataTests
{

    private TimeUnit _timeScale;

    [SetUp]
    public void Setup()
    {

        _timeScale = TimeUnit.ns;
    }

    [Test]
    public void Create_ReturnsSuccess()
    {
        var result = WaveformData.Create(_timeScale);
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public void AddSignal_WithEmptyId_ReturnsFailure()
    {
        var waveform = WaveformData.Create(_timeScale).Value;
        var signal = Signal.Create(VerilogDataType.Wire, "valid", 1).Value;

        var result = waveform.AddSignal("", signal);
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual(DomainErrors.WaveformData.EmptySignalId, result.Error);
    }

    [Test]
    public void AddSignal_WithNullSignal_ReturnsFailure()
    {
        var waveform = WaveformData.Create(_timeScale).Value;

        var result = waveform.AddSignal("id1", null);
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual(DomainErrors.WaveformData.NullSignal, result.Error);
    }

    [Test]
    public void AddSignal_WithDuplicateId_ReturnsFailure()
    {
        var waveform = WaveformData.Create(_timeScale).Value;
        var signal = Signal.Create(VerilogDataType.Wire, "valid", 1).Value;
        waveform.AddSignal("id1", signal);

        var result = waveform.AddSignal("id1", signal);
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual(DomainErrors.WaveformData.DuplicateSignalId, result.Error);
    }

    [Test]
    public void AddSignal_WithValidParameters_ReturnsSuccess()
    {
        var waveform = WaveformData.Create(_timeScale).Value;
        var signal = Signal.Create(VerilogDataType.Wire, "valid", 1).Value;

        var result = waveform.AddSignal("id1", signal);
        Assert.IsTrue(result.IsSuccess);
    }

    [Test]
    public void GetSignal_WithNonExistentId_ReturnsFailure()
    {
        var waveform = WaveformData.Create(_timeScale).Value;

        var result = waveform.GetSignal("nonexistent");
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual(DomainErrors.WaveformData.SignalNotFound, result.Error);
    }

    [Test]
    public void AddValueToSignal_WithNonExistentSignal_ReturnsFailure()
    {
        var waveform = WaveformData.Create(_timeScale).Value;

        var result = waveform.AddValueToSignal("nonexistent", 1, 0.5);
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual(DomainErrors.WaveformData.SignalNotFound, result.Error);
    }
}

