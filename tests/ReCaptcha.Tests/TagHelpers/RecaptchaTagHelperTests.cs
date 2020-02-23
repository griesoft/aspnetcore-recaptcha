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
        private Mock<IOptionsMonitor<RecaptchaOptions>> _optionsMock;
        private TagHelperOutput _tagHelperOutputStub;
        private TagHelperContext _contextStub;

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

            _optionsMock = new Mock<IOptionsMonitor<RecaptchaOptions>>();
            _optionsMock.SetupGet(instance => instance.CurrentValue)
                .Returns(new RecaptchaOptions())
                .Verifiable();

            _tagHelperOutputStub = new TagHelperOutput("recaptcha",
                new TagHelperAttributeList(), (useCachedResult, htmlEncoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    tagHelperContent.SetContent(string.Empty);
                    return Task.FromResult<TagHelperContent>(tagHelperContent);
                });

            _contextStub = new TagHelperContext(new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));
        }

        [Test]
        public void Construction_IsSuccessful()
        {
            // Arrange


            // Act
            var instance = new RecaptchaTagHelper(_settingsMock.Object, _optionsMock.Object);

            // Assert
            Assert.NotNull(instance);
            _settingsMock.Verify(settings => settings.CurrentValue, Times.Once);
            _optionsMock.Verify(options => options.CurrentValue, Times.Once);
        }

        [Test]
        public void Constructor_ShouldThrow_WhenSettingsNull()
        {
            // Arrange


            // Act


            // Assert
            Assert.Throws<ArgumentNullException>(() => new RecaptchaTagHelper(null, _optionsMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrow_WhenOptionsNull()
        {
            // Arrange


            // Act


            // Assert
            Assert.Throws<ArgumentNullException>(() => new RecaptchaTagHelper(_settingsMock.Object, null));
        }

        [Test]
        public void Constructor_ShouldSet_DefaultValues_FromGlobalOptions()
        {
            // Arrange
            _optionsMock.SetupGet(instance => instance.CurrentValue)
                .Returns(new RecaptchaOptions()
                {
                    Theme = Theme.Dark,
                    Size = Size.Compact
                })
                .Verifiable();

            // Act
            var tagHelper = new RecaptchaTagHelper(_settingsMock.Object, _optionsMock.Object);

            // Assert
            _optionsMock.Verify(options => options.CurrentValue, Times.Once);
            Assert.AreEqual(Theme.Dark, tagHelper.Theme);
            Assert.AreEqual(Size.Compact, tagHelper.Size);
        }

        [Test]
        public void Process_ShouldThrow_ArgumentNullException()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object, _optionsMock.Object);

            // Act


            // Assert
            Assert.Throws<ArgumentNullException>(() => scriptTagHelper.Process(_contextStub, null));
        }

        [Test]
        public void Process_ShouldChangeTagTo_DivTag()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object, _optionsMock.Object);

            // Act
            scriptTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.AreEqual("div", _tagHelperOutputStub.TagName);
        }

        [Test]
        public void Process_ShouldChange_TagModeTo_StartTagAndEndTag()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object, _optionsMock.Object);
            _tagHelperOutputStub.TagMode = TagMode.SelfClosing;

            // Act
            scriptTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.AreEqual(TagMode.StartTagAndEndTag, _tagHelperOutputStub.TagMode);
        }

        [Test]
        public void Process_ShouldAdd_RecaptchaClass()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object, _optionsMock.Object);

            // Act
            scriptTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.IsTrue(_tagHelperOutputStub.Attributes.ContainsName("class"));
            Assert.AreEqual("g-recaptcha", _tagHelperOutputStub.Attributes["class"].Value);
        }

        [Test]
        public void Process_ShouldAdd_RecaptchaClassAnd_KeepExistingClasses()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object, _optionsMock.Object);
            _tagHelperOutputStub.Attributes.Add("class", "container text-center");

            // Act
            scriptTagHelper.Process(_contextStub, _tagHelperOutputStub);
            var classes = _tagHelperOutputStub.Attributes["class"].Value.ToString().Split(" ", StringSplitOptions.RemoveEmptyEntries);

            // Assert
            Assert.AreEqual(3, classes.Length);
            Assert.IsTrue(classes.Contains("g-recaptcha"));
            Assert.IsTrue(classes.Contains("text-center"));
            Assert.IsTrue(classes.Contains("container"));
        }

        [Test]
        public void Process_ShouldReady_FromSettings_OnlyOnce()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object, _optionsMock.Object);

            // Act
            scriptTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            _settingsMock.Verify(mock => mock.CurrentValue, Times.Once);
        }

        [Test]
        public void Process_ShouldAdd_SiteKeyAttribute()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object, _optionsMock.Object);

            // Act
            scriptTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.IsTrue(_tagHelperOutputStub.Attributes.ContainsName("data-sitekey"));
            Assert.AreEqual(SiteKey, _tagHelperOutputStub.Attributes["data-sitekey"].Value.ToString());
        }

        [Test]
        public void Process_ShouldOverrideExisting_SiteKeyAttribute()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object, _optionsMock.Object);
            _tagHelperOutputStub.Attributes.Add("data-sitekey", "false-key");

            // Act
            scriptTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.AreEqual(1, _tagHelperOutputStub.Attributes.Count(attribute => attribute.Name == "data-sitekey"));
            Assert.AreEqual(SiteKey, _tagHelperOutputStub.Attributes["data-sitekey"].Value.ToString());
        }

        [Test]
        public void Process_ShouldAdd_DataSizeAttribute()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object, _optionsMock.Object);

            // Act
            scriptTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.IsTrue(_tagHelperOutputStub.Attributes.ContainsName("data-size"));
            Assert.AreEqual(Size.Normal.ToString().ToLowerInvariant(), _tagHelperOutputStub.Attributes["data-size"].Value.ToString());
        }

        [Test]
        public void Process_ShouldOverrideExisting_DataSizeAttribute()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object, _optionsMock.Object)
            {
                Size = Size.Compact
            };

            _tagHelperOutputStub.Attributes.Add("data-size", "normal");

            // Act
            scriptTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.AreEqual(1, _tagHelperOutputStub.Attributes.Count(attribute => attribute.Name == "data-size"));
            Assert.AreEqual(Size.Compact.ToString().ToLowerInvariant(), _tagHelperOutputStub.Attributes["data-size"].Value.ToString());
        }

        [Test]
        public void Process_ShouldAdd_DataThemeAttribute()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object, _optionsMock.Object)
            {
                Size = Size.Compact
            };

            // Act
            scriptTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.IsTrue(_tagHelperOutputStub.Attributes.ContainsName("data-theme"));
            Assert.AreEqual(Theme.Light.ToString().ToLowerInvariant(), _tagHelperOutputStub.Attributes["data-theme"].Value.ToString());
        }

        [Test]
        public void Process_ShouldOverrideExisting_DataThemeAttribute()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object, _optionsMock.Object)
            {
                Theme = Theme.Dark
            };

            _tagHelperOutputStub.Attributes.Add("data-theme", "light");

            // Act
            scriptTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.AreEqual(1, _tagHelperOutputStub.Attributes.Count(attribute => attribute.Name == "data-theme"));
            Assert.AreEqual(Theme.Dark.ToString().ToLowerInvariant(), _tagHelperOutputStub.Attributes["data-theme"].Value.ToString());
        }

        [Test]
        public void Process_ShouldAdd_DataTabindexAttribute()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object, _optionsMock.Object)
            {
                TabIndex = 2
            };

            // Act
            scriptTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.IsTrue(_tagHelperOutputStub.Attributes.ContainsName("data-tabindex"));
            Assert.AreEqual(2, (int)_tagHelperOutputStub.Attributes["data-tabindex"].Value);
        }

        [Test]
        public void Process_ShouldOverrideExisting_DataTabIndexAttribute()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object, _optionsMock.Object)
            {
                TabIndex = 3
            };

            _tagHelperOutputStub.Attributes.Add("data-tabindex", "1");

            // Act
            scriptTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.AreEqual(1, _tagHelperOutputStub.Attributes.Count(attribute => attribute.Name == "data-tabindex"));
            Assert.AreEqual(3, (int)_tagHelperOutputStub.Attributes["data-tabindex"].Value);
        }

        [Test]
        public void Process_ShouldNotAdd_DataTabIndexAttribute_WhenTabIndexNull()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object, _optionsMock.Object);

            // Act
            scriptTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.IsFalse(_tagHelperOutputStub.Attributes.ContainsName("data-tabindex"));
        }

        [Test]
        public void Process_ShouldAdd_DataCallbackAttribute()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object, _optionsMock.Object)
            {
                Callback = "myCallback"
            };

            // Act
            scriptTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.IsTrue(_tagHelperOutputStub.Attributes.ContainsName("data-callback"));
            Assert.AreEqual("myCallback", _tagHelperOutputStub.Attributes["data-callback"].Value.ToString());
        }

        [Test]
        public void Process_ShouldOverrideExisting_DataCallbackAttribute()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object, _optionsMock.Object)
            {
                Callback = "myCallback"
            };

            _tagHelperOutputStub.Attributes.Add("data-callback", "fake-callback");

            // Act
            scriptTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.AreEqual(1, _tagHelperOutputStub.Attributes.Count(attribute => attribute.Name == "data-callback"));
            Assert.AreEqual("myCallback", _tagHelperOutputStub.Attributes["data-callback"].Value.ToString());
        }

        [Test]
        public void Process_ShouldNotAdd_DataCallbackAttribute_WhenCallbackNullOrEmpty()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object, _optionsMock.Object);

            // Act
            scriptTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.IsFalse(_tagHelperOutputStub.Attributes.ContainsName("data-callback"));
        }

        [Test]
        public void Process_ShouldAdd_DataExpiredCallbackAttribute()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object, _optionsMock.Object)
            {
                ExpiredCallback = "myCallback"
            };

            // Act
            scriptTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.IsTrue(_tagHelperOutputStub.Attributes.ContainsName("data-expired-callback"));
            Assert.AreEqual("myCallback", _tagHelperOutputStub.Attributes["data-expired-callback"].Value.ToString());
        }

        [Test]
        public void Process_ShouldOverrideExisting_DataExpiredCallbackAttribute()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object, _optionsMock.Object)
            {
                ExpiredCallback = "myCallback"
            };

            _tagHelperOutputStub.Attributes.Add("data-expired-callback", "fake-callback");

            // Act
            scriptTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.AreEqual(1, _tagHelperOutputStub.Attributes.Count(attribute => attribute.Name == "data-expired-callback"));
            Assert.AreEqual("myCallback", _tagHelperOutputStub.Attributes["data-expired-callback"].Value.ToString());
        }

        [Test]
        public void Process_ShouldNotAdd_DataExpiredCallbackAttribute_WhenExpiredCallbackNullOrEmpty()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object, _optionsMock.Object);

            // Act
            scriptTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.IsFalse(_tagHelperOutputStub.Attributes.ContainsName("data-expired-callback"));
        }

        [Test]
        public void Process_ShouldAdd_DataErrorCallbackAttribute()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object, _optionsMock.Object)
            {
                ErrorCallback = "myCallback"
            };

            // Act
            scriptTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.IsTrue(_tagHelperOutputStub.Attributes.ContainsName("data-error-callback"));
            Assert.AreEqual("myCallback", _tagHelperOutputStub.Attributes["data-error-callback"].Value.ToString());
        }

        [Test]
        public void Process_ShouldOverrideExisting_DataErrorCallbackAttribute()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object, _optionsMock.Object)
            {
                ErrorCallback = "myCallback"
            };

            _tagHelperOutputStub.Attributes.Add("data-error-callback", "fake-callback");

            // Act
            scriptTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.AreEqual(1, _tagHelperOutputStub.Attributes.Count(attribute => attribute.Name == "data-error-callback"));
            Assert.AreEqual("myCallback", _tagHelperOutputStub.Attributes["data-error-callback"].Value.ToString());
        }

        [Test]
        public void Process_ShouldNotAdd_DataErrorCallbackAttribute_WhenErrorCallbackNullOrEmpty()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaTagHelper(_settingsMock.Object, _optionsMock.Object);

            // Act
            scriptTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.IsFalse(_tagHelperOutputStub.Attributes.ContainsName("data-error-callback"));
        }
    }
}
