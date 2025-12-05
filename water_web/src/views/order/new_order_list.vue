<template>
  <div class="order-list-app">
    <backRouter />
    <div class="order-list-main">
      <div class="order-list-header">
        <div class="header-content">
          <div class="search-section">
            <div class="search-wrapper">
              <input-icon 
                style="width: 300px; height:28px;" 
                v-model="search" 
                @input="searchClick" 
                :placeholder="$t('search_here')"
              ></input-icon>
              <mdb-btn 
                class="search-btn m-0" 
                color="info"  
                @click="searchClick()" 
                size="sm"
              >
                {{$t('search')}}
              </mdb-btn>
            </div>
          </div>
          
          <div class="date-section">
            <mdb-input 
              class="date-input m-0 ml-3" 
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
            
            <router-link to="/map_order">
              <mdb-btn 
                class="action-btn m-0" 
                color="orange" 
                size="sm"
              >
                {{$t('map')}}
              </mdb-btn>
            </router-link>

            <mdb-btn 
              class="action-btn ml-5" 
              color="primary"  
              @click="fetchTodayOrderList" 
              size="sm"
            >
              {{$t('today')}}
            </mdb-btn>
            
            <mdb-btn 
              class="action-btn m-0 " 
              color="secondary"  
              @click="fetchTomorrowOrderList" 
              size="sm"
            >
              {{$t('tomorrow')}}
            </mdb-btn>
            
            <mdb-btn 
              class="action-btn m-0 px-2" 
              color="indigo"  
              @click="fetchAllOrderList" 
              size="sm"
            >
              {{$t('all')}}
            </mdb-btn>
          </div>
        </div>
      </div>
      <div class="order-table-container">
        <loader-table v-if="loading" />
        <div v-else class="order-table-wrapper">
          <table class="order-table">
          <thead class="header_table ">
            <tr class="stiky_position">
              <th>№</th>
               <th>Id</th>
              <th>{{$t('fio')}}
                <span style="position:relative;">
                  <span @click="sortedArrayAsc('client_name_str')"><mdb-icon icon="angle-up"  class="px-1 "  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                  <span @click="sortedArray('client_name_str')"><mdb-icon icon="angle-down"  class="px-1 " style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                </span>
              </th>
              <th width="80" class="text-center">{{$t('qty')}}
                <span style="position:relative;">
                  <span @click="sortedArrayAsc('water_count')"><mdb-icon icon="angle-up"  class="px-1 "  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                  <span @click="sortedArray('water_count')"><mdb-icon icon="angle-down"  class="px-1 " style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                </span>
              </th>
              <th width="100">{{$t('product')}}
                <span style="position:relative;">
                  <span ><mdb-icon icon="angle-up"  class="px-1 up_down_icon"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                  <span ><mdb-icon icon="angle-down"  class="px-1 up_down_icon" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                </span>
              </th>
              <th>{{$t('address')}}</th>
              <th style="min-width:140px;">{{$t('note')}}
                <span style="position:relative;">
                  <span @click="sortedArrayAsc('note')"><mdb-icon icon="angle-up"  class="px-1 "  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                  <span @click="sortedArray('note')"><mdb-icon icon="angle-down"  class="px-1 " style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                </span>
              </th>
              <th width="130">{{$t('date')}}
                <span style="position:relative;">
                  <span @click="sortedArrayAsc('order_date')"><mdb-icon icon="angle-up"  class="px-1 "  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                  <span @click="sortedArray('order_date')"><mdb-icon icon="angle-down"  class="px-1 " style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                </span>
              </th>
              <th>Car_id</th>
              <th width="60">{{$t('Action')}}</th>
            </tr>
          </thead>
          <tbody class="body_table">
            <tr v-for="(item, index) in order_list" :key="index" class="hoverTr" :style="{background: item.reserverd_note_3 + '!important',}" :class="{'bg_dark_tr text-white': item.color_value == 'black', 'bg_red_tr text-white': item.color_value == 'green', 'bg_reded_tr text-white': item.color_value == 'red'}">
              <td>{{index+1}}</td>
              <td>{{item.id}}</td>
              <td class=" font-weight-bold" style="font-size: 12px;">{{item.client_name_str}}</td>
              <td class="text-center text-primary font-weight-bold" style="font-size: 12px;">{{item.water_count}}</td>
              <td>{{item.name_pp1}}</td>
              <td>{{item.address.address}}</td>
              <td>{{item.note}}</td>
              <td>{{item.order_date.slice(0,10)}}</td>
              <td>{{item.deleivered_user_auth_id}}</td>
              <td class="m-0 p-0 px-2 bg-white">
                <div class="d-flex align-items-center">
                  <div @click="UpdateOrder(item)" style="cursor:pointer" class="">
                    <mdb-icon icon="edit" style="font-size:14px;" class="p-1 text-warning " far></mdb-icon>
                  </div>
                  <div @click="showOrder(item)" style="cursor:pointer">
                    <mdb-icon icon="check-circle" style="font-size:14px;" class="p-1 text-success" far></mdb-icon>
                  </div>
                  <div @click="deleteOrder(item)" style="cursor:pointer">
                    <mdb-icon icon="trash" style="font-size:14px;" class="p-1 text-danger" ></mdb-icon>
                  </div>
                  <div @click="delete_from_user(item)" style="cursor:pointer" class="">
                    <mdb-icon icon="minus-square" style="font-size:14px;" class="p-1 text-danger " far></mdb-icon>
                  </div>
                </div>
                <!-- <mdb-btn class="mr-1 ml-0 mt-0 mt-1 btn-acp"  style="font-size: 8px; width:80px; padding: 5px;"   
                  size="sm">{{$t('accept')}}
                </mdb-btn> -->
              </td>
            </tr>
          </tbody>
        </table>
        </div>
      </div>
    </div>
    <div class="order-stats-sidebar">
      <div class="stats-card">
        <div class="stats-label">{{$t('all')}}</div>
        <div class="stats-value text-right">{{all_water_count}}</div>
      </div>
    </div>

    <modal-train  :show="pay_show" headerbackColor="white"  titlecolor="black" :title="$t('pay')" 
      @close="pay_show = false" width="50%">
        <template v-slot:body>
          <payNewOrder ref="payNew" @close="closeAcceptOrder" @closeUpdate="closeUpdate"  :orderId="order_id" :shown="pay_show"></payNewOrder>
        </template>
    </modal-train>
    <modal-train  :show="update_show" headerbackColor="white"  titlecolor="black" :title="$t('editCol_edit')" 
      @close="update_show = false" width="50%">
        <template v-slot:body>
          <payNewUpdate ref="updateOrder" @close="closeUpdateOrder" @closeUpdate="closeUpdateOrder"  :orderId="order_id" :shown="update_show"></payNewUpdate>
        </template>
    </modal-train>
    <mdb-modal :show="delete_show" @close="delete_show = false" size="sm" class="text-center" danger>
      <mdb-modal-header center :close="false">
        <p class="heading">{{$t('Are_you_sure')}}</p>
      </mdb-modal-header>
      <mdb-modal-body>
        <span>Zakazni bekor qilmoqchimisz?</span>
      </mdb-modal-body>
      <mdb-modal-footer center>
        <mdb-btn outline="danger" @click="promise">{{$t('Yes')}}</mdb-btn>
        <mdb-btn color="danger" @click="delete_show = false">{{$t('No')}}</mdb-btn>
      </mdb-modal-footer>
    </mdb-modal>
    <Toast ref="message"></Toast>
  </div>
</template>

<script>
import {mdbBtn, mdbInput, mdbIcon, mdbModal, mdbModalHeader, mdbModalBody, mdbModalFooter,mdbBadge,mdbBtnGroup, mdbDropdown, mdbDropdownMenu, mdbDropdownItem,} from 'mdbvue'

import loaderTable from '../../components/loaderTable.vue';
import payNewOrder from './update_accept/payNew_Accept.vue'
import payNewUpdate from './update_accept/payNew_Update.vue'
import { mapActions, mapGetters } from 'vuex';
export default {
  components: { loaderTable, mdbBtn, mdbInput, payNewOrder, mdbIcon, payNewUpdate,
  mdbModal, mdbModalHeader, mdbModalBody, mdbModalFooter,mdbBadge,mdbBtnGroup, mdbDropdown, 
  mdbDropdownMenu, mdbDropdownItem, },
  data() {
    return {
      id: 0,
      loading:false,
      pay_show: false,
      update_show:false,
      delete_show: false,

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
    async fetchTodayOrderList(){
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
    async fetchTomorrowOrderList(){
      let nowDate = new Date();
      let next_day = nowDate.setDate(nowDate.getDate() + 1);
      var next_days_date = new Date(next_day).toISOString();
      this.today_date = next_days_date.slice(0,10);
      this.b_date = this.today_date;
      this.e_date = this.today_date;

      let d_time = {
        b_date: this.b_date  + 'T00:00:01.504Z',
        e_date: this.e_date + 'T23:59:01.504Z',
      }

      await this.fetchDateOrderList(d_time);
    },
    async fetchAllOrderList(){
      this.show_order_list = true;
      await this.fetchOrder_list();
      this.order_list = this.allOrder_list;
      this.cach_order_list = this.allOrder_list;
      this.all_water_count = 0;
      console.log(this.allOrder_list);
      for(let i=0; i<this.allOrder_list.length; i++){
        this.all_water_count += this.allOrder_list[i].water_count;
      }
    },
    async fetchDateOrderList(date){
      this.show_order_list = false;
      try{
        this.loading = true;
        const res = await fetch(this.$store.state.hostname + '/WaterOrders/getPaginationBeatweanDateWithoutTimeOpenOrdersList?page=0&size=1000&begin_date='+ date.b_date+'&end_date=' + date.e_date);
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
    showOrder(data){
      console.log(data)
      this.pay_show = true;
      this.order_id = data.id;
      this.$refs.payNew.fetchMounted(data.id);
    },
   async delete_from_user(data)
    {
      console.log(data)
      try{
        const res = await fetch(this.$store.state.hostname + '/WaterOrders/addDeOrderToUsersWithAuthidRemoveCar?id='+ data.id);

        
        if(res.status == 200 || res.status == 201){
          data.deleivered_user_auth_id = '';
          this.$refs.message.success('Added_successfully');
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
    UpdateOrder(data){
      this.update_show = true;
      this.order_id = data.id;
      this.$refs.updateOrder.fetchMounted(data.id);
    },
    async closeAcceptOrder(){
      this.pay_show = false;
      if(this.show_order_list){
        await this.fetchAllOrderList();
      }
      else{
        await this.acceptBtn();
      }
    },
     async closeUpdateOrder(){
      this.update_show = false;
      if(this.show_order_list){
        await this.fetchAllOrderList();
      }
      else{
        await this.acceptBtn();
      }
    },
    async closeUpdate(){
      this.pay_show = false;
      if(this.show_order_list){
        await this.fetchAllOrderList();
      }
      else{
        await this.acceptBtn();
      }
    },
     // ===> sort table <===

    deleteOrder(data){
      this.delete_show = true;
      this.order_id = data.id;
    },  
    async promise(){
      const requestOptions = {
        method : "delete",
      };
      try{
        const response = await fetch(this.$store.state.hostname + "/WaterOrders/" + this.order_id, requestOptions);
        const data = await response.text();
        if(response.status == 201 || response.status == 200)
        {
          this.$refs.message.success('Successfully_removed')
          if(this.show_order_list){
            await this.fetchAllOrderList();
          }
          else{
            await this.acceptBtn();
          }
          this.delete_show = false;
        }
        else{
          this.modal_info = data;
          this.modal_status = true;
          this.delete_show = false;
        }
      }
      catch{
        this.$refs.message.error('network_ne_connect')
      }
      
    },
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
  padding: 8px 16px;
  
  @media (max-width: 991px) {
    padding: 8px 12px;
  }
  
  .header-content {
    display: flex;
    align-items: center;
    gap: 12px;
    flex-wrap: wrap;
  }
  
  .search-section {
    flex: 0 0 auto;
    min-width: 200px;
    
    .search-wrapper {
      display: flex;
      gap: 8px;
      align-items: center;
      
      input-icon {
        width: 250px !important;
        flex: 0 0 auto;
        border-radius: 8px;
      }
      
      .search-btn {
        font-size: 10px !important;
        padding: 3px 12px !important;
        border-radius: 8px;
        font-weight: 500;
        // letter-spacing: -0.01em; 
        height: 28px !important;
        white-space: nowrap;
        transition: all 0.2s ease;
        
        &:hover {
          transform: translateY(-1px);
          box-shadow: 0 2px 8px rgba(16, 185, 129, 0.15);
        }
      }
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
    
    .action-btn {
      font-size: 10px !important;
      padding: 3px 18px !important;
      border-radius: 8px;
      font-weight: 500;
      letter-spacing: -0.01em;
      height: 28px !important;
      white-space: nowrap;
      transition: all 0.2s ease;
      
      &:hover {
        transform: translateY(-1px);
        box-shadow: 0 2px 8px rgba(16, 185, 129, 0.15);
      }
    }
  }
  
  @media (max-width: 768px) {
    .header-content {
      flex-direction: column;
      align-items: stretch;
    }
    
    .search-section,
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
      
      // Color value backgrounds
      &.bg_dark_tr {
        background: #6b7280 !important;
        color: white !important;
        
        td {
          color: white !important;
        }
      }
      
      &.bg_red_tr {
        background: #10b981 !important;
        color: white !important;
        
        td {
          color: white !important;
        }
      }
      
      &.bg_reded_tr {
        background: #ef4444 !important;
        color: white !important;
        
        td {
          color: white !important;
        }
      }
      
      // Action icons
      .mdb-icon {
        transition: all 0.2s ease;
        
        &:hover {
          transform: scale(1.1);
        }
      }
    }
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

// Legacy class support
.all_info_order {
  min-height: 100vh;
  background: #f8fafb;
  display: flex;
}

.order_new_list {
  flex: 1;
  display: flex;
  flex-direction: column;
  overflow: hidden;
}

.order_info {
  width: 200px;
  background: #ffffff;
  border-left: 1px solid #e5e7eb;
  padding: 16px;
  
  .borderSolder {
    background: linear-gradient(135deg, #ecfdf5 0%, #d1fae5 100%);
    border: 1px solid #a7f3d0;
    border-radius: 12px;
    padding: 20px;
    box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
    
    span {
      color: #065f46;
      font-size: 13px;
      font-weight: 600;
      letter-spacing: -0.01em;
    }
    
    p {
      color: #047857;
      font-weight: 700;
      font-size: 28px;
      margin: 12px 0 0 0;
      padding: 0;
      letter-spacing: -0.02em;
    }
  }
}

.header_table {
  background: #10b981;
  color: white;
  
  th {
    padding: 10px 12px;
    font-weight: 600;
    font-size: 11px;
    letter-spacing: -0.01em;
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

.body_table {
  tr {
    border-bottom: 1px solid #f3f4f6;
    
    &:nth-child(even) {
      background-color: #fafbfc;
    }
    
    td {
      padding: 10px 12px;
      font-size: 11px;
      color: #374151;
      letter-spacing: -0.01em;
    }
  }
}

.table {
  padding: 16px;
  background: #f8fafb;
}

.tabled {
  border-collapse: separate;
  border-spacing: 0;
}

.bg_dark_tr {
  background: #6b7280 !important;
  color: white !important;
}

.bg_red_tr {
  background: #10b981 !important;
  color: white !important;
}

.bg_reded_tr {
  background: #ef4444 !important;
  color: white !important;
}
</style>