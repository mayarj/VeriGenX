using VeriGenX.Domain.Repository;
using VeriGenX.Domain.Shared;
using VeriGenX.Domain.ValueObjects;
using VeriGenX.SimulationTool.Errors;
using VeriGenX.SimulationTool.Parsers;

namespace VeriGenX.SimulationTool
{
    public class VerilogRepository : IVerilogRepository
    {
        private readonly DockerVerilogService _dockerService;
        private bool _disposed;
        private const string MainVerilogFile = "main.v";
        private const string TestVerilogFile = "test.v";
        private const string VvpOutputFile = "output.vvp";
        private const string VcdFile = "waveform.vcd";

        public VerilogRepository(string containerName)
        {

            _dockerService = new DockerVerilogService(containerName);

        }
        public VerilogRepository()
        {
            _dockerService = new DockerVerilogService();

        }
        public VerilogRepository(DockerVerilogService dockerService)
        {
            this._dockerService = dockerService;
        }

        public async Task<Result> SimulationToolIsActiveAsync()
        {
            try
            {
                // Simple test command to verify container is responsive
                var testResult = await _dockerService.ExecuteCommandAsync("echo 'test'");
                return testResult.IsSuccess
                    ? Result.Success()
                    : Result.Failure(SimulationErrors.Docker.NotResponding);
            }
            catch (Exception ex)
            {
                return Result.Failure(new ContainerError(ex.Message));
            }
        }
        public async Task<Result<TestResult>> RunTestsAsync(string verilogCode, string testCode = "")
        {

            try
            {
                // Run both file creations concurrently
                var createCodeFileTask = _dockerService.CreateFileAsync(MainVerilogFile, verilogCode);
                var createTestFileTask = _dockerService.CreateFileAsync(TestVerilogFile, testCode);

                // Wait for both to complete
                await Task.WhenAll(createCodeFileTask, createTestFileTask);

                if (!createCodeFileTask.Result.IsSuccess || !createCodeFileTask.Result.IsSuccess)
                {
                    return Result.Failure<TestResult>(SimulationErrors.File.CreationFailed);
                }

                // Validate syntax
                var validationResult = await _dockerService.RunTests(MainVerilogFile, TestVerilogFile, VvpOutputFile);
                var failures = OutputParser.Parse(validationResult.output);
                var (codeErrors, testErrors) = OutputParser.GetErrorLines(failures);

                var testResult = TestResult.Create(validationResult.IsSuccess, codeErrors, testErrors, failures);
                return testResult;
            }
            catch (Exception ex)
            {
                return Result.Failure<TestResult>(new CompilationError(ex.Message));
            }
        }

        public async Task<Result<WaveformData>> RunSimulationAsync()
        {
            try
            {
                var vcd = await _dockerService.RunSimulationAsync(VvpOutputFile);
                if (!vcd.IsSuccess)
                {
                    return Result.Failure<WaveformData>(new CompilationError(vcd.output));
                }
                var vcdContent = await _dockerService.GetFileContentAsync(VcdFile);
                if (vcdContent.IsSuccess)
                {
                    var waveFormData = VcdParser.Parse(vcdContent.Output);
                    return waveFormData;
                }
                return Result.Failure<WaveformData>(SimulationErrors.File.ReadFailed);
            }
            catch (Exception ex)
            {
                return Result.Failure<WaveformData>(new CompilationError(ex.Message));

            }

        }

        public async void StopAsync()
        {
            await _dockerService.StopContainerAsync().ConfigureAwait(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _dockerService?.Dispose();
                }
                _disposed = true;
            }
        }
    }
}