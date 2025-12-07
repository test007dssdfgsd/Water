<template>
  <div class="map-order-car">
    <!-- Header - All elements in one row -->
    <div class="app-header">
      <div class="header-content">
        <!-- Back Button -->
        <button class="back-btn" @click="$router.go(-1)">
          <i class="fas fa-arrow-left"></i>
          {{$t('back')}}
        </button>

        <!-- Title -->
        <h2 class="header-title">{{$t('order')}} Map</h2>

        <!-- Stats -->
        <div class="stats-container">
          <div class="stat-card">
            <span class="stat-label">{{$t('order')}}:</span>
            <span class="stat-value">{{all_water_count}}</span>
          </div>
          <div class="stat-card" v-if="user_name">
            <span class="stat-label">{{user_name}}:</span>
            <span class="stat-value">{{postavchik_water_count}}</span>
          </div>
        </div>

        <!-- Date Filter -->
        <div class="date-filter">
          <div class="date-input-wrapper">
            <i class="fas fa-calendar-alt date-icon"></i>
            <input 
              type="date" 
              v-model="b_date" 
              class="date-input"
            />
          </div>
          <div class="date-input-wrapper">
            <i class="fas fa-calendar-alt date-icon"></i>
            <input 
              type="date" 
              v-model="e_date" 
              class="date-input"
            />
          </div>
          <button class="apply-btn" @click="acceptBtn()">
            <i class="fas fa-check"></i>
            {{$t('apply')}}
          </button>
        </div>
      </div>
    </div>

    <!-- Sidebar -->
    <div class="sidebar" :class="{'sidebar-open': show_car, 'sidebar-close': !show_car}">
      <div class="sidebar-header">
        <h3 class="sidebar-title">Доставчикам закрепить заказы</h3>
        <button class="sidebar-close-btn" @click="show_car = false">
          <i class="fas fa-times"></i>
        </button>
      </div>
      
      <div class="sidebar-content">
        <div class="user-select-wrapper">
          <erpSelect
            size="sm"
            :options="allUser.rows" 
            @select="selectOptionUser"
            :selected="user_name"
            :label="$t('province')"
          />
          <small v-if="$v.user_name.$dirty && user_id == null" class="invalid-text">
            {{$t('select_item')}}
          </small>
        </div>

        <div class="order-list-container">
          <div class="order-list-header">
            <i class="fas fa-list"></i>
            <span>Назначенные поставщику заказы</span>
          </div>
          <draggable 
            v-model="postavchik_list" 
            ghost-class="ghost" 
            class="scroll-orders" 
            group="people" 
            @start="drag=true" 
            @end="onEnd"
          >
            <transition-group type="transition" class="order-list" name="flip-list">
              <div 
                class="order-item-card" 
                v-for="(item,index) in postavchik_list" 
                :key="item.id"
              >
                <div class="order-item-number">{{index+1}}</div>
                <div class="order-item-content">
                  <div class="order-item-name">{{item.client_name_str}}</div>
                  <div class="order-item-meta">
                    <span class="order-item-date">
                      <i class="fas fa-calendar"></i>
                      {{item.order_date.slice(0,10)}}
                    </span>
                    <span class="order-item-qty">
                      <i class="fas fa-box"></i>
                      {{item.water_count}}
                    </span>
                  </div>
                </div>
                <button 
                  class="order-item-remove" 
                  @click="removeFromPost(item, index)"
                >
                  <i class="fas fa-times"></i>
                </button>
              </div>
            </transition-group>
          </draggable>
        </div>
      </div>
    </div>

    <!-- Toggle Sidebar Button -->
    <div 
      class="toggle-sidebar-btn" 
      :class="{'btn-open': show_car, 'btn-close': !show_car}" 
      @click="show_car = !show_car"
    >
      <mdb-icon 
        :icon="show_car ? 'angle-double-right' : 'angle-double-left'" 
        fas 
        class="toggle-icon"
      />
    </div>

    <!-- Map Container -->
    <div id="map" class="map-container">
      <loaderTable class="map-loader" v-if="loading"/>
      <yandex-map 
        v-if="map_show"
        :coords="coords"
        zoom="12"
        style="width: 100%; height: 100vh;"
      >
        <div v-for="(mark,i) in order_list" :key="i">
          <ymap-marker  
            :markerId="mark.id"
            marker-type="placemark"
            :coords="[mark.address.latidu, mark.address.longitu]"
            :hint-content="mark.client.fio"
            :icon="{
              imageSize: [43, 55],
              imageOffset: [-22, -55],
              content: mark.client.fio,
              color: mark.reserverd_note_3,
              contentOffset: [-22, -55],
            }"
            :cluster-name="mark.id"
            @click="sendFunc(mark, i)"
          >
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

    <massage_box 
      :hide="modal_status" 
      :detail_info="modal_info"
      :m_text="$t('Failed_to_add')" 
      @to_hide_modal="modal_status= false"
    />
    <Toast ref="message" style="z-index:2222222;"></Toast>
  </div>
</template>

<script>
import draggable from 'vuedraggable'
import { required } from 'vuelidate/lib/validators'
import {mdbIcon, mdbBtn, mdbInput} from 'mdbvue'
import { loadYmap } from 'vue-yandex-maps';
import { yandexMap, ymapMarker } from 'vue-yandex-maps'
import { mapActions, mapGetters } from 'vuex';
import myComponent from './ballon.vue'
import erpSelect from "../../components/erpSelectFio.vue";
import loaderTable from '../../components/loaderTable.vue';

export default {
  components:{
    mdbIcon, mdbBtn, mdbInput,
    yandexMap, ymapMarker, myComponent,
    erpSelect, draggable, loaderTable
  },
  data() {
    return {
      show_car: false,
      modal_info: '',
      modal_status: false,
      loading: false,
      map_show: false,
      coords: [41.311516, 69.283250],

      b_date: '',
      e_date: '',
      today_date: '',
      all_water_count: 0,
      postavchik_water_count: 0,
      orange: 'orange',

      order_list: [],
      cash_order_list: [],
      selected_order: [],
      postavchik_list: [],
      postavchik_array: [],
      selectMark_id: null,

      user_name: '',
      user_id: null,
      auth_id: null,
    }
  },
  validations: {
    user_name: {required},
  },
  computed: {
    ...mapGetters(['allOrder_list', 'allUser']),
  },
  async mounted() {
    this.loading = true;

    const settings = { lang: 'en_US' };
    await loadYmap(settings);
    await this.fetchUser();
    
    this.loading = false;
    this.map_show = true;

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
  methods: {
    ...mapActions(['fetchOrder_list', 'fetchUser']),
    bindListener() {
      document.getElementById('btnclose').addEventListener('click', this.closeOrder);
      document.getElementById('btn').addEventListener('click', this.handleropen);
    },
    unbindListener() {
      document.getElementById('btn').removeEventListener('click', this.handlerclose);
    },
    handleropen() {
      window.history.pushState('https://t.me/extreme_water_bot')
    },
    handlerclose() {
      console.log('Ishladi close')
    },
    closeOrder(){
      // this.$router.push('/close_order/' + this.selectMark_id)
    },

    async array_move(arr, old_index, new_index) {
      if (new_index >= arr.length) {
          var k = new_index - arr.length + 1;
          while (k--) {
              arr.push(undefined);
          }
      }
      arr.splice(new_index, 0, arr.splice(old_index, 1)[0]);
      return arr;
    },

    async onEnd(evt){
      // Eski holatni saqlab qo'yish (xatolik bo'lsa qaytarish uchun)
      const oldArray = [...this.postavchik_array];
      
      try {
        // Array tartibini yangilash
        let postavchikArr = await this.array_move(this.postavchik_array, evt.oldIndex, evt.newIndex);
        // Array ni to'g'ri formatda string ga o'tkazish (vergul bilan ajratilgan)
        let stringArray = postavchikArr.join(',');
        
        // Auth ni yangilash - loading ko'rsatmaslik (drag & drop jarayonida UX uchun)
        await this.fetchUpdateAuthOrderList(stringArray);
      } catch (error) {
        console.error('onEnd Error:', error);
        // Xatolik bo'lsa, eski holatga qaytarish
        this.postavchik_array = oldArray;
        // postavchik_list ni ham qaytarish kerak
        this.postavchik_list = this.postavchik_list.slice(); // Shallow copy
        this.$refs.message.error('network_ne_connect');
      }
    },

    async acceptBtn(){
      let d_time = {
        b_date: this.b_date  + 'T00:00:01.504Z',
        e_date: this.e_date + 'T23:59:01.504Z',
      }
      await this.fetchDateOrderList(d_time);
    },
    async fetchDateOrderList(date){
      try{
        this.loading = true;
        this.order_list= [];
        const res = await fetch(this.$store.state.hostname + '/WaterOrders/getPaginationBeatweanDateWithoutTimeOpenOrdersListByNotAddedAnyUser?page=0&size=1000&begin_date='+ date.b_date+'&end_date=' + date.e_date);
        const data = await res.json();
        this.loading = false;
        if(res.status == 200 || res.status == 201){
          this.order_list = data.items_list;
          this.all_water_count = 0;
          for(let i=0; i<data.items_list.length; i++){
            this.all_water_count += data.items_list[i].water_count;
          }
          console.log(this.all_water_count)
        }
        else{
          this.$refs.message.error('network_ne_connect')
        }
      }
      catch{
        this.$refs.message.error('network_ne_connect')
      }
    },

    async selectOptionUser(option){
      console.log(option)
      this.user_name = option.fio;
      this.user_id = option.id;
      this.auth_id = option.auth.id;
      this.postavchik_list= [];
      await this.fetchPostavchikList(this.auth_id);
    },
    async fetchPostavchikList(auth_id){
      try{
        console.log(auth_id)
        this.loading = true;
        this.postavchik_list= [];
        this.postavchik_array = [];
        this.postavchik_water_count = 0;
        const res = await fetch(this.$store.state.hostname + '/WaterOrders/getOrderListByStrListWithAuthId?id_auth=' + auth_id);
        const data = await res.json();
        console.log('data');
        console.log(data);
        this.loading = false;
        if(res.status == 200 || res.status == 201){
          this.postavchik_list = data;
          console.log('this.postavchik_water_count')
          console.log(this.postavchik_water_count)
          console.log(this.postavchik_array)
          for(let i=0; i<data.length; i++){
            this.postavchik_array.push(data[i].id);
            this.postavchik_water_count += parseFloat(data[i].water_count);
          }
        }
        else if(res.status == 404){
          this.$refs.message.error('order_not_found')
        }
        else{
          this.$refs.message.error('network_ne_connect')
        }
      }
      catch{
        this.$refs.message.error('network_ne_connect')
        this.loading = false;
      }
    },

    async sendFunc(mark){
      this.loading = true;
      this.selectMark_id = mark.id;
      if(this.auth_id){
        this.postavchik_list.push(mark);
        this.postavchik_array.push(mark.id);
        // Array ni to'g'ri formatda string ga o'tkazish (vergul bilan ajratilgan)
        let stringArray = this.postavchik_array.join(',');
        await this.fetchAddOrderToUserWithAuthUpdate(mark.id, stringArray);
      }
      else{
        this.$refs.message.error('postavchik_no_selected')
      }
      this.loading = false;
    },

    // Yangi optimal API - bitta so'rovda barcha operatsiyalarni bajaradi
    async fetchAddOrderToUserWithAuthUpdate(order_id, id_str_list){
      try{
        // Ma'lumotlarni tekshirish
        if(!order_id || !this.auth_id){
          this.$refs.message.error('postavchik_no_selected');
          return false;
        }

        this.loading = true;
        
        // URL ni to'g'ri formatda yaratish
        const url = this.$store.state.hostname + 
          '/WaterOrders/addOrderToUserWithAuthUpdate?order_id=' + order_id + 
          '&user_auth_id=' + this.auth_id + 
          '&id_str_list=' + encodeURIComponent(id_str_list || '');
        
        console.log('API Request URL:', url);
        
        const res = await fetch(url);
        
        // Response ni tekshirish
        if(!res.ok && res.status !== 200 && res.status !== 201){
          const errorText = await res.text();
          console.error('API Error Response:', res.status, errorText);
          this.loading = false;
          
          if(res.status == 400){
            this.$refs.message.error('postavchik_no_selected');
          }
          else if(res.status == 404){
            this.$refs.message.error('order_not_found');
          }
          else{
            this.$refs.message.error('network_ne_connect');
          }
          return false;
        }
        
        const data = await res.json();
        this.loading = false;
        
        if(res.status == 200 || res.status == 201){
          if(data.success){
            this.$refs.message.success('Added_successfully');
            // Ma'lumotlarni yangilash
            await this.acceptBtn();
            await this.fetchPostavchikList(this.auth_id);
            return true;
          } else {
            this.$refs.message.error('network_ne_connect');
            return false;
          }
        }
        else{
          this.$refs.message.error('network_ne_connect');
          return false;
        }
      }
      catch(error){
        console.error('API Error:', error);
        this.$refs.message.error('network_ne_connect');
        this.loading = false;
        return false;
      }
    },

    // Eski API-lar - orqaga moslik uchun saqlanadi (agar kerak bo'lsa)
    async fetchAddMarkToPostavchik(id){
      try{
        this.loading = true;
        console.log(id)
        const res = await fetch(this.$store.state.hostname + '/WaterOrders/addOrderToUsersWithAuthid?id='+ id +'&user_auth_id=' + this.auth_id);
        this.loading = false;
        if(res.status == 200 || res.status == 201){
          await this.acceptBtn();
          await this.fetchPostavchikList(this.auth_id); 
          return true;
        }
        else if(res.status == 400){
          this.$refs.message.error('postavchik_no_selected')
          return false;
        }
        else{
          this.$refs.message.error('network_ne_connect')
          return false;
        }
      }
      catch{
        this.$refs.message.error('network_ne_connect')
        this.loading = false;
        return false;
      }
    },
    // Yangi optimal funksiya - Auth ni yangilash (loading ko'rsatmaslik)
    async fetchUpdateAuthOrderList(string){
      try{
        const res = await fetch(
          this.$store.state.hostname + 
          '/WaterAuths/addOrderIdListForAuth?auth_id=' + this.auth_id + 
          '&id_str_list=' + encodeURIComponent(string)
        );
        
        if(res.status == 200 || res.status == 201){
          return true;
        }
        else{
          throw new Error('API Error: ' + res.status);
        }
      }
      catch(error){
        console.error('fetchUpdateAuthOrderList Error:', error);
        throw error;
      }
    },

    // Eski funksiya - orqaga moslik uchun saqlanadi
    async fetchAddMarkIDList(string){
      try{
        this.loading = true;
        const res = await fetch(this.$store.state.hostname + '/WaterAuths/addOrderIdListForAuth?auth_id=' + this.auth_id + '&id_str_list=' + encodeURIComponent(string));
        this.loading = false;
        if(res.status == 200 || res.status == 201){
          return true;
        }
        else{
          this.$refs.message.error('network_ne_connect')
          return false;
        }
      }
      catch{
        this.$refs.message.error('network_ne_connect')
        this.loading = false;
        return false;
      }
    },

    async removeFromPost(item, index){
      // Eski holatni saqlab qo'yish (xatolik bo'lsa qaytarish uchun)
      const oldArray = [...this.postavchik_array];
      const oldList = [...this.postavchik_list];
      const oldWaterCount = this.postavchik_water_count;
      
      // Frontend da o'chirish (UX uchun)
      this.postavchik_array.splice(index,1);
      this.postavchik_list.splice(index,1);
      this.postavchik_water_count -= parseFloat(item.water_count);
      
      // Array ni to'g'ri formatda string ga o'tkazish (vergul bilan ajratilgan)
      let stringArray = this.postavchik_array.length > 0 ? this.postavchik_array.join(',') : '';
      
      try {
        const success = await this.fetchRemoveOrderFromUserWithAuthUpdate(item.id, stringArray);
        
        if(!success){
          // Xatolik bo'lsa, eski holatga qaytarish
          this.postavchik_array = oldArray;
          this.postavchik_list = oldList;
          this.postavchik_water_count = oldWaterCount;
        }
      } catch (error) {
        console.error('removeFromPost Error:', error);
        // Xatolik bo'lsa, eski holatga qaytarish
        this.postavchik_array = oldArray;
        this.postavchik_list = oldList;
        this.postavchik_water_count = oldWaterCount;
      }
    },

    // Yangi optimal API - bitta so'rovda order ni olib tashlash va auth ni yangilash
    async fetchRemoveOrderFromUserWithAuthUpdate(order_id, id_str_list){
      try{
        // Ma'lumotlarni tekshirish
        if(!order_id || !this.auth_id){
          this.$refs.message.error('postavchik_no_selected');
          return false;
        }

        this.loading = true;
        
        // URL ni to'g'ri formatda yaratish
        const url = this.$store.state.hostname + 
          '/WaterOrders/removeOrderFromUserWithAuthUpdate?order_id=' + order_id + 
          '&user_auth_id=' + this.auth_id + 
          '&id_str_list=' + encodeURIComponent(id_str_list || '');
        
        console.log('API Request URL:', url);
        
        const res = await fetch(url);
        
        // Response ni tekshirish
        if(!res.ok && res.status !== 200 && res.status !== 201){
          const errorText = await res.text();
          console.error('API Error Response:', res.status, errorText);
          this.loading = false;
          
          if(res.status == 400){
            this.$refs.message.error('postavchik_no_selected');
          }
          else if(res.status == 404){
            this.$refs.message.error('order_not_found');
          }
          else{
            this.$refs.message.error('network_ne_connect');
          }
          return false;
        }
        
        const data = await res.json();
        this.loading = false;
        
        if(res.status == 200 || res.status == 201){
          if(data.success){
            // Ma'lumotlarni yangilash - xaritani va dostavchik ro'yxatini
            await this.acceptBtn(); // Xaritani yangilash
            await this.fetchPostavchikList(this.auth_id); // Dostavchik ro'yxatini yangilash
            return true;
          } else {
            this.$refs.message.error('network_ne_connect');
            return false;
          }
        }
        else{
          this.$refs.message.error('network_ne_connect');
          return false;
        }
      }
      catch(error){
        console.error('API Error:', error);
        this.$refs.message.error('network_ne_connect');
        this.loading = false;
        return false;
      }
    },

    // Eski API - orqaga moslik uchun saqlanadi (agar kerak bo'lsa)
    async deleteRow(id){
      try{
        this.loading = true;
        const res = await fetch(this.$store.state.hostname + '/WaterOrders/addDeOrderToUsersWithAuthidRemoveCar?id='+ id);
        this.loading = false;
        if(res.status == 200 || res.status == 201){
          await this.acceptBtn();
          return true;
        }
        else{
          this.$refs.message.error('network_ne_connect')
          return false;
        }
      }
      catch{
        this.$refs.message.error('network_ne_connect')
        this.loading = false;
        return false;
      }
    },
  },
}
</script>

<style lang="scss" scoped>
@import url(https://fonts.googleapis.com/css?family=Open+Sans:400,600,700);

* {
  box-sizing: border-box;
}

.map-order-car {
  min-height: 100vh;
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
  font-family: 'Open Sans', sans-serif;
  position: relative;
}

// Header - All elements in one row
.app-header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 12px 30px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
  position: fixed;
  top: 0;
  left: 0;
  right: 0;
  z-index: 1000;
  height: 45px;
  display: flex;
  align-items: center;
  
  .header-content {
    width: 100%;
    display: flex;
    align-items: center;
    gap: 20px;
    
    .back-btn {
      background: rgba(255,255,255,0.2);
      border: none;
      color: white;
      padding: 5px 10px;
      border-radius: 8px;
      cursor: pointer;
      font-size: 12px;
      font-weight: 600;
      display: flex;
      align-items: center;
      gap: 8px;
      transition: all 0.3s;
      white-space: nowrap;
      flex-shrink: 0;
      
      &:hover {
        background: rgba(255,255,255,0.3);
      }
    }
    
    .header-title {
      margin: 0;
      font-size: 15px;
      font-weight: 600;
      white-space: nowrap;
      flex-shrink: 0;
    }
    
    .stats-container {
      display: flex;
      align-items: center;
      gap: 15px;
      flex-shrink: 0;
      
      .stat-card {
        background: rgba(255,255,255,0.15);
        backdrop-filter: blur(10px);
        border-radius: 8px;
        padding: 5px 10px;
        display: flex;
        align-items: center;
        gap: 8px;
        transition: all 0.3s;
        white-space: nowrap;
        
        &:hover {
          background: rgba(255,255,255,0.25);
        }
        
        .stat-label {
          font-size: 12px;
          opacity: 0.9;
          font-weight: 600;
        }
        
        .stat-value {
          font-size: 12px;
          font-weight: 700;
        }
      }
    }
    
    .date-filter {
      display: flex;
      align-items: center;
      gap: 10px;
      margin-left: auto;
      flex-shrink: 0;
      
      .date-input-wrapper {
        position: relative;
        display: flex;
        align-items: center;
        
        .date-icon {
          position: absolute;
          left: 10px;
          color: rgba(255,255,255,0.7);
          font-size: 14px;
          z-index: 1;
        }
        
        .date-input {
          width: 160px;
          padding: 5px 10px 5px 35px;
          border: 2px solid rgba(255,255,255,0.3);
          border-radius: 8px;
          background: rgba(255,255,255,0.15);
          color: white;
          font-size: 12px;
          outline: none;
          transition: all 0.3s;
          
          &::-webkit-calendar-picker-indicator {
            filter: invert(1);
            cursor: pointer;
          }
          
          &:focus {
            border-color: rgba(255,255,255,0.5);
            background: rgba(255,255,255,0.25);
          }
        }
      }
      
      .apply-btn {
        background: rgba(255,255,255,0.25);
        border: 2px solid rgba(255,255,255,0.3);
        color: white;
        padding: 5px 18px;
        border-radius: 8px;
        font-size: 12px;
        font-weight: 600;
        cursor: pointer;
        display: flex;
        align-items: center;
        gap: 8px;
        transition: all 0.3s;
        white-space: nowrap;
        
        &:hover {
          background: rgba(255,255,255,0.35);
          transform: translateY(-1px);
        }
      }
    }
  }
}

// Sidebar
.sidebar {
  position: fixed;
  top: 45px;
  bottom: 0;
  right: 0;
  width: 380px;
  background: white;
  z-index: 999;
  box-shadow: -2px 0 10px rgba(0,0,0,0.1);
  transition: transform 0.5s ease;
  
  &.sidebar-open {
    transform: translateX(0);
  }
  
  &.sidebar-close {
    transform: translateX(100%);
  }
  
  .sidebar-header {
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    color: white;
    padding: 5px 20px;
    display: flex;
    justify-content: space-between;
    align-items: center;
    
    .sidebar-title {
      margin: 0;
      font-size: 13px;
      font-weight: 600;
    }
    
    .sidebar-close-btn {
      background: rgba(255,255,255,0.2);
      border: none;
      color: white;
      padding: 5px 10px;
      border-radius: 8px;
      cursor: pointer;
      font-size: 13px;
      transition: all 0.3s;
      
      &:hover {
        background: rgba(255,255,255,0.3);
      }
    }
  }
  
  .sidebar-content {
    padding: 10px 20px;
    height: calc(100vh - 45px);
    overflow-y: auto;
    
    .user-select-wrapper {
      margin-bottom: 15px;
      
      .invalid-text {
        color: #f44336;
        font-size: 12px;
        margin-top: 5px;
        display: block;
      }
    }
    
    .order-list-container {
      .order-list-header {
        display: flex;
        align-items: center;
        gap: 8px;
        margin-bottom: 15px;
        font-size: 12px;
        font-weight: 700;
        color: #667eea;
        text-transform: uppercase;
        
        i {
          font-size: 16px;
        }
      }
      
      .scroll-orders {
        max-height: calc(100vh - 190px);
        overflow-y: auto;
        
        .order-list {
          display: flex;
          flex-direction: column;
          gap: 4px;
          
          .order-item-card {
            cursor: pointer;
            background: white;
            border-radius: 12px;
            padding: 3px 10px;
            box-shadow: 0 2px 8px rgba(0,0,0,0.08);
            display: flex;
            align-items: center;
            gap: 12px;
            transition: all 0.3s;
            border-left: 3px solid #667eea;
            
            &:hover {
              transform: translateY(-2px);
              box-shadow: 0 4px 15px rgba(0,0,0,0.12);
            }
            
            .order-item-number {
              background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
              color: white;
              width: 30px;
              height: 28px;
              border-radius: 50%;
              display: flex;
              align-items: center;
              justify-content: center;
              font-weight: 700;
              font-size: 11px;
              flex-shrink: 0;
            }
            
            .order-item-content {
              flex: 1;
              min-width: 0;
              
              .order-item-name {
                font-size: 13px;
                font-weight: 700;
                color: #333;
                margin-bottom: 6px;
                overflow: hidden;
                text-overflow: ellipsis;
                white-space: nowrap;
              }
              
              .order-item-meta {
                display: flex;
                gap: 12px;
                font-size: 12px;
                color: #666;
                
                span {
                  display: flex;
                  align-items: center;
                  gap: 4px;
                  
                  i {
                    font-size: 10px;
                  }
                }
                
                .order-item-qty {
                  color: #667eea;
                  font-weight: 600;
                }
              }
            }
            
            .order-item-remove {
              background: #ffebee;
              border: none;
              color: #f44336;
              width: 30px;
              height: 28px;
              border-radius: 50%;
              display: flex;
              align-items: center;
              justify-content: center;
              cursor: pointer;
              transition: all 0.3s;
              flex-shrink: 0;
              
              &:hover {
                background: #f44336;
                color: white;
                transform: scale(1.1);
              }
            }
          }
        }
      }
    }
  }
}


// Toggle Sidebar Button
.toggle-sidebar-btn {
  position: fixed;
  z-index: 1111;
  right: 0;
  top: 50%;
  transform: translateY(-50%);
  width: 45px;
  height: 60px;
  cursor: pointer;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  border-top-left-radius: 8px;
  border-bottom-left-radius: 8px;
  display: flex;
  align-items: center;
  justify-content: center;
  box-shadow: -2px 0 10px rgba(0,0,0,0.2);
  transition: all 0.5s ease;
  
  &.btn-open {
    right: 380px;
  }
  
  &.btn-close {
    right: 0;
  }
  
  .toggle-icon {
    color: white;
    font-size: 22px;
  }
  
  &:hover {
    background: linear-gradient(135deg, #764ba2 0%, #667eea 100%);
    transform: translateY(-50%) scale(1.05);
  }
}

// Map Container
.map-container {
  width: 100%;
  height: calc(100vh - 45px);
  margin-top: 50px;
  position: relative;
  
  .map-loader {
    position: absolute;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -50%);
    z-index: 100;
  }
}

// Ghost class for drag
.ghost {
  opacity: 0.5;
}

// Flip list animation
.flip-list-move {
  transition: transform 0.3s;
}
</style>