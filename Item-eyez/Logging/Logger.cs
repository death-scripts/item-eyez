// ----------------------------------------------------------------------------
// <copyright company="death-scripts">
// Copyright (c) death-scripts. All rights reserved.
// </copyright>
// ----------------------------------------------------------------------------
using System;
using System.IO;

namespace Item_eyez.Logging
{
    /// <summary>
    /// Simple file-based logger for application diagnostics.
    /// </summary>
    internal static class Logger
    {
        private static readonly object SyncRoot = new();

        /// <summary>
        /// Logs an exception with contextual information.
        /// </summary>
        /// <param name="context">The context in which the exception occurred.</param>
        /// <param name="exception">The exception.</param>
        public static void LogException(string context, Exception exception)
        {
            if (exception == null)
            {
                return;
            }

            Log("ERROR", $"{context}: {exception}");
        }

        /// <summary>
        /// Logs a message with the specified severity level.
        /// </summary>
        /// <param name="level">The severity level.</param>
        /// <param name="message">The message.</param>
        public static void Log(string level, string message)
        {
            try
            {
                string baseFolder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                string logFolder = Path.Combine(baseFolder, "ItemEyez", "logs");
                Directory.CreateDirectory(logFolder);

                string logFilePath = Path.Combine(logFolder, "item-eyez.log");
                string timestamp = DateTime.Now.ToString("O");
                string line = $"{timestamp} [{level}] {message}{Environment.NewLine}";

                lock (SyncRoot)
                {
                    File.AppendAllText(logFilePath, line);
                }
            }
            catch
            {
                // Swallow all logging failures to avoid impacting the app.
            }
        }
    }
}

