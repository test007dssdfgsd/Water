<template>
  <div class="order-list-app">
    <backRouter />
    <div class="order-list-main">
      <div class="order-list-header">
        <div class="header-content">
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
              class="action-btn" 
              color="info"  
              @click="acceptBtn()" 
              size="sm"
            >
              {{$t('apply')}}
            </mdb-btn>
            
            <mdb-btn 
              :disabled="get_postavchik_order_list.length<=0" 
              class="action-btn m-0" 
              color="orange"  
              @click="$router.push('/map_order_postavchik')" 
              size="sm"
            >
              {{$t('map')}}
            </mdb-btn>
          </div>
        </div>
      </div>
      
      <div class="order-table-container">
        <loader-table v-if="loading" />
        <div v-else class="order-table-wrapper">
          <table class="order-table">
            <thead class="header_table">
              <tr class="stiky_position">
                <th>№</th>
                <th>{{$t('fio')}}
                  <span style="position:relative;">
                    <span @click="sortedArrayAsc('client_name_str')"><mdb-icon icon="angle-up"  class="px-1"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                    <span @click="sortedArray('client_name_str')"><mdb-icon icon="angle-down"  class="px-1" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                  </span>
                </th>
                <th class="text-center">{{$t('water_count')}}
                  <span style="position:relative;">
                    <span @click="sortedArrayAsc('water_count')"><mdb-icon icon="angle-up"  class="px-1"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                    <span @click="sortedArray('water_count')"><mdb-icon icon="angle-down"  class="px-1" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                  </span>
                </th>
                <th>{{$t('address')}}</th>
                <th width="130">{{$t('date')}}
                  <span style="position:relative;">
                    <span @click="sortedArrayAsc('order_date')"><mdb-icon icon="angle-up"  class="px-1"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                    <span @click="sortedArray('order_date')"><mdb-icon icon="angle-down"  class="px-1" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                  </span>
                </th>
                <th width="100">{{$t('Action')}}</th>
              </tr>
            </thead>
            <tbody class="body_table">
              <tr 
                v-for="(item, index) in get_postavchik_order_list" 
                :key="index" 
                class="hoverTr" 
                :style="{background: item.reserverd_note_3}"
              >
                <td>{{index+1}}</td>
                <td class="font-weight-bold" style="font-size: 12px;">{{item.client_name_str}}</td>
                <td class="text-center text-primary font-weight-bold" style="font-size: 12px;">{{item.water_count}}</td>
                <td>{{item.address.address}}</td>
                <td>{{item.order_date.slice(0,10)}}</td>
                <td class="m-0 p-0 px-2">
                  <mdb-btn 
                    class="btn-accept" 
                    @click="showOrder(item)" 
                    size="sm"
                  >
                    {{$t('accept')}}
                  </mdb-btn>
                </td>
              </tr>
            </tbody>
            <tbody class="body_table all_qty_border">
              <tr>
                <td></td>
                <td class="font-weight-bold">Общий</td>
                <td class="text-center font-weight-bold">{{postavchik_all_qty}}</td>
                <td></td>
                <td></td>
                <td></td>
              </tr>
            </tbody>
          </table>
        </div>
      </div>
    </div>
    
    <div class="order-stats-sidebar">
      <div class="stats-card">
        <div class="stats-label">{{$t('all')}}</div>
        <div class="stats-value text-right">{{postavchik_all_qty}}</div>
      </div>
    </div>

    <modal-train  
      :show="pay_show" 
      headerbackColor="white"  
      titlecolor="black" 
      :title="$t('pay')" 
      @close="pay_show = false" 
      width="50%"
    >
      <template v-slot:body>
        <payNewOrder 
          ref="payNew" 
          @close="closeAcceptOrder"  
          @closeUpdate="closeUpdate" 
          :orderId="order_id" 
          :shown="pay_show"
        ></payNewOrder>
      </template>
    </modal-train>
    <Toast ref="message"></Toast>
  </div>
</template>

<script>
import {mdbBtn, mdbInput, mdbIcon} from 'mdbvue'
import loaderTable from '../../components/loaderTable.vue';
import payNewOrder from '../order/payNew.vue'
import erpSelect from "../../components/erpSelectFio.vue";
import { required } from 'vuelidate/lib/validators'
import { mapActions, mapGetters } from 'vuex';

export default {
  components: { loaderTable, mdbBtn, mdbIcon, mdbInput, payNewOrder, erpSelect },
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

      user_name: localStorage.postavchikName,
      user_id: localStorage.postavchikId,
      auth_id: localStorage.postavchikAuthId,
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
      await this.fetchPostavchikOrder(date_and_item);
    },
    showOrder(data){
      console.log(data)
      this.pay_show = true;
      this.order_id = data.id;
      this.$refs.payNew.fetchMounted(data.id);
    },
    async closeAcceptOrder(){
      this.pay_show = false;
      await this.acceptBtn();
    },
    async closeUpdate(){
      this.pay_show = false;
      await this.acceptBtn();
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
      this.get_postavchik_order_list.sort(compare);
    },
    sortedArray(key){
      function compare(a, b) {
        if (a[key] > b[key])
          return -1;
        if (a[key] < b[key])
          return 1;
        return 0;
      }
      this.get_postavchik_order_list.sort(compare);
    }
    // ===> sort table <===
  },
}
</script>

<style lang="scss" scoped>
// Modern, clean, minimal light theme with soft green accents
.order-list-app {
  min-height: 100vh;
  background: #f8fafb;
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen', 'Ubuntu', 'Cantarell', sans-serif;
  display: flex;
  overflow: hidden;
}

.order-list-main {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.order-list-header {
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
    .action-buttons {
      width: 100%;
    }
    
    .date-section {
      flex-direction: column;
      
      .date-input {
        width: 100%;
      }
    }
    
    .action-buttons {
      justify-content: flex-start;
      margin-left: 0;
    }
  }
}

.order-table-container {
  flex: 1;
  overflow: auto;
  padding: 16px;
  background: #f8fafb;
  
  @media (max-width: 991px) {
    padding: 8px 12px;
  }
}

.order-table-wrapper {
  background: #ffffff;
  border-radius: 12px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
  border: 1px solid #f0f0f0;
  overflow: hidden;
}

.order-table {
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
      
      mdb-icon {
        color: rgba(255, 255, 255, 0.8);
        transition: color 0.2s;
        
        &:hover {
          color: white;
        }
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
  }
}

.btn-accept {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%) !important;
  color: white !important;
  border: none !important;
  font-size: 9px !important;
  padding: 4px 12px !important;
  border-radius: 8px;
  font-weight: 600;
  transition: all 0.2s ease;
  
  &:hover {
    transform: translateY(-1px);
    box-shadow: 0 2px 8px rgba(102, 126, 234, 0.3);
  }
}

.order-stats-sidebar {
  width: 200px;
  background: #ffffff;
  border-left: 1px solid #e5e7eb;
  padding: 16px;
  display: flex;
  flex-direction: column;
  
  .stats-card {
    background: linear-gradient(135deg, #ecfdf5 0%, #d1fae5 100%);
    border-radius: 12px;
    padding: 17px 20px;
    box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
    border: 1px solid #a7f3d0;
    
    .stats-label {
      color: #065f46;
      font-size: 14px;
      font-weight: 600;
      margin-bottom: 12px;
      letter-spacing: -0.01em;
    }
    
    .stats-value {
      color: #047857;
      font-size: 28px;
      font-weight: 700;
      letter-spacing: -0.02em;
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

// Legacy class support
.all_info_order {
  min-height: 100vh;
  background: #f8fafb;
  display: flex;
}

.pos_order_list {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}
</style>