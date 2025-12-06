<template>
  <div>
    <div class="d-flex allContent" >
      <div class="" :class="{'leftmenu': !show_title, 'smallleftmenu': show_title }">
        <div class="d-flex pb-1 pt-1 sidebar-header">
          <svg v-if="!show_title" xmlns="http://www.w3.org/2000/svg" @click="backMenu(!show_title)" class="icon icon-tabler icon-tabler-chevron-left leftdown" style="cursor:pointer;" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="#065f46" fill="none" stroke-linecap="round" stroke-linejoin="round">
            <path stroke="none" d="M0 0h24v24H0z" fill="none"/>
            <polyline points="15 6 9 12 15 18" />
          </svg>
          <svg v-else xmlns="http://www.w3.org/2000/svg" @click="backMenu(!show_title)" class="icon icon-tabler icon-tabler-menu-2  backleftdown"  style="cursor:pointer;" width="24" height="24" viewBox="0 0 24 24" stroke-width="2" stroke="#065f46" fill="none" stroke-linecap="round" stroke-linejoin="round">
            <path stroke="none" d="M0 0h24v24H0z" fill="none"/>
            <line x1="4" y1="6" x2="20" y2="6" />
            <line x1="4" y1="12" x2="20" y2="12" />
            <line x1="4" y1="18" x2="20" y2="18" />
          </svg>
          <div  class="d-flex align-items-center" v-if="!show_title">
            <h5 class="m-0 p-0 ml-2 sidebar-title">Extreme CRM</h5>
          </div>
        </div>
        <div class="mainMenuList " >
          
          <div v-for="(link,i) in links" :key="i"
            class="d-flex justify-content-center align-items-center"
            :class="{'maiMenuItem': !show_title, 'mainSmallMenuItem': show_title}">
            <div class="text-center" >
              <mdb-icon :icon="link.icon" style="font-size:18px;" :class="{'largeIcon': !show_title, 'smallIcon': show_title }"/>
              <span v-if="!show_title" class="d-block mt-2 sidebar-item-text" style="font-size:13px; font-weight: 500;">{{$t(link.title)}}</span>
            </div>
            
            <div style="position:absolute; right:-260px; top:0; width:260px; z-index:9999999999 !important;" class="SideBarListRight">
              <div v-if="show_title" class="titleSideItem d-flex align-items-center">
                <a :href="link.url" class="d-block  ml-4" style="font-size:14px;">{{$t(link.title)}}</a>
              </div>
              <div v-for="(item,index) in link.down_list" :key="index" style="cursor:pointer">
                <router-link 
                tag="li" custom v-slot="{ navigate }"
                :to="item.url"
                :class="{'active_link': item.view}"
                class="d-flex hoverSideBarItem sidebar-dropdown-link"

                >
                <!-- <MDBIcon style="color: red; margin-right: 10px; padding-left: 15px;" icon="camera-retro" />
                Xodimlar -->
                <li @click="navigate" role="link">        
                    <p  style="padding-left: 15px; font-size: 12px;"  class="m-0 sidebar-dropdown-text">{{$t(item.title)}}</p>
                </li>
                
              </router-link>
              </div>
            </div>
          </div>
        </div>
        

      </div>
     <div div class="main  bg-white" >
        <router-view></router-view>
      </div>
    </div>
  </div>
</template>

<script>
  import { mdbIcon } from 'mdbvue';
  // import { ref } from 'vue';


  export default {
    components: {
      mdbIcon,
    },
    mounted() {
      this.show_title = localStorage.sidebar;
      console.log(this.show_title)
      console.log(localStorage.sidebar)
      this.name = localStorage.Name;
      for (let j = 0; j < this.links.length; j++) {
        if(this.links[j].url == this.$route.fullPath){
          this.links[j].view = true;
          return
        }else{
          if(this.links[j].down_list.length > 0){
            for (let k = 0; k < this.links[j].down_list.length; k++){
              if(this.links[j].down_list[k].url == this.$route.fullPath){
                this.links[j].down_list[k].dview = true;
                this.links[j].view = true;
                this.indexMain = j;
                return
              }
            }
          }
        }
      }
    },
    data() {
      return {
        name: 'sidebar',
        show_title: localStorage.sidebar,
        indexMain: -1,
        links: [

            { title: "mainMenu", icon: 'home', url: '/order_Add/0', view: false, color: '#000', down_list:[
            ] },

            { title: "producs", icon: 'cart-arrow-down', url: '', view: false, color: '#000', down_list:[
              { title: "product", icon: 'home', url: '/product', view: false },
              ]
            },
            // { title: "contragent", icon: 'bell', url: '', view: false, color: '#000', down_list:[
            //   { title: "contragent", icon: 'box', url: '/contragent', view: false },
            //   { title: "contragent_type", icon: 'laptop-code', url: '/contragent_type', view: false },
            // ] },
            // { title: "classes", icon: 'graduation-cap', url: '', view: false, color: '#000', down_list:[
            //   { title: "fans", icon: 'box', url: '/fans', view: false },
            //   { title: "groups", icon: 'shopping-basket', url: '/groups', view: false },
            //   // { title: "probniyFans", icon: 'shopping-basket', url: '/probniyFans', view: false },
            //   { title: "test", icon: 'laptop-code', url: '/test', view: false },
            // ] },
            { title: "menu_setting", icon: 'cog', url: '', view: false, color: '#000', down_list:[
              { title: "user", icon: 'landmark', url: '/user', view: false,},
              { title: "client", icon: 'landmark', url: '/client', view: false,},
              { title: "contragent", icon: 'box', url: '/contragent', view: false },
              { title: "contragent_type", icon: 'laptop-code', url: '/contragent_type', view: false },
              { title: "province", icon: 'landmark', url: '/province', view: false,},
              { title: "district", icon: 'bell', url: '/district', view: false,},
            ] },
            

            { title: "report", icon: 'calendar-alt', url: '', view: false, color: '#000', down_list:[
              { title: "dashboard", icon: 'tachometer-alt', url: '/dashboard', view: false,},
              { title: "report_date", icon: 'landmark', url: '/check', view: false,},
              { title: "postavchik_report", icon: 'landmark', url: '/postov_money_report', view: false,},
              { title: "statistic", icon: 'landmark', url: '/statistic', view: false,},
              { title: "lastOrderReport", icon: 'landmark', url: '/lastOrderReport', view: false,},
              { title: "deleteList", icon: 'landmark', url: '/deleteReport', view: false,},
            ] },
            { title: "order", icon: 'map-marked', url: '/new_order_list', view: false, color: '#000', down_list:[
              { title: "order_list", icon: 'landmark', url: '/new_order_list', view: false,},
              { title: "accept_order", icon: 'landmark', url: '/accept_order', view: false,},
            ] },
             { title: "car_order", icon: 'car', url: '/map_order_car', view: false, color: '#000', down_list:[
              { title: "car_order", icon: 'landmark', url: '/map_order_car', view: false,},
              { title: "postavchik_list", icon: 'landmark', url: '/pos_order_list', view: false,},
              { title: "postavchik_statistics", icon: 'landmark', url: '/postavchik_statistics', view: false,},
            ] },

            { title: "logout", icon: 'sign-out-alt', url: '/', view: false, color: '#000', down_list:[
            ] },
            // { title: "logout", icon: 'sign-out-alt', url: '/postavchik_list', view: false, color: '#000', down_list:[
            // ] },

            // { title: "salary", icon: 'credit-card', url: '', view: false, color: '#000', down_list:[
            //   { title: "salary_pay", icon: 'landmark', url: '/salaryPay', view: false,},
            //   { title: "salary_list", icon: 'landmark', url: '/salary', view: false,}
            // ] },
            // { title: "company", icon: 'landmark', url: '#', view: false, color: '#000', down_list:[
            //   { title: "province", icon: 'landmark', url: '/province', view: false,},
            //   { title: "district", icon: 'bell', url: '/district', view: false,},
            // ] },
            

        ],
      }
    },
    created() {
        this.$root.$refs.sidebar = this;
    },
    methods: {
      update_down: function (i) {
        this.indexMain = i;
        for (let j = 0; j < this.links.length; j++) {
          if (j !== i) {
            this.links[j].view = false
            if(this.links[j].down_list.length > 0) {
              for (let k = 0; k < this.links[j].down_list.length; k++) {
                  this.links[j].down_list[k].dview = false
              }
            }
          }
        }
        this.links[i].view = true
      },
      update_down_click: function (i) {
        for (let j = 0; j < this.links[this.indexMain].down_list.length; j++) {
          if (j !== i) {
            this.links[this.indexMain].down_list[j].dview = false
          }
        }
        this.links[this.indexMain].down_list[i].dview = true
       // console.log(this.links[this.indexMain].down_list[i].dview)
      },
      backMenu(a){
        this.show_title = a
        localStorage.sidebar = this.show_title
        console.log(localStorage.sidebar)

      }
    }
  };
</script>

<style lang="scss" >
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
  background: #4e6a7c;
  border-radius: 5px;
}

/* Handle on hover */
::-webkit-scrollbar-thumb:hover {
  background: #2b485d;
}
*{
  font-family: 'Montserrat', sans-serif;
}
.main{
  background: #ffffff;
  width: 100%;
  min-height: 100vh;
  font-family: 'Inter', -apple-system, BlinkMacSystemFont, 'Segoe UI', 'Roboto', 'Oxygen', 'Ubuntu', 'Cantarell', sans-serif;
}
.leftmenu{
  z-index: 999999 !important;
  width: 240px;
  position: -webkit-sticky; /* Safari */
  position: sticky;
  top: 0;
  height: 100vh;
  padding: 0px 0px;
  margin: 0;
  background: linear-gradient(180deg, #d1fae5 0%, #a7f3d0 50%, #86efac 100%);
  font-size: 14px;
  box-sizing: border-box;
  border-right: 1px solid #6ee7b7;
  // overflow: hidden;
  // overflow-y: scroll;
}
.backleftmenu{
  margin: 0;
  box-sizing: border-box;
  padding: 0;
  background: linear-gradient(180deg, #d1fae5 0%, #a7f3d0 50%, #86efac 100%);
  font-size: 12px;

}
.smallleftmenu{
  z-index: 999999 !important;
  width: 55px;
  position: -webkit-sticky; /* Safari */
  position: sticky;
  top: 0;
  height: 100vh;
  // padding: 2px 0px;
  margin: 0;
  background: linear-gradient(180deg, #d1fae5 0%, #a7f3d0 50%, #86efac 100%);
  font-size: 14px;
  border-right: 1px solid #6ee7b7;
  // overflow: hidden;
  // overflow-y: scroll;
  
}

.allContent{
  min-height: 100vh;
  
  
}

.round{
  transform: rotate(90deg);
  transition: all 0.5s;
}
.back_round{
  transition: all 0.5s;
}
.ico{
  padding-left: 15px;
}
.backico{
  padding-left: 17px;
}
.leftdown{
  margin: 10px 8px;
  padding: 2px;
}
.backleftdown{
  margin: 9px 12px;
  padding: 1px;
}
.largeIcon{
  font-size:20px;
  color:#065f46;
  transition: all 0.2s ease;

}
.smallIcon{
  font-size: 18px;
  color:#065f46;
  transition: all 0.2s ease;
}

.sidebar-item-text {
  color: #065f46 !important;
  font-weight: 500;
  letter-spacing: -0.01em;
  transition: all 0.2s ease;
  font-size: 13px;
}

.maiMenuItem{
  position: relative;
  height: 60px;
  border-bottom: none;
  transition: all 0.2s ease;
  cursor: pointer;
  
  &.active {
    background: rgba(16, 185, 129, 0.25);
    border-left: 3px solid #10b981;
    
    .largeIcon {
      color: #064e3b;
    }
    
    .sidebar-item-text {
      color: #064e3b !important;
      font-weight: 600;
    }
  }
}
.maiMenuItem .SideBarListRight{
    display: none;
  }
.maiMenuItem:hover{
    background: rgba(16, 185, 129, 0.2);
    border-left: 3px solid #10b981;
    .largeIcon{
      color: #064e3b;
    }
    .sidebar-item-text {
      color: #064e3b !important;
      font-weight: 600;
    }
    .SideBarListRight{
      display: block !important;
      z-index: 11111111;
      background: linear-gradient(135deg, #d1fae5 0%, #a7f3d0 100%);
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
      border-radius: 8px;
      border: 1px solid #86efac;
      .hoverSideBarItem{
        padding: 10px 12px;
        transition: all 0.2s ease;
      }
      .hoverSideBarItem:hover{
        background: rgba(16, 185, 129, 0.2);
        border-left: 3px solid #10b981;
      }
    }
  }

.mainSmallMenuItem{
  position: relative;
  height: 50px;
  border-bottom: none;
  transition: all 0.2s ease;
  cursor: pointer;
  
  &.active {
    background: rgba(16, 185, 129, 0.25);
    border-left: 3px solid #10b981;
    
    .smallIcon {
      color: #064e3b;
    }
  }

  .SideBarListRight{
    display: none;
  }
  &:hover{
    background: rgba(16, 185, 129, 0.2);
    border-left: 3px solid #10b981;
    .smallIcon{
      color: #064e3b;
    }

    .SideBarListRight{
      display: block;
      background: linear-gradient(135deg, #d1fae5 0%, #a7f3d0 100%);
      box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
      border-radius: 8px;
      border: 1px solid #86efac;

      .hoverSideBarItem{
        padding: 10px 12px;
        transition: all 0.2s ease;
      }
      .hoverSideBarItem:hover{
        background: rgba(16, 185, 129, 0.2);
        border-left: 3px solid #10b981;
      }

    }
  }
}

.titleSideItem{
  height: 50px;
  border-bottom: 1px solid #86efac;
  background: linear-gradient(135deg, #a7f3d0 0%, #86efac 100%);
  border-radius: 8px 8px 0 0;
  
  a {
    color: #065f46 !important;
    font-weight: 600;
    transition: all 0.2s ease;
    font-size: 14px;
    
    &:hover {
      color: #064e3b !important;
    }
  }
}

.sidebar-header {
  background: linear-gradient(135deg, #a7f3d0 0%, #86efac 100%);
  border-bottom: 1px solid #6ee7b7;

  display: flex;
  align-items: center;
  justify-content: center;
}

.sidebar-title {
  font-size: 18px;
  font-weight: 700;
  letter-spacing: -0.02em;
  color: #065f46;
}

.sidebar-link-item {
  color: #495057 !important;
  
  .sidebar-link-text {
    color: #495057;
    font-weight: 500;
    transition: all 0.2s ease;
    font-size: 13px;
  }
  
  &:hover .sidebar-link-text {
    color: #212529;
    font-weight: 600;
  }
  
  &.active_link {
    background: #e9ecef;
    
    .sidebar-link-text {
      color: #212529;
      font-weight: 600;
    }
  }
}

.sidebar-dropdown-link {
  
  .sidebar-dropdown-text {
    font-weight: 500;
    transition: all 0.2s ease;
    font-size: 12px;
  }
  
  &:hover .sidebar-dropdown-text {
    font-weight: 600;
  }
  
  &.active_link {
    background: rgba(16, 185, 129, 0.3);
    border-left: 3px solid #10b981;
    
    .sidebar-dropdown-text {
      font-weight: 600;
    }
  }
}

</style>