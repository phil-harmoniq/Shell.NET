# Shell.NET  [![NuGet](https://img.shields.io/nuget/v/Shell.NET.svg)](https://preview.nuget.org/packages/Shell.NET/) [![Build Status](https://travis-ci.org/phil-harmoniq/Shell.NET.svg?branch=master)](https://travis-ci.org/phil-harmoniq/Shell.NET)

Interact intuitively with Bash directly in .NET Core.

## Example

```C#
var bash = new Bash();
bash.Cp("~/.bashrc", "~/Desktop/bashrc-backup");
bash.Grep("export", "~/Desktop/bashrc-backup", redirect: false);

// Commands return a BashResult that stores output information:
if (bash.Rm("~/Desktop/bashrc-backup").ExitCode == 0)
    bash.Echo("Success!");

// With redirect (default in most commands), access the command's output from BashResult.Output:
Console.WriteLine(bash.Cat("~/.bashrc").Output);

// Without redirect, the command's output gets printed to the terminal:
bash.Cat("~/.bashrc", redirect: false);

// BashResult.Lines splits BashResult.Output by new-lines and stores the result as an array:
foreach (var line in bash.Ls("-lhaF").Lines)
    Console.WriteLine(line);

// Generic commands can be made with Bash.Command():
bash.Command("ldd /usr/bin/dotnet", redirect: false);
var dotNetVersion = bash.Command("dotnet --version").Output;
```

## Details

Bash commands return an instance of `BashResult` that stores redirected output information in `BashResult.Output`, `BashResult.ErrorMsg`, `BashResult.ExitCode`, and `BashResult.Lines`. By default, all commands (except for `Bash.Echo()`) will redirect their output information. If a command is run with `redirect: false`, all properties in `BashResult` except for BashResult.ExitCode will be `null`.
