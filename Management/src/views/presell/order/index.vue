<template>
  <div class="app-container">
    <div class="filter-container">
      <!-- <el-input
        v-model="listQuery.code"
        size="small"
        placeholder="Code"
        clearable
        class="filter-item w-200"
      /> -->
      <el-autocomplete
      class="filter-item w-200"
      size="small"
      v-model="listQuery.title"
      :fetch-suggestions="querySearch"
      valueKey="title"
      placeholder="请选择产品"
      :trigger-on-focus="true"
      @select="handleSelect"
    ></el-autocomplete>
      <el-input
        v-model="listQuery.orderNumber"
        size="small"
        placeholder="商户订单号"
        clearable
        class="filter-item w-200"
      />
      <el-input
        v-model="listQuery.transactionId"
        size="small"
        placeholder="微信订单号"
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
          icon="el-icon-sold-out"
          @click="add"
        >
          核销
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
        label="产品名称"
        align="center"
        prop="title"
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
        label="金额（元）"
        align="center"
      >
        <template slot-scope="scope">
          <span class="text-price">{{ scope.row.price }}</span>
        </template>
      </el-table-column>
      
      <el-table-column
        label="状态"
        align="center"
        :filters="[{ text: '等待支付', value: 1 }, { text: '已支付', value: 2 }, { text: '退款中', value: 3 }, { text: '已退款', value: 4 }, { text: '已取消', value: 5 }]"
        :filter-method="filteStatus"
      >
        <template slot-scope="scope">
          <span :class="{'text-gray':(scope.row.status==5),'text-orange':(scope.row.status==1),'text-green':(scope.row.status==2),
            'text-red':(scope.row.status==3),'text-black':(scope.row.status==4)}">
            {{ scope.row.statusName }}
          </span>
        </template>
      </el-table-column>
      
      <el-table-column
        label="商户订单号"
        align="center"
        :show-overflow-tooltip="true"
        width="230"
      >
        <template slot-scope="scope">
          {{ scope.row.orderNumber }}
        </template>
      </el-table-column>
      
      <el-table-column
        label="微信订单号"
        align="center"
        width="240"
        :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          {{ scope.row.transactionId }}
        </template>
      </el-table-column>
      
      <el-table-column
        label="创建时间"
        align="center"
        :sortable="true"
        prop="createdAt"
        width="220"
        :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <span>{{ scope.row.createdAt }}</span>
        </template>
      </el-table-column>
<!--      
      <el-table-column
        label="创建人"
        align="center"
        prop="createdByName"
        width="120"
      >
        <template slot-scope="scope">
          <span>{{ scope.row.createdByName }}</span>
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
            <!-- <el-button
              type="danger"
              icon="el-icon-delete"
              size="mini"
              @click="del(scope)"
            >
              删除
            </el-button> -->
            <el-button
              type="danger"
              icon="el-icon-refresh"
              size="mini"
              @click="refund(scope)"
            >
              退款
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
      :title="dialogType === 'modify' ? '修改' : '核销'"
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
import { getList,edit,refund,add,del } from '@/api/order'
import { deepClone } from '@/utils'

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
      productList:[],
      listLoading: true,
      listQuery: {
        page: 1,
        limit: 20,
        productId: undefined,
        orderNumber: undefined,
        transactionId: undefined
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
  watch:{
    list:function(){
      if(this.list!=null && this.list.length>0){
        var productArr=new Array();
        this.list.forEach(item => {
          var isExist=productArr.some(nitem=>{
            if(nitem.title==item.product.title){
                return true 
            } 
          });
          if(!isExist){
            productArr.push(item.product);
          }
        });
        this.productList=productArr;
      }
    }
  },
  methods: {
    querySearch(queryString, cb) {
      var list = this.productList;
      var results = queryString ? list.filter(this.createFilter(queryString)) : list;
      // 调用 callback 返回建议列表的数据
      cb(results);
    },
    createFilter(queryString) {
      return (item) => {
        return (item.title.toLowerCase().indexOf(queryString.toLowerCase()) === 0);
      };
    },
    handleSelect(item) {
      console.log(item);
      this.listQuery.productId=item.id;
    },
    filteStatus(value, row) {
      return row.status === value;
    },
    search() {
      this.fetchData()
    },
    refresh() {
      this.listQuery = {
        page: 1,
        limit: 20,
        productId: undefined,
        orderNumber: undefined,
        transactionId: undefined
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
    add() {
      this.resetTemp()
      this.dialogVisible = true
      this.dialogType = 'create'
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
    refund(scope) {
      this.$confirm('确认进行退款吗？', '提示', {
        confirmButtonText: '确定',
        cancelButtonText: '取消',
        type: 'warning'
      }).then(() => {
        refund({id:scope.row.id}).then(res=>{
            if(res.code==0){
                this.list.splice(scope.$index, 1);
                this.$message({
                    message: '已申请退款！',
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
      this.loading = true
      if(this.dialogType=="create"){
        add(this.temp).then(res=>{console.log("addRes:"+res.message)
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
