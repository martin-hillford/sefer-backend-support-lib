#!/bin/bash
rm -rf ./build
dotnet restore
dotnet build --no-restore --configuration Release
dotnet pack --no-build --configuration Release --output build