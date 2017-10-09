<style>
.sql-code textarea {
  font-family: "Courier New";
  font-size: 16px !important;
  font-weight: 900 !important;
  letter-spacing: 2px;
}

.base-where {
  background-color: #f7f7f7;
  padding: 15px;
  margin-top: 10px;
  margin-bottom: 10px;
  border-radius: 10px;
}
</style>

<template>
  <div class="db-info">
    <Row :gutter="26">
      <Col span="8">
      <Input v-model="sql" type="textarea" :autosize="true" placeholder="请输入完整的SQL" class="sql-code">
      </Input>
      </Col>
      <Col span="16">
      <Card>
        <Row :gutter="10">
          <Col span="12" v-for="(where,index) in whereList" :key="index" style="padding-bottom: 10px">
          <Select v-model="where.coexist" style="width:17%" @on-change="getSql(top,orderType,whereList)">
            <Option value="AND">AND</Option>
            <Option value="OR">OR</Option>
          </Select>
          <Select v-model="where.field" style="width:28%" filterable @on-change="getSql(top,orderType,whereList)" clearable>
            <Option v-for="field in listCol" :key="field.key" :value="field.key">{{field.key}}</Option>
          </Select>
          <Select v-model="where.rlation" style="width:15%" @on-change="getSql(top,orderType,whereList)">
            <Option value="="></Option>
            <Option value="IN"></Option>
            <Option value="<>"></Option>
            <Option value="LIKE"></Option>
            <Option value="<="></Option>
            <Option value="<"></Option>
            <Option value=">="></Option>
            <Option value=">"></Option>
          </Select>
          <Input v-model="where.value" placeholder="输入值" style="width: 30%" @on-change="getSql(top,orderType,whereList)"></Input>
          <Button type="ghost" shape="circle" icon="close-round" @click="removeWhere(index)"></Button>
          </Col>
          <Col span="12">
          <Button type="ghost" long @click="addWhere()">添加条件</Button>
          </Col>
        </Row>
      </Card>
      </Col>
    </Row>
    <div class="base-where">
      <Row style="text-align: right;">
        <!-- <Col span="18">
              <el-checkbox :indeterminate="isShowAll" v-model="checkAll" @change="changeShowAll">SELECT</el-checkbox>
              <div style="margin: 7px 0;"></div>
              <el-checkbox-group v-model="checkedShow" @change="changeShow">
                <el-checkbox v-for="(col,index) in listCol" :label="col.key" :key="index">{{col.key}}</el-checkbox>
              </el-checkbox-group>
              </Col> -->
        Top:
        <Input v-model.number="top" icon="pin" placeholder="请输入Top值" style="width: 150px;margin-right: 40px;"></Input>
        <RadioGroup v-model="orderType" style="margin-right: 30px;" v-if="orderType != 'NULL'">
          <Radio label="ASC"></Radio>
          <Radio label="DESC"></Radio>
        </RadioGroup>
        <Button type="warning" :loading="loading" icon="checkmark-round" @click="doSql">
          <span v-if="!loading">执行</span>
          <span v-else>Loading...</span>
        </Button>
        <Col span="6">
        </Col>
      </Row>

    </div>
    <Table border :columns="listCol" height="635" :data="list" stripe></Table>
  </div>
</template>
<script>
import { mapState } from 'vuex'
import lodash from 'lodash'

export default {
  data() {
    return {
      sql: '',
      top: 100,
      orderType: 'ASC',
      orderKey: '',
      loading: false,
      listCol: [],
      list: [],
      whereList: [],
      checkAll: true,
      checkedShow: [],
      isShowAll: false
    }
  },
  methods: {
    doSql() {
      this.loading = !this.loading
      var that = this
      that.$store.dispatch('getDataList', {
        data: that.sql,
        call(res) {
          that.list = res.data
          that.loading = false
        },
        errCall(msg) {
          var msgarr = msg.split('------------')
          that.$Notice.error({
            title: msgarr.length > 1 ? msgarr[0] : '',
            desc: msgarr.length > 1 ? msgarr[1] : msgarr[0]
          })
          that.loading = false
        }
      })
    },
    init() {
      var that = this
      this.$store.dispatch('getDataHead', {
        db: this.request.db,
        table: this.request.table,
        call: function(res) {
          that.listCol = res.data
          that.listCol.forEach(col => {
            that.checkedShow.push(col.key)
          })
          var orderKey = lodash.find(res.data, function(d) {
            return d.orderKey
          })
          if (orderKey) {
            that.orderKey = orderKey.title
          } else {
            that.orderType = 'NULL'
          }
          that.getSql(that.top, that.orderKey, that.whereList)
        }
      })
    },
    getSql(top, orderType, whereList) {
      if (!isNaN(top)) {
        this.sql = `SELECT TOP ${top ? top : 100} \r\n* \r\nFROM \r\n${this.request.db}..${this.request.table} \r\nWHERE 1=1 `
      } else {
        this.sql = `SELECT TOP 100 \r\n* \r\nFROM \r\n${this.request.db}..${this.request.table} \r\nWHERE 1=1 `
      }
      if (whereList) {
        whereList.forEach(where => {
          this.sql += `\r\n    ${where.coexist} ${where.field} ${where.rlation} '${where.value}'`
        })
      }
      if (orderType && this.orderType !== 'NULL') {
        this.sql += ' \r\nORDER BY ' + this.orderKey + ' ' + this.orderType
      }
    },
    addWhere() {
      this.whereList.push({
        coexist: 'AND',
        rlation: '=',
        field: '',
        value: ''
      })
    },
    removeWhere(index) {
      this.whereList.splice(index, 1)
    },
    changeShow(data) {
      if (this.checkedShow.length === this.listCol.length) {
        this.isShowAll = false
        this.checkAll = true
      } else {
        if (this.checkedShow.length === 0) {
          this.checkAll = false
        } else {
          this.checkAll = true
          this.isShowAll = true
        }
      }
    },
    changeShowAll(event) {
      var all = []
      this.listCol.forEach(col => {
        all.push(col.key)
      })
      this.checkedShow = event.target.checked ? all : []
      this.isShowAll = false
    }
  },
  watch: {
    top(val) {
      this.getSql(val, this.orderType, this.whereList)
    },
    orderType(val) {
      this.getSql(this.top, val, this.whereList)
    },
    whereList(val) {
      this.getSql(this.top, this.orderType, val)
    },
    checkedShow(val) {
      // var that = this
      // that.listCol = []
      // val.forEach(v => {
      //   that.listCol.push(lodash.find(that.listCol, function(col) {
      //     return v === col.key
      //   }))
      // })
    }
  },
  computed: {
    ...mapState([
      'request'
    ]),
    lineNumber() {
      return Math.ceil(this.whereList.length / 2)
    }
  },
  beforeRouteEnter(to, from, next) {
    next(vm => {
      vm.$store.commit('setRequest', to.params)
      vm.init()
    })
  },
  beforeRouteUpdate(to, from, next) {
    this.$store.commit('setRequest', to.params)
    next()
  }
}
</script>
