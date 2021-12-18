dotnet test /p:CollectCoverage=true /p:CoverletOutputFormat=cobertura /p:CoverletOutput=./reports/ /p:ExcludeByAttribute=\"Obsolete,GeneratedCode,CompilerGenerated\" /p:SkipAutoProps=true /p:DoesNotReturnAttribute="DoesNotReturnAttribute"
REM reportgenerator -reports:./reports/coverage.net5.0-windows.cobertura.xml -targetdir:./reports/html/
REM cd reports/html/
REM index.htm