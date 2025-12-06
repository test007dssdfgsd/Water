<template>
  <div class="report-app">
    <div class="report-main">
      <div class="report-header">
        <div class="header-content">
          <h4 class="page-title">
            <mdb-icon icon="chart-line" class="mr-2" />
            {{$t('postavchik_report')}}
          </h4>
          
          <div class="stats-summary" v-if="pos_money_report_list.length > 0">
            <div class="stat-item">
              <span class="stat-label">{{$t('cash')}}</span>
              <span class="stat-value cash">{{formatMoney(all_summ.cash)}}</span>
            </div>
            <div class="stat-item">
              <span class="stat-label">{{$t('card')}}</span>
              <span class="stat-value card">{{formatMoney(all_summ.card)}}</span>
            </div>
            <div class="stat-item">
              <span class="stat-label">{{$t('summ')}}</span>
              <span class="stat-value total">{{formatMoney(all_summ.summ)}}</span>
            </div>
            <div class="stat-item">
              <span class="stat-label">Tarqatildi / Vozvrat</span>
              <span class="stat-value water">{{all_summ.tarqatildi}} / {{all_summ.vozvrat}}</span>
            </div>
          </div>
        </div>
      </div>
      
      <div class="controls-section">
        <div class="controls-content">
          <div class="user-select-section">
            <erpSelect
              size="sm"
              :options="allUser.rows" 
              @select="selectOption"
              :selected="user_name"
              :label="$t('province')"
            />
            <small v-if="$v.user_name.$dirty && user_id == null" class="invalid-text">
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
              :disabled="get_postavchik_order_list.length<=0" 
              class="action-btn map-btn" 
              color="orange"  
              @click="$router.push('/map_order_postavchik')" 
              size="sm"
            >
              <mdb-icon icon="map" class="mr-1" />
              {{$t('map')}}
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
              <th  width="40" class="text-left">№</th>
              <th>{{$t('client_name')}}
                <span style="position:relative;">
                  <span @click="sortedArrayAsc('user_name')"><mdb-icon icon="angle-up"  class="px-1 up_down_icon"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                  <span @click="sortedArray('user_name')"><mdb-icon icon="angle-down"  class="px-1 up_down_icon" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                </span>
              </th>
              <!-- <th>{{$t('getten_bootle')}}
                <span style="position:relative;">
                  <span ><mdb-icon icon="angle-up"  class="px-1 up_down_icon"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                  <span ><mdb-icon icon="angle-down"  class="px-1 up_down_icon" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                </span>
              </th> -->
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
              <th>{{$t('summ')}}
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
          <tbody class="body_table">
            <tr v-for="(row, rowIndex) in pos_money_report_list" :key="rowIndex" class="hoverTr">
              <td> <span >{{rowIndex+1}}</span> </td>
              <td> <span >{{row.user_name}}</span> </td>
              <!-- <td> <span >{{row.reserverd_number_id_1}}</span> </td> -->
              <td> <span >{{row.reserverd_number_id_2}} / {{row.reserverd_number_id_1}} </span> </td>
              <td> <span >{{row.product_name_list_pp}}</span> </td>
              <td> <span class="text-success ">{{row.cash.toString().replace(/(\d)(?=(\d{3})+(\.(\d){0,2})*$)/g, '$1 ')}}</span> </td>
              <td> <span class="text-primary">{{row.card.toString().replace(/(\d)(?=(\d{3})+(\.(\d){0,2})*$)/g, '$1 ')}}</span> </td>
              <!-- <td> <span >{{row.online}}</span> </td>
              <td> <span >{{row.rasxod}}</span>  </td> -->
              <td> <span class="text-indigo font-weight-bold">{{row.summ.toString().replace(/(\d)(?=(\d{3})+(\.(\d){0,2})*$)/g, '$1 ')}}</span> </td>
              <td width="170"> <span >{{row.created_date_time.slice(0,10)}}</span> <span class="ml-2">{{row.created_date_time.slice(11,16)}}</span></td>
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
import {mdbBtn, mdbInput, mdbIcon, mdbModal, mdbModalHeader, mdbModalBody, mdbModalFooter,mdbBadge,mdbBtnGroup, mdbDropdown, mdbDropdownMenu, mdbDropdownItem,} from 'mdbvue'
import loaderTable from '../../components/loaderTable.vue';
import erpSelect from "../../components/erpSelectFio.vue";
import { required } from 'vuelidate/lib/validators'
import { mapActions, mapGetters } from 'vuex';


export default {
  components: { loaderTable, mdbBtn, mdbInput,mdbIcon, erpSelect,
    mdbModal, mdbModalHeader, mdbModalBody, mdbModalFooter,mdbBadge,
    mdbBtnGroup, mdbDropdown, mdbDropdownMenu, mdbDropdownItem,
  },
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

      user_name: localStorage.postavchikName,
      user_id: localStorage.postavchikId,
      auth_id: localStorage.postavchikAuthId,

      all_cash: 0,
      all_card: 0,
      all_summ: 0,
      all_summ: {
        cash: 0,
        tarqatildi : 0,
        vozvrat : 0,
        card: 0,
        online:0,
        rasxod: 0,
        summ: 0,
      },

      delete_show: false,
      order_id: null,
      client_name: '',
    }
  },
  validations: {
    user_name: {required},
  },
  computed: {
    ...mapGetters([ 'allUser', 'get_postavchik_order_list', 'postavchik_all_qty']),
  },
  async mounted() {
    let date = new Date();
    this.today_date = date.toISOString().slice(0,10);
    this.b_date = this.today_date;
    this.e_date = this.today_date;

    await this.fetchUser();
    console.log(localStorage.postavchikAuthId)
    if(this.auth_id){
      await this.acceptBtn();
    }
  },
  methods: {
  ...mapActions(['fetchUser', 'fetchPostavchikOrder']),
    async selectOption(option){
      console.log(option)
      this.user_name = option.fio;
      this.user_id = option.id;
      this.auth_id = option.auth.id;
      localStorage.postavchikAuthId = this.auth_id;
      localStorage.postavchikId = this.user_id;
      localStorage.postavchikName = this.user_name;
      // await this.fetchPostavchikList(this.auth_id);
    },
    async acceptBtn(){
       if(this.$v.$invalid)
        {
          this.$v.$touch();
          this.$refs.message.warning('please_fill')
          return false;
        }
      let date_and_item = {
        b_date: this.b_date  + 'T00:00:01.504Z',
        e_date: this.e_date + 'T23:59:01.504Z',
        auth_id: this.auth_id
      }
      await this.fetchPosMoneyReport(date_and_item);
    },

    async fetchPosMoneyReport(date_auth_id){
      console.log(date_auth_id)
      try{
        this.loading = true;
        const res = await fetch(this.$store.state.hostname + '/WaterChecks/getPaginationByAuthIdByDateTime?page=0&size=1000&auth_id='+ date_auth_id.auth_id + '&begin_date=' + date_auth_id.b_date + '&end_date=' + date_auth_id.e_date);
        const data = await res.json();
        this.loading = false;
        if(res.status == 200 || res.status == 201){
          console.log('data.items_list')
          console.log(data)
          this.pos_money_report_list = data.items_list;
          // this.all_water_count = 0;
          this.all_summ = {
            cash: 0,
            card: 0,
            tarqatildi: 0,
            vozvrat: 0,
            online:0,
            rasxod: 0,
            summ: 0,
          }
          for(let i=0; i<this.pos_money_report_list.length; i++){
            this.all_summ.cash += parseFloat(this.pos_money_report_list[i].cash)
            this.all_summ.tarqatildi += parseFloat(this.pos_money_report_list[i].reserverd_number_id_2)
            this.all_summ.vozvrat += parseFloat(this.pos_money_report_list[i].reserverd_number_id_1)
            this.all_summ.card += parseFloat(this.pos_money_report_list[i].card)
            this.all_summ.online += parseFloat(this.pos_money_report_list[i].online)
            this.all_summ.rasxod += parseFloat(this.pos_money_report_list[i].rasxod)
            this.all_summ.summ += parseFloat(this.pos_money_report_list[i].summ)
          }
           console.log('bak -- ' + this.all_summ.tarqatildi)
        }
        else{
          this.$refs.message.error('network_ne_connect')
        }
      }
      catch{
        this.$refs.message.error('network_ne_connect')
      }
    },

    async promise(id, name){
      console.log(id)
      console.log(name)
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
          await this.acceptBtn();
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
    formatMoney(value) {
      if (!value) return '0';
      return value.toString().replace(/(\d)(?=(\d{3})+(\.(\d){0,2})*$)/g, '$1 ');
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
        this.pos_money_report_list.sort(compare);
    },
    sortedArray(key){
        function compare(a, b) {
          if (a[key] > b[key])
            return -1;
          if (a[key] < b[key])
            return 1;
          return 0;
        }

        this.pos_money_report_list.sort(compare);
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
      
      &.map-btn {
        background: linear-gradient(135deg, #f97316 0%, #ea580c 100%);
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
    
    .action-buttons {
      width: 100%;
      
      .action-btn {
        flex: 1;
      }
    }
  }
}
</style>