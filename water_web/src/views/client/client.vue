<template>
  <div class="page-app">
    <div class="page-main">
      <div class="page-header">
        <div class="header-content">
          <h4 class="page-title">
            <mdb-icon icon="user-friends" class="mr-2" />
            {{$t('client')}}
          </h4>
          
          <div class="header-actions">
            <router-link to="/client_Add/0" class="add-btn-link">
              <mdb-btn class="add-btn" color="success" size="sm">
                <mdb-icon icon="plus" class="mr-2" />
                {{$t('add')}}
              </mdb-btn>
            </router-link>
          </div>
        </div>
      </div>
      
      <div class="search-section">
        <input-icon 
          class="search-input" 
          v-model="search" 
          @input="searchClick" 
          :placeholder="$t('search_client')"
        />
      </div>
      
      <div class="table-section">
        <loaderTable v-if="loading"/>
        <anyTable 
          v-else
          :datasource="clientList"
          @for_delete="for_delete"
          @for_edit="for_edit"
          @page="page"
          @size="size"
        />
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
import {
  mdbIcon,
  mdbBtn,
} from "mdbvue";
import {mapActions, mapGetters, mapMutations} from 'vuex'
import anyTable from "../../components/erpTable"
export default {
  data(){
    return {
      modal_info: '',
      modal_status: false,
      loading: false,
      auth_id: localStorage.AuthId,
      clientList: {
        rows: [],
        columns: ['fio','first_phone_number_name','first_address_name'],
        col: []
      },
      search: '',
    }
  },
  components: {
    anyTable, mdbIcon,
    mdbBtn,
  },
  computed: mapGetters(['allClient', 'get_pagination']),
  methods: {
    ...mapActions(['fetchClient', ]),
    ...mapMutations(['client_delete_row', 'product_delete_row', 'update_pagination_first']),
  
      for_edit(edit_data)
      {
        this.$router.push("/client_add/" + edit_data.id);
      },
      async for_delete(del_data,index,note)
      {
        console.log(index)
          const response = await fetch(this.$store.state.hostname + "/WaterClients/deleteClientByIdAndNote?id=" + del_data.id + '&note=!' + note);
          const data = await response.json();
          if(response.status == 201 || response.status == 200)
          {
            this.$refs.message.success('Successfully_removed')
            this.alert_success = true;
            await this.refresh();
          }
          else{
            this.modal_info = data;
            this.modal_status = true;
          }
      },
      page(){
        this.refresh();
      },
      size(){
        this.refresh();
      },
      async refresh(){
        const res = await fetch(this.$store.state.hostname + '/WaterClients/getPagination?page=' + this.get_pagination.page + '&size=' + this.get_pagination.size);
        const res_data = await res.json();
        // await this.update_column();
        this.update_pagination_first({current_item_count: res_data.current_item_count, current_page: res_data.current_page+1, items_count: res_data.items_count});
        this.clientList.rows = res_data.items_list;
        this.loading = false;
      },
      async searchClick(){
        if(this.search == '' || this.search == null){
          await this.refresh();
        }
        else{
          const res = await fetch(this.$store.state.hostname + '/WaterClients/getPaginationByName?page=' + this.get_pagination.page + '&size=' + this.get_pagination.size + '&fio=' + this.search);
          const res_data = await res.json();
          console.log('dasd')
          console.log(res_data)
          // await this.update_column();
          // this.update_pagination_first({current_item_count: res_data.current_item_count, current_page: res_data.current_page+1, items_count: res_data.items_count});
          this.clientList.rows = res_data.items_list;
          this.loading = false;
        }
        
      }
  },
  async mounted(){
    this.loading = true;
    await this.refresh();
    this.loading = false;
    if(this.$store.state.alert){
      this.$refs.message.success('Added_successfully')
      this.$store.state.alert = false
    }
  }
}
</script>

<style lang="scss" scoped>
.page-app {
  min-height: 100vh;
  background: #f8fafb;
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen', 'Ubuntu', 'Cantarell', sans-serif;
  padding: 16px;
}

.page-main {
  max-width: 1600px;
  margin: 0 auto;
}

.page-header {
  background: linear-gradient(135deg, #10b981 0%, #059669 100%);
  border-radius: 12px;
  padding: 5px 24px;
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
  
  .header-actions {
    display: flex;
    align-items: center;
    gap: 12px;
    
    .add-btn-link {
      text-decoration: none;
      
      .add-btn {
        border-radius: 8px;
        font-size: 12px;
        font-weight: 500;
        padding: 6px 16px;
        height: 36px;
        transition: all 0.2s ease;
        box-shadow: 0 2px 4px rgba(16, 185, 129, 0.2);
        background: linear-gradient(135deg, #10b981 0%, #059669 100%);
        border: none;
        
        &:hover {
          transform: translateY(-1px);
          box-shadow: 0 4px 8px rgba(16, 185, 129, 0.3);
        }
        
        mdb-icon {
          font-size: 13px;
        }
      }
    }
  }
}

.search-section {
  background: #ffffff;
  border-radius: 12px;
  box-shadow: 0 1px 3px rgba(0, 0, 0, 0.05);
  border: 1px solid #f0f0f0;
  padding: 12px 16px;
  margin-bottom: 16px;
  
  .search-input {
    width: 100%;
    max-width: 400px;
    border-radius: 8px;
  }
}

.table-section {
  // Table komponenti o'zining dizayniga ega
}

@media (max-width: 768px) {
  .page-header {
    .header-content {
      flex-direction: column;
      align-items: flex-start;
    }
    
    .header-actions {
      width: 100%;
      
      .add-btn-link {
        width: 100%;
        
        .add-btn {
          width: 100%;
        }
      }
    }
  }
  
  .search-section {
    .search-input {
      max-width: 100%;
    }
  }
}
</style>