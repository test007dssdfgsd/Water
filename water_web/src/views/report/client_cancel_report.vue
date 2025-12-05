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
          @change="fetchClients"
          type="date"
        ></mdb-input>
      </div>
      <div class="action-buttons-top">
        <mdb-btn 
          color="info" 
          @click="fetchClients"
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
                  :checked="selectedClients.length === clients.length && clients.length > 0"
                  @change="toggleSelectAll"
                  class="select-all-checkbox"
                />
              </th>
              <th>№</th>
              <th>{{$t('fio')}}</th>
              <th>Telefon</th>
              <th>{{$t('address')}}</th>
              <th>Baklashka</th>
              <th>Tuman</th>
              <th>Turi</th>
              <th>Manzillar soni</th>
            </tr>
          </thead>
          <tbody>
            <template v-for="(client, index) in clients">
              <tr 
                :key="client.id"
                class="client-row"
                :class="{ 'selected': selectedClients.includes(client.id) }"
                @click="toggleClient(client.id)"
              >
                <td>
                  <input 
                    type="checkbox" 
                    :checked="selectedClients.includes(client.id)"
                    @change="toggleClient(client.id)"
                    @click.stop
                    class="client-checkbox"
                  />
                </td>
                <td>{{index + 1}}</td>
                <td class="font-weight-bold">{{client.fio}}</td>
                <td>
                  <span v-if="client.phone_numbers_list && client.phone_numbers_list.length > 0">
                    {{formatPhone(client.phone_numbers_list[0].phone_number)}}
                    <span v-if="client.phone_numbers_list.length > 1" class="phone-count">
                      (+{{client.phone_numbers_list.length - 1}})
                    </span>
                  </span>
                  <span v-else class="text-muted">-</span>
                </td>
                <td>
                  <div class="addresses-list">
                    <div 
                      v-for="(addr) in client.addresses" 
                      :key="addr.id"
                      class="address-item"
                    >
                      <i class="fas fa-map-marker-alt mr-1"></i>
                      {{addr.address}}
                      <span class="bottle-info" v-if="addr.bottle_count !== undefined">
                        ({{addr.bottle_count}} ta)
                      </span>
                    </div>
                    <span v-if="!client.addresses || client.addresses.length === 0" class="text-muted">-</span>
                  </div>
                </td>
                <td>
                  <div class="bottles-list">
                    <div 
                      v-for="(addr) in client.addresses" 
                      :key="addr.id"
                      class="bottle-item"
                    >
                      <span v-if="addr.bottle_count !== undefined && addr.bottle_count > 0" class="bottle-badge">
                        {{addr.bottle_count}} ta
                      </span>
                      <span v-else class="text-muted">0</span>
                    </div>
                    <span v-if="!client.addresses || client.addresses.length === 0" class="text-muted">-</span>
                  </div>
                </td>
                <td>
                  <span v-if="client.tuman">
                    {{client.tuman.name}}
                  </span>
                  <span v-else class="text-muted">-</span>
                </td>
                <td>
                  <span class="text-muted">-</span>
                </td>
                <td class="text-center">
                  <span v-if="client.addresses" class="address-count-badge">
                    {{client.addresses.length}}
                  </span>
                  <span v-else>0</span>
                </td>
              </tr>
            </template>
            
            <tr v-if="clients.length === 0">
              <td colspan="9" class="empty-state">
                <i class="fas fa-inbox"></i>
                <p>Klientlar topilmadi</p>
              </td>
            </tr>
          </tbody>
        </table>
      </div>
    </div>

    <div class="cancel-actions" v-if="selectedClients.length > 0">
      <div class="selected-count">
        <i class="fas fa-check-circle mr-2"></i>
        Tanlangan: <strong>{{selectedClients.length}}</strong> ta klient
      </div>
      <mdb-btn 
        color="danger" 
        @click="cancelSelectedClients"
        :disabled="cancelling"
        class="cancel-btn"
      >
        <i class="fas fa-times-circle mr-2"></i>
        <span v-if="!cancelling">Tanlangan klientlarni otmen qilish</span>
        <span v-else>Jarayonda...</span>
      </mdb-btn>
    </div>

    <Toast ref="message"></Toast>
  </div>
</template>

<script>
import { mdbBtn, mdbInput } from 'mdbvue'
import loaderTable from '../../components/loaderTable.vue'
import { mapActions } from 'vuex'

export default {
  components: {
    mdbBtn,
    mdbInput,
    loaderTable
  },
  data() {
    return {
      loading: false,
      cancelling: false,
      selectedDate: '',
      clients: [],
      selectedClients: []
    }
  },
  async mounted() {
    // Bugungi sanani default qilib qo'yish
    const today = new Date()
    this.selectedDate = today.toISOString().slice(0, 10)
    await this.fetchClients()
  },
  methods: {
    async fetchClients() {
      if (!this.selectedDate) {
        this.$refs.message.warning('Iltimos, sanani tanlang')
        return
      }

      try {
        this.loading = true
        const response = await fetch(
          this.$store.state.hostname + 
          `/WaterOrders/getClientsWithoutOrdersAfterDate?date=${this.selectedDate}`
        )
        
        if (response.status === 200 || response.status === 201) {
          const data = await response.json()
          this.clients = data
          this.selectedClients = [] // Tanlovni tozalash
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
    toggleClient(clientId) {
      const index = this.selectedClients.indexOf(clientId)
      if (index > -1) {
        this.selectedClients.splice(index, 1)
      } else {
        this.selectedClients.push(clientId)
      }
    },
    selectAll() {
      this.selectedClients = this.clients.map(c => c.id)
    },
    deselectAll() {
      this.selectedClients = []
    },
    toggleSelectAll(event) {
      if (event.target.checked) {
        this.selectAll()
      } else {
        this.deselectAll()
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
    async cancelSelectedClients() {
      if (this.selectedClients.length === 0) {
        this.$refs.message.warning('Iltimos, kamida bitta klientni tanlang')
        return
      }

      if (!this.selectedDate) {
        this.$refs.message.warning('Iltimos, sanani tanlang')
        return
      }

      if (!confirm(`${this.selectedClients.length} ta klientni otmen qilmoqchimisiz?`)) {
        return
      }

      try {
        this.cancelling = true
        const requestOptions = {
          method: 'POST',
          headers: { 'Content-Type': 'application/json' },
          body: JSON.stringify({
            ClientIds: this.selectedClients,
            OrderDate: this.selectedDate,
            Note: 'Toplu otmen'
          })
        }

        const response = await fetch(
          this.$store.state.hostname + '/WaterOrders/cancelMultipleClients',
          requestOptions
        )

        if (response.status === 200 || response.status === 201) {
          const data = await response.json()
          const successCount = data.results.filter(r => r.success).length
          const failCount = data.results.filter(r => !r.success).length
          const totalOrders = data.results
            .filter(r => r.success && r.addressesCount)
            .reduce((sum, r) => sum + r.addressesCount, 0)

          if (failCount === 0) {
            this.$refs.message.success(
              `${successCount} ta klient, ${totalOrders} ta manzil muvaffaqiyatli otmen qilindi`
            )
          } else {
            this.$refs.message.warning(
              `${successCount} ta muvaffaqiyatli, ${failCount} ta xatolik yuz berdi`
            )
          }

          // Ro'yxatni yangilash
          await this.fetchClients()
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
        
        .addresses-list {
          display: flex;
          flex-direction: column;
          gap: 4px;
          
          .address-item {
            font-size: 11px;
            display: flex;
            align-items: center;
            
            i {
              color: #10b981;
              font-size: 10px;
            }
            
            .bottle-info {
              color: #9ca3af;
              font-size: 10px;
              margin-left: 4px;
            }
          }
        }
        
        .bottles-list {
          display: flex;
          flex-direction: column;
          gap: 4px;
          
          .bottle-item {
            .bottle-badge {
              display: inline-block;
              background: #fef3c7;
              color: #d97706;
              padding: 2px 6px;
              border-radius: 8px;
              font-weight: 600;
              font-size: 10px;
            }
          }
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

