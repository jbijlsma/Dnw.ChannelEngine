# Introduction

Package containing pubsub messages.

# Build & publish

To build the nuget package:

```shell
git tag 0.0.1
dotnet pack

```

And to push the nuget package to the local nuget server:

```shell
dotnet nuget push -s http://localhost:5555/v3/index.json -k NUGET-SERVER-API-KEY ./.publish/Dnw.ChannelEngine.Messages.0.0.1.nupkg
```

Add source to nuget:

```shell
dotnet nuget add source --username jbijlsma --password NUGET-SERVER-API-KEY --store-password-in-clear-text --name local "http://localhost:5555/v3/index.json"
```

Some other useful dotnet nuget commands:

```shell
dotnet nuget remove source local
dotnet nuget list source 
```