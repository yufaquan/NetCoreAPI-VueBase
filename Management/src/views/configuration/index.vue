<template>
  <div class="app-container">
    <el-form
      ref="dataForm"
      v-loading="listLoading"
      :model="temp"
      label-width="200px"
      :label-position="labelObj"
    >
      <div class="form-container">
        <div class="form-container-body">
          <el-row>
            <el-col :md="18" :lg="18" :xl="18">
              <el-form-item label="附件上传大小限制（兆）">
                <el-input-number v-model="temp.limitUpFileSize" controls-position="right" :min="1" :max="50" @keyup.enter.native="submit"></el-input-number>
              </el-form-item>
              
              <el-form-item label="产品拼团上限（个）">
                <el-input-number v-model="temp.productGroupMax" controls-position="right" :min="1" @keyup.enter.native="submit"></el-input-number>
              </el-form-item>
              
              <el-form-item label="预售金（元）">
                <el-input-number v-model="temp.bookingPrice" controls-position="right" :min="1" @keyup.enter.native="submit"></el-input-number>
              </el-form-item>
              
              <el-form-item label="记录API日志">
                <el-switch
                  v-model="temp.isWriteAPILog"
                  active-text="开启"
                  inactive-text="关闭"
                  @change="submit">
                </el-switch>
              </el-form-item>
              
            </el-col>
          </el-row>
        </div>
        <div class="form-container-footer">
          <el-button type="primary" size="small" @click="submit">提交</el-button>
        </div>
      </div>
    </el-form>
  </div>
</template>

<script>
import { get,set } from '@/api/configuartion';
import { deepClone } from '@/utils'

const _temp = {
  limitUpFileSize: undefined,
  productGroupMax: undefined,
  bookingPrice: undefined,
  isWriteAPILog: undefined
}

export default {
  data() {
    return {
      listLoading: false,
      loading: false,
      uploadUrl: '',
      temp: Object.assign({}, _temp)
    }
  },
  computed: {
    labelObj() {
      return this.$store.state.app.device === 'mobile' ? 'top' : 'right'
    }
  },
  created() {
    this.getInfo()
  },
  methods: {
    getInfo() {
      var than=this;
      if (this.listLoading) {
        return false
      }
      get().then(res=>{
        if(res.code==0){
          than.temp = deepClone(res.data)
        }
        than.listLoading = false;
      });
    },
    submit() {
      var than=this;
      if (this.loading) {
        return;
      }
      this.loading = true;
      set(than.temp).then(res=>{
            if(res.code==0){
              than.$message({
                message: '保存成功',
                type: 'success'
              });
            }
            than.loading = false
        });
    }
  }
}
</script>
