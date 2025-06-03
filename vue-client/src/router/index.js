// router/index.js
import { createRouter, createWebHistory } from "vue-router";
import HelloApi from "../components/HelloApi.vue";
// import AuthPage from "../components/AuthPage.vue";
import LoginPage from "../components/LoginPage.vue";
import RegisterPage from "../components/RegisterPage.vue";
import TopicMenu from "../components/TopicMenu.vue";
import TopicDetail from "../components/TopicDetail.vue";
import ChatPage from "../components/ChatPage.vue";
import ConversationPage from "../components/ConversationPage.vue";

const routes = [
  { path: "/", component: HelloApi },
  // { path: "/AuthPage", component: AuthPage },
  { path: "/login", component: LoginPage },
  { path: "/register", component: RegisterPage },
  { path: "/", redirect: "/login" },
  { path: "/TopicMenu", component: TopicMenu },
  { path: "/topics/:id", component: TopicDetail },
  {
    path: "/ChatPage",
    component: ChatPage,
    meta: { requiresAuth: true },
  },
  {
    path: "/ConversationPage",
    component: ConversationPage,
    meta: { requiresAuth: true },
  },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

// ✅ 全域守衛：限制未登入用戶進入特定頁面
router.beforeEach((to, from, next) => {
  const token = localStorage.getItem("token");

  if (to.meta.requiresAuth && !token) {
    next("/AuthPage"); // 導向登入頁
  } else {
    next(); // 放行
  }
});

export default router;
