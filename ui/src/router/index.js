import Vue from 'vue'
import Router from 'vue-router'
import DbInfo from '@/components/dbInfo.vue'
import Data from '@/components/data.vue'

Vue.use(Router)

export default new Router({
  routes: [
    {
      path: '/dbInfo/:dbName',
      component: DbInfo
    },
    {
      path: '/data/:db/:table',
      component: Data
    }
  ]
})
