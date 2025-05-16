

using System.Text.RegularExpressions;

namespace VeriGenX.SimulationTool.Parsers
{
    public static  class OutputParser
    {
        public static List<string> Parse(string output)
        {
            string normalizedOutput = output.Replace("\\\n", "\n").Replace("\\n", "\n");
            return normalizedOutput.Split(new[] { '\n' }, StringSplitOptions.RemoveEmptyEntries).ToList();

        }
        public static (HashSet<int> codeLineError, HashSet<int> testLineError) GetErrorLines(List<string> lines)
        {
            HashSet<int> codeLineError = new HashSet<int>();
            HashSet<int> testLineError = new HashSet<int>();
            Regex CodeRegex = new Regex(@"\s*main.v:\s*(\d+)\s*:");
            Regex testRegex = new Regex(@"\s*test.v:\s*(\d+)\s*:");
            foreach (string line in lines)
            {
                var CodeMatches = CodeRegex.Matches(line);
                foreach (Match match in CodeMatches)
                {
                    if (match.Groups.Count > 1 && int.TryParse(match.Groups[1].Value, out int number))
                    {
                        codeLineError.Add(number);
                    }
                }
                var testMatches = testRegex.Matches(line);
                foreach (Match match in testMatches)
                {
                    if (match.Groups.Count > 1 && int.TryParse(match.Groups[1].Value, out int number))
                    {
                        testLineError.Add(number);
                    }
                }

            }

            return (codeLineError, testLineError);
        }
    }
}
