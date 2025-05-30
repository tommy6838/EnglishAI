<script setup>
import HelloApi from "./components/HelloApi.vue";
</script>

<template>
  <header>
    <h1>AI 英語學習平台 - Hello 前端！</h1>
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
