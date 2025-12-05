<template>
  <div class="payment-app">
    <LoaderTable v-if="loading"/>
    
    <template v-else>
    <!-- Desktop Version -->
    <div class="desktop-payment-content">
      <div class="desktop-payment-card">
        <div class="desktop-payment-header">
          <h3 class="desktop-client-name">{{client_name}}</h3>
          <h3 class="desktop-total-sum">{{summInString}} сум</h3>
        </div>

        <div class="desktop-payment-form">
          <!-- Note Field -->
          <div class="desktop-form-group" v-show="note">
            <label class="desktop-form-label">{{$t('note')}}</label>
            <textarea 
              v-model="note" 
              rows="2" 
              class="desktop-form-input"
              :placeholder="$t('note')"
            ></textarea>
          </div>

          <!-- Date Field -->
          <div class="desktop-form-group">
            <label class="desktop-form-label">{{$t('date')}}</label>
            <input 
              type="date" 
              disabled 
              v-model="order_date"
              class="desktop-form-input"
            />
          </div>

          <!-- Cash Field -->
          <div class="desktop-form-group">
            <label class="desktop-form-label">{{$t('cash')}}</label>
            <input 
              type="text" 
              v-model="cashInString"  
              v-on:keyup.13="payed" 
              @keyup="funcCash($event.target.value)"  
              ref="cashIn"  
              v-on:click.capture="cashNol"
              class="desktop-form-input desktop-input-right"
            />
          </div>

          <!-- Card Field -->
          <div class="desktop-form-group">
            <label class="desktop-form-label">{{$t('card')}}</label>
            <input 
              type="text" 
              v-model="cardInString"  
              v-on:keyup.13="payed" 
              @keyup="funcCard($event.target.value)"
              ref="cashIn" 
              v-on:click.capture="cardNol" 
              class="desktop-form-input desktop-input-right"
            />
          </div>

          <!-- Give Bottle Field -->
          <div class="desktop-form-group">
            <label class="desktop-form-label">{{$t('give_bootle')}}</label>
            <input 
              type="number" 
              v-model="main_product_qty" 
              @blur="funcGiveNol" 
              @keyup="funcGiveBootle($event.target.value)"
              class="desktop-form-input desktop-input-right"
            />
          </div>

          <!-- Get Bottle Field -->
          <div class="desktop-form-group">
            <label class="desktop-form-label">{{$t('getten_bootle')}}</label>
            <input 
              type="number" 
              v-model="get_bootle" 
              @keyup="funcBootle($event.target.value)" 
              @keyup.enter="payed"
              ref="get_Bootle" 
              v-on:click.capture="funcBootle" 
              @blur="funcGetNol"
              class="desktop-form-input desktop-input-right"
            />
          </div>

          <!-- Order Items -->
          <div class="desktop-order-items" v-for="(item,index) in order_item" :key="index">
            <div class="desktop-item-row">
              <div class="desktop-item-name">
                <input 
                  type="text" 
                  v-model="item.product_name" 
                  disabled
                  class="desktop-form-input desktop-input-disabled"
                />
              </div>
              <div class="desktop-item-qty">
                <label class="desktop-form-label-small">{{$t('qty')}}</label>
                <input 
                  type="text" 
                  v-model="item.qty" 
                  @input="funcItemProduct(index)"
                  class="desktop-form-input desktop-input-right"
                />
              </div>
              <div class="desktop-item-price">
                <label class="desktop-form-label-small">{{$t('price')}}</label>
                <input 
                  type="text" 
                  disabled 
                  :value="item.price*item.qty" 
                  class="desktop-form-input desktop-input-right desktop-input-disabled"
                />
              </div>
            </div>
          </div>

          <!-- Balance Messages -->
          <div class="desktop-balance-messages">
            <div v-if="parseFloat(defaultSum.toFixed(2)) > summa" class="desktop-balance-success">
              <span>Больше денег</span>
              <span>{{(parseFloat(defaultSum.toFixed(2))-summa).toFixed(2)}}</span>
            </div>
            <div v-if="parseFloat(defaultSum.toFixed(2)) < summa" class="desktop-balance-danger">
              <span>Не хватить</span>
              <span>{{(parseFloat(defaultSum.toFixed(2))-summa).toFixed(2)}}</span>
            </div>
          </div>

          <!-- Action Button -->
          <div class="desktop-payment-actions">
            <mdb-btn 
              color="success" 
              @click="payed" 
              class="desktop-pay-btn"
            >
              <i class="fas fa-check-circle"></i>
              {{$t('pay')}}
            </mdb-btn>
          </div>
        </div>
      </div>
    </div>

    <!-- Mobile Version -->
    <div class="mobile-payment-content">
      <div class="mobile-payment-card">
        <div class="mobile-payment-header">
          <h4 class="mobile-client-name">{{client_name}}</h4>
          <h4 class="mobile-total-sum">{{summInString}} сум</h4>
        </div>

        <div class="mobile-payment-form">
          <!-- Note Field -->
          <div class="mobile-form-group" v-show="note">
            <label class="mobile-form-label">{{$t('note')}}</label>
            <textarea 
              v-model="note" 
              rows="2" 
              class="mobile-form-input"
              :placeholder="$t('note')"
            ></textarea>
          </div>

          <!-- Date Field -->
          <div class="mobile-form-group">
            <label class="mobile-form-label">{{$t('date')}}</label>
            <input 
              type="date" 
              disabled 
              v-model="order_date"
              class="mobile-form-input"
            />
          </div>

          <!-- Cash Field -->
          <div class="mobile-form-group">
            <label class="mobile-form-label">{{$t('cash')}}</label>
            <input 
              type="text" 
              v-model="cashInString"  
              v-on:keyup.13="payed" 
              @keyup="funcCash($event.target.value)"  
              ref="cashIn"  
              v-on:click.capture="cashNol"
              class="mobile-form-input mobile-input-right"
            />
          </div>

          <!-- Card Field -->
          <div class="mobile-form-group">
            <label class="mobile-form-label">{{$t('card')}}</label>
            <input 
              type="text" 
              v-model="cardInString"  
              v-on:keyup.13="payed" 
              @keyup="funcCard($event.target.value)"
              ref="cashIn" 
              v-on:click.capture="cardNol" 
              class="mobile-form-input mobile-input-right"
            />
          </div>

          <!-- Give Bottle Field -->
          <div class="mobile-form-group">
            <label class="mobile-form-label">{{$t('give_bootle')}}</label>
            <input 
              type="number" 
              v-model="main_product_qty" 
              @blur="funcGiveNol" 
              @keyup="funcGiveBootle($event.target.value)"
              class="mobile-form-input mobile-input-right"
            />
          </div>

          <!-- Get Bottle Field -->
          <div class="mobile-form-group">
            <label class="mobile-form-label">{{$t('getten_bootle')}}</label>
            <input 
              type="number" 
              v-model="get_bootle" 
              @keyup="funcBootle($event.target.value)" 
              @keyup.enter="payed"
              ref="get_Bootle" 
              v-on:click.capture="funcBootle" 
              @blur="funcGetNol"
              class="mobile-form-input mobile-input-right"
            />
          </div>

          <!-- Order Items -->
          <div class="mobile-order-items" v-for="(item,index) in order_item" :key="index">
            <div class="mobile-item-row">
              <div class="mobile-item-name">
                <input 
                  type="text" 
                  v-model="item.product_name" 
                  disabled
                  class="mobile-form-input mobile-input-disabled"
                />
              </div>
              <div class="mobile-item-qty">
                <label class="mobile-form-label-small">{{$t('qty')}}</label>
                <input 
                  type="text"
                  v-model="item.qty" 
                  @input="funcItemProduct(index)"
                  class="mobile-form-input mobile-input-right"
                />
              </div>
              <div class="mobile-item-price">
                <label class="mobile-form-label-small">{{$t('price')}}</label>
                <input 
                  type="text" 
                  disabled 
                  :value="item.price*item.qty" 
                  class="mobile-form-input mobile-input-right mobile-input-disabled"
                />
              </div>
            </div>
          </div>

          <!-- Balance Messages -->
          <div class="mobile-balance-messages">
            <div v-if="parseFloat(defaultSum.toFixed(2)) > summa" class="mobile-balance-success">
              <span>Больше денег</span>
              <span>{{(parseFloat(defaultSum.toFixed(2))-summa).toFixed(2)}}</span>
            </div>
            <div v-if="parseFloat(defaultSum.toFixed(2)) < summa" class="mobile-balance-danger">
              <span>Не хватить</span>
              <span>{{(parseFloat(defaultSum.toFixed(2))-summa).toFixed(2)}}</span>
            </div>
          </div>

          <!-- Action Button -->
          <div class="mobile-payment-actions">
            <mdb-btn 
              color="success" 
              @click="payed" 
              class="mobile-pay-btn"
            >
              <i class="fas fa-check-circle"></i>
              {{$t('pay')}}
            </mdb-btn>
          </div>
        </div>
      </div>
    </div>

    <massage_box 
      :hide="modal_status" 
      :detail_info="modal_info"
      :m_text="$t('Failed_to_add')" 
      @to_hide_modal="modal_status= false"
    />

    <Toast ref="message"></Toast>
    </template>
  </div>
</template>

<script>
import {mdbBtn, } from "mdbvue"
import LoaderTable from "../../../components/loaderTable.vue";

export default {
  components:{
    mdbBtn,
    LoaderTable
},
  data() {
    return {
      modal_info: '',
      modal_status: false,
      loading: false,

      cashInString: '0',
      cashIn: 0,
      cardInString: '0',
      cardIn: 0,

      summa: 0,
      discount: 0,
      discountSum: 0,
      defaultSum: 0,

      get_bootle: 0,

      main_product_qty: 0,
      default_product_qty: 0,

      main_order_id: null,
      main_product_id: 1,
      order_summa: 0,
      summInString: '',
      client_name: '',
      client_id: null,
      address_id: null,
      main_product_price: 0,
      deleivered_user_auth_id:null,

      delete_client_stats : 0,

      order_item: [],
      invoice_id: null,
      order_date: '',
      note: '',
      client_bootle_id: 0,
      check_id: 0,
    }
  },
  props: {
    order:
    {
      type: Object,
      default(){
        return {}
      }
    },
    product_id : {
      type : Number,
      default : 0
    },
    bootle_qty : {
      type : Number,
      default : 0
    },
    shown: {
      type: Boolean,
      default: false,
    },
    orderId : {
      type : Number,
      default : 0
    },
  },
  async mounted() {
    
  },
  methods: {
    async fetchMounted(id){
      try{
        const res = await fetch(this.$store.state.hostname + '/WaterOrders/getOrderFullInfoByid?order_id=' + id);
        const data = await res.json();
        console.log('this is by id')
        console.log(data)
        this.invoice_id = id;
        this.main_product_qty = 0;
        this.default_product_qty = 0;
        this.main_product_price = 0;
        this.main_product_id = 1,
        this.main_order_id = null,
        this.order_summa= 0;
        this.order_item = [];
        this.client_name = data.client_name_str;
        this.delete_client_stats = data.reserverd_note_2;
        this.client_id = data.client.id;
        this.address_id = data.address.id;
        this.note = data.note;
        this.order_date = data.order_date.slice(0,10);
        this.deleivered_user_auth_id = data.deleivered_user_auth_id;
        for(let i=0; i<data.items.length; i++){
          this.order_summa += parseFloat(data.items[i].qty* parseFloat(data.items[i].product.info));
          if(data.items[i].product.main_product == true){
            this.main_product_id = data.items[i].product.id;
            this.main_order_id = data.items[i].id;
            this.main_product_qty = data.items[i].qty;
            this.default_product_qty = data.items[i].qty;
            this.main_product_price = parseFloat(data.items[i].product.info);
          }
          else{
            let item_cash = {
              product_name: data.items[i].product.name,
              waterProductid: data.items[i].product.id,
              qty: data.items[i].qty,
              real_qty: data.items[i].qty,
              note: '',
              id: data.items[i].id,
              price: parseFloat(data.items[i].product.info)
            }
             this.order_item.push(item_cash)
          }
        }
        this.summInString = new Intl.NumberFormat().format(this.order_summa)
        console.log(this.order_summa)
        this.summa = this.order_summa;
        this.cashIn = this.order_summa;
        this.cardInString = '0';
        this.cardIn = 0;
        this.defaultSum = this.order_summa;
        this.cashInString = this.summInString;
      }
      catch{
        console.log('error')
      }
      this.$refs.get_Bootle.focus();
      this.get_bootle = null;
      await this.fetchOstatka();
    },
    async fetchOstatka(){
      try{
        this.loadingSimple = true;
        const response = await fetch(this.$store.state.hostname + "/WaterClientBottleInfoes/getPaginationClientIdAndAddressId?page=0&size=1&client_id=" + this.client_id + '&address_id=' + this.address_id);
        const data = await response.json();
        this.loadingSimple = false;
        console.log(data)
        if(data.items_list.length>0){
          this.get_bootle = data.items_list[0].bottle_count
        }
        else{
          this.get_bootle = null
        }
      }
      catch{
        this.get_bootle = null
      }
    },
    funcItemProduct(index){
      this.order_item[index].real_qty = this.order_item[index].qty;
      let easy_sum = 0;
      easy_sum = this.main_product_qty * this.main_product_price;
      for(let i=0; i<this.order_item.length; i++){
        easy_sum += (this.order_item[i].qty * this.order_item[i].price)
      }
      this.summa = easy_sum;
      this.summInString = new Intl.NumberFormat().format(this.summa);
      this.cashIn = this.summa;
      this.defaultSum = this.summa;
      this.cashInString = this.summInString;
      this.cardInString = '0';
      this.cardIn = 0;
    },
    getSumma(summ, summString){
      this.cashIn = summ;
      this.summa = summ;
      this.defaultSum = summ;
      this.cashInString = summString;
    },
    funcCash(n){
      this.discount = parseFloat(this.cashIn) + parseFloat(this.cardIn);
      this.discountSum = parseFloat(this.summa) - parseFloat(this.discount);
      this.discountSum = parseFloat(this.discountSum.toFixed(2))

      if(this.discountSum == 0){
      console.log('this.discountSum')
        this.cashIn = 0;
        this.cashInString = ''; 
        n = n[n.length-1];
      }

      console.log(n)
      var tols = ''
      for(let i=0; i<n.length; i++){
        if(n[i] != ' '){
          tols += n[i];
        }
       }

       this.cashInString = tols.replace(/(\d)(?=(\d{3})+(\.(\d){0,2})*$)/g, '$1 ');
       var temp = ''
       for(let i=0; i<this.cashInString.length; i++){
        if(this.cashInString[i] != ' '){
          temp += this.cashInString[i];
        }
       }
      this.cashIn = parseFloat(temp);
      this.defaultSum = parseFloat(this.cashIn) + parseFloat(this.cardIn);
    },
    funcCard(n){
      this.discount = parseFloat(this.cashIn) + parseFloat(this.cardIn);
      this.discountSum = parseFloat(this.summa) - parseFloat(this.discount);
      this.discountSum = parseFloat(this.discountSum.toFixed(2))

      if(this.discountSum == 0){
        console.log('this.discountSum')
        this.cardIn = 0;
        this.cardInString = ''; 
        n = n[n.length-1];
      }
      console.log(n)
      var tols = ''
      for(let i=0; i<n.length; i++){
        if(n[i] != ' '){
          tols += n[i];
        }
       }
       this.cardInString = tols.replace(/(\d)(?=(\d{3})+(\.(\d){0,2})*$)/g, '$1 ');
       var temp = ''
       for(let i=0; i<this.cardInString.length; i++){
        if(this.cardInString[i] != ' '){
          temp += this.cardInString[i];
        }
       }
      this.cardIn = parseFloat(temp);
      this.defaultSum = parseFloat(this.cashIn) + parseFloat(this.cardIn);
    },
    funcGiveBootle(n){
      var tols = ''
      for(let i=0; i<n.length; i++){
        if(n[i] != ' '){
          tols += n[i];
        }
      }
      if(tols == '' || tols == null){
        tols = 0;
      }
      console.log(tols)
      let easy_sum = 0;
      easy_sum = this.main_product_price * parseFloat(tols);
      for(let i=0; i<this.order_item.length; i++){
        easy_sum += (this.order_item[i].qty * this.order_item[i].price)
      }
      this.summa = easy_sum;
      this.summInString =  new Intl.NumberFormat().format(this.summa);
      this.cashIn = this.summa;
      this.defaultSum = this.summa;
      this.cashInString = this.summInString;
      this.cardInString = '0';
      this.cardIn = 0;
    },
    funcBootle(){
      if(this.get_bootle == 0){
        this.get_bootle = null;
      }
    },
    funcGetNol(){
      if(this.get_bootle == null){
        this.get_bootle = 0;
      }
    },
    funcGiveNol(){
      if(this.main_product_qty == null || this.main_product_qty == ''){
        this.main_product_qty = 0;
      }
    },
   
    cashNol(){
      this.discount = parseFloat(this.cardIn);
      this.discountSum = parseFloat(this.summa) - parseFloat(this.discount);

      if(this.cashIn == this.summa || this.cardIn == this.summa ){
        this.cashIn = this.summa;
        this.cashInString = this.cashIn.toString().replace(/(\d)(?=(\d{3})+(\.(\d){0,2})*$)/g, '$1 ');
        this.cardIn = 0;
        this.cardInString = '0';
        console.log('this.cashIn')
        console.log(this.cashIn)
      }
      else if(this.discountSum > 0){
        this.cashIn = parseFloat(this.discountSum.toFixed(2));
        console.log(this.cashIn)
        this.cashInString = this.cashIn.toString().replace(/(\d)(?=(\d{3})+(\.(\d){0,2})*$)/g, '$1 ');
      }

      this.defaultSum = this.cashIn + this.cardIn;
    },
    cardNol(){
      this.discount = parseFloat(this.cashIn);
      this.discountSum = parseFloat(this.summa) - parseFloat(this.discount);

      if(this.cashIn == this.summa || this.cardIn == this.summa ){
        this.cashIn = 0;
        this.cashInString = '0';
        this.cardIn = this.summa;
        this.cardInString = this.cardIn.toString().replace(/(\d)(?=(\d{3})+(\.(\d){0,2})*$)/g, '$1 ');
        console.log('this.cardIn')
        console.log(this.cardIn)
      }
       else if(this.discountSum > 0){
        this.cardIn = parseFloat(this.discountSum.toFixed(2));
        console.log(this.cardIn)
        this.cardInString = this.cardIn.toString().replace(/(\d)(?=(\d{3})+(\.(\d){0,2})*$)/g, '$1 ');
      }
      this.defaultSum = this.cashIn + this.cardIn;
    },
    async fetchBootle(){
      if(this.get_bootle == null || this.get_bootle == ''){
        this.get_bootle = 0;
      }
      try{
        this.loading = true;
        const response = await fetch(this.$store.state.hostname + "/WaterOrders/addBotlfInfoToClientForAccept?olingan_bakalshka_soni=" + ((-1) * parseInt(this.get_bootle)) + '&berilgan_bakalshka_soni=' + this.main_product_qty + '&product_id=' + this.main_product_id + '&client_id=' + this.client_id + '&address_id=' + this.address_id);
        this.loading = false;
        if(response.status == 201 || response.status == 200)
        {
          const data = await response.json();
          console.log('client_bootle_id')
          console.log(data)
          this.client_bootle_id = data.id;
          if(this.delete_client_stats == 5)
          {
            console.log('To delete client')
             await fetch(this.$store.state.hostname + "/WaterClients/deleteClientByIdAndNote?id=" + this.client_id + '&note=!...');
          }
          // this.$refs.message.success('Added_successfully')
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
        return false;
      }
    },
    async fetchCheck(){
      const requestOptions = {
        method : "POST",
        headers: { "Content-Type" : "application/json" },
        body: JSON.stringify({
          "cash" : this.cashIn,
          "card" : this.cardIn,
          "summ" : this.summa,
         
          "user_name": this.client_name,
          "reserverd_number_id_1":this.get_bootle, // olgan baklashka soni
          "reserverd_number_id_2":this.main_product_qty, // bergan suv soni
          "reserverd_number_id_3": this.invoice_id, // invoice id si
          "waterAuthid": localStorage.AuthId,
          "id" : 0,
        })
      };
      try{
        this.loading = true;
          console.log('fetchCheck  respons')
          console.log(requestOptions)
        const response = await fetch(this.$store.state.hostname + "/WaterChecks", requestOptions);
      
        this.loading = false;
        if(response.status == 201 || response.status == 200)
        {
          const data = await response.json();
          console.log('check_id')
          console.log(data)
          this.check_id = data.id;
          this.$refs.message.success('Added_successfully')
          return true;
        }
        else{
          this.modal_info = this.$i18n.t('network_ne_connect');
          this.modal_status = true;
          return false;
        }
      }
      catch{
        this.loading = false;
        this.modal_info = this.$i18n.t('network_ne_connect'); 
        this.modal_status = true;
        return false;
      }
    },

    async closeUpdateOrder(){
      if(await this.fetchUpdateOrder())
        this.$emit('closeUpdate')
    },

    async fetchUpdateOrder(){
      let itemsList = [];
      for(let i=0; i<this.order_item.length; i++){
        itemsList.push(this.order_item[i]);
      }

      let item_main = {
        waterProductid: this.main_product_id,
        qty: this.main_product_qty,
        real_qty: this.main_product_qty,
        note: '',
        id: this.main_order_id,
      }
      itemsList.unshift(item_main)
      

      const requestOptions = {
        method : "POST",
        headers: { "Content-Type" : "application/json" },
        body: JSON.stringify({
          "order_date" : this.order_date + 'T12:00:00.000Z',
          "waterClientid": this.client_id,
          "water_count": this.main_product_qty,
          "waterClientAddressid": this.address_id,
          "deleivered_user_auth_id": this.deleivered_user_auth_id,
          "note": this.note,
          "items": itemsList,
          "id" : this.invoice_id,
        })
      };
      try{
        this.loading = true;
        const response = await fetch(this.$store.state.hostname + "/WaterOrders", requestOptions);
        const data = await response.text();
        console.log(response)
        this.loading = false;
        if(response.status == 201 || response.status == 200)
        {
          return true;
        }
        else{
          this.modal_info = data;
          this.modal_status = true;
          return false;
        }
      }
      catch{
        this.loading = false;
        this.modal_info = this.$i18n.t('network_ne_connect'); 
        this.modal_status = true;
        return false;
      }


      
    },
    async AcceptOrder(id){
      try{
        const res = await fetch(this.$store.state.hostname + '/WaterOrders/closeAcceptOrder?id=' + id);
        const data = await res.json();
        console.log(data)
        if(res.status == 200 || res.status == 201){
          this.$refs.message.success('Added_successfully')
          return true;
        }
        else{
          this.modal_info = this.$i18n.t('network_ne_connect');
          this.modal_status = true;
          return false;
        }
      }
      catch{
        this.modal_info = this.$i18n.t('network_ne_connect');
        this.modal_status = true;
        return false;
      }
    },

    async payed(){
      if(this.get_bootle == null || this.get_bootle == ''){
        this.get_bootle = 0;
      }
      if(this.default_product_qty != this.main_product_qty || this.order_item.length>0){
        console.log('update qil orderni ishladi')
        await this.fetchUpdateOrder();
      }
      // await this.fetchBootle();
      // await this.fetchCheck();

      // if(await this.AcceptOrder(this.invoice_id) && await this.fetchBootle() && await this.fetchCheck()){
      //   this.$emit('close')
      //   await this.compOrder();
      // }
      await this.closeOrder();

    }, 
    async closeOrder(){
      const requestOptions = {
        method : "POST",
        headers: { "Content-Type" : "application/json" },
        body: JSON.stringify({
          orderId: this.invoice_id,
          clientId: this.client_id,
          addressId: this.address_id,
          productId: this.main_product_id,
          olinganBakalshkaSoni: ((-1) * parseInt(this.get_bootle)),
          berilganBakalshkaSoni: this.main_product_qty,
          checkId: 0,
          check:{
            "cash" : this.cashIn,
            "card" : this.cardIn,
            "summ" : this.summa,
            "user_name": this.client_name,
            "reserverd_number_id_1":this.get_bootle, // olgan baklashka soni
            "reserverd_number_id_2":this.main_product_qty, // bergan suv soni
            "reserverd_number_id_3": this.invoice_id, // invoice id si
            "waterAuthid": localStorage.AuthId,
            "id" : 0,
          }
        })
      };
      try{
        this.loading = true;
          console.log('fetchCheck  respons')
          console.log(requestOptions)
        const response = await fetch(this.$store.state.hostname + "/WaterOrders/close-order", requestOptions);
      
        this.loading = false;
        if(response.status == 201 || response.status == 200)
        {
          const data = await response.json();
          console.log('close_order')
          console.log(data)
          if(this.delete_client_stats == 5)
          {
            console.log('To delete client')
             await fetch(this.$store.state.hostname + "/WaterClients/deleteClientByIdAndNote?id=" + this.client_id + '&note=!...');
          }
          this.$refs.message.success('Added_successfully')
          this.$emit('close')
          return true;
        }
        else{
          this.modal_info = this.$i18n.t('network_ne_connect');
          this.modal_status = true;
          return false;
        }
      }
      catch{
        this.loading = false;
        this.modal_info = this.$i18n.t('network_ne_connect'); 
        this.modal_status = true;
        return false;
      }
    },
    async compOrder(){
      try{
        this.loading = true;
        const response = await fetch(this.$store.state.hostname + "/WaterOrders/updateOrderFullReturningInfo?order_id=" + this.invoice_id + '&olingan_bakalshka_soni=' + this.get_bootle + '&berilgan_bakalshka_soni=' + this.main_product_qty + '&product_id=' + this.main_product_id + '&water_client_bottle_info=' + this.client_bootle_id + '&check_id=' + this.check_id);
        this.loading = false;
        if(response.status == 201 || response.status == 200)
        {
          // const data = await response.json();
          // console.log('coplex', data)
          // console.log(data)
          // this.$refs.message.success('Added_successfully')
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
        return false;
      }
    },
  },
}
</script>

<style lang="scss" scoped>
.payment-app {
  min-height: 100vh;
  background: linear-gradient(135deg, #f5f7fa 0%, #c3cfe2 100%);
  font-family: 'Open Sans', sans-serif;
  padding: 15px;
  
  @media (max-width: 767px) {
    padding: 0;
    min-height: auto;
  }
}

// Desktop Styles
.desktop-payment-content {
  display: none;
  
  @media (min-width: 768px) {
    display: block;
    max-width: 800px;
    margin: 0 auto;
  }
}

.desktop-payment-card {
  background: white;
  border-radius: 16px;
  box-shadow: 0 4px 20px rgba(0,0,0,0.1);
  overflow: hidden;
}

.desktop-payment-header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 25px 30px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  
  .desktop-client-name {
    margin: 0;
    font-size: 24px;
    font-weight: 600;
  }
  
  .desktop-total-sum {
    margin: 0;
    font-size: 24px;
    font-weight: 700;
  }
}

.desktop-payment-form {
  padding: 30px;
}

.desktop-form-group {
  margin-bottom: 20px;
  
  .desktop-form-label {
    display: block;
    font-size: 13px;
    font-weight: 600;
    color: #555;
    margin-bottom: 8px;
  }
  
  .desktop-form-label-small {
    display: block;
    font-size: 11px;
    font-weight: 600;
    color: #555;
    margin-bottom: 5px;
  }
  
  .desktop-form-input {
    width: 100%;
    padding: 12px 15px;
    border: 2px solid #e0e0e0;
    border-radius: 10px;
    font-size: 15px;
    outline: none;
    transition: all 0.3s;
    background: #f8f9fa;
    
    &:focus {
      border-color: #667eea;
      background: white;
      box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
    }
    
    &.desktop-input-right {
      text-align: right;
      font-weight: 600;
    }
    
    &.desktop-input-disabled {
      background: #e9ecef;
      color: #6c757d;
      cursor: not-allowed;
    }
  }
  
  textarea.desktop-form-input {
    resize: vertical;
    min-height: 60px;
  }
}

.desktop-order-items {
  margin-bottom: 15px;
  padding: 15px;
  background: #f8f9fa;
  border-radius: 10px;
  
  .desktop-item-row {
    display: grid;
    grid-template-columns: 2fr 1fr 1fr;
    gap: 15px;
    align-items: end;
  }
}

.desktop-balance-messages {
  margin-top: 20px;
  padding: 15px;
  border-radius: 10px;
  
  .desktop-balance-success {
    display: flex;
    justify-content: space-between;
    color: #28a745;
    font-weight: 600;
    font-size: 14px;
    padding: 10px;
    background: #d4edda;
    border-radius: 8px;
  }
  
  .desktop-balance-danger {
    display: flex;
    justify-content: space-between;
    color: #dc3545;
    font-weight: 600;
    font-size: 14px;
    padding: 10px;
    background: #f8d7da;
    border-radius: 8px;
  }
}

.desktop-payment-actions {
  margin-top: 25px;
  display: flex;
  justify-content: flex-end;
  
  .desktop-pay-btn {
    padding: 12px 30px;
    font-size: 16px;
    font-weight: 600;
    border-radius: 10px;
    display: flex;
    align-items: center;
    gap: 8px;
    box-shadow: 0 4px 15px rgba(40, 167, 69, 0.3);
    
    &:hover {
      transform: translateY(-2px);
      box-shadow: 0 6px 20px rgba(40, 167, 69, 0.4);
    }
  }
}

// Mobile Styles
.mobile-payment-content {
  display: block;
  
  @media (min-width: 768px) {
    display: none;
  }
}

.mobile-payment-card {
  background: white;
  border-radius: 12px;
  box-shadow: 0 2px 10px rgba(0,0,0,0.1);
  overflow: hidden;
}

.mobile-payment-header {
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  color: white;
  padding: 15px 18px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  
  .mobile-client-name {
    margin: 0;
    font-size: 16px;
    font-weight: 600;
    flex: 1;
  }
  
  .mobile-total-sum {
    margin: 0;
    font-size: 16px;
    font-weight: 700;
    text-align: right;
  }
}

.mobile-payment-form {
  padding: 18px;
}

.mobile-form-group {
  margin-bottom: 15px;
  
  .mobile-form-label {
    display: block;
    font-size: 11px;
    font-weight: 600;
    color: #555;
    margin-bottom: 6px;
  }
  
  .mobile-form-label-small {
    display: block;
    font-size: 10px;
    font-weight: 600;
    color: #555;
    margin-bottom: 4px;
  }
  
  .mobile-form-input {
    width: 100%;
    padding: 10px 12px;
    border: 1.5px solid #e0e0e0;
    border-radius: 8px;
    font-size: 13px;
    outline: none;
    transition: all 0.3s;
    background: #f8f9fa;
    
    &:focus {
      border-color: #667eea;
      background: white;
      box-shadow: 0 0 0 2px rgba(102, 126, 234, 0.1);
    }
    
    &.mobile-input-right {
      text-align: right;
      font-weight: 600;
    }
    
    &.mobile-input-disabled {
      background: #e9ecef;
      color: #6c757d;
      cursor: not-allowed;
    }
  }
  
  textarea.mobile-form-input {
    resize: vertical;
    min-height: 50px;
  }
}

.mobile-order-items {
  margin-bottom: 12px;
  padding: 12px;
  background: #f8f9fa;
  border-radius: 8px;
  overflow-x: auto;
  -webkit-overflow-scrolling: touch;
  
  .mobile-item-row {
    display: grid;
    grid-template-columns: 2fr 1fr 1fr;
    gap: 10px;
    align-items: end;
    min-width: 280px;
    
    .mobile-item-name {
      min-width: 100px;
    }
    
    .mobile-item-qty {
      min-width: 70px;
    }
    
    .mobile-item-price {
      min-width: 80px;
    }
  }
}

.mobile-balance-messages {
  margin-top: 15px;
  padding: 12px;
  border-radius: 8px;
  
  .mobile-balance-success {
    display: flex;
    justify-content: space-between;
    color: #28a745;
    font-weight: 600;
    font-size: 12px;
    padding: 8px;
    background: #d4edda;
    border-radius: 6px;
  }
  
  .mobile-balance-danger {
    display: flex;
    justify-content: space-between;
    color: #dc3545;
    font-weight: 600;
    font-size: 12px;
    padding: 8px;
    background: #f8d7da;
    border-radius: 6px;
  }
}

.mobile-payment-actions {
  margin-top: 20px;
  display: flex;
  justify-content: flex-end;
  
  .mobile-pay-btn {
    padding: 10px 25px;
    font-size: 14px;
    font-weight: 600;
    border-radius: 8px;
    display: flex;
    align-items: center;
    gap: 6px;
    box-shadow: 0 3px 12px rgba(40, 167, 69, 0.3);
    
    &:hover {
      transform: translateY(-2px);
      box-shadow: 0 5px 15px rgba(40, 167, 69, 0.4);
    }
  }
}

@media (max-width: 480px) {
  .payment-app {
    padding: 10px;
  }
  
  .mobile-payment-header {
    padding: 12px 15px;
    
    .mobile-client-name {
      font-size: 14px;
    }
    
    .mobile-total-sum {
      font-size: 14px;
    }
  }
  
  .mobile-payment-form {
    padding: 15px;
  }
  
  .mobile-form-group {
    margin-bottom: 12px;
    
    .mobile-form-input {
      padding: 8px 10px;
      font-size: 12px;
    }
  }
  
  .mobile-order-items {
    padding: 10px;
    
    .mobile-item-row {
      gap: 8px;
      min-width: 260px;
      
      .mobile-item-name {
        min-width: 90px;
      }
      
      .mobile-item-qty {
        min-width: 60px;
      }
      
      .mobile-item-price {
        min-width: 70px;
      }
    }
  }
}
</style>
