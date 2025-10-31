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
            path: 'DataStatistics', 
            name: 'DataStatistics',
            component: () => import('../views/Menu/BackendHome/Hospital/DataStatistics.vue') 
          },
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
          { 
            path: 'PatientManagement', 
            name: 'PatientManagement',
            component: () => import('../views/Menu/BackendHome/Hospital/PatientManagement.vue') 
          },
          { 
            path: 'ExaminationManagement', 
            name: 'ExaminationManagement',
            component: () => import('../views/Menu/BackendHome/Hospital/ExaminationManagement.vue') 
          },
          { 
            path: 'PrintRecordManagement', 
            name: 'PrintRecordManagement',
            component: () => import('../views/Menu/BackendHome/Hospital/PrintRecordManagement.vue') 
          },
          { 
            path: 'DoctorManagement', 
            name: 'DoctorManagement',
            component: () => import('../views/Menu/BackendHome/Hospital/DoctorManagement.vue') 
          },
          { 
            path: 'DepartmentManagement', 
            name: 'DepartmentManagement',
            component: () => import('../views/Menu/BackendHome/Hospital/DepartmentManagement.vue') 
          },
          { 
            path: 'SelfServicePrinter', 
            name: 'SelfServicePrinter',
            component: () => import('../views/Menu/BackendHome/Hospital/SelfServicePrinter.vue') 
          },
          { 
            path: 'FilmPrinter', 
            name: 'FilmPrinter',
            component: () => import('../views/Menu/BackendHome/Hospital/FilmPrinter.vue') 
          },
          { 
            path: 'ReportPrinter', 
            name: 'ReportPrinter',
            component: () => import('../views/Menu/BackendHome/Hospital/ReportPrinter.vue') 
          },
          { 
            path: 'LaserPrinter', 
            name: 'LaserPrinter',
            component: () => import('../views/Menu/BackendHome/Hospital/LaserPrinter.vue') 
          },
          { 
            path: 'FolderMonitorManagement', 
            name: 'FolderMonitorManagement',
            component: () => import('../views/Menu/BackendHome/Hospital/FolderMonitorManagement.vue') 
          },
          { 
            path: 'OcrConfigManagement', 
            name: 'OcrConfigManagement',
            component: () => import('../views/Menu/BackendHome/Hospital/OcrConfigManagement.vue') 
          },
          { 
            path: 'OssConfigManagement', 
            name: 'OssConfigManagement',
            component: () => import('../views/Menu/BackendHome/Hospital/OssConfigManagement.vue') 
          },
          { 
            path: 'DbConnectionManagement', 
            name: 'DbConnectionManagement',
            component: () => import('../views/Menu/BackendHome/Hospital/DbConnectionManagement.vue') 
          },
          { 
            path: 'PrintTemplateManagement', 
            name: 'PrintTemplateManagement',
            component: () => import('../views/Menu/BackendHome/Hospital/PrintTemplateManagement.vue') 
          },
          { 
            path: 'AiConfigManagement', 
            name: 'AiConfigManagement',
            component: () => import('../views/Menu/BackendHome/Hospital/AiConfigManagement.vue') 
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
