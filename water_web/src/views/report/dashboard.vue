<template>
  <div class="dashboard-app">
    <backRouter />
    <div class="dashboard-main">
      <!-- Bugungi kun statistikasi -->
      <div class="today-stats-section">
        <h5 class="section-title">
          <mdb-icon icon="calendar-day" class="mr-2" />
          Bugungi kun statistikasi
        </h5>
        <div class="stats-cards">
          <div class="stat-card">
            <div class="stat-icon" style="background: linear-gradient(135deg, #10b981 0%, #059669 100%);">
              <mdb-icon icon="tint" />
            </div>
            <div class="stat-content">
              <div class="stat-label">Tarqatilgan suv</div>
              <div class="stat-value">{{today_stats.tarqatilgan_suv || 0}}</div>
            </div>
          </div>
          
          <div class="stat-card">
            <div class="stat-icon" style="background: linear-gradient(135deg, #f59e0b 0%, #d97706 100%);">
              <mdb-icon icon="clock" />
            </div>
            <div class="stat-content">
              <div class="stat-label">Tarqatilishi kerak</div>
              <div class="stat-value">{{today_stats.tarqatilishi_kerak || 0}}</div>
            </div>
          </div>
          
          <div class="stat-card">
            <div class="stat-icon" style="background: linear-gradient(135deg, #3b82f6 0%, #2563eb 100%);">
              <mdb-icon icon="box" />
            </div>
            <div class="stat-content">
              <div class="stat-label">Yig'ilgan baklashka</div>
              <div class="stat-value">{{today_stats.yigilgan_baklashka || 0}}</div>
            </div>
          </div>
          
          <div class="stat-card">
            <div class="stat-icon" style="background: linear-gradient(135deg, #8b5cf6 0%, #7c3aed 100%);">
              <mdb-icon icon="money-bill-wave" />
            </div>
            <div class="stat-content">
              <div class="stat-label">Topilgan pul</div>
              <div class="stat-value">{{formatMoney(today_stats.topilgan_pul || 0)}}</div>
            </div>
          </div>
        </div>
      </div>

      <!-- Sana oralig'i statistikasi -->
      <div class="period-stats-section">
        <h5 class="section-title">
          <mdb-icon icon="chart-bar" class="mr-2" />
          Sana oralig'i statistikasi
        </h5>
        <div class="date-inputs">
          <div class="date-group">
            <label>Boshlanish sana</label>
            <mdb-input 
              class="date-input m-0" 
              size="sm" 
              v-model="begin_date" 
              type="date"
            ></mdb-input>
          </div>
          <div class="date-group">
            <label>Oxirgi sana</label>
            <mdb-input 
              class="date-input m-0" 
              size="sm" 
              v-model="end_date" 
              type="date"
            ></mdb-input>
          </div>
          <div class="date-group-button">
            <mdb-btn 
              class="apply-btn" 
              color="success"  
              @click="fetchStatistics()" 
              size="sm"
            >
              <mdb-icon icon="check" class="mr-1" />
              OK
            </mdb-btn>
          </div>
        </div>
        
        <div class="stats-grid">
          <div class="stat-box">
            <div class="stat-box-label">Tarqatilgan suv soni</div>
            <div class="stat-box-value">{{period_stats.tarqatilgan_suv_soni || 0}}</div>
          </div>
          
          <div class="stat-box">
            <div class="stat-box-label">Tarqatilgan suv summasi</div>
            <div class="stat-box-value">{{formatMoney(period_stats.tarqatilgan_suv_summasi || 0)}}</div>
          </div>
          
          <div class="stat-box">
            <div class="stat-box-label">Yangi qo'shilgan klientlar</div>
            <div class="stat-box-value">{{period_stats.yangi_klientlar_soni || 0}}</div>
          </div>
          
          <div class="stat-box">
            <div class="stat-box-label">Otmen bo'lgan klientlar</div>
            <div class="stat-box-value">{{period_stats.otmen_klientlar_soni || 0}}</div>
          </div>
        </div>
      </div>

      <!-- Kunlik to'lov o'sishi chart -->
      <div class="chart-section" v-if="daily_payments && daily_payments.length > 0">
        <h5 class="section-title">
          <mdb-icon icon="chart-line" class="mr-2" />
          Kunlik to'lov o'sishi
        </h5>
        <div class="chart-container">
          <apexchart
            type="line"
            height="350"
            :options="chartOptions"
            :series="chartSeries"
          ></apexchart>
        </div>
      </div>
      <div class="chart-section" v-else-if="begin_date && end_date">
        <h5 class="section-title">
          <mdb-icon icon="chart-line" class="mr-2" />
          Kunlik to'lov o'sishi
        </h5>
        <div class="empty-chart">
          <i class="fas fa-chart-line"></i>
          <p>Ma'lumotlar topilmadi</p>
        </div>
      </div>

      <!-- O'tgan yil bilan solishtirish -->
      <div class="compare-stats-section">
        <h5 class="section-title">
          <mdb-icon icon="chart-line" class="mr-2" />
          O'tgan yil bilan solishtirish
        </h5>
        <div class="date-inputs">
          <div class="date-group">
            <label>Boshlanish sana (o'tgan yil)</label>
            <mdb-input 
              class="date-input m-0" 
              size="sm" 
              v-model="compare_begin_date" 
              type="date"
            ></mdb-input>
          </div>
          <div class="date-group">
            <label>Tugash sana (o'tgan yil)</label>
            <mdb-input 
              class="date-input m-0" 
              size="sm" 
              v-model="compare_end_date" 
              type="date"
            ></mdb-input>
          </div>
          <div class="date-group-button">
            <mdb-btn 
              class="apply-btn" 
              color="success"  
              @click="fetchStatistics()" 
              size="sm"
            >
              <mdb-icon icon="check" class="mr-1" />
              OK
            </mdb-btn>
          </div>
        </div>
        
        <div class="compare-grid">
          <div class="compare-item">
            <div class="compare-label">Tarqatilgan suv</div>
            <div class="compare-values">
              <span class="current-value">{{period_stats.tarqatilgan_suv_soni || 0}}</span>
              <span class="vs">vs</span>
              <span class="compare-value">{{compare_stats.tarqatilgan_suv_soni || 0}}</span>
            </div>
            <div class="compare-diff" :class="getDiffClass(differences.tarqatilgan_suv_foiz)">
              <span>{{formatPercent(differences.tarqatilgan_suv_foiz)}}%</span>
              <span class="diff-oq">({{formatNumber(differences.tarqatilgan_suv_oq)}})</span>
            </div>
          </div>
          
          <div class="compare-item">
            <div class="compare-label">Yangi klientlar</div>
            <div class="compare-values">
              <span class="current-value">{{period_stats.yangi_klientlar_soni || 0}}</span>
              <span class="vs">vs</span>
              <span class="compare-value">{{compare_stats.yangi_klientlar_soni || 0}}</span>
            </div>
            <div class="compare-diff" :class="getDiffClass(differences.yangi_klientlar_foiz)">
              <span>{{formatPercent(differences.yangi_klientlar_foiz)}}%</span>
              <span class="diff-oq">({{differences.yangi_klientlar_oq || 0}})</span>
            </div>
          </div>
          
          <div class="compare-item">
            <div class="compare-label">Otmen klientlar</div>
            <div class="compare-values">
              <span class="current-value">{{period_stats.otmen_klientlar_soni || 0}}</span>
              <span class="vs">vs</span>
              <span class="compare-value">{{compare_stats.otmen_klientlar_soni || 0}}</span>
            </div>
            <div class="compare-diff" :class="getDiffClass(differences.otmen_klientlar_foiz)">
              <span>{{formatPercent(differences.otmen_klientlar_foiz)}}%</span>
              <span class="diff-oq">({{differences.otmen_klientlar_oq || 0}})</span>
            </div>
          </div>
        </div>
      </div>
    </div>
    
    <Toast ref="message"></Toast>
  </div>
</template>

<script>
import {mdbBtn, mdbInput, mdbIcon} from 'mdbvue'
import loaderTable from '../../components/loaderTable.vue';

export default {
  components: { loaderTable, mdbBtn, mdbIcon, mdbInput },
  data() {
    return {
      loading: false,
      today_stats: {
        tarqatilgan_suv: 0,
        tarqatilishi_kerak: 0,
        yigilgan_baklashka: 0,
        topilgan_pul: 0
      },
      begin_date: '',
      end_date: '',
      compare_begin_date: '',
      compare_end_date: '',
      period_stats: {
        tarqatilgan_suv_soni: 0,
        tarqatilgan_suv_summasi: 0,
        yangi_klientlar_soni: 0,
        otmen_klientlar_soni: 0,
        daily_payments: []
      },
      daily_payments: [],
      compare_stats: {
        tarqatilgan_suv_soni: 0,
        tarqatilgan_suv_summasi: 0,
        yangi_klientlar_soni: 0,
        otmen_klientlar_soni: 0
      },
      differences: {
        tarqatilgan_suv_foiz: 0,
        tarqatilgan_suv_oq: 0,
        yangi_klientlar_foiz: 0,
        yangi_klientlar_oq: 0,
        otmen_klientlar_foiz: 0,
        otmen_klientlar_oq: 0
      }
    }
  },
  async mounted() {
    // Birinchi sana oralig'i - oxirgi bir oy
    const today = new Date();
    const lastMonth = new Date(today);
    lastMonth.setMonth(lastMonth.getMonth() - 1);
    
    this.end_date = today.toISOString().slice(0, 10);
    this.begin_date = lastMonth.toISOString().slice(0, 10);
    
    // Ikkinchi sana oralig'i - birinchi sanalarning 1 yil oldingi versiyasi
    const compareEnd = new Date(lastMonth);
    compareEnd.setFullYear(compareEnd.getFullYear() - 1);
    const compareBegin = new Date(today);
    compareBegin.setFullYear(compareBegin.getFullYear() - 1);
    compareBegin.setMonth(compareBegin.getMonth() - 1);
    
    this.compare_end_date = compareEnd.toISOString().slice(0, 10);
    this.compare_begin_date = compareBegin.toISOString().slice(0, 10);
    
    await this.fetchStatistics();
  },
  methods: {
    async fetchStatistics(){
      try {
        this.loading = true;
        const params = new URLSearchParams();
        if (this.begin_date) params.append('begin_date', this.begin_date + 'T00:00:00.000Z');
        if (this.end_date) params.append('end_date', this.end_date + 'T23:59:59.999Z');
        if (this.compare_begin_date) params.append('compare_begin_date', this.compare_begin_date + 'T00:00:00.000Z');
        if (this.compare_end_date) params.append('compare_end_date', this.compare_end_date + 'T23:59:59.999Z');
        
        const response = await fetch(
          this.$store.state.hostname + 
          `/WaterOrders/getDashboardStatistics?${params.toString()}`
        );
        
        const data = await response.json();
        this.loading = false;
        
        if (response.status == 200 || response.status == 201) {
          this.today_stats = data.today_stats || this.today_stats;
          this.period_stats = data.period_stats || this.period_stats;
          this.compare_stats = data.compare_stats || this.compare_stats;
          this.differences = data.differences || this.differences;
          this.daily_payments = (data.period_stats && data.period_stats.daily_payments) ? data.period_stats.daily_payments : [];
        } else {
          this.$refs.message.error('network_ne_connect');
        }
      } catch (error) {
        console.error(error);
        this.loading = false;
        this.$refs.message.error('network_ne_connect');
      }
    },
    formatMoney(value) {
      if (!value) return '0';
      return new Intl.NumberFormat('ru-RU', {
        minimumFractionDigits: 0,
        maximumFractionDigits: 0
      }).format(value);
    },
    formatPercent(value) {
      if (!value) return '0';
      return value.toFixed(1);
    },
    formatNumber(value) {
      if (!value) return '0';
      return new Intl.NumberFormat('ru-RU').format(value);
    },
    getDiffClass(value) {
      if (value > 0) return 'positive';
      if (value < 0) return 'negative';
      return 'neutral';
    }
  },
  computed: {
    chartOptions() {
      return {
        chart: {
          type: 'line',
          height: 350,
          toolbar: {
            show: true
          },
          zoom: {
            enabled: true
          }
        },
        dataLabels: {
          enabled: false
        },
        stroke: {
          curve: 'smooth',
          width: 3
        },
        xaxis: {
          categories: this.daily_payments.map(d => {
            const date = new Date(d.date);
            return date.toLocaleDateString('ru-RU', { day: '2-digit', month: '2-digit' });
          }),
          labels: {
            style: {
              fontSize: '11px',
              colors: '#6b7280'
            }
          }
        },
        yaxis: {
          labels: {
            formatter: (value) => {
              return this.formatMoney(value);
            },
            style: {
              fontSize: '11px',
              colors: '#6b7280'
            }
          }
        },
        colors: ['#10b981'],
        grid: {
          borderColor: '#f3f4f6',
          strokeDashArray: 4
        },
        tooltip: {
          y: {
            formatter: (value) => {
              return this.formatMoney(value) + ' сум';
            }
          }
        },
        title: {
          text: 'Kunlik to\'lov o\'sishi',
          align: 'left',
          style: {
            fontSize: '14px',
            fontWeight: 600,
            color: '#111827'
          }
        }
      };
    },
    chartSeries() {
      return [{
        name: 'To\'lov summasi',
        data: this.daily_payments.map(d => d.daily_sum || 0)
      }];
    }
  }
}
</script>

<style lang="scss" scoped>
.dashboard-app {
  min-height: 100vh;
  background: #f8fafb;
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen', 'Ubuntu', 'Cantarell', sans-serif;
  padding: 16px;
}

.dashboard-main {
  max-width: 1400px;
  margin: 0 auto;
}

.section-title {
  color: #111827;
  font-weight: 600;
  font-size: 18px;
  margin-bottom: 16px;
  display: flex;
  align-items: center;
  
  mdb-icon {
    color: #10b981;
    font-size: 20px;
  }
}

// Bugungi kun statistikasi
.today-stats-section {
  background: #ffffff;
  border-radius: 12px;
  padding: 20px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
  margin-bottom: 24px;
  border: 1px solid #f0f0f0;
}

.stats-cards {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(250px, 1fr));
  gap: 16px;
}

.stat-card {
  display: flex;
  align-items: center;
  padding: 16px;
  background: #fafbfc;
  border-radius: 10px;
  border: 1px solid #f3f4f6;
  transition: all 0.2s ease;
  
  &:hover {
    transform: translateY(-2px);
    box-shadow: 0 4px 12px rgba(0, 0, 0, 0.1);
  }
  
  .stat-icon {
    width: 50px;
    height: 50px;
    border-radius: 10px;
    display: flex;
    align-items: center;
    justify-content: center;
    margin-right: 16px;
    color: white;
    font-size: 24px;
  }
  
  .stat-content {
    flex: 1;
    
    .stat-label {
      font-size: 12px;
      color: #6b7280;
      margin-bottom: 4px;
      font-weight: 500;
    }
    
    .stat-value {
      font-size: 20px;
      font-weight: 600;
      color: #111827;
    }
  }
}

// Sana oralig'i statistikasi
.period-stats-section {
  background: #ffffff;
  border-radius: 12px;
  padding: 20px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
  margin-bottom: 24px;
  border: 1px solid #f0f0f0;
}

.date-inputs {
  display: flex;
  gap: 16px;
  margin-bottom: 20px;
  flex-wrap: wrap;
  align-items: flex-end;
  
  .date-group {
    flex: 1;
    min-width: 200px;
    
    label {
      display: block;
      font-size: 12px;
      color: #6b7280;
      margin-bottom: 6px;
      font-weight: 500;
    }
    
    .date-input {
      border-radius: 8px;
      font-size: 12px;
    }
  }
  
  .date-group-button {
    display: flex;
    align-items: center;
    min-width: 80px;
    
    .apply-btn {
      font-size: 12px !important;
      padding: 6px 16px !important;
      border-radius: 8px;
      font-weight: 500;
      letter-spacing: -0.01em;
      height: 32px !important;
      white-space: nowrap;
      transition: all 0.2s ease;
      box-shadow: 0 2px 8px rgba(16, 185, 129, 0.2);
      
      &:hover:not(:disabled) {
        transform: translateY(-1px);
        box-shadow: 0 4px 12px rgba(16, 185, 129, 0.3);
      }
      
      mdb-icon {
        font-size: 13px;
      }
    }
  }
}

.stats-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
  gap: 16px;
}

.stat-box {
  padding: 16px;
  background: linear-gradient(135deg, #f0fdf4 0%, #ecfdf5 100%);
  border-radius: 10px;
  border: 1px solid #d1fae5;
  
  .stat-box-label {
    font-size: 11px;
    color: #6b7280;
    margin-bottom: 8px;
    font-weight: 500;
  }
  
  .stat-box-value {
    font-size: 24px;
    font-weight: 600;
    color: #059669;
  }
}

// Kunlik to'lov o'sishi chart
.chart-section {
  background: #ffffff;
  border-radius: 12px;
  padding: 20px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
  margin-bottom: 24px;
  border: 1px solid #f0f0f0;
}

.chart-container {
  margin-top: 16px;
  background: #fafbfc;
  border-radius: 10px;
  padding: 16px;
}

.empty-chart {
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

// O'tgan yil bilan solishtirish
.compare-stats-section {
  background: #ffffff;
  border-radius: 12px;
  padding: 20px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
  border: 1px solid #f0f0f0;
}

.compare-grid {
  display: grid;
  grid-template-columns: repeat(auto-fit, minmax(300px, 1fr));
  gap: 20px;
}

.compare-item {
  padding: 20px;
  background: #fafbfc;
  border-radius: 10px;
  border: 1px solid #f3f4f6;
  
  .compare-label {
    font-size: 13px;
    color: #6b7280;
    margin-bottom: 12px;
    font-weight: 500;
  }
  
  .compare-values {
    display: flex;
    align-items: center;
    justify-content: space-between;
    margin-bottom: 12px;
    
    .current-value {
      font-size: 20px;
      font-weight: 600;
      color: #10b981;
    }
    
    .vs {
      font-size: 12px;
      color: #9ca3af;
      font-weight: 500;
    }
    
    .compare-value {
      font-size: 20px;
      font-weight: 600;
      color: #6b7280;
    }
  }
  
  .compare-diff {
    display: flex;
    align-items: center;
    gap: 8px;
    font-size: 14px;
    font-weight: 600;
    padding: 8px 12px;
    border-radius: 6px;
    
    &.positive {
      background: #d1fae5;
      color: #059669;
    }
    
    &.negative {
      background: #fee2e2;
      color: #dc2626;
    }
    
    &.neutral {
      background: #f3f4f6;
      color: #6b7280;
    }
    
    .diff-oq {
      font-size: 12px;
      opacity: 0.8;
    }
  }
}

@media (max-width: 768px) {
  .stats-cards,
  .stats-grid,
  .compare-grid {
    grid-template-columns: 1fr;
  }
  
  .date-inputs {
    flex-direction: column;
  }
}
</style>

