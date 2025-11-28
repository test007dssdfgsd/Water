#!/bin/bash

# ApiAll .NET Core 3.1 Application Startup Script
# Bu script server yonganda avtomatik ishga tushadi

# Application papkasi (o'zgartiring kerak bo'lsa)
APP_DIR="/usr/dotnet/app/app"
APP_NAME="ApiAll"

# Log papkasi
LOG_DIR="/var/log/apiall"
LOG_FILE="$LOG_DIR/app.log"

# Log papkasini yaratish
mkdir -p "$LOG_DIR"

# Application papkasiga o'tish
cd "$APP_DIR" || {
    echo "$(date '+%Y-%m-%d %H:%M:%S') - XATO: $APP_DIR papkasiga kirib bo'lmadi" >> "$LOG_FILE"
    exit 1
}

# .NET Runtime mavjudligini tekshirish
if ! command -v dotnet &> /dev/null; then
    echo "$(date '+%Y-%m-%d %H:%M:%S') - XATO: .NET Runtime topilmadi" >> "$LOG_FILE"
    exit 1
fi

# Application ni ishga tushirish
echo "$(date '+%Y-%m-%d %H:%M:%S') - $APP_NAME ishga tushmoqda..." >> "$LOG_FILE"

# Environment variables (kerak bo'lsa)
export ASPNETCORE_ENVIRONMENT=Production
export ASPNETCORE_URLS="http://0.0.0.0:5003;https://0.0.0.0:5006"

# Application ni ishga tushirish
dotnet "$APP_NAME.dll" >> "$LOG_FILE" 2>&1

# Agar xatolik bo'lsa
if [ $? -ne 0 ]; then
    echo "$(date '+%Y-%m-%d %H:%M:%S') - XATO: Application xatolik bilan to'xtadi" >> "$LOG_FILE"
    exit 1
fi