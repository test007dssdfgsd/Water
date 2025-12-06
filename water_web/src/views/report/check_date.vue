<template>
  <div class="report-app">
    <div class="report-main">
      <div class="report-header">
        <div class="header-content">
          <h4 class="page-title">
            <mdb-icon icon="calendar-alt" class="mr-2" />
            {{$t('report_date')}}
          </h4>
          
          <div class="stats-summary" v-if="checkList.length > 0">
            <div class="stat-item">
              <span class="stat-label">{{$t('cash')}}</span>
              <span class="stat-value cash">{{formatMoney(all_summ.cash)}}</span>
            </div>
            <div class="stat-item">
              <span class="stat-label">{{$t('card')}}</span>
              <span class="stat-value card">{{formatMoney(all_summ.card)}}</span>
            </div>
            <div class="stat-item">
              <span class="stat-label">{{$t('rasxod')}}</span>
              <span class="stat-value expense">{{formatMoney(all_summ.rasxod)}}</span>
            </div>
            <div class="stat-item">
              <span class="stat-label">{{$t('summ')}}</span>
              <span class="stat-value total">{{formatMoney(all_summ.summ)}}</span>
            </div>
          </div>
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
              @click="clickDate()" 
              size="sm"
            >
              <mdb-icon icon="check" class="mr-1" />
              {{$t('ok')}}
            </mdb-btn>
          </div>
        </div>
      </div>

      <div class="table-container">
        <div v-if="searchQuery" class="search-indicator">
          <span class="icon">🔍</span>
          <span>Qidirilmoqda:</span>
          <b>{{ searchQuery }}</b>
        </div>
        <loader v-if="loading"/>
        <div v-else class="table-wrapper">
          <table class="report-table">
          <thead>
            <tr class="header stiky_position">
              <th  width="40" class="text-left">№</th>
              <th >{{$t('client_name')}}
                <span style="position:relative;">
                  <span @click="sortedArrayAsc('user_name')"><mdb-icon icon="angle-up"  class="px-1 up_down_icon"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                  <span @click="sortedArray('user_name')"><mdb-icon icon="angle-down"  class="px-1 up_down_icon" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                </span>
              </th>
              <th>{{$t('getten_bootle')}}
                <span style="position:relative;">
                  <span ><mdb-icon icon="angle-up"  class="px-1 up_down_icon"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                  <span ><mdb-icon icon="angle-down"  class="px-1 up_down_icon" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                </span>
              </th>
              <th>{{$t('water_count')}}
                <span style="position:relative;">
                  <span ><mdb-icon icon="angle-up"  class="px-1 up_down_icon"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                  <span ><mdb-icon icon="angle-down"  class="px-1 up_down_icon" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                </span>
              </th>
              <th>{{$t('product')}}
                <span style="position:relative;">
                  <span ><mdb-icon icon="angle-up"  class="px-1 up_down_icon"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                  <span ><mdb-icon icon="angle-down"  class="px-1 up_down_icon" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                </span>
              </th>
              <th>{{$t('cash')}}
                <span style="position:relative;">
                  <span @click="sortedArrayAsc('cash')"><mdb-icon icon="angle-up"  class="px-1 up_down_icon"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                  <span @click="sortedArray('cash')"><mdb-icon icon="angle-down"  class="px-1 up_down_icon" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                </span>
              </th>
              <th>{{$t('card')}}
                <span style="position:relative;">
                  <span @click="sortedArrayAsc('card')"><mdb-icon icon="angle-up"  class="px-1 up_down_icon"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                  <span @click="sortedArray('card')"><mdb-icon icon="angle-down"  class="px-1 up_down_icon" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                </span>
              </th>
              <!-- <th>{{$t('skidka')}}
                <span style="position:relative;">
                  <span @click="sortedArrayAsc('online')"><mdb-icon icon="angle-up"  class="px-1 up_down_icon"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                  <span @click="sortedArray('online')"><mdb-icon icon="angle-down"  class="px-1 up_down_icon" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                </span>
              </th> -->
              <th>{{$t('rasxod')}}
                <span style="position:relative;">
                  <span @click="sortedArrayAsc('rasxod')"><mdb-icon icon="angle-up"  class="px-1 up_down_icon"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                  <span @click="sortedArray('rasxod')"><mdb-icon icon="angle-down"  class="px-1 up_down_icon" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                </span>
              </th>
              <th >{{$t('summ')}}
                <span style="position:relative;">
                  <span @click="sortedArrayAsc('summ')"><mdb-icon icon="angle-up"  class="px-1 up_down_icon"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                  <span @click="sortedArray('summ')"><mdb-icon icon="angle-down"  class="px-1 up_down_icon" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                </span>
              </th>
              <th>{{$t('date')}}
                <span style="position:relative;">
                  <span @click="sortedArrayAsc('created_date_time')"><mdb-icon icon="angle-up"  class="px-1 up_down_icon"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                  <span @click="sortedArray('created_date_time')"><mdb-icon icon="angle-down"  class="px-1 up_down_icon" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                </span>
              </th>
              <th width="70">{{$t('Action')}}</th>

              <!-- <th >{{$t('lessons_cout')}}</th> -->

            </tr>
          </thead>
          <tbody>
            <tr v-for="(row,rowIndex) in checkList" :key="rowIndex" @click="show_infoDebit(row)" class="hoverTr">
              <td> <span >{{rowIndex+1}}</span> </td>
              <td> <span >{{row.user_name}}</span> </td>
              <td> <span >{{row.reserverd_number_id_1}}</span> </td>
              <td> <span >{{row.reserverd_number_id_2}}</span> </td>
              <td> <span >{{row.product_name_list_pp_1}}</span> </td>
              <td> <span class="text-success ">{{row.cash.toString().replace(/(\d)(?=(\d{3})+(\.(\d){0,2})*$)/g, '$1 ')}}</span> </td>
              <td> <span class="text-primary">{{row.card.toString().replace(/(\d)(?=(\d{3})+(\.(\d){0,2})*$)/g, '$1 ')}}</span> </td>
              <!-- <td> <span class="text-indigo">{{row.online.toString().replace(/(\d)(?=(\d{3})+(\.(\d){0,2})*$)/g, '$1 ')}}</span> </td> -->
              <td> <span class="text-danger">{{row.rasxod.toString().replace(/(\d)(?=(\d{3})+(\.(\d){0,2})*$)/g, '$1 ')}}</span>  </td>
              <td> <span class="text-indigo">{{row.summ.toString().replace(/(\d)(?=(\d{3})+(\.(\d){0,2})*$)/g, '$1 ')}}</span> </td>
              <td> <span >{{row.created_date_time.slice(0,10)}}</span> <span class="ml-2">{{row.created_date_time.slice(11,16)}}</span></td>
              <!-- <td> <span >{{row.lessons_cout}}</span> </td> -->
              <td class="action-cell">
                <div class="action-buttons-cell">
                  <div @click="promise(row.reserverd_number_id_3, row.user_name)" class="delete-icon-wrapper">
                    <mdb-icon icon="trash" class="delete-icon" />
                  </div>
                </div>
              </td>
            </tr>
          </tbody>
        </table>
        </div>
        <!-- Pagination tugmalari -->
        <div class="pagination-section" v-if="totalPages > 0">
          <Pagination 
            :totalPages="totalPages" 
            :currentPage="currentPage" 
            @page-changed="changePage" 
          />
        </div>
      </div>
    </div>
    <mdb-modal :show="delete_show" @close="delete_show = false" size="md" class="text-center" danger>
      <mdb-modal-header center :close="false">
        <p class="heading">{{$t('Are_you_sure')}}</p>
      </mdb-modal-header>
      <mdb-modal-body>
        <span>Olib borilgan <span class="text-primary font-weight-bold px-2">{{client_name}}</span> zakazni bekor qilmoqchimisz?
          Olib borilmagan zakazlar listiga qaytadi.
        </span>
      </mdb-modal-body>
      <mdb-modal-footer center>
        <mdb-btn outline="danger" @click="deleteOrder">{{$t('Yes')}}</mdb-btn>
        <mdb-btn color="danger" @click="delete_show = false">{{$t('No')}}</mdb-btn>
      </mdb-modal-footer>
    </mdb-modal>
    <Toast ref="message"></Toast>
 </div>
</template>

<script>
// import lineSelect from "../../components/lineSelect.vue";
import Pagination from './pagination.vue'
import {mdbBtn, mdbIcon, mdbInput, mdbModal, mdbModalHeader, mdbModalBody, mdbModalFooter,mdbBadge,mdbBtnGroup, mdbDropdown, mdbDropdownMenu, mdbDropdownItem,} from 'mdbvue'
import {mapActions, mapGetters} from 'vuex'
import month from '../../components/month.vue'
export default {
  components:{
    mdbBtn, 
    mdbIcon,mdbInput,Pagination,
    month,
    mdbModal, mdbModalHeader, mdbModalBody, mdbModalFooter,mdbBadge,mdbBtnGroup, mdbDropdown, mdbDropdownMenu, mdbDropdownItem,
  },
  data() {
    return {
      id: 0,
      loading:false,
      checkList: [],
      b_date:'',
      e_date:'',

      search: '',

      payClient: false,
      today_date: '',
      check_client_list: [],
      group_data: {},

      all_summ: {
        cash: 0,
        card: 0,
        online:0,
        rasxod: 0,
        summ: 0,
        
      },
      delete_show: false,
      order_id: null,
      client_name: '',

      currentPage: 0,
      totalPages: 0,
      pageSize: 100,
      searchQuery: '',
    }
  },
 
  async mounted() {
    let date = new Date();
    this.today_date = date.toISOString().slice(0,10);
    this.b_date = this.today_date;
    this.e_date = this.today_date;
    await this.clickDate();
    window.addEventListener("keydown", this.handleKey);
  },
  beforeDestroy() { window.removeEventListener("keydown", this.handleKey); },
  computed: mapGetters(['allGroups', 'group_client_list']),

  methods: {
    ...mapActions(['fetchGroups', 'fetchClient', 'fetchGroupsClientList']),
    async handleKey(e) {
      if (e.key === "Backspace")
      {
        this.searchQuery = this.searchQuery.slice(0, -1);
        await this.clickDate();
        this.currentPage = 0;

      }
      else if (e.key.length === 1) {
        this.searchQuery += e.key;
        await this.clickDate();
        this.currentPage = 0;
      }
      if(this.searchQuery == '' || this.searchQuery == null){
        this.searchQuery = '';
        await this.clickDate();
        this.currentPage = 0;
      }
    },
    show_infoDebit(i){
      console.log(i)
    },
    changePage (page) {
      this.currentPage = page;
      this.clickDate();
    },

    // ===> send client check to base<===
    payDebit(data){
      this.payClient = true;
      this.group_data = data
    },
    async promise(id, name){
      this.order_id = id;
      this.client_name = name;
      this.delete_show = true;
    },
    async deleteOrder(){
      console.log(this.order_id)
      try{
        const res = await fetch(this.$store.state.hostname + '/WaterOrders/deAgainAcceptAlredyPlannedOrderFullReturningInfo?order_id=' + this.order_id);
        const data = await res.json();
        console.log(data)
        if(res.status == 200 || res.status == 201){
          console.log('das')
          await this.clickDate();
          this.delete_show = false;
        }
        else{
          this.$refs.message.error('not_found')
          this.delete_show = false;
          this.loading = false;
        }
        this.loading = false;
      }
      catch{
        this.$refs.message.error('network_ne_connect')
        this.loading = false;
        this.delete_show = false;
      }

    },
    async clickDate(){
      if(this.b_date == '' || this.e_date == '') return;
      let date = {
        begin_date: this.b_date  + 'T00:00:01.504Z',
        end_date: this.e_date + 'T23:59:01.504Z',
      }
      await this.all_checkSum(date);
      await this.selectMonth(date);
    },
    async all_checkSum(date){
      try{
        this.loading = true;
        const res = await fetch(this.$store.state.hostname + '/WaterChecks/getSummaryByDateTime?begin_date=' + date.begin_date + '&end_date=' + date.end_date);
        const data = await res.json();
        this.loading = false;
        if(res.status == 200 || res.status == 201){
          this.all_summ = {
            cash: data.cash,
            card: data.card,
            online:data.online,
            rasxod: data.rasxod,
            summ: data.summ,
          }
        }
      }
      catch(e){
        this.$refs.message.error(e,'network_ne_connect')
        this.loading = false;
      }
    },
    async selectMonth(date){
      console.log(date)
      this.loading = true;
      try{
        const res = await fetch(this.$store.state.hostname + `/WaterChecks/getPaginationByDateTime?page=${this.currentPage}&size=${this.pageSize}&begin_date=` + date.begin_date + '&end_date=' + date.end_date + '&search=' + this.searchQuery);
        const data = await res.json();
        console.log(data)
        if(res.status == 200 || res.status == 201){
          this.checkList = data.items_list;
          this.totalPages = Math.ceil(data.items_count / this.pageSize);
          // this.all_summ = {
          //   cash: 0,
          //   card: 0,
          //   online:0,
          //   rasxod: 0,
          //   summ: 0,
          // }
          // for(let i=0; i<this.checkList.length; i++){
          //   this.all_summ.cash += parseFloat(this.checkList[i].cash)
          //   this.all_summ.card += parseFloat(this.checkList[i].card)
          //   this.all_summ.online += parseFloat(this.checkList[i].online)
          //   this.all_summ.rasxod += parseFloat(this.checkList[i].rasxod)
          //   this.all_summ.summ += parseFloat(this.checkList[i].summ)
          // }

        }
        else{
          this.$refs.message.error('not_found')
          this.loading = false;
        }
        this.loading = false;
      }
      catch{
        this.$refs.message.error('network_ne_connect')
        this.loading = false;
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
        this.checkList.sort(compare);
    },
    sortedArray(key){
        function compare(a, b) {
          if (a[key] > b[key])
            return -1;
          if (a[key] < b[key])
            return 1;
          return 0;
        }

        this.checkList.sort(compare);
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
        
        &.cash {
          color: #d1fae5;
        }
        
        &.card {
          color: #bfdbfe;
        }
        
        &.expense {
          color: #fecaca;
        }
        
        &.total {
          color: #fef3c7;
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

.search-indicator {
  padding: 12px 16px;
  background: #fef3c7;
  border-bottom: 1px solid #f0f0f0;
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 13px;
  color: #374151;
  
  .icon {
    display: inline-block;
    animation: shake 1s infinite;
  }
  
  b {
    color: #059669;
    font-weight: 600;
  }
}

@keyframes shake {
  0% { transform: rotate(0deg); }
  25% { transform: rotate(15deg); }
  50% { transform: rotate(0deg); }
  75% { transform: rotate(-15deg); }
  100% { transform: rotate(0deg); }
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

.report-table th {
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

.report-table td {
  padding: 10px 12px;
  font-size: 11px;
  color: #374151;
  letter-spacing: -0.01em;
  border-bottom: 1px solid #f3f4f6;
  text-align: left;
}

.report-table tbody tr {
  transition: all 0.15s ease;
  
  &:hover {
    background: #f0fdf4 !important;
    transform: translateX(2px);
  }
}

.stiky_position {
  position: -webkit-sticky;
  position: sticky;
  top: 0;
  background: #10b981;
  color: white;
  z-index: 111111;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
}

.action-cell {
  padding: 8px !important;
  text-align: center;
  
  .action-buttons-cell {
    display: flex;
    justify-content: center;
    align-items: center;
    
    .delete-icon-wrapper {
      cursor: pointer;
      padding: 6px;
      border-radius: 6px;
      transition: all 0.2s ease;
      
      &:hover {
        background: rgba(239, 68, 68, 0.1);
        transform: scale(1.1);
      }
    }
    
    .delete-icon {
      color: #ef4444;
      font-size: 14px;
    }
  }
}

.pagination-section {
  padding: 16px;
  border-top: 1px solid #f3f4f6;
  display: flex;
  justify-content: center;
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
    
    .date-section {
      flex-direction: column;
      
      .date-input {
        width: 100%;
      }
    }
  }
}
</style>