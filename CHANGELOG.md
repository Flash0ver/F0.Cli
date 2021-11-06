# F0.Cli
CHANGELOG

## vNext
- Added target framework: `.NET 5`.
- Added annotations for _nullable reference types_.
- Added option binding of _native-sized integers_, both `nint` and `nuint` (`System.IntPtr` and `System.UIntPtr`).
- Added option binding of `System.Half` (applies to `.NET 5.0` _or greater_).
- Changed _.NET Generic Host_ Builder extension methods to require the _Assembly_ containing the CLI _Commands_ to be passed (rather than using the entry assembly), or specify a type argument from that "Command-Assembly" instead.
- Package: Embed icon (fixed _NuGet Warning NU5048_), keep fallback icon URL.
- License: Changed to _MIT_ License.

## v0.5.0 (2020-10-27)
- Added option binding of _built-in numeric types (C#)_ and `System.Numerics.BigInteger`.
- Updated dependency on Generic Host from `Microsoft.Extensions.Hosting 3.1.0` to `Microsoft.Extensions.Hosting 3.1.9`.

## v0.4.0 (2019-12-31)
- Updated dependency on Generic Host from `Microsoft.Extensions.Hosting 3.0.0` to `Microsoft.Extensions.Hosting 3.1.0`.

## v0.3.0 (2019-10-31)
- Updated dependency on Generic Host from `Microsoft.Extensions.Hosting 2.2.0` to `Microsoft.Extensions.Hosting 3.0.0`.

## v0.2.0 (2019-05-31)
- Added cancellation of commands through the host's lifetime.
- Package: Use license expression instead of deprecated license URL (fixed _NuGet Warning NU5125_).

## v0.1.0 (2019-04-21)
- Added command-based programming model with support for dependency injection, arguments binding, options binding, exit code communication.
- Added command pipeline, executed as a long running background task.
- Added command-line arguments parsing, inspired by the .NET Core command-line interface command structure (verb, arguments, options).
- Added reporting abstraction, with a console implementation provided by dependency injection.
- Added Generic Host configuration extensions, read from assembly attributes (environment, application name).
