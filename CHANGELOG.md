# [2.2.1](https://github.com/griesoft/aspnetcore-recaptcha/releases/tag/v2.2.1) (25.09.2022)

### Added:
- Support forward proxy #19 ([mikocot](https://github.com/mikocot))

### New Contributor:
[mikocot](https://github.com/mikocot)

# [2.0.0](https://github.com/griesoft/aspnetcore-recaptcha/releases/tag/v2.0.1) (22.04.2022)

### Added:
- An Action property to the ValidateRecaptchaFilter and the ValidateRecaptchaAttribute, for reCAPTCHA V3 action validation.
- A default callback script, which is added to the bottom of the body when automatically binding the challeng to an element.
- New FormId property to RecaptchaInvisibleTagHelper, which should be set when automatically binding to a challenge.
- A new RecaptchaV3TagHelper for automatic binding of V3 challenges.
- Added NETCOREAPP3.1, NET5 and NET6 as target frameworks.

### Updated:
- XML documentation was updated for some classes, methods and properties.
- The RecaptchaInvisibleTagHelper now supports automatic binding to the challenge.
- The RecaptchaScriptTagHelper now fully supports reCAPTCHA V3, so you may now make use of automatic or explicit rendering.

### Removed:
- Dropped NETCOREAPP2.1 and NETCOREAPP3.0 from target framworks.
