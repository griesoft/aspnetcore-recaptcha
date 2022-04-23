using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Griesoft.AspNetCore.ReCaptcha.Configuration;
using Griesoft.AspNetCore.ReCaptcha.TagHelpers;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace ReCaptcha.Tests.TagHelpers
{
    [TestFixture]
    public class RecaptchaV3TagHelperTests
    {
        private const string SiteKey = "unit_test_site_key";

        private Mock<IOptionsMonitor<RecaptchaSettings>> _settingsMock;
        private Mock<ITagHelperComponentManager> _tagHelperComponentManagerMock;
        private Mock<ICollection<ITagHelperComponent>> _tagHelperComponentCollectionMock;
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

            _tagHelperComponentCollectionMock = new Mock<ICollection<ITagHelperComponent>>();
            _tagHelperComponentCollectionMock.Setup(instance => instance.Add(It.IsAny<CallbackScriptTagHelperComponent>()))
                .Verifiable();

            _tagHelperComponentManagerMock = new Mock<ITagHelperComponentManager>();
            _tagHelperComponentManagerMock.SetupGet(instance => instance.Components)
                .Returns(_tagHelperComponentCollectionMock.Object);

            _tagHelperOutputStub = new TagHelperOutput("recaptchav3",
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
            var instance = new RecaptchaV3TagHelper(_settingsMock.Object, _tagHelperComponentManagerMock.Object);

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
            Assert.Throws<ArgumentNullException>(() => new RecaptchaV3TagHelper(null, _tagHelperComponentManagerMock.Object));
        }

        [Test]
        public void Constructor_ShouldThrow_WhenTagHelperComponentManagerNull()
        {
            // Arrange


            // Act


            // Assert
            Assert.Throws<ArgumentNullException>(() => new RecaptchaV3TagHelper(_settingsMock.Object, null));
        }

        [Test]
        public void Process_ShouldThrow_ArgumentNullException()
        {
            // Arrange
            var scriptTagHelper = new RecaptchaV3TagHelper(_settingsMock.Object, _tagHelperComponentManagerMock.Object);

            // Act


            // Assert
            Assert.Throws<ArgumentNullException>(() => scriptTagHelper.Process(_contextStub, null));
        }

        [Test]
        public void Process_ShouldThrow_InvalidOperationException_WhenCallbackAndFormIdNullOrEmpty()
        {
            // Arrange
            var nullCallbackTagHelper = new RecaptchaV3TagHelper(_settingsMock.Object, _tagHelperComponentManagerMock.Object)
            {
                Callback = null,
                FormId = null
            };
            var emptyCallbackTagHelper = new RecaptchaV3TagHelper(_settingsMock.Object, _tagHelperComponentManagerMock.Object)
            {
                Callback = string.Empty,
                FormId = string.Empty
            };

            // Act


            // Assert
            Assert.Throws<InvalidOperationException>(() => nullCallbackTagHelper.Process(_contextStub, _tagHelperOutputStub));
            Assert.Throws<InvalidOperationException>(() => emptyCallbackTagHelper.Process(_contextStub, _tagHelperOutputStub));
        }

        [Test]
        public void Process_ShouldThrow_InvalidOperationException_WhenActionIsNullOrEmpty()
        {
            // Arrange
            var nullCallbackTagHelper = new RecaptchaV3TagHelper(_settingsMock.Object, _tagHelperComponentManagerMock.Object)
            {
                FormId = "formId",
                Action = null
            };
            var emptyCallbackTagHelper = new RecaptchaV3TagHelper(_settingsMock.Object, _tagHelperComponentManagerMock.Object)
            {
                FormId = "formId",
                Action = String.Empty
            };

            // Act


            // Assert
            Assert.Throws<InvalidOperationException>(() => nullCallbackTagHelper.Process(_contextStub, _tagHelperOutputStub));
            Assert.Throws<InvalidOperationException>(() => emptyCallbackTagHelper.Process(_contextStub, _tagHelperOutputStub));
        }

        [Test]
        public void Process_ShouldSet_CallbackToDefaultCallback_WhenCallbackIsNullOrEmpty()
        {
            // Arrange
            var formId = $"formId";
            var tag = new RecaptchaV3TagHelper(_settingsMock.Object, _tagHelperComponentManagerMock.Object)
            {
                Callback = null,
                FormId = formId,
                Action = "submit"
            };

            // Act
            tag.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.AreEqual(tag.Callback, $"submit{formId}");
        }

        [Test]
        public void Process_ShouldAdd_CallbackScriptTagHelperComponent_WhenCallbackIsNullOrEmpty()
        {
            // Arrange
            var tag = new RecaptchaV3TagHelper(_settingsMock.Object, _tagHelperComponentManagerMock.Object)
            {
                Callback = null,
                FormId = "formId",
                Action = "submit"
            };

            // Act
            tag.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            _tagHelperComponentCollectionMock.Verify();
        }

        [Test]
        public void Process_ShouldChange_TagModeTo_StartTagAndEndTag()
        {
            // Arrange
            var tagHelper = new RecaptchaV3TagHelper(_settingsMock.Object, _tagHelperComponentManagerMock.Object)
            {
                Callback = "submit",
                Action = "submit"
            };
            _tagHelperOutputStub.TagMode = TagMode.SelfClosing;

            // Act
            tagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.AreEqual(TagMode.StartTagAndEndTag, _tagHelperOutputStub.TagMode);
        }

        [Test]
        public void Process_ShouldChangeTag_ToButtonTag()
        {
            // Arrange
            var tagHelper = new RecaptchaV3TagHelper(_settingsMock.Object, _tagHelperComponentManagerMock.Object)
            {
                Callback = "submit",
                Action = "submit"
            };

            // Act
            tagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.AreEqual("button", _tagHelperOutputStub.TagName);
        }

        [Test]
        public void Process_ShouldAdd_RecaptchaClass()
        {
            // Arrange
            var tagHelper = new RecaptchaV3TagHelper(_settingsMock.Object, _tagHelperComponentManagerMock.Object)
            {
                Callback = "submit",
                Action = "submit"
            };

            // Act
            tagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.IsTrue(_tagHelperOutputStub.Attributes.ContainsName("class"));
            Assert.AreEqual("g-recaptcha", _tagHelperOutputStub.Attributes["class"].Value);
        }

        [Test]
        public void Process_ShouldAdd_RecaptchaClassAnd_KeepExistingClasses()
        {
            // Arrange
            var tagHelper = new RecaptchaV3TagHelper(_settingsMock.Object, _tagHelperComponentManagerMock.Object)
            {
                Callback = "submit",
                Action = "submit"
            };
            _tagHelperOutputStub.Attributes.Add("class", "container text-center");

            // Act
            tagHelper.Process(_contextStub, _tagHelperOutputStub);
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
            var tagHelper = new RecaptchaV3TagHelper(_settingsMock.Object, _tagHelperComponentManagerMock.Object)
            {
                Callback = "myCallback",
                Action = "submit"
            };

            // Act
            tagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.IsTrue(_tagHelperOutputStub.Attributes.ContainsName("data-sitekey"));
            Assert.AreEqual(SiteKey, _tagHelperOutputStub.Attributes["data-sitekey"].Value.ToString());
        }

        [Test]
        public void Process_ShouldOverrideExisting_SiteKeyAttribute()
        {
            // Arrange
            var tagHelper = new RecaptchaV3TagHelper(_settingsMock.Object, _tagHelperComponentManagerMock.Object)
            {
                Callback = "myCallback",
                Action = "submit"
            };
            _tagHelperOutputStub.Attributes.Add("data-sitekey", "false-key");

            // Act
            tagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.AreEqual(1, _tagHelperOutputStub.Attributes.Count(attribute => attribute.Name == "data-sitekey"));
            Assert.AreEqual(SiteKey, _tagHelperOutputStub.Attributes["data-sitekey"].Value.ToString());
        }

        [Test]
        public void Process_ShouldAdd_DataCallbackAttribute()
        {
            // Arrange
            var tagHelper = new RecaptchaV3TagHelper(_settingsMock.Object, _tagHelperComponentManagerMock.Object)
            {
                Callback = "myCallback",
                Action = "submit"
            };

            // Act
            tagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.IsTrue(_tagHelperOutputStub.Attributes.ContainsName("data-callback"));
            Assert.AreEqual("myCallback", _tagHelperOutputStub.Attributes["data-callback"].Value.ToString());
        }

        [Test]
        public void Process_ShouldOverrideExisting_DataCallbackAttribute()
        {
            // Arrange
            var tagHelper = new RecaptchaV3TagHelper(_settingsMock.Object, _tagHelperComponentManagerMock.Object)
            {
                Callback = "myCallback",
                Action = "submit"
            };

            _tagHelperOutputStub.Attributes.Add("data-callback", "fake-callback");

            // Act
            tagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.AreEqual(1, _tagHelperOutputStub.Attributes.Count(attribute => attribute.Name == "data-callback"));
            Assert.AreEqual("myCallback", _tagHelperOutputStub.Attributes["data-callback"].Value.ToString());
        }

        [Test]
        public void Process_ShouldAdd_DataActionAttribute()
        {
            // Arrange
            var tagHelper = new RecaptchaV3TagHelper(_settingsMock.Object, _tagHelperComponentManagerMock.Object)
            {
                Callback = "myCallback",
                Action = "submit"
            };

            // Act
            tagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.IsTrue(_tagHelperOutputStub.Attributes.ContainsName("data-action"));
            Assert.AreEqual("submit", _tagHelperOutputStub.Attributes["data-action"].Value.ToString());
        }

        [Test]
        public void Process_ShouldOverrideExisting_DataActionAttribute()
        {
            // Arrange
            var tagHelper = new RecaptchaV3TagHelper(_settingsMock.Object, _tagHelperComponentManagerMock.Object)
            {
                Callback = "myCallback",
                Action = "submit"
            };

            _tagHelperOutputStub.Attributes.Add("data-action", "fake-action");

            // Act
            tagHelper.Process(_contextStub, _tagHelperOutputStub);

            // Assert
            Assert.AreEqual(1, _tagHelperOutputStub.Attributes.Count(attribute => attribute.Name == "data-action"));
            Assert.AreEqual("submit", _tagHelperOutputStub.Attributes["data-action"].Value.ToString());
        }
    }
}
