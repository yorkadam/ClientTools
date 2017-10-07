# How To: .Net Core 2.0 Code Examples for HTTP, Databases, Logging, Dependency Injection

## Audience
The intended audience for this article is Microsoft .Net framework software developers and coders wanting to learn how to implement .Net Core 2.0.  Readers are assumed to have working knowledge of prior .net frameworks and C# coding experience.

## Scope
This article shall only cover examples specific to .Net core 2 for demonstration purposes and not the development of a complete application. This article is not a detailed tutorial but rather notation of working examples.

## Topics
Topics covered are as follows: HTTP (post & get), Web scraping, databases (ADO), configuration, logging, and dependency injection.

## Application Type
The example application is a .Net Core 2 console application which implements code for the topics listed in the topic section.

## Intended Uses
The code provided is intended as examples only and is not a “Production ready” application. The intent is to setup and configure specific examples for usage in other application or for simple debug / testing purposes. This application is intended to be run from within the developer tools in “debug” mode. 

## Tool Kit & Items Needed
The items and tools needed to make this code run are as follows:
A PC or computer with sufficient technical specification to run either Visual Studio 2017 or Visual Studio Code and access to the Internet.

## Warranty & Support
As usual; code is provided “AS-IS”. Use and reuse at your own risk. This is provided without warranty of any kind direct or implied.  No support is provided for this code / application in any form.  However, I don't mind answering reasonable questions if I have time.

## License
This code is licensed under the standard MIT open-source license. Free to use as you see fit. Keep in mind you must follow Microsoft's licensing where applicable. 

## Project Type
C# Console Application

# Introduction
The Microsoft .NET Core application framework has been rapidly changing with the latest release at 2.0. (As of this article date). The rapid change has made it difficult to find documentation and examples which made sense and accurately represented working code.

Additionally, the majority of application and code examples were focused towards web based portions of the .Net Core framework such as ASP.NET / MVC / restful services; with very little available on console application and library development.

Official Microsoft documentation is, and continues to be, very poor; and I developed this application to lessen perceived gaps (for me) discovered while researching and learning implementations of the new .net core framework(s).

Topic Summary and Usage Comments

Readers, after reviewing / using this code should have learned the following concepts:

## HTTP and Web

The HTTP examples cover basic POST and GET; accounting for parameterized requests, media type definition, and certificate (SSL) properties. Additionally a working example of how to “scrape-the-web” is included.

### Method(s) HttpGetExample & HttpPostExample

How to implement GET requests which are basic, parameterized, and certificate (SSL) setting specific.
How to implement POST request which are basic, form encoded, and certificate (SSL) setting specific.

### Methods(s) ExtractLinksAndPageText

How to extract text only and links (URIs) from a web page. How to use the HTMLAgilityPack (.net core version). How the new System.Net works and other items.

Key points to be aware of; in this example are; how media types are represented within the new framework as well as request / response handling.  The .net core uses; for web components, a “Fluid API” which is not immediately obvious if your are coming to .net core directly from  standard ASP.NET or older MVC versions.  Additionally, how to properly configure client / server certificate handling use-cases as  you may encounter with invalid certificates or self-signed certificates. 

## Databases and Data

### Method(s) GetRecords...
The get record methods include implementations for Microsoft SQL Server, PostgresSQL, and SQLite. The database examples show how to use ADO/SQLClient to implement general and reusable data methods. I avoided usage of “Data Sets / Data Table” in favor of dictionaries because not all of the data set and data table functionality has been converted by Microsoft to .Net core from the legacy libraries.  I did not include MY SQL because the Oracle .net core libraries seem incomplete and not yet implemented completely to correctly follow the same patterns as the other three databases.

## System and Environment

### Method(s) ReadEnvironmentVariables
This method is only provided as a quick-and-dirty way to show the basic structure of the environment classes and how to read the properties.  I've included the most-common cross-platform-friendly properties but there are may others; much of which, are platform specific.

### Method(s)  WriteToLogs
How to implement alternative logging using the built-in logging tools.  The default logging tools from Microsoft do not include a file-logger and I wanted to provide readers with, one of many, possible ways to implement file loggers. 

### Configuration & Dependency Injection
The examples show how to use the new configuration builder and includes how to implement dependency injection within configuration via services collection extension methods.  The key point here is; dependency injection is not “out-of-the-box” ready for console applications as it is for MVC/WebAPI applications and you must manually implement it.

This example shows how to setup and configure dependency injection in a console application and includes how to setup and configure a logging factory for consumption by dependency injection.  A not-so-obvious point is the extent for which the new .net core components are reusable between project types.

The implementation in this console application is nearly the same as the default setup in a .net core web application. The only difference is that Visual Studio scaffolds start up classes for web applications and does not for console applications.

Additionally, examples are given for one, of several ways; to implement, organize, and read the new appsettings.json configuration files. Users will see how to group and nest configuration settings for better read-ability.

# General Notes
Overall, there is a lot going on in this example application. I wrote it for my specific use-cases and to have as a point of reference on central items that may need to be implemented later in projects I'm working on.

Please keep in mind when reviewing; this code is in a style of “demonstration code” and NOT my normal style of coding as I would do for actual production line of  business enterprise application.  My goal was to provide simple and compact console based application implementation useful for potential web automation tasks and services. For example, building a web crawler, feed-reader services, or perhaps; automated batch data processing from Internet based data providers.

Feel free to add issues for this project on GitHub (or fork it) but keep in mind this is neither a community project nor actively developed consistently and my time is limited. Therefore, I may or may not have time to respond to requests. 

Hopefully, in the future I will have time to turn these examples into a reusable library.

Thanks for taking the time to read this article.
