$currentConfig = 'Release'
$packageVersion = '0.1.0'

Remove-Item -Path "artifacts\*" -Recurse
dotnet nuget locals all --clear

function Remove-BinObjFirstLevel {
    Get-ChildItem -Path . -Directory | Where-Object {
        $_.Name -notmatch '\.git'
    } | ForEach-Object {
        $binPath = "$($_.FullName)\bin"
        $objPath = "$($_.FullName)\obj"

        if (Test-Path -Path $binPath) {
            Remove-Item -Path $binPath -Recurse -Force
            Write-Output "Removed bin folder: $binPath"
        }

        if (Test-Path -Path $objPath) {
            Remove-Item -Path $objPath -Recurse -Force
            Write-Output "Removed obj folder: $objPath"
        }
    }
}

# Execute the function to remove bin and obj folders in the first level of subfolders
Remove-BinObjFirstLevel
Push-Location Myna.TheFatChicken/src
Remove-BinObjFirstLevel
Pop-Location

dotnet build Myna.Weaver/Myna.Weaver.csproj -c $currentConfig

dotnet pack Myna.Build/Myna.Build.csproj -c $currentConfig -p:PackageVersion=$packageVersion -o artifacts
dotnet pack Myna.API/Myna.API.csproj -c $currentConfig -p:PackageVersion=$packageVersion -o artifacts
dotnet pack Myna.TheFatChicken/src/Moq/Moq.csproj -c $currentConfig -p:PackageVersion=$packageVersion -o artifacts
