# Create TestResults directory if it doesn't exist
if (-not (Test-Path -Path "TestResults")) {
    New-Item -ItemType Directory -Path "TestResults"
}

# Check if reportgenerator is installed
$reportGeneratorInstalled = (Get-Command reportgenerator -ErrorAction SilentlyContinue)

if (-not $reportGeneratorInstalled) {
    Write-Host "reportgenerator not found. Installing..."
    dotnet tool install --global dotnet-reportgenerator-globaltool
} else {
    Write-Host "reportgenerator is already installed."
}

# Run tests and generate coverage report
dotnet test --collect:"XPlat Code Coverage"

# Find the coverage file
$coverageFile = Get-ChildItem -Path . -Recurse -Filter "coverage.cobertura.xml" | Select-Object -First 1

if ($coverageFile) {
    # Generate the HTML report
    reportgenerator -reports:$coverageFile.FullName -targetdir:"TestResults" -reporttypes:"Html"
    Write-Host "Test coverage report generated in TestResults folder."
} else {
    Write-Host "Could not find coverage file. Report generation skipped."
}
