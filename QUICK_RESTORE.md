# ⚡ Tezkor Restore Qo'llanmasi

## 🎯 Eng Oson Usul (3 Qadam)

### 1️⃣ Backup Faylini Yuklab Olish

**FileZillada:**
- `Ctrl+H` bosing (yashirin fayllarni ko'rsatish)
- Script papkasiga boring
- `2024-01-15-14-30.backup.gz` faylni Windows papkasiga ko'chiring

### 2️⃣ Compress Ochish

**7-Zip orqali** (eng oson):
1. `.gz` faylga o'ng bosish
2. **7-Zip** → **Extract Here**
3. `.backup` fayl paydo bo'ladi

### 3️⃣ pgAdmin orqali Restore

1. **pgAdmin** oching
2. **Databases** → **Create** → **Database** (`alldb_restore`)
3. Database ga o'ng bosish → **Restore...**
4. `.backup` faylni tanlang
5. **Format**: `Tar` tanlash
6. **Restore** tugmasini bosing

**✅ Tugadi!**

---

## 🔧 PowerShell Script (Avtomatik)

```powershell
# 1. Script ni ishga tushirish:
.\restore_backup.ps1 -BackupFile "C:\Backups\2024-01-15-14-30.backup.gz"

# 2. Password kiriting (so'ralganda)
# 3. Kutib turing...
```

**✅ Avtomatik barcha qadamlarni bajaradi!**

---

## 💡 Maslahatlar

- ✅ 7-Zip o'rnatish (compress ochish uchun)
- ✅ pgAdmin ishlatish (GUI - oson)
- ✅ Backup fayl yo'lini tekshirish
- ✅ Database nomi boshqa bo'lmasligi

---

**Batafsil qo'llanma**: `RESTORE_QOLLANMA.md`

