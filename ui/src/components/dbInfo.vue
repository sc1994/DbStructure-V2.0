<style>
.db-info-list {
  border-top-left-radius: 6px;
  border-top-right-radius: 6px;
}

.el-tabs {
  height: 0px;
}

.el-tabs__header {
  margin-top: 1px !important;
}
</style>


<template>
  <div class="db-info">
    <Row :gutter="16">
      <Col span="4">
      <Input v-model="findTable">
      <Button slot="append" icon="ios-search"></Button>
      </Input>
      </Col>
      <Col span="20">
      <el-tabs v-model="editableTabsValue" type="card" closable @tab-remove="removeTab" v-show="infoList.length > 0">
        <el-tab-pane v-for="(item, index) in infoList" :key="index" :label="item.name" :name="item.name">
        </el-tab-pane>
      </el-tabs>
      </Col>
    </Row>
    <div style="height:10px"></div>
    <Row :gutter="16">
      <Col span="4">
      <Table border :columns="tableListCol" :data="tableList" class="db-info-list" height="835" @on-row-click="getTableInfo">
      </Table>
      </Col>
      <Col span="17">
      <Table border :columns="infoListCol" :data="info" class="db-info-list" height="835">
      </Table>
      </Col>
      <Col span="3">
      <div style="height: 5px;"></div>
      <Button type="info" long @click="gotoData" v-show="currentInfo.table.length>2">数据查询</Button>
      <div style="height: 25px;" v-show="currentInfo.table.length>2"></div>
      <Button type="success" long @click="closeOther">关闭其他</Button>
      <div style="height: 25px;"></div>
      <Button type="warning" long @click="closeAll">关闭全部</Button>
      <div style="height: 50px;"></div>
      <Card v-show="currentInfo.table.length>2">
        <p slot="title">{{currentInfo.table}}</p>
        <p style="margin-left: 2px;padding-bottom: 7px;">描述</p>
        <Input v-model="currentInfo.describe" type="textarea" :rows="4" placeholder="请输入..."></Input>
        <Button type="ghost" shape="circle" icon="edit" style="margin-top: 6px;margin-left: 82%;" @click="editTableDescribe"></Button>
      </Card>
      </Col>
    </Row>
  </div>
</template>

<script>
import { mapState } from 'vuex'
import lodash from 'lodash'
export default {
  data() {
    return {
      params: {},
      editableTabsValue: '',
      findTable: '',
      tableListCol: [
        {
          title: '表名称(描述)',
          key: 'name',
          sortable: true
        }
      ],
      tableList: [
        {
          name: '加载中,请稍后...'
        }
      ],
      soundTableList: [],
      infoListCol: [
        {
          title: '字段名称',
          key: 'fieldname',
          sortable: true
        },
        {
          title: '主键',
          key: 'primarykey'
        },
        {
          title: '自增键',
          key: 'identifying'
        },
        {
          title: '值类型',
          key: 'types'
        },
        {
          title: '可为空',
          key: 'ornull'
        },
        {
          title: '默认值',
          key: 'defaults'
        },
        {
          title: '描述',
          key: 'describe',
          width: 500
        }
      ],
      infoList: [],
      info: [],
      currentInfo: {
        db: '',
        table: '',
        describe: ''
      }
    }
  },
  methods: {
    getTableInfo(data, index) {
      var that = this
      that.editableTabsValue = data.name
      var having = lodash.find(that.infoList, function(i) {
        return i.name === data.name
      })
      if (!having) {
        that.$store.dispatch('getTableInfo', {
          data: data.name,
          call: function(res) {
            that.infoList.push({
              name: data.name,
              data: res.data
            })
            that.info = res.data
          }
        })
      } else {
        that.info = having.data
      }
    },
    removeTab(name) {
      var current = lodash.find(this.infoList, function(i) {
        return i.name === name
      })
      var index = this.infoList.indexOf(current)
      if (name === this.editableTabsValue) {
        if (index > 0) {
          this.editableTabsValue = this.infoList[index - 1].name
        } else if (index === 0 && this.infoList.length > 1) {
          this.editableTabsValue = this.infoList[index + 1].name
        } else {
          this.editableTabsValue = ''
        }
      }
      this.infoList.splice(index, 1)
    },
    gotoData() {
      this.$store.commit('setRequest', this.currentInfo)
      this.$router.push({ path: `/data/${this.currentInfo.db}/${this.currentInfo.table}` })
    },
    closeAll() {
      this.infoList = []
      this.info = []
      this.currentInfo = {
        db: '',
        table: '',
        describe: ''
      }
    },
    closeOther() {
      var that = this
      that.infoList = lodash.remove(that.infoList, function(i) {
        return i.name === that.editableTabsValue
      })
    },
    editTableDescribe() {
      var that = this
      if (!that.currentInfo.db || !that.currentInfo.table || !that.currentInfo.describe) {
        that.$Notice.warning({
          title: '信息不完善'
        })
        return
      }
      that.$store.dispatch('editTableDescribe', {
        db: that.currentInfo.db,
        table: that.currentInfo.table,
        describe: that.currentInfo.describe,
        call: function(res) {
          that.$Notice.info({
            title: res.data
          })
          lodash.find(that.infoList, function(i) {
            return i.name === that.editableTabsValue
          }).name = `${that.currentInfo.db}(${that.currentInfo.describe})`
          lodash.find(that.soundTableList, function(i) {
            return i.name === that.editableTabsValue
          }).name = `${that.currentInfo.db}(${that.currentInfo.describe})`
          lodash.find(that.tableList, function(i) {
            return i.name === that.editableTabsValue
          }).name = `${that.currentInfo.db}(${that.currentInfo.describe})`
          that.editableTabsValue = `${that.currentInfo.db}(${that.currentInfo.describe})`
        }
      })
    },
    initPage(val) {
      if (val) {
        var that = this
        that.$store.dispatch('getTableList', {
          data: val,
          call: function(res) {
            that.tableList = []
            res.data.forEach(r => {
              that.tableList.push({
                name: r
              })
              that.soundTableList.push({
                name: r
              })
            })
          }
        })
      }
    }
  },
  computed: {
    ...mapState(['request'])
  },
  watch: {
    'request'(val) {
      this.initPage(val.dbName)
    },
    findTable(val) {
      if (!val) {
        this.tableList = this.soundTableList
        return
      }
      var list = []
      this.soundTableList.forEach(s => {
        if (s.name.toLowerCase().indexOf(val) > -1) {
          list.push({
            name: s.name
          })
        }
      })
      this.tableList = list
    },
    editableTabsValue(val) {
      var info = lodash.find(this.infoList, function(i) {
        return i.name === val
      })
      if (info) {
        this.info = info.data
      } else {
        this.info = []
      }
      if (val.indexOf('(') > -1) {
        this.currentInfo.table = val.split('(')[0]
        this.currentInfo.describe = val.split('(')[1].substring(0, val.split('(')[1].length - 1)
      } else {
        this.currentInfo.table = val.split('(')[0]
      }
      this.currentInfo.db = this.$store.state.request.dbName
    }
  },
  mounted() {
    this.initPage(this.request.dbName)
  },
  beforeRouteEnter(to, from, next) {
    next(vm => {
      vm.$store.commit('setRequest', to.params)
    })
  },
  beforeRouteUpdate(to, from, next) {
    this.$store.commit('setRequest', to.params)
    next()
  }
}
</script>

