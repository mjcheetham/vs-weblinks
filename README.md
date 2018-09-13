# VS WebLinks

An extension for Visual Studio that enables you to quickly copy a link to a
selection in a file hosted in a remote Git repository.

## Build status

[![Build Status](https://dev.azure.com/mjcheetham/vs-weblinks/_apis/build/status/mjcheetham.vs-weblinks)](https://dev.azure.com/mjcheetham/vs-weblinks/_build/latest?definitionId=4)

## Features

- Open or copy a link to a file from the Solution Explorer context menu..
  ![image](https://user-images.githubusercontent.com/5658207/44086843-d899f03a-9fb5-11e8-8c53-c20ed564e3ff.png)
  ..or from a selected region of text in the editor
  ![image](https://user-images.githubusercontent.com/5658207/43273053-5e4c8f5c-90f3-11e8-9a77-882aa868189c.png)

  ![image](https://user-images.githubusercontent.com/5658207/44086962-29cc2afe-9fb6-11e8-8dd3-6b32d02b75ee.png)

### Supported Git Hosting Providers

- [x] GitHub
- [x] Visual Studio Team Services
- [ ] ..more to follow...

## Building

### Prerequisites

Note: Windows only (sorry! it requires the Visual Studio SDK to build)

- Visual Studio Community 2017
  - Workloads:
    - .NET Desktop Development
    - Visual Studio Extensibility

### Compile and debug

To build from the command line just run the following from the root of the repository:

```
$ nuget restore
$ msbuild
```

To install and debug the VSIX from within Visual Studio, open the solution file and hit F5. That's it!
