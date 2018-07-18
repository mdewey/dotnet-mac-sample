# Welcome


## Table of Contents

- [Intro](https://github.com/mdewey/dotnet-mac-sample#intro)
- [Required Software](https://github.com/mdewey/dotnet-mac-sample#required-software)
- [Setting up the Project](https://github.com/mdewey/dotnet-mac-sample#setting-up-the-project)
- [Viewing all Shops](https://github.com/mdewey/dotnet-mac-sample#viewing-all-shops)
- [Adding a Coffee Shop](https://github.com/mdewey/dotnet-mac-sample#bonus-adding-a-coffee-shop)


## Intro
Welcome to learning about .NET Core on a Mac with Mark Dewey. Together we will creating a simple app that allows us to save our favorite coffee shops. This will help explore the topics of tooling and MVC using ASP.NET Core. 

This is meant to go hand and hand with the Crash Course `.NET on a Mac`. If you are trying this out own your own, feel free to reach out to Mark @ mark@suncoast.io if you have issues or questions


## Required Software

// Somthing tomseting links.....

## Setting up the Project

First we need to make a new project. This can be done with the new CLI. 

Open up a terminal, navigate where you want to create the site, create a new folder and run :

```
dotnet new mvc
```

this will give us a new, blank starter project. You can run it using: 

``` 
dotnet run
```

Take a few seconds to checkout what all the starter gave you. 

## Viewing all Shops
Lets update the scaffolding homepage to display a list of coffee shops. This means we need to do a couple of things. We need to 
```
- Add a data source
- query the data source 
- pass the data from the controllers to the view
- Display the Items

```

Let's approach this one at a time 

### Add a data source
To keep our stack simple, we will use an in memory database. .NET Core comes with Entity Framework Core. EF Core is the ORM created to work with .NET Core. 

In order to use an ORM, like EF Core, we first must Model our data using POCOs. In the `Models` folder, create a new file called `CoffeeShop.cs` and paste in the follow code:

>IMPORTANT: Update the namespace from `dotnet_mac_sample` to your namespace. You can find your namespace in your `Startup.cs` file


```C#
namespace dotnet_mac_sample.Models
{

    public class CoffeeShop
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Image { get; set; }
        public string Address { get; set; }
        public int Price { get; set; }
        public int Rating { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
```

To use EF Core we need to add a `DataContext` to our project and register the new `DataContext` with our web app. The `DataContext` is an in code representation of your database. We will running our queries against this later.  

Next, we need to create a new `DataContext`. In the `Models` folder, Create a new file called `CoffeeContext.cs` and paste the follow code:

>IMPORTANT: Update the namespace from `dotnet_mac_sample` to your namespace. You can find your namespace in your `Startup.cs` file


```C#
using Microsoft.EntityFrameworkCore;

namespace dotnet_mac_sample.Models
{
    public class CoffeeContext : DbContext
    {
        public CoffeeContext(DbContextOptions<CoffeeContext> options)
            : base(options)
        {
        }

        public DbSet<CoffeeShop> CoffeeShops { get; set; }
    }
}
```

Once that is created, open `Startup.cs`. Most of this file will look like goobly-gook. Feel free to look around. Most of the names are good enough to make a pretty good guess to what it does. This file is what configures our web site once we run it. 

We need to add two lines here.

First, we need to add a `using` statement. `using` is telling our code to pull in code from some place else. With the other using statements, add a reference to your `Models` namespace and to EF Core. It should look like this:

>IMPORTANT: Update the namespace from `dotnet_mac_sample` to your namespace. You can find your namespace in your `Startup.cs` file

```C#
using Microsoft.EntityFrameworkCore;
using dotnet_mac_sample.Models;
```

Next, in the method `ConfigureServices` add: 


```C#
 services.AddDbContext<CoffeeContext>(opt =>
                          opt.UseInMemoryDatabase("CoffeeShops"));
```

This registers our `DataContext` to our services to injected into our controllers that depend on our context. 

Run your add again using `dotnet run` to verify everything is building correctly. "Correctly" means that are not compiler errors


### Query the data source 
Now that we have our datasource set up, we can use it. Head over to the `Controllers` folder and open `HomeController.cs`. This is were we will be using our `DataContext` to query our datasource. In order to use our context, we need to add a `using`. Like before, add this using with the other `usings`:

>IMPORTANT: Update the namespace from `dotnet_mac_sample` to your namespace. You can find your namespace in your `Startup.cs` file

```C#
using dotnet_mac_sample.Models;
```

.NET uses dependency injection to help manage resources. Add this chunk of code before the method called `Index`. 

```C#
// Allows us to have access to the database across all routes
private readonly CoffeeContext _context;

// Saids "This controller needs a CoffeeContext to run"
public HomeController(CoffeeContext dbContext)
{
    this._context = dbContext;

    // Populates our database with 1 coffee shop if there are none. 
    // There are more eloquent ways to seed a database in .NET Core, today we are keeping it simple
    if (this._context.CoffeeShops.Count() == 0)
    {
        this._context.CoffeeShops.Add(new CoffeeShop
        {
            Address = "In the toy chest, when Andy isn't looking",
            Rating = 5,
            Name = "Woody's Coffee", 
            Image ="https://i.pinimg.com/236x/1f/ef/22/1fef22de7108aee735ed502c5b3bb771--disney-characters-disney-princesses.jpg"
        });
        this._context.SaveChanges();
    }
}
```

Next we need to update our `Index` route to pass the data from the `Controller` to the view. Replace the `Index` code with the following:

```C#
public IActionResult Index()
{
    return View(this._context.CoffeeShops.ToList());
}
```

Run your add again using `dotnet run` to verify everything is building correctly. "Correctly" means that are not compiler errors

### Display the Items

This brings us to needed to display the data. This is the `View` part in `MVC`. Inside the `Views` folder, look in the `Home` folder and open `Index.cshtml` This the Razor that will be rendered into HTML. Replace the entire file with this:

```HTML
@model IEnumerable<CoffeeShop>

    @{ ViewData["Title"] = "My Favorite Coffee Shops"; }

    <div class="jumbotron">
        <h3>
            @ViewData["Title"]
        </h3>
    </div>



    <div class="row">
        @foreach(var shop in Model){

        <div class="col-sm-6 col-md-4">
            <div class="thumbnail">
                <img src="@shop.Image" alt="...">
                <div class="caption">
                    <h3>@shop.Name</h3>
                    <p>@shop.Address</p>
                    <p>@shop.Rating / 5</p>
                </div>
            </div>
        </div>

        }
    </div>
```

What did we do?

- We added a strongly typed Model to tell the compiler, this page needs a List of Coffee Shops to work. 
- Using Razor, we are looping over the Model and display a simple bootstrap based html structure

>NOTE: the form will not work yet. that is next...


## BONUS: Adding a Coffee Shop

Congratulations! You have created a simple data driven .NET Core site. But a read only site is nice, lets upgrade our site to let users input data as well. 

We need to make a few changes. 

```
- Add a form to our UI
- Add a route that accepts the POST
- Add Data to to the data source
- Show the updated home page
```

First, Add this form to `Index.cshtml` page. Anywhere you want:

``` HTML
<form asp-controller="Home" asp-action="Add" method="POST" class="form-inline">
    <div class="form-group">
        <label for="Name"> Name:</label>
        <input type="text" name="Name" placeholder="add new coffee shop.">
    </div>
    <div class="form-group">
        <label for="Address">  Address:</label>
        <input type="text" name="Address">
    </div>
    <div class="form-group">
        <label for="Image"> Image Ur:</label>
        <input type="text" name="Image">
    </div>
    <div class="form-group">
        <label for="Rating"> Rating:</label>
        <select class="form-control" name="Rating">
            <option selected>1</option>
            <option>2</option>
            <option>3</option>
            <option>4</option>
            <option>5</option>
        </select>

    </div>

    <button class="btn btn-primary btn-lg" type="submit"> Add One </button>

</form>
```

This is doing a `POST` back to our server. That means our server will need to accept the `POST`. Open up `HomeController.cshtml` and add the following code:


```C#
 [HttpPost]
public async Task<IActionResult> Add(CoffeeShop newShop)
{
    if (newShop != null){
        this._context.Add(newShop);
        await this._context.SaveChangesAsync();
        return this.RedirectToAction(nameof(this.Index));
    } else {
        return this.RedirectToAction(nameof(this.Index));
    }
}
```

What did this code add?

- It enables our server to accept a `POST` request at the URL `/home/add` 
- It looks at the incoming data and creates a new coffee shop object. 
- Adds this object to our data source.
- redirects the user back to the `Index` page.




## Feedback

If you have any feedback or questions about what we covered here, let me know via email/twitter, or open up a pull request or an issue. 

```
email: [mark@suncoast.io](mailto:mark@suncoast.io)
twitter: @juggler2009
```


---
made with 💜 @ SDG
