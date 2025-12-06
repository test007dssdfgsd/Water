<template>
  <div class="real-time-stats-app">
    <backRouter />
    <div class="stats-main">
      <div class="stats-header">
        <div class="header-content">
          <h5 class="page-title">
            <mdb-icon icon="chart-line" class="mr-2" />
            Real vaqtda dostavchiklar statistikasi
          </h5>
          <div class="header-actions">
            <mdb-btn 
              class="refresh-btn" 
              color="info"  
              @click="fetchStatistics()" 
              size="sm"
            >
              <mdb-icon icon="sync-alt" class="mr-1" />
              Yangilash
            </mdb-btn>
          </div>
        </div>
      </div>
      
      <div class="stats-table-container">
        <loader-table v-if="loading" />
        <div v-else class="stats-table-wrapper">
          <table class="stats-table">
            <thead class="header_table">
              <tr class="stiky_position">
                <th>№</th>
                <th>Dostavchik</th>
                <th class="text-center">Tarqatilgan suv</th>
                <th class="text-center">Tarqatilishi kerak</th>
                <th class="text-center">Olingan baklashka</th>
              </tr>
            </thead>
            <tbody class="body_table">
              <tr 
                v-for="(item, index) in postavchik_stats" 
                :key="index" 
                class="hoverTr"
              >
                <td>{{index+1}}</td>
                <td class="font-weight-bold" style="font-size: 12px;">{{item.user_fio}}</td>
                <td class="text-center text-success font-weight-bold" style="font-size: 12px;">
                  {{item.tarqatilgan_suv}}
                </td>
                <td class="text-center text-warning font-weight-bold" style="font-size: 12px;">
                  {{item.tarqatilishi_kerak}}
                </td>
                <td class="text-center text-primary font-weight-bold" style="font-size: 12px;">
                  {{item.olingan_baklashka}}
                </td>
              </tr>
            </tbody>
            <tbody v-if="postavchik_stats.length === 0">
              <tr>
                <td colspan="5" class="empty-state">
                  <i class="fas fa-inbox"></i>
                  <p>Ma'lumotlar topilmadi</p>
                </td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
    
    <Toast ref="message"></Toast>
  </div>
</template>

<script>
import {mdbBtn, mdbIcon} from 'mdbvue'
import loaderTable from '../../components/loaderTable.vue';

export default {
  components: { loaderTable, mdbBtn, mdbIcon },
  data() {
    return {
      loading: false,
      postavchik_stats: []
    }
  },
  async mounted() {
    await this.fetchStatistics();
    // Auto refresh har 30 soniyada
    this.autoRefreshInterval = setInterval(() => {
      this.fetchStatistics();
    }, 30000);
  },
  beforeDestroy() {
    if (this.autoRefreshInterval) {
      clearInterval(this.autoRefreshInterval);
    }
  },
  methods: {
    async fetchStatistics(){
      try {
        this.loading = true;
        const response = await fetch(
          this.$store.state.hostname + 
          `/WaterOrders/getRealTimePostavchikStatistics`
        );
        
        const data = await response.json();
        this.loading = false;
        
        if (response.status == 200 || response.status == 201) {
          this.postavchik_stats = data || [];
        } else {
          this.$refs.message.error('network_ne_connect');
          this.postavchik_stats = [];
        }
      } catch (error) {
        console.error(error);
        this.loading = false;
        this.$refs.message.error('network_ne_connect');
        this.postavchik_stats = [];
      }
    }
  },
}
</script>

<style lang="scss" scoped>
// Modern, clean, minimal light theme with soft green accents
.real-time-stats-app {
  min-height: 100vh;
  background: #f8fafb;
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen', 'Ubuntu', 'Cantarell', sans-serif;
  display: flex;
  overflow: hidden;
}

.stats-main {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.stats-header {
  background: linear-gradient(135deg, #ffffff 0%, #f0fdf4 50%, #ecfdf5 100%);
  border-bottom: 1px solid #d1fae5;
  box-shadow: 0 1px 8px rgba(16, 185, 129, 0.08);
  padding: 12px 16px;
  
  .header-content {
    display: flex;
    align-items: center;
    justify-content: space-between;
    
    .page-title {
      color: #111827;
      font-weight: 600;
      font-size: 16px;
      margin: 0;
      letter-spacing: -0.02em;
      display: flex;
      align-items: center;
      
      mdb-icon {
        color: #10b981;
        font-size: 18px;
      }
    }
    
    .header-actions {
      display: flex;
      gap: 8px;
      align-items: center;
      
      .refresh-btn {
        font-size: 11px !important;
        padding: 4px 14px !important;
        border-radius: 8px;
        font-weight: 500;
        letter-spacing: -0.01em;
        height: 30px !important;
        white-space: nowrap;
        transition: all 0.2s ease;
        
        &:hover:not(:disabled) {
          transform: translateY(-1px);
          box-shadow: 0 2px 8px rgba(16, 185, 129, 0.15);
        }
        
        mdb-icon {
          font-size: 12px;
        }
      }
    }
  }
}

.stats-table-container {
  flex: 1;
  overflow: auto;
  padding: 16px;
  background: #f8fafb;
}

.stats-table-wrapper {
  background: #ffffff;
  border-radius: 12px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
  border: 1px solid #f0f0f0;
  overflow: hidden;
}

.stats-table {
  width: 100%;
  border-collapse: separate;
  border-spacing: 0;
  
  thead {
    background: #10b981;
    color: white;
    
    th {
      padding: 10px 12px;
      font-size: 12px;
      font-weight: 600;
      text-align: left;
      white-space: nowrap;
      letter-spacing: -0.01em;
      position: sticky;
      top: 0;
      z-index: 10;
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
      
      td {
        padding: 10px 12px;
        font-size: 12px;
        color: #374151;
        letter-spacing: -0.01em;
        
        &.font-weight-bold {
          font-weight: 600;
          color: #111827;
        }
        
        &.text-success {
          color: #059669 !important;
        }
        
        &.text-warning {
          color: #d97706 !important;
        }
        
        &.text-primary {
          color: #0284c7 !important;
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

.stiky_position {
  position: sticky;
  top: 0;
  background: #10b981;
  color: white;
  z-index: 10;
}

.hoverTr:hover {
  background: #f0fdf4 !important;
  transform: translateX(2px);
}
</style>

