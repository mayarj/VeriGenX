using VeriGenX.Domain.Entites;
using VeriGenX.Domain.Shared;
using VeriGenX.Domain.ValueObjects;

namespace VeriGenX.Domain.Repository
{
    public interface IVerilogRepository: IDisposable
    {
        Task<Result> SimulationToolIsActiveAsync();
        Task<Result<TestResult>> RunTestsAsync(string verilogCode, string testCode = "");
        Task<Result<WaveformData>> RunSimulationAsync();
        void StopAsync();
    }
}
