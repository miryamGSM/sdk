// Copyright (c) .NET Foundation and contributors. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Build.Framework;
using System;
using System.Globalization;

namespace Microsoft.NET.Build.Tasks
{
    /// <summary>
    /// Provides a localizable mechanism for logging an messages of different kids from the SDK targets.
    /// </summary>
    public class
#if EXTENSIONS
        // This task source is shared with multiple task Dlls.  Since both tasks
        // may be loaded into the same project and each task accesses only resources
        // in its own assembly they must have a unique name so-as not to clash.
        NETBuildExtensionsMessage
#else
        NETSdkMessage
#endif
     : TaskBase
    {
        /// <summary>
        /// The name of the resource in Strings.resx that contains the desired error message.
        /// </summary>
        [Required]
        public string ResourceName { get; set; }

        /// <summary>
        /// The arguments provided to <see cref="string.Format"/> along with the retrieved resource as the format.
        /// </summary>
        public string[] FormatArguments { get; set; }

        public string Severity { get; set; } = "Info";

        public string Importance { get; set; } = "Normal";

        private static readonly string[] EmptyArguments = new[] { "" };

        protected override void ExecuteCore()
        {
            if (FormatArguments == null || FormatArguments.Length == 0)
            {
                // We use a single-item array with one empty string in this case so that
                // it is possible to interpret FormatArguments="$(EmptyVariable)" as a request
                // to pass an empty string on to string.Format. Note if there are not placeholders
                // in the string, then the empty string arg will be ignored.
                FormatArguments = EmptyArguments;
            }

            DiagnosticMessageSeverity severity =
                (DiagnosticMessageSeverity)Enum.Parse(typeof(DiagnosticMessageSeverity), Severity, true);
            MessageImportance importance = (MessageImportance)Enum.Parse(typeof(MessageImportance), Importance, true);

            string format = Strings.ResourceManager.GetString(ResourceName, Strings.Culture);
            string message = string.Format(CultureInfo.CurrentCulture, format, FormatArguments);

            switch (severity)
            {
                case DiagnosticMessageSeverity.Error:
                    Log.LogError(message);
                    break;
                case DiagnosticMessageSeverity.Warning:
                    Log.LogWarning(message);
                    break;
                case DiagnosticMessageSeverity.Info:
                    Log.LogMessage(importance, message);
                    break;
            }
        }
    }
}
