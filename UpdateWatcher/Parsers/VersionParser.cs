using System.Text.RegularExpressions;

namespace Alisha.UpdateWatcher.Parserss
{
    class VersionParser
    {
        private static readonly Regex _versionRegex = new Regex(@"(\d+(?:\.\d+)+)");

        public static string Parse(string path) => _versionRegex.Match(path)?.Value ?? "";
    }
}
