<script setup>
import { ref } from "vue";
import { useRouter } from "vue-router";
import api from "../utils/axios"; // 使用封裝的 axios

const registerUsername = ref("");
const registerPassword = ref("");
const router = useRouter();

// 註冊功能
const register = async () => {
  try {
    await api.post("/accounts/register", {
      userName: registerUsername.value,
      password: registerPassword.value,
    });
    alert("註冊成功，請登入");
    router.push("/login"); // 導向登入頁
  } catch (err) {
    alert("註冊失敗：" + err.response?.data?.message || err.message);
  }
};
</script>

<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-100 p-4">
    <div class="w-full max-w-md bg-white p-8 rounded-xl shadow-lg">
      <h2 class="text-2xl font-bold text-center text-green-600 mb-6">
        會員註冊
      </h2>

      <!-- 帳號輸入 -->
      <input
        v-model="registerUsername"
        type="text"
        placeholder="輸入帳號"
        class="w-full border rounded p-2 mb-2"
      />

      <!-- 密碼輸入 -->
      <input
        v-model="registerPassword"
        type="password"
        placeholder="輸入密碼"
        class="w-full border rounded p-2 mb-4"
      />

      <!-- 註冊按鈕 -->
      <button
        @click="register"
        class="w-full bg-green-500 hover:bg-green-600 text-white font-medium py-2 rounded"
      >
        註冊
      </button>
    </div>
  </div>
</template>
