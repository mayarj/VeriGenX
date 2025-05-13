using VeriGenX.Domain.Errors;
using VeriGenX.Domain.Shared;

namespace VeriGenX.Domain.ValueObjects;

public sealed class Email : ValueObject
{
    private Email(string value)
    {
        Value = value;
    }

    public string Value { get; }

    public static Result<Email> Create(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
        {
            return Result.Failure<Email>(DomainErrors.Email.Empty);
        }

        if (firstName.Split('@').Length != 2)
        {
            return Result.Failure<Email>(DomainErrors.Email.InvalidFormat);
        }

        return new Email(firstName);
    }

    public override IEnumerable<object> GetAtomicValues()
    {
        yield return Value;
    }
}