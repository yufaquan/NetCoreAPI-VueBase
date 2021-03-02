import { constantRoutes } from '@/router'
import { getroutes } from '@/api/permission'
import Layout from '@/layout'

/**
 * 循环生成异步路由
 * @Author   HarveyCheng
 * @DateTime 2018-10-09
 * @param    {[type]}    routes [description]
 * @return   {[type]}           [description]
 */
function loopCreateRouter(routes) {
  const res = []
  routes.forEach(route => {
    const tmp = { ...route }
    const tpl = Object.assign({}, tmp)
    if (typeof tmp['component'] !== 'undefined' && tmp['component']) {
      if (tmp['component'] === 'Layout') {
        tpl['component'] = Layout
      } else {
        tpl['component'] = resolve => require([`../../${tmp['component']}.vue`], resolve)
      }
    }
    if (tmp['children']) {
      tpl['children'] = loopCreateRouter(tmp['children'])
    }
    res.push(tpl)
  })
  return res
}

const state = {
  routes: [],
  second_routes: [],
  third_routes: []
}

const mutations = {
  SET_ROUTES: (state, routes) => {
    state.routes = routes
  },
  SET_SECOND_ROUTES: (state, routes) => {
    state.second_routes = routes
  },
  SET_THIRD_ROUTES: (state, routes) => {
    state.third_routes = routes
  }
}
const routes = [
  {
    path: '/',
    component: 'Layout',
    redirect: '/dashboard',
    name: 'Home',
    alwaysShow: true,
    children: [{
      path: '/dashboard',
      name: 'Dashboard',
      component: 'views/dashboard/index',
      meta: { title: '首页', icon: 'dashboard' }
    }]
  },

  {
    path: '/ads',
    component: 'Layout',
    redirect: '/ads/banner',
    name: 'Ads',
    meta: { title: '广告管理', icon: 'el-icon-help' },
    children: [
      {
        path: '/ads/banner',
        name: 'Banner',
        component: 'views/banner/index',
        meta: { title: '轮播图' }
      },
      {
        path: '/ads/link',
        name: 'Link',
        component: 'views/link/index',
        meta: { title: '友情链接' }
      }
    ]
  },

  {
    path: '/article',
    component: 'Layout',
    redirect: '/article/list',
    name: 'Article',
    meta: { title: '文章管理', icon: 'el-icon-document' },
    children: [
      {
        path: '/article/list',
        name: 'ArticleList',
        component: 'views/article/index',
        meta: { title: '文章管理' }
      },
      {
        path: '/article/category',
        name: 'ArticleCategory',
        component: 'views/article/category',
        meta: { title: '文章分类' }
      }
    ]
  },

  {
    path: '/data',
    component: 'Layout',
    redirect: '/data/article',
    name: 'Data',
    meta: { title: '数据统计', icon: 'el-icon-s-data' },
    children: [
      {
        path: '/data/article',
        name: 'Article',
        component: 'views/data/index',
        meta: { title: '文章统计' }
      }
    ]
  },

  {
    path: '/nested',
    component: 'Layout',
    redirect: '/nested/menu1',
    name: 'Nested',
    meta: {
      title: '多级菜单',
      icon: 'el-icon-s-operation'
    },
    children: [
      {
        path: '/nested/menu1',
        alwaysShow: true,
        redirect: '/nested/menu1/menu1-1',
        component: 'views/nested/menu1/index', // Parent router-view
        name: 'Menu1',
        meta: { title: '二级菜单' },
        children: [
          {
            path: '/nested/menu1/menu1-1',
            component: 'views/nested/menu1/menu1-1/index',
            name: 'Menu1-1',
            meta: { title: '三级菜单' }
          },
          {
            path: '/nested/menu1/menu1-2',
            alwaysShow: true,
            redirect: 'noRedirect',
            component: 'views/nested/menu1/menu1-2/index',
            name: 'Menu1-2',
            meta: { title: '三级菜单' },
            children: [
              {
                path: '/nested/menu1/menu1-2/menu1-2-1',
                component: 'views/nested/menu1/menu1-2/menu1-2-1/index',
                name: 'Menu1-2-1',
                meta: { title: '四级菜单' }
              },
              {
                path: '/nested/menu1/menu1-2/menu1-2-2',
                component: 'views/nested/menu1/menu1-2/menu1-2-2/index',
                name: 'Menu1-2-2',
                meta: { title: '四级菜单' }
              }
            ]
          },
          {
            path: '/nested/menu1/menu1-3',
            component: 'views/nested/menu1/menu1-3/index',
            name: 'Menu1-3',
            meta: { title: '三级菜单' }
          }
        ]
      },
      {
        path: '/nested/menu2',
        component: 'views/nested/menu2/index',
        name: 'Menu2',
        meta: { title: '二级菜单' }
      }
    ]
  },

  {
    path: '/site',
    component: 'Layout',
    alwaysShow: true,
    children: [
      {
        path: '/site/index',
        name: 'Site',
        component: 'views/site/index',
        meta: { title: '站点设置', icon: 'el-icon-setting' }
      }
    ]
  },

  {
    path: '/setting',
    component: 'Layout',
    redirect: '/setting/user',
    name: 'Setting',
    meta: { title: '系统管理', icon: 'el-icon-s-tools' },
    children: [
      {
        path: '/setting/user',
        name: 'User',
        component: 'views/user/index',
        meta: { title: '用户管理' }
      },
      {
        path: '/setting/role',
        name: 'Role',
        component: 'views/role/index',
        meta: { title: '角色管理' }
      },
      {
        path: '/setting/menu',
        name: 'Menu',
        component: 'views/menu/index',
        meta: { title: '菜单管理' }
      },
      {
        path: '/setting/log',
        name: 'Log',
        component: 'views/log/index',
        meta: { title: '日志管理' },
        redirect: '/setting/log/api',
        children:[
          {
            path: '/setting/log/api',
            name: 'api',
            component: 'views/log/API',
            meta: { title: 'Api访问日志' }
          },
          {
            path: '/setting/log/event',
            name: 'event',
            component: 'views/log/Event',
            meta: { title: '系统事件日志' }
          },
          {
            path: '/setting/log/error',
            name: 'error',
            component: 'views/log/Error',
            meta: { title: '系统错误日志' }
          }
        ]
      }
    ]
  },

  {
    path: 'external-link',
    component: 'Layout',
    alwaysShow: true,
    children: [
      {
        path: 'https://panjiachen.gitee.io/vue-element-admin-site/zh/',
        meta: { title: '参考文档', icon: 'el-icon-link' }
      }
    ]
  },

  {
    path: 'external-link',
    component: 'Layout',
    alwaysShow: true,
    children: [
      {
        path: 'http://localhost:8002/',
        meta: { title: '前端模版', icon: 'el-icon-link' }
      }
    ]
  },
  {
    path: 'external-link',
    component: 'Layout',
    alwaysShow: true,
    children: [
      {
        path: 'http://localhost:8005/',
        meta: { title: 'API文档', icon: 'el-icon-link' }
      }
    ]
  }
]

const routes1=[{
  "name": "Home",
  "redirect": "/dashboard",
  "path": "/",
  "alwaysShow":true,
  "component": "Layout",
  "meta": {
    "title": null,
    "icon": null
  },
  "children": [
    {
      "name": "Dashboard",
      "path": "/dashboard",
      "alwaysShow":false,
      "redirect": null,
      "component": "views/dashboard/index",
      "meta": {
        "title": "首页",
        "icon": "dashboard"
      }
    }
  ]
  }
];

const actions = {
  generateRoutes({ commit }) {
    return new Promise((resolve, reject) => {
      getroutes().then(response => {
        const { data } = response
        const asyncRouterMap = JSON.parse(data.list)
        const accessedRoutes = loopCreateRouter(asyncRouterMap)
        accessedRoutes.push({ path: '*', redirect: '/404', hidden: true })
        commit('SET_ROUTES', constantRoutes.concat(accessedRoutes))
        resolve(accessedRoutes)
      }).catch(error => {
        reject(error)
      })

      // const asyncRouterMap = routes1
      // const accessedRoutes = loopCreateRouter(asyncRouterMap)
      // accessedRoutes.push({ path: '*', redirect: '/404', hidden: true })
      // commit('SET_ROUTES', constantRoutes.concat(accessedRoutes))
      // resolve(accessedRoutes)
    })
  },
  changeSecondRoutes({ commit, state }, data) {
    commit('SET_SECOND_ROUTES', [])
    const second_routes = state.routes.find(item => {
      return !item.hidden && (item.path === data.path || (data.path === '' && item.path === '/'))
    })
    if (typeof second_routes !== 'undefined' && second_routes.children) {
      const res = second_routes.children.filter(item => {
        return !item.hidden && item.path
      })
      if (res.length > 1) {
        commit('SET_SECOND_ROUTES', res)
      }
    }
  },
  changeThirdRoutes({ commit, state }, data) {
    commit('SET_THIRD_ROUTES', [])
    const third_routes = state.second_routes.find(item => {
      return !item.hidden && (item.path === data.path || (data.path === '' && item.path === '/'))
    })
    if (typeof third_routes !== 'undefined' && third_routes.children) {
      const res = third_routes.children.filter(item => {
        return !item.hidden && item.path
      })
      if (res.length > 1) {
        commit('SET_THIRD_ROUTES', res)
      }
    }
  }
}

export default {
  namespaced: true,
  state,
  mutations,
  actions
}
