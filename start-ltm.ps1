# This script will attempt to setup and start a Levy Transfers Management development environment
# It currently will:
# * Look for required git repo's and clone them if they don't exist
# * Look for and load the solutions in VS2019
# * Look for and start Docker Desktop
# * Look for and start the CosmosDB emulator
# 
# The script also attempts to make use of a number of Environment variables. If these variables don't exist then the script falls back to defaults
#
# * PROJECTS_DIR            - The directory where your source code can be found (Defaults to D:\Projects)
# * VS2019_PATH             - The install path for VS2019 (Defaults to: C:\Program Files (x86)\Microsoft Visual Studio\2019\Professional\Common7\IDE\)
# * DOCKER_PATH             - The install path for Docker Desktop (Defaults to: C:\Program Files\Docker\Docker\frontend\)
# * COSMOS_EMULATOR_PATH    - The install path for the CosmosDB Emulator (Defaults to: C:\Program Files\Azure Cosmos DB Emulator\)
#
# Usage:
#
# ./start-ltm.ps1 [switches]
#
# Switches:
# * -clone  - Looks for the LTM and 3rd party git repo dependencies, if it cannot find them it will attempt to clone them. Repositories will only be cloned if they don't already exist in the location
# * -open   - Looks for VS2019 and opens all LTM and 3rd party code bases if found
# * -docker - Looks for Docker Desktop and starts it if found. It will also attempt to start 3 containers: shared-redis, das-config & ms-sql. If it can't find the redis image it will download and run it
# * -cosmos - Looks for the CosmosDB emulator and starts it if found
#
# Example to do everything
#
# ./start-ltm.ps1 -clone -open -docker -cosmos
#
# Note:
# You will have to set the install locations of VS2019, Docker Desktop and CosmosDB Emulator to use all features of this script
# All the LTM code repositories are assumed to be in a folder called "transfers" under the project root. External code dependencies are assumed to be in the project root.
#
# Potential further work:
# * Download and insert config values to the Storage emulator

[CmdletBinding()]
param (

    [Parameter(Mandatory=$false)]
    [switch]
    $clone,

    [Parameter(Mandatory=$false)]
    [switch]
    $open,

    [Parameter(Mandatory=$false)]
    [switch]
    $docker,

    [Parameter(Mandatory=$false)]
    [switch]
    $cosmos,

    [Parameter(Mandatory=$false)]
    [switch]
    $help
)

if($help)
{
    Write-Information -MessageData 'Usage: start-ltm [-clone] [-open] [-docker] [-cosmos] [-help]' -InformationAction Continue
    return
}

$projectDir = $Env:PROJECTS_DIR
if([string]::IsNullOrEmpty($projectDir))
{
    $projectDir = "C:\Code\Contracting\"
}

$vsLocation = $Env:VS2019_PATH
if([string]::IsNullOrEmpty($vsLocation))
{
    $vsLocation = "C:\Program Files (x86)\Microsoft Visual Studio\2019\Enterprise\Common7\IDE\"
}

$dockerLocation = $Env:DOCKER_PATH
if([string]::IsNullOrEmpty($dockerLocation))
{
    $dockerLocation = "C:\Program Files\Docker\Docker\frontend\"
}

$cosmosLocation = $Env:COSMOS_EMULATOR_PATH
if([string]::IsNullOrEmpty($cosmosLocation))
{
    $cosmosLocation = "C:\Program Files\Azure Cosmos DB Emulator\"
}

function Clone-Repo {
    param(
        [Parameter()]
        [string]
        $location,
        
        [Parameter()]
        [string]
        $path           
    )

    if(-not (Test-Path -Path $path -PathType Container))
    {
        Push-Location $path
        git clone $location
        Pop-Location
    }
}

function Start-VisualStudio {
    param (
        [Parameter()]
        [string]
        $projectPath
    )

    &"${vsLocation}\devenv.exe" $projectPath
}

function Start-Redis {
    $output = (cmd /c "docker image inspect redis") | Out-String
    
    if($output -like "*Error:*")
    {
        Write-Information -MessageData 'Installing redis'
        docker pull redis
        docker run --name shared-redis -d redis redis-server
    }

    else
    {
        Write-Information -MessageData 'Starting redis'
        docker start shared-redis
    }
}

Set-Location $projectDir

if($clone)
{
    Clone-Repo -location 'https://github.com/SkillsFundingAgency/das-levy-transfer-matching-web.git' -path "${projectDir}\transfers\das-levy-transfer-matching-web\"
    Clone-Repo -location 'https://github.com/SkillsFundingAgency/das-levy-transfer-matching-api.git' -path "${projectDir}\transfers\das-levy-transfer-matching-api\"
    Clone-Repo -location 'https://github.com/SkillsFundingAgency/das-levy-transfer-matching-functions.git' -path "${projectDir}\transfers\das-levy-transfer-matching-functions\"
    Clone-Repo -location 'https://github.com/SkillsFundingAgency/das-apim-endpoints.git' -path "${projectDir}\das-apim-endpoints\"
    Clone-Repo -location 'https://github.com/SkillsFundingAgency/das-employerapprenticeshipsservice.git' -path "${projectDir}\das-employerapprenticeshipsservice\"
    Clone-Repo -location 'https://github.com/SkillsFundingAgency/das-courses-api.git' -path "${projectDir}\das-courses-api\"
    Clone-Repo -location 'https://github.com/SkillsFundingAgency/das-location-api.git' -path "${projectDir}\das-location-api\"
}

# Start LTM projects in VS2019
if($open -and (Test-Path -Path "${vsLocation}" -PathType Container))
{
    Start-VisualStudio -projectPath '.\das-levy-transfer-matching-web\src\SFA.DAS.LevyTransferMatching.Web.sln'
    Start-VisualStudio -projectPath '.\das-levy-transfer-matching-functions\src\SFA.DAS.LevyTransferMatching.Functions.sln'
    Start-VisualStudio -projectPath '.\das-apim-endpoints\src\SFA.DAS.Apim.Endpoints.sln'
    Start-VisualStudio -projectPath '.\das-levy-transfer-matching-api\src\SFA.DAS.LevyTransferMatching.Api.sln'
    Start-VisualStudio -projectPath '.\das-employerapprenticeshipsservice\src\SFA.DAS.EAS.sln'
    Start-VisualStudio -projectPath '.\das-courses-api\src\SFA.DAS.Courses.sln'
    Start-VisualStudio -projectPath '.\das-location-api\src\SFA.DAS.Location.sln'
}

elseif($open -and (-not (Test-Path -Path "${vsLocation}" -PathType Container)))
{
    Write-Error "You must have Visual Studio 2019 installed and have set the vsLocation variable to start Visual Studio (https://visualstudio.microsoft.com/vs/professional/)"
}
 
if($docker -and (Test-Path -Path "${dockerLocation}" -PathType Container))
{
    # Start docker desktop
    $myprocss = Start-Process "${dockerLocation}Docker Desktop.exe" -PassThru
    $myprocss.WaitForExit()

    # Wait for the docker daemon to start
    $params = "info"
    $output = (cmd /c "docker $params") | Out-String

    while($output -like '*Error:*') 
    {
        Write-Information -MessageData 'Sleeping...'
        Start-Sleep -Seconds 10
        $output = (cmd /c "docker $params") | Out-String
    }


    # Start the docker containers
    Start-Redis
    docker start das-config
    docker start ms-sql
}

elseif ($docker -and (-not (Test-Path -Path "${dockerLocation}" -PathType Container)))
{
    Write-Error "You must have the Docker Desktop installed and have set the dockerLocation variable to start Docker Desktop (https://www.docker.com/products/docker-desktop)"
}

# Start CosmosDb
if($cosmos -and (Test-Path -Path "${cosmosLocation}" -PathType Container))
{
    &"${cosmosLocation}Microsoft.Azure.Cosmos.Emulator.exe"
}

elseif ($cosmos -and (-not (Test-Path -Path "${cosmosLocation}" -PathType Container)))
{
    Write-Error "You must have the CosmosDB emulator installed and have set the cosmosLocation variable to start the CosmosDB emulator (https://aka.ms/cosmosdb-emulator)"
}

Set-Location $projectDir
