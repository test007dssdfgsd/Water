<template>
  <div class="pagination">
    <!-- Previous -->
    <button 
      class="page-btn"
      :disabled="currentPage === 0" 
      @click="changePage(currentPage - 1)">
      ‹
    </button>

    <!-- Pages -->
    <span 
      v-for="page in visiblePages" 
      :key="page" 
      @click="page !== '...' && changePage(page - 1)"
      :class="['page-number', { active: currentPage === page - 1, dots: page === '...' }]">
      {{ page }}
    </span>

    <!-- Next -->
    <button 
      class="page-btn"
      :disabled="currentPage >= totalPages - 1" 
      @click="changePage(currentPage + 1)">
      ›
    </button>
  </div>
</template>

<script>
export default {
  props: {
    totalPages: { type: Number, required: true },
    currentPage: { type: Number, required: true }
  },
  computed: {
    visiblePages() {
      const pages = []
      const maxVisible = 9

      if (this.totalPages <= maxVisible) {
        for (let i = 1; i <= this.totalPages; i++) {
          pages.push(i)
        }
      } else {
        if (this.currentPage < 5) {
          pages.push(1, 2, 3, 4, 5, 6, "...", this.totalPages)
        } else if (this.currentPage > this.totalPages - 6) {
          pages.push(1, "...", this.totalPages-5, this.totalPages-4, this.totalPages-3, this.totalPages-2, this.totalPages-1, this.totalPages)
        } else {
          pages.push(1, "...", this.currentPage, this.currentPage+1, this.currentPage+2, "...", this.totalPages)
        }
      }
      return pages
    }
  },
  methods: {
    changePage(page) {
      if (page >= 0 && page < this.totalPages) {
        this.$emit("page-changed", page)
      }
    }
  }
}
</script>

<style scoped>
.pagination {
  display: flex;
  gap: 6px;
  justify-content: center;
  align-items: center;
  margin-top: 10px;
  flex-wrap: wrap;
}

.page-btn,
.page-number {
  min-width: 28px;
  height: 28px;
  padding: 0 10px;
  border-radius: 8px;
  border: 1px solid #dcdcdc;
  background: #fff;
  cursor: pointer;
  font-size: 14px;
  transition: all 0.2s ease;
  display: flex;
  align-items: center;
  justify-content: center;
}

.page-btn:hover:not(:disabled),
.page-number:hover:not(.active):not(.dots) {
  background: #f0f0f0;
}

.page-btn:disabled {
  opacity: 0.5;
  cursor: not-allowed;
}

.page-number.active {
  background: #007bff;
  color: white;
  border-color: #007bff;
}

.page-number.dots {
  cursor: default;
  border: none;
  background: transparent;
  font-weight: bold;
}
</style>

