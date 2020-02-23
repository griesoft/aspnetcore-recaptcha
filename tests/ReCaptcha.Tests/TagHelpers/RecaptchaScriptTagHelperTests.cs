using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GSoftware.AspNetCore.ReCaptcha.Configuration;
using GSoftware.AspNetCore.ReCaptcha.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace ReCaptcha.Tests.TagHelpers
{
    [TestFixture]
    public class RecaptchaScriptTagHelperTests
    {
        private const string SiteKey = "unit_test_site_key";

        private Mock<IOptionsMonitor<RecaptchaSettings>> _settingsMock;
        private TagHelperOutput _tagHelperOutput;
        private TagHelperContext _context;

        [SetUp]
        public void Initialize()
        {
            _settingsMock = new Mock<IOptionsMonitor<RecaptchaSettings>>();
            _settingsMock.SetupGet(instance => instance.CurrentValue)
                .Returns(new RecaptchaSettings()
                {
                    SiteKey = SiteKey,
                    SecretKey = string.Empty
                })
                .Verifiable();

            _tagHelperOutput = new TagHelperOutput("recaptcha-script",
                new TagHelperAttributeList(), (useCachedResult, htmlEncoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    tagHelperContent.SetContent(string.Empty);
                    return Task.FromResult<TagHelperContent>(tagHelperContent);
                });

            _context = new TagHelperContext(new TagHelperAttributeList(), 
                new Dictionary<object, object>(), 
                Guid.NewGuid().ToString("N"));
        }

        [Test]
        public void Construction_IsSuccessful()
        {
            // Arrange


            // Act
            var instance = new RecaptchaScriptTagHelper(_settingsMock.Object);

            // Assert
            Assert.NotNull(instance);
            _settingsMock.Verify(settings => settings.CurrentValue, Times.Once);
        }

        [Test]
        public void Constructor_ShouldThrow_WhenSettingsNull()
        {
            // Arrange


            // Act


            // Assert
            Assert.Throws<ArgumentNullException>(() => new RecaptchaScriptTagHelper(null));
        }

        [Test]
        public void Process_ShouldChangeTagTo_ScriptTag()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaScriptTagHelper(_settingsMock.Object);

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);

            // Assert
            Assert.AreEqual("script", _tagHelperOutput.TagName);
        }

        [Test]
        public void Process_ShouldChange_TagModeTo_StartTagAndEndTag()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaScriptTagHelper(_settingsMock.Object);
            _tagHelperOutput.TagMode = TagMode.SelfClosing;

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);

            // Assert
            Assert.AreEqual(TagMode.StartTagAndEndTag, _tagHelperOutput.TagMode);
        }

        [Test]
        public void Process_ShouldContain_ThreeAttributes()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaScriptTagHelper(_settingsMock.Object);

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);

            // Assert
            Assert.AreEqual(3, _tagHelperOutput.Attributes.Count);
            Assert.IsTrue(_tagHelperOutput.Attributes.ContainsName("src"));
            Assert.IsTrue(_tagHelperOutput.Attributes.ContainsName("async"));
            Assert.IsTrue(_tagHelperOutput.Attributes.ContainsName("defer"));
        }

        [Test]
        public void Process_ShouldNotContain_Content()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaScriptTagHelper(_settingsMock.Object);
            _tagHelperOutput.Content.SetContent("<p>Inner HTML<p>");

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);

            // Assert
            Assert.IsTrue(_tagHelperOutput.Content.IsEmptyOrWhiteSpace);
        }

        [Test]
        public void Process_ShouldNotAdd_CallbackAndLanguageToQuery_WhenRenderV3()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaScriptTagHelper(_settingsMock.Object)
            {
                Render = Render.V3,
                OnloadCallback = "myCallback",
                Language = "fi"
            };

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);
            var query = QueryHelpers.ParseQuery(new Uri(_tagHelperOutput.Attributes["src"].Value.ToString()).Query);

            // Assert
            Assert.AreEqual(1, query.Count);
            Assert.IsTrue(query.ContainsKey("render"));
        }

        [Test]
        public void Process_ShouldAddSiteKey_ToQueryRenderKey_WhenRenderV3()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaScriptTagHelper(_settingsMock.Object)
            {
                Render = Render.V3
            };

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);
            var query = QueryHelpers.ParseQuery(new Uri(_tagHelperOutput.Attributes["src"].Value.ToString()).Query);

            // Assert
            _settingsMock.Verify();
            Assert.AreEqual(SiteKey, query["render"]);
        }

        [Test]
        public void Process_ShouldNotAdd_RenderQueryKey_WhenRenderIsDefaultOnload()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaScriptTagHelper(_settingsMock.Object)
            {
                OnloadCallback = "myCallback"
            };

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);
            var query = QueryHelpers.ParseQuery(new Uri(_tagHelperOutput.Attributes["src"].Value.ToString()).Query);

            // Assert
            Assert.IsFalse(query.ContainsKey("render"));
        }

        [Test]
        public void Process_ShouldAddExplicit_ToQueryRenderKey()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaScriptTagHelper(_settingsMock.Object)
            {
                Render = Render.Explicit
            };

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);
            var query = QueryHelpers.ParseQuery(new Uri(_tagHelperOutput.Attributes["src"].Value.ToString()).Query);

            // Assert
            Assert.AreEqual("explicit", query["render"]);
        }

        [Test]
        public void Process_ShouldAdd_CallbackName_ToQuery()
        {
            // Arrange
            var callback = "myCallbackName";

            var scriptTagHelper = new RecaptchaScriptTagHelper(_settingsMock.Object)
            {
                OnloadCallback = callback
            };
            
            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);
            var query = QueryHelpers.ParseQuery(new Uri(_tagHelperOutput.Attributes["src"].Value.ToString()).Query);

            // Assert
            Assert.IsTrue(query.ContainsKey("onload"));
            Assert.AreEqual(callback, query["onload"]);
        }

        [Test]
        public void Process_ShouldAdd_Language_ToQuery()
        {
            // Arrange
            var language = "fi";

            var scriptTagHelper = new RecaptchaScriptTagHelper(_settingsMock.Object)
            {
                Language = language
            };

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);
            var query = QueryHelpers.ParseQuery(new Uri(_tagHelperOutput.Attributes["src"].Value.ToString()).Query);

            // Assert
            Assert.IsTrue(query.ContainsKey("hl"));
            Assert.AreEqual(language, query["hl"]);
        }

        [Test]
        public void Process_ShouldContain_AllQueryKeys()
        {
            // Arrange
            var callback = "myCallbackName";
            var language = "fi";

            var scriptTagHelper = new RecaptchaScriptTagHelper(_settingsMock.Object)
            {
                Render = Render.Explicit,
                OnloadCallback = callback,
                Language = language
            };

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);
            var query = QueryHelpers.ParseQuery(new Uri(_tagHelperOutput.Attributes["src"].Value.ToString()).Query);

            // Assert
            Assert.AreEqual(3, query.Count);
            Assert.IsTrue(query.ContainsKey("render"));
            Assert.IsTrue(query.ContainsKey("onload"));
            Assert.IsTrue(query.ContainsKey("hl"));
            Assert.AreEqual("explicit", query["render"]);
            Assert.AreEqual(callback, query["onload"]);
            Assert.AreEqual(language, query["hl"]);
        }

        [Test]
        public void Process_SrcAttribute_ExistsOnlyOnce_AndHasUrlValue()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaScriptTagHelper(_settingsMock.Object);
            _tagHelperOutput.Attributes.Add("src", "test");
            _tagHelperOutput.Attributes.Add("src", "othertest");

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);

            // Assert
            Assert.AreEqual(1, _tagHelperOutput.Attributes.Count(attribut => attribut.Name == "src"));
            Assert.IsTrue(_tagHelperOutput.Attributes["src"].Value.ToString().StartsWith("https://"));
        }

        [Test]
        public void Process_AsyncAttribute_ExistsOnlyOnce_AndHasEmptyValue()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaScriptTagHelper(_settingsMock.Object);
            _tagHelperOutput.Attributes.Add("async", "test");
            _tagHelperOutput.Attributes.Add("async", "othertest");

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);

            // Assert
            Assert.AreEqual(1, _tagHelperOutput.Attributes.Count(attribut => attribut.Name == "async"));
            Assert.IsTrue(string.IsNullOrEmpty(_tagHelperOutput.Attributes["async"].Value?.ToString()));
        }

        [Test]
        public void Process_DeferAttribute_ExistsOnlyOnce_AndHasEmptyValue()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaScriptTagHelper(_settingsMock.Object);
            _tagHelperOutput.Attributes.Add("defer", "test");
            _tagHelperOutput.Attributes.Add("defer", "othertest");

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);

            // Assert
            Assert.AreEqual(1, _tagHelperOutput.Attributes.Count(attribut => attribut.Name == "defer"));
            Assert.IsTrue(string.IsNullOrEmpty(_tagHelperOutput.Attributes["defer"].Value?.ToString()));
        }
    }
}
