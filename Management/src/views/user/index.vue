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
      
      <el-input
        v-model="listQuery.name"
        size="small"
        placeholder="用户名"
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
        <!-- <el-button
          size="small"
          type="primary"
          icon="el-icon-plus"
          @click="add"
        >
          新增
        </el-button> -->
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
        align="center"
        width="80"
        label="头像"
        prop="headImgUrl"
      >
        <template slot-scope="scope">
          <img :src="scope.row.headImgUrl" height="40" class="round-ss">
        </template>
      </el-table-column>
      <el-table-column
        label="用户名"
        align="center"
        prop="name"
        width="120"
      >
        <template slot-scope="scope">
          {{ scope.row.name }}
        </template>
      </el-table-column>
      <el-table-column
        label="昵称"
        width="150"
        align="center"
        prop="nickName"
      >
        <template slot-scope="scope">
          <span>{{ scope.row.nickName }}</span>
        </template>
      </el-table-column>
      <el-table-column
        label="性别"
        width="70"
        align="center"
        prop="sex"
        :filters="[{ text: '未知', value: 0 }, { text: '男', value: 1 }, { text: '女', value: 2 }]"
        :filter-method="filteSex"
      >
        <template slot-scope="scope">
          <span>{{ showSex(scope.row.sex) }}</span>
        </template>
      </el-table-column>
      
      <el-table-column
        label="手机"
        align="center"
      >
        <template slot-scope="scope">
          {{ scope.row.mobile }}
        </template>
      </el-table-column>
      <el-table-column
        label="邮箱"
        align="center"
        prop="email"
        :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          {{ scope.row.email }}
        </template>
      </el-table-column>
      <el-table-column
        label="所在地"
        align="center"
        prop="area"
        width="120"
        :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          {{ scope.row.area }}
        </template>
      </el-table-column>

      <el-table-column
        label="角色"
        align="center"
        prop="role"
      >
        <template slot-scope="scope">
          <el-tag class="mr5 mb5" size="medium" :type="role.id==1?'danger':(role.id==2?'info':'')" v-for="role in scope.row.roles" :key="role.id">{{ role.name }}</el-tag>
        </template>
      </el-table-column>

      <el-table-column
        label="注册时间"
        width="120"
        align="center"
        :sortable="true"
        prop="createdAt"
        :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <span>{{ scope.row.createdAt }}</span>
        </template>
      </el-table-column>
     
      <el-table-column
        label="创建人"
        align="center"
        prop="createdBy"
        width="120"
      >
        <template slot-scope="scope">
          <span>{{ scope.row.createdBy }}</span>
        </template>
      </el-table-column>

      <!-- <el-table-column
        fixed="right"
        label="操作"
        width="200"
        align="center"
        prop=""
      >
        <template slot-scope="scope">
          <el-button-group>
            <el-button
              type="primary"
              icon="el-icon-edit"
              size="mini"
              @click="edit(scope)"
            >
              修改
            </el-button>
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
      </el-table-column> -->
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
      :title="dialogType === 'modify' ? '修改' : '新增'"
    >
      <el-form
        ref="dataForm"
        :model="temp"
        label-width="120px"
        label-position="right"
      >
        <el-form-item label="用户名">
          <el-input v-model="temp.name" placeholder="请输入用户名" />
        </el-form-item>
        <el-form-item label="真实姓名">
          <el-input v-model="temp.nickName" placeholder="请输入真实姓名" />
        </el-form-item>
        <el-form-item label="电子邮箱">
          <el-input v-model="temp.email" placeholder="请输入电子邮箱" />
        </el-form-item>
        <el-form-item label="性别">
          <el-radio-group v-model="temp.sex">
            <el-radio :label="0" >未知</el-radio>
            <el-radio :label="1">男</el-radio>
            <el-radio :label="2">女</el-radio>
          </el-radio-group>
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
import { getList,Add } from '@/api/user'
import { deepClone } from '@/utils'

const _temp = {
  id: 0,
  name: '',
  nickName: '',
  roles: [],
  sex:0,
  email: ''
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
        created_at: undefined,
        status: undefined,
        name: undefined
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
        this.list = response.data.list
        this.total = response.data.pageCount;
        this.listLoading = false
      })
    },
    showSex(cellValue){
      switch (cellValue) {
        case 0:
          return "未知";
        case 1:
          return "男";
        case 2:
          return "女";
      
        default:
         return "未知";
      }
    },
    resetTemp() {
      this.temp = Object.assign({}, _temp)
    },
    add() {
      this.resetTemp()
      this.dialogVisible = true
      this.dialogType = 'create'
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate();
      });
    },
    edit(scope) {
      this.resetTemp()
      this.dialogVisible = true
      this.dialogType = 'modify'
      this.temp = deepClone(scope.row)
      this.$nextTick(() => {
        this.$refs['dataForm'].clearValidate()
      })
    },
    changeStatus(value, scope) {
      setTimeout(() => {
        this.list[scope.$index].status = value
        this.$message({
          message: '更新成功',
          type: 'success'
        })
      }, 300)
    },
    del(scope) {
      this.$confirm('确认删除该条数据吗？', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        setTimeout(() => {
          this.list.splice(scope.$index, 1)
          this.$message({
            message: '删除成功',
            type: 'success'
          })
        }, 300)
      })
    },
    submit() {
      if (this.loading) {
        return
      }
      this.doSubmit();
    }
    ,doSubmit(){
      this.loading = true
      if(this.dialogType=="create"){
        Add(this.temp).then(res=>{
            if(res.code==0){
              this.$message({
                message: '提交成功',
                type: 'success'
              })
              this.dialogVisible = false
            }
            this.loading = false
        });
      }
    }
  }
}
</script>
