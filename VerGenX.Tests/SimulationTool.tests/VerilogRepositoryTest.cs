
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using VeriGenX.Domain.enums;
using VeriGenX.Domain.ValueObjects;
using VeriGenX.SimulationTool;
using VeriGenX.SimulationTool.Parsers;

namespace VeriGenX.Tests.SimulationTool.tests;
[TestFixture]
public class VerilogRepositoryTest
{
    private VerilogRepository verilogRepository ;

    [SetUp]
    public void Setup()
    {
        verilogRepository = new VerilogRepository();
    }
    [Test]
    public void VcdParserTEst()
    {
        var waveformData = WaveformData.Create(TimeUnit.ns, 1);
        var signal = Signal.Create(VerilogDataType.Wire, "clk", 1);
        waveformData.Value.AddSignal("a", signal.Value);
        waveformData.Value.AddValueToSignal("a", 0, 0);  // #0
        waveformData.Value.AddValueToSignal("a", 5, 1);  // #5
        waveformData.Value.AddValueToSignal("a", 10, 0); // #10
        waveformData.Value.AddValueToSignal("a", 15, 1); // #15
        waveformData.Value.AddValueToSignal("a", 20, 0); // #20
        var dataSignalResult = Signal.Create(VerilogDataType.Wire, "data", 8);
        waveformData.Value.AddSignal("c", dataSignalResult.Value);
        waveformData.Value.AddValueToSignal("c", 0, 0);         
        waveformData.Value.AddValueToSignal("c", 10, 255);      
        waveformData.Value.AddValueToSignal("c", 25, 0);        
        waveformData.Value.AddValueToSignal("c", 30, 255);      
        waveformData.Value.AddValueToSignal("c", 35, 1);        
        waveformData.Value.AddValueToSignal("c", 40, 0);      
        waveformData.Value.AddValueToSignal("c", 45, 1);
        waveformData.Value.AddValueToSignal("c", 50, 0);
        waveformData.Value.AddValueToSignal("c", 55, 0);
        waveformData.Value.AddValueToSignal("c", 60, 1);
        waveformData.Value.AddValueToSignal("c", 65, 1);
        waveformData.Value.AddValueToSignal("c", 70, 0);
        waveformData.Value.AddValueToSignal("c", 75, 0);      
        waveformData.Value.AddValueToSignal("c", 80, 1);
        waveformData.Value.AddValueToSignal("c", 85, 1);
        waveformData.Value.AddValueToSignal("c", 90, 0);      
        waveformData.Value.AddValueToSignal("c", 95, 1);        
        var testFile = "testFiles/vcdParcerTest.txt";
        string test = File.ReadAllText(testFile);
        var result = VcdParser.Parse(test);
        Assert.AreEqual(string.Empty, result.Error.Message);
        Assert.IsTrue(result.IsSuccess);
        Assert.IsTrue(waveformData.IsSuccess);
        Assert.AreEqual(result.Value.TimeUnit , waveformData.Value.TimeUnit);
        Assert.AreEqual(waveformData.Value.TimeScale , result.Value.TimeScale);
        Assert.AreEqual(waveformData.Value.GetSignal("a").Value , result.Value.GetSignal("a").Value);
        Assert.AreEqual(waveformData.Value.GetSignal("c").Value, result.Value.GetSignal("c").Value);
        Assert.AreEqual(result.Value, waveformData.Value);
    }

    [Test]
    public async Task CompileVerilogAsync()
    {
        // Arrange

        var testFile = "testFiles/test.txt";
        var testBenchFile = "testFiles/testBench.txt";

        string test = File.ReadAllText(testFile);
        string testBench = File.ReadAllText(testBenchFile);
       
        var result =  await verilogRepository.RunTestsAsync(test, testBench);
        Assert.AreEqual(string.Empty , result.Error.Message);
        Assert.IsTrue(result.IsSuccess);
        List<string> var = new List<string>();
        Assert.AreEqual(var, result.Value.Failures);
        Assert.IsTrue(result.Value.Passed);
   
    }
    [Test]
    public async Task RunsemulationVerilogAsync()
    {
        // Arrange

        var testFile = "testFiles/test.txt";
        var testBenchFile = "testFiles/testBench.txt";

        string test = File.ReadAllText(testFile);
        string testBench = File.ReadAllText(testBenchFile);
        
        await verilogRepository.RunTestsAsync(test, testBench);
        var result =await  verilogRepository.RunSimulationAsync();
        Assert.AreEqual("", result.Error.Message);
        Assert.IsTrue(result.IsSuccess);

    }

    [TearDown]
    public void TearDown()
    {
      verilogRepository.Dispose();
    }
}