# VS WebLinks

An extension for Visual Studio that enables you to quickly copy a link to a
selection in a file hosted in a remote Git repository.

## Features

- Open or copy a link to selected text in a file from the Visual Studio editor context menu.

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
