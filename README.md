# Shell.NET  [![NuGet](https://img.shields.io/nuget/v/Shell.NET.svg)](https://www.nuget.org/packages/Shell.NET/) [![Build Status](https://travis-ci.org/phil-harmoniq/Shell.NET.svg?branch=master)](https://travis-ci.org/phil-harmoniq/Shell.NET)

Interact with Bash directly in C#/.NET Core.

## Installation

Use the .NET CLI to get this library! Prereleases need an explicit version.

```bash
dotnet add package Shell.NET -v 0.1.7-alpha
```

Or add the following to your .csproj:

```xml
<ItemGroup>
  <PackageReference Include="Shell.NET" Version="0.1.7-alpha" />
</ItemGroup>
```

## Example

```C#
var bash = new Bash();
bash.Grep("export", "~/.bashrc", redirect: false);

// Commands return a BashResult that stores output information:
if (bash.Rm("~/Desktop/bashrc-backup").ExitCode == 0)
    Console.WriteLine("Success!");

// With redirect (default in most commands), access the command's output from BashResult.Output:
var bashrc = bash.Cat("~/.bashrc").Output;

// Without redirect, the command's output gets printed to the terminal:
bash.Cat("~/.bashrc", redirect: false);

// BashResult.Lines splits BashResult.Output by new-lines and stores the result as an array:
foreach (var line in bash.Ls("-lhaF").Lines)
    Console.WriteLine(line);

// Run custom commands using Bash.Command():
var libs = bash.Command("ldd /usr/bin/dotnet").Lines;
Console.WriteLine($"OS: {bash.Command("uname -s").Output}");
```

## Details

Bash commands return an instance of `BashResult` that stores redirected output information in `BashResult.Output`, `BashResult.ErrorMsg`, `BashResult.ExitCode`, and `BashResult.Lines`. By default, all commands (except for `Bash.Echo()`) will redirect their output information. If a command is run with `redirect: false`, all properties in `BashResult` except for `BashResult.ExitCode` will be `null`.
