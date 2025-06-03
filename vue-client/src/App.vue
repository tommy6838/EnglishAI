<script setup>
import { useRouter } from "vue-router";
import { ref } from "vue";

// router 導向
const router = useRouter();

// reactive 狀態：登入判斷
const token = ref(localStorage.getItem("token"));

// 登出邏輯：清除 token 並導向登入頁
function logout() {
  localStorage.removeItem("token");
  token.value = null;
  router.push("/login");
}
</script>

<template>
  <div>
    <!-- ✅ 導覽列 -->
    <header class="bg-white shadow sticky top-0 z-50">
      <div
        class="max-w-7xl mx-auto px-4 py-2 flex justify-between items-center"
      >
        <h1 class="text-2xl font-bold text-blue-600">AI 英文學習平台</h1>
        <div class="space-x-4">
          <!-- 尚未登入顯示 -->
          <button
            v-if="!token"
            @click="router.push('/login')"
            class="text-blue-600 hover:underline"
          >
            登入
          </button>
          <button
            v-if="!token"
            @click="router.push('/register')"
            class="text-blue-600 hover:underline"
          >
            註冊
          </button>

          <!-- 已登入顯示 -->
          <button v-else @click="logout" class="text-red-500 hover:underline">
            登出
          </button>
        </div>
      </div>
    </header>

    <!-- 🚀 畫面主體 -->
    <main class="p-4 max-w-5xl mx-auto">
      <router-view />
    </main>
  </div>
</template>

<style scoped>
/* 可加入一些全域樣式或背景 */
</style>
