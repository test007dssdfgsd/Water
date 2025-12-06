<template>
  <div class="report-app">
    <div class="report-main">
      <div class="report-header">
        <div class="header-content">
          <h4 class="page-title">
            <mdb-icon icon="trash-alt" class="mr-2" />
            {{$t('deleteList')}}
          </h4>
        </div>
      </div>
      
      <div class="controls-section">
        <div class="controls-content">
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
              @click="clickDate(0, searchQuery)" 
              size="sm"
            >
              <mdb-icon icon="check" class="mr-1" />
              {{$t('ok')}}
            </mdb-btn>
          </div>
        </div>
      </div>
      
      <div class="table-container">
        <div v-if="searchQuery" class="search-indicator">
          <span class="icon">🔍</span>
          <span>Qidirilmoqda:</span>
          <b>{{ searchQuery }}</b>
        </div>
        <loader v-if="loading"/>
        <div v-else class="table-wrapper">
          <table class="report-table">
        <thead>
          <tr class="header py-3 stiky_position">
            <th  width="40" class="text-left">№</th>
            <th>{{$t('client_name')}}
              <span style="position:relative;">
                <span @click="sortedArrayAsc('fio')"><mdb-icon icon="angle-up"  class="px-1 up_down_icon"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                <span @click="sortedArray('fio')"><mdb-icon icon="angle-down"  class="px-1 up_down_icon" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
              </span>
            </th>
            <th>{{$t('note')}}
              <span style="position:relative;">
                <span @click="sortedArrayAsc('reserverd_note_3')"><mdb-icon icon="angle-up"  class="px-1 up_down_icon"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                <span @click="sortedArray('reserverd_note_3')"><mdb-icon icon="angle-down"  class="px-1 up_down_icon" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
              </span>
            </th>
            <th width="150">{{$t('date')}}
              <span style="position:relative;">
                <span @click="sortedArrayAsc('created_date_time')"><mdb-icon icon="angle-up"  class="px-1 up_down_icon"  style="position:absolute; top:-2px; cursor:pointer;"/></span>
                <span @click="sortedArray('created_date_time')"><mdb-icon icon="angle-down"  class="px-1 up_down_icon" style="position:absolute; bottom:-4px; cursor:pointer;"/></span>
              </span>
            </th>
            <th width="80">{{$t('Action')}}
            
            </th>
            <!-- <th >{{$t('lessons_cout')}}</th> -->

          </tr>
        </thead>
        <tbody>
          <tr v-for="(row,rowIndex) in checkList" :key="rowIndex" class="hoverTr">
            <td> <span >{{rowIndex+1}}</span> </td>
            <td> <span >{{row.fio}}</span> </td>
            <td> <span >{{row.reserverd_note_3}}</span> </td>
            <td> <span >{{row.created_date_time.slice(0,10)}}</span> <span class="ml-2">{{row.created_date_time.slice(11,16)}}</span></td>
            <td class="action-cell">
              <div class="action-buttons-cell">
                <div @click="return_client(row)" class="restore-icon-wrapper">
                  <mdb-icon icon="check-circle" class="restore-icon" far></mdb-icon>
                </div>
              </div>
            </td>
          </tr>
        </tbody>
      </table>
        </div>
        <!-- Pagination tugmalari -->
        <div class="pagination-section" v-if="totalPages > 0">
          <Pagination 
            :totalPages="totalPages" 
            :currentPage="currentPage" 
            @page-changed="changePage" 
          />
        </div>
      </div>
    </div>
    
    <Toast ref="message"></Toast>
  </div>
</template>

<script>
// import lineSelect from "../../components/lineSelect.vue";
import Pagination from './pagination.vue'
import {mdbBtn, mdbIcon, mdbInput} from 'mdbvue'
import {mapActions, mapGetters} from 'vuex'
import month from '../../components/month.vue'
export default {
  components:{
    mdbBtn, 
    mdbIcon,mdbInput,
    month,Pagination
  },
  data() {
    return {
      id: 0,
      loading:false,
      checkList: [],
      b_date:'',
      e_date:'',

      search: '',

      payClient: false,
      today_date: '',
      check_client_list: [],
      group_data: {},

      currentPage: 0,
      totalPages: 0,
      pageSize: 100,
      searchQuery: '',
    }
  },
  created () {
    this.clickDate(0, this.searchQuery)
  },
  computed: {
    ...mapGetters(['allGroups', 'group_client_list']),
  },
 
  async mounted() {
    let date = new Date();
    this.today_date = date.toISOString().slice(0,10);
    this.b_date = this.today_date;
    this.e_date = this.today_date;
    await this.clickDate(0,this.searchQuery);
    window.addEventListener("keydown", this.handleKey);
  },
  beforeDestroy() { window.removeEventListener("keydown", this.handleKey); },

  methods: {
    ...mapActions(['fetchGroups', 'fetchClient', 'fetchGroupsClientList']),

    show_infoDebit(i){
      console.log(i)
    },
   async return_client(data){
      console.log(data)
     this.loading = true;
      try{
        const res = await fetch(this.$store.state.hostname + '/WaterClients/restoreDeleteClientByIdAndNote?id='+ data.id);
        
        if(res.status == 200 || res.status == 201){
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
    // ===> send client check to base<===
    payDebit(data){
      this.payClient = true;
      this.group_data = data
    },
    changePage (page) {
      this.currentPage = page;
      this.clickDate(page,this.searchQuery);
    },
    async handleKey(e) {
      if (e.key === "Backspace")
      {
        this.searchQuery = this.searchQuery.slice(0, -1);
        await this.clickDate(0,this.searchQuery);
        this.currentPage = 0;

      }
      else if (e.key.length === 1) {
        this.searchQuery += e.key;
        await this.clickDate(0,this.searchQuery);
        this.currentPage = 0;
      }
      if(this.searchQuery == '' || this.searchQuery == null){
        await this.clickDate(0,this.searchQuery);
        this.currentPage = 0;
      }
    },

    async clickDate(page,search){
      this.loading = true;
      try{
        const res = await fetch(this.$store.state.hostname + `/WaterClients/getPaginationDeletedClients?page=${page}&size=${this.pageSize}&search=${search}`);
        const data = await res.json();
        console.log(data)
        if(res.status == 200 || res.status == 201){
          console.log('das')
          this.checkList = data.items_list;
          this.totalPages = Math.ceil(data.items_count / this.pageSize);
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

    // ===> send client <===

    // ===> sort table <===
    sortedArrayAsc(key){
        function compare(a, b) {
          if (a[key] < b[key])
            return -1;
          if (a[key] > b[key])
            return 1;
          return 0;
        }
        this.checkList.sort(compare);
    },
    sortedArray(key){
        function compare(a, b) {
          if (a[key] > b[key])
            return -1;
          if (a[key] < b[key])
            return 1;
          return 0;
        }

        this.checkList.sort(compare);
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

.search-indicator {
  padding: 12px 16px;
  background: #fef3c7;
  border-bottom: 1px solid #f0f0f0;
  display: flex;
  align-items: center;
  gap: 8px;
  font-size: 13px;
  color: #374151;
  
  .icon {
    display: inline-block;
    animation: shake 1s infinite;
  }
  
  b {
    color: #059669;
    font-weight: 600;
  }
}

@keyframes shake {
  0% { transform: rotate(0deg); }
  25% { transform: rotate(15deg); }
  50% { transform: rotate(0deg); }
  75% { transform: rotate(-15deg); }
  100% { transform: rotate(0deg); }
}

.table-wrapper {
  overflow-x: auto;
}

.report-table {
  width: 100%;
  border-collapse: separate;
  border-spacing: 0;
  font-size: 12px;
  table-layout: fixed;
  
  tbody tr:nth-child(even) {
    background-color: #fafbfc;
  }
}

.report-table th {
  padding: 10px 12px;
  font-weight: 600;
  font-size: 11px;
  color: white;
  letter-spacing: -0.01em;
  text-align: left;
  background: #10b981;
  
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

.report-table td {
  padding: 10px 12px;
  font-size: 11px;
  color: #374151;
  letter-spacing: -0.01em;
  border-bottom: 1px solid #f3f4f6;
  text-align: left;
}

.report-table tbody tr {
  transition: all 0.15s ease;
  
  &:hover {
    background: #f0fdf4 !important;
    transform: translateX(2px);
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
    
    .restore-icon-wrapper {
      cursor: pointer;
      padding: 6px;
      border-radius: 6px;
      transition: all 0.2s ease;
      
      &:hover {
        background: rgba(16, 185, 129, 0.1);
        transform: scale(1.1);
      }
    }
    
    .restore-icon {
      color: #10b981;
      font-size: 16px;
    }
  }
}

.pagination-section {
  padding: 16px;
  border-top: 1px solid #f3f4f6;
  display: flex;
  justify-content: center;
}

@media (max-width: 768px) {
  .report-header {
    .header-content {
      flex-direction: column;
      align-items: flex-start;
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
  }
}
</style>