<template>
  <div class="delivery-app">
    <!-- Desktop Header -->
    <div class="desktop-header">
      <div class="desktop-app-header">
        <div class="desktop-header-top">
          <div class="desktop-user-info">
            <h2 class="desktop-user-name">{{user_name}}</h2>
            <button class="desktop-logout-btn" @click="$router.push('/')">
              <i class="fas fa-sign-out-alt"></i>
              {{$t('logout')}}
            </button>
          </div>
        </div>
        
        <!-- Desktop Stats Cards -->
        <div class="desktop-stats-container">
          <div class="desktop-stat-card">
            <div class="desktop-stat-label">{{$t('order')}}</div>
            <div class="desktop-stat-value">{{all_water_count}}</div>
          </div>
          <div class="desktop-stat-card">
            <div class="desktop-stat-label">Tarqatildi</div>
            <div class="desktop-stat-value">{{all_summ.tarqatildi.toString().replace(/(\d)(?=(\d{3})+(\.(\d){0,2})*$)/g, '$1 ')}}</div>
          </div>
          <div class="desktop-stat-card">
            <div class="desktop-stat-label">Vozvrat</div>
            <div class="desktop-stat-value">{{all_summ.vozvrat.toString().replace(/(\d)(?=(\d{3})+(\.(\d){0,2})*$)/g, '$1 ')}}</div>
          </div>
          <div class="desktop-stat-card">
            <div class="desktop-stat-label">{{$t('cash')}}</div>
            <div class="desktop-stat-value">{{all_summ.cash.toString().replace(/(\d)(?=(\d{3})+(\.(\d){0,2})*$)/g, '$1 ')}}</div>
          </div>
          <div class="desktop-stat-card">
            <div class="desktop-stat-label">{{$t('card')}}</div>
            <div class="desktop-stat-value">{{all_summ.card.toString().replace(/(\d)(?=(\d{3})+(\.(\d){0,2})*$)/g, '$1 ')}}</div>
          </div>
          <div class="desktop-stat-card desktop-highlight">
            <div class="desktop-stat-label">{{$t('summ')}}</div>
            <div class="desktop-stat-value">{{all_summ.summ.toString().replace(/(\d)(?=(\d{3})+(\.(\d){0,2})*$)/g, '$1 ')}}</div>
          </div>
        </div>
      </div>
    </div>

    <!-- Mobile Header -->
    <div class="mobile-header">
      <div class="app-header">
        <div class="header-top">
          <div class="user-info">
            <h3 class="user-name">{{user_name}}</h3>
            <button class="logout-btn" @click="$router.push('/')">
              <i class="fas fa-sign-out-alt"></i>
            </button>
          </div>
        </div>
        
        <!-- Stats Cards - All Statistics -->
        <div class="stats-container">
          <div class="stat-card">
            <div class="stat-label">{{$t('order')}}</div>
            <div class="stat-value">{{all_water_count}}</div>
          </div>
          <div class="stat-card">
            <div class="stat-label">Tarqatildi</div>
            <div class="stat-value">{{all_summ.tarqatildi.toString().replace(/(\d)(?=(\d{3})+(\.(\d){0,2})*$)/g, '$1 ')}}</div>
          </div>
          <div class="stat-card">
            <div class="stat-label">Vozvrat</div>
            <div class="stat-value">{{all_summ.vozvrat.toString().replace(/(\d)(?=(\d{3})+(\.(\d){0,2})*$)/g, '$1 ')}}</div>
          </div>
          <div class="stat-card">
            <div class="stat-label">{{$t('cash')}}</div>
            <div class="stat-value">{{all_summ.cash.toString().replace(/(\d)(?=(\d{3})+(\.(\d){0,2})*$)/g, '$1 ')}}</div>
          </div>
          <div class="stat-card">
            <div class="stat-label">{{$t('card')}}</div>
            <div class="stat-value">{{all_summ.card.toString().replace(/(\d)(?=(\d{3})+(\.(\d){0,2})*$)/g, '$1 ')}}</div>
          </div>
          <div class="stat-card highlight">
            <div class="stat-label">{{$t('summ')}}</div>
            <div class="stat-value">{{all_summ.summ.toString().replace(/(\d)(?=(\d{3})+(\.(\d){0,2})*$)/g, '$1 ')}}</div>
          </div>
        </div>
      </div>
    </div>

    <!-- Desktop Content -->
    <div class="desktop-content">
      <!-- Desktop Search and Filter Bar -->
      <div class="desktop-search-filter-bar">
        <div class="desktop-search-box">
          <i class="fas fa-search desktop-search-icon"></i>
          <input 
            type="text" 
            v-model="search" 
            @input="searchClick" 
            :placeholder="$t('search_here')"
            class="desktop-search-input"
          />
        </div>
        <button class="desktop-map-btn" @click="$router.push('/postavchik_map')">
          <i class="fas fa-map-marked-alt"></i>
          {{$t('map')}}
        </button>
      </div>

      <!-- Desktop Filter Tabs -->
      <div class="desktop-filter-tabs">
        <button 
          class="desktop-filter-tab" 
          :class="{ active: filterStatus === 'pending' }"
          @click="setFilterStatus('pending')"
        >
          Pending
        </button>
        <button 
          class="desktop-filter-tab" 
          :class="{ active: filterStatus === 'complete' }"
          @click="setFilterStatus('complete')"
        >
          Complete
        </button>
      </div>

      <!-- Desktop Orders Grid -->
      <div class="desktop-orders-container">
        <loader-table v-if="loading" />
        <div v-else class="desktop-orders-grid">
          <div 
            v-for="(item, index) in filteredOrderList" 
            :key="index" 
            class="desktop-order-card"
            :class="{
              'desktop-order-pending': item.color_value === 'green',
              'desktop-order-overdue': item.color_value === 'black',
              'desktop-order-complete': item.accepted_status
            }"
          >
            <div class="desktop-order-card-header">
              <div class="desktop-order-id">#{{item.id}}</div>
              <div class="desktop-order-status" :class="getStatusClass(item)">
                {{getStatusText(item)}}
              </div>
            </div>
            
            <div class="desktop-order-content">
              <div class="desktop-order-client">
                <div class="desktop-client-name-wrapper">
                  <h4 class="desktop-client-name">
                    {{item.client_name_str}}
                    <mdb-icon v-if="item.note" icon="star" color="orange" class="ml-1" />
                  </h4>
                  <button 
                    class="desktop-info-btn" 
                    @click="showClientInfo(item.client.id)"
                    title="Client ma'lumotlari"
                  >
                    <i class="fas fa-info-circle"></i>
                  </button>
                </div>
                <div class="desktop-order-address" v-if="item.address">
                  <i class="fas fa-map-marker-alt"></i>
                  {{item.address.address}}
                </div>
              </div>

              <div class="desktop-order-details">
                <div class="desktop-detail-item">
                  <span class="desktop-detail-label">{{$t('qty')}}:</span>
                  <span class="desktop-detail-value desktop-highlight-qty">{{item.water_count}}</span>
                </div>
                <div class="desktop-detail-item" v-if="item.name_pp">
                  <span class="desktop-detail-label">{{$t('product')}}:</span>
                  <span class="desktop-detail-value">{{item.name_pp}}</span>
                </div>
                <div class="desktop-detail-item">
                  <span class="desktop-detail-label">{{$t('date')}}:</span>
                  <span class="desktop-detail-value">
                    {{formatDate(item.order_date)}}
                    <a 
                      v-for="(list_item,indx) in item.client.phone_numbers_list" 
                      :key="indx" 
                      :href="`tel:${list_item.phone_number}`"
                      class="desktop-phone-number"
                    >
                      <i class="fas fa-phone"></i> {{formatPhone(list_item.phone_number)}}
                    </a>
                  </span>
                </div>
                <div class="desktop-detail-item" v-if="item.note">
                  <span class="desktop-detail-label">{{$t('note')}}:</span>
                  <span class="desktop-detail-value">{{item.note}}</span>
                </div>
              </div>

              <!-- Desktop Order Items List -->
              <div v-if="item.items && item.items.length > 0" class="desktop-order-items-list">
                <div class="desktop-items-header">
                  <i class="fas fa-box"></i>
                  <span>Mahsulotlar:</span>
                </div>
                <div class="desktop-items-container">
                  <div 
                    v-for="(orderItem, idx) in item.items" 
                    :key="idx" 
                    class="desktop-order-item-badge"
                  >
                    <span class="desktop-item-name">{{orderItem.product ? orderItem.product.name : 'N/A'}}</span>
                    <span class="desktop-item-qty">{{orderItem.qty}}</span>
                  </div>
                </div>
              </div>
            </div>

            <div class="desktop-order-actions">
              <button 
                v-if="!item.accepted_status" 
                class="desktop-action-btn desktop-primary-btn" 
                @click="showOrder(item)"
              >
                <i class="fas fa-check-circle"></i>
                {{$t('accept')}}
              </button>
              <button class="desktop-action-btn desktop-secondary-btn" @click="openYandex(item)">
                <i class="fas fa-map-marked-alt"></i>
                Поехали
              </button>
            </div>
          </div>

          <div v-if="filteredOrderList.length === 0" class="desktop-empty-state">
            <i class="fas fa-inbox"></i>
            <p>Zakazlar topilmadi</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Mobile Content -->
    <div class="mobile-content">
      <!-- Search and Filter Bar -->
      <div class="search-filter-bar">
        <div class="search-box">
          <i class="fas fa-search search-icon"></i>
          <input 
            type="text" 
            v-model="search" 
            @input="searchClick" 
            :placeholder="$t('search_here')"
            class="search-input"
          />
        </div>
        <button class="map-btn" @click="$router.push('/postavchik_map')">
          <i class="fas fa-map-marked-alt"></i>
          <span class="map-btn-text">{{$t('map')}}</span>
        </button>
      </div>

      <!-- Filter Tabs -->
      <div class="filter-tabs">
        <button 
          class="filter-tab" 
          :class="{ active: filterStatus === 'pending' }"
          @click="setFilterStatus('pending')"
        >
          Pending
        </button>
        <button 
          class="filter-tab" 
          :class="{ active: filterStatus === 'complete' }"
          @click="setFilterStatus('complete')"
        >
          Complete
        </button>
      </div>

      <!-- Orders List -->
      <div class="orders-container">
        <loader-table v-if="loading" />
        <div v-else class="orders-list">
          <div 
            v-for="(item, index) in filteredOrderList" :style="{background: item.reserverd_note_3}"
            :key="index" 
            class="order-card"
            :class="{
              'order-pending': item.color_value === 'green',
              'order-overdue': item.color_value === 'black',
              'order-complete': item.accepted_status
            }"
          >
            <div class="order-card-header">
              <div class="order-id">№{{item.id}}</div>
              <div class="order-status" :class="getStatusClass(item)">
                {{getStatusText(item)}}
              </div>
            </div>
            
            <div class="order-content">
              <div class="order-client">
                <div class="client-name-wrapper">
                  <h4 class="client-name">
                    {{item.client_name_str}}
                    <mdb-icon v-if="item.note" icon="star" color="orange" class="ml-1" />
                  </h4>
                  <button 
                    class="info-btn" 
                    @click="showClientInfo(item.client.id)"
                    title="Client ma'lumotlari"
                  >
                    <i class="fas fa-info-circle"></i>
                  </button>
                </div>
                <div class="order-address" v-if="item.address">
                  <i class="fas fa-map-marker-alt"></i>
                  {{item.address.address}}
                </div>
              </div>

              <div class="order-details">
                <div class="detail-item">
                  <span class="detail-label">{{$t('qty')}}:</span>
                  <span class="detail-value highlight-qty">{{item.water_count}}</span>
                </div>
                <div class="detail-item" v-if="item.name_pp">
                  <span class="detail-label">{{$t('product')}}:</span>
                  <span class="detail-value">{{item.name_pp}}</span>
                </div>
                <div class="detail-item">
                  <span class="detail-label">{{$t('date')}}:</span>
                  <span class="desktop-detail-value ">
                    {{formatDate(item.order_date)}}
                    <a 
                      v-for="(list_item,indx) in item.client.phone_numbers_list" 
                      :key="indx" 
                      :href="`tel:${list_item.phone_number}`"
                      class="desktop-phone-number ml-3"
                    >
                      <i class="fas fa-phone"></i> {{formatPhone(list_item.phone_number)}}
                    </a>
                  </span>
                </div>
                <div class="detail-item" v-if="item.note">
                  <span class="detail-label">{{$t('note')}}:</span>
                  <span class="detail-value">{{item.note}}</span>
                </div>
              </div>

              <!-- Order Items List -->
              <div v-if="item.items && item.items.length > 0" class="order-items-list">
                <div class="items-header">
                  <i class="fas fa-box"></i>
                  <span>Mahsulotlar:</span>
                </div>
                <div class="items-container">
                  <div 
                    v-for="(orderItem, idx) in item.items" 
                    :key="idx" 
                    class="order-item-badge"
                  >
                    <span class="item-name">{{orderItem.product ? orderItem.product.name : 'N/A'}}</span>
                    <span class="item-qty">{{orderItem.qty}}</span>
                  </div>
                </div>
              </div>
            </div>

            <div class="order-actions">
              <button 
                v-if="!item.accepted_status" 
                class="action-btn primary-btn" 
                @click="showOrder(item)"
              >
                <i class="fas fa-check-circle"></i>
                {{$t('accept')}}
              </button>
              <button class="action-btn secondary-btn" @click="openYandex(item)">
                <i class="fas fa-map-marked-alt"></i>
                Поехали
              </button>
            </div>
          </div>

          <div v-if="filteredOrderList.length === 0" class="empty-state">
            <i class="fas fa-inbox"></i>
            <p>Zakazlar topilmadi</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Payment Modal -->
    <modal-train  
      :show="pay_show" 
      headerbackColor="white"  
      titlecolor="black" 
      :title="$t('pay')" 
      @close="pay_show = false" 
      width="100%"
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

    <!-- Client Info Modal -->
    <modal-train  
      :show="client_info_show" 
      headerbackColor="white"  
      titlecolor="black" 
      title="Client zakazlari tarixi" 
      @close="client_info_show = false" 
      width="80%"
    >
      <template v-slot:body>
        <div class="client-info-modal">
          <loader-table v-if="client_info_loading" />
          <div v-else class="client-orders-list">
            <div v-if="client_orders_list.length === 0" class="empty-orders">
              <i class="fas fa-inbox"></i>
              <p>Zakazlar topilmadi</p>
            </div>
            <div 
              v-for="(order, index) in client_orders_list" 
              :key="index" 
              class="client-order-item"
            >
              <div class="client-order-header">
                <div class="client-order-id">Zakaz #{{order.id}}</div>
                <div class="client-order-date">{{formatDate(order.order_date)}}</div>
              </div>
              <div class="client-order-details">
                <div class="client-order-detail-row">
                  <span class="detail-label">Miqdor:</span>
                  <span class="detail-value">{{order.water_count}}</span>
                </div>
                <div class="client-order-detail-row" v-if="order.name_pp">
                  <span class="detail-label">Mahsulot:</span>
                  <span class="detail-value">{{order.name_pp}}</span>
                </div>
                <div class="client-order-detail-row" v-if="order.address">
                  <span class="detail-label">Manzil:</span>
                  <span class="detail-value">{{order.address.address}}</span>
                </div>
                <div class="client-order-detail-row" v-if="order.note">
                  <span class="detail-label">Izoh:</span>
                  <span class="detail-value">{{order.note}}</span>
                </div>
                <div class="client-order-detail-row" v-if="order.accepted_status">
                  <span class="detail-label">Holat:</span>
                  <span class="detail-value status-complete">Bajarilgan</span>
                </div>
              </div>
            </div>
          </div>
        </div>
      </template>
    </modal-train>
    
    <Toast ref="message"></Toast>
  </div>
</template>

<script>
import {mdbBtn, mdbInput, mdbIcon, 
    mdbContainer,
    mdbNavbar,
    mdbNavbarBrand,
    mdbNavbarToggler,
    mdbNavbarNav,
    mdbNavItem
} from 'mdbvue'

import loaderTable from '../../components/loaderTable.vue';
import payNewOrder from '../order/update_accept/payNew_Accept.vue'
import { mapActions, mapGetters } from 'vuex';
export default {
  components: { loaderTable, mdbBtn, mdbInput, payNewOrder, mdbIcon,
    mdbContainer,
    mdbNavbar,
    mdbNavbarBrand,
    mdbNavbarToggler,
    mdbNavbarNav,
    mdbNavItem 
  },
  data() {
    return {
      id: 0,
      loading:false,
      pay_show: false,

      order_list:  [],
      cach_order_list: [],
      complete_order_list: [],
      cach_complete_order_list: [],
      order_id: null,

      all_water_count: 0,
      all_water_sum: 0,
      water_price: 0,
      all_summ: {
        cash: 0,
        tarqatildi : 0,
        vozvrat : 0,
        card: 0,
        online:0,
        rasxod: 0,
        summ: 0,
      },
      pos_money_report_list: [],
      today_date: '',
      b_date : '',
      e_date : '',

      search: '',
      user_name: localStorage.UserName,
      filterStatus: 'pending', // pending, complete
      
      // Client info modal
      client_info_show: false,
      client_info_loading: false,
      client_orders_list: [],
    }
  },
  async mounted() {
    let date = new Date();
    this.today_date = date.toISOString().slice(0,10);
    this.b_date = this.today_date + 'T00:00:01.504Z';
    this.e_date = this.today_date + 'T23:59:01.504Z'; 
    await this.updateList();
    await this.fetchPosMoneyReport();
    
  },
  computed: {
    ...mapGetters(['allOrder_list', 'get_postavchik_order_list']),
    filteredOrderList() {
      if (this.filterStatus === 'complete') {
        return this.complete_order_list;
      } else {
        // Pending - filter from order_list
        let filtered = this.order_list;
        if (this.search) {
          // Search is already applied in searchClick
          return filtered;
        }
        return filtered;
      }
    }
  },
  methods: {
  ...mapActions(['fetchOrder_list', 'fetchPostavchikOrder']),
    searchClick(){
      this.loading = true;
      if(this.filterStatus === 'complete'){
        // Search in complete orders
        this.complete_order_list = this.cach_complete_order_list;
        if(this.search != ''){
          let userSearchList = [];
          for(let i=0; i<this.complete_order_list.length;i++){
            if((this.complete_order_list[i].client_name_str && this.complete_order_list[i].client_name_str.toLowerCase().includes(this.search.toLowerCase())) || 
               (this.complete_order_list[i].address && this.complete_order_list[i].address.address && this.complete_order_list[i].address.address.toLowerCase().includes(this.search.toLowerCase()))){
              userSearchList.push(this.complete_order_list[i])
            }
          }
          this.complete_order_list = userSearchList;
        }
        else{
          this.complete_order_list = this.cach_complete_order_list;
        }
      }
      else{
        // Search in pending orders
        this.order_list = this.cach_order_list;
        if(this.search != ''){
          let userSearchList = [];
          for(let i=0; i<this.order_list.length;i++){
            if((this.order_list[i].client_name_str && this.order_list[i].client_name_str.toLowerCase().includes(this.search.toLowerCase())) || 
               (this.order_list[i].address && this.order_list[i].address.address && this.order_list[i].address.address.toLowerCase().includes(this.search.toLowerCase()))){
              userSearchList.push(this.order_list[i])
            }
          }
          this.order_list = userSearchList;
        }
        else{
          this.order_list = this.cach_order_list;
        }
      }
      this.loading = false;
    },
    async fetchPosMoneyReport(){
     let auth_id = {
        auth_id: localStorage.AuthId
      }
      try{
        this.loading = true;
        const res = await fetch(this.$store.state.hostname + '/WaterChecks/getPaginationByAuthIdByDateTime?page=0&size=1000&auth_id='
         + auth_id.auth_id + '&begin_date=' + this.b_date + '&end_date=' + this.e_date);
        const data = await res.json();
        this.loading = false;
        if(res.status == 200 || res.status == 201){
          console.log('data.items_list')
          console.log(data)
          this.pos_money_report_list = data.items_list;
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
            this.all_summ.cash += parseFloat(this.pos_money_report_list[i].cash || 0)
            this.all_summ.tarqatildi += parseFloat(this.pos_money_report_list[i].reserverd_number_id_2 || 0)
            this.all_summ.vozvrat += parseFloat(this.pos_money_report_list[i].reserverd_number_id_1 || 0)
            this.all_summ.card += parseFloat(this.pos_money_report_list[i].card || 0)
            this.all_summ.online += parseFloat(this.pos_money_report_list[i].online || 0)
            this.all_summ.rasxod += parseFloat(this.pos_money_report_list[i].rasxod || 0)
            this.all_summ.summ += parseFloat(this.pos_money_report_list[i].summ || 0)
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
    
    showOrder(data){
      this.pay_show = true;
      this.order_id = data.id;
      this.$refs.payNew.fetchMounted(data.id);
    },
    openYandex(data){
      console.log(data)
      if (data.address && data.address.latidu && data.address.longitu) {
        const url = `https://yandex.ru/maps/?rtext=~${encodeURIComponent(data.address.latidu + ',' + data.address.longitu)}&rtt=auto`;
        window.open(url, '_blank', 'noopener');
      }
    },

    async updateList(){
      let auth_id = {
        auth_id: localStorage.AuthId
      }
       console.log(auth_id)
      await this.fetchPostavchikOrder(auth_id);
      this.order_list = this.get_postavchik_order_list;
      this.cach_order_list = this.get_postavchik_order_list;
      this.all_water_count = 0;
      for(let i=0; i<this.order_list.length; i++){
        this.all_water_count += this.order_list[i].water_count;
      }
    },

    async fetchCompleteOrders(){
      try {
        this.loading = true;
        const auth_id = localStorage.AuthId;
        const res = await fetch(this.$store.state.hostname + '/WaterOrders/getTodayCompletedOrdersByAuthId?id_auth=' + auth_id);
        const data = await res.json();
        this.loading = false;
        if(res.status == 200 || res.status == 201){
          this.complete_order_list = data;
          this.cach_complete_order_list = data;
        }
        else{
          this.complete_order_list = [];
          this.cach_complete_order_list = [];
        }
      }
      catch{
        this.$refs.message.error('network_ne_connect');
        this.complete_order_list = [];
        this.cach_complete_order_list = [];
        this.loading = false;
      }
    },

    async setFilterStatus(status){
      this.filterStatus = status;
      if(status === 'complete'){
        await this.fetchCompleteOrders();
      }
    },

    async closeAcceptOrder(){
      this.pay_show = false;
      await this.updateList();
      await this.fetchPosMoneyReport();
      // If viewing complete orders, refresh them
      if(this.filterStatus === 'complete'){
        await this.fetchCompleteOrders();
      }
    },
    async closeUpdate(){
      this.pay_show = false;
      await this.updateList();
      await this.fetchPosMoneyReport();
      // If viewing complete orders, refresh them
      if(this.filterStatus === 'complete'){
        await this.fetchCompleteOrders();
      }
    },
    
    formatDate(dateString) {
      if (!dateString) return '';
      const date = new Date(dateString);
      return date.toLocaleDateString('ru-RU', { 
        day: '2-digit', 
        month: 'short', 
        year: 'numeric' 
      });
    },
    
    formatPhone(phone) {
      if (!phone) return ''
      // Telefon raqamini formatlash: 99 777 22 47
      const cleaned = phone.replace(/\D/g, '')
      
      // Agar +998 yoki 998 bilan boshlansa, uni olib tashlash
      let digits = cleaned
      if (digits.startsWith('998')) {
        digits = digits.substring(3)
      } else if (digits.startsWith('+998')) {
        digits = digits.substring(4)
      }
      
      // 9 raqamli bo'lsa: 99 777 22 47 formatida
      if (digits.length === 9) {
        const match = digits.match(/^(\d{2})(\d{3})(\d{2})(\d{2})$/)
        if (match) {
          return `${match[1]} ${match[2]} ${match[3]} ${match[4]}`
        }
      }
      
      // Agar formatlash mumkin bo'lmasa, asl raqamni qaytarish
      return phone
    },
    
    getStatusClass(item) {
      if (item.accepted_status) {
        return 'status-complete';
      } else if (item.color_value === 'green') {
        return 'status-pending';
      } else if (item.color_value === 'black') {
        return 'status-overdue';
      }
      return 'status-pending';
    },
    
    getStatusText(item) {
      if (item.accepted_status) {
        return 'Complete';
      } else if (item.color_value === 'green') {
        return 'Pending';
      } else if (item.color_value === 'black') {
        return 'Overdue';
      }
      return 'Pending';
    },

    async showClientInfo(clientId) {
      if (!clientId) {
        this.$refs.message.warning('Client ID topilmadi');
        return;
      }
      
      this.client_info_show = true;
      this.client_info_loading = true;
      this.client_orders_list = [];
      
      try {
        const res = await fetch(
          this.$store.state.hostname + 
          '/WaterOrders/getPaginationOrderByClientId?page=0&size=200&client_id=' + 
          clientId
        );
        const data = await res.json();
        this.client_info_loading = false;
        
        if (res.status == 200 || res.status == 201) {
          this.client_orders_list = Array.isArray(data.items_list) ? data.items_list : [];
        } else {
          this.$refs.message.error('Ma\'lumotlarni yuklashda xatolik');
          this.client_orders_list = [];
        }
      } catch (error) {
        console.error('Error fetching client orders:', error);
        this.$refs.message.error('network_ne_connect');
        this.client_info_loading = false;
        this.client_orders_list = [];
      }
    }

  },
}
</script>

<style lang="scss" scoped>
@import url(https://fonts.googleapis.com/css?family=Open+Sans:400,600,700);

* {
  box-sizing: border-box;
}

.delivery-app {
  min-height: 100vh;
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
  font-family: 'Open Sans', sans-serif;
  padding-bottom: 20px;
}

// Desktop Styles
.desktop-header {
  display: none;
  
  @media (min-width: 768px) {
    display: block;
  }
}

.desktop-app-header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 20px 30px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
  
  .desktop-header-top {
    margin-bottom: 20px;
    
    .desktop-user-info {
      display: flex;
      justify-content: space-between;
      align-items: center;
      
      .desktop-user-name {
        margin: 0;
        font-size: 24px;
        font-weight: 600;
      }
      
      .desktop-logout-btn {
        background: rgba(255,255,255,0.2);
        border: none;
        color: white;
        padding: 10px 20px;
        border-radius: 8px;
        cursor: pointer;
        font-size: 14px;
        font-weight: 600;
        display: flex;
        align-items: center;
        gap: 8px;
        transition: all 0.3s;
        
        &:hover {
          background: rgba(255,255,255,0.3);
        }
      }
    }
  }
  
  .desktop-stats-container {
    display: grid;
    grid-template-columns: repeat(6, 1fr);
    gap: 15px;
    
    .desktop-stat-card {
      background: rgba(255,255,255,0.15);
      backdrop-filter: blur(10px);
      border-radius: 12px;
      padding: 15px;
      text-align: center;
      transition: all 0.3s;
      
      &:hover {
        background: rgba(255,255,255,0.25);
        transform: translateY(-2px);
      }
      
      &.desktop-highlight {
        background: rgba(255,255,255,0.25);
        border: 2px solid rgba(255,255,255,0.5);
      }
      
      .desktop-stat-label {
        font-size: 12px;
        opacity: 0.9;
        margin-bottom: 8px;
        font-weight: 600;
      }
      
      .desktop-stat-value {
        font-size: 20px;
        font-weight: 700;
      }
    }
  }
}

.desktop-content {
  display: none;
  
  @media (min-width: 768px) {
    display: block;
    padding: 20px 30px;
  }
}

// Desktop Search and Filter Bar
.desktop-search-filter-bar {
  display: flex;
  gap: 15px;
  padding: 20px;
  background: white;
  box-shadow: 0 2px 5px rgba(0,0,0,0.05);
  border-radius: 12px;
  margin-bottom: 15px;
  
  .desktop-search-box {
    flex: 1;
    position: relative;
    display: flex;
    align-items: center;
    
    .desktop-search-icon {
      position: absolute;
      left: 15px;
      color: #999;
      font-size: 16px;
    }
    
    .desktop-search-input {
      width: 100%;
      padding: 12px 12px 12px 45px;
      border: 2px solid #e0e0e0;
      border-radius: 25px;
      font-size: 14px;
      outline: none;
      transition: all 0.3s;
      
      &:focus {
        border-color: #667eea;
        box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
      }
    }
  }
  
  .desktop-map-btn {
    background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
    color: white;
    border: none;
    padding: 12px 25px;
    border-radius: 25px;
    font-weight: 600;
    cursor: pointer;
    display: flex;
    align-items: center;
    gap: 8px;
    font-size: 14px;
    transition: all 0.3s;
    white-space: nowrap;
    
    &:hover {
      transform: translateY(-2px);
      box-shadow: 0 5px 15px rgba(245, 87, 108, 0.4);
    }
  }
}

// Desktop Filter Tabs
.desktop-filter-tabs {
  display: flex;
  gap: 10px;
  padding: 15px 20px;
  background: white;
  border-radius: 12px;
  margin-bottom: 20px;
  justify-content: center;
  
  .desktop-filter-tab {
    padding: 10px 30px;
    border: 2px solid #e0e0e0;
    background: white;
    border-radius: 20px;
    font-size: 14px;
    font-weight: 600;
    color: #666;
    cursor: pointer;
    white-space: nowrap;
    transition: all 0.3s;
    min-width: 150px;
    text-align: center;
    
    &:hover {
      border-color: #667eea;
      color: #667eea;
    }
    
    &.active {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      border-color: transparent;
    }
  }
}

// Desktop Orders Container
.desktop-orders-container {
  display: block;
  
  .desktop-orders-grid {
    display: grid;
    grid-template-columns: repeat(auto-fill, minmax(300px, 1fr));
    gap: 20px;
    
    @media (min-width: 1400px) {
      grid-template-columns: repeat(3, 1fr);
    }
    
    @media (min-width: 992px) and (max-width: 1399px) {
      grid-template-columns: repeat(2, 1fr);
    }
    
    @media (min-width: 768px) and (max-width: 991px) {
      grid-template-columns: repeat(2, 1fr);
      gap: 15px;
      min-width: 0;
    }
  }
}

// Desktop Order Card
.desktop-order-card {
  background: white;
  border-radius: 16px;
  padding: 20px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.08);
  transition: all 0.3s;
  border-left: 4px solid #667eea;
  
  &:hover {
    transform: translateY(-3px);
    box-shadow: 0 5px 20px rgba(0,0,0,0.12);
  }
  
  &.desktop-order-pending {
    border-left-color: #ff9800;
  }
  
  &.desktop-order-overdue {
    border-left-color: #f44336;
    background: #fff5f5;
  }
  
  &.desktop-order-complete {
    border-left-color: #4caf50;
    opacity: 0.9;
  }
  
  .desktop-order-card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 15px;
    padding-bottom: 12px;
    border-bottom: 1px solid #f0f0f0;
    
    .desktop-order-id {
      font-size: 16px;
      font-weight: 700;
      color: #667eea;
    }
    
    .desktop-order-status {
      padding: 6px 14px;
      border-radius: 12px;
      font-size: 11px;
      font-weight: 700;
      text-transform: uppercase;
      
      &.status-pending {
        background: #fff3e0;
        color: #ff9800;
      }
      
      &.status-overdue {
        background: #ffebee;
        color: #f44336;
      }
      
      &.status-complete {
        background: #e8f5e9;
        color: #4caf50;
      }
    }
  }
  
  .desktop-order-content {
      .desktop-order-client {
        margin-bottom: 15px;
        
        .desktop-client-name-wrapper {
          display: flex;
          align-items: center;
          justify-content: space-between;
          margin-bottom: 10px;
          
          .desktop-client-name {
            margin: 0;
            font-size: 18px;
            font-weight: 700;
            color: #333;
            display: flex;
            align-items: center;
            flex: 1;
          }
          
          .desktop-info-btn {
            background: transparent;
            border: none;
            color: #667eea;
            font-size: 18px;
            cursor: pointer;
            padding: 4px 8px;
            border-radius: 6px;
            transition: all 0.3s;
            margin-left: 8px;
            
            &:hover {
              background: rgba(102, 126, 234, 0.1);
              transform: scale(1.1);
            }
            
            i {
              font-size: 18px;
            }
          }
        }
        
        .desktop-order-address {
          display: flex;
          align-items: center;
          gap: 8px;
          color: #666;
          font-size: 13px;
          
          i {
            color: #f5576c;
          }
        }
      }
    
    .desktop-order-details {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: 12px;
      margin-bottom: 15px;
      
      .desktop-detail-item {
        display: flex;
        flex-direction: column;
        gap: 4px;
        
        .desktop-detail-label {
          font-size: 11px;
          color: #999;
          font-weight: 600;
          text-transform: uppercase;
        }
        
        .desktop-detail-value {
          font-size: 12px;
          font-weight: 600;
          color: #333;
          display: flex;
          flex-direction: column;
          gap: 4px;
          
          &.desktop-highlight-qty {
            color: #667eea;
            font-size: 20px;
            font-weight: 700;
          }
          
          .desktop-phone-number {
            display: flex;
            align-items: center;
            gap: 6px;
            font-size: 9px;
            color: #667eea;
            font-weight: 500;
            margin-top: 4px;
            text-decoration: none;
            cursor: pointer;
            transition: all 0.3s;
            
            &:hover {
              color: #764ba2;
              text-decoration: underline;
            }
            
            i {
              font-size: 10px;
            }
          }
        }
      }
    }
    
    .desktop-order-items-list {
      margin-top: 15px;
      padding-top: 15px;
      border-top: 1px solid #f0f0f0;
      
      .desktop-items-header {
        display: flex;
        align-items: center;
        gap: 8px;
        margin-bottom: 10px;
        font-size: 12px;
        font-weight: 700;
        color: #667eea;
        text-transform: uppercase;
        
        i {
          font-size: 14px;
        }
      }
      
      .desktop-items-container {
        display: flex;
        flex-wrap: wrap;
        gap: 8px;
        
        .desktop-order-item-badge {
          display: flex;
          align-items: center;
          gap: 8px;
          background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
          padding: 6px 12px;
          border-radius: 20px;
          font-size: 12px;
          border: 1px solid #e0e0e0;
          
          .desktop-item-name {
            font-weight: 600;
            color: #333;
            max-width: 150px;
            overflow: hidden;
            text-overflow: ellipsis;
            white-space: nowrap;
          }
          
          .desktop-item-qty {
            background: #667eea;
            color: white;
            padding: 3px 10px;
            border-radius: 12px;
            font-weight: 700;
            font-size: 11px;
            min-width: 28px;
            text-align: center;
          }
        }
      }
    }
  }
  
  .desktop-order-actions {
    display: flex;
    gap: 10px;
    margin-top: 15px;
    padding-top: 15px;
    border-top: 1px solid #f0f0f0;
    
    .desktop-action-btn {
      flex: 1;
      padding: 12px;
      border: none;
      border-radius: 12px;
      font-size: 14px;
      font-weight: 700;
      cursor: pointer;
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 8px;
      transition: all 0.3s;
      
      i {
        font-size: 16px;
      }
      
      &.desktop-primary-btn {
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        color: white;
        
        &:hover {
          transform: translateY(-2px);
          box-shadow: 0 5px 15px rgba(102, 126, 234, 0.4);
        }
      }
      
      &.desktop-secondary-btn {
        background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
        color: white;
        
        &:hover {
          transform: translateY(-2px);
          box-shadow: 0 5px 15px rgba(245, 87, 108, 0.4);
        }
      }
    }
  }
}

// Desktop Empty State
.desktop-empty-state {
  grid-column: 1 / -1;
  text-align: center;
  padding: 80px 20px;
  color: #999;
  
  i {
    font-size: 80px;
    margin-bottom: 20px;
    opacity: 0.5;
  }
  
  p {
    font-size: 18px;
    margin: 0;
  }
}

// Mobile Styles
.mobile-header {
  display: block;
  
  @media (min-width: 768px) {
    display: none;
  }
}

.mobile-content {
  display: block;
  
  @media (min-width: 768px) {
    display: none;
  }
}

// Mobile Header Section
.app-header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 15px 15px 10px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
  
  .header-top {
    margin-bottom: 15px;
    
    .user-info {
      display: flex;
      justify-content: space-between;
      align-items: center;
      
      .user-name {
        margin: 0;
        font-size: 20px;
        font-weight: 600;
      }
      
      .logout-btn {
        background: rgba(255,255,255,0.2);
        border: none;
        color: white;
        padding: 8px 12px;
        border-radius: 8px;
        cursor: pointer;
        font-size: 16px;
        transition: all 0.3s;
        
        &:hover {
          background: rgba(255,255,255,0.3);
        }
      }
    }
  }
  
  .stats-container {
    display: grid;
    grid-template-columns: repeat(3, 1fr);
    gap: 10px;
    
    .stat-card {
      background: rgba(255,255,255,0.15);
      backdrop-filter: blur(10px);
      border-radius: 12px;
      padding: 12px;
      text-align: center;
      transition: all 0.3s;
      
      &:hover {
        background: rgba(255,255,255,0.25);
        transform: translateY(-2px);
      }
      
      &.highlight {
        background: rgba(255,255,255,0.25);
        border: 2px solid rgba(255,255,255,0.5);
      }
      
      .stat-label {
        font-size: 10px;
        opacity: 0.9;
        margin-bottom: 5px;
        font-weight: 600;
      }
      
      .stat-value {
        font-size: 16px;
        font-weight: 700;
      }
    }
  }
}

// Search and Filter Bar
.search-filter-bar {
  display: flex;
  gap: 8px;
  padding: 10px 12px;
  background: white;
  box-shadow: 0 2px 5px rgba(0,0,0,0.05);
  
  .search-box {
    flex: 1;
    position: relative;
    display: flex;
    align-items: center;
    
    .search-icon {
      position: absolute;
      left: 10px;
      color: #999;
      font-size: 12px;
    }
    
    .search-input {
      width: 100%;
      padding: 8px 8px 8px 32px;
      border: 1.5px solid #e0e0e0;
      border-radius: 20px;
      font-size: 12px;
      outline: none;
      transition: all 0.3s;
      
      &:focus {
        border-color: #667eea;
        box-shadow: 0 0 0 2px rgba(102, 126, 234, 0.1);
      }
    }
  }
  
  .map-btn {
    background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
    color: white;
    border: none;
    padding: 8px 15px;
    border-radius: 20px;
    font-weight: 600;
    cursor: pointer;
    display: flex;
    align-items: center;
    gap: 6px;
    font-size: 12px;
    transition: all 0.3s;
    white-space: nowrap;
    
    &:hover {
      transform: translateY(-2px);
      box-shadow: 0 5px 15px rgba(245, 87, 108, 0.4);
    }
    
    .map-btn-text {
      @media (max-width: 480px) {
        display: none;
      }
    }
  }
}

// Filter Tabs
.filter-tabs {
  display: flex;
  gap: 6px;
  padding: 8px 12px;
  background: white;
  justify-content: center;
  
  .filter-tab {
    flex: 1;
    padding: 6px 12px;
    border: 1.5px solid #e0e0e0;
    background: white;
    border-radius: 16px;
    font-size: 11px;
    font-weight: 600;
    color: #666;
    cursor: pointer;
    white-space: nowrap;
    transition: all 0.3s;
    text-align: center;
    
    &:hover {
      border-color: #667eea;
      color: #667eea;
    }
    
    &.active {
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      color: white;
      border-color: transparent;
    }
  }
}

// Orders Container
.orders-container {
  padding: 12px;
  
  .orders-list {
    display: flex;
    flex-direction: column;
    gap: 10px;
  }
}

// Order Card
.order-card {
  background: white;
  border-radius: 12px;
  padding: 12px;
  box-shadow: 0 2px 8px rgba(0,0,0,0.08);
  transition: all 0.3s;
  border-left: 3px solid #667eea;
  
  &:hover {
    transform: translateY(-2px);
    box-shadow: 0 4px 15px rgba(0,0,0,0.12);
  }
  
  &.order-pending {
    border-left-color: #ff9800;
  }
  
  &.order-overdue {
    border-left-color: #f44336;
    background: #fff5f5;
  }
  
  &.order-complete {
    border-left-color: #4caf50;
    opacity: 0.8;
  }
  
  .order-card-header {
    display: flex;
    justify-content: space-between;
    align-items: center;
    margin-bottom: 10px;
    padding-bottom: 8px;
    border-bottom: 1px solid #f0f0f0;
    
    .order-id {
      font-size: 12px;
      font-weight: 700;
      color: #667eea;
    }
    
    .order-status {
      padding: 4px 10px;
      border-radius: 10px;
      font-size: 10px;
      font-weight: 700;
      text-transform: uppercase;
      
      &.status-pending {
        background: #fff3e0;
        color: #ff9800;
      }
      
      &.status-overdue {
        background: #ffebee;
        color: #f44336;
      }
      
      &.status-complete {
        background: #e8f5e9;
        color: #4caf50;
      }
    }
  }
  
  .order-content {
    .order-client {
      margin-bottom: 10px;
      
      .client-name-wrapper {
        display: flex;
        align-items: center;
        justify-content: space-between;
        margin-bottom: 6px;
        
        .client-name {
          margin: 0;
          font-size: 15px;
          font-weight: 700;
          color: #333;
          display: flex;
          align-items: center;
          flex: 1;
        }
        
        .info-btn {
          background: transparent;
          border: none;
          color: #667eea;
          font-size: 16px;
          cursor: pointer;
          padding: 4px 6px;
          border-radius: 6px;
          transition: all 0.3s;
          margin-left: 6px;
          
          &:hover {
            background: rgba(102, 126, 234, 0.1);
            transform: scale(1.1);
          }
          
          i {
            font-size: 16px;
          }
        }
      }
      
      .order-address {
        display: flex;
        align-items: center;
        gap: 6px;
        color: #666;
        font-size: 11px;
        
        i {
          color: #f5576c;
          font-size: 10px;
        }
      }
    }
    
    .order-details {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: 8px;
      margin-bottom: 10px;
      
      .detail-item {
        display: flex;
        flex-direction: column;
        gap: 3px;
        
        .detail-label {
          font-size: 9px;
          color: #999;
          font-weight: 600;
          text-transform: uppercase;
        }
        
        .detail-value {
          font-size: 11px;
          font-weight: 600;
          color: #333;
          display: flex;
          flex-direction: column;
          gap: 3px;
          
          &.highlight-qty {
            color: #667eea;
            font-size: 15px;
            font-weight: 700;
          }
          
          .phone-number {
            display: flex;
            align-items: center;
            gap: 4px;
            font-size: 9px;
            color: #667eea;
            font-weight: 500;
            margin-top: 3px;
            
            i {
              font-size: 9px;
            }
          }
        }
      }
    }
    
    .order-items-list {
      margin-top: 10px;
      padding-top: 10px;
      border-top: 1px solid #f0f0f0;
      
      .items-header {
        display: flex;
        align-items: center;
        gap: 6px;
        margin-bottom: 8px;
        font-size: 10px;
        font-weight: 700;
        color: #667eea;
        text-transform: uppercase;
        
        i {
          font-size: 11px;
        }
      }
      
      .items-container {
        display: flex;
        flex-wrap: wrap;
        gap: 6px;
        
        .order-item-badge {
          display: flex;
          align-items: center;
          gap: 5px;
          background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
          padding: 4px 10px;
          border-radius: 16px;
          font-size: 10px;
          border: 1px solid #e0e0e0;
          
          .item-name {
            font-weight: 600;
            color: #333;
            max-width: 100px;
            overflow: hidden;
            text-overflow: ellipsis;
            white-space: nowrap;
          }
          
          .item-qty {
            background: #667eea;
            color: white;
            padding: 2px 6px;
            border-radius: 10px;
            font-weight: 700;
            font-size: 9px;
            min-width: 20px;
            text-align: center;
          }
        }
      }
    }
  }
  
  .order-actions {
    display: flex;
    gap: 8px;
    margin-top: 10px;
    padding-top: 10px;
    border-top: 1px solid #f0f0f0;
    
    .action-btn {
      flex: 1;
      padding: 10px;
      border: none;
      border-radius: 10px;
      font-size: 12px;
      font-weight: 700;
      cursor: pointer;
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 6px;
      transition: all 0.3s;
      
      i {
        font-size: 14px;
      }
      
      &.primary-btn {
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
        color: white;
        
        &:hover {
          transform: translateY(-2px);
          box-shadow: 0 5px 15px rgba(102, 126, 234, 0.4);
        }
      }
      
      &.secondary-btn {
        background: linear-gradient(135deg, #f093fb 0%, #f5576c 100%);
        color: white;
        
        &:hover {
          transform: translateY(-2px);
          box-shadow: 0 5px 15px rgba(245, 87, 108, 0.4);
        }
      }
    }
  }
}

// Empty State
.empty-state {
  text-align: center;
  padding: 60px 20px;
  color: #999;
  
  i {
    font-size: 64px;
    margin-bottom: 20px;
    opacity: 0.5;
  }
  
  p {
    font-size: 16px;
    margin: 0;
  }
}

// Responsive Design for Mobile
@media (max-width: 480px) {
  .app-header {
    .stats-container {
      grid-template-columns: repeat(2, 1fr);
      
      .stat-card {
        .stat-label {
          font-size: 9px;
        }
        
        .stat-value {
          font-size: 14px;
        }
      }
    }
  }
  
  .order-card {
    padding: 10px;
    
    .order-content {
      .order-details {
        grid-template-columns: 1fr;
        gap: 6px;
        margin-bottom: 8px;
      }
      
      .order-items-list {
        margin-top: 8px;
        padding-top: 8px;
        
        .items-header {
          font-size: 9px;
          margin-bottom: 6px;
        }
        
        .items-container {
          gap: 5px;
          
          .order-item-badge {
            font-size: 9px;
            padding: 3px 8px;
            
            .item-name {
              max-width: 90px;
              font-size: 9px;
            }
            
            .item-qty {
              font-size: 8px;
              padding: 1px 5px;
            }
          }
        }
      }
    }
    
    .order-actions {
      flex-direction: column;
      gap: 6px;
      margin-top: 8px;
      padding-top: 8px;
      
      .action-btn {
        width: 100%;
        padding: 8px;
        font-size: 11px;
        
        i {
          font-size: 12px;
        }
      }
    }
  }
}

@media (min-width: 481px) and (max-width: 991px) {
  .app-header {
    .stats-container {
      grid-template-columns: repeat(3, 1fr);
    }
  }
}

// Client Info Modal Styles
.client-info-modal {
  padding: 20px;
  min-height: 200px;
  
  .client-orders-list {
    display: flex;
    flex-direction: column;
    gap: 15px;
  }
  
  .empty-orders {
    text-align: center;
    padding: 60px 20px;
    color: #999;
    
    i {
      font-size: 64px;
      margin-bottom: 20px;
      opacity: 0.5;
    }
    
    p {
      font-size: 16px;
      margin: 0;
    }
  }
  
  .client-order-item {
    background: #f8fafb;
    border-radius: 12px;
    padding: 15px;
    border-left: 4px solid #667eea;
    transition: all 0.3s;
    
    &:hover {
      box-shadow: 0 2px 8px rgba(0,0,0,0.1);
      transform: translateX(2px);
    }
    
    .client-order-header {
      display: flex;
      justify-content: space-between;
      align-items: center;
      margin-bottom: 12px;
      padding-bottom: 10px;
      border-bottom: 1px solid #e0e0e0;
      
      .client-order-id {
        font-size: 16px;
        font-weight: 700;
        color: #667eea;
      }
      
      .client-order-date {
        font-size: 13px;
        color: #666;
        font-weight: 600;
      }
    }
    
    .client-order-details {
      display: grid;
      grid-template-columns: repeat(2, 1fr);
      gap: 10px;
      
      @media (max-width: 768px) {
        grid-template-columns: 1fr;
      }
      
      .client-order-detail-row {
        display: flex;
        flex-direction: column;
        gap: 4px;
        
        .detail-label {
          font-size: 11px;
          color: #999;
          font-weight: 600;
          text-transform: uppercase;
        }
        
        .detail-value {
          font-size: 14px;
          font-weight: 600;
          color: #333;
          
          &.status-complete {
            color: #4caf50;
          }
        }
      }
    }
  }
}
</style>