<template>
  <div class="backend-container">
    <!-- 左侧菜单区域 - 支持折叠 -->
    <aside class="backend-sidebar">
      <div class="sidebar-menu">
        <div
          v-for="group in menuGroups"
          :key="group.groupKey"
          class="menu-group"
        >
          <div
            class="group-header"
            @click="toggleGroup(group.groupKey)"
          >
            <img :src="getAssetPath(group.icon)" :alt="group.groupTitle" class="group-icon" />
            <h3 class="group-title" :class="{ 'hidden-text': isCollapsed }">{{ group.groupTitle }}</h3>
            <img
              src="/src/assets/折叠.png"
              alt="展开/折叠"
              class="group-arrow"
              :class="{ 'rotated': groupCollapsed[group.groupKey], 'hidden-arrow': isCollapsed }"
            />
          </div>
          <ul
            class="menu-list"
            :class="{ 'collapsed-group': groupCollapsed[group.groupKey] }"
          >
            <li
              v-for="item in group.children"
              :key="item.key"
              class="menu-item"
              :class="{ active: $route.name === item.name }"
              @click="handleMenuClick(item.key)"
            >
              <img :src="getAssetPath(item.icon)" :alt="item.title" class="menu-icon" />
              <span :class="{ 'hidden-text': isCollapsed }" style="color: black;">{{ item.title }}</span>
            </li>
          </ul>
        </div>
      </div>
    </aside>

    <!-- 右侧内容区域（路由视图） -->
    <main class="backend-content" :class="{ 'expanded': isCollapsed }">
      <div class="content-body">
        <el-tabs v-model="activeTab" type="card" @tab-remove="removeTab" @tab-click="handleTabClick" closable>
          <el-tab-pane
            v-for="tab in tabs"
            :key="tab.name"
            :label="tab.title"
            :name="tab.name"
            :closable="tab.name !== 'HomeIndex'"
          >
            <component :is="tab.component" />
          </el-tab-pane>
        </el-tabs>
      </div>
    </main>
  </div>
</template>

<script lang="ts" setup>
import { ref, onMounted, shallowRef, watch } from 'vue';
import { useRouter, useRoute } from 'vue-router';
import { getMenuList } from '../../../api/login';
import { pa } from 'element-plus/es/locales.mjs';

function getAssetPath(path: string) {
  // 开发环境用 /src/assets/，生产环境用 /assets/
  if (import.meta.env.MODE === 'development') {
    // Vite开发环境下，public目录下的图片也可以用 /assets/xxx.png 访问
    return path;
  } else {
    // 打包后，public目录下的图片在 /assets/xxx.png
    return path.replace('/src/', '/');
  }
}
// 模拟接口获取菜单数据
async function fetchMenuData() {
  // 这里模拟接口返回的数据结构，和你原本的菜单结构一致
  return await getMenuList().then((res:any)=>{
    console.log(res.response)
    return res.response
  })
}

const router = useRouter();
const route = useRoute();

const isCollapsed = ref(false);
const groupCollapsed = ref<Record<string, boolean>>({});
const menuGroups = ref<any[]>([]);
const activeTab = ref('HomeIndex');
const tabs = ref<any[]>([]);



const toggleGroup = (group: string) => {
  if (!isCollapsed.value) {
    groupCollapsed.value[group] = !groupCollapsed.value[group];
  }
};

const handleMenuClick = (key: string) => {
  // 查找菜单项的name
  let menuName = '';
  for (const group of menuGroups.value) {
    const item = group.children.find((c: any) => c.key === key);
    if (item) {
      menuName = item.name;
      break;
    }
  }
  if (menuName) {
    router.push({ path: `/Layout/Backendhome/${menuName}` });
  }
};



function addTab(routeName: string) {
  // 查找菜单项
  let tabTitle = '后台管理';
  for (const group of menuGroups.value) {
    const item = group.children.find((c: any) => c.name === routeName);
    if (item) {
      tabTitle = item.title;
      break;
    }
  }
  // 判断是否已存在
  if (!tabs.value.find(t => t.name === routeName)) {
    tabs.value.push({ name: routeName, title: tabTitle, component: shallowRef('router-view') });
  }
  activeTab.value = routeName;
}
function removeTab(name: string) {
  const idx = tabs.value.findIndex(t => t.name === name);
  if (idx !== -1) {
    tabs.value.splice(idx, 1);
    // 切换到最后一个标签
    if (tabs.value.length) {
      activeTab.value = tabs.value[tabs.value.length - 1].name;
       router.push({ path: `/Layout/Backendhome/${activeTab.value}` });
    }
  }
}
function handleTabClick(tab: any) {
  console.log('点击了标签:', tab.paneName);
   router.push({ path: `/Layout/Backendhome/${ tab.paneName}` });
}
watch(() => route.name, (newName) => {
  if (typeof newName === 'string') {
    addTab(newName);
  }
});

onMounted(async () => {
  // 获取菜单数据
  const data = await fetchMenuData();
  menuGroups.value = data;
  // 初始化折叠状态
  data.forEach((g: any) => {
    groupCollapsed.value[g.groupKey] = true;
  });
  // 默认跳转首页
  if (route.path === '/Layout/Backendhome') {
    router.push({ path: `/Layout/Backendhome/HomeIndex` });
  }
  addTab(route.path.replace('/Layout/Backendhome/', ''));
});
</script>

<style scoped>
/* 保持原有样式不变，样式会自动适配新增菜单 */
.backend-container {
 display: flex;
 height: 100%;
 width: 100%;
 transition: all 0.3s ease;
}

.backend-sidebar {
 width: 220px;
 background-color: #ffffff;
 color: #ECF0F1;
 height: 100%;
 box-shadow: 2px 0 5px rgba(0, 0, 0, 0.1);
 /* 关键修改：隐藏滚动条但保留滚动功能 */
 overflow-y: auto;
 scrollbar-width: none; /* Firefox 隐藏滚动条 */
 -ms-overflow-style: none; /* IE 和 Edge 隐藏滚动条 */
 transition: width 0.3s ease;
 position: relative;
 display: flex;
 flex-direction: column;
 align-items: center;
}

/* Chrome, Safari 和 Opera 隐藏滚动条 */
.backend-sidebar::-webkit-scrollbar {
 display: none;
}

.backend-sidebar.collapsed {
 width: 60px;
}

.collapse-btn {
  position: absolute;
  top: 10px;
  right: -10px;
  width: 30px;
  height: 30px;
  background-color: #2C3E50;
  border-radius: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
  cursor: pointer;
  box-shadow: 0 0 5px rgba(0, 0, 0, 0.2);
  z-index: 10;
}

.collapse-icon {
  width: 30px;
  height: 30px;
  filter: invert(1);
  transition: transform 0.3s ease;
}

.collapse-icon.rotated {
  transform: rotate(180deg);
}

.sidebar-menu {
  padding-top: 40px;
  width: 100%;
}

.menu-group {
  margin-bottom: 10px;
  border-radius: 4px;
  overflow: hidden;
  width: 100%;
}

.group-header {
  display: flex;
  align-items: center;
  justify-content: space-between;
  padding: 0 10px;
  cursor: pointer;
  width: 100%;
}

.group-header:hover {
  background-color: #34495E;
}

.group-icon {
  width: 20px;
  height: 20px;
  margin-right: 10px;
  /* filter: invert(1); */
}

.group-title {
  font-size: 14px;
  color: #0a0b0b;
  padding: 10px 0;
  margin: 0;
  transition: opacity 0.3s ease;
  flex: 1;
}

.group-arrow {
  width: 20px;
  height: 20px;
  /* filter: invert(0.7); */
  transition: transform 0.3s ease;
  margin-left: 5px;
}

.group-arrow.rotated {
  transform: rotate(90deg);
}

.group-arrow.hidden-arrow {
  display: none;
}

.menu-list {
  list-style: none;
  padding: 5px 0;
  margin: 0;
  transition: max-height 0.3s ease, padding 0.3s ease;
  max-height: 500px;
  width: 100%;
}

.menu-list.collapsed-group {
  max-height: 0;
  padding: 0;
  overflow: hidden;
}

.menu-item {
  display: flex;
  align-items: center;
  padding: 12px 15px;
  cursor: pointer;
  transition: background-color 0.2s;
  border-radius: 4px;
  margin: 0 5px 4px;
  width: 100%;
}

.menu-item:hover {
  background-color: #34495E;
}

.menu-item.active {
  background-color: #1D99AD;
  border-left: 3px solid #fff;
}

.menu-icon {
  width: 20px;
  height: 20px;
  margin-right: 10px;
  /* filter: invert(1); */
}

.hidden-text {
  opacity: 0;
  width: 0;
  overflow: hidden;
  transition: all 0.3s ease;
}

.backend-content {
  flex: 1;
  padding: 10px;
  overflow-y: auto;
  background-color: #fff;
  transition: margin-left 0.3s ease;
}

.backend-content.expanded {
  margin-left: 0;
}

.content-header {
  display: flex;
  justify-content: space-between;
  align-items: center;
  margin-bottom: 20px;
  padding-bottom: 10px;
  border-bottom: 1px solid #eee;
}

.page-title {
  font-size: 18px;
  color: #333;
  margin: 0;
}

.content-body {
  min-height: calc(100% - 60px);
}

@media (max-width: 768px) {
  .backend-sidebar {
    width: 60px;
  }
  
  .backend-sidebar:not(.collapsed) {
    width: 220px;
  }
  
  .group-title, .menu-item span, .group-arrow {
    opacity: 0;
    width: 0;
    display: none;
  }
  
  .backend-sidebar:not(.collapsed) .group-title,
  .backend-sidebar:not(.collapsed) .menu-item span,
  .backend-sidebar:not(.collapsed) .group-arrow {
    opacity: 1;
    width: auto;
    display: block;
  }
}
</style>