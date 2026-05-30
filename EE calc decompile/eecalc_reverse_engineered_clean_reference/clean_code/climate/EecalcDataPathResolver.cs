using System;
using System.IO;

namespace EE.Doklad.Services.EecalcClimate
{
    internal static class EecalcDataPathResolver
    {
        public static string FindRequiredFile(params string[] relativeSegments)
        {
            var start = new DirectoryInfo(AppContext.BaseDirectory);

            for (var directory = start; directory != null; directory = directory.Parent)
            {
                var candidate = Path.Combine(directory.FullName, Path.Combine(relativeSegments));
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            var cwd = new DirectoryInfo(Environment.CurrentDirectory);
            for (var directory = cwd; directory != null; directory = directory.Parent)
            {
                var candidate = Path.Combine(directory.FullName, Path.Combine(relativeSegments));
                if (File.Exists(candidate))
                {
                    return candidate;
                }
            }

            throw new FileNotFoundException(
                "Could not locate EECalc data file.",
                Path.Combine(relativeSegments));
        }
    }
}
