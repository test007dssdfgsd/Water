<template>
  <div  class="modal_template text-dark" style="z-index:9999999; ">
   <!-- <mdb-btn class="" color="success" m="b4" @click="show = !show"  type="submit"  p="r4 l4 t2 b2">show modal</mdb-btn> -->
   <transition name="fade" style="z-index: 10000;" appear>
     <div class="modal-overlay"  @click="close" v-if="show">
     </div>
   </transition>
   <transition name="slide"  appear>
     <div class="card modal_FP " style="z-index: 999999;" :style="{'width': width}" v-show="show">
       <div class="card-header d-flex justify-content-between modal_head_shadow border-bottom" :class="'bg-' + headerbackColor">
         <h5 class="m-0 mt-1" :style="{'color': titlecolor}" style="font-size:16px;">{{title}}</h5>
         <svg @click="close" xmlns="http://www.w3.org/2000/svg" style="border-radius: 50%; cursor:pointer;" class="icon icon-tabler icon-tabler-x bg-danger p-1" width="21" height="21" viewBox="0 0 24 24" stroke-width="2.7" stroke="#fff" fill="none" stroke-linecap="round" stroke-linejoin="round">
          <path stroke="none" d="M0 0h24v24H0z" fill="none"/>
          <line x1="18" y1="6" x2="6" y2="18" />
          <line x1="6" y1="6" x2="18" y2="18" />
        </svg>
       </div>
       <div class="modal_slot">
          <slot name="body">
          </slot>
          <!-- <div>
            <mdb-btn class="" color="success" m="b4" @click="show = !show"  type="submit"  p="r4 l4 t2 b2">Close</mdb-btn>
          </div> -->
       </div>
     </div>
    </transition>
  </div>
</template>

<script>
// import {mdbBtn} from "mdbvue";
export default {
  data(){
    return{
      clos: this.show,
    }
  },
  props:{
    show:{
        type:Boolean,
        default:false
      },
    width:{
      type: String,
      default: ''
    },
    closeColor:{
      type: String,
      default: '#ff2825'
    },
    titlecolor:{
      type: String,
      default: 'black'
    },
    headerbackColor:{
      type: String,
      default: 'white'
    },
    title:{
      type: String,
      default: ''
    }
  },
  methods:{
    close(){
      this.$emit('close');
    }
  }
  // components: {
  //   mdbBtn
  // }
}
</script>

<style lang="scss">
  .modal_RP{
    z-index: 11111111;
    position: relative;
    display: flex;
    justify-content: center;
    align-items: center;
    width: 100%;
    max-height: 100vh;
    overflow-x: hidden;
  }
  .modal-overlay{
    position: fixed;
    top:0;
    left: 0;
    right: 0;
    bottom: 0;
    z-index: 999999;
    background: rgba(2, 2, 2,0.7);
  }
  .fade-enter-active, 
  .fade-leave-active{
    transition: opacity 0.5s;
  }
  .fade-enter,
  .fade-leave-to{
    opacity: 0;
  }
  .modal_FP{
    position: fixed;
    top: 50%;
    left: 50%;
    transform: translate(-50%, -51%);
    z-index: 110;
    max-height: 95vh;
    display: flex;
    flex-direction: column;
    
    @media (max-width: 767px) {
      top: 0;
      left: 0;
      right: 0;
      bottom: 0;
      transform: none;
      max-height: 100vh;
      width: 100% !important;
      border-radius: 0;
    }
    
     .modal_head_shadow{
      box-shadow: 0px 2px 3px #eee;
      position: sticky;
      top: 0;
      z-index: 10;
      flex-shrink: 0;
      
      @media (max-width: 767px) {
        padding: 12px 15px !important;
        
        h5 {
          font-size: 16px !important;
          font-weight: 600;
        }
        
        svg {
          width: 24px !important;
          height: 24px !important;
        }
      }
    }
  }
  .modal_slot{
    max-height: calc(95vh - 60px);
    overflow: hidden;
    overflow-y: auto;
    flex: 1;
    
    @media (max-width: 767px) {
      max-height: calc(100vh - 60px);
    }
  }
  .modal_FP .card-body{
    max-height: 680px;
    overflow: hidden;
    overflow-y: auto;
  }
   .slide-enter-active{
     transition: transform 0.6s, opacity 0.6s;
   } 
  .slide-leave-active{

    transition: transform 1s, opacity 1s;
    
  }
  .slide-enter,
  .slide-leave-to{
    transform:translateY(-220%) translateX(-50%) ;
    opacity: 0;

  }
</style>