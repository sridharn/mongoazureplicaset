<#
 # Copyright 2010-2011 10gen Inc.
 # file : solutionsetup.ps1
 # Licensed under the Apache License, Version 2.0 (the "License");
 # you may not use this file except in compliance with the License.
 # You may obtain a copy of the License at
 # 
 # 
 # http://www.apache.org/licenses/LICENSE-2.0
 # 
 # 
 # Unless required by applicable law or agreed to in writing, software
 # distributed under the License is distributed on an "AS IS" BASIS,
 # WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 # See the License for the specific language governing permissions and
 # limitations under the License.
 #>
 
$cloudConfigTemplateFile = Join-Path $pwd "configfiles\ServiceConfiguration.Cloud.cscfg.ref"
$cloudConfigFile = Join-Path $pwd "MongoDBReplicaSet\ServiceConfiguration.Cloud.cscfg"
$mongodbDownloadUrl = "http://downloads.mongodb.org/win32/mongodb-win32-x86_64-latest.zip"
$mongodbBinaryTarget = Join-Path $pwd "ReplicaSetRole\MongoDBBinaries"

function Setup-CloudConfig {
    Write-Host "Creating Cloud config file"
    if (!(Test-Path -LiteralPath $cloudConfigFile -PathType Leaf)) {
     cp $cloudConfigTemplateFile $cloudConfigFile
    }
    Write-Host "Cloud config file created"
}

function Download-Binaries {
    $storageDir = Join-Path $pwd "downloadtemp"
    $webclient = New-Object System.Net.WebClient
    $split = $mongodbDownloadUrl.split("/")
    $fileName = $split[$split.Length-1]
    $filePath = Join-Path $storageDir $fileName
    
    if (!(Test-Path -LiteralPath $storageDir -PathType Container)) {
        New-Item -type directory -path $storageDir | Out-Null
    }
    else {
        Write-Host "Cleaning out temporary download directory"
        Remove-Item (Join-Path $storageDir "*") -Recurse -Force
        Write-Host "Temporary download directory cleaned"
    }
    
    Write-Host "Downloading mongodb binaries. This could take time..."
    $webclient.DownloadFile($mongodbDownloadUrl, $filePath)
    Write-Host "mongodb binaries downloaded. Unzipping..."
    
    $shell_app=new-object -com shell.application
    $zip_file = $shell_app.namespace($filePath)
    $destination = $shell_app.namespace($storageDir)
    
    $destination.Copyhere($zip_file.items())
    
    Write-Host "Binaries unzipped. Copying to destination"
    $unzipDir = GetUnzipPath($storageDir, $filePath)
    Copy-Item $unzipDir -destination $mongodbBinaryTarget -Recurse
    Write-Host "Done copying. Clearing temporary storage directory"
    
    if (Test-Path -LiteralPath $storageDir -PathType Container) {
        Remove-Item -path $storageDir -force -Recurse
    }
    
}

function GetUnzipPath {
    Param($downloadDir, $downloadFile)
    $dir = Get-Item (Join-Path $storageDir "*") -Exclude $fileName
    return $dir.FullName
}


Write-Host "Start with setup"
Setup-CloudConfig
Download-Binaries
Write-Host "Done with setup"