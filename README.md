# EntroBuilder and EntroTester #
## What are they?

### EntroBuilder
Is a library that facilitates generation of random test data. Through a fluent interface it allows to define generation strategies for certain types, or for  a specific property explicitly within an object graph.

Combining randomness and tests, creates the risk of having non-deterministic tests. For this reason, the default random seed used is a fixed value and it is configurable. As such, consecutive test runs should yield the same results.

### EntroTester
Is a library that leverages `EntroBuilder` for automated blackbox testing. Random testing allows you to discover branches in your code which do not have expected behavior.

This technique is also known as [Fuzz testing](http://en.wikipedia.org/wiki/Fuzz_testing "Fuzz testing"):

> **Fuzz testing** or **fuzzing** is a software testing technique, often automated or semi-automated, that involves providing invalid, unexpected, or random data to the inputs of a computer program. The program is then monitored for exceptions such as crashes, or failing built-in code assertions, or for finding potential memory leaks.

The idea is that `EntroTester` provides a random seed to `EntroBuilder` to verify failure of certain assertions. In case a failure is detected, an exception is thrown with the random seed, and iteration at which the failure occurred. This enables the developer to replay and debug the failure scenario.

## Installation

The packages `EntroBuilder`, `EntroTester` and their dependency `FARE` are currently hosted on the following feed: [http://www.myget.org/F/entrotester/](http://www.myget.org/F/entrotester/)

Add it to your NuGet package sources as explained in the [myget docs](http://docs.myget.org/docs/how-to/register-myget-feeds-in-visual-studio "myget docs").

Then install either package from the package manager console. (EntroTester automatically includes EntroBuilder)

    Install-Package EntroBuilder

If you just want the test data builder.

    Install-Package EntroTester

If you want the entire package.

## Concepts

### IGenerator\<T\>
`EntroBuilder` relies on a concept called `IGenerator<T>`:

    interface IGenerator<T>
    {
        T Next(Random random);
    }

Similar to `IEnumerator<T>`, `IGenerator<T>` produces a sequence of `Ts`, be it that the latter is considered to be an infinite sequence. Some `IGenerator<T>` implementations use a finite collection to produce values, however when the internal sequence has reached the end, it wraps around, and starts from the beginning.

This interface is the core of the library, and any and all types are constructed in this fashion.

`EntroBuilder` offers a number of different generators:

- `ScalarGenerator<T>` is a base type for all the primitive generators. There are *type generators* for most primitives as well as some complex types: `bool`, `byte`, `DateTime`, `decimal`, `double`, `float`, `Guid`, `short`, `int`, `long`, `string` as well as their `Nullable<T>` counterparts.
- `SequenceGenerator<T>` takes an `IEnumerable<T>` and produces uniformly distributed random values from the sequence. The issue with `IEnumerable<T>` is that it can represent an infinite sequence, and since we want to produce uniformly distributed values, we need to determine whether it's a finite or infinite sequence. 
- `RangeGenerator<T>` takes an upper and lower bound and produces random values between the two bounds. Implementations exist for most countable primitives.
- `RegexGenerator` takes a regular expression and leverages [FARE](https://github.com/moodmosaic/Fare "FARE") to produce random strings that match the regex pattern.

### Builder\<T\>
The Builder is a class that does the actual work of binding a Type to a `IGenerator<T>`. It equally offers a way to override the default generation strategy via `For()` and `Property()`:

- `For<T>(IGenerator<T>)` allows to override the default generation strategy for given type. All instances of given type will be generated using this generator.
- `Property<TProperty>(Expression<Func<T, TProperty>>, IGenerator<TProperty>)` allows to override the generation strategy for a specific property within the object graph.


## Usage

### IGenerator\<T\>
The factory class `Any` allows to easily produce instances of the aforementioned generators. Following methods are available:

    Any.ValueIn<T>(params T[]) // Produces a SequenceGenerator<T>
    Any.ValueBetween(int, int) // Produces a RangeGenerator<int>
    Any.ValueLike(string)      // Produces a RegexGenerator

The factory class `Is` allows to produce a generator for a static instance.

    Is.Value<T>(T)             // Produces a SequenceGenerator<T> with one element.

*TODO: Elaborate on `CustomGenerator<T>`*


### Builder\<T\>
#### Creating an instance

Every instance of test data can be constructed using the Builder Factory method:

    var builder = Builder.Create<int>();

The builder can produce instances of `int` by either calling `Build()`, either `Take(int)`.

    var myInteger = builder.Build();   // Produces an Integer instance
    var myIntegers = builder.Take(10); // Produces 10 Integer instances

#### Overriding default generation strategy

The following creates an `Order` for which all properties of type `int` will have a value between `0` and `10`.

    var order = Builder.Create<Order>()
                       .For<int>(Any.ValueBetween(0, 10))
                       .Build();

#### Overriding generation strategy for specific property

The following creates a `Customer` for which the `FirstName` will either be John, Bob or Will, and the `LastName` will either be Appleseed, Builder or Shakespear.

	var order = Builder.Create<Order>()
                       .Property(
                           o => o.Customer.FullName.FirstName, 
                           Any.ValueIn("John", "Bob", "Will"))
                       .Property(
                           o => o.Customer.FullName.LastName,
                           Any.ValueIn("Appleseed", "Builder", "Shakespear"))
                       .Build();

The following creates an `Order` for which the `OrderLines` will have an Amount between 0.01 and 1.99.
    
    var order = Builder.Create<Order>()
                       .Property(
                           o => o.OrderLines.Select(ol => ol.Amount),
                           Any.ValueBetween(0.01M, 1.99M))
                       .Build();

*TODO: Elaborate*