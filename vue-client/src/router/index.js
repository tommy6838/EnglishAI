// router/index.js
import { createRouter, createWebHistory } from "vue-router";
import HelloApi from "../components/HelloApi.vue";
import AuthPage from "../components/AuthPage.vue";

const routes = [
  { path: "/", component: HelloApi },
  { path: "/AuthPage", component: AuthPage },
];

const router = createRouter({
  history: createWebHistory(),
  routes,
});

export default router;
