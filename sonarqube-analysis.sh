#!/bin/sh

dotnet sonarscanner begin /k:"kyameru" /n:"kyameru" /d:sonar.login=f50d5f16d6c3958333240100e2c29dd1bc02e122 /d:sonar.host.url=http://localhost:9000 /d:sonar.cs.vstest.reportsPaths="/**/*.trx" /d:sonar.cs.opencover.reportsPaths="**/coverage.opencover.xml"
dotnet build build/Kyameru.sln
dotnet test tests/UnitTests/Kyameru.Tests/Kyameru.Tests.csproj --logger trx /p:CollectCoverage=true /p:CoverletOutputFormat=opencover /p:CoverletOutput=coverage
dotnet sonarscanner end /d:sonar.login=f50d5f16d6c3958333240100e2c29dd1bc02e122