﻿$EncryptionKey = (Get-Credential -Message "Enter the encryption key you would like to use. This must match the encryption key used by the rest of the confCons file." -UserName "DontNeedUsername").Password
$PathToMrngFolder = "C:\Users\SparerD\Downloads\mRemoteNG-Portable-1.75.7012.16846"

if ($PathToMrngFolder -eq "") {
    Write-Error -Message 'You must set the $PathToMrngFolder variable in this script to the folder which contains mRemoteNG.exe'
}

$assembly = [System.Reflection.Assembly]::LoadFile((Join-Path -Path $PathToMrngFolder -ChildPath "mRemoteNG.exe"))
$assembly = [System.Reflection.Assembly]::LoadFile((Join-Path -Path $PathToMrngFolder -ChildPath "BouncyCastle.Crypto.dll"))

function New-mRemoteNGXmlSerializer {
    [CmdletBinding()]
    param (
        [SecureString]
        $EncryptionKey
    )

    PROCESS {
        $cryptoProvider = New-Object -TypeName mRemoteNG.Security.SymmetricEncryption.AeadCryptographyProvider
        $saveFilter = New-Object -TypeName mRemoteNG.Security.SaveFilter -ArgumentList @($false)
        $xmlSerializer = New-Object -TypeName mRemoteNG.Config.Serializers.XmlConnectionNodeSerializer -ArgumentList @($cryptoProvider, $encryptionKey, $saveFilter)
        Write-Output $xmlSerializer
    }
}

function New-mRemoteNGConnectionInfo {
    [CmdletBinding()]
    param (
    )

    PROCESS {
        $connectionInfo = New-Object -TypeName mRemoteNG.Connection.ConnectionInfo
        Write-Output $connectionInfo
    }
}

function New-mRemoteNGContainerInfo {
    [CmdletBinding()]
    param (
    )

    PROCESS {
        $connectionInfo = New-Object -TypeName mRemoteNG.Container.ContainerInfo
        Write-Output $connectionInfo
    }
}

# Setup the services needed to do serialization
$xmlSerializer = New-mRemoteNGXmlSerializer -EncryptionKey $EncryptionKey



#----------------------------------------------------------------
# Example 1: serialize many connections
# Here you can define the number of connection info objects to create
# You can also provide a list of desired hostnames and iterate over those
$xml = ""
foreach($i in 1..5)
{
    $connectionInfo = New-mRemoteNGConnectionInfo

    # Set connection info properties
    $connectionInfo.Name = "server-$i"
    $connectionInfo.Hostname = "some-win-server-$i"
    $connectionInfo.Protocol = [mRemoteNG.Connection.Protocol.ProtocolType]::RDP
    $connectionInfo.Inheritance.Username = $true
    $connectionInfo.Inheritance.Domain = $true
    $connectionInfo.Inheritance.Password = $true

    $serializedConnection = $xmlSerializer.SerializeConnectionInfo($connectionInfo).ToString()
    $xml += $serializedConnection + [System.Environment]::NewLine
}

Write-Output $xml




#----------------------------------------------------------------
# Example 2: serialize a container which has connections
# You can also create containers and add connections to them, which will be nested correctly when serialized
$xml = ""
$container = New-mRemoteNGContainerInfo
$container.Name = "ProductionServers"
$serializedContainer = $xmlSerializer.SerializeConnectionInfo($container)

foreach($i in 1..3)
{
    $connectionInfo = New-mRemoteNGConnectionInfo

    # Set connection info properties
    $connectionInfo.Name = "server-$i"
    $connectionInfo.Hostname = "some-linux-server-$i"
    $connectionInfo.Protocol = [mRemoteNG.Connection.Protocol.ProtocolType]::SSH2
    $connectionInfo.Inheritance.Username = $true
    $connectionInfo.Inheritance.Domain = $true
    $connectionInfo.Inheritance.Password = $true
    
    # serialize the connection
    $serializedConnection = $xmlSerializer.SerializeConnectionInfo($connectionInfo)
    # add the connection to the container
    $serializedContainer.Add($serializedConnection)
}

# get the raw xml text
Write-Output $serializedContainer.ToString()