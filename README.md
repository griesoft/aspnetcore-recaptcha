# ASP.NET Core reCAPTCHA
A Google reCPATCHA validation wrapper service for ASP.NET Core. With only a few simple setup steps you are ready to block bots from filling in and submitting forms on your website.

This package also supports reCAPTCHA V3, but at the moment does not provide any frontend tag helpers for that. So only backend validation is supported right now.

[![Build Status](https://dev.azure.com/griesingersoftware/ASP.NET%20Core%20Recaptcha/_apis/build/status/jgdevlabs.aspnetcore-recaptcha?branchName=master)](https://dev.azure.com/griesingersoftware/ASP.NET%20Core%20Recaptcha/_build/latest?definitionId=17&branchName=master)
[![License](https://badgen.net/github/license/jgdevlabs/aspnetcore-recaptcha)](https://github.com/jgdevlabs/aspnetcore-recaptcha/blob/master/LICENSE)

## Quickstart

The first thing you need to do is to sign up for a new API key-pair for your project. You can follow [Google's guide](https://developers.google.com/recaptcha/intro#overview), if you haven't done that yet.

After sign-up you should now have a **Site key** and a **Secret key**. Make note of those, you will need them for the next step.

### Configuration

Open your `appsettings.json` and add the following lines:

```json
"RecaptchaSettings": {
    "SiteKey": "<Your site key goes here>",
    "SecretKey": "<Your secret key goes here>"
}
```

Make sure to place your site & secret key in the right spot. You risk to expose your secret key to the public if you switch it with the site key.

For more inforamtion about ASP.NET Core configuration check out the [Microsoft docs](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/configuration/?view=aspnetcore-3.1).

In your `Startup.cs` you now need to add the service. Add the following line `services.AddRecaptchaService();` into the `ConfigureServices(IServiceCollection services)` method, like this for excample.

```csharp
public void ConfigureServices(IServiceCollection services)
{
    // Add other services here
    
    services.AddRecaptchaService();
    
    // Add other services here
}
```

### Adding a reCAPTCHA element on your view

First you will need to import the tag helpers. Open your `_ViewImports.cshtml` file and add the following lines:

```razor
@using GSoftware.AspNetCore.ReCaptcha

@addTagHelper *, GSoftware.AspNetCore.ReCaptcha
```

Now you are ready to use the tag helpers in your views. Always add the `<recaptcha-script>` tag on the bottom of your view. This will render the script tag which will load the reCAPTCHA.js API.

Next you only need to add a `<recaptcha>` tag in your form and you are all set. This is the most simplest way of adding reCAPTCHA to your views. Now you only need to add backend validation to the controller of your view.

### Adding backend validation to an action

Add a using statement to `GSoftware.AspNetCore.ReCaptcha` in your controller. Next you just need to the `[ValidateRecaptcha]` attribute to the action which is triggered by your form.

```csharp
using GSoftware.AspNetCore.ReCaptcha;
using Microsoft.AspNetCore.Mvc;

namespace ReCaptcha.Sample.Controllers
{
    public class ExampleController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [ValidateRecaptcha]
        public IActionResult FormSubmit(SomeModel model)
        {
            // Will hit the next line only if validation was successfull
            return View("FormSubmitSuccessView");
        }
    }
}
```

Now if validation would fail, the action method would never get called.

You can configure that behaviour and a lot of other stuff globally at startup or even just seperatly for each controller or action.

### Addition information

For more detailed usage guides check out the wiki. You can find guides about additional configuration options, response validation behaviour, explicit rendering of tags, invisible reCAPTCHA elements and the usage of reCAPTCHA V3.
