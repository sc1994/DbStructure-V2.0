<style>
.db-info {
  margin-top: 10px;
  width: 98%;
  margin-left: 1%;
}
</style>


<template>
  <div id="app">
    <Menu mode="horizontal" theme="light" :active-name="activeName" @on-select="gotoUrl">
      <MenuItem name="0">
      <Icon type="coffee" size="22"></Icon>
      <span style="font-size:18px">数据结构查询工具</span>
      </MenuItem>
      <Submenu name="1">
        <template slot="title">
          <Icon type="soup-can-outline" size="16"></Icon>
          {{currentDb}}
        </template>
        <MenuGroup title="库列表">
          <MenuItem :name="db.id" v-for="(db,index) in dbList" :key="index">{{db.label}}</MenuItem>
        </MenuGroup>
      </Submenu>
      <MenuItem name="2" v-show="activeName == '2'">
      <Icon type="search" size="16"></Icon>
      {{currentDb}}..{{currentTable}}
      </MenuItem>
    </Menu>
    <router-view></router-view>
  </div>
</template>

<script>
import { mapState } from 'vuex'
import lodash from 'lodash'

export default {
  name: 'app',
  data() {
    return {
      currentDb: 'master',
      currentTable: 'master',
      dbList: [
        {
          id: '1-0',
          label: '加载中...'
        }
      ],
      activeName: '0'
    }
  },
  computed: {
    ...mapState([
      'suncheng',
      'request'
    ])
  },
  methods: {
    gotoUrl(data) {
      switch (data) {
        case '0':
          this.currentDb = 'master'
          this.currentTable = 'master'
          this.$router.push({ path: '/' })
          break
        case '1-0':
        case '1-1':
        case '1-2':
        case '1-3':
        case '1-4':
        case '1-5':
        case '1-6':
        case '1-7':
        case '1-8':
        case '1-9':
          var db = lodash.find(this.dbList, function(db) {
            return db.id === data
          })
          if (db) {
            this.currentDb = db.label
            this.$router.push({ path: '/dbInfo/' + this.currentDb })
            // this.$store.commit('setRequest', { dbName: this.currentDb })
          }
          break
        case '2':
          this.$router.push({ path: `/data/${this.request.db}/${this.request.table}` }); break
      }
    }
  },
  watch: {
    'request'(val) {
      if (val.dbName) {
        var db = lodash.find(val, function(db) {
          return db.label === val.dbName
        })
        this.activeName = db ? db.id : '1'
        this.currentDb = val.dbName
      } else if (val.db) {
        this.activeName = '2'
        this.currentDb = val.db
        this.currentTable = val.table
      }
    },
    'dbList'(val) {
      var that = this
      if (that.request.dbName) {
        var db = lodash.find(val, function(db) {
          return db.label === that.request.dbName
        })
        that.activeName = db ? db.id : '1'
      }
    }
  },
  mounted() {
    var that = this
    // this.$store.commit('setRequest', this.$route.params)
    this.$store.dispatch('getDbList', {
      data: '',
      call: function(res) {
        that.dbList = res.data
      }
    })
  }
}
</script>




