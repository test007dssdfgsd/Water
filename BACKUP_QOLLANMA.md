# backup_db.sh - Qanday Ishlaydi?

## 📍 Backup Qayerga Saqlanadi?

**Backup fayl scriptni ishga tushirgan papkada saqlanadi!**

### Misol:
```bash
# Agar siz /home/user/ papkasida scriptni ishga tushirsangiz:
cd /home/user/
./backup_db.sh

# Backup fayl shu yerda saqlanadi:
# /home/user/2024-01-15-14-30.backup.gz
```

### Alohida Papkaga Saqlash uchun:

Scriptda `BACKUP_DIR` o'zgaruvchisini o'zgartiring:

```bash
# Hozirgi holat (script papkasida):
BACKUP_DIR="."

# Alohida papkaga (tavsiya etiladi):
BACKUP_DIR="./backups"
mkdir -p $BACKUP_DIR
```

## 🚀 Qanday Ishga Tushirish?

### 1. Scriptni executable qilish:
```bash
chmod +x backup_db.sh
```

### 2. Ishga tushirish:
```bash
# Joriy papkadan:
./backup_db.sh

# Yoki to'liq yo'l bilan:
/path/to/backup_db.sh
```

### 3. Terminaldan ko'rish:
```bash
# Script ishlayapti:
=========================================
Database Backup boshlanmoqda...
Database: alldb
Host: 127.0.0.1:5454
Fayl: ./2024-01-15-14-30.backup
=========================================
Pulling Database: This may take a few minutes
✓ Database backup yaratildi!
Backup compress qilinmoqda...
✓ Backup compress qilindi!
Fayl: ./2024-01-15-14-30.backup.gz
Hajm: 25M
Pull Complete
Clearing old backups...
Clearing Complete
=========================================
Backup jarayoni yakunlandi!
Backup joylashuvi: .
=========================================
```

## 📂 Fayl Strukturasi

### Backup fayl nomi:
```
YYYY-MM-DD-HH-MM.backup.gz

Misol: 2024-01-15-14-30.backup.gz
       └─ 2024 yil
          └─ 01 oy
             └─ 15 kun
                └─ 14:30 vaqt
```

## 🔄 Avtomatik Backup (Cron Job)

### Har kuni ertalab 2:00 da backup olish:

```bash
# Crontab ni ochish:
crontab -e

# Qo'shish:
0 2 * * * cd /path/to/backup && /path/to/backup_db.sh >> /var/log/postgres_backup.log 2>&1
```

### Har soat backup olish:
```bash
0 * * * * cd /path/to/backup && /path/to/backup_db.sh
```

## 📋 Script Qanday Ishlaydi?

1. **Vaqtni belgilash**: `TIMESTAMP=$(date +%Y-%m-%d-%H-%M)`
2. **Backup yaratish**: `pg_dump` bilan PostgreSQL dan backup oladi
3. **Compress qilish**: `gzip` bilan siqadi
4. **Eski backuplarni tozalash**: 15 kundan eski backuplarni o'chiradi
5. **Xavfsizlik**: Har oyning 1-kuni yaratilgan backuplarni saqlaydi

## 🔍 Backuplarni Ko'rish

```bash
# Barcha backuplarni ko'rish:
ls -lh *.backup.gz

# Tafsilotlar bilan:
ls -lht *.backup.gz | head -10

# Backup hajmini ko'rish:
du -sh *.backup.gz
```

## ⚠️ Muhim Maslahatlar

1. **Backup papkasini yaratish**:
   ```bash
   mkdir -p backups
   ```

2. **Backup papkasiga ruxsat berish**:
   ```bash
   chmod 755 backups
   ```

3. **Remote serverga ko'chirish** (masalan, yedek nusxa):
   ```bash
   scp *.backup.gz user@remote-server:/path/to/backups/
   ```

4. **Backup ni tekshirish**:
   ```bash
   # Tar formatni ko'rish:
   gunzip -c backup_file.backup.gz | pg_restore --list | head -20
   ```

## 🛠️ Sozlash

### Database nomini o'zgartirish:
```bash
DATABASE=new  # o'zgartiring
```

### Host va Port ni o'zgartirish:
```bash
HOSTNAME=62.209.128.51
PORT=5454
```

### Backup saqlash muddatini o'zgartirish:
```bash
# 15 kun o'rniga 30 kun:
find "$BACKUP_DIR" -type f -iname '*.backup.gz' -ctime +30 ...
```

