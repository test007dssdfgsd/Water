<template>
  <div class="login-app">
    <loader v-if="loading"/>
    
    <!-- Language Switcher -->
    <div class="language-switcher">
      <mdb-btn color="elegant" @click="rus_btn" class="lang-btn">{{$t('ru')}}</mdb-btn>
    </div>

    <!-- Keyboards -->
    <krillKeyboard @text="matn" v-show="klavish && ruskiy" style="position: absolute; bottom: 0; right: 0;"/>
    <numberKeyboard @number="num_func" v-show="raqam" style="position: absolute; bottom: 10px; right: 0;"/>
    
    <!-- Login Form Container -->
    <div v-if="admin_btn" class="login-container">
      <div class="login-card">
        <!-- Header -->
        <div class="login-header">
          <h2 class="login-title">{{$t('authorization')}}</h2>
        </div>

        <!-- Error Message -->
        <div class="error-message" v-if="error">
          <span class="error-text">{{this.error}}</span>
        </div>

        <!-- Login Form -->
        <form v-on:submit.prevent="submit" class="login-form">
          <!-- Username Field -->
          <div class="form-group">
            <div class="input-wrapper">
              <div class="input-icon">
                <svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-user" width="24" height="24" viewBox="0 0 24 24" stroke-width="1.5" stroke="#667eea" fill="none" stroke-linecap="round" stroke-linejoin="round">
                  <path stroke="none" d="M0 0h24v24H0z" fill="none"/>
                  <circle cx="12" cy="7" r="4" />
                  <path d="M6 21v-2a4 4 0 0 1 4 -4h4a4 4 0 0 1 4 4v2" />
                </svg>
              </div>
              <input 
                type="text" 
                v-model="login" 
                :placeholder="$t('username')"
                class="form-input"
              >
              <small class="invalid-text" v-if="$v.login.$dirty && !$v.login.required">
                {{$t('name_invalid_text')}}
              </small>
            </div>
          </div>

          <!-- Password Field -->
          <div class="form-group">
            <div class="input-wrapper">
              <div class="input-icon">
                <svg xmlns="http://www.w3.org/2000/svg" class="icon icon-tabler icon-tabler-lock" width="24" height="24" viewBox="0 0 24 24" stroke-width="1.5" stroke="#667eea" fill="none" stroke-linecap="round" stroke-linejoin="round">
                  <path stroke="none" d="M0 0h24v24H0z" fill="none"/>
                  <rect x="5" y="11" width="14" height="10" rx="2" />
                  <circle cx="12" cy="16" r="1" />
                  <path d="M8 11v-4a4 4 0 0 1 8 0v4" />
                </svg>
              </div>
              <input 
                type="password" 
                v-model="pass"  
                @input="handleHashing($event.target.value)" 
                :placeholder="$t('password')"
                class="form-input"
              >
              <small class="invalid-text" v-if="$v.pass.$dirty && !$v.pass.required">
                {{$t('name_invalid_text')}}
              </small>
            </div>
          </div>

          <!-- Submit Button -->
          <div class="form-actions">
            <mdb-btn color="primary" type="submit" class="submit-btn">
              <i class="fas fa-sign-in-alt"></i>
              {{$t('enter')}}
            </mdb-btn>
          </div>
        </form>
      </div>
    </div>
    
    <Toast ref="message"></Toast>
  </div>
</template>

<script>
import krillKeyboard from './krillKeyboard'
import numberKeyboard from './numberKeyboard'
import { required } from 'vuelidate/lib/validators'
import {mdbBtn,mdbInput } from "mdbvue";
import {mapActions, mapGetters} from "vuex"
import md5 from 'js-md5'
export default {
  components: {
    mdbBtn,mdbInput, krillKeyboard, numberKeyboard
  },
  validations: {
    login: {required},
    pass: {required},
  },
  data (){
      return{ 
        list: [],
        pass: '',
        login: '',
        admin_btn : true,
        user_btn : false,
        search_data : '',
        md: '',
        error: '',
        mahbus: [],
        show_search: false,
        loading: false,
        input: '',
        izlash: false,
        klavish: false,
        inglish: false,
        ruskiy: true,
        raqam: false,
        keshRus: '',
      }
    },
  async mounted(){  
    await this.fetchUser();
    localStorage.AuthId = 1;
    localStorage.CompId = 1;
    localStorage.CompName = "Company"
    localStorage.sidebar = false;
    localStorage.Login = ""
    localStorage.AccessType = 0;
    localStorage.Type = 0;
    localStorage.postavchikAuthId = null;
    localStorage.postavchikId = null;
    localStorage.postavchikName = '';

    localStorage.size_value = 50
    localStorage.numPage = 1
    localStorage.pageNum = 1
    localStorage.Items_count = 10
    console.log(this.allUser)
    if(this.allUser.rows.length == 0){
      localStorage.Login = 'admin';
      localStorage.AuthId = 1;
      localStorage.CompId = 1;
      localStorage.AccessType = 0;
      localStorage.CompName = "Company"
      localStorage.sidebar = false;
      localStorage.Type = 0;
      this.$router.push('/user')
    }
  },
  computed: mapGetters(['allUser']),
  watch: {
    search_data: function(){
      this.search_click();
    }
  },
  methods: {
    ...mapActions(['fetchUser']),
    handleHashing (data) {
        this.pass = data
        this.md = md5(data)
      },
      rus_btn(){
        this.$i18n.locale = 'ru'
        this.search_data = this.keshRus;
        this.input = '';
        this.ruskiy = true;
        this.inglish = false;
      },
      en_btn(){
        this.$i18n.locale = 'en'
        this.search_data = '';
        this.input = '';
        this.ruskiy = false;
        this.inglish = true;
      },
      matn(option){
        this.search_data = option;
        this.keshRus = option;
      },
      num_func(option){
        this.md = option;
      },
      parol_Mahbus(){
        this.raqam = true;
        this.klavish = false;
      },
      
       onChange(input) {
        this.input = input;
        this.search_data = this.input
      },

    async submit(){
         if(this.$v.$invalid )
          {
            this.$v.$touch();
            this.$refs.message.warning('please_fill')
            return false;
          }
        try{
          this.loading = true;
          console.log(this.login)
          console.log(this.md)
            const response = await fetch(this.$store.state.hostname + '/WaterAuths/checkAuth?login=' + this.login + '&password=' + this.md)
            const data = await response.json()  
            this.loading = false;
            console.log('data')
            console.log(data)
          if(response.status == '200' || response.status == '201'){
            localStorage.Login = data.password;
            localStorage.AuthId = data.id;
            localStorage.CompId = 1;
            localStorage.UserId = data.waterUserid;
            localStorage.Type = data.user_type;
            localStorage.AccessType = data.user_type;
            await this.fetchUserName(data.waterUserid);
            if(data.user_type == 0){
              this.$router.push('/order_Add/0')
              return
            }
            else if(data.user_type == 1){
              this.$router.push('/postavchik_list')
              return
            }
            this.$router.push('/user')
          }
          else{
              this.$refs.message.error("error_login")
              this.error = this.$i18n.t("error_login")
          }
        }
        catch{
            this.loading = false;
            this.$refs.message.error("network_ne_connect")
        }
    },
    async fetchUserName(id){
      try{
      this.loading = true;
      const res = await fetch(this.$store.state.hostname + '/WaterUsers/'+ id);
      this.loading = false;
      if(res.status == 200 || res.status == 201){
        const data = await res.json()
        localStorage.UserName = data.fio;
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
    async search_btn(item) {
      this.login = item.fio;
      this.show_search = false;
      this.search_data = item.fio
    },
    async search_click()
    {
      if(this.search_data != ''){
        this.show_search = true;
        const res = await fetch(this.$store.state.hostname + '/Users/searchUserByFIO?FIO=' + this.search_data + '&countResult=10');
        const data = await res.json();
        console.log(data);
        this.mahbus = data;
      }
      else{
        this.mahbus = []
      }
    } 
  }
}
</script>

<style lang="scss" scoped>
.login-app {
  min-height: 100vh;
  background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  font-family: 'Open Sans', sans-serif;
  display: flex;
  justify-content: center;
  align-items: center;
  position: relative;
  padding: 20px;
}

.language-switcher {
  position: fixed;
  top: 20px;
  right: 20px;
  z-index: 100;
  
  .lang-btn {
    padding: 8px 16px;
    border-radius: 20px;
    font-size: 13px;
    font-weight: 600;
    box-shadow: 0 2px 10px rgba(0,0,0,0.1);
    transition: all 0.3s;
    
    &:hover {
      transform: translateY(-2px);
      box-shadow: 0 4px 15px rgba(0,0,0,0.2);
    }
  }
}

.login-container {
  width: 100%;
  max-width: 450px;
  display: flex;
  justify-content: center;
  align-items: center;
}

.login-card {
  background: white;
  border-radius: 20px;
  box-shadow: 0 10px 40px rgba(0,0,0,0.2);
  width: 100%;
  padding: 40px;
  animation: slideUp 0.5s ease-out;
}

@keyframes slideUp {
  from {
    opacity: 0;
    transform: translateY(30px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

.login-header {
  text-align: center;
  margin-bottom: 30px;
  
  .login-title {
    color: #333;
    font-size: 28px;
    font-weight: 700;
    margin: 0;
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    -webkit-background-clip: text;
    -webkit-text-fill-color: transparent;
    background-clip: text;
  }
}

.error-message {
  text-align: center;
  margin-bottom: 20px;
  
  .error-text {
    color: #dc3545;
    font-size: 14px;
    font-weight: 600;
    padding: 10px 15px;
    background: #f8d7da;
    border-radius: 8px;
    display: inline-block;
  }
}

.login-form {
  .form-group {
    margin-bottom: 25px;
    
    .input-wrapper {
      position: relative;
      display: flex;
      align-items: center;
      background: #f8f9fa;
      border: 2px solid #e0e0e0;
      border-radius: 12px;
      padding: 0 15px;
      transition: all 0.3s;
      
      &:focus-within {
        border-color: #667eea;
        background: white;
        box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
      }
      
      .input-icon {
        display: flex;
        align-items: center;
        margin-right: 12px;
        flex-shrink: 0;
      }
      
      .form-input {
        flex: 1;
        height: 50px;
        border: none;
        outline: none;
        background: transparent;
        font-size: 16px;
        color: #333;
        font-weight: 500;
        
        &::placeholder {
          color: #999;
          font-weight: 400;
        }
      }
      
      .invalid-text {
        position: absolute;
        top: 100%;
        left: 0;
        margin-top: 5px;
        color: #dc3545;
        font-size: 12px;
        font-weight: 600;
      }
    }
  }
  
  .form-actions {
    margin-top: 30px;
    
    .submit-btn {
      width: 100%;
      height: 50px;
      border-radius: 12px;
      font-size: 16px;
      font-weight: 600;
      background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
      border: none;
      box-shadow: 0 4px 15px rgba(102, 126, 234, 0.4);
      transition: all 0.3s;
      display: flex;
      align-items: center;
      justify-content: center;
      gap: 10px;
      
      &:hover {
        transform: translateY(-2px);
        box-shadow: 0 6px 20px rgba(102, 126, 234, 0.5);
      }
      
      &:active {
        transform: translateY(0);
      }
    }
  }
}

// Mobile Styles
@media (max-width: 767px) {
  .login-app {
    padding: 15px;
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
  }
  
  .language-switcher {
    top: 15px;
    right: 15px;
    
    .lang-btn {
      padding: 6px 12px;
      font-size: 12px;
    }
  }
  
  .login-card {
    padding: 30px 20px;
    border-radius: 16px;
  }
  
  .login-header {
    margin-bottom: 25px;
    
    .login-title {
      font-size: 24px;
    }
  }
  
  .login-form {
    .form-group {
      margin-bottom: 20px;
      
      .input-wrapper {
        padding: 0 12px;
        border-radius: 10px;
        
        .input-icon {
          margin-right: 10px;
          
          svg {
            width: 20px;
            height: 20px;
          }
        }
        
        .form-input {
          height: 45px;
          font-size: 15px;
        }
      }
    }
    
    .form-actions {
      margin-top: 25px;
      
      .submit-btn {
        height: 45px;
        font-size: 15px;
      }
    }
  }
}

// Small Mobile
@media (max-width: 480px) {
  .login-app {
    padding: 10px;
  }
  
  .login-card {
    padding: 25px 18px;
    border-radius: 12px;
  }
  
  .login-header {
    margin-bottom: 20px;
    
    .login-title {
      font-size: 22px;
    }
  }
  
  .login-form {
    .form-group {
      margin-bottom: 18px;
      
      .input-wrapper {
        padding: 0 10px;
        
        .form-input {
          height: 42px;
          font-size: 14px;
        }
      }
    }
    
    .form-actions {
      margin-top: 20px;
      
      .submit-btn {
        height: 42px;
        font-size: 14px;
      }
    }
  }
}

// Desktop Styles
@media (min-width: 768px) {
  .login-card {
    padding: 50px 45px;
  }
  
  .login-header {
    .login-title {
      font-size: 32px;
    }
  }
  
  .login-form {
    .form-group {
      .input-wrapper {
        .form-input {
          font-size: 17px;
        }
      }
    }
  }
}
</style>
