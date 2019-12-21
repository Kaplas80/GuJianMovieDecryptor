// -------------------------------------------------------
// © Kaplas. Licensed under MIT. See LICENSE for details.
// -------------------------------------------------------
namespace GuJianMovieDecryptor
{
    using System;
    using System.IO;
    using CommandLine;
    using Yarhl.IO;

    /// <summary>
    /// Main program.
    /// </summary>
    internal static partial class Program
    {
        private static readonly byte[] EncryptionKey =
        {
            0x08, 0xF3, 0x89, 0xE6, 0x57, 0x2B, 0xA9, 0xCC,
            0xCC, 0xE5, 0xEA, 0xD2, 0xD2, 0xD2, 0x44, 0x82,
            0xDC, 0x42, 0x06, 0xD5, 0x54, 0xE5, 0x0B, 0x86,
            0x23, 0xBB, 0x15, 0xE7, 0xDF, 0xF4, 0x4C, 0xC4,
            0x7B, 0x91, 0xD2, 0xC6, 0xC5, 0xF0, 0x22, 0x00,
            0x48, 0xA7, 0x6F, 0xD9, 0x88, 0x00, 0x00, 0x00,
            0x7B, 0x91, 0xD2, 0xC6, 0xC5, 0xF0, 0x22, 0x12,
            0x15, 0x37, 0xDD, 0x10, 0x8E, 0x58, 0x74, 0x22,
            0x9A, 0x08, 0x4E, 0xFF, 0x93, 0x32, 0xC5, 0x16,
            0x99, 0x14, 0x34, 0xE8, 0xA6, 0xA2, 0xA4, 0x4E,
            0xE3, 0x41, 0x14, 0xCA, 0x1B, 0x3F, 0xA8, 0xDE,
            0xBE, 0xE6, 0x33, 0xCB, 0x78, 0x75, 0x2D, 0xF0,
            0xBF, 0x7D, 0xD5, 0x29, 0xA7, 0x3E, 0xE0, 0x90,
            0xD3, 0x41, 0x46, 0x51, 0x7D, 0x9D, 0x31, 0x80,
            0xC2, 0x60, 0x56, 0xC0, 0xB9, 0x27, 0x65, 0x80,
            0x1B, 0x5E, 0x7E, 0xFD, 0x3D, 0x4B, 0xD6, 0x00,
            0x1B, 0x5E, 0x7E, 0xFD, 0x3D, 0x4B, 0xD0, 0x82,
            0x67, 0xB5, 0x91, 0xDF, 0x50, 0xF9, 0xF4, 0xD7,
            0xA1, 0x07, 0x5C, 0xCF, 0x88, 0x48, 0x2B, 0xB1,
            0x15, 0x37, 0xDD, 0x10, 0x8E, 0x58, 0x74, 0x22,
            0xDC, 0x42, 0x06, 0xD5, 0x54, 0xE5, 0x0B, 0x86,
            0x23, 0xBB, 0x91, 0xD2, 0xC6, 0xC5, 0xF0, 0xCC,
            0x93, 0x32, 0xC5, 0x16, 0x48, 0x2B, 0xB1, 0x15,
            0xCE, 0xF3, 0x89, 0xE6, 0x57, 0x2B, 0xA9, 0xCC,
            0x23, 0x71, 0x96, 0xE4, 0x73, 0x2B, 0xC4, 0xCC,
            0xCC, 0xE5, 0x67, 0xD2, 0xD2, 0xD2, 0x44, 0x82,
            0xE3, 0xA7, 0x78, 0xAA, 0x40, 0xF1, 0x58, 0x00,
            0xA1, 0x7F, 0xB8, 0x04, 0x26, 0x90, 0x00, 0x0B,
            0x25, 0x48, 0xD7, 0x1C, 0x5F, 0x47, 0x3F, 0x1A,
            0x29, 0xC0, 0x07, 0x66, 0x27, 0x85, 0x79, 0x14,
            0xF2, 0xE8, 0xE9, 0x85, 0x3A, 0x54, 0x2E, 0x93,
            0x52, 0xB5, 0x29, 0x6A, 0x56, 0xB4, 0x54, 0xA7,
        };

        private static void Main(string[] args)
        {
            Parser.Default.ParseArguments<Options.Decrypt>(args).WithParsed<Options.Decrypt>(Decrypt);
        }

        private static void WriteHeader()
        {
            Console.WriteLine(CommandLine.Text.HeadingInfo.Default);
            Console.WriteLine(CommandLine.Text.CopyrightInfo.Default);
            Console.WriteLine();
        }

        private static void Decrypt(Options.Decrypt opts)
        {
            WriteHeader();

            if (!File.Exists(opts.InputFile))
            {
                Console.WriteLine($"ERROR: \"{opts.InputFile}\" not found!!!!");
                return;
            }

            if (File.Exists(opts.OutputFile))
            {
                Console.WriteLine($"WARNING: \"{opts.OutputFile}\" already exists. It will be overwritten.");
                Console.Write("Continue? (y/N) ");
                string answer = Console.ReadLine();
                if (!string.IsNullOrEmpty(answer) && answer.ToUpperInvariant() != "Y")
                {
                    Console.WriteLine("CANCELLED BY USER.");
                    return;
                }
            }

            // Encryption uses 2 keys:
            // 1. First bytes in file, XORed with input file name without extension in upper case
            // 2. Fixed byte array.
            string fileWithoutExtension = Path.GetFileNameWithoutExtension(opts.InputFile).ToUpperInvariant();

            using DataStream input = DataStreamFactory.FromFile(opts.InputFile, FileOpenMode.Read);
            using DataStream output = DataStreamFactory.FromMemory();

            Console.Write($"Decrypting '{opts.InputFile}'...");
            Decrypt(input, output, fileWithoutExtension);
            output.WriteTo(opts.OutputFile);
            Console.WriteLine(" DONE!");
        }

        private static void Decrypt(DataStream input, DataStream output, string fileName)
        {
            input.Position = 0;
            output.Position = 0;

            var key = new byte[0x200];
            input.Read(key, 0, 0x200);

            int keyLength = key[0];

            for (int i = 0; i < keyLength; i++)
            {
                key[i] = (byte)(key[i] ^ fileName[i % fileName.Length]);
            }

            int keyIndex = key[37] % keyLength;

            input.Position = keyLength;

            var buffer = new byte[0x4000];
            while (!input.EndOfStream)
            {
                int readCount = (int)Math.Min(0x4000, input.Length - input.Position);
                int read = input.Read(buffer, 0, readCount);
                for (int i = 0; i < read; i++)
                {
                    buffer[i] = (byte)(buffer[i] ^ EncryptionKey[keyIndex] ^ key[keyIndex]);
                    keyIndex = (keyIndex + 1) % keyLength;
                }

                output.Write(buffer, 0, read);
            }
        }
    }
}