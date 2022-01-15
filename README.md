# Building OnlineTipCalculator With ASP.NET Core using C# and Vanilla Javascript
### Goal
The goal of this tutorial is to attempt to walk programmers through the process of building a simple web app however unlike many other tutorials this is targeted at doing its best to implement a number of practical technique and libraries. To achieve that goal this article will walk a programmer through building a MVC(Model-View-Controller) style C# backend. It will also walkthough using the repository patten as well as unit tests for the controller endpoints. The front end will also be built with a mix of vanilla Javascript taking advantage of the Fetch API to hit the endpoints as well as mixing Razor syntax as well. This tutorial has been written with the goal of explaining and detailing out the reasons, purposes and details of its implementation with the goal that developers of any level will be able to follow and add value but specifically the target audience is any developer trying to find a more practical of tools and techniques that go into a full stack ASP.NET Core web application.
### Requirements
These will act as our business requirements for what this application will need and do. They will act as our main driving force for most of this guide. 
```
" Tip Calculator needs to present to a user a calculated result that is 15 % of a bill"
```
# Part 1: Solution Creation and Set Up
The IDE used in this tutorial was Visual Studio For Mac. For the sake of time we're going to avoid going into detail about how to create a project in VS For Mac but if you do need assistance here's a [link](https://docs.microsoft.com/en-us/visualstudio/mac/tutorial-aspnet-core-vsmac-getting-started?view=vsmac-2019) to Microsofts tutorials on making projects with VS For Mac. The one required step I will go over in creating the code is to ensure that you select the "Individual Authentication (in-app) of the Authentication section. <img src="OnlineTipCalculator/Images/Screen%20Shot%202022-01-04%20at%209.27.25%20PM.png"> Please note that while in this tutorial my IDE of choice was VS For Mac you are more than welcome to use a different IDE that you are comfortable with.

### Step 1
Assuming that you are able to create a new ASP.Net Core (Verion 5.0 >) project lets find and review our appsettings.json file. In a real world environment one of the first tasks that needs to be taken care of is ensuring that sensitive information is not committed to a source control repository. Passwords and api keys should never be hard coded into our source code let a lone be committed. Once that is done it is almost impossible to correct that error without destroying the repo altogther. There are a number of technique and ways to solve this problem, the one we will be using in this article is the [Secrets Manager](https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-6.0&tabs=windows) provided by microsoft. We are going to use the secret manager to store the database connection string. In the examples the database used in this project was SQLite which required no password to access but if there was a password the process would be the exact same. We will need to get to a terminal prompt of the target project. This can be done in VSFor Mac by right clicking project and going to tools and selecting either Open in Terminal or Open In Terminal Window. I will be using the Terminal window in this article.
<img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-26%20at%209.31.06%20PM%20(3).png">
<img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-26%20at%209.35.20%20PM.png">

After the terminal prompt is open we are 1st going to make sure we are in the project folder and not the solution folder as the below commands will not work if they are not in the same folder as the OnlineTipCalculator.csproj file. One way to know if you are in the correct folder is to type ls on Mac or Linux and DIR on Windows to show the files in the current folder.<img src="OnlineTipCalculator/Images/Screen%20Shot%202022-01-04%20at%209.42.23%20PM.png">

If your terminal window looks like the above screenshot then you are in the correct folder. If not you may need to change the project folder by typing cd OnlineTipCalculator.If you do the files in the screenshot above then type the command " dotnet user-secrets init " which will allow the project to know to reference the secret manager locally for any values we need referenced by our appsettings file.


Next we want to add an entry for the data we want referenced. This can be done by typing "dotnet user-secrets set "ConnectionStrings:DefaultConnection" DataSource=app.db;Cached=Shared ".
  
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

  One of the features of the ef core ORM are a thing called migration scripts. They are basically code files that are autogenerated and will alter our database for us when executed. To ensure that you have the command line tools install we will need to run the command " dotnet tool update --global dotnet-ef ". This will esnure the the ef command line tools can be access globally across your computer. A migration file can be created using the ef core command line tools. The command do to so is "dotnet ef migrations add <File Name> 
  
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-26%20at%209.55.15%20PM.png">
  
  Once this command finishes you project will get a file created with the datetime pre-pended to the name you provided the command.
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-26%20at%209.58.13%20PM.png">
  This migration is empty and we are going to add the code to execute ourselves but be aware migrations are powerful and if we follow a few other steps later were going to see how code can be autogenerated but for this purpose we can create it ourselves.
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-26%20at%2010.31.56%20PM.png">
  Lets take a minute and go over what is happening and why I did somethings a certain way. The main thing happening here is that the migration file will call the sql function of the migration builder class passed into the function. This function will allow for any sql string we provide to the function to get executed as long as it valid sql. We are also using C# string interpolation and string literal syntax together so that we don't have to be concerned about any escape characters and I can prevent any spelling mistakes on my part between the sql and the data I'm inserting. Something to note here is that I'm using the nameof function in C# versus ToString here. For enums both function will have the same result but behind the scenes the ToString function does a lot in order to achieve a string format and nameof does far less so its more for effeciency but note you could use either one and get the same result. Using the nameof in this case is more of a personal preference for myself. Something else to keep in mind is I created 4 total enum values but only added data for 3 why is that? Due to the nature of enums they start at zero but database do not start at zero. Now I could have inserted the zero enum but I'm using the zero enum to represent the lack of data in this case where in the sql world that is represented as null. C# does allow for a nullable enum type but in my professional opinion developers should avoid too much nullability where they do so logically to reduce the chances of null reference exceptions. In most cases where I've seen a nullable enum used there has to be some null checking done but that same code can easily be switched with code checking for a zero value.
  
 Now the last task for this step is to execute our migrations which can be done using the "dotnet ef database update " command which will take any migrations that haven't be run and apply them to our database as long as the sql is valid. 
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-26%20at%2010.32.34%20PM.png">

  If this runs with no issues then step 2is done and feel free to check the data with the database tool of your choice. 
  
  ### Step 3 Adding Calculations
  So now that our tip type exists we are good to create the model that will reflect the tips we actually calculate. Go ahead and create a new file called Calculation.
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-26%20at%2010.35.05%20PM.png">
 
  Note in the screenshot you can see I originally named the file Calculations with an s at the end but the class is still called Calculation the s was a typo that was too trivial for me to feel to correct for this tutorial. 
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-26%20at%2010.46.33%20PM.png">
 
 In the above screenshot you can see this class or model is very simple since this class in meant to present the results of some query models will rarely have functions or even constructors. There may be good reasons to do this but in general models should avoid having logic in them. That being said to help us have some protection of the state of the models data we are going to use data annotations. If you've not used these before these are some meta data tags that the asp .net core framework will use to enforce other behavoir. You can for example see the [key] tag of the Id property. This will by default let the table created for this model know that there needs to be a column called Id of type int that is the primary key. Since the primary key is an int it will also auto increment for us. The required column translates to a column being not null meaning data must always be present for that column. Range will ensure that a number value can only be between the beginning and ending number range that I've set. In this case the result can only be between 1 and the max of a double. The Max of a double is a massive number that will likely never be practically hit by most systems but in any case this range will enforce no tips of anything below 1 since in our use case that doesnt make sense to allow. Now that our model is created were going to look at our ApplicationDbContext file and make some adjustments.
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-26%20at%2010.57.10%20PM.png">
  
 Now lets take some time and go over the dbcontext. A dbcontext of any type is the backbone of what drives ef core. The dbcontext will be a class that will contain the instructions on how to build our database it's tables if those tables have models that match the table. The 1st thing we need to do is create a property that will be the table representation of our Calculations. Any such tables will need to have a DbSet data type of the model we want to map to that table. By convention the table will always be the pural name of the model as it will be containing multiple rows of that data. We will be overriding the OnModelCreating soe that we can use that to write the instructions for building our table. To do this we need to instruct the builder object that any entity or table of a Calcuation will have 3 properties we want turned into columns and with that we've given enough basic instruction that ef core can build our table for us. Note there are many many powerful features in the ef core library and this is a simple use case for now. Next we are going to execute another models creation like we did earlier. 
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-26%20at%2010.59.13%20PM.png">
  
  However unlike before if we look at the content of the file we'll see something different than before.
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-26%20at%2010.58.30%20PM.png">
 
WOW! look at all that auto generated code. If your at all familiar with creating a table then the code is pretty good at self documentation but you can see a table will be dropped if it exists and created if it doesn't. That table will have 3 columns one will autoincrement and you can see the data type that SQLite will use to represent this data. Keep in mind that since I'm using SQLite in this project the data type conversation may not be the same as yours if you using a different sql engine like MySql or Sql Server. Another important topic I'd like to discuss is going over the migrations after they are made but before they are executed on the database. Each migration is auto-generated and its auto-generated from code you made but as with all auto-generated code it may not follow your instructions in the way you intended it to but it will do so based off what you TOLD it to do just like all code. For example I was created a class that used a Guid. I expected the result column to be a varchar but found out that the SQL server provider I was using had a specific data types for guids that I was unaware of at the time it didn't cause me any issues at the time but I was still very shocked and within as much reason as possible the more you know about what your code is doing the better.
  
Now back to our migration if you remember from before the next step would be to run a command line tool and execute this migration right? Well your wrong actually because I'm going to show you an alternative so that you have an understanding that there is always another way. I actually want you to find and open your StartUp.cs file and go to its Configure method and make some adjustments like I have in the screenshot below.
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-26%20at%2011.00.05%20PM.png">
  
I've added a new argument of the Configure method which will be an instance of the ApplicationDbContext and on line 43 I'm calling the Database Migrate function. What this will do for us is to execute all the migrations that haven't been run on start up of this application and it will even create the database if its found to not exist on that server in the connection string as long as the user account of the web application has rights to do so. From now on we shouldn't need to run any migrations manually for the rest of this project. I will make a word of caution will using the migrate function in start up can solve a number of huge problems for a lot of applications be mindful of adding any heavy database operations that you might not want to occur potential on start up. Ideally they should always only occur once but even then many companies and teams will not allow sql scripts to be ran on start up of a web app. I'mg personally an advocate for it. I've had more instances where having sql that is necessary for an application to work correctly benefits from it being executed on start up. This helps to prevent situations such as developers locally having different database and configuration set ups and helps to main other environments are in sync as well. For example if you have migrations then every developer laptop, stage site or production environment should be on the same database. Do think about taking steps if your code needs to be rolled back which is where I find this to be the most dangerous but even then if your team has a manual process toapply your sql then you'd need to also manually roll the sql back as well and once something is in production regardless of the change lanuage or technologies there should always be the goal of backward compatibility.
# Part 3: Controllers, Repositories, DTOs and Unit Tests
 Ok if you've made it this far thats great next we will be setting up the business logic needed to take request for calculations. In my personal opinion I consider this to be potentially the most complicated section as during this process will be developing in a sort of Test Driven mind set. It won't be pure test driven by any means but during the tutorial there will be a phase where focusing on our tests 1st will make more sense.
### Step 1: Creating a DTO
 So for the 1st step this is going to be simple but I wanted to go over it a bit more in most programming languages a Data Transfer Object is a class who's job is to move data around the application but just not hold data when we desire to save it to the data base as that work is handled by the modal. However C# has a custom data type called records which are built with DTOs in mind. Records help us achieve this goal because records are a type of class who's properties can not be changed after the record object is created. I called them a type of class because behind the scenes they are indeed classes, that being said for the rest of this tutorial I will speak as if they are different things.
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-27%20at%203.02.06%20PM.png">
 As you can see in the screenshot it takes less code to create a valid record than a normal class. There are other ways as well to build a valid record that we will see later but for now this is all we need for step 1.
 ### Step 2: Creating Our Repositories
 
  For those not familiar the Repository Pattern is a pattern where code that will access the database resides in a class called a repository. This pattern is a way to enforce the single reponsibility principle and should do nothing but saving and getting data from your database. Going even deeper the repository should also only have to save and gather data for one feature. For example our current project will only have one feature around calculations so its name will be called the CalculationRepository. There will be user data so I could have potentially created a userRepository but since I had little need to do anything with most of the user data it made little sense for the purpose of this project. To create a repository we are first going to create an interface that our class can inherit from. To be clear while it is not required to use interfaces, interfaces make mocking data for unit test possible as well as allow us to use dependency injection. Go ahead and add a folder called Repositories and add an ICalculationRepository infterface and a class called CalculationRepository which will inherit from ICalculationRepository.
 
<img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-27%20at%203.07.00%20PM.png">
 
 Go ahead and add a SaveCalculation function like you see in the screenshot below once the interface has been created. 
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-27%20at%203.12.35%20PM.png">
  
  No lets create our CalculationRepository class and then implement our interface. 
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-27%20at%203.13.31%20PM.png">
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-27%20at%203.14.20%20PM.png">
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-27%20at%203.14.42%20PM.png">
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-27%20at%203.26.53%20PM.png">
  
  Ok here's were we going to stop for a bit and review what is happening, what we did and most importantly why. So we started by creating our interface but the interface only had one method which is for saving a calculation so it will take a model and return a task of boolean so that callers of this function can get information on if the save operation was a success or not. You can also see that the CalculationRepository class has 2 properties one for the db context and the other is it's logger. Logging is a very undervalued mechanism in many tutorials and lessons but honestly it's critical in production level applications. For security reasons for example you want to log all exceptions and only throw those you have ways to catch. Logging information is also critical for debugging and tracking down issues. Final step is to register the CalculationRepository repository we've created. This is done by opening our projects StartUp file and adding the line of code I've add on line 31 of this screenshot as all the using statement for the repositories folder.
  
 <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-27%20at%208.26.00%20PM.png">
  
### Step 3: Making a Request Object

The next step is a very simple one but we need to create a request object. We created a response object which will represent how the data will look when we return it from a model but we don't have one to represent what it will look like when we get a request. To achieve this we are going to create a record but using a different syntax also so that we can take advantage of something we didn't use in the response object. 
  
<img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-27%20at%204.58.02%20PM.png">
So as you can see I went ahead and used data annotations again for this record because I don't even want to process a request that violates the requirements. We are also using a new annotation called Display which will present a nicer string format for the property instead of the literal column name. This will come in handy when we get to the UI bits.

### Step 4: Working With our Controllers.
Just if your new to controllers, controllers are merely classes that get the job of acting as the middle man between any api request and the data being sent or returned from that request. For the sake of simplicity we are going to start using the default HomeController. Later we will modify this but for now this is a great way to start. So first thing to do is to add the code shown in the screenshot below.
  
<img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-27%20at%204.59.16%20PM.png">
  
You can see that we are keeping this private function pretty simple for now. It takes advantage of the Math.Round function which we get out the box from the System library and it requires 2 things first a number and then a precision for the resulting rounded number. Next we are going to add some properties and a new function to our controller.
  
 <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-27%20at%204.59.57%20PM.png"> 
 
 As you can see we are taking advantage of CalculationRepository we made early and passing a brand new calculation instance into it. You will also see that we are returning both our calculate data and an ok sucessful status code of 200 with the OK object.
  
### Step 5: Unit Tests
Ok now here's when we're going to need a glass of our favorite adult beverage because its time for some unit tests! Let go ahead and create a new test project by right clicking on the solution folder and then adding our new project.
  
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-27%20at%205.02.21%20PM.png"> 
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-27%20at%205.03.06%20PM.png"> 
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-27%20at%205.03.30%20PM.png"> 
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-27%20at%205.09.58%20PM.png"> 
  
  If everything worked correctly you should see something like this file below once the project has finished being made.
  
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-27%20at%205.11.21%20PM.png">
  
  Next we are going to add a nuget package that will allow us to mock data since we don't want to run test against any real data. We can add a nuget package by right clicking our project and
  
 <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-27%20at%205.53.13%20PM.png">
  
 In the browse section go ahead and select the Browse tab and look for Moq which is the name of nuget package library we need to install. Make sure you also accept the license agreement to use this library. 
  
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-27%20at%205.12.34%20PM.png">
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-27%20at%205.54.07%20PM.png">
  
 Now that our unit test project has all the libraries it needs we need are going to add a reference to the main Tip Calculator project. We need to do this because code in different projects can never automatically communicate with one another so we need to add references to other projects in order for projects to access code outside it's own project. Let go ahead and click on the add option like we've done before but select Reference this time. 
  
<img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-27%20at%205.55.18%20PM.png">
  
 After the references prompt shows up go ahead and make sure are OnlineTipCalculator project is checked off and then select Ok.
  
 <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-27%20at%208.27.36%20PM.png">
  
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-27%20at%208.28.03%20PM.png">
  
  This is a section where we're going to slow down and review a lot of the code here. For anyone not used to mocking the term in plan english is merely a process that lets code designed to query a database not use real data but fake or "mock" data. If your new to any library that mocks data one of the core reasons we do this is because honestly its incredibly dangerous to mix test data into your production database. Even when you run your code against a developer or staging copy of your real database you could be polluting your test data in unexpected ways creating problems for yourself and others your not even aware of. You might be asking yourself "well how am I going to properly test my logic works if it doesn't touch my real database?" and that problem is solved with different categories of test called integration tests and even automation test. Those types of tests are meant to be used to mimic behavior between your code and real data. Thats a topic I won't get into much more for this guide but it's a worth while subject to research and understand. 
  
You can observe online 23 one of the main things the Moq library provides us which is the Setup functions. Here it's "setting up" an instance a copy of of the SaveCalcuation function from our repository with mocked data. The code on line 24 in the above screenshot is also a critical part of working with mocks because they don't have the actual data type of the thing they are mocking. Calling the Object property will expose an instance of the data type with the data we mocked. We are also setting that to a property just for the sake of simplicity.
  
 In the actual tests themselves we are then creating new instances of the Home Controller using our test repository and test logger. We then create a new instance of CalculationRequest and set it's properties with some random test data. Next we call the function this test is meant to target which in this case is RunCalculationAsync. Something to be aware of when working with endpoints that return an IActionResult is that they will often need to aliased to some concrete datatype that can inherit from that interface to get any relevant information we can test. In the 1st test I'm simply testing that the end point will return a 200 or OK status code when valid data is sent.  The test below it is very similar and build on a lot of what we did in the preceeding test but in this test we "casting" the value of the result as well and in this case we are vasting the result to a Calculation since that's the class we send in the response. For this test we are only checking to make sure that the result amount will be a number that we expect given our inputs.
  
 Now we get into the real reason this is the hardest section actually. Our requirements for our application are about to change! We get and emergency email from out stakeholders telling us that because of "article 53 of the United State Financial Regulartory Bureau and company that is responsible for any calculations of any sales done by a customer must not be shared and or exposed outside said company. This data must be treated as personaly identifiable information  no different than a social security number as malicious entities can use this data to sell to other parties the spending habit of US citizens"(I 100% made this up but I wouldn't be surprised if you ask a team mate with 5 or more years of experience they will have had to deal with some government rule that sounds like this). Long story short we need to encrypt our calculation results so hackers don't get them.

This situation is the exact reason I feel it is very important to try to get your unit tests done very quickly in your development cycle. In the real word requirements change quickly and very very often. It's extremely unfair and there are many many processes in information technology to prevent this as much as possible but it is something that is not likely to even be truly avoided. Any customer or stakeholder of your application will have a very different mindset of what is reasonable to ask of you and how long things may take. Many of them aren't able to understand the difficulty and time it takes to achieve tasks others honestly simply do not care because they are in some way shape or form paying you for a service and even if they forget to tell you the full requirements they are often still part of the service. Sometimes you can push back on late requirements and fix them at more appropriate times but with something like governement and or security regulations you need to act quickly. Having test that we can run regularly on establish functionality means we are far less likely to break parts of our code we had working accidently in an effort to make adjustments to support new requirements. I personally like to view my software as a living being and every living being needs some type of immune system, some internal mechanism that can protect your inner workings against outside threats.Am I comparing requirement changes to viruses? Yes yes I am you work as long as I have you might too :) 
  
  ### Step 6: Adding Encryption
 
So next step in our guide, now that our requirements have changed will be to add the [DataProtection](https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/using-data-protection?view=aspnetcore-5.0) nuget packages that will help us secure out data. Please note that security is an ever changing topic especially for software. There are many tools that exist to secure software you should always do research before implementing any technology for that person. Getting that out the way open the Nuget package manager and search for DataProtection. For this guide we will be using Microsoft.AspNetCore.DataProtection and Microsoft.AspNetCore.DataProtection.Abstractions.
  
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-27%20at%208.36.20%20PM.png">
 
  Once thos packages have been installed lets next register the service in our StartUp file. 
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-27%20at%208.40.20%20PM.png">
  
  The next step will be to go ahead and change the data type of our Calculation class's ResultAmount. The things we will be storing in the database will now be an encrypted string so the table's data type needs to be updated to support that. 
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-27%20at%208.45.03%20PM.png">
  
 The next few steps are luckily pretty simple at this step as we now need to inject the interfaces for the DataProtector and DataProtectionProvider.
 <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-27%20at%209.02.44%20PM.png">
  
  As you can see in the screenshot above we will need to create a new property for the IDataProtector and inject an DataProtectionProvider into the controller's constructor. In the construtor we will set the value of the Protector from the results of the providers CreateProtector function. It's paramter is whats call a [purpose string](https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/consumer-apis/purpose-strings?view=aspnetcore-5.0). Last step in the implementation is to then simply make a call to the protector's Protect function. It's input is also a string so I once again used string interperlation our result amount into a string before its protected.
  
### Step 7: Asking Question

Asking Question? Why is the section called that? A very important hard lesson when coding up any project for someone is to attempt to use common sense but also take the time to review what you've been asked and find gaps in the literal requests. It's an extremely valuable thought process to begin to read in-between the lines when it comes to your requirements and ask questions. I added this step directly after the requirements where changed because that's a good indication that your customer is unreliable enough that you need to be more aggressive in creating open dialog with them. A very useful question for example to now ask your stakeholder is this "Do you wish for the users to be able to see their previous results?". You then write up an email and send that to your stakeholder and they respond back with an email that reads
  ```
  "Well of course I do. I thought that was clear in the original email"
  ```
  So now we have a new requirement which is, we also need to present our historical results to a user which means we also need to keep track of users. At this point you can write up an email explaining this wasn't clear enough so you didn't build it in such a way but you'll get working on that. Now here's another moment where I'd like you to take a pause and think through this scenario but not as a developer but as a follower of a lesson. Did it at any point to you as a reader of this guide wonder if it would have made more sense to keep track of your users? If you didn't trust me thats ok I promise you as most tutorials are very simple and straightforward so readers often follow them word for word. Most writers of a guide also expect that. They expect that a reader will follow along word for word. Again there is nothing wrong this but the real world is messy and I'd like to challenge you to begin to think of ways to improve any code your writing rather that be for a guide or for a production system. Take a few minutes of pause to review that thought, maybe write it down for later review so as to not derail any progress too much but try to predict some pitfalls you may have with your system early. These maybe pitfalls that make it harder for your to write code or pitfalls that make it harder for your end users to use your system. Thinking of things that may go wrong and trying to prevent them as much as reasonably possible is a great skill to have in ones tool built in any profession. Ok soapbox time is over lets get back to the typey typey.
  
 ### Step 8: Tracking Users
 Now you don't have to worry too much in this case I promise. Being that this is just a guide I'm not trying to drive any one crazy and make re-write your code over and over too much (your customers will handle that part for me :smirk: ). If you followed the first steps of the tutorial and had the (in-app authorization) drop down selected then this solution has been pre-installed a Microsoft library called [IdentityCore](https://docs.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-5.0&tabs=visual-studio) which is built for managing users. With that part out of the way what we will need to do is add 2 new fields to our calculations. One for tracking the userId and the other a datetime of the calculation occuring. 
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-28%20at%2012.46.49%20PM.png">
  
  We will also make a change to our Request object as well. Here we will once again take pause and review. If you look at the Request it's still sending the bill amount as a double and not a string. We're going to leave this as is because the input should still be a NUMBER. Leaving the request object using the most appropriate data type will prevent the need from having to implement any unecessary checking to make sure the data is in the right format.
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-28%20at%2012.47.09%20PM.png">
  
  Before we go any further into updating our solution you may see this error depending on when you follow this guide. 
 <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-28%20at%201.02.47%20PM.png">
  
I was able to solve the issue when writing the guide by merely increasing the version of the Microsoft.Extensions.Logging.Abstractions nuget package to 6.0.0 but when you read this you may not have that issue at all or you may need to upgrade to a different version altogether. 
 <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-28%20at%201.07.16%20PM.png">
  
 I felt this was also an important lesson to address in this article as one of the many challenges of software engineering is finding and or dealing with versions of libraries. Good software is ever changing and evolving and versions change often and suddenly for different reasons so be careful of that in your career. You will often find sometimes code will only work with one version but doesn't play nice with another. 

  If there are no issues building your solution I want you to go ahead and create a new migration by running the below command.
  ```
  dotnet ef migrations add EncryptingResultAndAddingNewFields
  ```
  Since earlier in the project we set up the migration function in the StartUp file we can create the script but don't need to update the database manually as the next run of the application will handle that part.
  
# Part 4: Hooking Up Our UI

  
 ### Step 1: Applying Authorization
 I know I know I said this part was about hooking up our UI and it is I promise but a critical step before building our user interface is making sure our users are logged in before they even use our features. An easy way to handle that in Asp.NetCore is to add an Authorize attribute around a controller.
  
<img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-28%20at%209.17.41%20PM.png">
Once thats complete go ahead and attempt to run the web app. You should see a page like the below screenshot that will require you to log in. 
<img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-28%20at%209.18.32%20PM.png">
  
 If you haven't already go ahead and press the "Register as a new user" link to create a new account and then log in.
 ### Step 2: Creating Our Input Fields and JavaScript Ajax Calls
  
  Ok Ok now is the real UI set up. For this project our UI is going to be built with a mix of C#'s razor syntax and some ES6 Javascript. For those not famaliar Razor syntax can be explained as C# code that is turned into html at run time. It allows for a developer to write dynamic front end code in C#. That being said Javascript is well Javascript. If your building a web application it's going to be very hard to build a system where Javascript won't be apart of it in some way shape or form at least not at the time of writing this guide anyway. 
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-28%20at%2011.06.13%20PM.png">
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-28%20at%2011.07.11%20PM.png">

  With our main form set up we should have everything we need to begin creating requests.
  
  # Part 5: Refactoring and Fixing Our Tests
  
  So you may have by now noticed that these changes have broken our tests. We introduced 2 major breaking changes by adding the data protectionr provider. One the constructor now takes a 3 argument so we will need to address that because it won't build and also the RunCalculationWillRetun15Percent currently won't run because the result is no longer a number but now an encrypted string. Since our tests are broken however this gives us a prime opportunity to refactor our code in ways that make more sense and not just make the test pass. One of the keypoints of our current system that doesn't make much sense is to have our calculations done in the HomeController. The reason for this is because that controller has a responsibility to handle loading of the home page but while our functionality is currently presented on the Home Page our endpoints have a very specific purpose that isn't functionality needed to load the Home page. To address this we will simply be creating a new controller called the CalculationController.
 
  ### Step 1: TDD
 If you've never heard of TTD it stands for Test Driven Development. TDD is a design practice of writing Unit tests that won't pass and then writing enough code to make those tests pass. The value this provides is when you follow this thought process you end up writing far less code you need much earlier in your development process but be aware a pure TDD approach can add a lot of time invested in your development cycle. Manage that investment wisely. We're going to next make adjustments to our Test and have them instead of referencing the HomeController reference a CalculationController even though it doesn't exist. 
  
<img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-29%20at%201.14.47%20PM.png">
 Go ahead and refactor your code to look something like the above screenshot. You will notice we are also now mocking the protection provider logic as well. The code snippet below will also be how I intend for you to "fix" the RunCalculationWillReturn15Percent test so that it runs but I want you to take time again and really think about this test and what this test is now warning us we aren't testing correctly anymore.
  
 ```C#
  [Test]
        public async Task RunCalculationWillReturn15Percent()
        {
            //Arrange
            var expected = "cHJvdGV";
            var controller = new CalculationController(logger, calculationRepository, dataProtectionProvider);
            var TestCalc = new CalculationRequest { Id = 1, BillAmount = 37.50, TipType = TipType.SitDownRestaurant };
            //Act
            var result = await controller.RunCalculationAsync(TestCalc) as ObjectResult;
            var actual = (Calculation)result.Value;
            //Assert
            Assert.IsTrue(actual.ResultAmount.Contains(expected));
        }
  ```
  
 ### Step 2: Creating Our New Controller
  
  The next step is an easy and thats the creation of the new Controller for now however its not doing anything we weren't doing with th HomeController so we are going to be mostly movie code from one place to another. This is another good reason to have unit tests as sometimes you'll have a need to move code but not change any logic but we're humans and sometimes even when moving code we make mistakes so its good to have test to cath if we made mistakes during the move. Go ahead now and right click on the Controllers folder hit add and then New Scaffolding to create our new empty controller.
<img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-29%20at%201.15.18%20PM.png">
<img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-29%20at%201.15.59%20PM.png">
<img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-29%20at%201.16.19%20PM.png">
  
Once the scaffold is complete there should be a controller that looks like the below screenshot with only an Index function in it.
<img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-29%20at%201.19.54%20PM.png">

  If everything has indeed been scaffolded correctly then lets copy some code from our HomeController and move it here. 
<img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-29%20at%201.25.31%20PM.png">
 Feel free to now cut out CalculateTip and RunCalculationAsync function from the HomeController and paste them in the CalculationController. 
 <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-29%20at%201.27.01%20PM.png">
 You may also know I added another check that our data is correct by adding a condition that will check if our request model has all the data we marked as required and if it doesn't we will return a BadRequest status code of 400. 
  
  ### Step 3: Smoke Testing and More Fixes
  Before we run our tests I want you to go ahead and run the web app and attempt to save a request. After the main page opens right click on your page and select the inspect option. I then want you toselect the network tab and then attempt to save a calculation. 
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-29%20at%201.40.21%20PM.png">
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-29%20at%201.41.28%20PM.png">
  
  You should see the above behavior from the screenshots where the alert pops we implemented to let us know things went correctly saying there was a success but see in the network tab there was a 404 failure because the endpoint couldn't be found. Now this is an easy enough problem to solve but I wanted to pause here and bring attention to these are problems because its a good reminder of the different levels of error handling we often need to factor into our systems. We moved our RunCalculationAsync to a new controller so it makes complete sense that our front end can't find as we haven't changed that code yet but our alert's code is wrapped around the section that gets executed if a network call is successful but as long as a response is returned then a network call was successful regardless if that response indicates the work we tried to do was successful. The below adjustments will handle both issues but be mindful with your error messaging.

  These Javascript changes will allow for slightly more accurate error message as well as the correct endpoint route.
<img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-29%20at%201.46.23%20PM.png">
 
  You may still notice that the request will fail however. 
  
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-29%20at%201.46.46%20PM.png">
  
  That's because we didn't tell the new controller to actually respond to request at that route which we will address next by adding the route annotation above our controller to let it know to use its controller name as apart of its network request routes. Once all that is in place we should be able to save responses again under our new controller and all our unit tests should be able to run as well. 
 <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-29%20at%201.47.47%20PM.png">
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-29%20at%201.52.59%20PM.png">
  
  # Part 6: Getting Our History
  
 So we now know we need to be able to show the previous history of all a user calculations. We should have everything we need from a database perspective to achieve this goal so lets start hooking up the different code pieces that will allow us to show the history. We will be mostly using and building off a lot of the same principals we went over in the saving process.
 
  ### Step 1: Setting Up or Repository 
  Go ahead and start us off by writing a new repository function in out ICalculationRepository interface. 
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-29%20at%201.57.36%20PM.png">
  Ok so once that is done go ahead and now implement the interface from our CalculationRepository class.
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-29%20at%201.58.19%20PM.png">
  
  Since this function is only returning data we can easily call that thanks to our db context class as seen below.
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-29%20at%202.00.17%20PM.png">
  ### Step 2: Updating Our Controller and DTOs
  Next step first begins with creating a new DTO. So we have a DTO that is meant to act as a request and send data from our front end client to our controllers so the model can save data. Now because of our implementation of the protection provider we also can't just return the models because the data in encrypted. Also if we are using DTOs we want to have a DTO that is designed with the purpose of returning data. 
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-29%20at%203.56.46%20PM.png">
  Again were creating a record to send the DTO but if you've noticed I'm not concerned with any annotations and I've also left the result amount as a string why you may ask? Because I don't care. Yup I said it just like that for a reason I wanted to get your attention but to be more thorough in my explanation I have no value in adding a display or data annotation in the response because the response will be used to present data and not save any of it. The response has the job to display the data that we have stored in a meaningful way. Key word is meaningful. For example the DTO that creates data sends the users userid along with the save request but the response doesn't return that. The reason for that is that if I'm looking for calculations for a user then the information of the user needs to be in the request but if a caller already has information on the user they want the data for why return the userid if they already know what it is? This is another good reason to use a DTO in that if we don't need to send a piece of data either to or from our endpoints just don't send it. If you have no use for it leave it out of network traffic. 
  
 Once our response object exists we are now going to add our new endpoint.
 <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-29%20at%203.58.40%20PM.png">
  The above code isn't the prettiest by anymeans but it should be just enough to do the job we need and more importantly just enough for us to write some unit tests. Before we get into the tests I want to review two key functions being used in this code base, the Where and Select methods. The methods are apart of Linq library that is extremely commonly used in C# applications. Linq is short for language intergrated query and in simple terms lets you write C# like it was a sql statement. That is a simplification of how these functions work and not the in depth explanation so please do keep that in mind and feel free to review their inner workings in more depth. Another critical point to explain is that the functions paramters are anonymous functions called lambdas. Many programming languages have this syntax however and in short its a way to write a function just without giving it a name. It may be a struggle to read in the early stages of your programming career anonymous functions are extremely popular and powerful because they allow a programming to write and use a function in a single line of code. When I was trying to understand anonymous functions I found it easier to think of them as saying "I want the single character to represent each unique element in a collection and everything to the right of the => is what I want to do with that element". So if I tried to turn just the where clause into english I mgith say "It's returning the collection of unique calculations where the Userid of that calculation is equal to the userid that was provided when this endpoint is called". The Select function also works much like the sql select statement but in C# it opens up a number of use cases we can't do in sql. The select function lets us "project" selected bits of data from the collection to the left of => and because of projection I can turn a collection of calculations into a collection of collection responses. I would highly recommend taking time learning and practicing these functions and for that matter a number of functions in the Linq library. While using Linq lambda's may be hard to read at first the pros provided outweigh the cons.
  ### Step 3: Updating Our Tests
  So  know that we've gotten our endpoint lets go ahead and start testing it. I want you to update our test class's setup function to look like the below screenshot. 
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-29%20at%203.59.14%20PM.png">
  Now as part of our TDD practice if you run this test and have coding everything correctly it should fail because its actually checking to see if the results are empty. I'm having you write a failing test 1st for many reasons but most of which are to let you see the data and go into a deeper conversation about how mocking works as well as finally addressing an issue I've hinted at but haven't dealt with around the calculation private method.
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-29%20at%203.59.58%20PM.png">
  If you look at the screenshot you will see the data in the result amount is the string returned from the mocked function. This is exactly how mocks work the return the data told to them by the programmer making the mock. What this means in regards to what I've been hinting at since the change in our requirements is that even though we had passing tests we haven't actually been testing that the calculation formula returned the correct results. We've gotten tests that provided our data was protected but the data was no longer what we thought. This is a lesson meant to too teach two things. First it's meant to provide a warning when using private methods. Private methods are often something people are told not to test directly because its not exposed. I to different extents firmly disagree with this personally but what I do not disagree with is that fact of why that is. A private method is an inappropriate technique to write code that has logicial decisions or mathematical equations. Due to this I find private methods personally to be a code smell. If you've never heard the phrase a before a code smell is a term used to describe code that indirectly implies itself as solving a a symptom of a problem but not the actual root problem. I've found that while their are common code smells each developer builds their own list of code smells based off their own unique experiences. Private methods are one of mine but do not have to be one of yours they just aren't a good technique to use here.
  
Something a bit better is to use a library dedicated to the work of making the calculations.
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-29%20at%204.03.16%20PM.png">
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-29%20at%204.05.10%20PM.png">
  Once the library file exists feel free to cut the code from our controller that was responsible for the tips results and then paste those into the library. This will allow the  tip's calculation logic to be isolated from the encryption logic and this will now allow us to test that logic indepentitly of anything else.
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-29%20at%204.15.20%20PM.png">
  
  Once that is in place lets go ahead and as we did before make a test project for our library project. A good rule of thumb is to always have one specific test project for each project that is doing some form of business logic. This helps again to respect the concept of single responsibility.
Make sure that you also remember to create a reference the project that is going to be under test. In this case it will be the TipLibrary.
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-29%20at%204.08.25%20PM.png">
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-29%20at%2011.14.12%20PM.png">
  
  And now that our library has its own tests we can rename the test that was getting the controllers results to something more fitting. We don't want to remove it because we do indeed want to confirm its doing what its doing right now so just renaming the existing test is perfectly reasonable. Go ahead and change the tests name so it matches the below screenshot.
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-29%20at%2011.17.02%20PM.png">
   
  ### Step 4: Creating Our History UI 
  So our next step will be to create our actual History UI to show a users past results. To get ourselves started we are going to create a new folder under our Views folder called Calculations. Next right click and create a new index razor page like the below screenshot.
   <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-30%20at%2010.28.18%20PM.png">
  ```c#
  @*
    For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860
*@

@using Microsoft.AspNetCore.Identity
@inject SignInManager<IdentityUser> SignInManager
@inject UserManager<IdentityUser> UserManager

@{ ViewData["Title"] = "History"; }

<h1>History</h1>
<div class="text-center">
    <table class="table table-bordered">
        <thead class="thead-dark">
            <tr>
                <th>#</th>
                <th>Tip Amount</th>
                <th>Date and Time</th>
            </tr>
        </thead>
        <tbody id="calchistory"></tbody>
    </table>

</div>

<input type="hidden" id="UserId" value="@UserManager.GetUserId(User)" />
@section Scripts {
    @{await Html.RenderPartialAsync("_ValidationScriptsPartial");}
    <script>
        let userId = document.getElementById("UserId").value;
        fetch('https://localhost:5001/calculation/history', {
            method: 'post',
            headers: { 'Content-type': 'application/json' },
            body: JSON.stringify(userId)
        })
            .then(response => {
                if (response.status !== 200) {
                    alert("Data Not gathered");
                    return;
                }
                return response.json();
            })
            .then(data => {
                displayCalculations(data);
            })
            .catch(error => {
                console.log(error);
                alert("Data Not gathered");
            })
        function displayCalculations(calcs) {
            let table = '';
            calcs.forEach(calc => {
                table = table + `<tr>`;
                table = table + `<td>${calc.id}</td>`;
                table = table + `<td>$${calc.resultAmount}</td>`;
                table = table + `<td>${calc.createdDateTime}</td>`;
                table += `</tr>`;
            });
            document.getElementById("calchistory").innerHTML = table;
        }
    </script>
}

  ```
  Once thats done we are going to update our layout file a bit to allow us to see a link for the new view we just created. 
    <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-30%20at%2010.34.12%20PM.png">
  
  ### Step 5: Refactoring
  Now that all the heavy lifting is done and we have a good set of unit tests plus our core functionality appears to be working now's a great time to go ahead and start refactoring our code to make it a bit cleaner and more readable.
   <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-30%20at%2010.29.35%20PM.png">
   <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-30%20at%2010.30.14%20PM.png">
  As you can see from the above screenshots the only real changes of note we've added are some better exception handling for our errors so that we can catch and log any errors we get as well as return custom errors. We never want our user interface to display internal error messages as this is a secuirty risk and can expose a lot of information about our systems to malcious users and hackers. We are also updating our RunCalculationAsync endpoint to return a CreatedAtAction instead of an Ok. The reason for this change is that it will return a 201 status which is meant to reflect some form of data was created as a result of the network call which is more accurate to what is occuring in this workflow. Since the return type is changing we do need to make an update to our unit test for this method as well. 
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-30%20at%2010.32.29%20PM.png">
  
  Lastly since our return status is different that should be reflected in our error message check as well. 
  <img src="OnlineTipCalculator/Images/Screen%20Shot%202021-12-30%20at%2010.35.28%20PM.png">

# Closing Statements
  
  So hopefully you were able to follow along with this tutorial and stuck with me to the very end. As we wrap I want to stress as much as possible while I hope you follow some of the standards and guidelines I've mentioned please keep in mind that all software is as much like a living being as any human and while we all have a certain number of traits we share each of the traits can differ either slightly or wildly. No one technique or pattern is the magic bullet to solve all a developers problems. For that reason business level applications are usually made up of various programming languages, libraries, databases, design patterns etc. You will never know them all and nor are you expected too.
