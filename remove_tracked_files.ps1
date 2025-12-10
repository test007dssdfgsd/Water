# Git cache'dan bin, obj va migrations papkalarini olib tashlash skripti
# PowerShell skripti

Write-Host "Git cache'dan bin, obj va migrations papkalarini olib tashlayapman..." -ForegroundColor Yellow

# Bin papkalarini olib tashlash
git rm -r --cached **/bin/
git rm -r --cached **/obj/
git rm -r --cached ApiAll/bin/
git rm -r --cached ApiAll/obj/
git rm -r --cached **/Migrations/
git rm -r --cached ApiAll/Migrations/

Write-Host "Bajarildi! Endi GitHub Desktop'da commit qiling." -ForegroundColor Green
Write-Host "Eslatma: Bu buyruq faqat Git cache'dan olib tashlaydi, fayllarni diskdan o'chirmaydi." -ForegroundColor Cyan

