using NUnit.Framework;
using System.IO;
using UnityEditor.PackageManager.ValidationSuite.ValidationTests;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Utilities;

namespace UnityEditor.PackageManager.ValidationSuite.Tests
{
    internal class TestsValidationTests
    {
        static string testPackageRoot;

        static TestsValidationTests()
        {
            testPackageRoot = Path.Combine(TestDataLocator.GetCurrentFileDirectory(), "TestsValidationData");
        }

        [Test]
        public void FolderThatEndsWithTests_WithTests_Passes()
        {
            var validation = Validate("FolderThatEndsWithTests");
            ExpectSuccess(validation);
        }
        
        [Test]
        public void TestsFolder_WithTests_Passes()
        {
            var validation = Validate("TestsFolder");
            ExpectSuccess(validation);
        }

        [Test]
        public void NoTestsFolder_Fails()
        {
            List<string> messagesExpected = new List<string> { "Error: All Packages must include tests for automated testing.  No tests were found for this package." };
            var validation = Validate("NoTestsFolder");
            ExpectFailure(validation, messagesExpected);
        }

        static BaseValidation Validate(string testName)
        {
            var projectPackagePath = Path.GetFullPath(Path.Combine(testPackageRoot, testName));

            var testsValidation = new TestsValidation()
            {
                Context = new VettingContext()
                {
                    PublishPackageInfo = new VettingContext.ManifestData
                    {
                        path = projectPackagePath
                    },
                    relatedPackages = new List<RelatedPackage>()
                }
            };
            testsValidation.Setup();
            testsValidation.RunTest();
            return testsValidation;
        }

        private static void ExpectFailure(BaseValidation apiValidation, List<string> messagesExpected)
        {
            Assert.AreEqual(TestState.Failed, apiValidation.TestState);
            Assert.That(apiValidation.TestOutput.Where(o => o.StartsWith("Error")), Is.EquivalentTo(messagesExpected));
        }
        
        private static void ExpectSuccess(BaseValidation apiValidation)
        {
            Assert.AreEqual(TestState.Succeeded, apiValidation.TestState);
        }

        private static IEnumerable<string> GetAssemblyNames(string previousPackagePath)
        {
            foreach (var asmdefPath in Directory.GetFiles(previousPackagePath, "*.asmdef", SearchOption.AllDirectories))
            {
                var asmdef = Utilities.GetDataFromJson<AssemblyDefinition>(asmdefPath);
                yield return asmdef.name;
            }
        }
    }
}