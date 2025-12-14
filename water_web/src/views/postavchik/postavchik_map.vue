<template>
  <div class="map_order">
    <backRouter />
    <div style="position:fixed; top:50px; left:25px; transform: translate(-50%, 0%); z-index:1111111; cursor:pointer;" 
    class="bg_gradiunt p-1 px-2 rounded" @click="$router.push('/postavchik_list')">
      <mdb-icon icon="angle-double-left" />
    </div>
    <div id="map">
      <loaderTable v-if="loading"/>
      <yandex-map v-if="map_show"
        :coords="[41.311516, 69.283250]"
        zoom="13"
        style="width: 100%; height: 100vh;"
        ref="map"
        @map-was-initialized="mapLoaded"
      >
          <!-- :balloon="{header: 'Z-'+mark.address.id, body: mark.address.address, footer: mark.client_name_str + '. ' + ' Телефон: ' + mark.phone_number_list_arr }" -->
      <div v-for="(mark,i) in get_postavchik_order_list" :key="i" >
        <ymap-marker  
          :markerId="mark.id"
          marker-type="placemark"
          
          :coords="[mark.address.latidu, mark.address.longitu]"
          :hint-content="mark.client.fio"
          @balloonopen="bindListener"
          @balloonclose="unbindListener"
          :icon="{
            imageSize: [43, 55],
            imageOffset: [-22, -55],
            content: mark.client.fio,
            color: mark.reserverd_note_3,
            contentOffset: [-22, -55],
          }"
          :cluster-name="mark.id"
          @click="sendFunc(mark.id, mark)"
      >
        <my-component slot="balloon" :mark="mark" @show-client-info="showClientInfo"></my-component>
      </ymap-marker>
      </div>
    <ymap-marker 
      markerId="3"
      marker-type="circle"
      :coords="[41.234687, 69.263790]"
      circle-radius="16"
      hint-content="Hint content 1"
      :marker-fill="{color: '#000000', opacity: 0.4}"
      :marker-stroke="{color: '#ff0000', width: 5}"
      :balloon="{header: 'EXTREME WATER', body: 'MCHJ', footer: '997772247'}"
    ></ymap-marker>
      </yandex-map>
    </div>
    <massage_box :hide="modal_status" :detail_info="modal_info"
      :m_text="$t('Failed_to_add')" @to_hide_modal="modal_status= false"/>
    <Toast ref="message"></Toast>

    <modal-train  :show="pay_show" headerbackColor="white"  titlecolor="black" :title="$t('pay')" 
      @close="pay_show = false" width="98%">
        <template v-slot:body>
          <payNewOrder ref="payNew" @close="closeAcceptOrder"  @closeUpdate="closeUpdate" :orderId="selectMark_id" :shown="pay_show"></payNewOrder>
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
                  <span class="detail-value">{{order.water_count}} / {{ order.reserverd_numeric_id_1 }}</span>
                </div>
                <div class="client-order-detail-row" v-if="order.name_pp">
                  <span class="detail-label">Mahsulot:</span>
                  <span class="detail-value">{{order.name_pp}}</span>
                </div>
                <div class="client-order-detail-row" v-if="order.address">
                  <span class="detail-label">Manzil:</span>
                  <span class="detail-value">{{order.address.address}}</span>
                </div>
                <div class="client-order-detail-row" v-if="order.deleivered_user_auth">
                  <span class="detail-label">Yetkazib beruvchi:</span>
                  <span class="detail-value">{{order.deleivered_user_auth.user.fio}}</span>
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
  </div>
</template>


<script>
import {mdbIcon} from 'mdbvue'
import { loadYmap } from 'vue-yandex-maps';
import { yandexMap, ymapMarker } from 'vue-yandex-maps'
import { mapActions, mapGetters } from 'vuex';
import myComponent from './ballon.vue'
import payNewOrder from '../order/update_accept/payNew_Accept.vue'
import loaderTable from '../../components/loaderTable.vue';

export default {
  components:{
    mdbIcon,
    yandexMap, ymapMarker,
    myComponent,
    payNewOrder,
    loaderTable
  },
data() {
  return {
    modal_info: '',
    modal_status: false,
    loading: false,
    map_show: false,
    pay_show: false,
    myMap: {},

    order_id: null,

    order_list: [{
      id:1,
      client_name_str: 'test1',
      address: {
        latidu: 41.349307,
        longitu: 69.335900,
        address: 'Toshkent',
        id: 2
      },
    }],
    selectMark_id: null,
    select_mark: {},
    
    // Client info modal
    client_info_show: false,
    client_info_loading: false,
    client_orders_list: [],
  }
},
computed: {
  ...mapGetters(['get_postavchik_order_list']),
},
async mounted() {
  this.loading = true;
  const settings = { lang: 'en_US' };
    await loadYmap(settings);
    console.log(ymaps);
  console.log('this.get_postavchik_order_list', this.get_postavchik_order_list);
  this.loading = false;
  let date_and_item = {
    auth_id: localStorage.AuthId
  }
  await this.fetchPostavchikOrder(date_and_item);
  this.map_show = true;
  
  // Global funksiyani window ga qo'shish
  window.showClientInfoGlobal = (clientId) => {
    this.showClientInfo(clientId);
  };
},
methods: {
  ...mapActions(['fetchOrder_list', 'fetchPostavchikOrder']),

    bindListener() {
      const btnclose = document.getElementById('btnclose');
      const btn = document.getElementById('btn');
      const infoBtn = document.querySelector('.info-icon-btn');
      
      if (btnclose) {
        btnclose.addEventListener('click', this.closeOrder);
      }
      if (btn) {
        btn.addEventListener('click', this.handleropen);
      }
      if (infoBtn) {
        infoBtn.addEventListener('click', (e) => {
          e.preventDefault();
          e.stopPropagation();
          const clientId = infoBtn.getAttribute('data-client-id');
          if (clientId) {
            this.showClientInfo(parseInt(clientId));
          }
        });
      }
    },
    unbindListener() {
      document.getElementById('btnclose').removeEventListener('click', this.closeOrder);
      document.getElementById('btn').removeEventListener('click', this.handleropen);
    },
    handleropen() {
      // this.myMap.balloon.close();
      const url = `https://yandex.ru/maps/?rtext=~${encodeURIComponent(this.select_mark.address.latidu + ',' + this.select_mark.address.longitu)}&rtt=auto`;
      window.open(url, '_blank', 'noopener');

    //   const appUrl = `yandexmaps://maps.yandex.ru/?pt=${this.select_mark.address.longitu},${this.select_mark.address.latidu}&z=16`;
    //   const webUrl = `https://yandex.ru/maps/?rtext=~${this.select_mark.address.latidu},${this.select_mark.address.longitu}&rtt=auto`;

    // // 1) Ilova ochishga harakat
    // window.location.href = appUrl;

    // // 2) Agar 800ms ichida ilova ochilmasa, fallbackga o'tish
    // setTimeout(() => {
    //   window.open(webUrl, '_blank', 'noopener');
    // }, 800);
    },
    mapLoaded(e){
      // console.log(e)
      this.myMap = e;
    },
    closeOrder(){
      this.pay_show = true;
      this.$refs.payNew.fetchMounted(this.selectMark_id);
    },
    sendFunc(id,data){
      this.selectMark_id = id;
      this.select_mark = data;
      // Ballon ochilganda info button uchun event listener qo'shish
      this.$nextTick(() => {
        setTimeout(() => {
          const infoBtn = document.querySelector('.info-icon-btn');
          if (infoBtn && data.client && data.client.id) {
            const clientId = data.client.id;
            infoBtn.setAttribute('data-client-id', clientId);
            // Eski event listenerlarni olib tashlash
            const newInfoBtn = infoBtn.cloneNode(true);
            infoBtn.parentNode.replaceChild(newInfoBtn, infoBtn);
            // Yangi event listener qo'shish
            newInfoBtn.onclick = (e) => {
              e.preventDefault();
              e.stopPropagation();
              console.log('Info button clicked from sendFunc, clientId:', clientId);
              this.showClientInfo(clientId);
            };
          }
        }, 200);
      });
    },
    direct(){
      console.log('directed')
    },

  async closeUpdate(){
    this.map_show = false;
    this.pay_show = false;
    let date_and_item = {
      auth_id: localStorage.AuthId
    }
    await this.fetchPostavchikOrder(date_and_item);
    this.map_show = true;
  },
  
 

  async fetchOrder(){
     try{
      this.loading = true;
      const response = await fetch(this.$store.state.hostname + "/WaterOrders/getPaginationOpenOrdersList?page=0&size=100");
      this.loading = false;
      if(response.status == 201 || response.status == 200)
      {
        const data = await response.json();
        this.order_list = data.items_list
        console.log(data)
        this.$refs.message.success('Added_successfully')
        return true;
      }
      else{
        const data = await response.text();
        this.modal_info = data;
        this.modal_status = true;
        return false;
      }
    }
    catch{
      this.loading = false;
      this.modal_info = this.$i18n.t('network_ne_connect'); 
      this.modal_status = true;
    }
  },
  async closeAcceptOrder(){
    this.map_show = false;
    this.pay_show = false;
    let date_and_item = {
      auth_id: localStorage.AuthId
    }
    await this.fetchPostavchikOrder(date_and_item);
    this.map_show = true;
  },

  async showClientInfo(clientId) {
    if (!clientId) {
      this.$refs.message.warning('Client ID topilmadi');
      return;
    }
    console.log('clientId', clientId);
    this.client_info_show = true;
    this.client_info_loading = true;
    this.client_orders_list = [];
    
    try {
      const res = await fetch(
        this.$store.state.hostname + 
        '/WaterOrders/getPaginationOrderByClientId?page=0&size=300&client_id=' + 
        clientId
      );
      const data = await res.json();
      console.log('Client orders data:', data);
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
  },

  formatDate(dateString) {
    if (!dateString) return '';
    const date = new Date(dateString);
    const day = String(date.getDate()).padStart(2, '0');
    const month = String(date.getMonth() + 1).padStart(2, '0');
    const year = date.getFullYear();
    return `${day}.${month}.${year}`;
  }
},
}
</script>

<style lang="scss">
::-webkit-scrollbar {
  width: 5px;
  height: 5px;
}
/* Track */
::-webkit-scrollbar-track {
  background: #f1f1f1;
}

/* Handle */
::-webkit-scrollbar-thumb {
  background: rgb(78, 160, 255);
  border-radius: 5px;
}

/* Handle on hover */
::-webkit-scrollbar-thumb:hover {
  background: rgb(23, 65, 253);
}
.bg_gradiunt{
  background-image: radial-gradient( circle farthest-corner at 12.3% 19.3%,  rgba(85,88,218,1) 0%, rgba(95,209,249,1) 100.2% );
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
      font-size: 48px;
      margin-bottom: 15px;
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
        font-size: 14px;
        color: #666;
      }
    }
    
    .client-order-details {
      display: flex;
      flex-direction: column;
      gap: 8px;
      
      .client-order-detail-row {
        display: flex;
        justify-content: space-between;
        align-items: center;
        
        .detail-label {
          font-weight: 600;
          color: #555;
          font-size: 14px;
        }
        
        .detail-value {
          color: #333;
          font-size: 14px;
          text-align: right;
          
          &.status-complete {
            color: #4caf50;
            font-weight: 600;
          }
        }
      }
    }
  }
}
</style>