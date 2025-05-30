// router/index.js
import { createRouter, createWebHistory } from "vue-router";
import HelloApi from "../components/HelloApi.vue";
import AuthPage from "../components/AuthPage.vue";
import TopicMenu from "../components/TopicMenu.vue";
import TopicDetail from "../components/TopicDetail.vue";
import ChatPage from "../components/ChatPage.vue";

const routes = [
  { path: "/", component: HelloApi },
  { path: "/AuthPage", component: AuthPage },
  { path: "/TopicMenu", component: TopicMenu },
  { path: "/topics/:id", component: TopicDetail },
  { path: "/ChatPage", component: ChatPage },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
