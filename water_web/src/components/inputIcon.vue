<template>
  <div 
    class="input-wrapper" 
    @click="activeInput" 
    :class="{
      'input-wrapper--active': active, 
      'input-wrapper--error': error, 
      'input-wrapper--disabled': disabled
    }"
  >
    <div 
      v-if="icon" 
      class="input-icon-container"
      :class="{
        'input-icon-container--active': active, 
        'input-icon-container--default': !active && !error, 
        'input-icon-container--error': error
      }"
    >
      <mdb-icon 
        fab 
        v-if="fab" 
        :icon="icon" 
        class="input-icon"
      />
      <mdb-icon 
        v-else 
        :icon="icon" 
        class="input-icon"
      />
    </div>
    <input 
      :type="type" 
      @blur="notActiveInput"  
      ref="inputIcon"
      class="input-field"
      :placeholder="placeholder"
      @focus="activeInput" 
      :value="value" 
      @input="updateValue($event.target.value)"
      :disabled="disabled"
    >
    <div v-if="error" class="input-error-indicator"></div>
  </div>
</template>

<script>
import {mdbIcon} from 'mdbvue'
export default {
  components:{
    mdbIcon
  },
  props:{
    value: [Number, String],
    icon:{
      type : String,
      default : ''
    },
    
    type:{
      type : String,
      default : 'text'
    },
    placeholder:{
      type : String,
      default : ''
    },
    valid:Boolean,
    fab:Boolean,
    disabled:Boolean,

  },
  data() {
    return {
      active: false,
      data: '',
      error: false,
    }
  },
  methods: {
    updateValue(value){
      // console.log(value)
      // var x = value.replace(/\D/g, '').match(/(\d{0,2})(\d{0,3})(\d{0,4})/);
      // value = !x[2] ? x[1] : '(' + x[1] + ') ' + x[2] + (x[3] ? '-' + x[3] : '');
      this.data = value
      this.$emit('input', value)
    },
    activeInput(){
      this.active = true;
      this.error = false;
      // console.log(this.valid)
    },
    notActiveInput(){
      this.active = false;
      if(this.valid== true && this.data == ''){
        this.error = true
        this.active = false
      }
    },
    focus(){
      this.$refs.inputIcon.focus();
    }
  },
}
</script>

<style lang="scss" scoped>
.input-wrapper {
  position: relative;
  display: flex;
  align-items: center;
  width: 100%;
  height: 33px;
  background: #ffffff;
  border: 1.5px solid transparent;
  border-radius: 10px;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  overflow: hidden;
  cursor: text;
  background-image: linear-gradient(#ffffff, #ffffff), 
                    linear-gradient(135deg, #10b981 0%, #059669 50%, #047857 100%);
  background-origin: border-box;
  background-clip: padding-box, border-box;
  
  &:hover:not(.input-wrapper--disabled):not(.input-wrapper--active):not(.input-wrapper--error) {
    background-image: linear-gradient(#ffffff, #ffffff), 
                      linear-gradient(135deg, #34d399 0%, #10b981 50%, #059669 100%);
    box-shadow: 0 2px 8px rgba(16, 185, 129, 0.15);
    transform: translateY(-1px);
  }

  &--active {
    background-image: linear-gradient(#ffffff, #ffffff), 
                      linear-gradient(135deg, #10b981 0%, #059669 50%, #047857 100%);
    box-shadow: 0 0 0 3px rgba(16, 185, 129, 0.1), 
                0 4px 12px rgba(16, 185, 129, 0.15);
    transform: translateY(-1px);
  }

  &--error {
    background-image: linear-gradient(#ffffff, #ffffff), 
                      linear-gradient(135deg, #ef4444 0%, #dc2626 50%, #b91c1c 100%);
    box-shadow: 0 0 0 3px rgba(239, 68, 68, 0.1), 
                0 4px 12px rgba(239, 68, 68, 0.15);
    animation: shake 0.4s ease-in-out;
  }

  &--disabled {
    background-color: #ffffff;
    background-image: linear-gradient(#f8fafc, #f8fafc), 
                      linear-gradient(135deg, #d1fae5 0%, #a7f3d0 50%, #86efac 100%);
    cursor: not-allowed;
    opacity: 0.6;
  }
}

@keyframes shake {
  0%, 100% { transform: translateX(0); }
  25% { transform: translateX(-5px); }
  75% { transform: translateX(5px); }
}

.input-icon-container {
  display: flex;
  align-items: center;
  justify-content: center;
  width: 48px;
  height: 33px;
  flex-shrink: 0;
  transition: all 0.3s ease;
  position: relative;

  &::after {
    content: '';
    position: absolute;
    right: 0;
    top: 50%;
    transform: translateY(-50%);
    width: 1px;
    height: 60%;
    background: #e1e8ed;
    transition: all 0.3s ease;
  }

  &--default {
    color: #64748b;
    
    .input-icon {
      color: #94a3b8;
      transition: all 0.3s ease;
    }
  }

  &--active {
    color: #10b981;
    
    &::after {
      background: linear-gradient(to bottom, 
        rgba(16, 185, 129, 0.2), 
        rgba(16, 185, 129, 0.4), 
        rgba(16, 185, 129, 0.2)
      );
      width: 2px;
    }
    
    .input-icon {
      color: #10b981;
      transform: scale(1.1);
    }
  }

  &--error {
    color: #ef4444;
    
    &::after {
      background: linear-gradient(to bottom, 
        rgba(239, 68, 68, 0.2), 
        rgba(239, 68, 68, 0.4), 
        rgba(239, 68, 68, 0.2)
      );
      width: 2px;
    }
    
    .input-icon {
      color: #ef4444;
      animation: pulse 2s infinite;
    }
  }
}

@keyframes pulse {
  0%, 100% { opacity: 1; }
  50% { opacity: 0.7; }
}

.input-icon {
  font-size: 18px;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  z-index: 1;
}

.input-field {
  flex: 1;
  width: 100%;
  padding: 6px 14px;
  font-size: 15px;
  font-weight: 400;
  color: #1e293b;
  background: transparent;
  border: none;
  outline: none;
  appearance: none;
  transition: all 0.2s ease;
  
  &::placeholder {
    color: #94a3b8;
    font-weight: 400;
    transition: color 0.2s ease;
  }

  &:focus::placeholder {
    color: #cbd5e0;
  }

  &:disabled {
    cursor: not-allowed;
    color: #64748b;
  }

  // Calendar picker styling
  &[type="date"],
  &[type="datetime-local"],
  &[type="time"] {
    &::-webkit-calendar-picker-indicator {
      cursor: pointer;
      opacity: 0.6;
      transition: opacity 0.2s ease;
      
      &:hover {
        opacity: 1;
      }
    }
  }

  // Number input arrows styling
  &[type="number"] {
    &::-webkit-inner-spin-button,
    &::-webkit-outer-spin-button {
      opacity: 0.5;
      transition: opacity 0.2s ease;
      
      &:hover {
        opacity: 1;
      }
    }
  }
}

.input-error-indicator {
  position: absolute;
  top: 50%;
  right: 12px;
  transform: translateY(-50%);
  width: 8px;
  height: 8px;
  background: #ef4444;
  border-radius: 50%;
  animation: pulse-dot 2s infinite;
  box-shadow: 0 0 0 0 rgba(239, 68, 68, 0.7);
}

@keyframes pulse-dot {
  0% {
    transform: translateY(-50%) scale(1);
    box-shadow: 0 0 0 0 rgba(239, 68, 68, 0.7);
  }
  50% {
    transform: translateY(-50%) scale(1.2);
    box-shadow: 0 0 0 4px rgba(239, 68, 68, 0);
  }
  100% {
    transform: translateY(-50%) scale(1);
    box-shadow: 0 0 0 0 rgba(239, 68, 68, 0);
  }
}

// Responsive design
@media (max-width: 768px) {
  .input-wrapper {
    height: 33px;
  }

  .input-icon-container {
    width: 44px;
    height: 33px;
  }

  .input-field {
    padding: 6px 12px;
    font-size: 14px;
  }

  .input-icon {
    font-size: 16px;
  }
}

// Dark mode support (optional) - background oq bo'lishi kerak, shuning uchun dark mode ni o'chirib qo'yamiz
// @media (prefers-color-scheme: dark) {
//   .input-wrapper {
//     background: #ffffff;
//     background-image: linear-gradient(#ffffff, #ffffff), 
//                       linear-gradient(135deg, #3b82f6 0%, #2563eb 50%, #1d4ed8 100%);
//   }
// }
</style>