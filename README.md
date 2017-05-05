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

You first visit [/stck](http://stck.azurewebsites.net/stck) to start a computation. This will respond with a JSON-object containing your new STCK-computation and a collection of links. The links contain all the possible tokens you can add to your STCK expression. You can for instance follow the link to [/stck/contexts/FFBECBFBA01644216AD5841A990338D3/tokens/zero](http://stck.azurewebsites.net/stck/contexts/FFBECBFBA01644216AD5841A990338D3/tokens/zero) to add `zero` to the current expression. Note how the `expression` part of the JSON-response now contains a zero. Add another `zero`to the expression by visiting [/stck/contexts/F40BB63810269123C1853105AD84F92B/tokens/zero](http://stck.azurewebsites.net/stck/contexts/F40BB63810269123C1853105AD84F92B/tokens/zero), and note that you now have two zeroes in your expression.

Now we want to send the expression to the STCK-interpreter. This is done using [/stck/contexts/EEF0655ABDFE9DFD73207563DD704CBA/tokens/eval](http://stck.azurewebsites.net/stck/contexts/EEF0655ABDFE9DFD73207563DD704CBA/tokens/eval). Note that now the `expression` part of the response is empty, and that the `stack` part contains `[0, 0]`.

To recap, you follow the links to add STCK-operators (tokens) to your expression, and then evaluate your expression by following the `eval`-link.

### Where did all the numbers go?

In order to be able to return a finite number of links to new computation states, the number of supported numbers are reduced to one number: `zero`.
In order to construct different numbers you can use `zero`, along with the subroutine `onemore`. To get the number 3, you would for instance use `zero onemore onemore onemore`. In addition negative numbers can be constructed using two numbers and the `sub` subroutine.

### What about conditionals, subroutine declaration, math operators and other stuff?

Since a lot of characters used in vanilla STCK needs to be URL-encoded, they are replaced with words in REST-STCK. For instance is `<a boolean> ? <this will happen if true> : <this will happen if false> ;` replaced with `<a boolean> if <this will happen if true> else <this will happen if false> end`, and `# <expression>` is replaced with `defn <expression>`.

Operators are mapped according to the following rules:
* add = +
* sub = -
* mult = *
* gt = >
* lt = <
* idiv = i
* if = ?
* else = :
* drop = .
* end = ;
* mod = %
* defn = #
* bwap = swap
* meep = dup
* jump = over
* mop = rot
* howlong = len
* eq = =
* opposite = not
* 2meep = 2dup
* div = /


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

### TODO

There is a bug where execution contexts does not get an unique hash. It's probably because ToSting() is not the best way to get all information about the context object as input for the hash function...

Are there any sample programs?
------------------------------

Absolutely!

### (Almost) Generating all fibonacci numbers below ten

First, we define some numbers.

`defn one zero onemore eval`
http://stck.azurewebsites.net/stck/contexts/FFBECBFBA01644216AD5841A990338D3/tokens/defn
http://stck.azurewebsites.net/stck/contexts/345EED2712C4F314B4470C5B0E3A7807/tokens/one
http://stck.azurewebsites.net/stck/contexts/798977EE1B68A6BE0C706DFF1D2FEF70/tokens/zero
http://stck.azurewebsites.net/stck/contexts/28C0D8B46AA7E6D9CF65F2021A0A6817/tokens/onemore
http://stck.azurewebsites.net/stck/contexts/623B8FF53F405764560DE5FB0CF05BED/tokens/eval

`defn two one one add eval`
http://stck.azurewebsites.net/stck/contexts/A75AF36D0848632B8B4F26AC835A9A5B/tokens/defn
http://stck.azurewebsites.net/stck/contexts/6758BCCB8DC8374AE7E370DD6E386BF0/tokens/two
http://stck.azurewebsites.net/stck/contexts/4DD6A2E1A87D8940FE39884D6BCE662D/tokens/one
http://stck.azurewebsites.net/stck/contexts/0788C4316E4018B559C9E9FC576AF314/tokens/one
http://stck.azurewebsites.net/stck/contexts/9F643E0D691AF00A217E1FCEEAA3CB6D/tokens/add
http://stck.azurewebsites.net/stck/contexts/BA95DA0DB12A6D0CBB68766BDD7E0781/tokens/eval

`defn ten two two two two two add add add add eval`
http://stck.azurewebsites.net/stck/contexts/6650EA1FE1DAAD7D264CB7BEAF932B8F/tokens/defn
http://stck.azurewebsites.net/stck/contexts/22889EAC05FF3DF233672EB4DC65694B/tokens/ten
http://stck.azurewebsites.net/stck/contexts/CA90D20E4B26F97E5A78D85506A711BE/tokens/two
http://stck.azurewebsites.net/stck/contexts/EE1A96ABF230B25035C85C4F87B1F0B9/tokens/two
http://stck.azurewebsites.net/stck/contexts/A20826BFC72274D3387B9DDCD69B79A2/tokens/two
http://stck.azurewebsites.net/stck/contexts/FD620D98F37CAB5095617DBE9333A018/tokens/add
http://stck.azurewebsites.net/stck/contexts/55C8F2376DC789D1D858600A2B1A5AA0/tokens/add
http://stck.azurewebsites.net/stck/contexts/9D96F11DD5C796B8573B0F167A2D9645/tokens/two
http://stck.azurewebsites.net/stck/contexts/D55AC5E015A331236BB289AD48F2451D/tokens/add
http://stck.azurewebsites.net/stck/contexts/B267EA962CEAE5E04F9A2A760376D57B/tokens/two
http://stck.azurewebsites.net/stck/contexts/099DB437D7FCB7E76016B6AB191EE068/tokens/add
http://stck.azurewebsites.net/stck/contexts/B267EA962CEAE5E04F9A2A760376D57B/tokens/eval

Yeah, the first one didn't go all that well...

`defn rten ten two add eval`
http://stck.azurewebsites.net/stck/contexts/314A0740BB489C4F9DABFBAC7A4E59AD/tokens/defn
http://stck.azurewebsites.net/stck/contexts/2838EB8488B56E245C70529AB0AA6311/tokens/rten
http://stck.azurewebsites.net/stck/contexts/676A54BAD0DD94CE3A743767F1A630B7/tokens/ten
http://stck.azurewebsites.net/stck/contexts/A24FB02B77388B6032F5BC41131FF7EC/tokens/two
http://stck.azurewebsites.net/stck/contexts/FE38FE1BDC7F62E459DACBD7F8EF2894/tokens/add
http://stck.azurewebsites.net/stck/contexts/5D13EA078440AA8887226268A7305BBE/tokens/eval

We'll need to generate the next fibonacci number.

`defn next_fib 2meep add eval`
http://stck.azurewebsites.net/stck/contexts/2227F5B95BCDED07EC6336E6F4E66374/tokens/defn
http://stck.azurewebsites.net/stck/contexts/8A77D41389A34EEA4D801A058DFB3A37/tokens/next_fib
http://stck.azurewebsites.net/stck/contexts/1ABAC99034FE5D272D8F98C06DBB4EE9/tokens/2meep
http://stck.azurewebsites.net/stck/contexts/1C8A6B9BF29EA3CC5B0E172FC78712B4/tokens/add
http://stck.azurewebsites.net/stck/contexts/563A754919DF39891885D36072023667/tokens/eval

Why do we need this again?

`defn is_even meep two mod zero eq eval`
http://stck.azurewebsites.net/stck/contexts/2EAA5CC8A9C6390FA6A1B5689F43D9F1/tokens/defn
http://stck.azurewebsites.net/stck/contexts/242A9B4A47B619395B2526592E78FEBD/tokens/is_even
http://stck.azurewebsites.net/stck/contexts/DCF53404AFA58DAD600994BA353A15A5/tokens/meep
http://stck.azurewebsites.net/stck/contexts/FFAD709D3552FCCCD9B70E05019D7A69/tokens/two
http://stck.azurewebsites.net/stck/contexts/CD61D3949474EB895B69DD30B6A3A085/tokens/mod
http://stck.azurewebsites.net/stck/contexts/92D0B943B155471EBD122E76E6D9A098/tokens/zero
http://stck.azurewebsites.net/stck/contexts/6E6F85DF43C73E6034535312B676ECA8/tokens/eq
http://stck.azurewebsites.net/stck/contexts/D7BBE12A15922B3F003136FCAF89E789/tokens/eval

`defn next_is_zero meep zero eq eval`
http://stck.azurewebsites.net/stck/contexts/FA7EC150A96C27C8DFA2FD6BF22FF4AC/tokens/defn
http://stck.azurewebsites.net/stck/contexts/E8631846A726E029F3F49ECB183393A2/tokens/next_is_zero
http://stck.azurewebsites.net/stck/contexts/917FADFD54361A05AC31A85EDFFE8B44/tokens/meep
http://stck.azurewebsites.net/stck/contexts/8C66FC625C6E0B1DB491305B4AD5F1C6/tokens/zero
http://stck.azurewebsites.net/stck/contexts/D1DC544EFE33207DD24BB4C8C98CCD85/tokens/eq
http://stck.azurewebsites.net/stck/contexts/6BB94F9CA53C07AEC457291F725E54F5/tokens/eval

Well, back to the fibonacci numbers, a little recursion would help.

`defn fib_under_ten next_fib meep rten gt if fib_under_ten else drop end eval`
http://stck.azurewebsites.net/stck/contexts/E23E6F1CA3C91CD891F52CA141F2A9B0/tokens/defn
http://stck.azurewebsites.net/stck/contexts/2250D6BB2C4716C2DADE81DE4BD0FCD2/tokens/fib_under_ten
http://stck.azurewebsites.net/stck/contexts/99629A6013E3CD7E1C41EFB294985ADB/tokens/next_fib
http://stck.azurewebsites.net/stck/contexts/84F4E0931B56D9F57717C91B782E7279/tokens/meep
http://stck.azurewebsites.net/stck/contexts/6C5859C279F0F10EACD0B8E1992D3882/tokens/rten
http://stck.azurewebsites.net/stck/contexts/F5F1D76B0402E95B8C75720DEF2E5A85/tokens/gt
http://stck.azurewebsites.net/stck/contexts/908B021D590AABDE89D873932ECA2A4C/tokens/if
http://stck.azurewebsites.net/stck/contexts/0A770D5CFC877F984C10EF1711A6BB79/tokens/fib_under_ten
http://stck.azurewebsites.net/stck/contexts/43D84C0BF439268101BA343E46AAF827/tokens/else
http://stck.azurewebsites.net/stck/contexts/22F827F74188B2A3A142AE582254DFA1/tokens/drop
http://stck.azurewebsites.net/stck/contexts/578B75FB539558C6DE0FF9A19CFE99B9/tokens/end
http://stck.azurewebsites.net/stck/contexts/235ABBB686152FD86EFE5FC6F372CB17/tokens/eval

And finally, tying it all together!

`zero one two fib_under_ten eval`
http://stck.azurewebsites.net/stck/contexts/F9D6296E3BC9BACB8104D18E12D42781/tokens/zero
http://stck.azurewebsites.net/stck/contexts/F2E43F9DA5CC5D87AFA81412C8DA4999/tokens/one
http://stck.azurewebsites.net/stck/contexts/CD2968978DEB8E519E6AF47CB4F8465C/tokens/two
http://stck.azurewebsites.net/stck/contexts/1A0AC312D207DAD48B76C184035C12EC/tokens/fib_under_ten
http://stck.azurewebsites.net/stck/contexts/A8B95608F40373EEA4BDC7579225D290/tokens/eval