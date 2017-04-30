REST-STCK
=========

Arbitrary Computation as a Service (ACaaS)

What is this?
-------------

Imagine a wonderful world where turing-complete computation could be preformed through elegant REST APIs.
No longer just a fantasie, REST-STCK introduces REST-based computing to the masses.

Based on the award-winning, enterprise-grade programming language [STCK](https://github.com/teodoran/stck), frontend developers can now leverage the power of Arbitrary Computation as a Service (ACaaS).

Try it out at [stck.azurewebsites.net/stck](http://stck.azurewebsites.net/stck)


So how do I use this thing?
---------------------------

REST-STCK let's you preform STCK-computation through a REST API, using [HAL - Hypertext Application Language](http://stateless.co/hal_specification.html) to support HATEOAS.

### Come again?

First, read the [STCK readme](https://github.com/teodoran/stck#using-the-languague), and make sure you understand stack-based programs and basic STCK syntax.

All done? Then let's continue.

You first visit [/stck](http://stck.azurewebsites.net/stck) to start a computation. This will respond with a JSON-object containing your new STCK-computation and a collection of links. The links contain all the possible tokens you can add to your STCK expression. You can for instance follow the link to [/stck/contexts/6DE86D212068CBB0FBB77C431A0F40FB/tokens/zero](http://stck.azurewebsites.net/stck/contexts/6DE86D212068CBB0FBB77C431A0F40FB/tokens/zero) to add `zero` to the current expression. Note how the `expression` part of the JSON-response now contains a zero. Add another `zero`to the expression by visiting [/stck/contexts/6DE86D212068CBB0FBB77C431A0F40FB/tokens/zero](http://stck.azurewebsites.net/stck/contexts/6DE86D212068CBB0FBB77C431A0F40FB/tokens/zero), and note that you now have two zeroes in your expression.

Now we want to send the expression to the STCK-interpreter. This is done using [/stck/contexts/6DE86D212068CBB0FBB77C431A0F40FB/tokens/eval](http://stck.azurewebsites.net/stck/contexts/6DE86D212068CBB0FBB77C431A0F40FB/tokens/eval). Note that now the `expression` part of the response is empty, and that the `stack` part contains `[0, 0]`.

To recap, you follow the links to add STCK-operators (tokens) to your expression, and then evaluate your expression by following the `eval`-link.

### Where did all the numbers go?

In order to be able to return a finite number of links to new computation states, the number of supported numbers are reduced to one number: `zero`.
In order to construct different numbers you can use `zero`, along with the subroutine `onemore`. To get the number 3, you would for instance use `zero onemore onemore onemore`. In addition negative numbers can be constructed using two numbers and the `sub` subroutine.

### What about conditionals, subroutine declaration, math operators and other stuff?

Since a lot of characters used in vanilla STCK needs to be URL-encoded, they are replaced with words in REST-STCK. For instance is `<a boolean> ? <this will happen if true> : <this will happen if false> ;` replaced with `<a boolean> if <this will happen if true> else <this will happen if false> end`, and `# <expression>` is replaced with `defn <expression>`.

Determining which operators have been replaced whit what words are left as an exercise to the reader. 


Let's say I want to play with this on my own computer
-----------------------------------------------------

### Requirements

You'll need [.Net Core](https://www.microsoft.com/net/core) to compile and run the project.

### Installation

First, clone the project:

```
$> git clone https://github.com/teodoran/rest-stck.git
```

Then run the following commands to restore all nuget packages, build, and run the rest-stck server.

```
$> cd ./RestStck
$> dotnet restore
$> dotnet run
```

Now you should be able to navigate to localhost:5000/stck and try it out.

### Project structure

The core STCK-interpreter is located under /CoreStck. This is a F# project based on the vanilla STCK-interpreter.
Under /RestStck, you'll find the api-server. This is a C# project based on ASP.NET Core
