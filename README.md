# gitignore-cli
 A command-line tool to create gitignore files

[![NuGet](https://img.shields.io/nuget/v/gitignore-cli.svg)](https://www.nuget.org/packages/gitignore-cli)
[![Releases](https://img.shields.io/github/release/nuskey8/gitignore-cli.svg)](https://github.com/nuskey8/gitignore-cli/releases)
[![license](https://img.shields.io/badge/LICENSE-MIT-green.svg)](LICENSE)

## Overview

gitignore-cli is a CLI tool for creating .gitignore files from templates published at [github/gitignore](https://github.com/github/gitignore).

## Installation

You can install gitignore-cli using the .NET Core Global Tool.

```bash
$ dotnet tool install --global gitignore-cli
```

## Usage

| Command | Description |
| - | - |
| gitignore list | List available repository gitignore templates. |
| gitignore new \<template\> | Create new .gitignore file. |
| gitignore view \<template\> | View .gitignore file. |

## License

This library is under the [MIT License](./LICENSE).