namespace Thd;

public static class VerbosityExtensions
{
    extension(Verbosity verbosity)
    {
        /// <summary>
        /// Checks if the verbosity is at least Detailed or Diagnostic
        /// </summary>
        /// <returns>true if the verbosity is detailed</returns>
        public bool IsAtLeastDetailed() => verbosity.IsAtLeast(Verbosity.Detailed);

        private bool IsAtLeast(Verbosity level)
        {
            return verbosity >= level;
        }
    }
}