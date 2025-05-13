using VeriGenX.Domain.enums;
using VeriGenX.Domain.Errors;
using VeriGenX.Domain.Shared;

namespace VeriGenX.Domain.ValueObjects
{
    public class Signal : ValueObject
    {
        public VerilogDataType Type { get;private  set; }
        public string Name { get;private   set; }
        public int Size { get;private set; }
        public SortedList<int, double> _signalValues { get;private  set; }=new SortedList<int, double>();

        private Signal(VerilogDataType type, string name , int size )
        {
            Type = type;
            Name = name;
            Size = size;
        }
        public static Result<Signal> Create(VerilogDataType type, string name, int size)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure<Signal>(DomainErrors.Signal.EmptyName);

            if (size <= 0)
                return Result.Failure<Signal>(DomainErrors.Signal.InvalidSize);

            return Result.Success(new Signal(type, name.Trim(), size));
        }

        public Result AddValue(int time, double value)
        {
            if (time < 0)
                return Result.Failure(DomainErrors.Signal.InvalidTime);

            if (_signalValues.ContainsKey(time))
                return Result.Failure(DomainErrors.Signal.DuplicateTime);

            _signalValues.Add(time, value);
            return Result.Success();
        }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return Type;
            yield return Name;
            yield return Size;

            foreach (var kvp in _signalValues)
            {
                yield return kvp.Key;
                yield return kvp.Value;
            }
        }

    }
}
