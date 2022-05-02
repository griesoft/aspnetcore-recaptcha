# ASP.NET Core reCAPTCHA
A Google reCAPTCHA service for ASP.NET Core. Keep bots away from submitting forms or other actions in just a few steps.

The service supports V2 and V3 and comes with tag helpers that make it easy to add challenges to your forms. Also, backend validation is made easy and requires only the use of an attribute in your controllers or actions that should get validated.

[![Build Status](https://dev.azure.com/griesingersoftware/ASP.NET%20Core%20Recaptcha/_apis/build/status/jgdevlabs.aspnetcore-recaptcha?branchName=master)](https://dev.azure.com/griesingersoftware/ASP.NET%20Core%20Recaptcha/_build/latest?definitionId=17&branchName=master)
[![Build Status](https://vsrm.dev.azure.com/griesingersoftware/_apis/public/Release/badge/f9036ec9-eb1c-4aff-a2b8-27fdaa573d0f/1/2)](https://vsrm.dev.azure.com/griesingersoftware/_apis/public/Release/badge/f9036ec9-eb1c-4aff-a2b8-27fdaa573d0f/1/2)
[![License](https://badgen.net/github/license/griesoft/aspnetcore-recaptcha)](https://github.com/griesoft/aspnetcore-recaptcha/blob/master/LICENSE)
[![NuGet](https://badgen.net/nuget/v/Griesoft.AspNetCore.ReCaptcha)](https://www.nuget.org/packages/Griesoft.AspNetCore.ReCaptcha)
[![GitHub Release](https://badgen.net/github/release/griesoft/aspnetcore-recaptcha)](https://github.com/griesoft/aspnetcore-recaptcha/releases)

## Installation

Install via [NuGet](https://www.nuget.org/packages/Griesoft.AspNetCore.ReCaptcha/) using:

`PM> Install-Package Griesoft.AspNetCore.ReCaptcha`

## Quickstart

### Prequisites
You will need an API key pair which can be acquired by [signing up here](http://www.google.com/recaptcha/admin). For assistance or other questions regarding that topic, refer to [Google's guide](https://developers.google.com/recaptcha/intro#overview).

After sign-up, you should have a **Site key** and a **Secret key**. You will need those to configure the service in your app.

### Configuration

#### Settings

Open your `appsettings.json` and add the following lines:

```json
"RecaptchaSettings": {
    "SiteKey": "<Your site key goes here>",
    "SecretKey": "<Your secret key goes here>"
}
```
**Important:** The `SiteKey` will be exposed to the public, so make sure you don't accidentally swap it with the `SecretKey`.

#### Service Registration

Register this service by calling the `AddRecaptchaService()` method which is an extension method of `IServiceCollection`. For example:

##### .NET 6

```csharp
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRecaptchaService();
```

##### Prior to .NET 6

```csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddRecaptchaService();
}
```

### Adding a reCAPTCHA element to your view

First, import the tag helpers. Open your `_ViewImports.cshtml` file and add the following lines:

```razor
@using Griesoft.AspNetCore.ReCaptcha
@addTagHelper *, Griesoft.AspNetCore.ReCaptcha
```

Next, you need to add the `<recaptcha-script>` to every view you intend to use the reCAPTCHA. That will render the API script. Preferably you would add this somewhere close to the bottom of your body element.

Now you may add a reCAPTCHA challenge to your view where ever you need it. Using the `<recaptcha />` tag in your form will render a reCAPTCHA V2 checkbox inside it.

For invisible reCAPTCHA use:
```html
<button re-invisible form-id="yourFormId">Submit</button>
```

For reCAPTCHA V3 use:
```html
<recaptcha-v3 form-id="yourFormId" action="submit">Submit</recaptcha-v3>
```

### Adding backend validation to an action

Validation is done by decorating your controller or action with `[ValidateRecaptcha]`.

For example:

```csharp
using Griesoft.AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Mvc;

namespace ReCaptcha.Sample.Controllers
{
    public class ExampleController : Controller
    {
        [ValidateRecaptcha]
        public IActionResult FormSubmit(SomeModel model)
        {
            // Will hit the next line only if validation was successful
            return View("FormSubmitSuccessView");
        }
    }
}
```
Now each incoming request to that action will be validated for a valid reCAPTCHA token.

The default behavior for invalid tokens is a 404 (BadRequest) response. But this behavior is configurable, and you may also instead request the validation result as an argument to your action. 

This can be achieved like this:

```csharp
[ValidateRecaptcha(ValidationFailedAction = ValidationFailedAction.ContinueRequest)]
public IActionResult FormSubmit(SomeModel model, ValidationResponse recaptchaResponse)
{
    if (!recaptchaResponse.Success)
    {
        return BadRequest();
    }

    return View("FormSubmitSuccessView");
}
```

In case you are validating a reCAPTCHA V3 token, make sure you also add an action name to your validator.  

For example:

```csharp
[ValidateRecaptcha(Action = "submit")]
public IActionResult FormSubmit(SomeModel model)
{
    return View("FormSubmitSuccessView");
}
```

## Options & Customization

There are global defaults that you may modify on your application startup. Also, the appearance and position of V2 tags may be modified. Either globally or each tag individually.

All options from the [official reCAPTCHA docs](https://developers.google.com/recaptcha/intro) are available to you in this package.

## Detailed Documentation
Is on it's way...

## Contributing
Contributing is heavily encouraged. :muscle: The best way of doing so is by first starting a discussion about new features or improvements you would like to make. Or, in case of a bug, report it first by creating a new issue. From there, you may volunteer to fix it if you like. 😄
