<template>

  <div class="d_table">
    <loader v-if="loading"/>

      <div class="controls-section">
        <mdb-row>
          <mdb-col  class="col-12 col-sm-12 col-md-4 col-lg-2" :class="{'applied': !dontShowPagination}">
            <multiselect  v-model="value"  :options="options"
          :searchable="false"
          style="font-size: 13px; font-family: Ubuntu, sans-serif;"
          :show-labels="false" placeholder="Entries"></multiselect>
          </mdb-col>
            <mdb-col class="col-12 col-sm-12 col-md-6 col-lg-3 ">
              <mdb-input label="Search" hidden  size="sm" type="text" class="active-cyan-2 active-purple-2 mb-1" style="margin-top:0px;"/>
            </mdb-col>
        
          <mdb-col class="col-12 col-sm-12 col-md-8 col-lg-7 mt-1">
              <div class="d-inline float-right mr-3">
              <div class="d-inline ">
                <mdb-btn-group  style="margin-top:-18px">
                    <mdb-dropdown>
                      <mdb-btn class="mr-1 text-primary" outline="none" darkWaves tag="a" floating
                      icon="file-export"  size="sm" slot="toggle">{{$t('Export')}}</mdb-btn>
                      <mdb-dropdown-menu>
                        <mdb-dropdown-item>{{$t('Export_to_Excel')}}</mdb-dropdown-item>
                        <mdb-dropdown-item>{{$t('Export_to_PDF')}}</mdb-dropdown-item>
                      </mdb-dropdown-menu>
                    </mdb-dropdown>
                  </mdb-btn-group>
        
                  <mdb-btn class="mr-0 text-primary" outline="none" darkWaves tag="a" floating @click="showcheck_form=true"
                  icon="table"  size="sm">{{$t('editCol_columns')}}</mdb-btn>

                  <mdb-btn slot="reference" class="mr-1 text-primary" hidden outline="none" @click="clicked_filter"
                  darkWaves tag="a" floating  icon="filter"  size="sm" >{{$t('Filter')}}</mdb-btn>
                  
              </div>
              </div>
          </mdb-col>
        </mdb-row>
      </div>
      <div class="table-wrapper">
        <table  class="myTable">
                <thead>
                <tr class="header stiky_position">
                    <th width="60">№</th>
                    <th v-for="column in datasource.columns" :key="column">{{$t(column)}}
                      <span style="position:relative;">
                        <span @click="sortedArrayAsc(column)"><mdb-icon icon="angle-up"  class="px-1 up_down_icon"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                        <span @click="sortedArray(column)"><mdb-icon icon="angle-down"  class="px-1 up_down_icon" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
                      </span>
                    </th>
                    <th v-show="applyStatus" width="120" class="text-center">{{$t('btn')}}</th>
                    <th v-show="statusDebit" width="120" class="text-center">{{$t('btn')}}</th>
                    <th width="100" class="text-center">{{$t('Action')}}</th>
                </tr>
                </thead>
                <tbody>
                  <!-- .slice().reverse() -->
                <tr v-for="(row,rowIndex) in datasource.rows" :key="rowIndex">
                    <td>{{(numPag-1)*value + (rowIndex+1)}}</td>
                    <td v-for="(column,i) in datasource.columns" :key="i">
                        <mdb-badge v-show="row[column] === true" style="padding: 2px 8px;" pill color="success">{{row[column]}}</mdb-badge>
                        <mdb-badge v-show="row[column] === false"  pill color="danger" style="padding: 2px 8px;" >{{row[column]}}</mdb-badge>
                        <div v-show="column == 'colorCode'" :style="{background: row[column]}" style="width: 65px; height:3px; border-radius:10px;" ></div>
                        <div v-if="column == 'image'" class="m-0 p-0" style="width: 65px; height:45px;"><img :src="server_ip + row[column]" alt="" style="min-width:60px; max-height:45px;" class="img-fluid"></div>
                        <span v-show="row[column] !== true && row[column] !== false && column !== 'image' && column !== 'colorCode'">{{row[column]}}</span>
                    </td>
                    <td v-show="applyStatus" class="text-center">
                      <mdb-btn class="mr-1 py-1 px-2" style="font-size: 8px;" v-if="row['applyment_status'] == false" color="success"  @click="apply" :data-row="rowIndex"
                        size="sm">{{$t('apply')}}</mdb-btn>
                        <mdb-btn class="mr-1 py-1 px-2" v-if="row['applyment_status'] == true" style="font-size: 8px;" color="danger"  @click="cancel" :data-row="rowIndex"
                        size="sm">{{$t('cancel')}}</mdb-btn>
                    </td>
                    <td v-show="statusDebit" class="text-center">
                      <mdb-btn class="mr-1 py-1 px-2" style="font-size: 8px;" v-if="row['applyment_status'] == false" color="success"  @click="apply" :data-row="rowIndex"
                        size="sm">{{$t('apply')}}</mdb-btn>
                        <mdb-btn class="mr-1 py-1 px-2" v-if="row['applyment_status'] == true" style="font-size: 8px;" color="danger"  @click="cancel" :data-row="rowIndex"
                        size="sm">{{$t('cancel')}}</mdb-btn>
                    </td>
                    <td class="text-center">
                      <i v-if="userLock" class="fas fa-lock text-warning mask waves-effect m-0 pr-3"  v-on:click="LockRow" :data-row="rowIndex"></i>
                      <i  class="fas fa-pen editIcon mask waves-effect  m-0 pr-2" :class="{'applied': disable, 'applied': row['applyment_status'] == true,}" v-on:click="editRow" :data-row="rowIndex"></i>
                      <i class="fas fa-trash delIcon mask waves-effect m-0 pl-2" :class="{'applied': disable, 'applied': row['applyment_status'] == true,}" v-on:click="deleteRow" :data-row="rowIndex"></i>
                    </td>
                    
                </tr>

                </tbody>

      </table>
      </div>
      <div class="pagination-section"  v-if="dontShowPagination">
        <div class="d-inline dataTables_info"  role="status" aria-live="polite"> {{get_current_list.current_item_count}} {{$t('to_')}} {{value}} {{$t('of_')}} {{get_current_list.items_count}} </div>
        <mdb-pagination class="float-right" style="font-size:12px">
          <div @click="firstPage">
            <mdb-page-item :disabled="elementPageList==0">{{$t('First')}}</mdb-page-item>
          </div>
          
          <div @click="prevPage" :class="{'applied': elementPageList==0}">
            <mdb-page-nav  prev :disabled="elementPageList==0"></mdb-page-nav>
          </div>
          
          <div v-for="(num,i) in pageList[elementPageList]" :key="i"  @click="paginationClick(num)" >
            <mdb-page-item class="text-white"  :class="{ 'active' : numPag-1 == num }"  >{{num+1}}</mdb-page-item>
          </div>
          <div @click="nextPage" :class="{'applied': elementPageList==pageList.length-1}">
            <mdb-page-nav  next :disabled="elementPageList==pageList.length-1"></mdb-page-nav>
          </div>
          <div @click="lastPage">
            <mdb-page-item :disabled="elementPageList==pageList.length-1">{{$t('Last')}}</mdb-page-item>
          </div>

        </mdb-pagination>
      </div>

      <!-- <i class="material-icons" v-on:click="addRow">add_circle</i> -->

      <mdb-modal :show="confirm" @close="confirm = false" size="sm" class="text-center" danger>
        <mdb-modal-header center :close="false">
          <p class="heading">{{$t('Are_you_sure')}}</p>
        </mdb-modal-header>
        <mdb-modal-body>
          <div class="col-12 col-sm-12 col-md-5 col-lg-5 m-0 p-0">
            <p class="p-0 m-0 mt-2" style="font-size: 14px;">{{$t('note')}}</p>
          </div>
          <text-area rows="1"  v-model="note"  validate error="wrong" success="right"/>
        </mdb-modal-body>
        <mdb-modal-footer center>
          <mdb-btn outline="danger" @click="promise">{{$t('Yes')}}</mdb-btn>
          <mdb-btn color="danger" @click="confirm = false">{{$t('No')}}</mdb-btn>
        </mdb-modal-footer>
      </mdb-modal>

      


    <edit_col
        v-show="showcheck_form"
        :option="datasource"
        @select="checklist"
    />
  <Toast ref="message"></Toast>

  </div>
</template>


<script>
import TextArea from './textArea.vue';
import Multiselect from 'vue-multiselect'
import edit_col from "../components/editColumn";
import { mdbBtn,  mdbIcon, mdbModal, mdbModalHeader, mdbModalBody, mdbModalFooter,mdbBadge,mdbBtnGroup, mdbDropdown, mdbDropdownMenu, mdbDropdownItem,
mdbPagination, mdbPageItem, mdbPageNav,mdbRow, mdbCol, mdbInput} from 'mdbvue';
import {mapMutations, mapActions, mapGetters} from 'vuex'
export default {

  components: {
        mdbBtn,mdbBadge,TextArea,
        mdbIcon, 
        mdbModal,mdbRow,
        mdbModalHeader,mdbPagination, mdbPageItem, mdbPageNav,
        mdbModalBody,mdbDropdownMenu, mdbDropdownItem,
        mdbModalFooter, mdbCol, mdbInput,mdbBtnGroup,mdbDropdown,
        Multiselect,
        edit_col,
      },
  props:{
     datasource:{
        type: Object,
        default(){
          return {}
        }
      },
      openSer:{
        type: Boolean,
        default: false,
      },
      userLock:{
        type: Boolean,
        default: false,
      },
      applyStatus:{
        type: Boolean,
        default: false,
      },
      statusDebit:{
        type: Boolean,
        default: false,
      }
  },
  data() {
    return{
      value: localStorage.size_value,
      realSoni: 10,
      numPag: localStorage.numPage,
      pageNum: localStorage.pageNum,
      pageList: [],
      elementPageList:0,

      options: ['5','10','25','50','100'],
      promp: false,
      num: null,
      confirm: false,
      showcheck_form: false,
      show: false,
      disable: true,
      loading: false,
      admin: true,
      server_ip: this.$store.state.server_ip,
      note: '',
    }
  },
   watch:{
    'value': function(){
      localStorage.size_value = this.value;
      localStorage.numPage = 1;
      this.numPag = 1;
      this.update_pagination({size:this.value, page: this.numPag -1});
      this.pageNum = Math.ceil(this.get_current_list.items_count/(localStorage.size_value));
      this.pageList = []
      var box = []
      box.push(0);
      this.elementPageList = 0;
      for(let i=1; i<this.pageNum; i++){
        if(i%5!=0){
          console.log('dasdasd hiy')
          box.push(i)
        }
        else{
          this.pageList.push(box);
          box = []
          box.push(i)
          
        }
      }
      this.pageList.push(box);
      box = []
      console.log('this.pageList')
      console.log(this.pageList)
      console.log(this.pageNum)

      // localStorage.pageNum = this.pageNum;
      this.emit();

    },
    'get_current_list.items_count': function(){
      this.pageNum = Math.ceil(this.get_current_list.items_count/(localStorage.size_value))
      // localStorage.pageNum = this.pageNum
      this.pageList = []
      var box = []
      box.push(0);
      this.elementPageList = 0;
      for(let i=1; i<this.pageNum; i++){
        if(i%5!=0){
          console.log('dasdasd hiy')
          box.push(i)
        }
        else{
          this.pageList.push(box);
          box = []
          box.push(i)
          
        }
      }
      this.pageList.push(box);
      box = []
      this.emit();
    }
  },
  mounted (){
    this.update_pagination({size:localStorage.size_value, page: this.numPag -1})

    this.pageNum = Math.ceil(this.get_current_list.items_count/(localStorage.size_value))
    this.pageList = []
      var box = []
      box.push(0);
      this.elementPageList = 0;
      for(let i=1; i<this.pageNum; i++){
        if(i%5!=0){
          console.log('dasdasd hiy')
          box.push(i)
        }
        else{
          this.pageList.push(box);
          box = []
          box.push(i)
          
        }
      }
      this.pageList.push(box);
      box = []
    // localStorage.pageNum = this.pageNum 
    // this.disable = localStorage.Type
    localStorage.UserIdForAuth = null;
    if(localStorage.AccessType == 0){
      this.disable = false;
      this.admin = false;
    }
    else{
      this.disable = true;
    }
  },
  computed: mapGetters(['get_pagination', 'get_current_list', 'dontShowPagination']),
  methods: {
    ...mapMutations(['update_pagination']),
    ...mapActions(['fetch_service_medicine_list']),
    async checklist (option){
        this.showcheck_form = option;
        this.$emit('update_column');
      },
      clicked_filter()
      {
        this.$emit('clicked_filter');
      },
      emit(){
        this.$emit('size');
      },
      paginationClick(i){
        this.numPag = i+1;
        localStorage.numPage = this.numPag;

        this.update_pagination({size:this.value, page: i})
        this.$emit('page');
      },

        onChange(ev) {
            //console.dir(ev.target.dataset)
            let col = ev.target.dataset.column
            let row = ev.target.dataset.row
            this.datasource.rows[parseInt(row)][col] = ev.target.value
        },
        promise () {
          // this.del = true
          this.confirm = false
          this.$emit('for_delete',this.datasource.rows[this.num_target],this.num_target, this.note);
          this.note = '';

          //this.datasource.rows.splice( parseInt(this.num_target), 1)
        },

        editRow(ev)
        {
          this.num_target = ev.target.dataset.row;
          this.$emit('for_edit',this.datasource.rows[this.num_target]);
        },

        deleteRow(ev) {
            this.confirm = true
            this.num_target = ev.target.dataset.row
            //appData.updated()
        },
        LockRow(ev){
          this.$router.push('/authorization')
          this.num_target = ev.target.dataset.row
          localStorage.UserIdForAuth = this.datasource.rows[this.num_target].id
        },
        apply(ev){
          console.log(ev.target.dataset.row)
          this.$emit('for_apply',this.datasource.rows[ev.target.dataset.row].id);

        },
        cancel(ev){
          console.log(ev.target.dataset.row)
          this.$emit('for_cancel',this.datasource.rows[ev.target.dataset.row].id);

        },


        prevPage(){
          console.log('prev')
          this.elementPageList --;
          this.numPag = this.pageList[this.elementPageList][0]+1;
          localStorage.numPage = this.numPag;

          this.update_pagination({size:this.value, page: this.numPag})
          this.$emit('page');

        },
        nextPage(){
          console.log('next')
          this.elementPageList ++;
          this.numPag = this.pageList[this.elementPageList][0]+1;
          localStorage.numPage = this.numPag;

          this.update_pagination({size:this.value, page: this.numPag})
          this.$emit('page');
        },
        lastPage(){
           this.elementPageList = this.pageList.length-1;
           this.numPag = this.pageList[this.elementPageList][0]+1;
            localStorage.numPage = this.numPag;

            this.update_pagination({size:this.value, page: this.numPag})
            this.$emit('page');
        },
        firstPage(){
           this.elementPageList = 0;
           this.numPag = this.pageList[this.elementPageList][0]+1;
          localStorage.numPage = this.numPag;

          this.update_pagination({size:this.value, page: this.numPag})
          this.$emit('page');
        },

        sortedArrayAsc(key){
            function compare(a, b) {
              if (a[key].toLowerCase() < b[key].toLowerCase())
                return -1;
              if (a[key].toLowerCase() > b[key].toLowerCase())
                return 1;
              return 0;
            }
            this.datasource.rows.sort(compare);
        },
        sortedArray(key){
            function compare(a, b) {
              if (a[key].toLowerCase() > b[key].toLowerCase())
                return -1;
              if (a[key].toLowerCase() < b[key].toLowerCase())
                return 1;
              return 0;
            }

            this.datasource.rows.sort(compare);
        }
    },
}

</script>

<style src="vue-multiselect/dist/vue-multiselect.min.css"></style>
<style scoped lang="scss">
.applied{
  pointer-events: none;
  background:#fcfcfc;
  opacity: 0.6;
}

.d_table{
  padding: 16px;
  background: #f8fafb;
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen', 'Ubuntu', 'Cantarell', sans-serif;
}

.controls-section {
  background: #ffffff;
  border-radius: 12px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
  border: 1px solid #f0f0f0;
  padding: 16px;
  margin-bottom: 16px;
}

.table-wrapper {
  background: #ffffff;
  border-radius: 12px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
  border: 1px solid #f0f0f0;
  overflow: hidden;
  overflow-x: auto;
}

.myTable {
  table-layout: fixed;
  width: 100%;
  border-collapse: separate;
  border-spacing: 0;
  font-size: 12px;
  
  tr:nth-child(even) {
    background-color: #fafbfc;
  }
}

.myTable th {
  font-weight: 600;
  font-size: 11px;
  letter-spacing: -0.01em;
}

.myTable td {
  text-overflow: ellipsis; 
  overflow: hidden; 
  white-space: nowrap;
  font-size: 11px;
  color: #374151;
  letter-spacing: -0.01em;
}

.myTable th, .myTable td {
  text-align: left;
  padding: 8px 12px;
}

.myTable tbody tr {
  border-bottom: 1px solid #f3f4f6;
  transition: all 0.15s ease;
}

.myTable tbody tr:hover {
  background: #f0fdf4 !important;
  transform: translateX(2px);
}

.stiky_position {
  position: -webkit-sticky; /* Safari */
  position: sticky;
  top: 0;
  background: #10b981;
  color: white;
  z-index: 111111;
  box-shadow: 0 2px 4px rgba(0, 0, 0, 0.1);
  
  th {
    color: white;
    font-weight: 600;
    font-size: 11px;
    letter-spacing: -0.01em;
  }
  
  .up_down_icon {
    color: white;
    transition: all 0.2s ease;
    
    &:hover {
      background: rgba(255, 255, 255, 0.2);
      border-radius: 4px;
    }
  }
}

.editIcon {
  color: #10b981;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.2s ease;
  padding: 4px;
  border-radius: 4px;
  
  &:hover {
    color: #059669;
    background: rgba(16, 185, 129, 0.1);
    transform: scale(1.1);
  }
}

.delIcon {
  color: #ef4444;
  font-size: 14px;
  cursor: pointer;
  transition: all 0.2s ease;
  padding: 4px;
  border-radius: 4px;
  
  &:hover {
    color: #dc2626;
    background: rgba(239, 68, 68, 0.1);
    transform: scale(1.1);
  }
}

.pagination-section {
  background: #ffffff;
  border-radius: 12px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
  border: 1px solid #f0f0f0;
  padding: 16px;
  margin-top: 16px;
  display: flex;
  justify-content: space-between;
  align-items: center;
  flex-wrap: wrap;
  gap: 16px;
  
  .dataTables_info {
    color: #6b7280;
    font-size: 12px;
    font-weight: 500;
  }
  
  mdb-pagination {
    mdb-page-item {
      &.active {
        background: #10b981 !important;
        border-color: #10b981 !important;
      }
      
      &:hover:not(.disabled) {
        background: rgba(16, 185, 129, 0.1) !important;
      }
    }
    
    mdb-page-nav {
      &:hover:not(.disabled) {
        background: rgba(16, 185, 129, 0.1) !important;
      }
    }
  }
}
.delete{
  
  position: fixed;
  display: flex;
  justify-content: center;
  top:0;
  animation: logo 0.2s linear;
  z-index:111111;
  left:0;
  width:100%;
  height:100vh;
  background:rgba(0, 0, 0, 0.5);
  .delete_form{
    background: white;
    border-radius: 5px;
    // box-shadow: 0 0 3px rgb(121, 121, 121);
    max-height: 135px;
    width: 420px;
    transform: translate(0, 20px);
    animation: anime 0.2s linear;
    p{
      padding: 0px 35px;
    }
    .delete_btn{
      text-align: right;
      button{
        border-radius: 5px;
        padding: 3px 20px;
      }
    }
  }
}
@keyframes anime
{
  0%{
    transform: translate(0, -130px);
    opacity: 0;
  }
  100%{
    transform: translate(0, 20px);
    opacity: 1;
  }
}
@keyframes logo
{
  0%{
    opacity: 0;
  }
  100%{
    opacity: 1;
  }
}
// true and false background
.span_bg{
  border-radius: 3px;
  font-family: roboto, sans-serif;
  font-weight: 600;
  font-size: 12px;
  padding: 1px 5px;
  box-shadow: 1px 2px 7px rgb(191, 191, 191);
}
.multiselect__tag {
    min-height: calc(1.5em + .75rem + 2px);
    display: block;
    padding: 0 40px 0 8px;
    border-radius: 5px;
    border: 1px solid #e8e8e8;
    background: #fff;
    font-size: 12px;
}
.activeTable{
  background: #10b981;
  border-radius: 6px;
  color: white;
  padding: 4px 8px;
}

.up_down_icon {
  transition: all 0.2s ease;
  padding: 2px;
  border-radius: 4px;
  
  &:hover {
    background: rgba(255, 255, 255, 0.2);
  }
}

// Badge styling
mdb-badge {
  border-radius: 8px !important;
  font-weight: 500 !important;
  font-size: 10px !important;
  letter-spacing: -0.01em;
  padding: 4px 8px !important;
}

// Button styling in controls section
.controls-section {
  mdb-btn {
    border-radius: 8px;
    font-size: 12px;
    font-weight: 500;
    transition: all 0.2s ease;
    
    &:hover {
      transform: translateY(-1px);
      box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1);
    }
  }
  
  multiselect {
    border-radius: 8px;
    
    .multiselect__tags {
      border-radius: 8px;
      border: 1px solid #e5e7eb;
      min-height: 36px;
      
      &:focus-within {
        border-color: #10b981;
      }
    }
  }
}


</style>
