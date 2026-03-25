<template>
  <div class="admin-users">
    <loader v-if="loading"/>

    <div class="admin-users__hero">
      <div class="admin-users__hero-text">
        <button type="button" class="admin-users__back" @click="$router.push('/admin/companies')">
          <i class="fas fa-chevron-left"/>
        </button>
        <div class="admin-users__badge">
          <i class="fas fa-users"/>
        </div>
        <div>
          <h1 class="admin-users__title">{{ company.name || ('Company #' + companyId) }} — {{ $t('user') }}</h1>
          <p class="admin-users__desc">{{ $t('Add_user') }}</p>
        </div>
      </div>
      <button type="button" class="admin-users__cta" @click="openModal(null)">
        <i class="fas fa-plus"/>
        <span>{{ $t('add') }}</span>
      </button>
    </div>

    <div class="admin-users__panel">
      <div class="admin-users__panel-head">
        <span class="admin-users__count">{{ list.length }}</span>
        <span class="admin-users__count-label">записей</span>
      </div>

      <div v-if="!list.length" class="admin-users__empty">
        <div class="admin-users__empty-icon">
          <i class="fas fa-users"/>
        </div>
        <p class="admin-users__empty-title">Нет записей</p>
        <p class="admin-users__empty-hint">Нажмите «{{ $t('add') }}», чтобы добавить сотрудника</p>
      </div>

      <div v-else class="admin-users__table-wrap">
        <table class="admin-users__table">
          <thead>
            <tr>
              <th>#</th>
              <th>{{ $t('fio') }}</th>
              <th>{{ $t('phoneNumber') }}</th>
              <th>{{ $t('position') }}</th>
              <th>Login</th>
              <th class="admin-users__th-actions"/>
            </tr>
          </thead>
          <tbody>
            <tr v-for="row in list" :key="row.id">
              <td>
                <span class="admin-users__id">{{ row.id }}</span>
              </td>
              <td>
                <span class="admin-users__name">{{ row.fio || '—' }}</span>
              </td>
              <td>{{ row.phone_number ? formatPhoneDisplay(row.phone_number) : '—' }}</td>
              <td>{{ row.position || '—' }}</td>
              <td>
                <span class="admin-users__login">{{ row.auth && row.auth.login ? row.auth.login : '—' }}</span>
              </td>
              <td class="admin-users__cell-actions">
                <button
                  type="button"
                  class="admin-users__icon-btn admin-users__icon-btn--edit"
                  :title="$t('edit')"
                  @click="openModal(row)"
                >
                  <i class="fas fa-pen"/>
                </button>
                <button
                  type="button"
                  class="admin-users__icon-btn admin-users__icon-btn--danger"
                  :title="$t('delete')"
                  @click="remove(row)"
                >
                  <i class="fas fa-trash-alt"/>
                </button>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <modal-train
      :show="modalOpen"
      headerbackColor="primary"
      titlecolor="white"
      :title="isEditing ? $t('edit') : $t('Add_user')"
      width="520px"
      @close="closeModal"
    >
      <template v-slot:body>
        <div class="admin-modal">
          <form class="admin-modal__form" @submit.prevent="submitUser">
            <p class="admin-modal__section">{{ $t('user') }}</p>
            <div class="admin-modal__field">
              <label>{{ $t('fio') }} *</label>
              <input v-model.trim="form.fio" type="text" class="admin-modal__input" required>
            </div>
            <div class="admin-modal__field">
              <label>{{ $t('phoneNumber') }}</label>
              <input
                :value="form.phone_number"
                type="text"
                inputmode="tel"
                autocomplete="tel"
                maxlength="14"
                class="admin-modal__input"
                placeholder="(90) 123-4567"
                @input="onPhoneInput"
              >
            </div>
            <div class="admin-modal__field">
              <label>{{ $t('position') }}</label>
              <input v-model.trim="form.position" type="text" class="admin-modal__input">
            </div>
            <div class="admin-modal__field">
              <label>{{ $t('address') }}</label>
              <input v-model.trim="form.addrress" type="text" class="admin-modal__input">
            </div>
            <div class="admin-modal__field">
              <label>{{ $t('note') }}</label>
              <textarea v-model.trim="form.note" class="admin-modal__input admin-modal__input--area" rows="2"/>
            </div>

            <p class="admin-modal__section">{{ $t('authorization') }}</p>
            <div class="admin-modal__field">
              <label>Login *</label>
              <input v-model.trim="form.login" type="text" class="admin-modal__input" required autocomplete="username">
            </div>
            <div class="admin-modal__field">
              <label>{{ $t('password') }}{{ isEditing ? '' : ' *' }}</label>
              <input
                v-model="form.password"
                type="password"
                class="admin-modal__input"
                :required="!isEditing"
                autocomplete="new-password"
              >
              <p v-if="isEditing" class="admin-modal__hint">{{ $t('password_leave_blank') }}</p>
            </div>

            <div class="admin-modal__footer">
              <button type="button" class="admin-modal__btn admin-modal__btn--muted" @click="closeModal">
                {{ $t('cancel') }}
              </button>
              <button type="submit" class="admin-modal__btn admin-modal__btn--primary" :disabled="saving">
                <span v-if="saving" class="admin-modal__spin">{{ isEditing ? $t('save') + '…' : $t('add') + '…' }}</span>
                <span v-else>{{ isEditing ? $t('save') : $t('add') }}</span>
              </button>
            </div>
          </form>
        </div>
      </template>
    </modal-train>

    <Toast ref="message"/>
  </div>
</template>

<script>
import md5 from 'js-md5'

export default {
  name: 'AdminCompanyUsers',
  data () {
    return {
      loading: false,
      saving: false,
      modalOpen: false,
      list: [],
      company: {},
      form: {
        userId: 0,
        authId: 0,
        fio: '',
        phone_number: '',
        position: '',
        addrress: '',
        note: '',
        login: '',
        password: ''
      }
    }
  },
  computed: {
    companyId () {
      const id = Number(this.$route.params.companyId)
      return isNaN(id) ? 0 : id
    },
    isEditing () {
      return this.form.userId > 0
    }
  },
  mounted () {
    this.initPage()
  },
  methods: {
    api () {
      return this.$store.state.hostname
    },
    authHeaders () {
      const headers = {}
      if (localStorage.AuthToken) {
        headers.Authorization = 'Bearer ' + localStorage.AuthToken
      }
      return headers
    },
    formatPhoneDisplay (raw) {
      const d = String(raw || '').replace(/\D/g, '').slice(0, 9)
      const x = d.match(/(\d{0,2})(\d{0,3})(\d{0,4})/)
      if (!x) return ''
      return !x[2] ? x[1] : '(' + x[1] + ') ' + x[2] + (x[3] ? '-' + x[3] : '')
    },
    onPhoneInput (e) {
      this.form.phone_number = this.formatPhoneDisplay(e.target.value)
    },
    async initPage () {
      if (!this.companyId) {
        this.$router.push('/admin/companies')
        return
      }
      await this.loadCompany()
      await this.loadList()
    },
    async loadCompany () {
      try {
        const res = await fetch(this.api() + '/WaterCompanies/' + this.companyId)
        if (res.status === 200 || res.status === 201) {
          this.company = await res.json()
        }
      } catch (e) {
        this.company = {}
      }
    },
    async loadList () {
      try {
        this.loading = true
        const res = await fetch(this.api() + '/WaterUsers?company_id=' + this.companyId, {
          headers: this.authHeaders()
        })
        this.loading = false
        if (res.status === 200 || res.status === 201) {
          const data = await res.json()
          this.list = Array.isArray(data) ? data : []
        } else {
          this.$refs.message && this.$refs.message.error('network_ne_connect')
        }
      } catch (e) {
        this.loading = false
        this.$refs.message && this.$refs.message.error('network_ne_connect')
      }
    },
    openModal (row) {
      this.resetForm()
      if (row && row.id) {
        this.form.userId = row.id
        this.form.authId = row.auth_id || (row.auth && row.auth.id) || 0
        this.form.fio = row.fio || ''
        this.form.phone_number = this.formatPhoneDisplay(row.phone_number || '')
        this.form.position = row.position || ''
        this.form.addrress = row.addrress || ''
        this.form.note = row.note || ''
        this.form.login = (row.auth && row.auth.login) ? row.auth.login : ''
        this.form.password = ''
      }
      this.modalOpen = true
    },
    closeModal () {
      this.modalOpen = false
      this.resetForm()
    },
    resetForm () {
      this.form = {
        userId: 0,
        authId: 0,
        fio: '',
        phone_number: '',
        position: '',
        addrress: '',
        note: '',
        login: '',
        password: ''
      }
    },
    async submitUser () {
      const isEdit = this.isEditing
      if (!this.form.fio || !this.form.login) {
        this.$refs.message && this.$refs.message.warning('please_fill')
        return
      }
      if (!isEdit && !this.form.password) {
        this.$refs.message && this.$refs.message.warning('please_fill')
        return
      }
      try {
        this.saving = true
        let passwordHash = null
        if (isEdit) {
          if (this.form.password) {
            passwordHash = md5(this.form.password)
          } else if (this.form.authId) {
            const resAuthGet = await fetch(this.api() + '/WaterAuths/' + this.form.authId)
            if (resAuthGet.status !== 200) {
              this.saving = false
              this.$refs.message && this.$refs.message.error('network_ne_connect')
              return
            }
            const existingAuth = await resAuthGet.json()
            passwordHash = existingAuth.password != null ? existingAuth.password : existingAuth.Password
          } else {
            this.saving = false
            this.$refs.message && this.$refs.message.warning('please_fill')
            return
          }
        } else {
          passwordHash = md5(this.form.password)
        }

        const userBody = {
          id: isEdit ? this.form.userId : 0,
          active_status: true,
          fio: this.form.fio,
          phone_number: this.form.phone_number || null,
          position: this.form.position || null,
          addrress: this.form.addrress || null,
          note: this.form.note || null,
          car_number: null,
          telegram_phonenumber: null,
          bot_id: null,
          company_id: this.companyId
        }
        const resUser = await fetch(this.api() + '/WaterUsers', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(userBody)
        })
        if (resUser.status !== 200 && resUser.status !== 201) {
          this.saving = false
          this.$refs.message && this.$refs.message.error('Failed_to_add')
          return
        }
        const created = await resUser.json()
        const uid = isEdit ? this.form.userId : (created.id != null ? created.id : created.Id)
        const authBody = {
          id: isEdit ? this.form.authId : 0,
          active_status: true,
          login: this.form.login,
          password: passwordHash,
          user_type: 0,
          client_type_info: 0,
          waterUserid: uid,
          company_id: this.companyId
        }
        const resAuth = await fetch(this.api() + '/WaterAuths', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(authBody)
        })
        this.saving = false
        if (resAuth.status === 200 || resAuth.status === 201) {
          this.$refs.message && this.$refs.message.success('Added_successfully')
          this.closeModal()
          await this.loadList()
        } else {
          this.$refs.message && this.$refs.message.error('Failed_to_add')
        }
      } catch (e) {
        this.saving = false
        this.$refs.message && this.$refs.message.error('network_ne_connect')
      }
    },
    async remove (row) {
      if (!row || !row.id) return
      if (!window.confirm('OK?')) return
      try {
        this.loading = true
        if (row.auth_id || (row.auth && row.auth.id)) {
          const aid = row.auth_id || (row.auth && row.auth.id)
          await fetch(this.api() + '/WaterAuths/' + aid, { method: 'DELETE' })
        }
        const res = await fetch(this.api() + '/WaterUsers/' + row.id, { method: 'DELETE' })
        this.loading = false
        if (res.status === 200 || res.status === 201) {
          await this.loadList()
        } else {
          this.$refs.message && this.$refs.message.error('Failed_to_add')
        }
      } catch (e) {
        this.loading = false
        this.$refs.message && this.$refs.message.error('network_ne_connect')
      }
    }
  }
}
</script>

<style lang="scss" scoped>
.admin-users {
  margin: 0 auto;
  padding: 0.25rem;
}

.admin-users__hero {
  display: flex;
  align-items: center;
  justify-content: space-between;
  gap: 0.75rem;
  flex-wrap: wrap;
  margin-bottom: 0.75rem;
  padding: 0.65rem 0.85rem;
  background: linear-gradient(135deg, #f8fafc 0%, #f1f5f9 35%, #eef2ff 100%);
  border-radius: 10px;
  border: 1px solid #e2e8f0;
  box-shadow: 0 1px 2px rgba(15, 23, 42, 0.04);
}

.admin-users__hero-text {
  display: flex;
  align-items: center;
  gap: 0.65rem;
}

.admin-users__back {
  width: 28px;
  height: 28px;
  border: 1px solid #e2e8f0;
  border-radius: 6px;
  cursor: pointer;
  background: #fff;
  color: #475569;
}

.admin-users__badge {
  width: 36px;
  height: 36px;
  border-radius: 9px;
  background: #e0e7ff;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #4f46e5;
  font-size: 0.95rem;
}

.admin-users__title {
  margin: 0;
  font-size: 0.95rem;
  font-weight: 700;
  color: #334155;
}

.admin-users__desc {
  margin: 0.15rem 0 0;
  font-size: 0.7rem;
  color: #94a3b8;
}

.admin-users__cta {
  display: inline-flex;
  align-items: center;
  gap: 0.35rem;
  padding: 0.4rem 0.75rem;
  border: 1px solid #e2e8f0;
  border-radius: 8px;
  font-size: 0.75rem;
  font-weight: 600;
  cursor: pointer;
  color: #4338ca;
  background: #fff;
}

.admin-users__panel {
  background: #fff;
  border-radius: 10px;
  box-shadow: 0 1px 3px rgba(15, 23, 42, 0.05);
  border: 1px solid #e8e8ef;
  overflow: hidden;
}

.admin-users__panel-head {
  padding: 0.45rem 0.65rem;
  background: #fafbfc;
  border-bottom: 1px solid #ebeef2;
  font-size: 0.7rem;
  color: #94a3b8;
}

.admin-users__count {
  font-weight: 700;
  color: #475569;
  font-size: 0.8rem;
}

.admin-users__count-label {
  margin-left: 0.25rem;
}

.admin-users__empty {
  padding: 1.75rem 0.85rem;
  text-align: center;
}

.admin-users__empty-icon {
  width: 48px;
  height: 48px;
  margin: 0 auto 0.6rem;
  border-radius: 50%;
  background: #f1f5f9;
  color: #94a3b8;
  display: flex;
  align-items: center;
  justify-content: center;
  font-size: 1.1rem;
}

.admin-users__empty-title {
  margin: 0;
  font-size: 0.8rem;
  font-weight: 600;
  color: #64748b;
}

.admin-users__empty-hint {
  margin: 0.35rem 0 0;
  font-size: 0.68rem;
  color: #a8b0bd;
}

.admin-users__table-wrap {
  overflow-x: auto;
}

.admin-users__table {
  width: 100%;
  border-collapse: collapse;
  font-size: 0.72rem;
  th {
    text-align: left;
    padding: 0.28rem 0.4rem;
    font-weight: 600;
    color: #94a3b8;
    font-size: 0.62rem;
    text-transform: uppercase;
    letter-spacing: 0.03em;
    background: #fafbfc;
  }
  td {
    padding: 0.28rem 0.4rem;
    border-top: 1px solid #f1f3f6;
    color: #475569;
    vertical-align: middle;
    font-size: 0.75rem;
  }
}

.admin-users__th-actions {
  width: 68px;
}

.admin-users__id {
  display: inline-block;
  min-width: 1.5rem;
  padding: 0.1rem 0.35rem;
  border-radius: 4px;
  background: #f1f5f9;
  font-size: 0.65rem;
  font-weight: 600;
  color: #64748b;
}

.admin-users__name {
  font-weight: 600;
  color: #334155;
  font-size: 0.72rem;
}

.admin-users__login {
  font-family: ui-monospace, monospace;
  font-size: 0.68rem;
  color: #6366f1;
}

.admin-users__cell-actions {
  text-align: right;
  white-space: nowrap;
}

.admin-users__icon-btn {
  width: 24px;
  height: 24px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 0.7rem;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  & + & {
    margin-left: 0.2rem;
  }
  &--edit {
    background: #eef2ff;
    color: #4338ca;
  }
  &--danger {
    background: #fef2f2;
    color: #dc2626;
  }
}
</style>

<style lang="scss">
.admin-modal {
  padding: 0.55rem 0.75rem 0.75rem;
}
.admin-modal__section {
  margin: 0 0 0.45rem;
  font-size: 0.6rem;
  font-weight: 700;
  text-transform: uppercase;
  letter-spacing: 0.05em;
  color: #a8b0bd;
}
.admin-modal__section:not(:first-child) {
  margin-top: 0.75rem;
  padding-top: 0.65rem;
  border-top: 1px solid #ebeef2;
}
.admin-modal__field {
  margin-bottom: 0.5rem;
  label {
    display: block;
    font-size: 0.68rem;
    font-weight: 600;
    color: #64748b;
    margin-bottom: 0.2rem;
  }
}
.admin-modal__hint {
  margin: 0.25rem 0 0;
  font-size: 0.62rem;
  color: #94a3b8;
}
.admin-modal__input {
  width: 100%;
  border: 1px solid #e2e8f0;
  border-radius: 7px;
  padding: 0.35rem 0.5rem;
  font-size: 0.75rem;
}
.admin-modal__input--area {
  resize: vertical;
  min-height: 44px;
}
.admin-modal__footer {
  display: flex;
  justify-content: flex-end;
  gap: 0.45rem;
  margin-top: 0.65rem;
  padding-top: 0.65rem;
  border-top: 1px solid #ebeef2;
}
.admin-modal__btn {
  padding: 0.35rem 0.75rem;
  border-radius: 7px;
  font-size: 0.72rem;
  font-weight: 600;
  cursor: pointer;
  border: none;
  &--muted {
    background: #f1f5f9;
    color: #64748b;
  }
  &--primary {
    background: linear-gradient(90deg, #6366f1, #818cf8);
    color: #fff;
  }
  &:disabled {
    opacity: 0.65;
    cursor: not-allowed;
  }
}
.admin-modal__spin {
  opacity: 0.9;
}
</style>
