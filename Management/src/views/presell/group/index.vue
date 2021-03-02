<template>
  <div class="app-container">
    <div class="filter-container">
      <!-- <el-input
        v-model="listQuery.title"
        size="small"
        placeholder="title"
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
        label="产品名称"
        align="center"
        prop="title"
        :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <span>{{ scope.row.product.title }}</span>
        </template>
      </el-table-column>
      
      <el-table-column
        label="拼团价"
        align="center"
        :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <div class="text-price">{{ scope.row.price }}</div>
        </template>
      </el-table-column>
      
      <el-table-column
        label="开团人"
        align="center"
        prop="cGroup"
        :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <span>{{ scope.row.cGroup }}</span>
        </template>
      </el-table-column>

      <el-table-column
        label="拼团人"
        align="center"
        prop="jGroup"
        :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <span>{{ scope.row.jGroup }}</span>
        </template>
      </el-table-column>

      <el-table-column
        label="开团核销码"
        align="center"
        prop="cCode"
        width="235"
        :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <span>{{ scope.row.cCode }}</span>
          <span class='ml2' v-if="scope.row.cCode" :class="{'text-red':scope.row.isCCancel,'text-green':!scope.row.isCCancel}">{{scope.row.isCCancel?"已核销":"未核销"}}</span>
        </template>
      </el-table-column>
      
      <el-table-column
        label="参团核销码"
        align="center"
        prop="cCode"
        width="235"
        :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <span>{{ scope.row.jCode }}</span>
          <span class='ml2' v-if="scope.row.jCode" :class="{'text-red':scope.row.isJCancel,'text-green':!scope.row.isJCancel}">{{scope.row.isJCancel?"已核销":"未核销"}}</span>
        </template>
      </el-table-column>
      
      <el-table-column
        label="开团时间"
        align="center"
        :sortable="true"
        prop="startTime"
        width="220"
        :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <span>{{ scope.row.startTime }}</span>
        </template>
      </el-table-column>
     
      <el-table-column
        label="参团时间"
        align="center"
        prop="endTime"
        width="120"
        :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <span>{{ scope.row.endTime }}</span>
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
        :rules="rules"
      >
        <el-form-item label="产品标题">
          <el-input v-model="temp.title" placeholder="" />
        </el-form-item>
        <el-form-item label="原价（元）">
          <!-- <el-input v-model="temp.oPrice" placeholder="" /> -->
          <el-input-number v-model="temp.oPrice" controls-position="right" ></el-input-number>
        </el-form-item>
        <el-form-item label="团购价（元）">
          <!-- <el-input v-model="temp.nPrice" placeholder="" /> -->
          <el-input-number v-model="temp.nPrice" controls-position="right" ></el-input-number>
        </el-form-item>
        <el-form-item label="列表图">
          <el-input v-model="temp.bImg" placeholder="" />
        </el-form-item>
        <el-form-item label="缩略图">
          <el-input v-model="temp.sImg" placeholder="" />
        </el-form-item>
        <el-form-item label="详情图">
          <el-input disabled type="textarea" v-model="temp.listImg" placeholder="" />
        </el-form-item>
        <el-form-item
          v-for="(img, index) in temp.detailImgs"
          :label="'详情图' + index"
          :key="index"
        >
        <el-input class="haveBtnInput" v-model="temp.detailImgs[index]"></el-input><el-button @click.prevent="removeDetail(img)">删除</el-button>
      </el-form-item>
      <el-form-item>
        <el-button @click="addDetail">新增详情</el-button>
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
import { getList,edit,add,del } from '@/api/Group'
import { deepClone } from '@/utils'

const _temp = {
  id: 0,
  title: '',
  oPrice: '',
  nPrice: '',
  bImg: '',
  sImg: '',
  listImg: '',
  detailImgs: []
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
        title: undefined,
        productId:0
      },
      temp: Object.assign({}, _temp),
      dialogVisible: false,
      dialogType: 'create',
      loading: false,
      rules: {
        title: [
          { required: true, message: '请输入产品标题', trigger: 'blur' },
          { min: 1, max: 20, message: '长度在 3 到 5 个字符', trigger: 'blur' }
        ],
        oPrice: [
          { required: true, message: '请输入原价', trigger: 'change' }
        ],
        nPrice: [
          { type: 'date', required: true, message: '请输入拼团价', trigger: 'change' }
        ],
        bImg: [
          { type: 'date', required: true, message: '请输入列表图地址', trigger: 'change' }
        ],
        sImg: [
          { type: 'array', required: true, message: '请输入缩略地址', trigger: 'change' }
        ],
        listImg: [
          { required: true, message: '请输入至少一个详情图地址', trigger: 'change' }
        ]
      }
    }
  },
  watch:{
    "temp.detailImgs":function(){
      this.temp.listImg=this.temp.detailImgs.toString();
    },
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
  created() {
    this.fetchData()
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
        productId: undefined,
        title: undefined
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
    removeDetail(item) {
      var index = this.temp.detailImgs.indexOf(item)
      if (index !== -1) {
        this.temp.detailImgs.splice(index, 1)
      }
    },
    addDetail() {
      this.temp.detailImgs.push("");
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
      if(!this.temp.title || this.temp.title==""){
        this.$message({
          message:"标题不能为空",
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
                message: '创建成功',
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
      }else if(this.dialogType=="modify"){
        edit(this.temp).then(res=>{console.log("addRes:"+res.message)
          if(res.code==0){
            this.$message({
              message: '修改成功！',
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


<style lang="scss" scoped>
.haveBtnInput{
  width:90%;
  margin-right: 5px;
}
</style>