<template>
  <div class="app-container">
    <div class="filter-container">
      <el-date-picker
        v-model="listQuery.created_at"
        size="small"
        type="datetimerange"
        range-separator="-"
        start-placeholder="开始日期"
        end-placeholder="结束日期"
        value-format="yyyy-MM-dd HH:mm:ss"
        class="filter-item"
        :editable="false"
      />
      <el-select
        v-model="listQuery.level"
        size="small"
        placeholder="选择日志级别"
        clearable
        class="filter-item"
      >
        <el-option label="DEBUG" :value="'DEBUG'" />
        <el-option label="INFO" :value="'INFO'" />
        <el-option label="WARNING" :value="'WARNING'" />
        <el-option label="ERROR" :value="'ERROR'" />
      </el-select>
      <el-select
        v-model="listQuery.method"
        size="small"
        placeholder="选择请求方法"
        clearable
        class="filter-item"
      >
        <el-option label="GET" :value="'GET'" />
        <el-option label="POST" :value="'POST'" />
        <el-option label="PUT" :value="'PUT'" />
        <el-option label="PATCH" :value="'PATCH'" />
        <el-option label="DELETE" :value="'DELETE'" />
      </el-select>
      <el-input
        v-model="listQuery.keyword"
        size="small"
        placeholder="请输入关键词"
        clearable
        class="filter-item w-200"
      />
      <el-button-group class="filter-item">
        <el-button
          size="small"
          type="primary"
          icon="el-icon-search"
          @click="search"
        >
          搜索
        </el-button>
        <el-button
          size="small"
          type="primary"
          icon="el-icon-refresh"
          @click="refresh"
        >
          重置
        </el-button>
      </el-button-group>
    </div>

    <el-table
      v-loading="listLoading"
      :data="list"
      element-loading-text="Loading"
      border
      fit
      height="100%"
      class="table-container"
      highlight-current-row
    >
      <el-table-column
        align="center"
        label="ID"
        width="95"
      >
        <template slot-scope="scope">
          {{ scope.row.id }}
        </template>
      </el-table-column>
      <el-table-column
        label="行为描述"
      >
        <template slot-scope="scope">
          {{ scope.row.content }}
        </template>
      </el-table-column>
      <el-table-column
        label="事件类型"
        width="150"
        align="center"
      >
        <template slot-scope="scope">
          {{ scope.row.eventTypeName }}
        </template>
      </el-table-column>
      
      <el-table-column
        label="备注"
      >
        <template slot-scope="scope">
          {{ scope.row.remark }}
        </template>
      </el-table-column>

      <el-table-column
        label="操作人"
        width="150"
        align="center"
      >
        <template slot-scope="scope">
          {{ scope.row.userName }}
        </template>
      </el-table-column>
      <el-table-column
        align="center"
        label="操作时间"
        width="160"
      >
        <template slot-scope="scope">
          <span>{{ scope.row.writeDate }}</span>
        </template>
      </el-table-column>
    </el-table>

    <pagination
      v-show="total > 0"
      :total="total"
      :page.sync="listQuery.page"
      :limit.sync="listQuery.limit"
      @pagination="fetchData"
    />
  </div>
</template>

<script>
import Pagination from '@/components/Pagination'
import { getEventList } from '@/api/log'

export default {
  components: {
    Pagination
  },
  filters: {
    levelFilter(level) {
      const levelMap = {
        'DEBUG': 'success',
        'INFO': '',
        'WARNING': 'warning',
        'ERROR': 'danger'
      }
      return levelMap[level]
    }
  },
  data() {
    return {
      total: 0,
      list: [],
      listLoading: true,
      listQuery: {
        page: 1,
        limit: 20,
        created_at: undefined,
        status: undefined,
        level: undefined,
        method: undefined,
        keyword: undefined
      }
    }
  },
  created() {
    this.fetchData()
  },
  methods: {
    search() {
      this.fetchData()
    },
    refresh() {
      this.listQuery = {
        page: 1,
        limit: 20,
        created_at: undefined,
        status: undefined,
        level: undefined,
        method: undefined,
        keyword: undefined
      }
      this.fetchData()
    },
    fetchData() {
      this.listLoading = true
      getEventList(this.listQuery).then(response => {
        this.list = response.data.list
        this.total = response.data.total
        this.listLoading = false
      })
    }
  }
}
</script>
