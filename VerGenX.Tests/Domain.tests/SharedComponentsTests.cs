using NUnit.Framework;
using VeriGenX.Domain.Shared;

namespace VeriGenX.Tests.Domain.tests;

[TestFixture]
public class ResultTests
{
    [Test]
    public void Success_ReturnsSuccessResult()
    {
        var result = Result.Success();
        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(Error.None, result.Error);
    }

    [Test]
    public void Failure_ReturnsFailureResult()
    {
        var error = new Error("code", "message");
        var result = Result.Failure(error);

        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual(error, result.Error);
    }

    [Test]
    public void Create_WithNullValue_ReturnsFailure()
    {
        var result = Result.Create<string>(null);
        Assert.IsTrue(result.IsFailure);
        Assert.AreEqual(Error.NullValue, result.Error);
    }

    [Test]
    public void Create_WithNonNullValue_ReturnsSuccess()
    {
        var value = "test";
        var result = Result.Create(value);

        Assert.IsTrue(result.IsSuccess);
        Assert.AreEqual(value, result.Value);
    }
}

[TestFixture]
public class ErrorTests
{
    [Test]
    public void None_HasEmptyCodeAndMessage()
    {
        Assert.AreEqual(string.Empty, Error.None.Code);
        Assert.AreEqual(string.Empty, Error.None.Message);
    }

    [Test]
    public void NullValue_HasCorrectCodeAndMessage()
    {
        Assert.AreEqual("Error.NullValue", Error.NullValue.Code);
        Assert.AreEqual("The specified result value is null.", Error.NullValue.Message);
    }

    [Test]
    public void Equals_WithSameCodeAndMessage_ReturnsTrue()
    {
        var error1 = new Error("code", "message");
        var error2 = new Error("code", "message");

        Assert.IsTrue(error1.Equals(error2));
    }

    [Test]
    public void ToString_ReturnsCode()
    {
        var error = new Error("code", "message");
        Assert.AreEqual("code", error.ToString());
    }
}