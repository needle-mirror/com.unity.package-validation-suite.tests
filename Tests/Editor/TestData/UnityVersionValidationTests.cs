using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using NUnit.Framework;
using UnityEditor.PackageManager.ValidationSuite.ValidationTests;
using UnityEngine;

namespace UnityEditor.PackageManager.ValidationSuite.Tests
{
    internal class UnityVersionValidationTests
    {
        private string testDirectory = Path.Combine(Path.GetTempPath(), "tempUnityVersionValidationTests");

        [SetUp]
        public void Setup()
        {
            if (!Directory.Exists(testDirectory))
            {
                Directory.CreateDirectory(testDirectory);
            }
        }

        [TearDown]
        public void TearDown()
        {
            if (Directory.Exists(testDirectory))
            {
                Directory.Delete(testDirectory, true);
            }
        }

        [Test]
        public void When_Package_UnityVersion_Higher_Validation_Fails()
        {
            var unityVersion = UnityEngine.Application.unityVersion;
            var unityVersionArr = unityVersion.Split('.');
            var higherUnityVersion = $"{Int32.Parse(unityVersionArr[0])+1}.{unityVersionArr[1]}";
            
            var projectPackageInfo = new VettingContext.ManifestData
            {
                unity = higherUnityVersion
            };

            var unityVersionValidation = new UnityVersionValidation();
            var vettingContext = new VettingContext
            {
                ProjectPackageInfo = projectPackageInfo,
                ValidationType = ValidationType.LocalDevelopment
            };
            unityVersionValidation.Context = vettingContext;
            unityVersionValidation.Setup();
            unityVersionValidation.RunTest();

            Assert.AreEqual(TestState.Failed, unityVersionValidation.TestState);
            Assert.Greater(unityVersionValidation.TestOutput.Count, 0);
        }
        
        [Test]
        public void When_Package_UnityVersion_Lower_Validation_Succeeds()
        {
            var unityVersion = UnityEngine.Application.unityVersion;
            var unityVersionArr = unityVersion.Split('.');
            var lowerUnityVersion = $"{Int32.Parse(unityVersionArr[0])-1}.{unityVersionArr[1]}";
            
            var projectPackageInfo = new VettingContext.ManifestData
            {
                unity = lowerUnityVersion
            };

            var unityVersionValidation = new UnityVersionValidation();
            var vettingContext = new VettingContext
            {
                ProjectPackageInfo = projectPackageInfo,
                ValidationType = ValidationType.LocalDevelopment
            };
            unityVersionValidation.Context = vettingContext;
            unityVersionValidation.Setup();
            unityVersionValidation.RunTest();

            Assert.AreEqual(TestState.Succeeded, unityVersionValidation.TestState);
        }
        
        [Test]
        public void When_Package_UnityVersion_Same_Validation_Succeeds()
        {
            var unityVersion = UnityEngine.Application.unityVersion;
            var unityVersionArr = unityVersion.Split('.');
            var sameUnityVersion = $"{unityVersionArr[0]}.{unityVersionArr[1]}";
            
            var projectPackageInfo = new VettingContext.ManifestData
            {
                unity = sameUnityVersion
            };

            var unityVersionValidation = new UnityVersionValidation();
            var vettingContext = new VettingContext
            {
                ProjectPackageInfo = projectPackageInfo,
                ValidationType = ValidationType.LocalDevelopment
            };
            unityVersionValidation.Context = vettingContext;
            unityVersionValidation.Setup();
            unityVersionValidation.RunTest();

            Assert.AreEqual(TestState.Succeeded, unityVersionValidation.TestState);
        }
    }
}