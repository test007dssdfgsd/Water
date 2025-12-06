<template>
  <div class="report-app">
    <div class="report-main">
      <div class="report-header">
        <div class="header-content">
          <h4 class="page-title">
            <mdb-icon icon="chart-bar" class="mr-2" />
            {{$t('statistic')}}
          </h4>
          
          <div class="stats-summary">
            <div class="stat-item">
              <span class="stat-label">{{$t('summ')}}</span>
              <span class="stat-value total">{{formatMoney(all_money)}}</span>
            </div>
            <div class="stat-item">
              <span class="stat-label">{{$t('client')}}</span>
              <span class="stat-value clients">{{real_clients}}</span>
            </div>
          </div>
        </div>
      </div>
      
      <div class="controls-section">
        <div class="controls-content">
          <div class="user-select-section">
            <erpSelect
              size="sm"
              :options="allDepartment.rows"
              @select="selectOption"
              :selected="district_name"
              :label="$t('province')"
            />
            <small v-if="$v.district_name.$dirty && district_id == null" class="invalid-text">
              {{$t('select_item')}}
            </small>
          </div>
          
          <div class="client-search-section">
            <input-search  
              @select="selectClient"  
              url="/WaterClients/getPaginationByName?page=0&size=100&fio="
              ref="search_client" 
              :option="allClient.rows" 
              icon="user" 
              style="height:32px;"
            />
            <small class="search-label">
              {{$t('search_client')}}
            </small>
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

            <mdb-btn 
              class="action-btn update-btn" 
              color="primary"  
              @click="update()" 
              size="sm"
            >
              <mdb-icon icon="sync" class="mr-1" />
              {{$t('update')}}
            </mdb-btn>
          </div>
        </div>
      </div>
      
      <div class="table-container">
        <loader-table v-if="loading"/>
        <div v-else class="table-wrapper">
          <table class="report-table">
          <thead class="header_table">
            <tr class="header py-3 stiky_position" >
              <th  width="40" class="text-left">№</th>
              <th  width="120">{{$t('client_name')}}
                <span style="position:relative;">
                  <span @click="sortedArrayAsc2('fio')"><mdb-icon icon="angle-up"  class="px-1 up_down_icon"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                  <span @click="sortedArray2('fio')"><mdb-icon icon="angle-down"  class="px-1 up_down_icon" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                </span>
              </th>
              <th  width="150">{{$t('district')}}
                <span style="position:relative;">
                  <span @click="sortedArrayAsc('tuman_name')"><mdb-icon icon="angle-up"  class="px-1 up_down_icon"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                  <span @click="sortedArray('tuman_name')"><mdb-icon icon="angle-down"  class="px-1 up_down_icon" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                </span>
              </th>
                <th  width="150">{{$t('first_order')}}
                <span style="position:relative;">
                  <span @click="sortedArrayAsc('first_order_date')"><mdb-icon icon="angle-up"  class="px-1 up_down_icon"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                  <span @click="sortedArray('first_order_date')"><mdb-icon icon="angle-down"  class="px-1 up_down_icon" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                </span>
              </th>
                <th  width="150">{{$t('order_count')}}
                <span style="position:relative;">
                  <span @click="sortedArrayAsc('zakazlar_soni')"><mdb-icon icon="angle-up"  class="px-1 up_down_icon"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                  <span @click="sortedArray('zakazlar_soni')"><mdb-icon icon="angle-down"  class="px-1 up_down_icon" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                </span>
              </th>
              <th >{{$t('address')}}
                
              </th>
              <th  width="125">{{$t('ostatka')}}
                <span style="position:relative;">
                  <span @click="sortedArrayAsc('bakalashka_soni1')"><mdb-icon icon="angle-up"  class="px-1 up_down_icon"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                  <span @click="sortedArray('bakalashka_soni1')"><mdb-icon icon="angle-down"  class="px-1 up_down_icon" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                </span>
              </th>
              <th  width="125">{{$t('getten')}}
                <span style="position:relative;">
                  <span @click="sortedArrayAsc('olgan_suv_soni')"><mdb-icon icon="angle-up"  class="px-1 up_down_icon"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                  <span @click="sortedArray('olgan_suv_soni')"><mdb-icon icon="angle-down"  class="px-1 up_down_icon" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                </span>
              </th>
              <th width="120">{{$t('date')}}
                <span style="position:relative;">
                  <span @click="sortedArrayAsc3('last_order_date')"><mdb-icon icon="angle-up"  class="px-1 up_down_icon"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                  <span @click="sortedArray3('last_order_date')"><mdb-icon icon="angle-down"  class="px-1 up_down_icon" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                </span>
              </th>
            </tr>
          </thead>
          <tbody class="body_table">
            <tr v-for="(row, rowIndex) in order_list" :key="rowIndex" class="hoverTr">
              <td> <span >{{rowIndex+1}}</span> </td>
              <td> <span >{{row.fio}}</span> </td>
              <td> <span >{{row.tuman_name}}</span> </td>
              <td> <span >{{row.first_order_date}}</span> </td>
              <td> <span >{{row.zakazlar_soni}}</span> </td>
              <td> <span >{{row.address}}</span> </td>
              <td> <span >{{row.bakalashka_soni1}}</span> </td>
              <td> <span >{{row.olgan_suv_soni}}</span>  </td>
              <td> <span >{{row.last_order_date}}</span> </td>
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
import erpSelect from "../../components/erpSelect.vue";
import { required } from 'vuelidate/lib/validators';
import inputSearch from '../../components/inputSearch.vue'
import { mapActions, mapGetters } from 'vuex';


export default {
  components: { loaderTable, mdbBtn, mdbInput, mdbIcon, erpSelect, inputSearch },
  data() {
    return {
      id: 0,
      loading:false,
      pay_show: false,

      order_list:  [],
      order_id: null,
      b_date: '',
      e_date: '',
      today_date: '',
      all_water_count: 0,

      search: '',
      pos_money_report_list: [],

      district_name: '',
      district_id: 0,

      all_money : 0,
      real_clients :0,

      client_id: 0,
      client_name: ''
    }
  },
  validations: {
    district_name: {required},
  },
  computed: {
    ...mapGetters([ 'allUser', 'get_postavchik_order_list', 'postavchik_all_qty','allDepartment', 'allClient']),
  },
  async mounted() {
    await this.fetchDepartment();
    await this.fetchClient();
    await this.acceptBtn();
    await this.get_money();
  },
  methods: {
  ...mapActions(['fetchUser', 'fetchPostavchikOrder', 'fetchDepartment', 'fetchClient']),
    async selectOption(option){
      console.log(option)
      this.district_name = option.name;
      this.district_id = option.id;
      // await this.fetchPostavchikList(this.auth_id);
    },
    async selectClient(option){
      console.log(option)
      this.client_name = option.fio;
      this.client_id = option.id;
        await this.acceptBtn();
        //this.$refs.message.focus();
         
    },
    async update(){
      this.district_name ='';
      this.district_id = 0;
      this.client_id = 0;
      this.client_name = ''
        await this.acceptBtn();
    },
    async get_money()
    {
      try{
        this.loading = true;
        const res = await fetch(this.$store.state.hostname + '/WaterClients/getInfoAboutMoney');
        const data = await res.json();
        this.loading = false;
        if(res.status == 200 || res.status == 201){
          console.log('data.full_money')
          console.log(data)
           this.all_money = data[0].full_money;
           this.real_clients = data[0].real_client;
        }
        else{
          this.$refs.message.error('network_ne_connect')
        }
      }
      catch{
        this.$refs.message.error('network_ne_connect')
      }
    },
    async acceptBtn(){
        try{
        this.loading = true;
        const res = await fetch(this.$store.state.hostname + '/WaterClients/getPaginationStatistikReportQuery2?page=0&size=10000&tuman_id='+ this.district_id + '&client_id=' + this.client_id);
        const data = await res.json();
        this.loading = false;
        if(res.status == 200 || res.status == 201){
          console.log('data.items_list')
          console.log(data)
            this.order_list = data.items_list;
        }
        else{
          this.$refs.message.error('network_ne_connect')
        }
      }
      catch{
        this.$refs.message.error('network_ne_connect')
      }

    },

   sortedArrayAsc2(key){
        function compare(a, b) { 
          if ( parseInt(a[key].substring(1), 10) < parseInt(b[key].substring(1), 10))
            return -1;
          else
            return 1;
         
        }
        this.order_list.sort(compare);
    },
    sortedArray2(key){
        function compare(a, b) {
          if ( parseInt(a[key].substring(1), 10) > parseInt(b[key].substring(1), 10))
            return -1;
          else
            return 1;
         
        }

        this.order_list.sort(compare);
    },
     sortedArrayAsc3(key){
        function compare(a, b) { 
           if (a[key] != null && b[key] != null )
            return new Date(b[key].split('_')[0]) - new Date(a[key].split('_')[0]);
          else
            return 1;
         
        }
        this.order_list.sort(compare);
    },
    sortedArray3(key){
        function compare(a, b) {
          // var fields_a = a[key].split('_')[0];
          if (a[key] != null && b[key] != null )
            return new Date(a[key].split('_')[0]) - new Date(b[key].split('_')[0]);
          else
            return 1;
         
        }

        this.order_list.sort(compare);
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
    },
    // ===> sort table <===
    
    formatMoney(value) {
      if (!value) return '0';
      return value.toString().replace(/(\d)(?=(\d{3})+(\.(\d){0,2})*$)/g, '$1 ');
    }

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
        
        &.clients {
          color: #d1fae5;
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
  
  .user-select-section {
    max-width: 300px;
    width: 100%;
    
    .invalid-text {
      color: #ef4444;
      font-size: 11px;
      margin-top: 4px;
      display: block;
    }
  }
  
  .client-search-section {
    flex: 1;
    min-width: 250px;
    position: relative;
    
    .search-label {
      position: absolute;
      left: 5px;
      top: -17px;
      font-size: 12px;
      color: #6b7280;
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
      
      &.update-btn {
        background: linear-gradient(135deg, #8b5cf6 0%, #7c3aed 100%);
        border: none;
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
    
    .client-search-section {
      width: 100%;
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