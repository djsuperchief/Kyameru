#!/bin/sh

dotnet pack source/core/Kyameru.Core/Kyameru.Core.csproj

dotnet nuget push source/core/Kyameru.Core/bin/debug/Kyameru.Core.0.0.1.nupkg -s http://localhost:9011 --api-key password1.