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

`EntroBuilder` relies on a concept called `IGenerator<T>`:

    interface IGenerator<T>
    {
        T Next(Random random);
    }

Similar to `IEnumerator<T>`, `IGenerator<T>` also produces a sequence of `Ts`, be it that the latter is considered to be an infinite sequence. Some `IGenerator<T>` implementations use a finite collection to produce values, however when the internal sequence has reached the end, it wraps around, and starts from the beginning.

This interface is the core of the library, and any and all types are constructed in this fashion.


## Usage

Every instance of test data can be constructed using the Builder Factory method:

    var builder = Builder.Create<Order>();

The former statement creates an instance of `Builder<T>` that is configured using default *type generators* that construct scalar types.

There are *type generators* for most primitives as well as some complex types: `bool`, `byte`, `DateTime`, `decimal`, `double`, `float`, `Guid`, `short`, `int`, `long`, `string` as well as their `Nullable` counterparts.

Any unsupported types can be registered on the builder instance using the `For()` method.  e.g.:

    var builder = Builder.Create<Order>()
                         .For<ushort>(new MyUShortGenerator());

This also allows to override default generation strategies for built-in types.

The real power of EntroBuilder comes from being able to define the possible values for properties on non-scalars throughout an object graph.

	var builder = Builder.Create<Order>()
                         .Property(o => o.Customer.FullName.FirstName, Any.ValueIn("John", "Bob", "Will"))
                         .Property(o => o.Customer.FullName.LastName, Any.ValueIn("Appleseed", "Builder", "Shakespear");

`Any.ValueIn` is a factory that returns a `Generator<T>` where T is a string, in this example.

    var builder = Builder.Create<Order>()
                         .Property(o => o.OrderLines.Select(ol => ol.Amount, Any.ValueBetween(0.01M, 1.99M));

