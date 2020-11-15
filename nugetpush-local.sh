#!/bin/sh

version="$1"

dotnet pack source/core/Kyameru.Core/Kyameru.Core.csproj -p:PackageVersion=$version

dotnet nuget push source/core/Kyameru.Core/bin/debug/Kyameru.Core.$version.nupkg -s http://localhost:9011 --api-key password1.