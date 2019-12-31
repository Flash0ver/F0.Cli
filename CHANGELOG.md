# F0.Cli
CHANGELOG

## vNext

## v0.4.0 (2019-12-31)
- Updated dependency on Generic Host from `Microsoft.Extensions.Hosting 3.0.0` to `Microsoft.Extensions.Hosting 3.1.0`.

## v0.3.0 (2019-10-31)
- Updated dependency on Generic Host from `Microsoft.Extensions.Hosting 2.2.0` to `Microsoft.Extensions.Hosting 3.0.0`.

## v0.2.0 (2019-05-31)
- Added cancellation of commands through the host's lifetime.

## v0.1.0 (2019-04-21)
- Added command-based programming model with support for dependency injection, arguments binding, options binding, exit code communication.
- Added command pipeline, executed as a long running background task.
- Added command-line arguments parsing, inspired by the .NET Core command-line interface command structure (verb, arguments, options).
- Added reporting abstraction, with a console implementation provided by dependency injection.
- Added Generic Host configuration extensions, read from assembly attributes (environment, application name).
