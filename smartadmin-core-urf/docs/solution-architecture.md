# SmartAdmin for ASP.NET Core 3.1 - Documentation

## Table of Contents

1. **[Introduction](introduction.md)**
1. **[Getting Started](getting-started.md)**
1. **[Site Structure](site-structure.md)**
1. **Solution Architecture**
1. **[How To Contribute](howto-contribute.md)**
1. **[Licensing Information](licensing-information.md)**
1. **[Changelog](changelog.md)**

---

## Solution Architecture

The SmartAdmin for ASP.NET Core Theme is targeting ASP.NET Core 3.1 and has been verified to work with Visual Studio 2019 Community, Professional and Enterprise, and, depending on how comfortable you are with your development tools, can also be used with Visual Studio Code.

### Dependencies

SmartAdmin for ASP.NET Core relies on the following frameworks:

- **Bootstrap 4.4**: Responsive layouts on mobile devices and beyond
- **FontAwesome 5**: A vast library of scalable vector icons
- **jQuery 3**: ubiquitous JavaScript library that supports all major browsers
- **SmartAdmin 4.4**: The heart and back-bone of this template

### Cloud services

The ASP.NET Core project was designed with hosting on Microsoft Azure in mind, but can easily be expanded to suit another cloud platform, such as AWS. The following services were used for this application when deploying to Azure:

- **Azure App Service**: For providing serverless hosting of the .NET Core web application (on Unbuntu!)
- **SQL Serverless**: Serverless data storage of relational data used by the application when it is used
- **Azure Storage**: Storage of uploaded data and/or static assets

> **Note:** An on-premise database scenario is still supported and can be easily configured by adjusting the connection string and data provider within the Startup of the Application!

Please see the [Changing the Data Store](customization.md#Datastore) page for more details on how to configure this part of the the Application.

## Authentication and Authorization

SmartAdmin for ASP.NET Core uses **ASP.NET Core Identity** for providing support for common authentication scenario's. Not only does this give you an out of the box secure experience but also showcase how we were able to have these two frameworks work together and co-exist as they were shaped to match the **SmartAdmin 4** Theme look and feel.

All of this, resting comfortably on the back of [EntityFramework Core](https://docs.microsoft.com/en-us/ef/core/) which has been setup with Migration support to get you started!

> **Note:** Storing the data is not limited to any specific database, however the project was written with `SQLite` as the assumed storage provider and therefor should not pose any restriction on using cloud-based services that provide SQL Server instances or other editions of SQL Server or MySql.

## Configuration

The project relies on configuration settings at runtime, such as whether to use a local database or a Azure SQL Database for data storage, whether to load sample data, default account information and/or determining which theme sections are visible by default. These setting values can also be stored in the `appSettings.json` file. However, doing this could make it easier to accidentally expose secrets, so please be aware of who has access to this information.

> **Important:** When you publish the project to Azure or any other hosting provider, you should take care to protect these values.

Currently the project includes values for a fixed set 'section' toggles in the configuration through the use of an `ActionFilter`. You could of course modify this setup to load from a database and/or Authorization based settings instead but for the sake of simplicitly and demonstration we have decided to utilize the `ActionFilter` approach.

> Please see the [Application Features](#Features) section for more details on how to manage this part of the the Application.

## Template Structure

The application makes heavy use of both [Sections](https://docs.microsoft.com/en-us/aspnet/core/mvc/views/layout?view=aspnetcore-3.1#sections) and [Partials](https://docs.microsoft.com/en-us/aspnet/core/mvc/views/partial) which are both an intricate part of the Razor engine for ASP.NET Core. We have ensured that these names match those of the HTML Theme so that any information regarding them is still applicable and relevant for the .NET Core Flavor.

### Layout

The main layout of the Theme is defined in `/Views/Shared/_Layout.cshtml`.

```html
<!DOCTYPE html>
<partial name="_CopyrightHeader"/>
<html lang="en">
  <head>
    <partial name="_Head"/>
    @RenderSection("HeadBlock", required: false)
  </head>
  <body class="mod-bg-1 mod-nav-link @ViewBag.PreemptiveClass">
    <partial name="_ScriptsLoadingSaving"/>
    <div class="page-wrapper">
      <div class="page-inner">
        <partial name="_LeftPanel"/>
        <div class="page-content-wrapper">
          <partial name="_PageHeader"/>
            <main id="js-page-content" role="main" class="page-content">
                <partial name="_PageBreadcrumb"/>
                <div class="subheader">
                    <partial name="_PageHeading"/>
                    @RenderSection("SubheaderBlock", required: false)
                </div>
                @RenderBody()
            </main>
          <partial name="_PageContentOverlay"/>
          <partial name="_PageFooter"/>
          <partial name="_ShortcutModal"/>
          <partial name="_ColorProfileReference"/>
        </div>
      </div>
    </div>
    <partial name="_ShortcutMenu"/>
    <partial name="_ShortcutMessenger"/>
    <partial name="_PageSettings"/>
    <partial name="_GoogleAnalytics"/>
    <partial name="_ScriptsBasePlugins"/>
    @RenderSection("ScriptsBlock", required: false)
  </body>
</html>
```

### Features

Each `partial` can be controlled by their respective setting in the `/Models/ViewBagFilter.cs` file:

> **Note:** When using **Razor Pages** the `controller` below is replaced by `pageModel`.

```cs
  // SmartAdmin Toggle Features
  controller.ViewData["AppSidebar"] = Enabled;
  controller.ViewData["AppHeader"] = Enabled;
  controller.ViewData["AppLayoutShortcut"] = Enabled;
  controller.ViewData["AppFooter"] = Enabled;
  controller.ViewData["ShortcutMenu"] = Enabled;
  controller.ViewData["ChatInterface"] = Enabled;
  controller.ViewData["LayoutSettings"] = Enabled;
```

Simply change `Enabled` to `Disabled` and the partial content will not be rendered. As an alternative and/or when you simply do not wish to include the content/feature, just remove the `partial` tag from the `_Layout.cshtml` file.

> **Note:** `ViewData` properties are also available inside your `.cshtml` pages by using the `ViewBag.Foo` syntax, however since using `dynamic` can have a negative impact on rendering performance it is only available when **MVC** is used.

### Content

The majority of the included files are a direct representation of the associated page in the HTML Theme of SmartAdmin. However, page specific **style** and/or **script** files are loaded in through the **sections** defined in the `_Layout.cshtml` page.

> **/Views/FormPlugins/Select2.cshtml**

```js
@section HeadBlock {
  <link rel="stylesheet" media="screen, print" href="~/css/formplugins/select2/select2.bundle.css">
}

@section ScriptsBlock {
  <script src="~/js/formplugins/select2/select2.bundle.js"></script>
  <script>
    $(document).ready(function () {
        $('.select2').select2();
    });
  </script>
}
```

### Routing

The routing between MVC and Razor Pages is kept the same. This means that `/foo/bar` is routed to the same equivalent content page for both technologies. Routing for MVC is handled by the **Action** methods of each `Controller` class inside the **/Controllers/** folder. When Razor Pages is used however the route is determined based on conventions, and where needed can be specified as part of the `@page` directive on the first line of the `.cshtml` file.

> **Note:** Automatic highlighting of the current menu item is done by inspecting the current route. This applies to both the parent and child menu item.

---

Copyright &copy; 2020 by Walapa. All rights reserved. This documentation or any portion thereof
may not be reproduced or used in any manner whatsoever without the express written permission of the publisher except for the use of brief quotations in a review.