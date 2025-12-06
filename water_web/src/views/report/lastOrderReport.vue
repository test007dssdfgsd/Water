<template>
  <div class="report-app">
    <div class="report-main">
      <div class="report-header">
        <div class="header-content">
          <h4 class="page-title">
            <mdb-icon icon="history" class="mr-2" />
            {{$t('lastOrderReport')}}
          </h4>
        </div>
      </div>
      
      <div class="controls-section">
        <div class="controls-content">
          <div class="date-section">
            <mdb-input 
              class="date-input m-0" 
              size="sm" 
              v-model="b_date" 
              type="date"
            ></mdb-input>
          </div>
          
          <div class="client-search-section">
            <input-search  
              @select="selectClient"  
              url="/WaterClients/getPaginationByName?page=0&size=100&fio="
              ref="search_client" 
              :option="allClient.rows"
              style="height:32px;" 
              icon="user"
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
            <tr class="header py-3 stiky_position">
              <th  width="40" class="text-left">№</th>
              <th  width="120">{{$t('client_name')}}
                <span style="position:relative;">
                  <span @click="sortedArrayAsc('fio')"><mdb-icon icon="angle-up"  class="px-1 up_down_icon"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                  <span @click="sortedArray('fio')"><mdb-icon icon="angle-down"  class="px-1 up_down_icon" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                </span>
              </th>
              <th  width="150">{{$t('note')}}
                <span style="position:relative;">
                  <span @click="sortedArrayAsc('note')"><mdb-icon icon="angle-up"  class="px-1 up_down_icon"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                  <span @click="sortedArray('note')"><mdb-icon icon="angle-down"  class="px-1 up_down_icon" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                </span>
              </th>
              <th width="150">
                {{$t('water_count')}}
              </th>
              <th width="150">
                {{$t('order_date')}}
              </th>
              
            </tr>
          </thead>
          <tbody class="body_table">
            <tr v-for="(row, rowIndex) in order_list" :key="rowIndex" class="hoverTr">
              <td> <span >{{rowIndex+1}}</span> </td>
              <td> <span >{{row.fio}}</span> </td>
              <td> <span >{{row.note}}</span> </td>
              <td> <span v-if="row.last_order_of_client != null" >{{row.last_order_of_client.water_count}}</span> </td>
              <td> <span v-if="row.last_order_of_client != null">{{row.last_order_of_client.order_date.slice(0,10)}}</span> </td>
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
// import { required } from 'vuelidate/lib/validators';
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

      client_id: 0,
      client_name: ''
    }
  },
  // validations: {
  // },
  computed: {
    ...mapGetters([ 'allUser', 'get_postavchik_order_list', 'postavchik_all_qty','allDepartment', 'allClient']),
  },
  async mounted() {
    let date = new Date();
    this.today_date = date.toISOString().slice(0,10);
    this.b_date = this.today_date;
    this.e_date = this.today_date;
    await this.fetchClient();
    await this.acceptBtn();
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
    },
    async update(){
      this.district_name ='';
      this.district_id = 0;
      this.client_id = 0;
      this.client_name = ''
        await this.acceptBtn();
    },
    async acceptBtn(){
        try{
        this.loading = true;
        let date = this.b_date + 'T23:00:10.504Z'
        const res = await fetch(this.$store.state.hostname + '/WaterClients/getLastOrderWithDate?date_time='+ date + '&client_id=' + this.client_id);
        const data = await res.json();
        this.loading = false;
        if(res.status == 200 || res.status == 201){
          console.log('data.items_list')
          console.log(data)
            this.order_list = data;
        }
        else{
          this.$refs.message.error('network_ne_connect')
        }
      }
      catch{
        this.$refs.message.error('network_ne_connect')
      }

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
  
  .date-section {
    display: flex;
    gap: 12px;
    
    .date-input {
      min-width: 160px;
      border-radius: 8px;
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
  table-layout: fixed;
  
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
  }
  
  .controls-section {
    .controls-content {
      flex-direction: column;
      align-items: stretch;
    }
    
    .date-section {
      flex-direction: column;
      
      .date-input {
        width: 100%;
      }
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