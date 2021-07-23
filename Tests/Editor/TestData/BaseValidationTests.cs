using NUnit.Framework;
using System.IO;
using UnityEditor.PackageManager.ValidationSuite.ValidationTests;
using System.Collections.Generic;
using System.Linq;
#if UNITY_2018_1_OR_NEWER
using UnityEditor.Compilation;
using Debug = UnityEngine.Debug;

namespace UnityEditor.PackageManager.ValidationSuite.Tests
{
    [TestFixture]
    internal class BaseValidationTests
    {
        private static string TestMessage = "hello{}";

        class TestValidation : BaseValidation
        {
            protected override void Run() => throw new System.NotImplementedException();

            public void Test_Error(string message) => base.Error(message);
            public void Test_Warning(string message) => base.Warning(message);
            public void Test_Information(string message) => base.Information(message);
        }
        [Test]
        public void ErrorWithCurlyBracesDoesNotThrow()
        {
            var testValidation = new TestValidation();
            testValidation.Test_Error(TestMessage);
            testValidation.TestOutput.Contains(TestMessage);
        }

        [Test]
        public void WarningWithCurlyBracesDoesNotThrow()
        {
            var testValidation = new TestValidation();
            testValidation.Test_Warning(TestMessage);
            testValidation.TestOutput.Contains(TestMessage);
        }

        [Test]
        public void InformationWithCurlyBracesDoesNotThrow()
        {
            var testValidation = new TestValidation();
            testValidation.Test_Information(TestMessage);
            testValidation.TestOutput.Contains(TestMessage);
        }
    }
}
#endif
