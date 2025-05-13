
namespace VeriGenX.Domain.Repository
{
    public interface ICodeGenerationRepository
    {
        /// <summary>
        /// Generates text based on the input prompt
        /// </summary>
        /// <param name="prompt">The input text prompt</param>
        /// <returns>Generated text from the AI</returns>
        Task<string> GenerateTextAsync(string prompt);

        /// <summary>
        /// Generates text with additional parameters
        /// </summary>
        /// <param name="prompt">The input text prompt</param>
        /// <param name="maxTokens">Maximum number of tokens to generate</param>
        /// <param name="temperature">Controls randomness (lower = more deterministic)</param>
        /// <returns>Generated text from the AI</returns>
        Task<string> GenerateTextAsync(string prompt, int maxTokens, float temperature);
    }
}
