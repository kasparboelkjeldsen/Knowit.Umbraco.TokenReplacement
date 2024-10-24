# Knowit.Umbraco.TokenReplacement

Ever wanted to use Umbracos translations in a text field or rich text editor? Now you can!

Token Replacement will replace any {{your.dictionary.item}} with the corresponding translation at runtime. Also throug Content Delivery API

**TokenReplacement** currently supports Umbraco 10 to 13.

## Installation

You will need to add our middleware as the last step in the pipeline. This is done by adding the following line to your `Program.cs` or `Startup.cs` file:

```csharp
app.UseUmbraco()
    .WithMiddleware(u =>
    {
        u.UseBackOffice();
        u.UseWebsite();
        u.AppBuilder.UseMiddleware<TokenReplacementMiddleWare>(); // this one!!!
    })
    .WithEndpoints(u =>
    {
        u.UseInstallerEndpoints();
        u.UseBackOfficeEndpoints();
        u.UseWebsiteEndpoints();
    });
```

It is important that it is the last middleware in the pipeline, as it needs to be able to replace the tokens in the response body after Umbraco has already processed it.

## How to use

After installation, simply type "{{" in a text field or rich text editor, and you will see a list of all available tokens. Select the token you want to use, and it will be replaced with the corresponding translation at runtime.

## Configuration options

In case you don't wish to include your entire dictionary in the options for Token Replacement, you can limit the options by inserting the following in your appsettings

```json
"Knowit.Umbraco.TokenReplacement": {
    "TokenKey": "token."
  },
```

After which, Token Replacement will only replace tokens that start with `token.` or whatever you choose.