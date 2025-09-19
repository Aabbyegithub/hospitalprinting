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
            :src="AvatarUrl" 
            alt="系统图标" 
            class="User-img"
          >
          <span style="margin-left: 8px;color: black;">{{userName}}</span>
        </div>
        <template #dropdown>
          <el-dropdown-menu>
            <el-dropdown-item command="profile">个人中心</el-dropdown-item>
            <el-dropdown-item command="logout">退出系统</el-dropdown-item>
          </el-dropdown-menu>
        </template>
      </el-dropdown>
      <!-- 个人中心弹窗 -->
      <el-dialog v-model="showProfile" title="个人中心" width="400px" :close-on-click-modal="false">
        <div class="profile-dialog-wrap">
          <div class="profile-avatar-wrap">
            <img :src="profileUser.AvatarUrl || '/src/assets/Frame-4.png'" class="profile-avatar" alt="头像" />
          </div>
          <div class="profile-info">
            <div class="profile-row"><span class="profile-label">姓名：</span><span>{{ profileUser.name }}</span></div>
            <div class="profile-row"><span class="profile-label">账号：</span><span>{{ profileUser.username }}</span></div>
            <div class="profile-row"><span class="profile-label">手机号：</span><span>{{ profileUser.phone || '未填写' }}</span></div>
            <div class="profile-row"><span class="profile-label">职位：</span><span>{{ profileUser.position }}</span></div>
            <div class="profile-row"><span class="profile-label">角色：</span><span>{{ profileUser.role }}</span></div>
            <div class="profile-row"><span class="profile-label">上次最后登录：</span><span>{{ profileUser.last_login_time || '无' }}</span></div>
          </div>
        </div>
        <template #footer>
          <el-button @click="showProfile = false">关闭</el-button>
        </template>
      </el-dialog>
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
import { ElButton, ElDropdown, ElDropdownMenu, ElDropdownItem, ElDialog, dayjs } from 'element-plus';
import { GetUserDetial, logoutApi } from '../../../api/login';

const userInfoStr = localStorage.getItem('UserInfo');
const userName = userInfoStr ? JSON.parse(userInfoStr).userName : '用户';
const AvatarUrl = userInfoStr && JSON.parse(userInfoStr).avatarUrl ? JSON.parse(userInfoStr).avatarUrl : '/src/assets/Frame-4.png';
const router = useRouter();
const currentRoute = ref('');
const isShow = ref(true);

// 个人中心弹窗控制
const showProfile = ref(false);
const profileUser = ref<any>({});

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
      router.push('/Layout/Backendhome/HomeIndex');
  }
  updateCurrentRoute();
});

const getUserDetails = async () => {
  try {
    const res:any = await GetUserDetial();
    if (res && res.response) {
      profileUser.value = {
        AvatarUrl: res.response.avatarUrl || '/src/assets/Frame-4.png',
        name: res.response.name || '',
        username: res.response.username || '',
        phone: res.response.phone || '',
        position: res.response.position || '',
        role: res.response.role || '',
        last_login_time: dayjs(res.response.last_login_time).format('YYYY-MM-DD HH:mm:ss') || '',
      };
    } else {
      console.error('获取用户详情失败');
    }
  } catch (error) {
    console.error('获取用户详情失败', error);
  }
};

// 处理下拉框命令（退出系统/个人中心弹窗）
const handleCommand = async (command: string) => {
  if (command === 'logout') {
    await logoutApi();
    router.push('/login'); 
  }
  if (command === 'profile') {
    await getUserDetails();
    showProfile.value = true;
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
  width: 35px;
  height: 35px;
  cursor: pointer;
  border-radius: 50%;
}
.user-icon {
  display: flex;
  align-items: center;
  justify-content: center;
  height: 40px;
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
.profile-dialog-wrap {
  display: flex;
  flex-direction: column;
  align-items: center;
  padding: 10px 0 0 0;
}
.profile-avatar-wrap {
  display: flex;
  justify-content: center;
  align-items: center;
  margin-bottom: 18px;
}
.profile-avatar {
  width: 90px;
  height: 90px;
  border-radius: 50%;
  object-fit: cover;
  border: 2px solid #1D99AD;
  background: #f5f7fa;
}
.profile-info {
  width: 100%;
  display: flex;
  flex-direction: column;
  gap: 12px;
  font-size: 17px;
  color: #333;
}
.profile-row {
  display: flex;
  align-items: center;
  gap: 8px;
}
.profile-label {
  font-weight: 500;
  color: #1D99AD;
  min-width: 110px;
  text-align: right;
}
</style>