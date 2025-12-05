<template>
  <div class="update-payment-app">
    <LoaderTable v-if="loading"/>
    <div v-else class="update-payment-content">
      <div class="payment-header">
        <h5 class="client-name">{{client_name}}</h5>
        <h5 class="total-sum">{{summInString}} сум</h5>
      </div>
      
      <div class="payment-form">
        <div class="form-group">
          <label class="form-label">{{$t('note')}}</label>
          <text-area rows="2" class="w-100 form-input" v-model="note" validate error="wrong" success="right"/>
        </div>
        
        <div class="form-group">
          <label class="form-label">{{$t('date')}}</label>
          <input 
            type="date" 
            v-model="order_date"
            class="form-input"
          />
        </div>
        
        <div class="form-group">
          <label class="form-label">{{$t('cash')}}</label>
          <input 
            type="text" 
            v-model="cashInString"  
            v-on:keyup.13="payed" 
            @keyup="funcCash($event.target.value)"  
            ref="cashIn"  
            v-on:click.capture="cashNol"
            class="form-input text-right"
          />
        </div>
        
        <div class="form-group">
          <label class="form-label">{{$t('card')}}</label>
          <input 
            type="text" 
            v-model="cardInString"  
            v-on:keyup.13="payed" 
            @keyup="funcCard($event.target.value)"
            ref="cardIn" 
            v-on:click.capture="cardNol" 
            class="form-input text-right"
          />
        </div>
        
        <div class="form-group">
          <label class="form-label">{{$t('give_bootle')}}</label>
          <input 
            type="number" 
            v-model="main_product_qty" 
            @blur="funcGiveNol" 
            @keyup="funcGiveBootle($event.target.value)"
            class="form-input text-right"
          />
        </div>

        <div class="form-group">
          <label class="form-label">{{$t('getten_bootle')}}</label>
          <input 
            type="number" 
            v-model="get_bootle" 
            @keyup="funcBootle($event.target.value)" 
            @keyup.enter="payed"
            ref="get_Bootle" 
            v-on:click.capture="funcBootle" 
            @blur="funcGetNol"
            class="form-input text-right"
          />
        </div>

        <div class="order-items-section" v-if="order_item.length > 0">
          <h6 class="section-title">{{$t('order_items')}}</h6>
          <div class="order-items-list">
            <div class="order-item-row" v-for="(item,index) in order_item" :key="index">
              <div class="item-col product-name">
                <input 
                  type="text" 
                  v-model="item.product_name" 
                  disabled
                  class="form-input"
                />
              </div>
              <div class="item-col qty-col">
                <label class="item-label">{{$t('qty')}}</label>
                <input 
                  type="text" 
                  v-model="item.qty" 
                  @input="funcItemProduct(index)"
                  class="form-input text-right"
                />
              </div>
              <div class="item-col price-col">
                <label class="item-label">{{$t('price')}}</label>
                <input 
                  type="text" 
                  disabled 
                  :value="item.price*item.qty" 
                  class="form-input text-right"
                />
              </div>
            </div>
          </div>
        </div>
        <div class="additional-products-section" v-show="productList.length>0">
          <h6 class="section-title">{{$t('additional_products')}}</h6>
          <div class="product-list">
            <div class="product-item" v-for="(item,i) in productList" :key="i">
              <div class="product-select">
                <erpSelect
                  size="sm"
                  :options="all_product_t.rows"
                  @select="selectOption"
                  :selected="item.product_name"
                  :label="$t('province')"
                  :index="i"
                />
                <label class="product-label">{{$t('select_product')}}</label>
              </div>
              <div class="product-qty">
                <input-img 
                  valid  
                  v-model="item.qty" 
                  style="height:32px;" 
                  error="wrong" 
                  success="right" 
                  icon="" 
                  type="number"
                />
                <label class="product-label">{{$t('qty')}}</label>
                <button 
                  class="remove-btn" 
                  @click="productList.splice(i,1)"
                >
                  <mdb-icon icon="trash" class="text-white"/>
                </button>
              </div>
            </div>
          </div>
        </div>

        <div class="form-group">
          <label class="form-label">{{$t('select_color')}}</label>
          <colorSelect
            size="sm"
            :options="color_list"
            @select="selectOptionColor"
            :selected="color_value"
            :color_name="color_name"
            :label="$t('province')"
          />
        </div>

        <div class="balance-info">
          <div class="balance-item success" v-if="parseFloat(defaultSum.toFixed(2)) > summa">
            <span class="balance-label">Больше денег</span>
            <span class="balance-value">{{(parseFloat(defaultSum.toFixed(2))-summa).toFixed(2)}}</span>
          </div>
          <div class="balance-item danger" v-if="parseFloat(defaultSum.toFixed(2)) < summa">
            <span class="balance-label">Не хватить</span>
            <span class="balance-value">{{(parseFloat(defaultSum.toFixed(2))-summa).toFixed(2)}}</span>
          </div>
        </div>
      </div>
      
      <div class="payment-actions">
        <mdb-btn 
          color="success" 
          @click="addProduct" 
          class="action-btn"
        >
          <i class="fas fa-plus mr-2"></i>
          {{$t('Add_product')}}
        </mdb-btn>
        <mdb-btn 
          color="primary" 
          @click="closeUpdateOrder" 
          class="action-btn primary-btn"
        >
          <i class="fas fa-save mr-2"></i>
          {{$t('update')}}
        </mdb-btn>
      </div>
    </div>
    
    <massage_box 
      :hide="modal_status" 
      :detail_info="modal_info"
      :m_text="$t('Failed_to_add')" 
      @to_hide_modal="modal_status= false"
    />

    <Toast ref="message"></Toast>
  </div>
</template>

<script>
import {mdbBtn,mdbIcon } from "mdbvue"
import LoaderTable from "../../../components/loaderTable.vue";
import InputImg from '@/components/inputImg.vue';
import {mapActions,mapGetters} from 'vuex'
import erpSelect from "@/components/erpSelectIndex";
import colorSelect from "@/components/colorSelect";

export default {
  components:{
    mdbBtn,
    LoaderTable,
    erpSelect,
    InputImg,
    mdbIcon,
    colorSelect
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
      main_product_id: null,
      order_summa: 0,
      summInString: '',
      client_name: '',
      client_id: null,
      address_id: null,
      main_product_price: 0,
      deleivered_user_auth_id:null,

      order_item: [],
      invoice_id: null,
      order_date: '',
      note: '',

      productList: [],
      
      color_list: [
        {
          name_fio: "Srochniy",
          name:'brown',
        },
        {
          name:'violet',
          name_fio: 'Bemalol'
        },
        {
          name:'yellow',
          name_fio: 'yellow'
        },
        {
          name:'pink',
          name_fio: 'Pink'
        },
        {
          name:'gray',
          name_fio: 'Gray'
        },
        {
          name:'olive',
          name_fio: 'Olive'
        },
      ],
      color_value: '',
      color_name: '',
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
    await this.fetch_product_t();
  },
  computed: mapGetters([ 'all_product_t']),

  methods: {
    ...mapActions(['fetch_product_t']),
    selectOption(option){
      console.log(option)
      this.productList[option.index].product_name = option.data.name;
      this.productList[option.index].product_id = option.data.id;
    },
    selectOptionColor(option){
      console.log(option)
      this.color_value = option.name;
      this.color_name = option.name_fio;
    },
    addProduct(){
      let item = {
        product_name: '',
        product_id: null,
        qty:0,
      }
      this.productList.push(item);
    },
    async fetchMounted(id){
      this.productList = [];
      try{
        const res = await fetch(this.$store.state.hostname + '/WaterOrders/getOrderFullInfoByid?order_id=' + id);
        const data = await res.json();
        console.log('this is by id')
        console.log(data)
        this.invoice_id = id;
        this.main_product_qty = 0;
        this.default_product_qty = 0;
        this.main_product_price = 0;
        this.main_product_id = null,
        this.main_order_id = null,
        this.order_summa= 0;
        this.order_item = [];
        this.client_name = data.client_name_str;
        this.client_id = data.client.id;
        this.address_id = data.address.id;
        this.note = data.note;
        this.order_date = data.order_date.slice(0,10);
        this.deleivered_user_auth_id = data.deleivered_user_auth_id;
        this.color_value = data.color_value || data.reserverd_note_3 || '';
        if(this.color_value) {
          const colorOption = this.color_list.find(c => c.name === this.color_value);
          if(colorOption) {
            this.color_name = colorOption.name_fio;
          }
        }
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
          console.log(data)
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
          "waterAuthid": localStorage.AuthId,
          "id" : 0,
        })
      };
      try{
        this.loading = true;
        const response = await fetch(this.$store.state.hostname + "/WaterChecks", requestOptions);
      
        this.loading = false;
        if(response.status == 201 || response.status == 200)
        {
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
      if(this.productList.length>0){
        for(let i=0; i<this.productList.length; i++){
          let tempInv = {
            waterProductid : this.productList[i].product_id,
            qty: this.productList[i].qty,
            real_qty: this.productList[i].qty,
            note: '',
            id: 0,
          }
          itemsList.push(tempInv);
        }
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
          "color_value": this.color_value,
          "reserverd_note_3": this.color_value,
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
      if(this.default_product_qty != this.main_product_qty || this.order_item.length>0){
        console.log('update qil orderni ishladi')
        await this.fetchUpdateOrder();
      }
      // await this.fetchBootle();
      // await this.fetchCheck();

      if(await this.AcceptOrder(this.invoice_id) && await this.fetchBootle() && await this.fetchCheck()){
        this.$emit('close')
      }
    }
  },
}
</script>

<style lang="scss" scoped>
// Modern, clean, minimal light theme with soft green accents
.update-payment-app {
  min-height: 100%;
  background: #f8fafb;
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen', 'Ubuntu', 'Cantarell', sans-serif;
  padding: 0;
  width: 100%;
  height: 100%;
  overflow-y: auto;
  overflow-x: hidden;
  position: relative;
}

.update-payment-content {
  width: 100%;
  min-height: 100%;
  margin: 0;
  background: #ffffff;
  border-radius: 0;
  box-shadow: none;
  border: none;
  overflow-y: auto;
  overflow-x: hidden;
  display: flex;
  flex-direction: column;
}

.payment-header {
  background: linear-gradient(135deg, #ffffff 0%, #f0fdf4 50%, #ecfdf5 100%);
  border-bottom: 1px solid #d1fae5;
  padding: 16px 20px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  
  .client-name {
    margin: 0;
    font-size: 18px;
    font-weight: 600;
    color: #10b981;
    letter-spacing: -0.02em;
  }
  
  .total-sum {
    margin: 0;
    font-size: 18px;
    font-weight: 700;
    color: #047857;
    letter-spacing: -0.02em;
  }
}

.payment-form {
  padding: 20px;
  flex: 1;
  overflow-y: auto;
}

.form-group {
  margin-bottom: 16px;
  position: relative;
  
  .form-label {
    display: block;
    font-size: 11px;
    font-weight: 600;
    color: #6b7280;
    margin-bottom: 6px;
    letter-spacing: -0.01em;
  }
  
  .form-input {
    width: 100%;
    padding: 10px 12px;
    border: 1.5px solid #e5e7eb;
    border-radius: 8px;
    font-size: 13px;
    font-weight: 500;
    outline: none;
    transition: all 0.2s ease;
    background: #ffffff;
    
    &:focus {
      border-color: #10b981;
      box-shadow: 0 0 0 3px rgba(16, 185, 129, 0.1);
    }
    
    &:disabled {
      background: #f9fafb;
      color: #6b7280;
      cursor: not-allowed;
    }
    
    &.text-right {
      text-align: right;
    }
  }
}

.order-items-section {
  margin: 24px 0;
  padding: 16px;
  background: #fafbfc;
  border-radius: 8px;
  border: 1px solid #f3f4f6;
  
  .section-title {
    font-size: 13px;
    font-weight: 600;
    color: #111827;
    margin-bottom: 12px;
    letter-spacing: -0.01em;
  }
  
  .order-items-list {
    display: flex;
    flex-direction: column;
    gap: 12px;
  }
  
  .order-item-row {
    display: grid;
    grid-template-columns: 2fr 1fr 1fr;
    gap: 12px;
    align-items: end;
    padding: 12px;
    background: #ffffff;
    border-radius: 8px;
    border: 1px solid #f3f4f6;
    
    .item-col {
      display: flex;
      flex-direction: column;
      
      .item-label {
        font-size: 10px;
        font-weight: 600;
        color: #6b7280;
        margin-bottom: 4px;
        letter-spacing: -0.01em;
      }
      
      .form-input {
        margin: 0;
      }
    }
  }
}

.additional-products-section {
  margin: 24px 0;
  padding: 16px;
  background: #fafbfc;
  border-radius: 8px;
  border: 1px solid #f3f4f6;
  
  .section-title {
    font-size: 13px;
    font-weight: 600;
    color: #111827;
    margin-bottom: 12px;
    letter-spacing: -0.01em;
  }
  
  .product-list {
    display: flex;
    flex-direction: column;
    gap: 12px;
  }
  
  .product-item {
    display: grid;
    grid-template-columns: 2fr 1fr;
    gap: 12px;
    align-items: end;
    padding: 12px;
    background: #ffffff;
    border-radius: 8px;
    border: 1px solid #f3f4f6;
    position: relative;
    
    .product-select,
    .product-qty {
      display: flex;
      flex-direction: column;
      position: relative;
      
      .product-label {
        font-size: 10px;
        font-weight: 600;
        color: #6b7280;
        margin-bottom: 4px;
        letter-spacing: -0.01em;
      }
    }
    
    .remove-btn {
      position: absolute;
      right: -30px;
      top: 1px;
      background: #ef4444;
      border: none;
      border-radius: 6px;
      width: 28px;
      height: 28px;
      display: flex;
      align-items: center;
      justify-content: center;
      cursor: pointer;
      transition: all 0.2s ease;
      
      &:hover {
        background: #dc2626;
        transform: scale(1.05);
      }
      
      mdb-icon {
        font-size: 12px;
      }
    }
  }
}

.balance-info {
  margin: 20px 0;
  padding: 12px;
  background: #fafbfc;
  border-radius: 8px;
  border: 1px solid #f3f4f6;
  
  .balance-item {
    display: flex;
    justify-content: space-between;
    align-items: center;
    padding: 8px 0;
    
    &.success {
      .balance-label,
      .balance-value {
        color: #10b981;
        font-weight: 600;
      }
    }
    
    &.danger {
      .balance-label,
      .balance-value {
        color: #ef4444;
        font-weight: 600;
      }
    }
    
    .balance-label {
      font-size: 12px;
      letter-spacing: -0.01em;
    }
    
    .balance-value {
      font-size: 13px;
      letter-spacing: -0.01em;
    }
  }
}

.payment-actions {
  display: flex;
  justify-content: flex-end;
  gap: 12px;
  padding: 16px 20px;
  background: #fafbfc;
  border-top: 1px solid #f3f4f6;
  
  .action-btn {
    font-size: 11px !important;
    padding: 8px 16px !important;
    border-radius: 8px;
    font-weight: 500;
    letter-spacing: -0.01em;
    transition: all 0.2s ease;
    
    i {
      font-size: 11px !important;
      margin-right: 6px !important;
    }
    
    &:hover {
      transform: translateY(-1px);
      box-shadow: 0 4px 12px rgba(16, 185, 129, 0.2);
    }
    
    &.primary-btn {
      background: #10b981 !important;
      border: none !important;
      
      &:hover {
        background: #059669 !important;
      }
    }
  }
}

// Responsive design
@media (max-width: 768px) {
  .update-payment-app {
    padding: 0;
    width: 100%;
    height: 100%;
  }
  
  .update-payment-content {
    border-radius: 0;
    height: 100%;
    overflow-y: auto;
  }
  
  .payment-header {
    padding: 12px 16px;
    flex-direction: column;
    align-items: flex-start;
    gap: 8px;
    position: sticky;
    top: 0;
    z-index: 10;
    background: linear-gradient(135deg, #ffffff 0%, #f0fdf4 50%, #ecfdf5 100%);
    
    .client-name,
    .total-sum {
      font-size: 16px;
    }
  }
  
  .payment-form {
    padding: 16px;
  }
  
  .order-item-row {
    grid-template-columns: 1fr;
    gap: 8px;
  }
  
  .product-item {
    grid-template-columns: 1fr;
    gap: 8px;
    
    .remove-btn {
      right: 8px;
      top: 8px;
    }
  }
  
  .payment-actions {
    flex-direction: column;
    padding: 12px 16px;
    position: sticky;
    bottom: 0;
    background: #fafbfc;
    border-top: 1px solid #f3f4f6;
    z-index: 10;
    
    .action-btn {
      width: 100%;
    }
  }
}
</style>