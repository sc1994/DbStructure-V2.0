import Vue from 'vue'
import Vuex from 'vuex'
import axios from 'axios'

Vue.use(Vuex)

axios.defaults.baseURL = 'http://192.168.0.102:7010/api/'

const store = new Vuex.Store({
  state: {
    userInfo: {
      name: 'suncheng'
    },
    request: {}
  },
  mutations: {
    setRequest(state, data) {
      state.request = data
    }
  },
  actions: {
    getDbList({ commit }, data) {
      axios.post('DataBase/GetAllDataBase?dbName=' + data.data).then(function (res) {
        data.call(res)
      })
    },
    getTableList({ commit }, data) {
      axios.post('DataBase/GetTablesByDb?dbName=' + data.data).then(function (res) {
        data.call(res)
      })
    },
    getTableInfo({ commit }, data) {
      axios.post(`Table/GetTableInfo?table=${data.data}&dbName=${this.state.request.dbName}`).then(function (res) {
        data.call(res)
      })
    },
    editTableDescribe({ commit }, data) {
      axios.post(`Table/EditTableDescribe?db=${data.db}&table=${data.table}&describe=${data.describe}`).then(function (res) {
        data.call(res)
      })
    },
    getDataHead({ commit }, data) {
      axios.get(`Data/GetDataHead?db=${data.db}&table=${data.table}`).then(function (res) {
        data.call(res)
      })
    },
    getDataList({ commit }, data) {
      axios.post(`Data/GetDataList?sql=` + data.data)
        .then(function (res) {
          data.call(res)
        })
        .catch(function (err) {
          if (err) {
            data.errCall(err.response.data.ExceptionMessage)
          }
        })
    }
  }
})

export default store
