# [2.0.0](https://github.com/jgdevlabs/aspnetcore-recaptcha/releases/tag/v2.0.1) (22.04.2022)

### Added:
- An Action property to the ValidateRecaptchaFilter and the ValidateRecaptchaAttribute, for reCAPTCHA V3 action validation. (eebb30953c322df5a903bcab8f51ff5746cf0f2a)
- A default callback script, which is added to the bottom of the body when automatically binding the challeng to an element. (ca3d8a38d21679cac55a8ff4ca7e547bdeb41cbf)
- New FormId property to RecaptchaInvisibleTagHelper, which should be set when automatically binding to a challenge. (ca3d8a38d21679cac55a8ff4ca7e547bdeb41cbf)
- A new RecaptchaV3TagHelper for automatic binding of V3 challenges. (d5fef5dab5541c35296c3716780e56e89a5af1fa)
- Added NETCOREAPP3.1, NET5 and NET6 as target frameworks. (7e5a4f42a22ea143a100d68a7b5ce525c92e28d9)

### Updated:
- XML documentation was updated for some classes, methods and properties.
- The RecaptchaInvisibleTagHelper now supports automatic binding to the challenge.
- The RecaptchaScriptTagHelper now fully supports reCAPTCHA V3, so you may now make use of automatic or explicit rendering. (c7710981ee4f544089bac388cde15078594f1fb7)

### Removed:
- Dropped NETCOREAPP2.1 and NETCOREAPP3.0 from target framworks. (7e5a4f42a22ea143a100d68a7b5ce525c92e28d9)
