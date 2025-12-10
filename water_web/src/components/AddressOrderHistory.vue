<template>
  <div class="address-order-history">
    <div v-if="loading" class="history-loading">
      <loader-table />
    </div>
    <div v-else>
      <div class="history-summary" v-if="data">
        <div class="summary-item summary-item-address">
          <span class="label">Manzil:</span>
          <span class="value">{{data.address.address}}</span>
        </div>
        <div class="summary-item summary-item-small">
          <span class="label">Zakazlar soni:</span>
          <span class="value">{{data.stats.total_orders}}</span>
        </div>
        <div class="summary-item summary-item-small">
          <span class="label">Umumiy suv (water_count):</span>
          <span class="value">{{data.stats.total_water_count}}</span>
        </div>
        <div class="summary-item summary-item-small">
          <span class="label">Olingan baklashka:</span>
          <span class="value">{{data.stats.total_oligan_baklashka}}</span>
        </div>
        <!-- <div class="summary-item">
          <span class="label">Umumiy qty:</span>
          <span class="value">{{data.stats.total_qty}}</span>
        </div> -->
      </div>

      <div class="history-table-wrapper" v-if="data && data.orders && data.orders.length">
        <table class="history-table">
          <thead>
            <tr>
              <th>№</th>
              <th>Sana</th>
              <th>Suv</th>
              <th>Olingan baklashka</th>
              <!-- <th>Qty</th> -->
              <th>Dostavchik</th>
              <th>Izoh</th>
            </tr>
          </thead>
          <tbody>
            <tr v-for="(ord, idx) in data.orders" :key="ord.id">
              <td>{{idx + 1}}</td>
              <td>{{formatDate(ord.order_date)}}</td>
              <td>{{ord.water_count}}</td>
              <td>{{getOlinganBaklashka(ord)}}</td>
              <!-- <td>{{sumQty(ord.items)}}</td> -->
              <td>
                <span>
                  {{ ord.delivery_user ? (ord.delivery_user.fio || ord.delivery_user.username || '—') : '—' }}
                </span>
              </td>
              <td class="note-cell">
                <span v-if="ord.note">{{ord.note}}</span>
                <span v-else class="text-muted">-</span>
              </td>
            </tr>
          </tbody>
        </table>
      </div>

      <div v-else class="empty-state mt-3">
        <i class="fas fa-inbox"></i>
        <p>Zakazlar topilmadi</p>
      </div>
    </div>
  </div>
</template>

<script>
import loaderTable from './loaderTable.vue'

export default {
  name: 'AddressOrderHistory',
  components: {
    loaderTable
  },
  props: {
    loading: {
      type: Boolean,
      default: false
    },
    data: {
      type: Object,
      default: null
    }
  },
  methods: {
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
    getOlinganBaklashka(order) {
      // Olingan baklashka - to'g'ridan-to'g'ri reserverd_number_id_1 dan olinadi
      return order.reserverd_numeric_id_1 || 0
    },
    sumQty(items) {
      if (!items || !Array.isArray(items)) return 0
      return items.reduce((sum, item) => sum + (item.qty || 0), 0)
    }
  }
}
</script>

<style lang="scss" scoped>
.address-order-history {
  width: 100%;
  
  .history-loading {
    padding: 40px;
    display: flex;
    justify-content: center;
    align-items: center;
  }
  
  .history-summary {
    display: grid;
    grid-template-columns: 2fr 1fr 1fr 1fr;
    gap: 12px;
    margin-bottom: 20px;
    padding: 16px;
    background: linear-gradient(135deg, #ffffff 0%, #f0fdf4 100%);
    border: 1.5px solid #10b981;
    border-radius: 12px;
    box-shadow: 0 2px 8px rgba(16, 185, 129, 0.1);
    
    @media (max-width: 768px) {
      grid-template-columns: 1fr;
    }
    
    .summary-item {
      display: flex;
      flex-direction: column;
      gap: 4px;
      
      .label {
        font-size: 11px;
        font-weight: 600;
        color: #64748b;
        letter-spacing: -0.01em;
        text-transform: uppercase;
      }
      
      .value {
        font-size: 14px;
        font-weight: 700;
        color: #10b981;
        letter-spacing: -0.02em;
      }
      
      &.summary-item-address {
        .label {
          font-size: 10px;
        }
        
        .value {
          font-size: 12px;
          word-break: break-word;
        }
      }
      
      &.summary-item-small {
        .label {
          font-size: 10px;
        }
        
        .value {
          font-size: 12px;
        }
      }
    }
  }
  
  .history-table-wrapper {
    max-height: 360px;
    overflow: auto;
    background: #ffffff;
    border-radius: 12px;
    border: 1px solid #e5e7eb;
    box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
    
    .history-table {
      width: 100%;
      border-collapse: separate;
      border-spacing: 0;
      
      thead {
        background: linear-gradient(135deg, #10b981 0%, #059669 100%);
        color: white;
        position: sticky;
        top: 0;
        z-index: 10;
        
        th {
          padding: 7px 12px;
          font-size: 11px;
          font-weight: 600;
          text-align: left;
          white-space: nowrap;
          letter-spacing: -0.01em;
          border-bottom: 2px solid #047857;
          
          &:first-child {
            border-top-left-radius: 12px;
          }
          
          &:last-child {
            border-top-right-radius: 12px;
          }
        }
      }
      
      tbody {
        tr {
          border-bottom: 1px solid #f3f4f6;
          transition: all 0.15s ease;
          
          &:nth-child(even) {
            background-color: #fafbfc;
          }
          
          &:hover {
            background: #f0fdf4 !important;
            transform: translateX(2px);
          }
          
          &:last-child {
            border-bottom: none;
          }
          
          td {
            padding: 7px 12px;
            font-size: 11px;
            color: #374151;
            letter-spacing: -0.01em;
            
            &.note-cell {
              max-width: 200px;
              overflow: hidden;
              text-overflow: ellipsis;
              white-space: nowrap;
            }
          }
        }
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
  
  .text-muted {
    color: #9ca3af;
    font-style: italic;
  }
  
  .mt-3 {
    margin-top: 16px;
  }
}
</style>

