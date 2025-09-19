import { createRouter, createWebHistory } from 'vue-router'
import Login from '../views/Login/Login.vue'

const routes = [
  { path: '/', redirect: '/login' },
  { path: '/login', component: Login },
  { 
    path: '/Layout', 
    component: () => import('../views/Menu/Layout/Layout.vue'),
    children: [
      { path: 'Backendhome',name: 'Backendhome',
        component: () => import('../views/Menu/BackendHome/BackendHome.vue'),
        children: [
          {
            path: 'HomeIndex', 
            name: 'HomeIndex',
            component: () => import('../views/Menu/BackendHome/Home/HomeIndex.vue')
          },
          { 
            path: 'StaffManagement', 
            name: 'StaffManagement',
            component: () => import('../views/Menu/BackendHome/System/StaffManagement.vue') 
          },
          { 
            path: 'RolePermission', 
            name: 'RolePermission',
            component: () => import('../views/Menu/BackendHome/System/RolePermissionManagement.vue') 
          },
          { 
            path: 'OrgSetting', 
            name: 'OrgSetting',
            component: () => import('../views/Menu/BackendHome/System/OrgManagement.vue') 
          },
          { 
            path: 'TaskManagement', 
            name: 'TaskManagement',
            component: () => import('../views/Menu/BackendHome/System/TimerTaskManagement.vue') 
          },
           { 
            path: 'OperationLogManagement', 
            name: 'OperationLogManagement',
            component: () => import('../views/Menu/BackendHome/System/OperationLogManagement.vue') 
          },
        ]
       },
    ]
  }
]

const router = createRouter({
  history: createWebHistory(),
  routes
})

export default router
