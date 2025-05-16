using System.Text.RegularExpressions;
using VeriGenX.Domain.Entites;
using VeriGenX.Domain.enums;
using VeriGenX.Domain.Shared;
using VeriGenX.Domain.ValueObjects;
using VeriGenX.SimulationTool.Errors;

namespace VeriGenX.SimulationTool.Parsers
{
    public static class VcdParser
    {


        public static Result<WaveformData> Parse(string vcdContent)
        {
            try
            {
                //var  vcdContent = vcd.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                // Parse the timescale
                var timescaleMatch = Regex.Match(vcdContent, @"\$timescale\s+(\d+)\s*([a-zA-Z]+)\s*\$end");
                long.TryParse(timescaleMatch.Groups[1].Value, out var timeScale);
                var timeUnitStr = timescaleMatch.Groups[2].Value.ToLower();
                var timeUnit = timeUnitStr switch
                {
                    "s" => TimeUnit.s,
                    "ms" => TimeUnit.ms,
                    "us" => TimeUnit.us,
                    "ns" => TimeUnit.ns,
                    "ps" => TimeUnit.ps,
                    "fs" => TimeUnit.fs,
                    _ => TimeUnit.ns // default to ns if not specified
                };

      
               
                var waveformResult = WaveformData.Create( timeUnit, timeScale);
                if (waveformResult.IsFailure)
                    return waveformResult;

                var waveformData = waveformResult.Value;

                // Parse variable definitions
                var varDefinitions = Regex.Matches(vcdContent, @"\$var\s+(\w+)\s+(\d+)\s+(\S+)\s+(\S+)\s*\$end");
                foreach (Match varDef in varDefinitions)
                {
                    var typeStr = varDef.Groups[1].Value;
                    var size = int.Parse(varDef.Groups[2].Value);
                    var identifier = varDef.Groups[3].Value;
                    var name = varDef.Groups[4].Value;
                    var type = typeStr switch
                    {
                        "wire" => VerilogDataType.Wire,
                        "reg" => VerilogDataType.Reg,
                        "input" => VerilogDataType.Input,
                        "output" => VerilogDataType.Output,
                        "integer" => VerilogDataType.Integer,
                        "real" => VerilogDataType.Real,
                        "time" => VerilogDataType.Time,
                        "realtime" => VerilogDataType.Realtime,
                        "event" => VerilogDataType.Event,
                        "supply0" => VerilogDataType.Supply0,
                        "supply1" => VerilogDataType.Supply1,
                        _ => VerilogDataType.Wire // default to wire if unknown
                    };

                    var signalResult = Signal.Create(type, name, size);
                    if (signalResult.IsFailure)
                        return Result.Failure<WaveformData>(signalResult.Error);

                    var addSignalResult = waveformData.AddSignal(identifier, signalResult.Value);
                    if (addSignalResult.IsFailure)
                        continue;
                        //return Result.Failure<WaveformData>(addSignalResult.Error);
                }

                // Parse value changes
                MatchCollection timeSections = Regex.Matches(vcdContent, @"#(\d+)");


                foreach (Match timeSection in timeSections)
                {
                    int time = int.Parse(timeSection.Groups[1].Value);
                    int startIndex = timeSection.Index + timeSection.Length;
                    int endIndex = (timeSection.NextMatch().Success) ? timeSection.NextMatch().Index : vcdContent.Length;
                    string betweenStrings = vcdContent.Substring(startIndex, endIndex - startIndex).Trim();
                    MatchCollection signalChanges = Regex.Matches(betweenStrings, @"\b(b?([01]+)) ?([^\d\s]+)\b");
                    foreach (Match match in signalChanges)
                    {
                        string binaryNumber = match.Groups[2].Value;
                        string symbol = match.Groups[3].Value;
                        if (string.IsNullOrEmpty(binaryNumber))
                        {
                            return Result.Failure<WaveformData>(new VcdParsingError("Error in parsing the value chnges on the signal "));
                        }
                        if (string.IsNullOrEmpty(symbol))
                        {
                            return Result.Failure<WaveformData>(new VcdParsingError("Error in parsing the symbol chnges on the signal "));
                        }

                        int parsedNumber = Convert.ToInt32(binaryNumber, 2);
                        var result = waveformData.AddValueToSignal(symbol, time, parsedNumber);
                        if (result.IsFailure)
                        {
                            return Result.Failure<WaveformData>(result.Error);
                        }
                    }
                }
               

                return Result.Success(waveformData);
            }
            catch (Exception ex)
            {
                return Result.Failure<WaveformData>(new VcdParsingError(ex.Message));
            }
        }
    }
}
