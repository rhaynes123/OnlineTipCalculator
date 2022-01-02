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
