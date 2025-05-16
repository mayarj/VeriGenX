using VeriGenX.Domain.enums;
using VeriGenX.Domain.Errors;
using VeriGenX.Domain.Shared;


namespace VeriGenX.Domain.ValueObjects
{

    public class WaveformData  : ValueObject
    {

        public TimeUnit TimeUnit { get; private set; }

        public long  TimeScale { get; private set; }
        public Dictionary<string,Signal> _signals { get;private set; }=new Dictionary<string,Signal>();


        private WaveformData( TimeUnit timeUnit , long timeScale  ) 
        {
           
            TimeUnit = timeUnit;
            TimeScale = timeScale;
        }

        public static Result<WaveformData> Create( TimeUnit timeUnit, long timeScale = 1)
        {
  
            return Result.Success(new WaveformData( timeUnit,timeScale));
        }

        public Result AddSignal(string signalId, Signal signal)
        {
            if (string.IsNullOrWhiteSpace(signalId))
                return Result.Failure(DomainErrors.WaveformData.EmptySignalId);

            if (signal == null)
                return Result.Failure(DomainErrors.WaveformData.NullSignal);

            if (_signals.ContainsKey(signalId))
                return Result.Failure(DomainErrors.WaveformData.DuplicateSignalId);

            _signals.Add(signalId, signal);
            return Result.Success();
        }

        public Result<Signal> GetSignal(string signalId)
        {
            if (!_signals.TryGetValue(signalId, out var signal))
                return Result.Failure<Signal>(DomainErrors.WaveformData.SignalNotFound);

            return Result.Success(signal);
        }

        public Result AddValueToSignal(string signalId, int time, double value)
        {
            var signalResult = GetSignal(signalId);
            if (signalResult.IsFailure)
                return Result.Failure(signalResult.Error);

            return signalResult.Value.AddValue(time, value);
        }

        public override IEnumerable<object> GetAtomicValues()
        {
            yield return TimeUnit;
            yield return TimeScale;

            foreach (var signal in _signals)
            {
                yield return signal.Key; // Signal ID
                yield return signal.Value; // Signal object
            }
        }
    }
   
}
