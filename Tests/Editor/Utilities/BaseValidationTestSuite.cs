using System.IO;
using NUnit.Framework;

namespace UnityEditor.PackageManager.ValidationSuite.Tests
{
    internal class BaseValidationTestSuite
    {
        
        protected string testDirectory => "temp" + GetType().Name;

        [SetUp]
        public virtual void Setup()
        {
            if (Directory.Exists(testDirectory))
            {
                Directory.Delete(testDirectory, true);
            }

            Directory.CreateDirectory(testDirectory);
        }

        [TearDown]
        public virtual void TearDown()
        {
            if (Directory.Exists(testDirectory))
            {
                Directory.Delete(testDirectory, true);
            }
        }

        protected void CreateFileOrFolder(bool folder, string name, bool withMeta, string dir = "")
        {
            var toCreatePath = Path.Combine(testDirectory, Path.Combine(dir, name));
            if (folder)
                Directory.CreateDirectory(toCreatePath);
            else
                File.Create(toCreatePath).Dispose();

            if (withMeta)
                File.Create(toCreatePath + ".meta").Dispose();
        }
    }
}