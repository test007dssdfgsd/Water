<template>
  <div class="statistics-app">
    <backRouter />
    <div class="statistics-main">
      <div class="statistics-header">
        <div class="header-content">
          <div class="user-select-section">
            <erpSelect
              size="sm"
              :options="allUser.rows" 
              @select="selectOption"
              :selected="user_name"
              :label="$t('province')"
            />
            <small v-if="$v.user_name.$dirty && auth_id == null" class="invalid-text">
              {{$t('select_item')}}
            </small>
          </div>
          
          <div class="date-section">
            <mdb-input 
              class="date-input m-0" 
              size="sm" 
              v-model="b_date" 
              type="date"
            ></mdb-input>
            <mdb-input 
              class="date-input m-0" 
              size="sm" 
              v-model="e_date" 
              type="date"
            ></mdb-input>
          </div>
          
          <div class="payment-info-section" v-if="daily_stats.length > 0">
            <div class="payment-item">
              <span class="payment-label">Naqd:</span>
              <span class="payment-value cash">{{formatMoney(total_stats.jami_naqd)}}</span>
            </div>
            <div class="payment-item">
              <span class="payment-label">Plastik:</span>
              <span class="payment-value card">{{formatMoney(total_stats.jami_plastik)}}</span>
            </div>
            <div class="payment-item total">
              <span class="payment-label">Jami:</span>
              <span class="payment-value total-amount">{{formatMoney(total_stats.jami_summa)}}</span>
            </div>
          </div>
          
          <div class="action-buttons">
            <mdb-btn 
              class="action-btn" 
              color="info"  
              @click="fetchStatistics()" 
              size="sm"
            >
              {{$t('apply')}}
            </mdb-btn>
          </div>
        </div>
      </div>
      
      <div class="statistics-table-container">
        <loader-table v-if="loading" />
        <div v-else class="statistics-table-wrapper">
          <table class="statistics-table">
            <thead class="header_table">
              <tr class="stiky_position">
                <th>№</th>
                <th>Sana</th>
                <th class="text-center">Tarqatilgan suv</th>
                <th class="text-center">Olingan idish</th>
              </tr>
            </thead>
            <tbody class="body_table">
              <tr 
                v-for="(item, index) in daily_stats" 
                :key="index" 
                class="hoverTr"
              >
                <td>{{index+1}}</td>
                <td class="font-weight-bold" style="font-size: 12px;">{{item.date}}</td>
                <td class="text-center text-primary font-weight-bold" style="font-size: 12px;">{{item.tarqatilgan_suv}}</td>
                <td class="text-center text-success font-weight-bold" style="font-size: 12px;">{{item.olingan_idish}}</td>
              </tr>
            </tbody>
            <tbody class="body_table all_qty_border" v-if="daily_stats.length > 0">
              <tr>
                <td></td>
                <td class="font-weight-bold">Jami</td>
                <td class="text-center font-weight-bold">{{total_stats.jami_tarqatilgan_suv}}</td>
                <td class="text-center font-weight-bold">{{total_stats.jami_olingan_idish}}</td>
              </tr>
              <tr>
                <td colspan="2" class="font-weight-bold">Ishlagan kunlar:</td>
                <td colspan="2" class="text-center font-weight-bold">{{total_stats.ishlagan_kunlar}} kun</td>
              </tr>
            </tbody>
            <tbody v-if="daily_stats.length === 0">
              <tr>
                <td colspan="4" class="empty-state">
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
import {mdbBtn, mdbInput, mdbIcon} from 'mdbvue'
import loaderTable from '../../components/loaderTable.vue';
import erpSelect from "../../components/erpSelectFio.vue";
import { required } from 'vuelidate/lib/validators'
import { mapActions, mapGetters } from 'vuex';

export default {
  components: { loaderTable, mdbBtn, mdbIcon, mdbInput, erpSelect },
  data() {
    return {
      loading: false,
      b_date: '',
      e_date: '',
      today_date: '',
      
      user_name: '',
      auth_id: null,
      
      daily_stats: [],
      total_stats: {
        jami_tarqatilgan_suv: 0,
        jami_olingan_idish: 0,
        ishlagan_kunlar: 0,
        jami_naqd: 0,
        jami_plastik: 0,
        jami_summa: 0
      }
    }
  },
  validations: {
    user_name: {required},
  },
  computed: {
    ...mapGetters([ 'allUser']),
  },
  async mounted() {
    let date = new Date();
    this.today_date = date.toISOString().slice(0,10);
    this.b_date = this.today_date;
    this.e_date = this.today_date;
    
    await this.fetchUser();
  },
  methods: {
    ...mapActions(['fetchUser']),
    async selectOption(option){
      this.user_name = option.fio;
      this.auth_id = option.auth.id;
    },
    async fetchStatistics(){
      if(this.$v.$invalid)
      {
        this.$v.$touch();
        this.$refs.message.warning('please_fill')
        return false;
      }
      
      if (!this.b_date || !this.e_date) {
        this.$refs.message.warning('Iltimos, sanalarni tanlang')
        return false;
      }
      
      try {
        this.loading = true;
        const beginDate = this.b_date + 'T00:00:00.000Z';
        const endDate = this.e_date + 'T23:59:59.999Z';
        
        const response = await fetch(
          this.$store.state.hostname + 
          `/WaterOrders/getPostavchikDailyStatistics?begin_date=${beginDate}&end_date=${endDate}&auth_id=${this.auth_id}`
        );
        
        const data = await response.json();
        this.loading = false;
        
        if (response.status == 200 || response.status == 201) {
          this.daily_stats = data.daily_stats || [];
          this.total_stats = data.total_stats || {
            jami_tarqatilgan_suv: 0,
            jami_olingan_idish: 0,
            ishlagan_kunlar: 0,
            jami_naqd: 0,
            jami_plastik: 0,
            jami_summa: 0
          };
        } else {
          this.$refs.message.error('network_ne_connect');
          this.daily_stats = [];
          this.total_stats = {
            jami_tarqatilgan_suv: 0,
            jami_olingan_idish: 0,
            ishlagan_kunlar: 0,
            jami_naqd: 0,
            jami_plastik: 0,
            jami_summa: 0
          };
        }
      } catch (error) {
        console.error(error);
        this.loading = false;
        this.$refs.message.error('network_ne_connect');
        this.daily_stats = [];
        this.total_stats = {
          jami_tarqatilgan_suv: 0,
          jami_olingan_idish: 0,
          ishlagan_kunlar: 0,
          jami_naqd: 0,
          jami_plastik: 0,
          jami_summa: 0
        };
      }
    },
    formatMoney(value) {
      if (!value) return '0';
      return new Intl.NumberFormat('ru-RU', {
        minimumFractionDigits: 0,
        maximumFractionDigits: 0
      }).format(value);
    }
  },
}
</script>

<style lang="scss" scoped>
// Modern, clean, minimal light theme with soft green accents
.statistics-app {
  min-height: 100vh;
  background: #f8fafb;
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen', 'Ubuntu', 'Cantarell', sans-serif;
  display: flex;
  overflow: hidden;
}

.statistics-main {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.statistics-header {
  background: linear-gradient(135deg, #ffffff 0%, #f0fdf4 50%, #ecfdf5 100%);
  border-bottom: 1px solid #d1fae5;
  box-shadow: 0 1px 8px rgba(16, 185, 129, 0.08);
  padding: 12px 16px;
  
  @media (max-width: 991px) {
    padding: 8px 12px;
  }
  
  .header-content {
    display: flex;
    align-items: center;
    gap: 12px;
    flex-wrap: wrap;
  }
  
  .user-select-section {
    flex: 0 0 auto;
    min-width: 250px;
    display: flex;
    flex-direction: column;
    gap: 4px;
    
    .invalid-text {
      color: #ef4444;
      font-size: 11px;
      margin-top: 2px;
    }
  }
  
  .date-section {
    display: flex;
    gap: 8px;
    align-items: center;
    
    .date-input {
      border-radius: 8px;
      font-size: 11px;
      min-width: 200px;
    }
  }
  
  .payment-info-section {
    display: flex;
    gap: 16px;
    align-items: center;
    padding: 6px 12px;
    background: rgba(255, 255, 255, 0.7);
    border-radius: 8px;
    border: 1px solid rgba(16, 185, 129, 0.2);
    
    .payment-item {
      display: flex;
      align-items: center;
      gap: 6px;
      
      .payment-label {
        font-size: 10px;
        color: #6b7280;
        font-weight: 500;
      }
      
      .payment-value {
        font-size: 11px;
        font-weight: 600;
        padding: 2px 8px;
        border-radius: 4px;
        background: rgba(255, 255, 255, 0.9);
        
        &.cash {
          color: #059669;
        }
        
        &.card {
          color: #0284c7;
        }
        
        &.total-amount {
          color: #dc2626;
          background: rgba(220, 38, 38, 0.1);
        }
      }
      
      &.total {
        padding-left: 12px;
        border-left: 1px solid rgba(16, 185, 129, 0.3);
      }
    }
    
    @media (max-width: 991px) {
      flex-wrap: wrap;
      gap: 8px;
      
      .payment-item {
        &.total {
          padding-left: 0;
          border-left: none;
          border-top: 1px solid rgba(16, 185, 129, 0.3);
          padding-top: 8px;
          width: 100%;
        }
      }
    }
  }
  
  .action-buttons {
    display: flex;
    gap: 6px;
    align-items: center;
    flex-wrap: wrap;
    margin-left: auto;
    
    .action-btn {
      font-size: 10px !important;
      padding: 3px 18px !important;
      border-radius: 8px;
      font-weight: 500;
      letter-spacing: -0.01em;
      height: 28px !important;
      white-space: nowrap;
      transition: all 0.2s ease;
      
      &:hover:not(:disabled) {
        transform: translateY(-1px);
        box-shadow: 0 2px 8px rgba(16, 185, 129, 0.15);
      }
      
      &:disabled {
        opacity: 0.5;
        cursor: not-allowed;
      }
    }
  }
  
  @media (max-width: 768px) {
    .header-content {
      flex-direction: column;
      align-items: stretch;
    }
    
    .user-select-section,
    .date-section,
    .payment-info-section,
    .action-buttons {
      width: 100%;
    }
    
    .date-section {
      flex-direction: column;
      
      .date-input {
        width: 100%;
      }
    }
    
    .payment-info-section {
      flex-direction: column;
      align-items: stretch;
      
      .payment-item {
        justify-content: space-between;
        
        &.total {
          border-top: 1px solid rgba(16, 185, 129, 0.3);
          padding-top: 8px;
          margin-top: 4px;
        }
      }
    }
    
    .action-buttons {
      justify-content: flex-start;
      margin-left: 0;
    }
  }
}

.statistics-table-container {
  flex: 1;
  overflow: auto;
  padding: 16px;
  background: #f8fafb;
  
  @media (max-width: 991px) {
    padding: 8px 12px;
  }
}

.statistics-table-wrapper {
  background: #ffffff;
  border-radius: 12px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
  border: 1px solid #f0f0f0;
  overflow: hidden;
}

.statistics-table {
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
        padding: 7px 12px;
        font-size: 11px;
        color: #374151;
        letter-spacing: -0.01em;
        
        &.font-weight-bold {
          font-weight: 600;
          color: #111827;
        }
        
        &.text-primary {
          color: #10b981 !important;
        }
        
        &.text-success {
          color: #059669 !important;
        }
      }
    }
    
    &.all_qty_border {
      background: linear-gradient(135deg, #ecfdf5 0%, #d1fae5 100%) !important;
      border-top: 2px solid #10b981;
      
      tr {
        &:hover {
          background: linear-gradient(135deg, #ecfdf5 0%, #d1fae5 100%) !important;
        }
        
        td {
          padding: 10px 12px;
          font-weight: 600;
          color: #047857;
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
