<template>
  <div class="erp-select">
    <div 
      class="select-wrapper" 
      @click="select_input" 
      :class="[
        {'select-focused': change, 'select-filled': change_color},
        {'select-md': size=='md', 'select-sm': size == 'sm'}
      ]"
    >
      <input 
        type="text" 
        class="select-input" 
        :value="selected" 
        @keyup="hideInput1" 
        readonly
      />
      <label v-if="selected == ''" class="select-label">
        {{$t('select_item')}}
      </label>
      <div class="select-icon">
        <i class="fa fa-chevron-down" :class="{'icon-rotate': areOptionsVisible}"></i> 
      </div>
    </div>

    <div class="select-dropdown" v-if="areOptionsVisible">
      <div class="dropdown-header">
        <div class="search-wrapper">
          <i class="fa fa-search search-icon"></i>
          <input 
            :placeholder="$t('search_here')" 
            @keydown.tab="hideInput2" 
            @keyup.39="EnterSelect" 
            @keyup.37="EnterSelect" 
            @keyup.40="downSelect" 
            @keyup.38="upSelect"
            v-model="search" 
            :ref="selected" 
            class="search-input" 
          />
        </div>
        <mdb-btn 
          v-if="btnshow" 
          class="add-btn" 
          color="primary"
          @click="btn_add" 
          size="sm"
        >
          <i class="fa fa-plus mr-1"></i>
          {{$t('add')}}
        </mdb-btn>
      </div>

      <div class="dropdown-list">
        <div 
          v-for="(option,i) in filteredList" 
          :key="i" 
          :class="{'option-active': i==downIndex, 'option-item': true}"
          @click="selectOption(option)"
        >
          {{option.fio}}
        </div>
        <div v-if="filteredList.length === 0" class="no-results">
          {{$t('no_results') || 'No results found'}}
        </div>
      </div>
    </div>

    <div>
      <massage_box 
        :hide="modal_status" 
        :detail_info="modal_info"
        :m_text="$t('Failed_to_add')" 
        @to_hide_modal="modal_status = false"
      />
    </div>
  </div>
</template>

<script>
import { mdbInput, mdbBtn, mdbIcon} from 'mdbvue';

export default {
  name: 'erp-select',
  components: {mdbInput, mdbBtn, mdbIcon},
  props: {
    options: {
      type: Array,
      default() {
        return []
      }
    },
    btnshow: {
      type: Boolean,
      default: false
    },
    selected: {
      type: String,
      default: ''
    },
    label: {
      type: String,
      default: 'Dropdown'
    },
    url: {
      type: String,
      default: ''
    },
    btnadd: {
      type: Boolean,
      default: false
    },
    size: {
      type: String,
      default: 'md'
    },
  },
  data() {
    return {
      alert_text: '',
      alert_success: false,
      alert_danger: false,
      modal_info: '',
      modal_status: false,
      downIndex: -1,
      search: '',
      areOptionsVisible: false,
      num: 0,
      change: false,
      change_color: false
    }
  },
  mounted() {
    if(this.selected !== '') {
      this.change = false
      this.change_color = true
    }
  },
  methods: {
    downSelect() {
      this.downIndex++;
      if(this.downIndex > this.filteredList.length - 1) {
        this.downIndex = 0;
      }
    },
    upSelect() {
      this.downIndex--;
      if(this.downIndex < 0) {
        this.downIndex = this.filteredList.length - 1;
      }
    },
    EnterSelect() {
      if(this.downIndex >= 0 && this.filteredList[this.downIndex]) {
        this.$emit('select', this.filteredList[this.downIndex]);
        this.areOptionsVisible = false;
      }
    },
    selectOption(option) {
      this.$emit('select', option);
      this.areOptionsVisible = false;
      this.search = '';
    },
    btn_add() {
      this.$emit('btn_add');
      this.areOptionsVisible = false;
    },
    hideInput1() {
      this.select_input();
    },
    hideInput2() {
      this.select_input();
      this.change = false
    },
    select_input() {
      this.num = 0
      this.downIndex = -1;
      this.change = true
      this.change_color = false
      this.$nextTick(function () {
        if(this.$refs[this.selected]) {
          this.$refs[this.selected].focus();
        }
        this.search = '';
      })
      this.areOptionsVisible = !this.areOptionsVisible;
      document.addEventListener("click", this.add_fun);
    },
    add_fun(e) {
      if(this.num != 0) {
        if(e.target.closest(".search-input") || e.target.closest(".select-dropdown")) return
        this.areOptionsVisible = false;
        if(this.selected === '') {
          this.change = false
        }
        if(this.selected !== '') {
          this.change_color = true;
        }
        document.removeEventListener("click", this.add_fun)
      }
      this.num++
    }
  },
  computed: {
    filteredList: function() {
      if(this.search) {
        return this.options.filter((item) => {
          return this.search.toLowerCase().split(' ').every(v => 
            item.fio.toLowerCase().includes(v)
          )
        })
      } else {
        return this.options;
      }
    }
  }
}
</script>

<style lang="scss" scoped>
// Custom Scrollbar
::-webkit-scrollbar {
  width: 6px;
}

::-webkit-scrollbar-track {
  background: #f5f5f5;
  border-radius: 3px;
}

::-webkit-scrollbar-thumb {
  background: #c1c1c1;
  border-radius: 3px;
  transition: background 0.2s;
}

::-webkit-scrollbar-thumb:hover {
  background: #a8a8a8;
}

// Main Container
.erp-select {
  position: relative;
  width: 100%;
  font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
}

// Select Wrapper
.select-wrapper {
  position: relative;
  width: 100%;
  background: #fff;
  border-radius: 8px;
  border: 1.5px solid #e0e0e0;
  transition: all 0.3s cubic-bezier(0.4, 0, 0.2, 1);
  cursor: pointer;
  overflow: hidden;

  &:hover {
    border-color: #bdbdbd;
  }

  &.select-focused {
    border-color: #4285F4;
    box-shadow: 0 0 0 3px rgba(66, 133, 244, 0.1);
  }

  &.select-filled {
    border-color: #4285F4;
  }

  &.select-md {
    height: 40px;
  }

  &.select-sm {
    height: 36px;
  }
}

// Input
.select-input {
  width: 100%;
  height: 100%;
  padding: 0 40px 0 12px;
  border: none;
  outline: none;
  background: transparent;
  font-size: 14px;
  color: #212121;
  cursor: pointer;
  pointer-events: none;
  font-weight: 400;
}

// Label
.select-label {
  position: absolute;
  left: 12px;
  top: 50%;
  transform: translateY(-50%);
  font-size: 14px;
  color: #9e9e9e;
  pointer-events: none;
  transition: all 0.2s ease;
  background: #fff;
  padding: 0 4px;
  z-index: 1;
}

.select-focused .select-label,
.select-filled .select-label {
  top: 0;
  font-size: 11px;
  color: #4285F4;
  font-weight: 500;
  transform: translateY(-50%);
}

.select-sm .select-label {
  font-size: 13px;
}

.select-sm.select-focused .select-label,
.select-sm.select-filled .select-label {
  font-size: 10px;
}

// Icon
.select-icon {
  position: absolute;
  right: 0;
  top: 0;
  height: 100%;
  width: 36px;
  display: flex;
  align-items: center;
  justify-content: center;
  color: #757575;
  pointer-events: none;
  transition: transform 0.3s ease;

  i {
    font-size: 12px;
    transition: transform 0.3s ease;
  }

  .icon-rotate {
    transform: rotate(180deg);
  }
}

// Dropdown
.select-dropdown {
  position: absolute;
  top: calc(100% + 4px);
  left: 0;
  right: 0;
  z-index: 1000;
  background: #fff;
  border-radius: 8px;
  box-shadow: 0 4px 12px rgba(0, 0, 0, 0.15);
  border: 1px solid #e0e0e0;
  overflow: hidden;
  animation: slideDown 0.2s ease;
  max-height: 320px;
  display: flex;
  flex-direction: column;
}

@keyframes slideDown {
  from {
    opacity: 0;
    transform: translateY(-8px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}

// Dropdown Header
.dropdown-header {
  padding: 8px;
  border-bottom: 1px solid #f0f0f0;
  display: flex;
  gap: 8px;
  align-items: center;
}

// Search Wrapper
.search-wrapper {
  flex: 1;
  position: relative;
  display: flex;
  align-items: center;
}

.search-icon {
  position: absolute;
  left: 10px;
  color: #9e9e9e;
  font-size: 13px;
  pointer-events: none;
  z-index: 1;
}

.search-input {
  width: 100%;
  padding: 8px 12px 8px 32px;
  border: 1px solid #e0e0e0;
  border-radius: 6px;
  font-size: 13px;
  outline: none;
  transition: all 0.2s;

  &:focus {
    border-color: #4285F4;
    box-shadow: 0 0 0 2px rgba(66, 133, 244, 0.1);
  }

  &::placeholder {
    color: #bdbdbd;
  }
}

// Add Button
.add-btn {
  white-space: nowrap;
  padding: 6px 12px;
  font-size: 12px;
  border-radius: 6px;
  box-shadow: 0 2px 4px rgba(66, 133, 244, 0.2);
  transition: all 0.2s;

  &:hover {
    box-shadow: 0 4px 8px rgba(66, 133, 244, 0.3);
    transform: translateY(-1px);
  }
}

// Dropdown List
.dropdown-list {
  max-height: 240px;
  overflow-y: auto;
  padding: 4px 0;
}

// Option Item
.option-item {
  padding: 10px 16px;
  font-size: 13px;
  color: #212121;
  cursor: pointer;
  transition: all 0.15s ease;
  border-left: 3px solid transparent;

  &:hover {
    background: #f5f5f5;
    border-left-color: #4285F4;
  }

  &.option-active {
    background: #e3f2fd;
    border-left-color: #4285F4;
    color: #1976d2;
    font-weight: 500;
  }
}

// No Results
.no-results {
  padding: 20px 16px;
  text-align: center;
  font-size: 13px;
  color: #9e9e9e;
  font-style: italic;
}

// Responsive
@media (max-width: 768px) {
  .select-wrapper {
    &.select-md {
      height: 44px;
    }

    &.select-sm {
      height: 40px;
    }
  }

  .select-input {
    font-size: 16px; // Prevent zoom on iOS
  }
}
</style>
