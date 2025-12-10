#!/bin/bash
# Git cache'dan bin, obj va migrations papkalarini olib tashlash skripti
# Bash skripti (Linux/Mac uchun)

echo "Git cache'dan bin, obj va migrations papkalarini olib tashlayapman..."

# Bin papkalarini olib tashlash
git rm -r --cached **/bin/
git rm -r --cached **/obj/
git rm -r --cached ApiAll/bin/
git rm -r --cached ApiAll/obj/
git rm -r --cached **/Migrations/
git rm -r --cached ApiAll/Migrations/

echo "Bajarildi! Endi GitHub Desktop'da commit qiling."
echo "Eslatma: Bu buyruq faqat Git cache'dan olib tashlaydi, fayllarni diskdan o'chirmaydi."

