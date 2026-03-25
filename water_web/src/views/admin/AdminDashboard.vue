<template>
  <div class="admin-dashboard">
    <h1 class="admin-dashboard__title">{{ $t('mainMenu') }}</h1>
    <p class="admin-dashboard__hint">
      Admin panel — kompaniyalar va foydalanuvchilarni shu yerdan boshqaring.
    </p>
  </div>
</template>

<script>
export default {
  name: 'AdminDashboard'
}
</script>

<style lang="scss" scoped>
.admin-dashboard {
  background: #fff;
  border-radius: 12px;
  padding: 1.5rem;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.06);
}
.admin-dashboard__title {
  margin: 0 0 0.5rem;
  font-size: 1.35rem;
  color: #0f172a;
}
.admin-dashboard__hint {
  margin: 0;
  color: #64748b;
  font-size: 0.95rem;
}
</style>
