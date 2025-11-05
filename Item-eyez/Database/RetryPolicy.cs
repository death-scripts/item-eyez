// ----------------------------------------------------------------------------
// <copyright company="death-scripts">
// Copyright (c) death-scripts. All rights reserved.
// </copyright>
//                   ██████╗ ███████╗ █████╗ ████████╗██╗  ██╗
//                   ██╔══██╗██╔════╝██╔══██╗╚══██╔══╝██║  ██║
//                   ██║  ██║█████╗  ███████║   ██║   ███████║
//                   ██║  ██║██╔══╝  ██╔══██║   ██║   ██╔══██║
//                   ██████╔╝███████╗██║  ██║   ██║   ██║  ██║
//                   ╚═════╝ ╚══════╝╚═╝  ╚═╝   ╚═╝   ╚═╝  ╚═╝
//
//              ███████╗ ██████╗██████╗ ██╗██████╗ ████████╗███████╗
//              ██╔════╝██╔════╝██╔══██╗██║██╔══██╗╚══██╔══╝██╔════╝
//              ███████╗██║     ██████╔╝██║██████╔╝   ██║   ███████╗
//              ╚════██║██║     ██╔══██╗██║██╔═══╝    ██║   ╚════██║
//              ███████║╚██████╗██║  ██║██║██║        ██║   ███████║
//              ╚══════╝ ╚═════╝╚═╝  ╚═╝╚═╝╚═╝        ╚═╝   ╚══════╝
// ----------------------------------------------------------------------------
namespace Item_eyez.Database
{
    /// <summary>
    /// The retry policy.
    /// </summary>
    internal static class RetryPolicy
    {
        /// <summary>
        /// Executes the specified action.
        /// </summary>
        /// <param name="action">The action.</param>
        /// <param name="maxAttempts">The maximum attempts.</param>
        /// <param name="initialDelayMs">The initial delay ms.</param>
        public static void Execute(Action action, int maxAttempts = 3, int initialDelayMs = 200)
            => Execute<object>(
            () =>
            {
                action();
                return null!;
            },
            maxAttempts,
            initialDelayMs);

        /// <summary>
        /// Executes the specified function.
        /// </summary>
        /// <typeparam name="T">The type.</typeparam>
        /// <param name="func">The function.</param>
        /// <param name="maxAttempts">The maximum attempts.</param>
        /// <param name="initialDelayMs">The initial delay ms.</param>
        /// <returns>
        /// The t.
        /// </returns>
        /// <exception cref="System.Exception">Unknown failure in RetryPolicy.</exception>
        public static T Execute<T>(Func<T> func, int maxAttempts = 3, int initialDelayMs = 200)
        {
            int delay = initialDelayMs;
            Exception? last = null;
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    return func();
                }
                catch (Exception ex) when (IsTransient(ex))
                {
                    last = ex;
                    if (attempt == maxAttempts)
                    {
                        break;
                    }

                    Thread.Sleep(delay);
                    delay = Math.Min(delay * 2, 2000);
                }
            }

            throw last ?? new Exception("Unknown failure in RetryPolicy");
        }

        /// <summary>
        /// Determines whether the specified ex is transient.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns>
        ///   <c>true</c> if the specified ex is transient; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsTransient(Exception ex) =>

            // Keep generic to avoid a hard dependency here; callers are SQL operations
            ex is TimeoutException || ex.GetType().Name.Contains("SqlException", StringComparison.OrdinalIgnoreCase);
    }
}