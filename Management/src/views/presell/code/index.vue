<template>
  <div class="app-container">
    <div class="filter-container">
      <el-input
        v-model="listQuery.code"
        size="small"
        placeholder="Code"
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
        <el-button
          size="small"
          type="primary"
          icon="el-icon-sold-out"
          @click="add"
        >
          手动核销
        </el-button>
        <el-button
          size="small"
          type="primary"
          icon="el-icon-sold-out"
          @click="autoAdd"
        >
          扫码核销
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
        fixed
        label="ID"
        width="80"
        align="center"
        prop="id"
      >
        <template slot-scope="scope">
          {{ scope.row.id }}
        </template>
      </el-table-column>
      
      <el-table-column
        label="产品名称"
        align="center"
        prop="nickName"
      >
        <template slot-scope="scope">
          <span>{{ scope.row.product.title }}</span>
        </template>
      </el-table-column>
      
      <el-table-column
        label="用户昵称"
        width="150"
        align="center"
        prop="nickName"
      >
        <template slot-scope="scope">
          <span>{{ scope.row.user.nickName }}</span>
        </template>
      </el-table-column>
      
      <el-table-column
        label="Code"
        align="center"
      >
        <template slot-scope="scope">
          {{ scope.row.code }}
        </template>
      </el-table-column>
      
      <el-table-column
        label="加密Code"
        align="center"
        :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          {{ scope.row.decryptCode }}
        </template>
      </el-table-column>
      
      <el-table-column
        label="核销时间"
        align="center"
        :sortable="true"
        prop="createdTime"
        width="220"
        :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <span>{{ scope.row.createdTime }}</span>
        </template>
      </el-table-column>
<!--      
      <el-table-column
        label="创建人"
        align="center"
        prop="createdBy"
        width="120"
      >
        <template slot-scope="scope">
          <span>{{ scope.row.createdBy }}</span>
        </template>
      </el-table-column> -->

      <el-table-column
        fixed="right"
        label="操作"
        width="120"
        align="center"
        prop=""
      >
        <template slot-scope="scope">
          <el-button-group>
            <el-button
              type="danger"
              icon="el-icon-delete"
              size="mini"
              @click="del(scope)"
            >
              删除
            </el-button>
          </el-button-group>
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

    <el-dialog
      :visible.sync="dialogVisible"
      :title="dialogType === 'autoCancel' ? '扫码核销' : '手动核销'"
    >
      <el-form
        ref="dataForm"
        :model="temp"
        label-width="120px"
        label-position="right"
      >
        <el-form-item label="Code">
          <el-input v-model="temp.code" placeholder="请输入Code" />
        </el-form-item>
      </el-form>
      <div class="text-right">
        <el-button type="danger" @click="dialogVisible = false">
          取消
        </el-button>
        <el-button type="primary" @click="submit">
          确定
        </el-button>
      </div>
    </el-dialog>
  </div>
</template>

<script>
import Pagination from '@/components/Pagination'
import { getList,cancel,add,del } from '@/api/code'
import { deepClone } from '@/utils'
import { Add } from '@/api/user'

const _temp = {
  id: 0,
  code: ''
}

export default {
  components: {
    Pagination
  },
  data() {
    return {
      total: 0,
      list: [],
      listLoading: true,
      listQuery: {
        page: 1,
        limit: 20,
        code: undefined
      },
      temp: Object.assign({}, _temp),
      dialogVisible: false,
      dialogType: 'create',
      loading: false
    }
  },
  created() {
    this.fetchData()
  },
  methods: {
    filteSex(value, row) {
      return row.sex === value;
    },
    search() {
      this.fetchData()
    },
    refresh() {
      this.listQuery = {
        page: 1,
        limit: 20,
        created_at: undefined,
        status: undefined,
        name: undefined
      }
      this.fetchData()
    },
    fetchData() {
      this.listLoading = true
      getList(this.listQuery).then(response => {
        if(response.data){
          this.list = response.data.list
          this.total = response.data.total;
        }
        this.listLoading = false
      })
    },
    resetTemp() {
      this.temp = Object.assign({}, _temp)
    },
    autoAdd() {
      this.resetTemp()
      this.dialogVisible = true
      this.dialogType = 'autoCancel'
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate();
      });
    },
    add() {
      this.resetTemp()
      this.dialogVisible = true
      this.dialogType = 'cancel'
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate();
      });
    },
    del(scope) {
      this.$confirm('确认删除该条数据吗？', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        del({id:scope.row.id}).then(res=>{
            if(res.code==0){
                this.list.splice(scope.$index, 1);
                this.$message({
                    message: '删除成功',
                    type: 'success'
                })
            }
            this.loading = false
        });
      })
    },
    submit() {
      if (this.loading) {
        return
      }
      if(!this.temp.code || this.temp.code==""){
        this.$message({
          message:"Code不能为空",
          type:"error"
        });
        return;
      }
      this.doSubmit();
    }
    ,doSubmit(){
      this.loading = true;
      if(this.dialogType=="autoCancel"){
        add(this.temp).then(res=>{
            if(res.code==0){
              this.$message({
                message: '核销成功',
                type: 'success'
              })
              this.dialogVisible = false
              this.fetchData();
            }
            this.loading = false
        },err=>{
            console.log("addErr:"+err.message);
            this.loading = false
        });
      }
      if(this.dialogType=="cancel"){
        cancel(this.temp).then(res=>{
            if(res.code==0){
              this.$message({
                message: '核销成功',
                type: 'success'
              })
              this.dialogVisible = false
              this.fetchData();
            }
            this.loading = false
        },err=>{
            console.log("addErr:"+err.message);
            this.loading = false
        });
      }
    }
  }
}
</script>
