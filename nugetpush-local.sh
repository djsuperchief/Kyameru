#!/bin/sh

version="$1"

dotnet build build/Kyameru.sln -c Release

dotnet pack source/core/Kyameru.Core/Kyameru.Core.csproj -c Release -p:PackageVersion=$version

dotnet nuget push source/core/Kyameru.Core/bin/release/Kyameru.Core.$version.nupkg -s http://localhost:9011 --api-key password1.
