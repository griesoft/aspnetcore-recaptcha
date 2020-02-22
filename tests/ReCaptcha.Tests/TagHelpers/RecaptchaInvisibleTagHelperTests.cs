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
    public class RecaptchaInvisibleTagHelperTests
    {
        private const string SiteKey = "unit_test_site_key";
        private const string CallbackName = "myCallback";

        private Mock<IOptionsMonitor<RecaptchaSettings>> _settingsMock;
        private Mock<IOptionsMonitor<RecaptchaOptions>> _optionsMock;
        private TagHelperOutput _tagHelperOutputStub;
        private TagHelperContext _contextStub;
        private RecaptchaInvisibleTagHelper invisibleTagHelper;

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

            _tagHelperOutputStub = new TagHelperOutput("recaptcha-invisible",
                new TagHelperAttributeList(), (useCachedResult, htmlEncoder) =>
                {
                    var tagHelperContent = new DefaultTagHelperContent();
                    tagHelperContent.SetContent(string.Empty);
                    return Task.FromResult<TagHelperContent>(tagHelperContent);
                });

            _contextStub = new TagHelperContext(new TagHelperAttributeList(),
                new Dictionary<object, object>(),
                Guid.NewGuid().ToString("N"));

            invisibleTagHelper = new RecaptchaInvisibleTagHelper(_settingsMock.Object, _optionsMock.Object)
            {
                Callback = CallbackName
            };
        }

        [Test]
        public void Construction_IsSuccessful()
        {
            // Arrange
            _settingsMock = new Mock<IOptionsMonitor<RecaptchaSettings>>();
            _settingsMock.SetupGet(instance => instance.CurrentValue)
                .Returns(new RecaptchaSettings())
                .Verifiable();

            _optionsMock = new Mock<IOptionsMonitor<RecaptchaOptions>>();
            _optionsMock.SetupGet(instance => instance.CurrentValue)
                .Returns(new RecaptchaOptions())
                .Verifiable();

            // Act
            var instance = new RecaptchaInvisibleTagHelper(_settingsMock.Object, _optionsMock.Object);

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
            Assert.Throws<ArgumentNullException>(() => new RecaptchaInvisibleTagHelper(null, _optionsMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrow_WhenOptionsNull()
        {
            // Arrange


            // Act


            // Assert
            Assert.Throws<ArgumentNullException>(() => new RecaptchaInvisibleTagHelper(_settingsMock.Object, null));
        }

        [Test]
        public void Constructor_ShouldSet_DefaultValues_FromGlobalOptions()
        {
            // Arrange
            _settingsMock = new Mock<IOptionsMonitor<RecaptchaSettings>>();
            _settingsMock.SetupGet(instance => instance.CurrentValue)
                .Returns(new RecaptchaSettings())
                .Verifiable();

            _optionsMock = new Mock<IOptionsMonitor<RecaptchaOptions>>();
            _optionsMock.SetupGet(instance => instance.CurrentValue)
                .Returns(new RecaptchaOptions()
                {
                    Badge = BadgePosition.Inline
                })
                .Verifiable();

            // Act
            var tagHelper = new RecaptchaInvisibleTagHelper(_settingsMock.Object, _optionsMock.Object);

            // Assert
            _optionsMock.Verify(options => options.CurrentValue, Times.Once);
            Assert.AreEqual(BadgePosition.Inline, tagHelper.Badge);
        }

        [Test]
        public void Process_ShouldThrow_ArgumentNullException_WhenOutputNull()
        {
            // Arrange


            // Act
            

            // Assert
            Assert.Throws<ArgumentNullException>(() => invisibleTagHelper.Process(_contextStub, null));
        }

        [Test]
        public void Process_ShouldThrow_NullReferenceException_WhenCallbackNullOrEmpty()
        {
            // Arrange
            var nullCallbackTagHelper = new RecaptchaInvisibleTagHelper(_settingsMock.Object, _optionsMock.Object)
            {
                Callback = null
            };
            var emptyCallbackTagHelper = new RecaptchaInvisibleTagHelper(_settingsMock.Object, _optionsMock.Object)
            {
                Callback = string.Empty
            };

            // Act


            // Assert
            Assert.Throws<NullReferenceException>(() => nullCallbackTagHelper.Process(_contextStub, _tagHelperOutputStub));
            Assert.Throws<NullReferenceException>(() => emptyCallbackTagHelper.Process(_contextStub, _tagHelperOutputStub));
        }

        [Test]
        public void Process_ShouldRemove_ReInvisibleAttribute_WhenButtonTag()
        {
            // Arrange
            _tagHelperOutputStub.TagName = "button";
            _tagHelperOutputStub.Attributes.Add("re-invisible", null);

            // Act
            invisibleTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.IsFalse(_tagHelperOutputStub.Attributes.ContainsName("re-invisible"));
        }

        [Test]
        public void Process_ShouldChange_TagNameToDiv_WhenNotButtonTag()
        {
            // Arrange


            // Act
            invisibleTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.AreEqual("div", _tagHelperOutputStub.TagName);
        }

        [Test]
        public void Process_ShouldAdd_DataSizeAttribute_WithValueInvisible()
        {
            // Arrange


            // Act
            invisibleTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.IsTrue(_tagHelperOutputStub.Attributes.ContainsName("data-size"));
            Assert.AreEqual("invisible", _tagHelperOutputStub.Attributes["data-size"].Value);
        }

        [Test]
        public void Process_ShouldOverrideExisting_DataSizeAttribute_WithValueInvisible()
        {
            // Arrange
            _tagHelperOutputStub.Attributes.Add("data-size", "normal");

            // Act
            invisibleTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.AreEqual(1, _tagHelperOutputStub.Attributes.Count(attribute => attribute.Name == "data-size"));
            Assert.AreEqual("invisible", _tagHelperOutputStub.Attributes["data-size"].Value);
        }

        [Test]
        public void Process_ShouldChange_TagModeTo_StartTagAndEndTag()
        {
            // Arrange
            _tagHelperOutputStub.TagMode = TagMode.SelfClosing;

            // Act
            invisibleTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.AreEqual(TagMode.StartTagAndEndTag, _tagHelperOutputStub.TagMode);
        }

        [Test]
        public void Process_ShouldAdd_RecaptchaClass()
        {
            // Arrange


            // Act
            invisibleTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.IsTrue(_tagHelperOutputStub.Attributes.ContainsName("class"));
            Assert.AreEqual("g-recaptcha", _tagHelperOutputStub.Attributes["class"].Value);
        }

        [Test]
        public void Process_ShouldAdd_RecaptchaClassAnd_KeepExistingClasses()
        {
            // Arrange
            _tagHelperOutputStub.Attributes.Add("class", "container text-center");

            // Act
            invisibleTagHelper.Process(_contextStub, _tagHelperOutputStub);
            var classes = _tagHelperOutputStub.Attributes["class"].Value.ToString().Split(" ", StringSplitOptions.RemoveEmptyEntries);

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


            // Act
            invisibleTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            _settingsMock.Verify(a => a.CurrentValue, Times.Once);
            Assert.IsTrue(_tagHelperOutputStub.Attributes.ContainsName("data-sitekey"));
            Assert.AreEqual(SiteKey, _tagHelperOutputStub.Attributes["data-sitekey"].Value.ToString());
        }

        [Test]
        public void Process_ShouldOverrideExisting_SiteKeyAttribute()
        {
            // Arrange
            _tagHelperOutputStub.Attributes.Add("data-sitekey", "false-key");

            // Act
            invisibleTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            _settingsMock.Verify(a => a.CurrentValue, Times.Once);
            Assert.AreEqual(1, _tagHelperOutputStub.Attributes.Count(attribute => attribute.Name == "data-sitekey"));
            Assert.AreEqual(SiteKey, _tagHelperOutputStub.Attributes["data-sitekey"].Value.ToString());
        }

        [Test]
        public void Process_ShouldAdd_DataBadgeAttribute()
        {
            // Arrange


            // Act
            invisibleTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.IsTrue(_tagHelperOutputStub.Attributes.ContainsName("data-badge"));
            Assert.AreEqual(BadgePosition.BottomRight.ToString().ToLowerInvariant(), _tagHelperOutputStub.Attributes["data-badge"].Value.ToString());
        }

        [Test]
        public void Process_ShouldOverrideExisting_DataBadgeAttribute()
        {
            // Arrange
            invisibleTagHelper.Badge = BadgePosition.BottomLeft;

            _tagHelperOutputStub.Attributes.Add("data-badge", "bottomright");

            // Act
            invisibleTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.AreEqual(1, _tagHelperOutputStub.Attributes.Count(attribute => attribute.Name == "data-badge"));
            Assert.AreEqual(BadgePosition.BottomLeft.ToString().ToLowerInvariant(), _tagHelperOutputStub.Attributes["data-badge"].Value.ToString());
        }

        [Test]
        public void Process_ShouldAdd_DataCallbackAttribute()
        {
            // Arrange


            // Act
            invisibleTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.IsTrue(_tagHelperOutputStub.Attributes.ContainsName("data-callback"));
            Assert.AreEqual(CallbackName, _tagHelperOutputStub.Attributes["data-callback"].Value.ToString());
        }

        [Test]
        public void Process_ShouldOverrideExisting_DataCallbackAttribute()
        {
            // Arrange

            _tagHelperOutputStub.Attributes.Add("data-callback", "myFakeCallback");

            // Act
            invisibleTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.AreEqual(1, _tagHelperOutputStub.Attributes.Count(attribute => attribute.Name == "data-callback"));
            Assert.AreEqual(CallbackName, _tagHelperOutputStub.Attributes["data-callback"].Value.ToString());
        }

        [Test]
        public void Process_ShouldAdd_DataTabindexAttribute()
        {
            // Arrange
            invisibleTagHelper.TabIndex = 2;

            // Act
            invisibleTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.IsTrue(_tagHelperOutputStub.Attributes.ContainsName("data-tabindex"));
            Assert.AreEqual(2, (int)_tagHelperOutputStub.Attributes["data-tabindex"].Value);
        }

        [Test]
        public void Process_ShouldOverrideExisting_DataTabIndexAttribute()
        {
            // Arrange
            invisibleTagHelper.TabIndex = 3;

            _tagHelperOutputStub.Attributes.Add("data-tabindex", "1");

            // Act
            invisibleTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.AreEqual(1, _tagHelperOutputStub.Attributes.Count(attribute => attribute.Name == "data-tabindex"));
            Assert.AreEqual(3, (int)_tagHelperOutputStub.Attributes["data-tabindex"].Value);
        }

        [Test]
        public void Process_ShouldNotAdd_DataTabIndexAttribute_WhenTabIndexNull()
        {
            // Arrange


            // Act
            invisibleTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.IsFalse(_tagHelperOutputStub.Attributes.ContainsName("data-tabindex"));
        }

        [Test]
        public void Process_ShouldAdd_DataExpiredCallbackAttribute()
        {
            // Arrange
            invisibleTagHelper.ExpiredCallback = CallbackName;

            // Act
            invisibleTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.IsTrue(_tagHelperOutputStub.Attributes.ContainsName("data-expired-callback"));
            Assert.AreEqual(CallbackName, _tagHelperOutputStub.Attributes["data-expired-callback"].Value.ToString());
        }

        [Test]
        public void Process_ShouldOverrideExisting_DataExpiredCallbackAttribute()
        {
            // Arrange
            invisibleTagHelper.ExpiredCallback = CallbackName;

            _tagHelperOutputStub.Attributes.Add("data-expired-callback", "fake-callback");

            // Act
            invisibleTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.AreEqual(1, _tagHelperOutputStub.Attributes.Count(attribute => attribute.Name == "data-expired-callback"));
            Assert.AreEqual(CallbackName, _tagHelperOutputStub.Attributes["data-expired-callback"].Value.ToString());
        }

        [Test]
        public void Process_ShouldNotAdd_DataExpiredCallbackAttribute_WhenExpiredCallbackNullOrEmpty()
        {
            // Arrange


            // Act
            invisibleTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.IsFalse(_tagHelperOutputStub.Attributes.ContainsName("data-expired-callback"));
        }

        [Test]
        public void Process_ShouldAdd_DataErrorCallbackAttribute()
        {
            // Arrange
            invisibleTagHelper.ErrorCallback = CallbackName;

            // Act
            invisibleTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.IsTrue(_tagHelperOutputStub.Attributes.ContainsName("data-error-callback"));
            Assert.AreEqual(CallbackName, _tagHelperOutputStub.Attributes["data-error-callback"].Value.ToString());
        }

        [Test]
        public void Process_ShouldOverrideExisting_DataErrorCallbackAttribute()
        {
            // Arrange
            invisibleTagHelper.ErrorCallback = CallbackName;

            _tagHelperOutputStub.Attributes.Add("data-error-callback", "fake-callback");

            // Act
            invisibleTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.AreEqual(1, _tagHelperOutputStub.Attributes.Count(attribute => attribute.Name == "data-error-callback"));
            Assert.AreEqual(CallbackName, _tagHelperOutputStub.Attributes["data-error-callback"].Value.ToString());
        }

        [Test]
        public void Process_ShouldNotAdd_DataErrorCallbackAttribute_WhenErrorCallbackNullOrEmpty()
        {
            // Arrange


            // Act
            invisibleTagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.IsFalse(_tagHelperOutputStub.Attributes.ContainsName("data-error-callback"));
        }
    }
}
