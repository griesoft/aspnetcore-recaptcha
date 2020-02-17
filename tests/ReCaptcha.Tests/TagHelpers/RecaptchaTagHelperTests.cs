using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GSoftware.AspNetCore.ReCaptcha.Configuration;
using GSoftware.AspNetCore.ReCaptcha.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace ReCaptcha.Tests.TagHelpers
{
    [TestFixture]
    public class RecaptchaTagHelperTests
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

            _tagHelperOutput = new TagHelperOutput("recaptcha",
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
        public void Process_ShouldChangeTagTo_DivTag()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object);

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);

            // Assert
            Assert.AreEqual("div", _tagHelperOutput.TagName);
        }

        [Test]
        public void Process_ShouldChange_TagModeTo_StartTagAndEndTag()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object);
            _tagHelperOutput.TagMode = TagMode.SelfClosing;

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);

            // Assert
            Assert.AreEqual(TagMode.StartTagAndEndTag, _tagHelperOutput.TagMode);
        }

        [Test]
        public void Process_ShouldAdd_RecaptchaClass()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object);

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);

            // Assert
            Assert.IsTrue(_tagHelperOutput.Attributes.ContainsName("class"));
            Assert.AreEqual("g-recaptcha", _tagHelperOutput.Attributes["class"].Value);
        }

        [Test]
        public void Process_ShouldAdd_RecaptchaClassAnd_KeepExistingClasses()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object);
            _tagHelperOutput.Attributes.Add("class", "container text-center");

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);
            var classes = _tagHelperOutput.Attributes["class"].Value.ToString().Split(" ", StringSplitOptions.RemoveEmptyEntries);

            // Assert
            Assert.AreEqual(3, classes.Length);
            Assert.IsTrue(classes.Contains("g-recaptcha"));
            Assert.IsTrue(classes.Contains("text-center"));
            Assert.IsTrue(classes.Contains("container"));
        }

        [Test]
        public void Process_ShouldAdd_SiteKeyAttribute()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object);

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);

            // Assert
            _settingsMock.Verify();
            Assert.IsTrue(_tagHelperOutput.Attributes.ContainsName("data-sitekey"));
            Assert.AreEqual(SiteKey, _tagHelperOutput.Attributes["data-sitekey"].Value.ToString());
        }

        [Test]
        public void Process_ShouldOverrideExisting_SiteKeyAttribute()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object);
            _tagHelperOutput.Attributes.Add("data-sitekey", "false-key");

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);

            // Assert
            _settingsMock.Verify();
            Assert.AreEqual(1, _tagHelperOutput.Attributes.Count(attribute => attribute.Name == "data-sitekey"));
            Assert.AreEqual(SiteKey, _tagHelperOutput.Attributes["data-sitekey"].Value.ToString());
        }

        [Test]
        public void Process_ShouldAdd_DataSizeAttribute()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object);

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);

            // Assert
            Assert.IsTrue(_tagHelperOutput.Attributes.ContainsName("data-size"));
            Assert.AreEqual(Size.Normal.ToString().ToLowerInvariant(), _tagHelperOutput.Attributes["data-size"].Value.ToString());
        }

        [Test]
        public void Process_ShouldOverrideExisting_DataSizeAttribute()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object)
            {
                Size = Size.Compact
            };

            _tagHelperOutput.Attributes.Add("data-size", "normal");

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);

            // Assert
            Assert.AreEqual(1, _tagHelperOutput.Attributes.Count(attribute => attribute.Name == "data-size"));
            Assert.AreEqual(Size.Compact.ToString().ToLowerInvariant(), _tagHelperOutput.Attributes["data-size"].Value.ToString());
        }

        [Test]
        public void Process_ShouldAdd_DataBadgeAttribute_WhenInvisibleSize()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object)
            {
                Size = Size.Invisible
            };

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);

            // Assert
            Assert.IsTrue(_tagHelperOutput.Attributes.ContainsName("data-badge"));
            Assert.AreEqual(BadgePosition.BottomRight.ToString().ToLowerInvariant(), _tagHelperOutput.Attributes["data-badge"].Value.ToString());
        }

        [Test]
        public void Process_ShouldOverrideExisting_DataBadgeAttribute_WhenInvisibleSize()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object)
            {
                Size = Size.Invisible,
                Badge = BadgePosition.BottomLeft
            };

            _tagHelperOutput.Attributes.Add("data-badge", "bottomright");

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);

            // Assert
            Assert.AreEqual(1, _tagHelperOutput.Attributes.Count(attribute => attribute.Name == "data-badge"));
            Assert.AreEqual(BadgePosition.BottomLeft.ToString().ToLowerInvariant(), _tagHelperOutput.Attributes["data-badge"].Value.ToString());
        }

        [Test]
        public void Process_ShouldNotAdd_DataBadgeAttribute_WhenSizeNotInvisible()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object)
            {
                Size = Size.Compact
            };

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);

            // Assert
            Assert.IsFalse(_tagHelperOutput.Attributes.ContainsName("data-badge"));
        }

        [Test]
        public void Process_ShouldAdd_DataThemeAttribute_WhenNotInvisibleSize()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object)
            {
                Size = Size.Compact
            };

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);

            // Assert
            Assert.IsTrue(_tagHelperOutput.Attributes.ContainsName("data-theme"));
            Assert.AreEqual(Theme.Light.ToString().ToLowerInvariant(), _tagHelperOutput.Attributes["data-theme"].Value.ToString());
        }

        [Test]
        public void Process_ShouldOverrideExisting_DataThemeAttribute_WhenNotInvisibleSize()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object)
            {
                Theme = Theme.Dark
            };

            _tagHelperOutput.Attributes.Add("data-theme", "light");

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);

            // Assert
            Assert.AreEqual(1, _tagHelperOutput.Attributes.Count(attribute => attribute.Name == "data-theme"));
            Assert.AreEqual(Theme.Dark.ToString().ToLowerInvariant(), _tagHelperOutput.Attributes["data-theme"].Value.ToString());
        }

        [Test]
        public void Process_ShouldNotAdd_DataThemeAttribute_WhenSizeIsInvisible()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object)
            {
                Size = Size.Invisible,
                Theme = Theme.Dark
            };

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);

            // Assert
            Assert.IsFalse(_tagHelperOutput.Attributes.ContainsName("data-theme"));
        }

        [Test]
        public void Process_ShouldAdd_DataTabindexAttribute()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object)
            {
                TabIndex = 2
            };

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);

            // Assert
            Assert.IsTrue(_tagHelperOutput.Attributes.ContainsName("data-tabindex"));
            Assert.AreEqual(2, (int)_tagHelperOutput.Attributes["data-tabindex"].Value);
        }

        [Test]
        public void Process_ShouldOverrideExisting_DataTabIndexAttribute()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object)
            {
                TabIndex = 3
            };

            _tagHelperOutput.Attributes.Add("data-tabindex", "1");

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);

            // Assert
            Assert.AreEqual(1, _tagHelperOutput.Attributes.Count(attribute => attribute.Name == "data-tabindex"));
            Assert.AreEqual(3, (int)_tagHelperOutput.Attributes["data-tabindex"].Value);
        }

        [Test]
        public void Process_ShouldNotAdd_DataTabIndexAttribute_WhenTabIndexNull()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object);

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);

            // Assert
            Assert.IsFalse(_tagHelperOutput.Attributes.ContainsName("data-tabindex"));
        }

        [Test]
        public void Process_ShouldAdd_DataCallbackAttribute()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object)
            {
                Callback = "myCallback"
            };

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);

            // Assert
            Assert.IsTrue(_tagHelperOutput.Attributes.ContainsName("data-callback"));
            Assert.AreEqual("myCallback", _tagHelperOutput.Attributes["data-callback"].Value.ToString());
        }

        [Test]
        public void Process_ShouldOverrideExisting_DataCallbackAttribute()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object)
            {
                Callback = "myCallback"
            };

            _tagHelperOutput.Attributes.Add("data-callback", "fake-callback");

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);

            // Assert
            Assert.AreEqual(1, _tagHelperOutput.Attributes.Count(attribute => attribute.Name == "data-callback"));
            Assert.AreEqual("myCallback", _tagHelperOutput.Attributes["data-callback"].Value.ToString());
        }

        [Test]
        public void Process_ShouldNotAdd_DataCallbackAttribute_WhenCallbackNull()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object);

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);

            // Assert
            Assert.IsFalse(_tagHelperOutput.Attributes.ContainsName("data-callback"));
        }

        [Test]
        public void Process_ShouldAdd_DataExpiredCallbackAttribute()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object)
            {
                ExpiredCallback = "myCallback"
            };

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);

            // Assert
            Assert.IsTrue(_tagHelperOutput.Attributes.ContainsName("data-expired-callback"));
            Assert.AreEqual("myCallback", _tagHelperOutput.Attributes["data-expired-callback"].Value.ToString());
        }

        [Test]
        public void Process_ShouldOverrideExisting_DataExpiredCallbackAttribute()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object)
            {
                ExpiredCallback = "myCallback"
            };

            _tagHelperOutput.Attributes.Add("data-expired-callback", "fake-callback");

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);

            // Assert
            Assert.AreEqual(1, _tagHelperOutput.Attributes.Count(attribute => attribute.Name == "data-expired-callback"));
            Assert.AreEqual("myCallback", _tagHelperOutput.Attributes["data-expired-callback"].Value.ToString());
        }

        [Test]
        public void Process_ShouldNotAdd_DataExpiredCallbackAttribute_WhenExpiredCallbackNull()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object);

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);

            // Assert
            Assert.IsFalse(_tagHelperOutput.Attributes.ContainsName("data-expired-callback"));
        }

        [Test]
        public void Process_ShouldAdd_DataErrorCallbackAttribute()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object)
            {
                ErrorCallback = "myCallback"
            };

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);

            // Assert
            Assert.IsTrue(_tagHelperOutput.Attributes.ContainsName("data-error-callback"));
            Assert.AreEqual("myCallback", _tagHelperOutput.Attributes["data-error-callback"].Value.ToString());
        }

        [Test]
        public void Process_ShouldOverrideExisting_DataErrorCallbackAttribute()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object)
            {
                ErrorCallback = "myCallback"
            };

            _tagHelperOutput.Attributes.Add("data-error-callback", "fake-callback");

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);

            // Assert
            Assert.AreEqual(1, _tagHelperOutput.Attributes.Count(attribute => attribute.Name == "data-error-callback"));
            Assert.AreEqual("myCallback", _tagHelperOutput.Attributes["data-error-callback"].Value.ToString());
        }

        [Test]
        public void Process_ShouldNotAdd_DataErrorCallbackAttribute_WhenErrorCallbackNull()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object);

            // Act
            scriptTagHelper.Process(_context, _tagHelperOutput);

            // Assert
            Assert.IsFalse(_tagHelperOutput.Attributes.ContainsName("data-error-callback"));
        }
    }
}
