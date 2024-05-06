using NUnit.Framework;
using UnitTestProject.Models;
using Moq;
using UnitTest.Services;
using FluentAssertions;
using System;

namespace JobApplicationLibrary.UnitTest
{
    public class ApplicationEvaluateUnitTest
    {
        //UnitOfWork_Condition_ExpectedResult
        [Test]
        public void Application_WithUnderAge_TransferredToAutoRejected()
        {
            //Arrange
            var evaluator = new ApplicationEvaluator(null);
            var form = new JobApplication()
            {
                Applicant = new Applicant()
                {
                    Age = 17
                }
            };

            //Action
            var appResult = evaluator.Evaluate(form);
            //Assert
           
            appResult.Should().Be(ApplicationResult.AutoRejected);
        }
        [Test]
        public void Application_WithNoTechStack_TransferredToAutoRejected()
        {
            //Arrange
            //týpký böyle bir interface varmýþ da burdaki nesne aracýlýðýyla bu interface e ve o interface içerisindeki methodlara ulaþýyormuþ gibi yapýyor.
            //defaultta mock un behaivoru Loosedur.

            var mockValidator = new Mock<IIdentityValidator>(MockBehavior.Strict);
            //içerisine string bir param gönderilen valid methodu çaðýrýldýðýnda true dön...
            //mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(true);
            //object propertysi IidentityValidator nesnesi veriyor  
            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant() { Age = 19 },
                TechStackList = new System.Collections.Generic.List<string>() { "" }
            };

            //Action
            var appResult = evaluator.Evaluate(form);
            //Assert
            appResult.Should().Be(ApplicationResult.AutoRejected);
        }
        [Test]
        public void Application_WithTechStackOver75P_TransferredToAutoAccepted()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(true);

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant() { Age = 24 },
                TechStackList = new System.Collections.Generic.List<string>() { "C#", "Redis", "Unit Test", "MongoDb" },
                YearsofExperience = 17
            };

            //Action
            var appResult = evaluator.Evaluate(form);
            //Assert
            appResult.Should().Be(ApplicationResult.AutoAccepted);
        }
        [Test]
        public void Application_WithInValidIdentityNumber_TransferredToHR()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.Setup(i => i.IsValid(It.IsAny<string>())).Returns(false);

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant() { Age = 24 },

            };

            //Action
            var appResult = evaluator.Evaluate(form);
            //Assert
            appResult.Should().Be(ApplicationResult.TransferredToHR);
        }
        [Test]
        public void Application_WithOfficeLocation_TransferredToCTO()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.Setup(i => i.Country).Returns("SPAIN");

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant() { Age = 24 },


            };

            //Action
            var appResult = evaluator.Evaluate(form);
            //Assert
            appResult.Should().Be(ApplicationResult.TransferredToCTO);
        }
        [Test]
        public void Application_WithOver50_ValidationModeToDetailed()
        {
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.SetupProperty(i => i.ValidationMode);

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant() { Age = 51 }
            };
            var appResult = evaluator.Evaluate(form);
            //Assert.AreEqual(ValidationMode.Detailed, mockValidator.Object.ValidationMode);

            mockValidator.Object.ValidationMode.Should().Be(ValidationMode.Detailed);
        }
        [Test]
        public void Application_WithNullApplicant_ThrowsArgumentNullException()
        {
            //Arrange
            var mockValidator = new Mock<IIdentityValidator>();
            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication();
            //Action
            Action appResult = () => evaluator.Evaluate(form);
            //Assert
            appResult.Should().Throw<ArgumentNullException>();
        }
        [Test]
        public void Application_WithDefaultValue_IsValidCalled()
        {
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;
            mockValidator.Setup(i => i.Country).Returns("TURKEY");
            //mockValidator.Setup(i=>i.IsValid(It.IsAny<string>())).Returns(true);
            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant() { Age = 19 },
                IdentityNumber = "123"
            };

            //Action
            var appResult = evaluator.Evaluate(form);
            //Assert
            mockValidator.Verify(i=>i.IsValid(It.IsAny<string>()),Times.Exactly(1),"IsValidMethod should be called with 123");
        }
        [Test]
        public void Application_WithYoungAge_IsValidNeverCalled()
        {
            var mockValidator = new Mock<IIdentityValidator>();
            mockValidator.DefaultValue = DefaultValue.Mock;

            mockValidator.Setup(i => i.Country).Returns("TURKEY");

            var evaluator = new ApplicationEvaluator(mockValidator.Object);
            var form = new JobApplication()
            {
                Applicant = new Applicant() { Age = 15 },

            };

            //Action
            var appResult = evaluator.Evaluate(form);
            //Assert
            mockValidator.Verify(i => i.IsValid(It.IsAny<string>()),Times.Exactly(0));
        }
    }
}