#!/bin/bash

# Written 2018-11-15 by 4410287
# This script will create a backup file of a postgres database and compress it.  It is capable of access a local or remote server to pull the backup.  After creating a new backup, it will delete backups that are older than 15 days, with the exception of backups created the first of every month.  It is recommended to create a seperate database user specifically for backup purposes, and to set the permissions of this script to prevent access to the login details.  Backup scripts for different databases should be run in seperate folders or they will overwrite each other.

HOSTNAME=127.0.0.1
USERNAME=postgres
PASSWORD=wRMlQlzLmuTO1K0qLOzP
DATABASE=alldb

# Note that we are setting the password to a global environment variable temporarily.
echo "Pulling Database: This may take a few minutes"
export PGPASSWORD="$PASSWORD"
pg_dump -F t -h $HOSTNAME -p 5454  -U $USERNAME $DATABASE > $(date +%Y-%m-%d-%H-%M).backup
unset PGPASSWORD
gzip $(date +%Y-%m-%d-%H-%M).backup
echo "Pull Complete"

echo "Clearing old backups"
find . -type f -iname '*.backup.gz' -ctime +15 -not -name '????-??-01.backup.gz' -delete
echo "Clearing Complete"