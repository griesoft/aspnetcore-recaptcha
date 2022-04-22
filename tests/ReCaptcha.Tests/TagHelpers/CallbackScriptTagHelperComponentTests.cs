using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Griesoft.AspNetCore.ReCaptcha.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using NUnit.Framework;

namespace ReCaptcha.Tests.TagHelpers
{
    [TestFixture]
    public class CallbackScriptTagHelperComponentTests
    {
        private TagHelperOutput _tagHelperOutput;

        [SetUp]
        public void Initialize()
        {
            _tagHelperOutput = new TagHelperOutput("body",
                new TagHelperAttributeList(), (useCachedResult, htmlEncoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    tagHelperContent.SetContent(string.Empty);
                    return Task.FromResult<TagHelperContent>(tagHelperContent);
                });
        }

        [Test]
        public void Construction_IsSuccessful()
        {
            // Arrange
            var formId = "formId";

            // Act
            var instance = new CallbackScriptTagHelperComponent(formId);

            // Assert
            Assert.NotNull(instance);
        }

        [Test]
        public void Constructor_ShouldThrow_WhenFormIdNull()
        {
            // Arrange


            // Act


            // Assert
            Assert.Throws<ArgumentNullException>(() => new CallbackScriptTagHelperComponent(null));
        }

        [Test]
        public void CallbackScript_ShouldReturn_ExpectedValue()
        {
            // Arrange
            var formId = "formId";
            var expectedResult = $"<script>function submit{formId}(token){{document.getElementById('{formId}').submit();}}</script>";

            // Act
            var result = CallbackScriptTagHelperComponent.CallbackScript(formId);

            // Assert
            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void Process_ShouldAppendScript_WhenTagIsBody()
        {
            // Arrange
            var formId = "formId";
            var comp = new CallbackScriptTagHelperComponent(formId);
            var context = new TagHelperContext("body", new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));

            // Act
            comp.Process(context, _tagHelperOutput);

            // Assert
            Assert.IsTrue(_tagHelperOutput.PostContent.GetContent().Contains(CallbackScriptTagHelperComponent.CallbackScript(formId)));
        }

        [Test]
        public void Process_ShouldSkip_WhenTagIsNotBody()
        {
            // Arrange
            var formId = "formId";
            var comp = new CallbackScriptTagHelperComponent(formId);
            var context = new TagHelperContext("head", new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));

            // Act
            comp.Process(context, _tagHelperOutput);

            // Assert
            Assert.IsTrue(_tagHelperOutput.PostContent.IsEmptyOrWhiteSpace);
        }
    }
}
