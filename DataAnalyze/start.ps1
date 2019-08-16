# The script sets the sa password and start the SQL Service
# Also it attaches additional database from the disk
# The format for attach_dbs

param(
[Parameter(Mandatory=$false)]
[string]$sa_password,

[Parameter(Mandatory=$false)]
[string]$ACCEPT_EULA,

[Parameter(Mandatory=$false)]
[string]$attach_dbs,

[Parameter(Mandatory=$false)]
[string]$start_date,

[Parameter(Mandatory=$false)]
[string]$end_date,

[Parameter(Mandatory=$false)]
[string]$currentDb,

[Parameter(Mandatory=$false)]
[string]$dataContextDb,

[Parameter(Mandatory=$false)]
[string]$dataContextPassword
)

if($ACCEPT_EULA -ne "Y" -And $ACCEPT_EULA -ne "y")
{
	Write-Verbose "ERROR: You must accept the End User License Agreement before this container can start."
	Write-Verbose "Set the environment variable ACCEPT_EULA to 'Y' if you accept the agreement."

    exit 1
}

#Write-Verbose "Changing port to $port"
#set-itemproperty -path 'HKLM:\software\microsoft\microsoft sql server\mssql14.MSSQLSERVER\mssqlserver\supersocketnetlib\tcp\ipall' -name tcpport -value $port

# start the service
Write-Verbose "Starting SQL Server"
start-service MSSQLSERVER

if($sa_password -eq "_") {
    if (Test-Path $env:sa_password_path) {
        $sa_password = Get-Content -Raw $secretPath
    }
    else {
        Write-Verbose "WARN: Using default SA password, secret file not found at: $secretPath"
    }
}

if($sa_password -ne "_")
{
    Write-Verbose "Changing SA login credentials"
    $sqlcmd = "ALTER LOGIN sa with password=" +"'" + $sa_password + "'" + ";ALTER LOGIN sa ENABLE;"
    & sqlcmd -Q $sqlcmd
}

$attach_dbs_cleaned = $attach_dbs.TrimStart('\\').TrimEnd('\\')

$dbs = $attach_dbs_cleaned | ConvertFrom-Json

if ($null -ne $dbs -And $dbs.Length -gt 0)
{
    Write-Verbose "Attaching $($dbs.Length) database(s)"
	    
    Foreach($db in $dbs) 
    {            
        $files = @();
        Foreach($file in $db.dbFiles)
        {
            $files += "(FILENAME = N'$($file)')";           
        }

        $files = $files -join ","
        $sqlcmd = "IF EXISTS (SELECT 1 FROM SYS.DATABASES WHERE NAME = '" + $($db.dbName) + "') BEGIN EXEC sp_detach_db [$($db.dbName)] END;CREATE DATABASE [$($db.dbName)] ON $($files) FOR ATTACH;"

        Write-Verbose "Invoke-Sqlcmd -Query $($sqlcmd)"
        & sqlcmd -Q $sqlcmd
	}
}

Write-Verbose "Starting SQL Agent"
start-service SQLSERVERAGENT

do
{
	Write-Verbose "Waiting SQLSERVERAGENT service"
	Start-Sleep -Seconds 2
	Start-Service SQLSERVERAGENT
	$startingAgent = (Get-Service SQLSERVERAGENT).Status
} while($startingAgent.Status -eq "Running" -or $startingAgent.Status -eq "Stopped")

if($startingAgent -eq "Running")
{
	Write-Verbose "Creating load job"
	& sqlcmd -i .\CREATE_JOB.sql
}

Write-Verbose "Loading DimDate $start_date $end_date"
& sqlcmd -Q "EXEC DataDW.[dbo].[SP_DimDate] @startdate='`$(startdate)', @enddate='`$(enddate)'"  -v startdate="$start_date" enddate="$end_date"

Write-Verbose "Changing Import data"
(Get-Content .\Import.dtsx).replace('Data Source=192.168.15.35,1533;User ID=sa;Password=Senh@123', "Data Source=$currentDb;User ID=sa;Password=$sa_password") | Set-Content .\Import.dtsx
(Get-Content .\Import.dtsx).replace('Data Source=192.168.15.35;User ID=sa;Password=Senh@123', "Data Source=$dataContextDb;User ID=sa;Password=$dataContextPassword") | Set-Content .\Import.dtsx

Write-Verbose "Holding service"
$lastCheck = (Get-Date).AddSeconds(-2) 
while ($true) 
{ 
    Get-EventLog -LogName Application -Source "MSSQL*" -After $lastCheck | Select-Object TimeGenerated, EntryType, Message	 
    $lastCheck = Get-Date 
    Start-Sleep -Seconds 2 
}
