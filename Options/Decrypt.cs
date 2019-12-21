// -------------------------------------------------------
// © Kaplas. Licensed under MIT. See LICENSE for details.
// -------------------------------------------------------
namespace GuJianMovieDecryptor.Options
{
    using CommandLine;

    /// <summary>
    /// GuJian BIK decrypt options.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "CA1812:Avoid uninstantiated internal classes", Justification = "Class is passed as type parameter.")]
    internal class Decrypt
    {
        /// <summary>
        /// Gets or sets the input file.
        /// </summary>
        [Value(0, MetaName = "input_file", Required = true, HelpText = "Encrypted file.")]
        public string InputFile { get; set; }

        /// <summary>
        /// Gets or sets the output file.
        /// </summary>
        [Value(1, MetaName = "output_file", Required = true, HelpText = "Output file.")]
        public string OutputFile { get; set; }
    }
}