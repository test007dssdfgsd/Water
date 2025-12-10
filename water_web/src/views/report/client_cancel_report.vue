<template>
  <div class="cancel-report-app">
    <backRouter />
    <div class="report-header">
      <h4 class="report-title">Klientlarni otmen qilish</h4>
    </div>

    <div class="filter-section">
      <div class="date-filter">
        <label class="filter-label">{{$t('date')}}</label>
        <mdb-input 
          class="date-input" 
          size="sm" 
          v-model="selectedDate" 
          @change="fetchAddresses"
          type="date"
        ></mdb-input>
      </div>
      <div class="action-buttons-top">
        <mdb-btn 
          color="info" 
          @click="fetchAddresses"
          class="filter-btn"
          size="sm"
        >
          <i class="fas fa-search mr-2"></i>
          {{$t('search')}}
        </mdb-btn>
        <mdb-btn 
          color="success" 
          @click="selectAll"
          class="filter-btn"
          size="sm"
        >
          <i class="fas fa-check-square mr-2"></i>
          Hammasini tanlash
        </mdb-btn>
        <mdb-btn 
          color="warning" 
          @click="deselectAll"
          class="filter-btn"
          size="sm"
        >
          <i class="fas fa-square mr-2"></i>
          Tanlovni bekor qilish
        </mdb-btn>
      </div>
    </div>

    <div class="clients-table-container">
      <loader-table v-if="loading" />
      <div v-else class="clients-table-wrapper">
        <table class="clients-table">
          <thead>
            <tr>
              <th width="40">
                <input 
                  type="checkbox" 
                  :checked="selectedAddresses.length === addresses.length && addresses.length > 0"
                  @change="toggleSelectAll"
                  class="select-all-checkbox"
                />
              </th>
              <th width="50">№</th>
              <th>{{$t('fio')}}</th>
              <th>Telefon</th>
              <th>{{$t('address')}}</th>
              <th width="100">Baklashka</th>
              <th>Tuman</th>
              <th>Oxirgi zakaz sana</th>
              <th>Oxirgi zakaz note</th>
              <th width="60" class="text-center">Info</th>
            </tr>
          </thead>
          <tbody>
            <tr 
              v-for="(item, index) in addresses"
              :key="item.address_id"
              class="client-row"
              :class="{ 'selected': selectedAddresses.includes(item.address_id) }"
              @click="toggleAddress(item.address_id)"
            >
                <td>
                  <input 
                    type="checkbox" 
                    :checked="selectedAddresses.includes(item.address_id)"
                    @change="toggleAddress(item.address_id)"
                    @click.stop
                    class="client-checkbox"
                  />
                </td>
                <td>{{index + 1}}</td>
                <td class="font-weight-bold">{{item.client.fio}}</td>
                <td>
                  <span v-if="item.client.phone_numbers_list && item.client.phone_numbers_list.length > 0">
                    {{formatPhone(item.client.phone_numbers_list[0].phone_number)}}
                    <span v-if="item.client.phone_numbers_list.length > 1" class="phone-count">
                      (+{{item.client.phone_numbers_list.length - 1}})
                    </span>
                  </span>
                  <span v-else class="text-muted">-</span>
                </td>
                <td>
                  <div class="address-item">
                    <i class="fas fa-map-marker-alt mr-1"></i>
                    {{item.address.address}}
                  </div>
                </td>
                <td class="text-center">
                  <span v-if="item.bottle_count > 0" class="bottle-badge">
                    {{item.bottle_count}} ta
                  </span>
                  <span v-else class="text-muted">0</span>
                </td>
                <td>
                  <span v-if="item.address.tuman">
                    {{item.address.tuman.name}}
                  </span>
                  <span v-else class="text-muted">-</span>
                </td>
                <td>
                  <span v-if="item.last_order && item.last_order.order_date" class="last-order-date">
                    {{formatDate(item.last_order.order_date)}}
                  </span>
                  <span v-else class="text-muted">Zakaz yo'q</span>
                </td>
              <td>
                <span v-if="item.last_order && item.last_order.note" class="last-order-note">
                  {{item.last_order.note}}
                </span>
                <span v-else class="text-muted">-</span>
              </td>
              <td class="text-center">
                <button class="info-btn" @click.stop="openHistory(item)">
                  <i class="fas fa-info-circle"></i>
                </button>
              </td>
              </tr>
            
            <tr v-if="addresses.length === 0">
              <td colspan="10" class="empty-state">
                <i class="fas fa-inbox"></i>
                <p>Zakaz bermagan manzillar topilmadi</p>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <div class="cancel-actions" v-if="selectedAddresses.length > 0">
      <div class="selected-count">
        <i class="fas fa-check-circle mr-2"></i>
        Tanlangan: <strong>{{selectedAddresses.length}}</strong> ta manzil
      </div>
      <mdb-btn 
        color="danger" 
        @click="cancelSelectedAddresses"
        :disabled="cancelling"
        class="cancel-btn"
      >
        <i class="fas fa-times-circle mr-2"></i>
        <span v-if="!cancelling">Tanlangan manzillarni otmen qilish</span>
        <span v-else>Jarayonda...</span>
      </mdb-btn>
    </div>

    <modal-train :show="historyModal.show" headerbackColor="white" titlecolor="black" title="Address zakaz tarixi" @close="historyModal.show = false" width="80%">
      <template v-slot:body>
        <AddressOrderHistory 
          :loading="historyModal.loading"
          :data="historyModal.data"
        />
      </template>
      <template v-slot:footer>
        <div class="d-flex justify-content-end w-100 px-3 pb-2">
          <mdb-btn color="secondary" size="sm" @click="historyModal.show = false">Yopish</mdb-btn>
        </div>
      </template>
    </modal-train>

    <Toast ref="message"></Toast>
  </div>
</template>

<script>
import { mdbBtn, mdbInput } from 'mdbvue'
import loaderTable from '../../components/loaderTable.vue'
import AddressOrderHistory from '../../components/AddressOrderHistory.vue'
import { mapActions } from 'vuex'

export default {
  components: {
    mdbBtn,
    mdbInput,
    loaderTable,
    AddressOrderHistory
  },
  data() {
    return {
      loading: false,
      cancelling: false,
      selectedDate: '',
      addresses: [],
      selectedAddresses: [],
      historyModal: {
        show: false,
        loading: false,
        data: null
      }
    }
  },
  async mounted() {
    // Bugungi sanani default qilib qo'yish
    const today = new Date()
    this.selectedDate = today.toISOString().slice(0, 10)
    await this.fetchAddresses()
  },
  methods: {
    async fetchAddresses() {
      if (!this.selectedDate) {
        this.$refs.message.warning('Iltimos, sanani tanlang')
        return
      }

      try {
        this.loading = true
        const response = await fetch(
          this.$store.state.hostname + 
          `/WaterOrders/getAddressesWithoutOrdersAfterDate?date=${this.selectedDate}`
        )
        
        if (response.status === 200 || response.status === 201) {
          const data = await response.json()
          this.addresses = data
          this.selectedAddresses = [] // Tanlovni tozalash
        } else {
          this.$refs.message.error('network_ne_connect')
        }
      } catch (error) {
        console.error(error)
        this.$refs.message.error('network_ne_connect')
      } finally {
        this.loading = false
      }
    },
    toggleAddress(addressId) {
      const index = this.selectedAddresses.indexOf(addressId)
      if (index > -1) {
        this.selectedAddresses.splice(index, 1)
      } else {
        this.selectedAddresses.push(addressId)
      }
    },
    selectAll() {
      this.selectedAddresses = this.addresses.map(a => a.address_id)
    },
    deselectAll() {
      this.selectedAddresses = []
    },
    toggleSelectAll(event) {
      if (event.target.checked) {
        this.selectAll()
      } else {
        this.deselectAll()
      }
    },
    formatDate(dateString) {
      if (!dateString) return ''
      try {
        const date = new Date(dateString)
        return date.toLocaleDateString('uz-UZ', {
          year: 'numeric',
          month: '2-digit',
          day: '2-digit'
        })
      } catch {
        return dateString
      }
    },
    async openHistory(item) {
      this.historyModal.show = true
      this.historyModal.loading = true
      this.historyModal.data = null
      try {
        const response = await fetch(
          this.$store.state.hostname + `/WaterOrders/getAddressOrderHistory?addressId=${item.address_id}`
        )
        if (response.status === 200 || response.status === 201) {
          const data = await response.json()
          console.log('Address order history data:', data)
          this.historyModal.data = data
        } else {
          const err = await response.text()
          this.$refs.message.error(err || 'network_ne_connect')
        }
      } catch (e) {
        console.error(e)
        this.$refs.message.error('network_ne_connect')
      } finally {
        this.historyModal.loading = false
      }
    },
    formatPhone(phone) {
      if (!phone) return ''
      // Telefon raqamini formatlash: 99 999 99 99
      // Avval barcha raqam bo'lmagan belgilarni olib tashlash
      const cleaned = phone.replace(/\D/g, '')
      
      // Agar +998 yoki 998 bilan boshlansa, uni olib tashlash
      let digits = cleaned
      if (digits.startsWith('998')) {
        digits = digits.substring(3)
      } else if (digits.startsWith('+998')) {
        digits = digits.substring(4)
      }
      
      // 9 raqamli bo'lsa: 99 999 99 99 formatida
      if (digits.length === 9) {
        const match = digits.match(/^(\d{2})(\d{3})(\d{2})(\d{2})$/)
        if (match) {
          return `${match[1]} ${match[2]} ${match[3]} ${match[4]}`
        }
      }
      
      // Agar formatlash mumkin bo'lmasa, asl raqamni qaytarish
      return phone
    },
    async cancelSelectedAddresses() {
      if (this.selectedAddresses.length === 0) {
        this.$refs.message.warning('Iltimos, kamida bitta manzilni tanlang')
        return
      }

      if (!this.selectedDate) {
        this.$refs.message.warning('Iltimos, sanani tanlang')
        return
      }

      if (!confirm(`${this.selectedAddresses.length} ta manzilni otmen qilmoqchimisiz?`)) {
        return
      }

      try {
        this.cancelling = true
        const requestOptions = {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({
            AddressIds: this.selectedAddresses,
            OrderDate: this.selectedDate,
            Note: 'Toplu otmen'
          })
        }

        const response = await fetch(
          this.$store.state.hostname + '/WaterOrders/cancelMultipleAddresses',
          requestOptions
        )

        if (response.status === 200 || response.status === 201) {
          const data = await response.json()
          const successCount = data.results.filter(r => r.success).length
          const failCount = data.results.filter(r => !r.success).length

          if (failCount === 0) {
            this.$refs.message.success(
              `${successCount} ta manzil muvaffaqiyatli otmen qilindi`
            )
          } else {
            this.$refs.message.warning(
              `${successCount} ta muvaffaqiyatli, ${failCount} ta xatolik yuz berdi`
            )
          }

          // Ro'yxatni yangilash
          await this.fetchAddresses()
        } else {
          const errorText = await response.text()
          this.$refs.message.error(errorText || 'network_ne_connect')
        }
      } catch (error) {
        console.error(error)
        this.$refs.message.error('network_ne_connect')
      } finally {
        this.cancelling = false
      }
    }
  }
}
</script>

<style lang="scss" scoped>
// Modern, clean, minimal light theme with soft green accents
.cancel-report-app {
  min-height: 100vh;
  background: #f8fafb;
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen', 'Ubuntu', 'Cantarell', sans-serif;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.report-header {
  background: linear-gradient(135deg, #ffffff 0%, #f0fdf4 50%, #ecfdf5 100%);
  border-bottom: 1px solid #d1fae5;
  box-shadow: 0 1px 8px rgba(16, 185, 129, 0.08);
  padding: 12px 20px;
  flex-shrink: 0;
  
  .report-title {
    margin: 0;
    font-size: 18px;
    font-weight: 600;
    font-style: italic;
    color: #111827;
    letter-spacing: -0.02em;
  }
}

.filter-section {
  padding: 0px 20px;
  background: #fafbfc;
  border-bottom: 1px solid #f3f4f6;
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 12px;
  flex-shrink: 0;
  
  .date-filter {
    display: flex;
    align-items: center;
    gap: 12px;
    
    .filter-label {
      font-size: 12px;
      font-weight: 600;
      color: #374151;
      white-space: nowrap;
      letter-spacing: -0.01em;
    }
    
    .date-input {
      border-radius: 8px;
      font-size: 11px;
      min-width: 200px;
    }
  }
  
  .action-buttons-top {
    display: flex;
    gap: 6px;
    flex-wrap: wrap;
  }
  
  .filter-btn {
    font-size: 10px !important;
    padding: 3px 12px !important;
    border-radius: 8px;
    font-weight: 500;
    letter-spacing: -0.01em;
    height: 28px !important;
    
    i {
      font-size: 10px !important;
      margin-right: 4px !important;
    }
  }
}

.clients-table-container {
  flex: 1;
  overflow: auto;
  padding: 16px 20px;
  background: #f8fafb;
}

.clients-table-wrapper {
  background: #ffffff;
  border-radius: 12px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
  border: 1px solid #f0f0f0;
  overflow: hidden;
}

.clients-table {
  width: 100%;
  border-collapse: separate;
  border-spacing: 0;
  
  thead {
    background: #10b981;
    color: white;
    
    th {
      padding: 7px 12px;
      font-size: 11px;
      font-weight: 600;
      text-align: left;
      white-space: nowrap;
      letter-spacing: -0.01em;
      position: sticky;
      top: 0;
      z-index: 10;
      
      .select-all-checkbox {
        width: 18px;
        height: 18px;
        cursor: pointer;
        accent-color: white;
      }
    }
  }
  
  tbody {
    tr {
      border-bottom: 1px solid #f3f4f6;
      transition: all 0.15s ease;
      cursor: pointer;
      
      &:nth-child(even) {
        background-color: #fafbfc;
      }
      
      &:hover {
        background: #f0fdf4 !important;
        transform: translateX(2px);
      }
      
      &.selected {
        background: #ecfdf5 !important;
        border-left: 3px solid #10b981;
      }
      
      td {
        padding: 7px 12px;
        font-size: 11px;
        color: #374151;
        letter-spacing: -0.01em;
        
        .client-checkbox {
          width: 18px;
          height: 18px;
          cursor: pointer;
          accent-color: #10b981;
        }
        
        &.font-weight-bold {
          font-weight: 600;
          color: #111827;
        }
        
        .phone-count {
          font-size: 10px;
          color: #9ca3af;
          margin-left: 4px;
        }
        
        .address-count-badge {
          display: inline-block;
          background: #ecfdf5;
          color: #10b981;
          padding: 2px 8px;
          border-radius: 12px;
          font-weight: 600;
          font-size: 10px;
        }
        
        .address-item {
          font-size: 11px;
          display: flex;
          align-items: center;
          
          i {
            color: #10b981;
            font-size: 10px;
          }
        }
        
        .bottle-badge {
          display: inline-block;
          background: #fef3c7;
          color: #d97706;
          padding: 2px 8px;
          border-radius: 8px;
          font-weight: 600;
          font-size: 10px;
        }
        
        .last-order-date {
          font-size: 11px;
          color: #475569;
          font-weight: 500;
        }
        
        .last-order-note {
          font-size: 10px;
          color: #64748b;
          font-style: italic;
          max-width: 200px;
          overflow: hidden;
          text-overflow: ellipsis;
          white-space: nowrap;
          display: inline-block;
        }
        
        .text-muted {
          color: #9ca3af;
          font-style: italic;
        }
      }
    }
    
    .empty-state {
      text-align: center;
      padding: 60px 20px;
      color: #9ca3af;
      
      i {
        font-size: 48px;
        margin-bottom: 16px;
        opacity: 0.5;
      }
      
      p {
        font-size: 14px;
        margin: 0;
      }
    }
  }
}

.info-btn {
  background: none;
  border: none;
  color: #0ea5e9;
  cursor: pointer;
  font-size: 16px;
  transition: transform 0.2s ease, color 0.2s ease;
  padding: 4px;

  &:hover {
    transform: scale(1.1);
    color: #0284c7;
  }
}



.cancel-actions {
  padding: 12px 20px;
  background: linear-gradient(135deg, #fafbfc 0%, #f0fdf4 100%);
  border-top: 1px solid #d1fae5;
  display: flex;
  justify-content: space-between;
  align-items: center;
  position: sticky;
  bottom: 0;
  z-index: 10;
  box-shadow: 0 -2px 8px rgba(0, 0, 0, 0.05);
  flex-shrink: 0;
  
  .selected-count {
    font-size: 13px;
    font-weight: 600;
    color: #374151;
    display: flex;
    align-items: center;
    letter-spacing: -0.01em;
    
    i {
      color: #10b981;
      margin-right: 6px;
    }
    
    strong {
      color: #10b981;
      margin: 0 4px;
    }
  }
  
  .cancel-btn {
    font-size: 11px !important;
    padding: 8px 18px !important;
    border-radius: 8px;
    font-weight: 600;
    letter-spacing: -0.01em;
    transition: all 0.2s ease;
    
    i {
      font-size: 11px !important;
      margin-right: 6px !important;
    }
    
    &:hover:not(:disabled) {
      transform: translateY(-1px);
      box-shadow: 0 4px 12px rgba(239, 68, 68, 0.3);
    }
    
    &:disabled {
      opacity: 0.6;
      cursor: not-allowed;
    }
  }
}

@media (max-width: 768px) {
  .cancel-report-app {
    padding: 12px;
  }
  
  .filter-section {
    flex-direction: column;
    align-items: stretch;
    
    .date-filter {
      width: 100%;
    }
    
    .action-buttons-top {
      width: 100%;
      
      .filter-btn {
        flex: 1;
      }
    }
  }
  
  .cancel-actions {
    flex-direction: column;
    gap: 12px;
    
    .cancel-btn {
      width: 100%;
    }
  }
}
</style>

