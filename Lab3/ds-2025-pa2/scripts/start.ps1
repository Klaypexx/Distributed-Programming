docker start my-nginx

docker start my-redis

docker start rabbitmq-pa3
Start-Sleep -Milliseconds 3000

$scriptDir = Split-Path -Path $MyInvocation.MyCommand.Definition -Parent

$valuatorProjectDir = Join-Path -Path $scriptDir -ChildPath "../Valuator"

Start-Process -FilePath "dotnet" -ArgumentList "run --no-build --urls http://localhost:5001" -WorkingDirectory $valuatorProjectDir
Start-Sleep -Milliseconds 1000

Start-Process -FilePath "dotnet" -ArgumentList "run --no-build --urls http://localhost:5002" -WorkingDirectory $valuatorProjectDir
Start-Sleep -Milliseconds 1000

Start-Process -FilePath "dotnet" -ArgumentList "run --no-build --urls http://localhost:5003" -WorkingDirectory $valuatorProjectDir
Start-Sleep -Milliseconds 1000

Start-Process -FilePath "dotnet" -ArgumentList "run --no-build --urls http://localhost:5004" -WorkingDirectory $valuatorProjectDir
Start-Sleep -Milliseconds 1000

$rankCalculatorProjectDir = Join-Path -Path $scriptDir -ChildPath "../RankCalculator/bin/Debug/net8.0"
$rankCalculatorExePath = Join-Path -Path $rankCalculatorProjectDir -ChildPath "RankCalculator.exe"

Start-Process -FilePath $rankCalculatorExePath
Start-Process -FilePath $rankCalculatorExePath