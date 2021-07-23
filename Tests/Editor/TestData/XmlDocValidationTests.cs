using NUnit.Framework;
using System.IO;
using UnityEditor.PackageManager.ValidationSuite.ValidationTests;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Compilation;
using Utilities;
using Debug = UnityEngine.Debug;

namespace UnityEditor.PackageManager.ValidationSuite.Tests
{
    internal class XmlDocValidationTests : BaseValidationTestSuite
    {

        private const string undocumentedClassDefinition = @"
                public class Foo
                {
                    public int member; 
                }";

        internal static string testPackageRoot;

        public XmlDocValidationTests()
        {
            testPackageRoot = Path.Combine(TestDataLocator.GetCurrentFileDirectory(), "XDAssemb");
        }

        [Test]
        public void FullyDocumentedFolderReturnsNoErrors()
        {
            var expectedMembersMissingDocs = new string[] {};

            File.WriteAllText(Path.Combine(this.testDirectory, "test.cs"), @"
                ///<summary>A Class</summary>
                public class Foo
                {
                    ///<summary>A Member</summary>
                    public int member; 
                }");
            var xmlDocValidation = ValidateTestDirectory();
            AssertSucceeded(xmlDocValidation);
        }
        [Test]
        public void UndocumentedFolderReturnsErrors()
        {
            var expectedMembersMissingDocs = new string[] {
                "Missing docs on Foo",
                "Missing docs on Foo.member"
            };

            File.WriteAllText(Path.Combine(this.testDirectory, "test.cs"), undocumentedClassDefinition);
            var xmlDocValidation = ValidateTestDirectory();
            AssertErrors(xmlDocValidation, expectedMembersMissingDocs);
        }
        [Test]
        public void UndocumentedTypeInPreviewReturnsWarnings()
        {
            var expectedMembersMissingDocs = new string[] {
                "Missing docs on Foo",
                "Missing docs on Foo.member"
            };

            File.WriteAllText(Path.Combine(this.testDirectory, "test.cs"), undocumentedClassDefinition);

            var xmlDocValidation = new XmlDocValidation()
            {
                Context = new VettingContext()
                {
                    ProjectPackageInfo = new VettingContext.ManifestData()
                    {
                        path = testDirectory,
                        version = "1.0.0-preview.1"
                    },
                    ValidationType = ValidationType.Publishing,
                }
            };
            xmlDocValidation.RunTest();

            Assert.AreEqual(TestState.Succeeded, xmlDocValidation.TestState);
            var errorMessage = "Warning: " + XmlDocValidation.FormatErrorMessage(expectedMembersMissingDocs);
            var actualMessage = xmlDocValidation.TestOutput.FirstOrDefault(o => o.StartsWith("Warning"));
            Assert.AreEqual(errorMessage, actualMessage);
        }

        [Test]
        [TestCase("Folder~")]
        [TestCase(".Folder")]
        [TestCase("Tests")]
        public void UndocumentedSourceInExcludedFolderSucceeds(string folderName)
        {
            var tildeDirectory = Path.Combine(this.testDirectory, folderName);
            Directory.CreateDirectory(tildeDirectory);
            File.WriteAllText(Path.Combine(tildeDirectory, "test.cs"), undocumentedClassDefinition);
            var xmlDocValidation = ValidateTestDirectory();
            AssertSucceeded(xmlDocValidation);
        }

        [Test]
        public void UndocumentedSourceInManyTildeFoldersSucceeds()
        {
            for (int i = 0; i < 500; i++)
            {
                var tildeDirectory = Path.Combine(this.testDirectory, i + "Folder~");
                Directory.CreateDirectory(tildeDirectory);
                File.WriteAllText(Path.Combine(tildeDirectory, "test.cs"), @"
                    public class Foo
                    {
                        public int member; 
                    }");
            }
            var xmlDocValidation = ValidateTestDirectory();
            AssertSucceeded(xmlDocValidation);
        }
        [Test]
        public void UndocumentedSourceInTestAsmdefSucceeds()
        {
            var xmlDocValidation = new XmlDocValidation()
            {
                Context = new VettingContext()
                {
                    ProjectPackageInfo = new VettingContext.ManifestData()
                    {
                        path = Path.Combine(testPackageRoot, "TestPackage")
                    },
                    ValidationType = ValidationType.Publishing
                }
            };
            xmlDocValidation.RunTest();
            AssertSucceeded(xmlDocValidation);
        }

        class PublicAsmdefNestedAssemblyInformation : ValidationAssemblyInformation
        {
            public override bool IsTestAssembly(AssemblyInfo assembly) => assembly.assembly.name != "Unity.PackageValidationSuite.EditorTests.NestedPublicAssembly";
        }

        [Test]
        public void UndocumentedSourceInPublicAsmdefNestedInTestAsmdefReturnsErrors()
        {
            var expectedMembersMissingDocs = new string[] {
                "Missing docs on PublicFoo",
                "Missing docs on PublicFoo.Bar"
            };
            var xmlDocValidation = new XmlDocValidation(new PublicAsmdefNestedAssemblyInformation())
            {
                Context = new VettingContext()
                {
                    ProjectPackageInfo = new VettingContext.ManifestData()
                    {
                        path = Path.Combine(testPackageRoot, "TestWithNestedPublic")
                    },
                    ValidationType = ValidationType.Publishing
                }
            };
            xmlDocValidation.RunTest();
            AssertErrors(xmlDocValidation, expectedMembersMissingDocs);
        }

        private XmlDocValidation ValidateTestDirectory()
        {
            var xmlDocValidation = new XmlDocValidation()
            {
                Context = new VettingContext()
                {
                    ProjectPackageInfo = new VettingContext.ManifestData()
                    {
                        path = testDirectory
                    },
                    ValidationType = ValidationType.Publishing
                }
            };
            xmlDocValidation.RunTest();
            return xmlDocValidation;
        }

        private static void AssertErrors(XmlDocValidation xmlDocValidation, string[] expectedMessages)
        {
            Assert.AreEqual(TestState.Succeeded, xmlDocValidation.TestState);  
            var errorMessage = "Warning: " + XmlDocValidation.FormatErrorMessage(expectedMessages);
            var actualMessage = xmlDocValidation.TestOutput.First(o => o.StartsWith("Warning"));
            Assert.AreEqual(errorMessage, actualMessage, $@"Unexpected output.
Expected:
{errorMessage}
Actual:
{actualMessage}");
        }

        private static void AssertSucceeded(XmlDocValidation xmlDocValidation)
        {
            Assert.AreEqual(TestState.Succeeded, xmlDocValidation.TestState,
                $"validation failed. Erorrs: {string.Join("\n", xmlDocValidation.TestOutput)}");
        }
    }
}
