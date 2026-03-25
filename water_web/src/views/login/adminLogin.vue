<template>
  <div class="login-app admin-login-app">
    <loader v-if="loading"/>

    <div class="language-switcher">
      <mdb-btn color="elegant" @click="rus_btn" class="lang-btn">{{$t('ru')}}</mdb-btn>
    </div>

    <krillKeyboard @text="matn" v-show="klavish && ruskiy" style="position: absolute; bottom: 0; right: 0;"/>
    <numberKeyboard @number="num_func" v-show="raqam" style="position: absolute; bottom: 10px; right: 0;"/>

    <div class="login-container">
      <div class="login-card">
        <div class="login-header">
          <h2 class="login-title admin-title">{{$t('authorization')}}</h2>
          <p class="admin-badge">Admin</p>
        </div>

        <div class="error-message" v-if="error">
          <span class="error-text">{{ error }}</span>
        </div>

        <form v-on:submit.prevent="submit" class="login-form">
          <div class="form-group">
            <div class="input-wrapper">
              <div class="input-icon">
                <svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-user" width="24" height="24" viewBox="0 0 24 24" stroke-width="1.5" stroke="#5a67d8" fill="none" stroke-linecap="round" stroke-linejoin="round">
                  <path stroke="none" d="M0 0h24v24H0z" fill="none"/>
                  <circle cx="12" cy="7" r="4" />
                  <path d="M6 21v-2a4 4 0 0 1 4 -4h4a4 4 0 0 1 4 4v2" />
                </svg>
              </div>
              <input
                type="text"
                v-model="login"
                :placeholder="$t('username')"
                class="form-input"
              >
              <small class="invalid-text" v-if="$v.login.$dirty && !$v.login.required">
                {{$t('name_invalid_text')}}
              </small>
            </div>
          </div>

          <div class="form-group">
            <div class="input-wrapper">
              <div class="input-icon">
                <svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-lock" width="24" height="24" viewBox="0 0 24 24" stroke-width="1.5" stroke="#5a67d8" fill="none" stroke-linecap="round" stroke-linejoin="round">
                  <path stroke="none" d="M0 0h24v24H0z" fill="none"/>
                  <rect x="5" y="11" width="14" height="10" rx="2" />
                  <circle cx="12" cy="16" r="1" />
                  <path d="M8 11v-4a4 4 0 0 1 8 0v4" />
                </svg>
              </div>
              <input
                type="password"
                v-model="pass"
                @input="handleHashing($event.target.value)"
                :placeholder="$t('password')"
                class="form-input"
              >
              <small class="invalid-text" v-if="$v.pass.$dirty && !$v.pass.required">
                {{$t('name_invalid_text')}}
              </small>
            </div>
          </div>

          <div class="form-actions">
            <mdb-btn color="primary" type="submit" class="submit-btn admin-submit-btn">
              <i class="fas fa-sign-in-alt"></i>
              {{$t('enter')}}
            </mdb-btn>
          </div>
        </form>
      </div>
    </div>

    <Toast ref="message"></Toast>
  </div>
</template>

<script>
import krillKeyboard from './krillKeyboard'
import numberKeyboard from './numberKeyboard'
import { required } from 'vuelidate/lib/validators'
import { mdbBtn, mdbInput } from 'mdbvue'
import md5 from 'js-md5'

export default {
  components: {
    mdbBtn, mdbInput, krillKeyboard, numberKeyboard
  },
  validations: {
    login: { required },
    pass: { required }
  },
  data () {
    return {
      pass: '',
      login: '',
      md: '',
      error: '',
      loading: false,
      klavish: false,
      ruskiy: true,
      raqam: false,
      keshRus: ''
    }
  },
  async mounted () {
    try {
      this.loading = true
      const res = await fetch(this.$store.state.hostname + '/WaterAdminUsers')
      this.loading = false
      if (res.status === 200 || res.status === 201) {
        const list = await res.json()
        if (!Array.isArray(list) || list.length === 0) {
          localStorage.Login = 'admin'
          localStorage.AuthId = 1
          localStorage.CompId = 1
          localStorage.CompName = 'Company'
          localStorage.sidebar = false
          localStorage.AccessType = 0
          localStorage.Type = 0
          localStorage.adminLogin = ''
          this.$router.push('/admin/users')
        }
      }
    } catch (e) {
      this.loading = false
    }
  },
  methods: {
    handleHashing (data) {
      this.pass = data
      this.md = md5(data)
    },
    rus_btn () {
      this.$i18n.locale = 'ru'
    },
    matn (option) {
      this.keshRus = option
    },
    num_func (option) {
      this.md = option
    },
    async submit () {
      if (this.$v.$invalid) {
        this.$v.$touch()
        this.$refs.message.warning('please_fill')
        return false
      }
      try {
        this.loading = true
        const base = this.$store.state.hostname
        const url = base + '/WaterAdminAuths/checkAuth?login=' + encodeURIComponent(this.login) + '&password=' + encodeURIComponent(this.md)
        const response = await fetch(url)
        const data = await response.json()
        this.loading = false

        const ok = response.status === 200 || response.status === 201
        if (ok) {
          const userId = data.waterAdminUserid != null ? data.waterAdminUserid : data.WaterAdminUserid
          localStorage.adminLogin = '1'
          localStorage.AdminAuthId = String(data.id)
          localStorage.UserId = userId != null ? String(userId) : ''
          localStorage.AuthId = String(data.id)
          localStorage.Type = data.user_type != null ? String(data.user_type) : '0'
          localStorage.AccessType = data.user_type != null ? String(data.user_type) : '0'
          localStorage.Login = data.login || this.login

          await this.fetchAdminUser(userId)

          this.$router.push('/admin/panel')
          return
        }
        this.$refs.message.error('error_login')
        this.error = this.$i18n.t('error_login')
      } catch (e) {
        this.loading = false
        this.$refs.message.error('network_ne_connect')
      }
    },
    async fetchAdminUser (id) {
      if (id == null || id === '') return false
      try {
        this.loading = true
        const res = await fetch(this.$store.state.hostname + '/WaterAdminUsers/' + id)
        this.loading = false
        if (res.status === 200 || res.status === 201) {
          const u = await res.json()
          localStorage.AdminUserId = String(u.id)
          localStorage.UserId = String(u.id)
          localStorage.AdminUserName = u.fio || ''
          if (u.waterCompanyid != null) {
            localStorage.WaterCompanyid = String(u.waterCompanyid)
          } else if (u.WaterCompanyid != null) {
            localStorage.WaterCompanyid = String(u.WaterCompanyid)
          } else {
            localStorage.WaterCompanyid = ''
          }
          localStorage.UserName = u.fio || ''
          return true
        }
        this.$refs.message.error('network_ne_connect')
        return false
      } catch (e) {
        this.loading = false
        this.$refs.message.error('network_ne_connect')
        return false
      }
    }
  }
}
</script>

<style lang="scss" scoped>
.admin-login-app {
  background: linear-gradient(135deg, #2d3748 0%, #5a67d8 50%, #6b46c1 100%);
}

.admin-title {
  margin-bottom: 6px !important;
}

.admin-badge {
  margin: 0;
  font-size: 13px;
  font-weight: 700;
  letter-spacing: 0.12em;
  text-transform: uppercase;
  color: #5a67d8;
}

.admin-submit-btn {
  background: linear-gradient(135deg, #2d3748 0%, #5a67d8 100%) !important;
  box-shadow: 0 4px 15px rgba(90, 103, 216, 0.45) !important;
}

.login-app {
  min-height: 100vh;
  font-family: 'Open Sans', sans-serif;
  display: flex;
  justify-content: center;
  align-items: center;
  position: relative;
  padding: 20px;
}

.language-switcher {
  position: fixed;
  top: 20px;
  right: 20px;
  z-index: 100;
  .lang-btn {
    padding: 8px 16px;
    border-radius: 20px;
    font-size: 13px;
    font-weight: 600;
    box-shadow: 0 2px 10px rgba(0,0,0,0.1);
  }
}

.login-container {
  width: 100%;
  max-width: 450px;
  display: flex;
  justify-content: center;
  align-items: center;
}

.login-card {
  background: white;
  border-radius: 20px;
  box-shadow: 0 10px 40px rgba(0,0,0,0.2);
  width: 100%;
  padding: 40px;
  animation: slideUp 0.5s ease-out;
}

@keyframes slideUp {
  from { opacity: 0; transform: translateY(30px); }
  to { opacity: 1; transform: translateY(0); }
}

.login-header {
  text-align: center;
  margin-bottom: 30px;
  .login-title {
    color: #333;
    font-size: 28px;
    font-weight: 700;
    margin: 0;
    background: linear-gradient(135deg, #2d3748 0%, #5a67d8 100%);
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
    background-clip: text;
  }
}

.error-message {
  text-align: center;
  margin-bottom: 20px;
  .error-text {
    color: #dc3545;
    font-size: 14px;
    font-weight: 600;
    padding: 10px 15px;
    background: #f8d7da;
    border-radius: 8px;
    display: inline-block;
  }
}

.login-form {
  .form-group {
    margin-bottom: 25px;
    .input-wrapper {
      position: relative;
      display: flex;
      align-items: center;
      background: #f8f9fa;
      border: 2px solid #e0e0e0;
      border-radius: 12px;
      padding: 0 15px;
      transition: all 0.3s;
      &:focus-within {
        border-color: #5a67d8;
        background: white;
        box-shadow: 0 0 0 3px rgba(90, 103, 216, 0.12);
      }
      .input-icon { display: flex; align-items: center; margin-right: 12px; flex-shrink: 0; }
      .form-input {
        flex: 1;
        height: 50px;
        border: none;
        outline: none;
        background: transparent;
        font-size: 16px;
        color: #333;
        font-weight: 500;
        &::placeholder { color: #999; font-weight: 400; }
      }
      .invalid-text {
        position: absolute;
        top: 100%;
        left: 0;
        margin-top: 5px;
        color: #dc3545;
        font-size: 12px;
        font-weight: 600;
      }
    }
  }
  .form-actions {
    margin-top: 30px;
    .submit-btn {
      width: 100%;
      height: 50px;
      border-radius: 12px;
      font-size: 16px;
      font-weight: 600;
      border: none;
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 10px;
    }
  }
}

@media (max-width: 767px) {
  .login-app { padding: 15px; }
  .login-card { padding: 30px 20px; border-radius: 16px; }
  .login-header .login-title { font-size: 24px; }
}
</style>
