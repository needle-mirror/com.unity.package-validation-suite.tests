using System.IO;
using System.Runtime.CompilerServices;

namespace Utilities
{
    public class TestDataLocator
    {
        public static string GetCurrentFileDirectory([CallerFilePath] string sourceFilePath = "")
        {
            return Path.GetDirectoryName(sourceFilePath);
        }
    }
}
