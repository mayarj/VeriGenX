using Docker.DotNet;
using Docker.DotNet.Models;
using System.Text;

namespace VeriGenX.SimulationTool
{
    public class DockerVerilogService : IDisposable
    {
        private readonly DockerClient _dockerClient;
        private const string ImageName = "iverilog-container";
        private const string ContainerWorkDir = "/workspace";
        private readonly string _containerId;
        private readonly string _containerName;
        public DockerVerilogService(DockerClient dockerClient = null)
        {
            _dockerClient = dockerClient ?? CreateDefaultClient();
            _containerName = $"iverilog_{Guid.NewGuid().ToString("N")}";
           // var localVerilogDir = Path.Combine(Directory.GetCurrentDirectory(), "VerilogFiles");

            //var hostConfig = new HostConfig
            //{
            //    Binds = new List<string>
            //    {
            //        $"{Path.GetFullPath(localVerilogDir)}:{ContainerWorkDir}"
            //    }
            //};

            var containerParameters = new CreateContainerParameters
            {
                Image = ImageName,
                Name = _containerName,
                //HostConfig = hostConfig,
                WorkingDir = ContainerWorkDir,
                Tty = true,
                AttachStdout = true,
                AttachStderr = true
            };

            var container = _dockerClient.Containers.CreateContainerAsync(containerParameters).Result;
            _containerId = container.ID;
            _dockerClient.Containers.StartContainerAsync(_containerId, new ContainerStartParameters()).Wait();
        }

        public DockerVerilogService(string containerName, DockerClient dockerClient = null)
        {
            _dockerClient = dockerClient ?? CreateDefaultClient();
            _containerName = containerName;
            // Try to find existing container
            var containers = _dockerClient.Containers.ListContainersAsync(new ContainersListParameters
            {
                All = true
            }).Result;

            var existingContainer = containers.FirstOrDefault(c => c.Names.Any(n => n == $"/{containerName}"));

            if (existingContainer != null)
            {
                _containerId = existingContainer.ID;

                // Start container if not running
                if (existingContainer.State != "running")
                {
                    _dockerClient.Containers.StartContainerAsync(_containerId, new ContainerStartParameters()).Wait();
                }
                return;
            }
            var localVerilogDir = Path.Combine(Directory.GetCurrentDirectory(), "VerilogFiles");

            var hostConfig = new HostConfig
            {
                Binds = new List<string>
                {
                    $"{Path.GetFullPath(localVerilogDir)}:{ContainerWorkDir}"
                }
            };

            var containerParameters = new CreateContainerParameters
            {
                Image = ImageName,
                Name = containerName,
                HostConfig = hostConfig,
                WorkingDir = ContainerWorkDir,
                Tty = true,
                AttachStdout = true,
                AttachStderr = true
            };

            var container = _dockerClient.Containers.CreateContainerAsync(containerParameters).Result;
            _containerId = container.ID;
            _dockerClient.Containers.StartContainerAsync(_containerId, new ContainerStartParameters()).Wait();


        }
        public async Task<(bool IsSuccess, string Output)> ExecuteCommandAsync(string command)
        {
            using (var process = new System.Diagnostics.Process())

            {
                process.StartInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "docker",
                    Arguments = $"exec {_containerId} /bin/sh -c \"{command.Replace("\"", "\\\"")}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };


                process.Start();
                string output = await process.StandardOutput.ReadToEndAsync();
                string error = await process.StandardError.ReadToEndAsync();
                await process.WaitForExitAsync();
                bool isSuccess = process.ExitCode == 0;
                if (!string.IsNullOrEmpty(error))
                {
                    output += "\nERROR: " + error;
                }

                return (isSuccess, output);
            }
        }

        public async Task<(bool IsSuccess, string Output)> CreateFileAsync(string filePath, string content)
        {
            {
                try
                {
                    // Normalize path (Docker prefers Linux-style paths)
                    filePath = filePath.Replace('\\', '/').TrimStart('/');

                    // Create parent directories first (atomic operation)
                    var directoryPath = Path.GetDirectoryName(filePath)?.Replace('\\', '/');
                    if (!string.IsNullOrEmpty(directoryPath))
                    {
                        var mkdirResult = await ExecuteCommandAsync($"mkdir -p \"{directoryPath}\"");
                        if (!mkdirResult.IsSuccess)
                            return (false, $"Failed to create directory: {mkdirResult.Output}");
                    }

                    // Method 1: Base64 encoding (most reliable for all content)
                    var base64Content = Convert.ToBase64String(Encoding.UTF8.GetBytes(content));
                    var createResult = await ExecuteCommandAsync(
                        $"echo \"{base64Content}\" | base64 --decode > \"{filePath}\""
                    );

                    // Fallback Method 2: If base64 isn't available
                    if (!createResult.IsSuccess)
                    {
                        // Use here-document for safer content passing
                        createResult = await ExecuteCommandAsync(
                            $"cat > \"{filePath}\" << 'EOF'\n{content}\nEOF\n"
                        );
                    }

                    // Verify file was created
                    if (createResult.IsSuccess)
                    {
                        var verifyResult = await ExecuteCommandAsync($"test -f \"{filePath}\" && echo exists");
                        if (!verifyResult.Output.Contains("exists"))
                            return (false, "File verification failed");
                    }

                    return createResult;
                }
                catch (Exception ex)
                {
                    return (false, $"Unexpected error: {ex.Message}");
                }
            }
        }

        public async Task<(bool IsSuccess, string Output)> GetFileContentAsync(string filePath)
        {
            // Ensure the file path is relative to the container's working directory
            if (Path.IsPathRooted(filePath))
            {
                filePath = filePath.TrimStart('/');
            }

            // Use cat command to read the file content
            var result = await ExecuteCommandAsync($"cat {filePath}");



            return result;
        }

        public async Task<(bool IsSuccess, string output)> RunTests(string verilogFile, string verilogTestFile, string vvpOutput)
        {
            string command = $"iverilog -o {vvpOutput} {verilogFile} { verilogTestFile}  ";
            var result = await ExecuteCommandAsync(command);
            return result;
        }

        public async Task<(bool IsSuccess, string output)> RunSimulationAsync(string vvpFile)
        {
            string command = $"vvp {vvpFile}";
            return await ExecuteCommandAsync(command);
        }

        private DockerClient CreateDefaultClient()
        {
            var dockerUri = Environment.OSVersion.Platform == PlatformID.Unix
                ? "unix:///var/run/docker.sock"
                : "npipe://./pipe/docker_engine";

            return new DockerClientConfiguration(new Uri(dockerUri))
                .CreateClient();
        }

        public async Task StopContainerAsync()
        {
            await _dockerClient.Containers.StopContainerAsync(_containerId, new ContainerStopParameters());
        }

        public void Dispose()
        {
            // Clean up resources and stop/remove the container if needed
            _dockerClient.Containers.StopContainerAsync(_containerId, new ContainerStopParameters()).Wait();
            _dockerClient.Containers.RemoveContainerAsync(_containerId, new ContainerRemoveParameters()).Wait();
            _dockerClient.Dispose();
        }
    }

}