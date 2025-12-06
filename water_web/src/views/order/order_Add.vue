<template>
  <div class="order-add-app">
    <div class="order-header navbar_sticky">
      <div class="w-100 d-flex justify-content-between px-2">
        <div class="d-flex align-items-center">
          <div class="d-flex align-items-center mt-1">
            <mdb-icon icon="user-alt" class="text-primary mr-3 " style="font-size: 14px;"/>
            <span style="font-size: 12px;" class="mb-0 ml-1" >{{main_client_name}}</span>
          </div>
          <div class="d-flex align-items-center ml-5">
            <mdb-icon icon="map-marker-alt" class="text-success mr-3 mt-1" style="font-size: 14px;"/>
            <span style="font-size: 12px;" class="mb-0 ml-2 mt-1">{{client_addressList.length}}</span>
          </div>
          <div class="d-flex align-items-center ml-5 mt-1">
            <mdb-icon icon="clock" far class="text-success mr-3 " style="font-size: 14px;"/>
            <span style="font-size: 12px;" class="mb-0 ml-2">{{last_order_date}}</span>
          </div>
          <div class="d-flex align-items-center ml-5 mt-1">
            <img  src="../../assets/bootle.jpg" class="text-success mr-1 " alt="b" width="22" height="16">
            <span style="font-size: 13px; padding-top:2px;" class="mb-0 ml-0 text-danger">  {{ostatik_bootle}}</span>
          </div>

          <div class="d-flex align-items-center ml-5 mt-1" @click="showInfo = !showInfo">
            <mdb-icon icon="markdown" fab  class="text-success mr-3 " style="font-size: 14px; cursor:pointer;"/>
            <!-- <span style="font-size: 14px;" class="mt-2 mb-0 ml-2">{{last_order_date}}</span> -->
          </div>

          <div class="d-flex align-items-center ml-5 mt-1">
            <router-link to="/know_mark_in_map">
              <mdb-icon icon="street-view"   class="text-indigo mr-3 " style="font-size: 14px; cursor:pointer;"/>
            </router-link>
            <!-- <span style="font-size: 14px;" class="mt-2 mb-0 ml-2">{{last_order_date}}</span> -->
          </div>
          
          <div class="d-flex align-items-center ml-4 mt-1">
            <router-link to="/client_cancel_report" class="nav-menu-item cancel-item">
              <mdb-icon icon="times-circle" class="mr-2" />
              <span>Отмена</span>
            </router-link>
          </div>
          
          <div class="d-flex align-items-center ml-4 mt-1">
            <router-link to="/real_time_postavchik_stats" class="nav-menu-item stats-item">
              <mdb-icon icon="chart-line" class="mr-2" />
              <span>Поставщики</span>
            </router-link>
          </div>
          
          <div class="d-flex align-items-center ml-4 mt-1">
            <router-link to="/dashboard" class="nav-menu-item dashboard-item">
              <mdb-icon icon="chart-area" class="mr-2" />
              <span>Dashboard</span>
            </router-link>
          </div>
          <!-- know_mark_in_map -->
        </div>
        
        <div class="admin d-flex align-items-center" >
            <span style="font-size: 12px;" class="mb-0 ml-2">{{user_name}}</span>
          <!-- <span style="font-size: 14px;" class="mt-2 mb-0 ml-2">{{}}</span> -->
        </div>
        
      </div>
    </div>
    <loader v-if="loading"/>
    <div v-else class="order-content">
      <form>
        <!-- Search Section -->
        <div class="search-section">
          <div class="row pt-2">
            <div class="col-12 col-sm-12 col-md-6 col-lg-4">
              <input-search  @select="selectClient"  
                url="/WaterClients/getPaginationByName?page=0&size=100&fio="
                ref="search_client" :option="allClient.rows" icon="user">
              </input-search>
              <small class="p-0" style="margin-left:5px; font-size: 12px; top:-17px; color: gray; position:absolute;"  >
                {{$t('search_client')}}
              </small>
            </div>
            <div class="col-12 col-sm-12 col-md-6 col-lg-4">
              <input-searchArray  @select="selectPhone"
                url="/WaterClients/getPaginationSearchByPhoneNumberFromArray?page=0&size=100&phone_number="
                ref="search_client_phone"  icon="phone">
              </input-searchArray>
              <small class="p-0" style="margin-left:5px; font-size: 12px; top:-17px; color: gray; position:absolute;"  >
                {{$t('search_phone')}}
              </small>
            </div>
            <div class="col-12 col-sm-12 col-md-6 col-lg-4">
              <input-search-array-address @select="selectAdress"
                url="/WaterClients/getPaginationSearchByAddressFromArray?page=0&size=100&address="
                ref="search_client_address"  icon="map-marker-alt">
              </input-search-array-address>
              <small class="p-0" style="margin-left:5px; font-size: 12px; top:-17px; color: gray; position:absolute;"  >
                {{$t('search_address')}}
              </small>
            </div>
          </div>
          <div class="action-buttons mt-1">
            <router-link to="/map_order">
              <mdb-btn outline="orange" class="mdb_btn">
                <i class="fas fa-map-marked-alt mr-2"></i>
                {{$t('map')}}
              </mdb-btn>
            </router-link>
            <mdb-btn outline="success" @click="add_client" class="mdb_btn">
              <i class="fas fa-user-plus mr-2"></i>
              {{$t('Add_client')}}
            </mdb-btn>
          </div>
        </div>
        <div class="blue-gradient" v-show="false">
          <hr class="mt-0 "/>
        </div>


        <div class="main_Order " v-if="client_name">
          <div class="infoOfClient container-fluid" v-show="showInfo">
            <div class="d-flex justify-content-center">
              <h5>{{$t('client_info')}}</h5>
            </div>
            <loader-table v-if="loadingSimple" />
            <div v-else class="row">
              <div class="col-5 p-0">
                <div class="client_info">
                  <p class="text_content_border mb-2">{{$t('fio')}} <span>{{client_name}}</span></p>
                  <p v-for="(item,index) in client_phoneList" :key="index" class="text_content_border mb-2">{{$t('phoneNumber')}} <span>{{formatPhone(item.phone_number)}}</span></p>
                  <div class="text-right">
                    <span @click="orders_info" style="font-size:13.5px; cursor:pointer;" class="text-primary mr-3">{{$t('client_info')}}      </span>

                    <span @click="EditFunc" style="font-size:13.5px; cursor:pointer;" class="text-primary"> {{$t('edit')}}</span>
                   
                  </div>
                </div>
              </div>
              <div class="col-5">
                <div class="client_info " >
                  <div v-for="(item,ind) in client_addressList" :key="ind" style="position:relative;" @click="chooseAddress(ind, item)">
                    <p  class="text_content_border px-1 py-1 m-0" :class="{'bg_active': address_active == ind}" style="cursor:pointer; text-align: right;">{{$t('address')}} <span>{{item.address}}</span></p>
                    <div class="maps" v-show="address_active == ind" style="position:absolute; top:-7px; left:-35px;">
                      <img src="../../assets/mark.jpg" width="30" alt="">
                    </div>
                  </div>
                </div>
              </div>
              <div class="col-2 border rounded d-flex align-items-center justify-content-center">
                <div class="text-center">
                  <h6 class="font-weight-bold">{{$t('ostatka_bootle')}} ({{all_water_count}})</h6>
                  <div class="d-flex justify-content-center w-100 align-items-center">
                    <h4 class="m-0 p-0 pt-1 text-danger font-weight-bold">{{ostatik_bootle}} - </h4>
                    <img  src="../../assets/bootle.jpg" alt="b" width="50" height="45" class="bottle">
                  </div>
                   <!-- <div class="text-center mt-2 ml-3">
                    <span @click="EditFunc" style="font-size:13.5px; cursor:pointer;" class="text-primary">{{$t('edit')}}</span>
                  </div> -->
                </div>
              </div>
            </div>
            <div class="d-flex justify-content-center">
              <h5>{{$t('last_order')}}</h5>
            </div>
            <div class="table mt-0">
              <table class="w-100 tabled">
                <thead class="header_table" style="background: #66E4AD;">
                  <tr>
                    <th>№</th>
                    <th>{{$t('fio')}}</th>
                    <th class="text-center">{{$t('water_count')}}</th>
                    <th>{{$t('address')}}</th>
                    <th>{{$t('created_date_time')}}</th>
                    <th>{{$t('accepted_date')}}</th>
                    <th>{{$t('car_order')}}</th>
                    <!-- <th width="60">{{$t('Action')}}</th> -->
                  </tr>
                </thead>
                <tbody class="body_table">
                  <tr v-for="(item, index) in client_last_order" :key="index" >
                    <td>{{index+1}}</td>
                    <td class="font-weight-bold" style="font-size: 12px;">{{item.client_name_str}}</td>
                    <td class="text-center text-primary font-weight-bold" style="font-size: 12px;">{{item.water_count}} / {{item.reserverd_numeric_id_1}}</td>
                    <td>{{item.address.address}}</td>
                    <td>{{item.created_date_time.substr(0,10)}} ( {{item.created_date_time_str.substr(0,5)}} )</td>
                    <td>
                      <mdb-badge v-if="item.accepted_status" style="padding: 2px 8px;" pill color="success">Доставлено</mdb-badge>
                      <mdb-badge v-else style="padding: 2px 8px;" pill color="danger">Не доставлено</mdb-badge>
                    </td>
                    <td > 
                      <span v-if="item.deleivered_user_auth_id">{{item.deleivered_user_auth.user.fio}}</span>
                    </td>
                    <!-- <td class="m-0 p-0">
                      <mdb-btn class="mr-1 ml-0 mt-0 mt-1 btn-acp"  style="font-size: 8px; width:80px; padding: 5px;"  @click="showOrder(item)" 
                        size="sm">{{$t('accept')}}
                      </mdb-btn>
                    </td> -->
                  </tr>
                </tbody>
              </table>
            </div>
          </div>
          
      

          <!-- Order Form Section -->
          <div class="order-form-section">
            <div class="d-flex justify-content-between align-items-center mb-4">
              <h5 class="mb-0">{{$t('order')}}:  <span class="text-primary">{{main_client_name}}</span></h5>
              <div class="d-flex gap-2">
                <mdb-btn outline="danger" @click="cancel_client_order" class="mdb_btn">
                  <i class="fas fa-times mr-2"></i>
                  {{$t('cancel')}}
                </mdb-btn>
                <mdb-btn outline="primary" @click="add_product" class="mdb_btn">
                  <i class="fas fa-plus mr-2"></i>
                  {{$t('Add_product')}}
                </mdb-btn>
              </div>
            </div>
            <div class="row">
              <div class="col-5">
                <div class="row">
                  <div class="col-12">
                    <div class="d-flex align-items-center mt-1">
                      <input-img disabled  v-model="main_client_address" type="text"  error="wrong" success="right" picture="mark.jpg" />
                      <small class="p-0" style="margin-left:5px; font-size: 12px; top:-17px; color: gray; position:absolute;"  >
                        {{$t('address')}}
                      </small>
                    </div>
                  </div>
                  <div class="col-6">
                    <div class="d-flex align-items-center mt-4">
                      <input-img valid  v-model="order_qty" @keyup="submit"  type="number" ref="order_qty" error="wrong" success="right" picture="bootle.jpg" />
                      <small class="p-0" style="margin-left:5px; font-size: 12px; top:6px; color: gray; position:absolute;">
                        {{main_product_name}}
                      </small>
                    </div>
                  </div>
                  <div class="col-6 pl-0">
                    <div class="d-flex align-items-center mt-4">
                      <input-img   v-model="order_date" @keyup="submit" type="date"  error="wrong" success="right" picture="date.jpg" />
                      <small class="p-0" style="margin-left:5px; font-size: 12px; top:6px; color: gray; position:absolute;"  >
                        {{$t('date')}}
                      </small>
                      <small class="invalid-text mt-5" style="margin-left:5px; "  v-if="$v.order_date.$dirty && !$v.order_date.required" >
                        {{$t('name_invalid_text')}}
                      </small>
                    </div>
                  </div>
                  <div class="col-12 mt-3">
                    <div class="d-flex align-items-center mt-1">
                      <text-area rows="2" class="w-100"  v-model="note"  validate error="wrong" success="right"/>
                      <small class="p-0" style="margin-left:5px; font-size: 12px; top:-12px; color: gray; position:absolute;"  >
                        {{$t('note')}}
                      </small>
                    </div>
                  </div>
                  <div class="col-12 mt-3">
                    <div class="d-flex align-items-center mt-1">
                      <colorSelect
                        size="sm"
                        :options="color_list"
                        @select="selectOptionColor"
                        :selected="color_value"
                        :color_name="color_name"
                        :label="$t('province')"
                      />
                      <small class="p-0" style="margin-left:5px; font-size: 12px; top:-17px; color: gray; position:absolute;"  >
                        {{$t('select_color')}}
                      </small>
                    </div>
                  </div>
                </div>
              </div>

              <div class="col-5">
                <div class="product">
                  <div class="row " v-for="(item,i) in productList" :key="i" :class="{'mt-1': i==0, 'mt-3': i>0}">
                    <div class="col-9">
                      <erpSelect
                        size="sm"
                        :options="all_product_t.rows"
                        @select="selectOption"
                        :selected="item.product_name"
                        :label="$t('province')"
                        :index="i"
                      />
                      <small class="p-0" style="margin-left:5px; font-size: 12px; top:-17px; color: gray; position:absolute;"  >
                        {{$t('select_product')}}
                      </small>
                      <!-- <small v-if="$v.comp_name.$dirty && comp_id == null" class="invalid-text mt-0 ml-2" >
                        {{$t('select_item')}}
                      </small> -->
                    </div>
                    <div class="col-3">
                      <input-img valid  v-model="item.qty" style="height:32px;" error="wrong" success="right" icon="" type="number"/>
                      <small class="p-0" style="margin-left:5px; font-size: 12px; top:-17px; color: gray; position:absolute;"  >
                        {{$t('qty')}}
                      </small>
                      <div class="added bg-danger rounded"  style="position:absolute; right:-30px; top:1px;" @click="productList.splice(i,1)">
                        <mdb-icon icon="trash p-2 text-white" style="font-size:12px; cursor:pointer;"/>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
          
        </div>

                           
        <div class="" v-if="client_name">
          
          <div class="submit-section">
            <div class="d-flex justify-content-end gap-2">
              <mdb-btn @click="submit_continue" color="primary" class="mdb_btn submit-btn">
                <i class="fas fa-check-circle mr-2"></i>
                {{$t('Add_and_continue')}}
              </mdb-btn>
              <mdb-btn v-show="false" color="success" @click="submit" class="mdb_btn">
                <mdb-icon/>{{$t('add')}}
              </mdb-btn>
            </div>
          </div>
        </div>
      </form>

      <modal-train  :show="clientAdd_show" headerbackColor="white"  titlecolor="black" :title="$t('Add_client')" 
        @close="clientAdd_show = false" width="80%">
          <template v-slot:body>
            <addpupil @close="clientAdd_show = false" ref="clientRef" @update="updateClient" :client_id="edit_client_id"></addpupil>
          </template>
      </modal-train>

       <modal-train  :show="orders_info_show" headerbackColor="white"  titlecolor="black" :title="$t('client_info')" 
        @close="orders_info_show = false" width="80%">
          <template v-slot:body>
            <orders_info_ @close="orders_info_show = false" ref="clientAllInfo"  :client_id="edit_client_id" ></orders_info_>
          </template>
      </modal-train>
    </div>
    <mdb-modal :show="confirm" @close="confirm = false" size="sm" class="text-center" danger>
      <mdb-modal-header center :close="false">
        <p class="heading">{{$t('Are_you_sure')}}</p>
      </mdb-modal-header>
      <mdb-modal-body>
        <span>Klient boshqa zakaz qilmaydimi? Klientni rostdan ham bekor qilmoqchimisz?</span>
      </mdb-modal-body>
      <mdb-modal-footer center>
        <mdb-btn outline="success" @click="promise">{{$t('Yes')}}</mdb-btn>
        <mdb-btn color="danger" @click="confirm = false">{{$t('cancel')}}</mdb-btn>
      </mdb-modal-footer>
    </mdb-modal>

    <mdb-modal :show="address_show" @close="address_show = false" size="md" class="text-center" danger>
      <mdb-modal-header center :close="false">
        <p class="heading">{{$t('Are_you_sure')}}</p>
      </mdb-modal-header>
      <mdb-modal-body>
        <span>Klientda {{client_addressList.length}} ta address bor. 
        <span class="text-primary font-weight-bold">{{main_client_address}}</span> shu addressga zakaz qilmoqchimisz ?</span>
      </mdb-modal-body>
      <mdb-modal-footer center>
        <mdb-btn outline="success" @click="promiseSubmit">{{$t('Yes')}}</mdb-btn>
        <mdb-btn color="danger" @click="address_show = false">{{$t('cancel')}}</mdb-btn>
      </mdb-modal-footer>
    </mdb-modal>
    <massage_box :hide="modal_status" :detail_info="modal_info"
      :m_text="$t('Failed_to_add')" @to_hide_modal="modal_status= false"/>

      <Toast ref="message"></Toast>
  </div>
</template>

<script>
// import md5 from 'js-md5'
import { mdbInput, mdbRow, mdbCol, mdbIcon, mdbBtn, mdbModal, mdbModalHeader, mdbModalBody, mdbModalFooter,mdbBadge,mdbBtnGroup, mdbDropdown, mdbDropdownMenu, mdbDropdownItem, } from "mdbvue"
import { required } from 'vuelidate/lib/validators'
import {mapActions,mapGetters} from 'vuex'
import InputIcon from '../../components/inputIcon.vue';
import InputImg from '../../components/inputImg.vue';
import TextArea from '../../components/textArea.vue';
import ErpSelectIcon from '../../components/erpSelectIcon.vue'
import inputSearch from '../../components/inputSearch.vue'
import inputSearchArray from '../../components/inputSearchArray.vue'
import addpupil from '../client/client_ad.vue'
import orders_info_ from '../client/client_orders_info.vue'
import erpSelect from "../../components/erpSelectIndex";
import colorSelect from "../../components/colorSelect";
import InputSearchArrayAddress from '../../components/inputSearchArrayAddress.vue';

export default {
 
  
  data(){
    return{
      modal_info: '',
      modal_status: false,
      loading: false,
      id: this.$route.params.id,
      loadingSimple: false,
      user_name: localStorage.UserName,
      confirm: false,
      address_show: false,

      showInfo: true,
      name: 'w',
      clientAdd_show: false,
      orders_info_show: false,
      client_name: '',
      client_phone: '',
      client_address: '',
      client_bottle: '',
      main_product_name: '',
      main_product_id: null,
      order_qty: 0,
      productList: [
        
      ],
      client_phoneList:[

      ],
      client_addressList: [],
      main_client_name: '',
      main_client_id: null,
      main_client_address: '',
      address_active: -1,
      note: '',

      order_date: '',
      last_order_date: '',
      water_address_id: null,

      edit_client_id: null,

      ostatik_bootle: 0,
      all_water_count : 0,

      client_last_order: [],
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
      reserverd_note_3: '',
      color_value: '',
      color_name: '',
    }
  },
  components: {
    mdbInput, mdbRow, mdbCol, mdbIcon, mdbBtn,
    InputIcon, inputSearchArray,InputSearchArrayAddress,
    TextArea, ErpSelectIcon, inputSearch, addpupil, orders_info_, erpSelect, InputImg, colorSelect,
    mdbModal, mdbModalHeader, mdbModalBody, mdbModalFooter,mdbBadge,
    mdbBtnGroup, mdbDropdown, mdbDropdownMenu, mdbDropdownItem,
  },
  validations: {
      name: {required,},
      order_date: {required}
    },
     props :
    {
      props_name : {
        type : String,
        default : ''
      },
      
    },
    async created()
    {
      
      if(this.id > 0)
      {
        const res = await fetch(this.$store.state.hostname + '/WaterClients/' + this.id);
        const data = await res.json();
        console.log('this is by id')
        console.log(data)
        this.name = data.fio;
        
        var y = data.phone_number.slice(4,13).replace(/\D/g, '').match(/(\d{0,2})(\d{0,3})(\d{0,4})/);
        this.phoneNumber = !y[2] ? y[1] : '(' + y[1] + ') ' + y[2] + (y[3] ? '-' + y[3] : '');

        var w = data.phone_number1.slice(4,13).replace(/\D/g, '').match(/(\d{0,2})(\d{0,3})(\d{0,4})/);
        this.phoneNumber1 = !w[2] ? w[1] : '(' + w[1] + ') ' + w[2] + (w[3] ? '-' + w[3] : '');

     
        this.adreess = data.adreess;
        this.home_number = data.home_number;
        // this.level_name = data.client_type.name;
        // this.level_id = data.client_type.id;
       
        this.home_code = data.home_code;
        this.padez = data.padez;
        
      }

    },
    async mounted(){
      await this.fetchClient();
      await this.fetch_product_t();
      await this.fetchLevel();
      await this.fetch_main_product();
      this.client_list = this.allClient.rows;
      this.getNewDate();
      this.cls_wnd();
    },
    computed: mapGetters(['all_district_t', 'all_contragent_t','allDepartment', 'allLevel', 'allClient', 'all_product_t']),

  methods:{
    ...mapActions(['fetch_district_t', 'fetchClient', 'fetch_contragent_t','fetchDepartment', 'fetchLevel', 'fetchClient', 'fetch_product_t', 'fetchOrder_list']),
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
    writeNol(){
      if(this.order_qty == 0 || this.order_qty == '0'){
        this.order_qty = null
      }
    },
    selectOptionColor(option){
      console.log(option)
      this.color_value = option.name;
      this.color_name = option.name_fio;
    },
    getNewDate(){
      let nowDate = new Date();
      let next_day = nowDate.setDate(nowDate.getDate() + 1);
      var next_days_date = new Date(next_day).toISOString();
      this.order_date = next_days_date.slice(0,10);
    },  
    focusOrderQty(){
      this.$refs.order_qty.focus();
      this.order_qty = null;
    },
    focusClientSearch(){
      this.$refs.search_client.focus();
    },
    
    EditFunc(){
      this.$refs.clientRef.updateMounted(this.main_client_id);
      this.edit_client_id = this.main_client_id;
      this.clientAdd_show = true;
    },
    orders_info(){
      
      this.$refs.clientAllInfo.updateMounted(this.main_client_id);
      this.edit_client_id = this.main_client_id;
      this.orders_info_show = true;
    },
    add_client(){
      this.$refs.clientRef.updateMounted(0);
      
      this.clientAdd_show = true;
      this.edit_client_id = 0;
    },
    async updateClient(option){
      this.address_active = 0;
      this.main_client_name = option.fio;
      this.main_client_id = option.id;
      this.client_name = option.fio;
      this.main_client_address = option.first_address_name;
      this.client_phoneList = option.phone_numbers_list;
      this.client_addressList = option.addresses;
      this.water_address_id = option.first_address.id;
      await this.fetchOstatka();
      await this.fetchClientLastOrder(this.main_client_id);
      this.focusOrderQty();

    },
    async selectClient(option){
      console.log(option)
      this.address_active = 0;
      this.main_client_name = option.fio;
      this.main_client_id = option.id;
      this.client_name = option.fio;
      this.main_client_address = option.first_address_name;
      this.client_phoneList = option.phone_numbers_list;
      this.client_addressList = option.addresses;
      this.water_address_id = option.first_address.id;
      await this.fetchOstatka();
      await this.fetchClientLastOrder(this.main_client_id);
      this.focusOrderQty();
    },
    async selectPhone(option){
      console.log(option)
      this.address_active = 0;
      this.main_client_name = option.fio;
      this.main_client_id = option.id;
      this.client_name = option.fio;
      this.main_client_address = option.first_address_name;
      this.client_phoneList = option.phone_numbers_list;
      this.client_addressList = option.addresses;
      this.water_address_id = option.first_address.id;
      await this.fetchOstatka();
      await this.fetchClientLastOrder(this.main_client_id);
      this.focusOrderQty();

    },
    async selectAdress(option){
      this.address_active = 0;
      console.log(option)
      this.main_client_name = option.fio;
      this.main_client_id = option.id;
      this.client_name = option.fio;
      this.main_client_address = option.first_address_name;
      this.client_phoneList = option.phone_numbers_list;
      this.client_addressList = option.addresses;
      this.water_address_id = option.first_address.id;
      await this.fetchOstatka();
      await this.fetchClientLastOrder(this.main_client_id);
      this.focusOrderQty();
    },
    async fetchOstatka(){
      try{
        this.loadingSimple = true;
        const response = await fetch(this.$store.state.hostname + "/WaterClientBottleInfoes/getPaginationClientIdAndAddressId?page=0&size=1&client_id=" + this.main_client_id + '&address_id=' + this.water_address_id);
        const data = await response.json();
        this.loadingSimple = false;
        // this.clientAdd_show = false;
        console.log(data)
        if(data.items_list.length>0){
          this.ostatik_bootle = data.items_list[0].bottle_count
          this.all_water_count =  data.items_list[0].bottle_count_real
        }
        else{
          this.ostatik_bootle = 0
        }
      }
      catch{
        this.ostatik_bootle = 0
      }
    },
    
    formatDateRu(dateString) {
      if (!dateString) return '';
      const date = new Date(dateString);
      const months = [
        'января', 'февраля', 'марта', 'апреля', 'мая', 'июня',
        'июля', 'августа', 'сентября', 'октября', 'ноября', 'декабря'
      ];
      const day = date.getDate();
      const month = months[date.getMonth()];
      const year = date.getFullYear();
      return `${day} ${month} ${year}`;
    },
    async fetchClientLastOrder(client_id){
      try{
        // this.loadingSimple = true;
        const response = await fetch(this.$store.state.hostname + "/WaterOrders/getLastOrderTwoOrdersFullInfoByid?client_id=" + client_id);
        const data = await response.json();
        console.log('fetch two order2')
        // this.loadingSimple = false;
        if(data.length>0){
          console.log(data)
          this.last_order_date = this.formatDateRu(data[0].order_date);
          this.client_last_order = data;  
        }
        else{
          this.client_last_order = [];
          this.last_order_date = '';
        }
      }
      catch{
          this.client_last_order = [];
          this.last_order_date = '';
      }
    },
    add_product(){
      let item = {
        product_name: '',
        product_id: null,
        qty:0,
      }
      this.productList.push(item);
    },
    selectOption(option){
      console.log(option)
      this.productList[option.index].product_name = option.data.name;
      this.productList[option.index].product_id = option.data.id;
    },
    async fetch_main_product(){
      const response = await fetch(this.$store.state.hostname + "/WaterProducts/getPaginationMainProduct?page=0&size=1");
      const data = await response.json();
      console.log(data);
      this.main_product_name = data.items_list[0].name;
      this.main_product_id = data.items_list[0].id;
    },
    async chooseAddress(i, data){
      console.log(data)
      this.address_active = i;
      this.main_client_address = data.address;
      this.water_address_id = data.id;
      await this.fetchOstatka();
      await this.fetchClientLastOrder(this.main_client_id);
    },
    cls_wnd(){
      this.order_qty = 0;
      this.productList = [];
      this.focusClientSearch();
      // this.getNewDate();
      this.note = '';
      this.reserverd_note_3 = '';
      this.color_value = '';
      this.color_name = '';
    },
    save_data :  async function(){
      var itemsList = [];
      if(this.order_qty != 0){
        let temp = {
          waterProductid : this.main_product_id,
          qty: this.order_qty,
          real_qty: this.order_qty,
          note: this.note,
          id: 0,
        }
        itemsList.push(temp);
      }
      for(let i=0; i< this.productList.length; i++){
        console.log(this.productList[i])
        let tempInv = {
          waterProductid : this.productList[i].product_id,
          qty: this.productList[i].qty,
          real_qty: this.productList[i].qty,
          note: '',
          id: 0,
        }
        itemsList.push(tempInv);
      }
      if(this.$v.$invalid)
        {
          this.$v.$touch();
          this.$refs.message.warning('please_fill')
          return false;
        }
      if(itemsList.length == 0){
        this.$refs.message.error('order_not_found');
        return;
      }
        else{ 
          // this.alert_danger = false;
          const requestOptions = {
            method : "POST",
            headers: { "Content-Type" : "application/json" },
            body: JSON.stringify({
              "order_date" : this.order_date + 'T12:00:00.000Z',
              "waterClientid": this.main_client_id,
              "water_count": this.order_qty,
              "waterClientAddressid": this.water_address_id,
              "note": this.note,
              "items": itemsList,
              "color_value": this.color_value,
              "reserverd_note_3": this.color_value,
              "id" : this.id,
            })
          };
          try{
            this.loading = true;
            console.log('requestOptions.body')
            console.log(requestOptions.body)
            const response = await fetch(this.$store.state.hostname + "/WaterOrders", requestOptions);
            const data = await response.text();
            console.log(response)
            this.loading = false;
            if(response.status == 201 || response.status == 200)
            {
              this.$refs.message.success('Added_successfully')
              await this.fetchOrder_list();
              await this.fetchClientLastOrder(this.main_client_id);
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
          }
        }
    },
    async submit()
      {
        if(this.client_addressList.length>1){
          this.address_show = true;
          return;
        }
        else{ 
          await this.promiseSubmit();
        }
        // if(await this.save_data())
        // {
        //   // this.$store.state.alert = true;
        //   // this.$router.push("/order");
        //   this.cls_wnd();
        // }

      },
    async submit_continue()
      {
        if(this.client_addressList.length>1){
          this.address_show = true;
        }
        else{
          await this.promiseSubmit();
        }
      },  
    async promiseSubmit(){
        if(await this.save_data())
        {
          if(this.id != 0){
            this.$router.push('/order_add/0')
            this.cls_wnd();
          }
          else{
            this.cls_wnd();
          }
          this.address_show = false;
        }
      },
      async cancel_client_order(){
        this.confirm = true;
        console.log('test')
      },
      async promise(){
      var itemsList = [];
      if(this.$v.$invalid)
        {
          this.$v.$touch();
          this.$refs.message.warning('please_fill')
          return false;
        }
        // this.alert_danger = false;
        const requestOptions = {
          method : "POST",
          headers: { "Content-Type" : "application/json" },
          body: JSON.stringify({
            "order_date" : this.order_date + 'T12:00:00.000Z',
            "waterClientid": this.main_client_id,
            "water_count": 0,
            "waterClientAddressid": this.water_address_id,
            "note": "Otmen -- " + this.note,
            "items": itemsList,
            "reserverd_note_3": "orange",
            "reserverd_note_2" : 5, // otmen bugnini bilish uchun
            "id" : this.id,
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
            this.$refs.message.success('Added_successfully')
            await this.fetchOrder_list();
            this.confirm = false;
            this.cls_wnd();
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
        }
      }
  }
}
</script>

<style lang="scss" scoped>
// Modern, clean, minimal light theme with soft green accents
.order-add-app {
  min-height: 100vh;
  background: #f8fafb;
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen', 'Ubuntu', 'Cantarell', sans-serif;
  overflow-y: auto;
  overflow-x: hidden;
}

.order-header {
  background: linear-gradient(135deg, #ffffff 0%, #f0fdf4 50%, #ecfdf5 100%);
  border-bottom: 1px solid #d1fae5;
  box-shadow: 0 1px 2px rgba(16, 185, 129, 0.08);
  padding: 8px 16px;
  z-index: 100;
  min-height: 48px;
  
  .w-100 {
    display: flex;
    justify-content: space-between;
    align-items: center;
  }
  
  .text-primary {
    color: #10b981 !important;
  }
  
  .text-success {
    color: #10b981 !important;
  }
  
  .text-danger {
    color: #ef4444 !important;
  }
  
  .text-indigo {
    color: #6366f1 !important;
  }
  
  span {
    font-size: 12px;
    font-weight: 500;
    color: #374151;
    letter-spacing: -0.01em;
  }
  
  mdb-icon {
    color: #10b981;
    font-size: 14px !important;
  }
  
  img {
    width: 18px !important;
    height: 14px !important;
  }
  
  .ml-5 {
    margin-left: 16px !important;
  }
  
  .mr-2 {
    margin-right: 6px !important;
  }
  
  .mt-2 {
    margin-top: 0 !important;
  }
  
  // Nav menu items styling
  .nav-menu-item {
    display: flex;
    align-items: center;
    padding: 6px 12px;
    border-radius: 8px;
    text-decoration: none;
    transition: all 0.2s ease;
    font-size: 12px;
    font-weight: 500;
    letter-spacing: -0.01em;
    border: 1.5px solid transparent;
    
    mdb-icon {
      font-size: 13px !important;
      transition: all 0.2s ease;
    }
    
    span {
      font-size: 12px;
      font-weight: 500;
      margin: 0;
      transition: all 0.2s ease;
    }
    
    &:hover {
      transform: translateY(-1px);
      box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
    }
    
    &.cancel-item {
      background: linear-gradient(135deg, #fef2f2 0%, #fee2e2 100%);
      border-color: #fecaca;
      color: #dc2626;
      
      mdb-icon {
        color: #dc2626 !important;
      }
      
      span {
        color: #dc2626;
      }
      
      &:hover {
        background: linear-gradient(135deg, #fee2e2 0%, #fecaca 100%);
        border-color: #f87171;
        box-shadow: 0 2px 8px rgba(220, 38, 38, 0.2);
      }
    }
    
    &.stats-item {
      background: linear-gradient(135deg, #eff6ff 0%, #dbeafe 100%);
      border-color: #bfdbfe;
      color: #2563eb;
      
      mdb-icon {
        color: #2563eb !important;
      }
      
      span {
        color: #2563eb;
      }
      
      &:hover {
        background: linear-gradient(135deg, #dbeafe 0%, #bfdbfe 100%);
        border-color: #93c5fd;
        box-shadow: 0 2px 8px rgba(37, 99, 235, 0.2);
      }
    }
    
    &.dashboard-item {
      background: linear-gradient(135deg, #f0fdf4 0%, #dcfce7 100%);
      border-color: #bbf7d0;
      color: #16a34a;
      
      mdb-icon {
        color: #16a34a !important;
      }
      
      span {
        color: #16a34a;
      }
      
      &:hover {
        background: linear-gradient(135deg, #dcfce7 0%, #bbf7d0 100%);
        border-color: #86efac;
        box-shadow: 0 2px 8px rgba(22, 163, 74, 0.2);
      }
    }
  }
  
  .ml-4 {
    margin-left: 12px !important;
  }
}

.order-content {
  padding: 16px;
  width: 100%;
  margin: 0;
  
  form {
    .row {
      margin: 0;
    }
  }
}

// Search Section - Premium Card Design
.search-section {
  background: #ffffff;
  border-radius: 12px;
  padding: 16px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
  margin-bottom: 16px;
  border: 1px solid #f0f0f0;
  
  .row {
    margin: 0;
  }
  
  .col-12, .col-sm-12, .col-md-6, .col-lg-4 {
    padding: 6px;
  }
  
  small {
    color: #6b7280;
    font-size: 10px;
    font-weight: 500;
    letter-spacing: -0.01em;
  }
}

.action-buttons {
  display: flex;
  justify-content: flex-end;
  gap: 6px;
  margin-top: 12px;
  
  .mdb_btn {
    font-size: 11px !important;
    padding: 5px 12px !important;
    border-radius: 8px;
    font-weight: 500;
    transition: all 0.2s ease;
    letter-spacing: -0.01em;
    border-width: 1.5px;
    
    i {
      font-size: 11px !important;
      margin-right: 4px !important;
    }
    
    &:hover {
      transform: translateY(-1px);
      box-shadow: 0 4px 12px rgba(16, 185, 129, 0.15);
    }
  }
}

// Client Info Section - Premium Card
.main_Order {
  .infoOfClient {
    background: #ffffff;
    border-radius: 12px;
    padding: 16px;
    box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
    margin-bottom: 16px;
    border: 1px solid #f0f0f0;
    
    h5 {
      color: #111827;
      font-weight: 600;
      margin-bottom: 12px;
      font-size: 14px;
      letter-spacing: -0.02em;
    }
    
    .client_info {
      width: 100%;
      padding: 10px 12px;
      
      .text_content_border {
        display: flex;
        justify-content: space-between;
        border-bottom: 1px solid #f3f4f6;
        color: #6b7280;
        font-size: 11px;
        padding: 6px 0;
        letter-spacing: -0.01em;
        
        span {
          font-weight: 500;
          color: #111827;
        }
      }
    }
    
    .bg_active {
      background: #ecfdf5;
      border: 1.5px solid #10b981;
      border-radius: 8px;
      padding: 6px 10px !important;
    }
    
    .border.rounded {
      border: 1px solid #f3f4f6 !important;
      border-radius: 10px !important;
      background: #fafbfc;
      padding: 10px;
      
      h6 {
        color: #374151;
        font-weight: 600;
        font-size: 10px;
        letter-spacing: -0.01em;
      }
      
      h4 {
        color: #ef4444;
        font-weight: 600;
        font-size: 14px;
        letter-spacing: -0.02em;
      }
    }
  }
  
  .table {
    background: #ffffff;
    border-radius: 12px;
    padding: 12px;
    box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
    overflow-x: auto;
    margin-top: 12px;
    border: 1px solid #f0f0f0;
    
    .tabled {
      width: 100%;
      border-collapse: separate;
      border-spacing: 0;
      
      thead {
        background: #10b981;
        color: white;
        border-radius: 8px;
        
        th {
          padding: 8px 10px;
          font-size: 10px;
          font-weight: 600;
          text-align: left;
          white-space: nowrap;
          letter-spacing: -0.01em;
          
          &:first-child {
            border-top-left-radius: 8px;
          }
          
          &:last-child {
            border-top-right-radius: 8px;
          }
        }
      }
      
      tbody {
        tr {
          border-bottom: 1px solid #f3f4f6;
          transition: all 0.15s ease;
          
          &:hover {
            background: #f9fafb;
          }
          
          &:last-child {
            border-bottom: none;
          }
          
          td {
            padding: 8px 10px;
            font-size: 10px;
            color: #374151;
            letter-spacing: -0.01em;
          }
        }
      }
    }
  }
}

// Order Form Section - Premium Card
.order-form-section {
  background: #ffffff;
  border-radius: 12px;
  padding: 16px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
  margin-top: 16px;
  border: 1px solid #f0f0f0;
  
  h5 {
    color: #111827;
    font-weight: 600;
    margin-bottom: 12px;
    font-size: 14px;
    letter-spacing: -0.02em;
    
    .text-primary {
      color: #10b981 !important;
    }
  }
  
  .product {
    .row {
      margin: 0;
      
      .col-9, .col-3 {
        padding: 4px;
      }
    }
  }
  
  small {
    color: #6b7280;
    font-size: 10px;
    font-weight: 500;
    letter-spacing: -0.01em;
  }
  
  .mdb_btn {
    font-size: 11px !important;
    padding: 5px 12px !important;
    border-radius: 8px;
    
    i {
      font-size: 11px !important;
      margin-right: 4px !important;
    }
  }
}

.blue-gradient {
  margin: 16px 0;
  hr {
    border: none;
    border-top: 1px solid #e5e7eb;
    margin: 0;
  }
}

.download {
  width: 35%;
  height: 33px;
  border: 0.5px solid #4285F4;
  border-radius: 4px;
  margin-top: 7px;
  display: flex;
  align-items: center;
  color: gray; 
  font-size: 13px;
  cursor: pointer;
  
  span {
    margin-left: 10px;
  }
}

.free_day {
}

.week_day_item {
  cursor: pointer;
}

.item_circle {
  margin-right: 10px;
  width: 20px;
  height: 20px;
  border-radius: 50%;
  background: #eee;
}

.into_circle {
  width: 15px;
  height: 15px;
  border-radius: 50%;
  background: rgb(255, 255, 255);
}

.into_circle_active {
  width: 20px;
  height: 20px;
  border-radius: 50%;
  background: #0fe22b;
}

.user_illSendPatient {
  overflow: hidden;
  overflow-y: scroll;
  height: 75vh;
  width: 100%;
  
  .item {
    .user_photo {
      background-color: #fff;
      border-radius: 50%;
      width: 45px;
      height: 45px;
      overflow: hidden;
    }
    
    width: 100%;
    box-shadow: 2px 2px 8px rgb(224, 224, 224), -1px -1px 2px rgb(224, 224, 224);
    height: 62px;
    margin: 5px auto;
    display: flex;
    align-items: center;
    justify-content: space-between;
    transition: all 0.5s ease-in-out;
    
    .rang {
      color: rgb(66, 167, 255);
      font-weight: bold;
    }
    
    &:hover {
      cursor: pointer;
      box-shadow: 2px 2px 5px rgb(224, 224, 224);
      background-color: rgb(215, 242, 255);
      transform: translate(6px, 0px);
      transition: all 0.1s ease-in-out;
    }
  }
}

.activeUser {
  cursor: pointer;
  box-shadow: 2px 2px 5px rgb(224, 224, 224);
  background-color: rgb(179, 230, 255);
  transform: translate(6px, 0px);
  transition: all 0.1s ease-in-out;
}

.client_full_info {
  background: rgb(245, 247, 248);
}

.border_dashed {
  border-bottom: 1px dashed #ddd;
}

.bottle {
  width: 50px;
  height: 45px;
  display: inline-block;
}

@keyframes swing {
  0%   { transform: rotate(0deg); }
  25%  { transform: rotate(15deg); }
  50%  { transform: rotate(-15deg); }
  75%  { transform: rotate(15deg); }
  100% { transform: rotate(0deg); }
}

.submit-section {
  background: #ffffff;
  border-radius: 12px;
  padding: 12px 16px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
  margin-top: 16px;
  border: 1px solid #f0f0f0;
  
  .submit-btn {
    min-width: 140px;
    font-size: 11px !important;
    padding: 6px 14px !important;
    border-radius: 8px;
    font-weight: 500;
    box-shadow: 0 2px 8px rgba(16, 185, 129, 0.2);
    transition: all 0.2s ease;
    letter-spacing: -0.01em;
    background: #10b981 !important;
    border: none !important;
    
    i {
      font-size: 11px !important;
      margin-right: 4px !important;
    }
    
    &:hover {
      transform: translateY(-1px);
      box-shadow: 0 4px 12px rgba(16, 185, 129, 0.3);
      background: #059669 !important;
    }
  }
}

.gap-2 {
  gap: 6px;
}

// Additional styling for better visual consistency
.text-primary {
  color: #10b981 !important;
}

.text-success {
  color: #10b981 !important;
}

.text-danger {
  color: #ef4444 !important;
}

// Badge styling
mdb-badge {
  border-radius: 8px !important;
  font-weight: 500 !important;
  font-size: 11px !important;
  letter-spacing: -0.01em;
  padding: 4px 10px !important;
}

// Button styling improvements
.mdb_btn {
  border-radius: 8px !important;
  font-weight: 500 !important;
  letter-spacing: -0.01em;
  transition: all 0.2s ease;
  font-size: 11px !important;
  padding: 5px 12px !important;
  
  i {
    font-size: 11px !important;
  }
  
  &:hover {
    transform: translateY(-1px);
  }
}

// Desktop only - no mobile styles needed
@media (min-width: 768px) {
  .order-content {
    padding: 16px;
  }
  
  .order-header {
    padding: 8px 16px;
  }
  
  .search-section {
    padding: 16px;
  }
  
  .order-form-section {
    padding: 16px;
  }
  
  .infoOfClient {
    padding: 16px !important;
  }
  
  .table {
    padding: 12px !important;
  }
}
</style>