<template>
  <div class="admin-users admin-companies">
    <loader v-if="loading"/>

    <div class="admin-users__hero">
      <div class="admin-users__hero-text">
        <div class="admin-users__badge">
          <i class="fas fa-building"/>
        </div>
        <div>
          <h1 class="admin-users__title">{{ $t('company_name') }}</h1>
          <p class="admin-users__desc">{{ $t('Add_company') }}</p>
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
          <i class="fas fa-building"/>
        </div>
        <p class="admin-users__empty-title">Нет записей</p>
        <p class="admin-users__empty-hint">Нажмите «{{ $t('add') }}», чтобы добавить компанию</p>
        <button type="button" class="admin-users__cta admin-users__cta--ghost" @click="openModal(null)">
          <i class="fas fa-plus"/>
          {{ $t('add') }}
        </button>
      </div>

      <div v-else class="admin-users__table-wrap">
        <table class="admin-users__table">
          <thead>
            <tr>
              <th>#</th>
              <th>{{ $t('company_name') }}</th>
              <th>{{ $t('phoneNumber') }}</th>
              <th>{{ $t('address') }}</th>
              <th>{{ $t('payment_date') }}</th>
              <th>{{ $t('sum') }}</th>
              <th class="admin-users__th-actions"/>
            </tr>
          </thead>
          <tbody>
            <tr v-for="row in list" :key="row.id">
              <td>
                <span class="admin-users__id">{{ row.id }}</span>
              </td>
              <td>
                <span class="admin-users__name">{{ row.name || '—' }}</span>
              </td>
              <td>{{ row.phone_number ? formatPhoneDisplay(row.phone_number) : '—' }}</td>
              <td>{{ row.address || '—' }}</td>
              <td class="admin-companies__cell-date">{{ formatTableDate(row.payment_date) }}</td>
              <td>{{ formatMoney(row.payment_amount) }}</td>
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
      :title="isEditing ? $t('edit') : $t('Add_company')"
      width="520px"
      @close="closeModal"
    >
      <template v-slot:body>
        <div class="admin-modal">
          <form class="admin-modal__form" @submit.prevent="submitCompany">
            <div class="admin-modal__field">
              <label>{{ $t('company_name') }} *</label>
              <input v-model.trim="form.name" type="text" class="admin-modal__input" required>
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
              <label>{{ $t('address') }}</label>
              <input v-model.trim="form.address" type="text" class="admin-modal__input">
            </div>
            <div class="admin-modal__field">
              <label>{{ $t('start_date') }}</label>
              <input v-model="form.start_date" type="date" class="admin-modal__input">
            </div>
            <div class="admin-modal__field">
              <label>{{ $t('payment_date') }}</label>
              <input v-model="form.payment_date" type="date" class="admin-modal__input">
            </div>
            <div class="admin-modal__field">
              <label>{{ $t('payment_amount') }}</label>
              <input
                :value="payment_amount_display"
                type="text"
                inputmode="decimal"
                class="admin-modal__input"
                placeholder="2 000"
                @input="onPaymentAmountInput"
              >
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
export default {
  name: 'AdminCompanies',
  data () {
    return {
      loading: false,
      saving: false,
      modalOpen: false,
      list: [],
      form: {
        id: 0,
        name: '',
        phone_number: '',
        address: '',
        start_date: '',
        payment_date: '',
        payment_amount: 0
      },
      payment_amount_display: ''
    }
  },
  computed: {
    isEditing () {
      return this.form.id > 0
    }
  },
  mounted () {
    this.loadList()
  },
  methods: {
    api () {
      return this.$store.state.hostname
    },
    toInputDate (v) {
      if (v == null || v === '') return ''
      const d = new Date(v)
      if (isNaN(d.getTime())) return ''
      const y = d.getFullYear()
      const m = String(d.getMonth() + 1).padStart(2, '0')
      const day = String(d.getDate()).padStart(2, '0')
      return `${y}-${m}-${day}`
    },
    toIsoDate (yyyyMmDd) {
      if (!yyyyMmDd) return null
      const d = new Date(yyyyMmDd + 'T12:00:00')
      return isNaN(d.getTime()) ? null : d.toISOString()
    },
    formatMoney (n) {
      if (n == null || n === '') return '—'
      const x = Number(n)
      if (isNaN(x)) return '—'
      return x.toLocaleString(undefined, { minimumFractionDigits: 0, maximumFractionDigits: 2 })
    },
    /** Jadvalda sana: DD.MM.YYYY */
    formatTableDate (v) {
      if (v == null || v === '') return '—'
      const d = new Date(v)
      if (isNaN(d.getTime())) return '—'
      const day = String(d.getDate()).padStart(2, '0')
      const m = String(d.getMonth() + 1).padStart(2, '0')
      const y = d.getFullYear()
      return `${day}.${m}.${y}`
    },
    /** (XX) XXX-XXXX — 9 raqamgacha, inputPhone.vue bilan mos */
    formatPhoneDisplay (raw) {
      const d = String(raw || '').replace(/\D/g, '').slice(0, 9)
      const x = d.match(/(\d{0,2})(\d{0,3})(\d{0,4})/)
      if (!x) return ''
      return !x[2] ? x[1] : '(' + x[1] + ') ' + x[2] + (x[3] ? '-' + x[3] : '')
    },
    onPhoneInput (e) {
      this.form.phone_number = this.formatPhoneDisplay(e.target.value)
    },
    /** Mingliklar bo‘shliq bilan: 2000 → "2 000", kasr — nuqta */
    formatAmountSpaces (num) {
      if (num == null || num === '' || (typeof num === 'number' && isNaN(num))) return ''
      const n = Number(num)
      if (isNaN(n)) return ''
      const fixed = n.toFixed(2)
      const [intp, dec] = fixed.split('.')
      const intWithSpaces = intp.replace(/\B(?=(\d{3})+(?!\d))/g, ' ')
      if (dec === '00') return intWithSpaces
      return intWithSpaces + '.' + dec
    },
    onPaymentAmountInput (e) {
      let raw = String(e.target.value || '').replace(/\s/g, '').replace(',', '.')
      if (raw === '' || raw === '.') {
        this.payment_amount_display = ''
        this.form.payment_amount = 0
        return
      }
      const dotCount = (raw.match(/\./g) || []).length
      if (dotCount > 1) {
        raw = raw.replace(/\.(?=.*\.)/g, '')
      }
      const n = parseFloat(raw)
      if (isNaN(n) || n < 0) {
        return
      }
      this.form.payment_amount = n
      this.payment_amount_display = this.formatAmountSpaces(n)
    },
    openModal (row) {
      this.resetForm()
      if (row && row.id) {
        this.form.id = row.id
        this.form.name = row.name || ''
        this.form.phone_number = this.formatPhoneDisplay(row.phone_number || '')
        this.form.address = row.address || ''
        this.form.start_date = this.toInputDate(row.start_date)
        this.form.payment_date = this.toInputDate(row.payment_date)
        this.form.payment_amount = row.payment_amount != null ? Number(row.payment_amount) : 0
        this.payment_amount_display = this.formatAmountSpaces(this.form.payment_amount)
      }
      this.modalOpen = true
    },
    closeModal () {
      this.modalOpen = false
      this.resetForm()
    },
    resetForm () {
      this.form = {
        id: 0,
        name: '',
        phone_number: '',
        address: '',
        start_date: '',
        payment_date: '',
        payment_amount: 0
      }
      this.payment_amount_display = ''
    },
    async loadList () {
      try {
        this.loading = true
        const res = await fetch(this.api() + '/WaterCompanies')
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
    async submitCompany () {
      if (!this.form.name) {
        this.$refs.message && this.$refs.message.warning('please_fill')
        return
      }
      try {
        this.saving = true
        const body = {
          id: this.isEditing ? this.form.id : 0,
          active_status: true,
          name: this.form.name,
          phone_number: this.form.phone_number || null,
          address: this.form.address || null,
          start_date: this.toIsoDate(this.form.start_date),
          payment_date: this.toIsoDate(this.form.payment_date),
          payment_amount: Number(this.form.payment_amount) || 0
        }
        const res = await fetch(this.api() + '/WaterCompanies', {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify(body)
        })
        this.saving = false
        if (res.status === 200 || res.status === 201) {
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
        const res = await fetch(this.api() + '/WaterCompanies/' + row.id, { method: 'DELETE' })
        this.loading = false
        if (res.status === 200 || res.status === 201) {
          await this.loadList()
        } else {
          this.$refs.message && this.$refs.message.error('Failed_to_delete')
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
  letter-spacing: -0.01em;
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
  box-shadow: 0 1px 2px rgba(15, 23, 42, 0.04);
  transition: border-color 0.15s, box-shadow 0.15s;
  &:hover {
    border-color: #c7d2fe;
    box-shadow: 0 2px 6px rgba(0, 0, 0, 0.05);
  }
}
.admin-users__cta--ghost {
  background: #fff;
  color: #4338ca;
  border: 1px solid #e2e8f0;
  margin-top: 0.35rem;
  &:hover {
    background: #f8fafc;
  }
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
  max-width: 280px;
  margin-left: auto;
  margin-right: auto;
  line-height: 1.35;
}

.admin-users__table-wrap {
  overflow-x: auto;
}
.admin-companies {
  .admin-users__table {
    width: 100%;
    border-collapse: collapse;
    font-size: 0.58rem;
    th {
      text-align: left;
      padding: 0.28rem 0.4rem;
      font-weight: 600;
      color: #94a3b8;
      font-size: 0.59rem;
      text-transform: uppercase;
      letter-spacing: 0.02em;
      background: #fafbfc;
    }
    td {
      padding: 0.28rem 0.4rem;
      border-top: 1px solid #f1f3f6;
      color: #475569;
      vertical-align: middle;
      font-size: 0.75rem;
    }
    tbody tr {
      transition: background 0.12s;
    }
    tbody tr:hover {
      background: #fafbff;
    }
  }
  .admin-users__th-actions {
    width: 62px;
  }
  .admin-users__id {
    font-size: 0.54rem;
    padding: 0.06rem 0.28rem;
  }
  .admin-users__name {
    font-size: 0.69rem;
  }
  .admin-companies__cell-date {
    white-space: nowrap;
    font-variant-numeric: tabular-nums;
    color: #64748b;
  }
  .admin-users__icon-btn {
    width: 24px;
    height: 24px;
    font-size: 0.62rem;
  }
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
.admin-users__cell-actions {
  text-align: right;
  white-space: nowrap;
}
.admin-users__icon-btn {
  width: 28px;
  height: 28px;
  border: none;
  border-radius: 6px;
  cursor: pointer;
  font-size: 0.7rem;
  display: inline-flex;
  align-items: center;
  justify-content: center;
  transition: background 0.15s, color 0.15s;
  vertical-align: middle;
  & + & {
    margin-left: 0.2rem;
  }
  &--edit {
    background: #eef2ff;
    color: #4338ca;
    &:hover {
      background: #e0e7ff;
    }
  }
  &--danger {
    background: #fef2f2;
    color: #dc2626;
    &:hover {
      background: #fee2e2;
    }
  }
}
</style>

<style lang="scss">
.admin-modal {
  padding: 0.55rem 0.75rem 0.75rem;
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
.admin-modal__input {
  width: 100%;
  border: 1px solid #e2e8f0;
  border-radius: 7px;
  padding: 0.35rem 0.5rem;
  font-size: 0.75rem;
  transition: border-color 0.15s, box-shadow 0.15s;
  &:focus {
    outline: none;
    border-color: #a5b4fc;
    box-shadow: 0 0 0 2px rgba(99, 102, 241, 0.12);
  }
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
    &:hover {
      background: #e2e8f0;
    }
  }
  &--primary {
    background: linear-gradient(90deg, #6366f1, #818cf8);
    color: #fff;
    &:hover:not(:disabled) {
      filter: brightness(1.03);
    }
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
