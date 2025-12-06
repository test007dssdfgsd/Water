<template>
  <div class="report-app">
    <div class="report-main">
      <div class="report-header">
        <div class="header-content">
          <h4 class="page-title">
            <mdb-icon icon="check-circle" class="mr-2" />
            {{$t('accept_order')}}
          </h4>
          
          <div class="stats-summary" v-if="order_list.length > 0">
            <div class="stat-item">
              <span class="stat-label">{{$t('total_orders')}}</span>
              <span class="stat-value total">{{order_list.length}}</span>
            </div>
            <div class="stat-item">
              <span class="stat-label">{{$t('water_count')}}</span>
              <span class="stat-value water">{{all_water_count}}</span>
            </div>
          </div>
        </div>
      </div>
      
      <div class="controls-section">
        <div class="controls-content">
          <div class="search-section">
            <input-icon 
              class="search-input" 
              v-model="search" 
              @input="searchClick" 
              :placeholder="$t('search_here')"
            />
            <mdb-btn 
              class="search-btn" 
              color="info"  
              @click="searchClick()" 
              size="sm"
            >
              <mdb-icon icon="search" class="mr-1" />
              {{$t('search')}}
            </mdb-btn>
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
          
          <div class="action-buttons">
            <mdb-btn 
              class="action-btn apply-btn" 
              color="info"  
              @click="acceptBtn()" 
              size="sm"
            >
              <mdb-icon icon="check" class="mr-1" />
              {{$t('apply')}}
            </mdb-btn>
          </div>
        </div>
      </div>
      
      <div class="table-container">
        <loader-table v-if="loading" />
        <div v-else class="table-wrapper">
          <table class="report-table">
            <thead class="header_table">
              <tr class="stiky_position">
                <th width="40">№</th>
                <th>{{$t('fio')}}
                  <span style="position:relative;">
                    <span @click="sortedArrayAsc('client_name_str')"><mdb-icon icon="angle-up"  class="px-1 up_down_icon"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                    <span @click="sortedArray('client_name_str')"><mdb-icon icon="angle-down"  class="px-1 up_down_icon" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                  </span>
                </th>
                <th width="100" class="text-center">{{$t('qty')}}
                  <span style="position:relative;">
                    <span @click="sortedArrayAsc('water_count')"><mdb-icon icon="angle-up"  class="px-1 up_down_icon"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                    <span @click="sortedArray('water_count')"><mdb-icon icon="angle-down"  class="px-1 up_down_icon" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                  </span>
                </th>
                <th>{{$t('address')}}</th>
                <th>{{$t('note')}}
                  <span style="position:relative;">
                    <span @click="sortedArrayAsc('note')"><mdb-icon icon="angle-up"  class="px-1 up_down_icon"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                    <span @click="sortedArray('note')"><mdb-icon icon="angle-down"  class="px-1 up_down_icon" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                  </span>
                </th>
                <th width="130">{{$t('date')}}
                  <span style="position:relative;">
                    <span @click="sortedArrayAsc('order_date')"><mdb-icon icon="angle-up"  class="px-1 up_down_icon"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                    <span @click="sortedArray('order_date')"><mdb-icon icon="angle-down"  class="px-1 up_down_icon" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                  </span>
                </th>
              </tr>
            </thead>
            <tbody class="body_table">
              <tr v-for="(item, index) in order_list" :key="index" class="hoverTr">
                <td>{{index+1}}</td>
                <td class="font-weight-bold">{{item.client_name_str}}</td>
                <td class="text-center text-primary font-weight-bold">{{item.water_count}}</td>
                <td>{{item.address.address}}</td>
                <td>{{item.note}}</td>
                <td>{{item.order_date.slice(0,10)}}</td>
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
import { mapActions, mapGetters } from 'vuex';
export default {
  components: { loaderTable, mdbBtn, mdbInput, mdbIcon },
  data() {
    return {
      id: 0,
      loading:false,
      pay_show: false,

      order_list:  [],
      cach_order_list: [],
      order_id: null,
      b_date: '',
      e_date: '',
      today_date: '',
      all_water_count: 0,
      all_water_sum: 0,
      water_price: 0,

      search: '',
      show_order_list: false,
    }
  },
  async mounted() {
    let date = new Date();
    this.today_date = date.toISOString().slice(0,10);
    this.b_date = this.today_date;
    this.e_date = this.today_date;

    let d_time = {
      b_date: this.b_date  + 'T00:00:01.504Z',
      e_date: this.e_date + 'T23:59:01.504Z',
    }

    await this.fetchDateOrderList(d_time);
  },
  computed: {
    ...mapGetters(['allOrder_list']),
  },
  methods: {
  ...mapActions(['fetchOrder_list']),
    async fetchDateOrderList(date){
      this.show_order_list = false;
      try{
        this.loading = true;
        const res = await fetch(this.$store.state.hostname + '/WaterOrders/getPaginationAllAcceptedByDateTme?page=0&size=1000&begin_date='+ date.b_date+'&end_date=' + date.e_date);
        const data = await res.json();
        this.loading = false;
        if(res.status == 200 || res.status == 201){
          console.log('data.items_list')
          console.log(data.items_list)
          this.order_list = data.items_list;
          this.cach_order_list = data.items_list;
          this.all_water_count = 0;
          for(let i=0; i<data.items_list.length; i++){
            this.all_water_count += data.items_list[i].water_count;
          }
        }
        else{
          this.$refs.message.error('network_ne_connect')
        }
      }
      catch{
        this.$refs.message.error('network_ne_connect')
      }
    },
    searchClick(){
      this.order_list = this.cach_order_list;
      this.loading = true;
      if(this.search != ''){
        let userSearchList = [];
        for(let i=0; i<this.order_list.length;i++){
          if(this.order_list[i].client_name_str.toLowerCase().includes(this.search.toLowerCase()) || this.order_list[i].address.address.toLowerCase().includes(this.search.toLowerCase())){
            userSearchList.push(this.order_list[i])
          }
        }
        this.order_list = userSearchList;
      }
      else{
        this.order_list = this.cach_order_list;
      }
      this.loading = false;
    },
    async acceptBtn(){
      let d_time = {
        b_date: this.b_date  + 'T00:00:01.504Z',
        e_date: this.e_date + 'T23:59:01.504Z',
      }
      await this.fetchDateOrderList(d_time);
    },
     // ===> sort table <===
    sortedArrayAsc(key){
        function compare(a, b) {
          if (a[key] < b[key])
            return -1;
          if (a[key] > b[key])
            return 1;
          return 0;
        }
        this.order_list.sort(compare);
    },
    sortedArray(key){
        function compare(a, b) {
          if (a[key] > b[key])
            return -1;
          if (a[key] < b[key])
            return 1;
          return 0;
        }

        this.order_list.sort(compare);
    }
    // ===> sort table <===
  },
}
</script>

<style lang="scss" scoped>
.report-app {
  min-height: 100vh;
  background: #f8fafb;
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen', 'Ubuntu', 'Cantarell', sans-serif;
  padding: 16px;
}

.report-main {
  max-width: 100%;
  margin: 0 auto;
}

// Header section
.report-header {
  background: linear-gradient(135deg, #10b981 0%, #059669 100%);
  border-radius: 12px;
  padding: 10px 24px;
  margin-bottom: 16px;
  box-shadow: 0 2px 8px rgba(16, 185, 129, 0.2);
  
  .header-content {
    display: flex;
    justify-content: space-between;
    align-items: center;
    flex-wrap: wrap;
    gap: 16px;
  }
  
  .page-title {
    color: white;
    font-weight: 600;
    font-size: 20px;
    margin: 0;
    display: flex;
    align-items: center;
    
    mdb-icon {
      font-size: 22px;
    }
  }
  
  .stats-summary {
    display: flex;
    gap: 24px;
    flex-wrap: wrap;
    
    .stat-item {
      display: flex;
      flex-direction: column;
      align-items: center;
      
      .stat-label {
        font-size: 11px;
        color: rgba(255, 255, 255, 0.9);
        margin-bottom: 4px;
        font-weight: 500;
      }
      
      .stat-value {
        font-size: 16px;
        font-weight: 700;
        color: white;
        
        &.total {
          color: #fef3c7;
        }
        
        &.water {
          color: #e0e7ff;
        }
      }
    }
  }
}

// Controls section
.controls-section {
  background: #ffffff;
  border-radius: 12px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
  border: 1px solid #f0f0f0;
  padding: 16px;
  margin-bottom: 16px;
  
  .controls-content {
    display: flex;
    gap: 16px;
    align-items: flex-end;
    flex-wrap: wrap;
  }
  
  .search-section {
    display: flex;
    gap: 8px;
    align-items: center;
    flex: 1;
    min-width: 300px;
    
    .search-input {
      flex: 1;
      max-width: 400px;
    }
    
    .search-btn {
      border-radius: 8px;
      font-size: 12px;
      font-weight: 500;
      padding: 6px 16px;
      height: 36px;
      transition: all 0.2s ease;
      box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
      background: linear-gradient(135deg, #3b82f6 0%, #2563eb 100%);
      border: none;
      
      &:hover {
        transform: translateY(-1px);
        box-shadow: 0 4px 8px rgba(0, 0, 0, 0.15);
      }
      
      mdb-icon {
        font-size: 13px;
      }
    }
  }
  
  .date-section {
    display: flex;
    gap: 12px;
    
    .date-input {
      min-width: 160px;
      border-radius: 8px;
    }
  }
  
  .action-buttons {
    display: flex;
    gap: 8px;
    
    .action-btn {
      border-radius: 8px;
      font-size: 12px;
      font-weight: 500;
      padding: 6px 16px;
      height: 36px;
      transition: all 0.2s ease;
      box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
      
      &:hover:not(:disabled) {
        transform: translateY(-1px);
        box-shadow: 0 4px 8px rgba(0, 0, 0, 0.15);
      }
      
      &.apply-btn {
        background: linear-gradient(135deg, #3b82f6 0%, #2563eb 100%);
        border: none;
      }
      
      &:disabled {
        opacity: 0.5;
        cursor: not-allowed;
      }
      
      mdb-icon {
        font-size: 13px;
      }
    }
  }
}

// Table section
.table-container {
  background: #ffffff;
  border-radius: 12px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
  border: 1px solid #f0f0f0;
  overflow: hidden;
}

.table-wrapper {
  overflow-x: auto;
}

.report-table {
  width: 100%;
  border-collapse: separate;
  border-spacing: 0;
  font-size: 12px;
  
  tbody tr:nth-child(even) {
    background-color: #fafbfc;
  }
}

.header_table {
  background: #10b981;
  
  th {
    padding: 10px 12px;
    font-weight: 600;
    font-size: 11px;
    color: white;
    letter-spacing: -0.01em;
    text-align: left;
    
    .up_down_icon {
      color: white;
      transition: all 0.2s ease;
      padding: 2px;
      border-radius: 4px;
      
      &:hover {
        background: rgba(255, 255, 255, 0.2);
      }
    }
  }
}

.body_table {
  td {
    padding: 10px 12px;
    font-size: 11px;
    color: #374151;
    letter-spacing: -0.01em;
    border-bottom: 1px solid #f3f4f6;
  }
  
  tr {
    transition: all 0.15s ease;
    
    &:hover {
      background: #f0fdf4 !important;
      transform: translateX(2px);
    }
  }
}

.stiky_position {
  position: -webkit-sticky;
  position: sticky;
  top: 0;
  z-index: 111111;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

@media (max-width: 768px) {
  .report-header {
    .header-content {
      flex-direction: column;
      align-items: flex-start;
    }
    
    .stats-summary {
      width: 100%;
      justify-content: space-around;
    }
  }
  
  .controls-section {
    .controls-content {
      flex-direction: column;
      align-items: stretch;
    }
    
    .search-section {
      flex-direction: column;
      min-width: 100%;
      
      .search-input {
        width: 100%;
        max-width: 100%;
      }
      
      .search-btn {
        width: 100%;
      }
    }
    
    .date-section {
      flex-direction: column;
      
      .date-input {
        width: 100%;
      }
    }
    
    .action-buttons {
      width: 100%;
      
      .action-btn {
        flex: 1;
      }
    }
  }
}
</style>