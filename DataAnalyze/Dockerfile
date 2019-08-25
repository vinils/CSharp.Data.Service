FROM vinils/mssql-server-windows-developer

ENV startdate="_" \
    attach_dbs="[]" \
    ACCEPT_EULA="_" \
    sa_password_path="C:\ProgramData\Docker\secrets\sa-password" \
	start_date=2005-01-01 \
	end_date=2022-12-31
	
# escape=`

COPY . .\DataAnalyze
WORKDIR .\DataAnalyze

#SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

##bug - Sqlcmd: Error: Microsoft ODBC Driver 13 for SQL Server : TCP Provider: No connection could be made because the target machine actively refused it
#RUN sqlcmd -S localhost,1433 -i .\DataContext\CREATE_DATABASE_DataContext.sql ; \
RUN do { $count = $count + 1; $healfcheck = (sqlcmd -S localhost,1433 -i .\DataContext\CREATE_DATABASE_DataContext.sql); Start-Sleep -Seconds 10 } while(!$healfcheck -and $count -ne 60) ;

RUN sqlcmd -S localhost,1433 -i .\DataContext\CREATE_TABLE_Data.sql ; \
		sqlcmd -S localhost,1433 -i .\DataContext\CREATE_TABLE_Group.sql ; \
		sqlcmd -S localhost,1433 -i .\DataContext\CREATE_TABLE_LimitDecimal.sql ; \
		sqlcmd -S localhost,1433 -i .\DataContext\CREATE_TABLE_LimitDecimalDenormalized.sql ; \
		sqlcmd -S localhost,1433 -i .\DataContext\CREATE_TABLE_LimitStringDenormalized.sql ; \
		sqlcmd -S localhost,1433 -i .\DataDW\CREATE_DATABASE_DataDW_MS.sql ; \
		sqlcmd -S localhost,1433 -i .\DataDW\CREATE_TABLE_DimDate.sql ; \
		sqlcmd -S localhost,1433 -i .\DataDW\CREATE_TABLE_DimGroup.sql ; \
		sqlcmd -S localhost,1433 -i .\DataDW\CREATE_TABLE_DimLimit.sql ; \
		sqlcmd -S localhost,1433 -i .\DataDW\CREATE_TABLE_FactDataLimit.sql ; \
		sqlcmd -S localhost,1433 -i .\DataDW\CREATE_TABLE_FactData.sql ; \
		sqlcmd -S localhost,1433 -i .\DataDW\CREATE_PROCEDURE_SP_DimDate.sql ; \
		sqlcmd -S localhost,1433 -i .\DataDW\CREATE_PROCEDURE_SP_Load.sql ; \
		sqlcmd -S localhost,1433 -i .\DataDW\CREATE_PROCEDURE_SP_Reload.sql ; \
		Remove-Item -Recurse -Force .\DataDW ; \
		Remove-Item -Recurse -Force .\DataContext ; 

CMD .\start -sa_password $env:sa_password -ACCEPT_EULA $env:ACCEPT_EULA -attach_dbs \"$env:attach_dbs\" -Verbose -start_date \"$env:start_date\" -end_date \"$env:end_date\" -currentDb \"$env:currentDb\" -dataContextDb \"$env:dataContextDb\" -dataContextPassword \"$env:dataContextPassword\"
