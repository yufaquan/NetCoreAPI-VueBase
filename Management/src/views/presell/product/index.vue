<template>
  <div class="app-container">
    <div class="filter-container">
      <el-input
        v-model="listQuery.title"
        size="small"
        placeholder="title"
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
          icon="el-icon-plus"
          @click="add"
        >
          新增
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
        :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <span>{{ scope.row.title }}</span>
        </template>
      </el-table-column>
      
      <el-table-column
        label="原价（元）"
        align="center"
        :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <div class="text-price">{{ scope.row.oPrice }}</div>
        </template>
      </el-table-column>
      
      <el-table-column
        label="拼团价（元）"
        align="center"
        :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <div class="text-price">{{ scope.row.nPrice }}</div>
        </template>
      </el-table-column>

      <!-- <el-table-column
        align="center"
        width="80"
        label="列表图"
        prop="bImg"
      >
        <template slot-scope="scope">
          <img :src="scope.row.bImg" height="40" class="round-ss">
        </template>
      </el-table-column>

      <el-table-column
        align="center"
        width="80"
        label="缩略图"
        prop="sImg"
      >
        <template slot-scope="scope">
          <img :src="scope.row.sImg" height="40" class="round-ss">
        </template>
      </el-table-column>

      <el-table-column
        align="center"
        width=""
        label="详情图"
        prop="detailImgs"
      >
        <template slot-scope="scope">
          <img :src="img" v-for="(img,index) in scope.row.detailImgs" :key="index" height="40" class="round-ss mr2">
        </template>
      </el-table-column> -->

      <el-table-column
        label="创建时间"
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
     
      <el-table-column
        label="创建人"
        align="center"
        prop="createdByName"
        width="120"
        :show-overflow-tooltip="true"
      >
        <template slot-scope="scope">
          <span>{{ scope.row.createdByName }}</span>
        </template>
      </el-table-column>

      <el-table-column
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
import { getList,edit,add,del } from '@/api/product'
import { deepClone } from '@/utils'
import { Add } from '@/api/user'

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
      listLoading: true,
      listQuery: {
        page: 1,
        limit: 20,
        title: undefined
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