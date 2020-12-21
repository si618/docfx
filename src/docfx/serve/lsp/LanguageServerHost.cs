// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using System.IO.Pipelines;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OmniSharp.Extensions.LanguageServer.Server;

namespace Microsoft.Docs.Build
{
    internal class LanguageServerHost
    {
        public static Task<LanguageServer> StartLanguageServer(string workingDirectory, CommandLineOptions options, Package? package = null)
        {
            var stdIn = Console.OpenStandardInput();
            var stdOut = Console.OpenStandardOutput();
            ResetConsoleOutput();

            return StartLanguageServer(workingDirectory, options, PipeReader.Create(stdIn), PipeWriter.Create(stdOut), package);
        }

        public static Task<LanguageServer> StartLanguageServer(
            string workingDirectory,
            CommandLineOptions commandLineOptions,
            PipeReader input,
            PipeWriter output,
            Package? package = null,
            ILanguageServerNotificationListener? notificationListener = null)
        {
            return LanguageServer.From(options => options
                .WithInput(input)
                .WithOutput(output)
                .ConfigureLogging(x => x.AddLanguageProtocolLogging())
                .WithHandler<TextDocumentHandler>()
                .WithServices(services => services
                    .AddSingleton(notificationListener ?? new LanguageServerNotificationListener())
                    .AddSingleton(new LanguageServerPackage(new MemoryPackage(workingDirectory), package ?? new LocalPackage(workingDirectory)))
                    .AddSingleton(commandLineOptions)
                    .AddSingleton<DiagnosticPublisher>()
                    .AddSingleton<LanguageServerBuilder>()
                    .AddOptions()
                    .AddLogging())
                .WithServices(x => x.AddLogging(b => b.SetMinimumLevel(LogLevel.Trace))));
        }

        private static void ResetConsoleOutput()
        {
            // TODO: redirect the console output to client through LSP
            Console.SetOut(StreamWriter.Null);
        }
    }
}
