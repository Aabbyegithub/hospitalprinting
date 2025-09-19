<template>
  <div class="layout-container">
    <!-- 顶部导航栏 -->
    <header class="top-nav">
      <div class="logo">
        <img src="/src/assets/main.png" alt="系统图标" class="logo-image" />
        <span class="system-name">***系统</span>
      </div>
      <!-- 用户图标及下拉退出 -->
      <el-dropdown trigger="click" @command="handleCommand">
        <div class="user-icon">
          <img 
            src="/src/assets/Frame-4.png" 
            alt="系统图标" 
            class="User-img"
          >
        </div>
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item command="logout">退出系统</el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
    </header>
    <!-- 内容区域，动态渲染路由组件 -->
    <main class="content">
      <router-view />
    </main>
  </div>
</template>

<script lang="ts" setup>
import { ref, onMounted } from 'vue';
import { useRouter } from 'vue-router';
import { ElButton, ElDropdown, ElDropdownMenu, ElDropdownItem } from 'element-plus';
import { logoutApi } from '../../../api/login';

const router = useRouter();
const currentRoute = ref('');
const isShow = ref(true);

function updateCurrentRoute() {
  const route = router.currentRoute.value;
  if (route.path.includes('Backendhome')) {
    currentRoute.value = 'Backendhome';
  } else {
    currentRoute.value = '';
  }
  console.log('当前路由变化:', route.fullPath, currentRoute.value);
};

// 初始化时设置当前路由
onMounted(() => {
  const userInfoStr = localStorage.getItem('UserInfo');
  const orgId = userInfoStr ? JSON.parse(userInfoStr).orgId : null;
  isShow.value = orgId !== 1;
  // 只在首次进入（没有子路由时）才跳转，刷新时保留当前页面
  const currentPath = router.currentRoute.value.fullPath;
  if (currentPath === '/Layout' || currentPath === '/Layout/') {
<<<<<<< HEAD
      router.push('/Layout/Backendhome/HomeIndex');
=======
    if (orgId == 1) {
      router.push('/Layout/Backendhome/HomeIndex');
    } else {
      router.push('/Layout/Orderhome');
    }
>>>>>>> 96f58664de39bae2f7a0c7785cbe650642840ad9
  }
  updateCurrentRoute();
});

<<<<<<< HEAD
=======
const handleNavClick = (routeName: string) => {
  currentRoute.value = routeName;
  if (routeName === 'Orderhome') {
    router.push('/Layout/Orderhome');
  } else if (routeName === 'Backendhome') {
    router.push('/Layout/Backendhome/HomeIndex');
  }
  // router.push(`/Layout/${routeName}`);
};

>>>>>>> 96f58664de39bae2f7a0c7785cbe650642840ad9
// 处理下拉框命令（退出系统逻辑）
const handleCommand = async (command: string) => {
  if (command === 'logout') {
    await logoutApi();
    router.push('/login'); 
  }
};
</script>

<style scoped>
.layout-container {
  display: flex;
  flex-direction: column;
  height: 100vh;
}
.top-nav {
  display: flex;
  align-items: center;
  justify-content: space-between;
  background: #1D99AD;
  padding: 0 20px;
  color: #fff;
  background: url("/src/assets/底部头部.png") no-repeat center center;
  background-size: 110% auto; 
  height: 60px;
  width: 100%;
}
.logo {
  display: flex;
  align-items: center;
  gap: 8px;
}
.logo-image {
  width: 40px;  
  height: 40px;
}
.system-name {
  font-size: 25px;
  font-weight: 500;
}
.User-img {
  width: 30px;
  height: 30px;
  cursor: pointer;
}
.content {
  flex: 1;
  overflow: auto;
  background-color: #f5f7fa;
}
/* 下拉框样式可按需调整 */
.el-dropdown-menu {
  min-width: 100px;
}
</style>