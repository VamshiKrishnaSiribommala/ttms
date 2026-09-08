#!/usr/bin/env bash
set -e

SA_PASSWORD="${MSSQL_SA_PASSWORD:-YourStrong@Password123}"
CONTAINER_NAME="tms-sqlserver"

echo "=========================================================="
echo " Initializing Train Management System (TMS) Databases     "
echo "=========================================================="

echo "Waiting for SQL Server in container '$CONTAINER_NAME' to accept connections..."
until docker exec "$CONTAINER_NAME" /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C -Q "SELECT 1" &> /dev/null || \
      docker exec "$CONTAINER_NAME" /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C -Q "SELECT 1" &> /dev/null
do
    echo "SQL Server is still warming up... waiting 3 seconds"
    sleep 3
done

echo "SQL Server is ONLINE. Running database restoration and initialization..."
if docker exec "$CONTAINER_NAME" test -f /opt/mssql-tools18/bin/sqlcmd; then
    docker exec "$CONTAINER_NAME" /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C -i /var/opt/mssql/backup/restore-databases.sql
else
    docker exec "$CONTAINER_NAME" /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P "$SA_PASSWORD" -C -i /var/opt/mssql/backup/restore-databases.sql
fi

echo "=========================================================="
echo " [SUCCESS] All databases restored and verified ready!     "
echo "=========================================================="
