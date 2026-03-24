<template>
  <div class="admin-layout">
    <header class="admin-header">
      <div class="admin-header__brand">
        <button
          type="button"
          class="admin-header__toggle"
          aria-label="Toggle menu"
          @click="sidebarCollapsed = !sidebarCollapsed"
        >
          <i class="fas fa-bars"></i>
        </button>
        <span class="admin-header__title">Water — Admin</span>
      </div>
      <div class="admin-header__actions">
        <span v-if="displayName" class="admin-header__user">{{ displayName }}</span>
        <button type="button" class="admin-header__logout" @click="logout">
          <i class="fas fa-sign-out-alt"></i>
          {{ $t('logout') }}
        </button>
      </div>
    </header>

    <div class="admin-shell">
      <aside class="admin-sidebar" :class="{ 'admin-sidebar--collapsed': sidebarCollapsed }">
        <div class="admin-sidebar__inner">
          <div class="admin-sidebar__brand" aria-hidden="true">
            <span class="admin-sidebar__brand-icon">
              <i class="fas fa-tint"></i>
            </span>
            <div class="admin-sidebar__brand-text">
              <span class="admin-sidebar__brand-title">Water</span>
              <span class="admin-sidebar__brand-sub">Admin</span>
            </div>
          </div>

          <nav class="admin-nav" aria-label="Admin navigation">
            <router-link
              class="admin-nav__link"
              active-class="admin-nav__link--active"
              to="/admin/panel"
            >
              <span class="admin-nav__icon-wrap">
                <i class="fas fa-home admin-nav__icon"></i>
              </span>
              <span class="admin-nav__text">{{ $t('mainMenu') }}</span>
            </router-link>
            <router-link
              class="admin-nav__link"
              active-class="admin-nav__link--active"
              to="/admin/users"
            >
              <span class="admin-nav__icon-wrap">
                <i class="fas fa-user-shield admin-nav__icon"></i>
              </span>
              <span class="admin-nav__text">{{ $t('user') }}</span>
            </router-link>
            <router-link
              class="admin-nav__link"
              active-class="admin-nav__link--active"
              to="/admin/companies"
            >
              <span class="admin-nav__icon-wrap">
                <i class="fas fa-building admin-nav__icon"></i>
              </span>
              <span class="admin-nav__text">{{ $t('company_name') }}</span>
            </router-link>
          </nav>

          <div class="admin-sidebar__foot">
            <span class="admin-sidebar__foot-dot"/>
            <span class="admin-sidebar__foot-dot admin-sidebar__foot-dot--2"/>
          </div>
        </div>
      </aside>

      <main class="admin-main">
        <router-view />
      </main>
    </div>
  </div>
</template>

<script>
export default {
  name: 'AdminLayout',
  data () {
    return {
      sidebarCollapsed: false
    }
  },
  computed: {
    displayName () {
      return localStorage.AdminUserName || localStorage.UserName || ''
    }
  },
  methods: {
    logout () {
      localStorage.adminLogin = ''
      localStorage.AdminAuthId = ''
      localStorage.AdminUserId = ''
      localStorage.WaterCompanyid = ''
      localStorage.AdminUserName = ''
      localStorage.Login = ''
      localStorage.AuthId = ''
      this.$router.push('/admin')
    }
  }
}
</script>

<style lang="scss" scoped>
.admin-layout {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
  background: #f0f2f5;
}

.admin-header {
  height: 56px;
  flex-shrink: 0;
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 1rem 0 0.75rem;
  background: linear-gradient(90deg, #1e293b 0%, #334155 100%);
  color: #f8fafc;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.12);
  z-index: 20;
}

.admin-header__brand {
  display: flex;
  align-items: center;
  gap: 0.5rem;
}

.admin-header__toggle {
  width: 44px;
  height: 44px;
  border: none;
  background: transparent;
  color: inherit;
  cursor: pointer;
  border-radius: 8px;
  &:hover {
    background: rgba(255, 255, 255, 0.08);
  }
}

.admin-header__title {
  font-weight: 700;
  font-size: 1.05rem;
  letter-spacing: 0.02em;
}

.admin-header__actions {
  display: flex;
  align-items: center;
  gap: 1rem;
}

.admin-header__user {
  font-size: 0.875rem;
  opacity: 0.9;
  max-width: 200px;
  overflow: hidden;
  text-overflow: ellipsis;
  white-space: nowrap;
}

.admin-header__logout {
  display: inline-flex;
  align-items: center;
  gap: 0.35rem;
  padding: 0.4rem 0.75rem;
  font-size: 0.875rem;
  border: 1px solid rgba(248, 250, 252, 0.35);
  border-radius: 8px;
  background: transparent;
  color: inherit;
  cursor: pointer;
  &:hover {
    background: rgba(255, 255, 255, 0.1);
  }
}

.admin-shell {
  flex: 1;
  display: flex;
  min-height: 0;
}

.admin-sidebar {
  width: 260px;
  flex-shrink: 0;
  transition: width 0.22s cubic-bezier(0.4, 0, 0.2, 1), box-shadow 0.22s ease;
  overflow: hidden;
  background: linear-gradient(165deg, #ffffff 0%, #f4f6fb 48%, #eef2ff 160%);
  box-shadow:
    inset -1px 0 0 rgba(226, 232, 240, 0.9),
    6px 0 32px -8px rgba(15, 23, 42, 0.08);
}

.admin-sidebar--collapsed {
  width: 0;
  border-right: none;
  box-shadow: none;
}

.admin-sidebar__inner {
  height: 100%;
  min-height: 0;
  display: flex;
  flex-direction: column;
  padding: 1.25rem 0.5rem 1rem;
  box-sizing: border-box;
}

.admin-sidebar__brand {
  display: flex;
  align-items: center;
  gap: 0.75rem;
  padding: 0.65rem 0.5rem 1.1rem;
  margin-bottom: 0.35rem;
  border-bottom: 1px solid rgba(226, 232, 240, 0.75);
  background: linear-gradient(135deg, rgba(99, 102, 241, 0.07) 0%, transparent 65%);
  border-radius: 14px;
}

.admin-sidebar__brand-icon {
  width: 42px;
  height: 42px;
  border-radius: 12px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: linear-gradient(145deg, #6366f1 0%, #4f46e5 100%);
  color: #fff;
  font-size: 1.05rem;
  box-shadow:
    0 4px 14px rgba(79, 70, 229, 0.35),
    inset 0 1px 0 rgba(255, 255, 255, 0.2);
}

.admin-sidebar__brand-text {
  display: flex;
  flex-direction: column;
  gap: 0.1rem;
  min-width: 0;
}

.admin-sidebar__brand-title {
  font-size: 1.02rem;
  font-weight: 800;
  letter-spacing: -0.03em;
  color: #0f172a;
  line-height: 1.15;
}

.admin-sidebar__brand-sub {
  font-size: 0.7rem;
  font-weight: 600;
  text-transform: uppercase;
  letter-spacing: 0.14em;
  color: #94a3b8;
}

.admin-nav {
  flex: 1;
  display: flex;
  flex-direction: column;
  gap: 0.19rem;
  padding: 0.15rem 0.15rem 0;
}

.admin-nav__link {
  position: relative;
  display: flex;
  align-items: center;
  gap: 0.7rem;
  padding: 0.4rem 0.85rem 0.4rem 0.75rem;
  border-radius: 12px;
  color: #475569;
  text-decoration: none;
  font-size: 0.790rem;
  font-weight: 500;
  transition: background 0.18s ease, color 0.18s ease, box-shadow 0.18s ease, transform 0.15s ease;
  overflow: hidden;
  &::after {
    content: '';
    position: absolute;
    left: 0;
    top: 50%;
    transform: translateY(-50%) scaleY(0);
    width: 3px;
    height: 82%;
    border-radius: 0 4px 4px 0;
    background: linear-gradient(180deg, #818cf8 0%, #4f46e5 100%);
    opacity: 0;
    transition: transform 0.2s ease, opacity 0.2s ease;
  }
  &:hover {
    background: rgba(241, 245, 249, 0.95);
    color: #1e293b;
  }
  &:hover .admin-nav__icon-wrap {
    background: rgba(148, 163, 184, 0.2);
    color: #475569;
  }
}

.admin-nav__link--active {
  background: linear-gradient(90deg, rgba(99, 102, 241, 0.14) 0%, rgba(238, 242, 255, 0.55) 52%, rgba(255, 255, 255, 0.4) 100%);
  color: #312e81;
  font-weight: 600;
  box-shadow: 0 1px 3px rgba(79, 70, 229, 0.12);
  &::after {
    transform: translateY(-50%) scaleY(1);
    opacity: 1;
  }
  .admin-nav__icon-wrap {
    background: rgba(99, 102, 241, 0.2);
    color: #4338ca;
    box-shadow: inset 0 1px 0 rgba(255, 255, 255, 0.35);
  }
}

.admin-nav__icon-wrap {
  flex-shrink: 0;
  width: 2.1rem;
  height: 2.1rem;
  border-radius: 10px;
  display: flex;
  align-items: center;
  justify-content: center;
  background: rgba(241, 245, 249, 0.85);
  color: #64748b;
  transition: background 0.18s ease, color 0.18s ease, box-shadow 0.18s ease;
}

.admin-nav__icon {
  font-size: 0.92rem;
  width: 1.1rem;
  text-align: center;
}

.admin-nav__text {
  flex: 1;
  min-width: 0;
  line-height: 1.3;
}

.admin-sidebar__foot {
  margin-top: auto;
  padding: 1rem 0.5rem 0.25rem;
  display: flex;
  align-items: center;
  justify-content: center;
  gap: 0.35rem;
  opacity: 0.45;
}

.admin-sidebar__foot-dot {
  width: 5px;
  height: 5px;
  border-radius: 50%;
  background: #c7d2fe;
  &--2 {
    background: #a5b4fc;
  }
}

.admin-main {
  flex: 1;
  min-width: 0;
  overflow: auto;
  padding: 1.25rem;
}

@media (max-width: 767px) {
  .admin-sidebar {
    position: fixed;
    left: 0;
    top: 56px;
    bottom: 0;
    z-index: 15;
    box-shadow: 4px 0 24px rgba(0, 0, 0, 0.08);
  }
  .admin-sidebar--collapsed {
    width: 0;
    transform: translateX(-100%);
  }
  .admin-main {
    padding: 1rem;
  }
  .admin-header__user {
    display: none;
  }
}
</style>
