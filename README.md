# Building OnlineTipCalculator With ASP.NET Core using C# and Vanilla Javascript
### Goal
The goal of this tutorial is to attempt to walk programmers through the process of building a simple web app however unlike many other tutorials this is targeted at doing its best to implement a number of practical technique and libraries. To achieve that goal this article will walk a programmer through building a MVC(Model-View-Controller) style C# backend. It will also walkthough using the repository patten as well as unit tests for the controller endpoints. The front end will also be built with a mix of vanilla Javascript taking advantage of the Fetch API to hit the endpoints as well as mixing Razor syntax as well. This tutorial has been written with the goal of explaining and detailing out the reasons, purposes and details of its implementation with the goal that developers of any level will be able to follow and add value but specifically the target audience is any developer trying to find a more practical of tools and techniques that go into a full stack ASP.NET Core web application.
# Part 1: Solution Creation and Set Up
The IDE used in this tutorial was Visual Studio For Mac. For the sake of time we're going to avoid going into detail about how to create a project in VS For Mac but if you do need assistance here's a [link](https://docs.microsoft.com/en-us/visualstudio/mac/tutorial-aspnet-core-vsmac-getting-started?view=vsmac-2019) to Microsofts tutorials on making projects with VS For Mac but you are more than welcome to use a different IDE that you are comfortable with.
### Step 1
Assuming that you are able to create a new ASP.Net Core (Verion 5.0 >) project lets find and review our appsettings.json file. In a real world environment one of the first tasks that needs to be taken care of is ensuring that sensitive information is not committed to a source control repository. Passwords and api keys should never be hard coded into our source code let a lone be committed. Once that is done it is almost impossible to correct that error without destroying the repo altogther. There are a number of technique and ways to solve this problem, the one we will be using in this article is the Secrets Manager provided by microsoft. We are going to use the secret manager to store the database connection string. In the examples the database used in this project was SQLite which required no password to access but if there was a password the process would be the exact same. We will need to get to a terminal prompt of the target project. This can be done in VSFor Mac by right clicking project and going to tools and selecting either Open in Terminal or Open In Terminal Window. I will be using the Terminal window in this article.
<img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-26%20at%209.31.06%20PM%20(3).png">
<img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-26%20at%209.35.20%20PM.png">

After the terminal prompt is open type the command " dotnet user-secrets init " which will allow the project to know to reference the secret manager locally for any values we need referenced by our appsettings file.

<img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-26%20at%209.36.37%20PM.png">
Next we want to add an entry for the data we want referenced. This can be done by typing "dotnet user-secrets set "<key>" "<value to save>" ".
  
<img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-26%20at%209.36.37%20PM.png">
  
Once this is complete go ahead and remove the connection string from te project and if everything was done correctly your project will run without the actual string in your source code.
  
<img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-26%20at%209.39.32%20PM.png">
<img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-26%20at%209.42.02%20PM.png">
 
# Part 2:  Models and Working With Ef Core
Next up were going to be configuring and working with models and setting up our database. For those who aren't familiar a model is a term to describe a class that will act as a representation of data in a database. A simple way to think of what a model is for is to take some database query, either stored procedure of raw select statement and for each column returned from that query you will want your class to have a property to contain that data. A type of tool that goes hand and hand with models are called ORMs short for Object Relation Mappers. These are libraries that are responsible for executing some form of sql and doing the work to map data to a property. There are many different ORMs that exist and they are not exclusive to C# and many different programming languages have access to ORMs. PHP for example uses PDO an ORM built directly into the PHP programming language. For this example were going to use Microsoft provided ORM Ef Core. 
### Step 1 Making our Tip Types
The 1st thing we will actually be creating is an enum short for enumeration. Enums are custom datatype like classes or structs but they are far simplier. They are named valued numbers they will by default start at 0 automatically and will be assigned a number value automatically in the order of the enum name creation. Long story short they are just strings that gets numbers assigned to them and thats really just it. Enums are good alternatives to any constant string value as they can always be casted to either a proper string or an interger. Some other good use cases for enums are to represent any form of category your business logic may need because categories are often very short lists and rarely change. For our use case we are going to have each tip given a particular "type" to be detail out what the tip was for. 
  
 A new enum can be created by righting clicking the Models folder hitting the add new file option and choosing the Empty Enumeration option from the general section.
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-26%20at%209.48.32%20PM.png">
   Once the TipType.cs is create feel free to paste in the following code snippet.
  ```C#
 using System;
namespace OnlineTipCalculator.Models
{
    public enum TipType
    {
        NA = 0,
        FastFood = 1,
        SitDownRestaurant = 2,
        Bar = 3
    }
}

  ```
  ### Step 2 Adding our Types to Our Database
  
To be clear step 2 is not "mandatory" but in my experience it is a extremly valuable best practice. So when working with enums the data used by an enum will used from within the C# code. Our code files will not need to access a database to know what the names or numbers mean our web app will have all it needs so why add anything to the database at all you might ask? We're not adding it to the database for the web app but for everyone else. When the enums are saved into a database that are saved by C# in there number value by default as thats their truest form and their most effcient form as well but a TipType of 1 or 2 is nothing something a human can determine the meaning of purely from the data and the definition of the numbers is hidden in the code meaning other developers or database admins won't know how to work with that data without access to the code base. Trust me you always want good descriptions of the meaning of some data in the same place as the data itself. When data is not well defined or that definition is not easy to find you open yourself up for future bugs. Programmers or dbas will write some bit of code or sql expecting a particular set of numbers that works fine in development or some staging environment only to deploy to prod find some other number value exists that they didn't find in dev or stage environment. I could go on with use cases but I hope by now you get the reason why so lets get into the how.

  One of the features of the ef core ORM are a thing called migration scripts. They are basically code files that are autogenerated and will alter our database for us when executed. A migration file can be created using the ef core command line tools. The command do to so is "dotnet ef migrations add <File Name> 
  
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-26%20at%209.55.15%20PM.png">
  
  Once this command finishes you project will get a file created with the datetime pre-pended to the name you provided the command.
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-26%20at%209.58.13%20PM.png">
  This migration is empty and we are going to add the code to execute ourselves but be aware migrations are powerful and if we follow a few other steps later were going to see how code can be autogenerated but for this purpose we can create it ourselves.
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-26%20at%2010.31.56%20PM.png">
  Lets take a minute and go over what is happening and why I did somethings a certain way. The main thing happening here is that the migration file will call the sql function of the migration builder class passed into the function. This function will allow for any sql string we provide to the function to get executed as long as it valid sql. We are also using C# string interpolation and string literal syntax together so that we don't have to be concerned about any escape characters and I can prevent any spelling mistakes on my part between the sql and the data I'm inserting. Something to note here is that I'm using the nameof function in C# versus ToString here. For enums both function will have the same result but behind the scenes the ToString function does a lot in order to achieve a string format and nameof does far less so its more for effeciency but note you could use either one and get the same result. Using the nameof in this case is more of a personal preference for myself. Something else to keep in mind is I created 4 total enum values but only added data for 3 why is that? Due to the nature of enums they start at zero but database do not start at zero. Now I could have inserted the zero enum but I'm using the zero enum to represent the lack of data in this case where in the sql world that is represented as null. C# does allow for a nullable enum type but in my professional opinion developers should avoid too much nullability where they do so logically to reduce the chances of null reference exceptions. In most cases where I've seen a nullable enum used there has to be some null checking done but that same code can easily be switched with code checking for a zero value.
  
 Now the last task for this step is to execute our migrations which can be done using the "dotnet ef database update " command which will take any migrations that haven't be run and apply them to our database as long as the sql is valid. 
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-26%20at%2010.32.34%20PM.png">

  If this runs with no issues then step 2is done and feel free to check the data with the database tool of your choice. 
