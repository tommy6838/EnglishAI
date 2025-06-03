<script setup>
import HelloApi from "./components/HelloApi.vue";

import { useRouter } from "vue-router";
const router = useRouter();

const logout = () => {
  localStorage.removeItem("token");
  localStorage.removeItem("userId");
  router.push("/AuthPage"); // 登出後導回登入頁
};
</script>

<template>
  <header>
    <h1>AI 英語學習平台 - Hello 前端！</h1>
    <!-- 放在畫面右上角等地方 -->
    <button @click="logout" class="text-red-500 hover:underline">登出</button>
  </header>
  <router-view />
</template>

<style scoped>
.logo {
  height: 6em;
  padding: 1.5em;
  will-change: filter;
  transition: filter 300ms;
}
.logo:hover {
  filter: drop-shadow(0 0 2em #646cffaa);
}
.logo.vue:hover {
  filter: drop-shadow(0 0 2em #42b883aa);
}
</style>
