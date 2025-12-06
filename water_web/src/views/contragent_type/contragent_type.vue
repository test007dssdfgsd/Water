<template>
  <div class="page-app">
    <div class="page-main">
      <div class="page-header">
        <div class="header-content">
          <h4 class="page-title">
            <mdb-icon icon="tags" class="mr-2" />
            {{$t('contragent_type')}}
          </h4>
          
          <div class="header-actions">
            <router-link to="/contragent_type_add/0" class="add-btn-link">
              <mdb-btn class="add-btn" color="success" size="sm">
                <mdb-icon icon="plus" class="mr-2" />
                {{$t('add')}}
              </mdb-btn>
            </router-link>
          </div>
        </div>
      </div>
      
      <div class="table-section">
        <loaderTable v-if="loading"/>
        <anyTable 
          v-else
          :datasource="allLevel"
          @for_delete="for_delete"
          @for_edit="for_edit"
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
import anyTable from "../../components/anyTable"
export default {
  data(){
    return {
      modal_info: '',
      modal_status: false,
      loading: false,
      auth_id: localStorage.AuthId
    }
  },
  components: {
    anyTable, mdbIcon,
    mdbBtn,
  },
  computed: mapGetters(['allLevel']),
  methods: {
    ...mapActions(['fetchLevel', ]),
    ...mapMutations(['level_delete_row']),
  
      for_edit(edit_data)
      {
        this.$router.push("/contragent_type_add/" + edit_data.id);
      },
      async for_delete(del_data,index)
      {
         const requestOptions = {
            method : "delete",
          };
          const response = await fetch(this.$store.state.hostname + "/WaterContragentTypes/" + del_data.id, requestOptions);
          const data = await response.text();

          if(response.status == 201 || response.status == 200)
          {
            this.$refs.message.success('Successfully_removed')
            this.alert_success = true;
            this.level_delete_row(index);
          }
          else{
            this.modal_info = data;
            this.modal_status = true;
          }
      },
  },
  async mounted(){
    this.loading = true;
    await this.fetchLevel();
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
}
</style>