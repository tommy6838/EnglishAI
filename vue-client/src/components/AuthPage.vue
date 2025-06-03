<script setup>
import { ref } from "vue";
import axios from "axios";
import { useRouter } from "vue-router";

const registerUsername = ref("");
const registerPassword = ref("");
const loginUsername = ref("");
const loginPassword = ref("");
const router = useRouter();

// 註冊
const register = async () => {
  try {
    await axios.post("http://localhost:5153/api/users/register", {
      userName: registerUsername.value,
      password: registerPassword.value,
    });
    alert("註冊成功，請登入");
    registerUsername.value = "";
    registerPassword.value = "";
  } catch (err) {
    alert("註冊失敗：" + err.message);
  }
};

// 登入
const login = async () => {
  try {
    const res = await axios.post("http://localhost:5153/api/users/login", {
      userName: loginUsername.value,
      password: loginPassword.value,
    });
    localStorage.setItem("token", res.data.token);
    alert("登入成功");
    router.push("/ConversationPage");
  } catch (err) {
    alert("登入失敗：" + err.message);
  }
};
</script>

<template>
  <div class="min-h-screen flex items-center justify-center bg-gray-100 p-4">
    <div class="w-full max-w-md bg-white p-8 rounded-xl shadow-lg">
      <h2 class="text-2xl font-bold text-center text-blue-600 mb-6">
        AI 英文學習平台
      </h2>

      <!-- 註冊區 -->
      <h3 class="text-lg font-semibold mb-2 text-gray-700">註冊</h3>
      <input
        v-model="registerUsername"
        type="text"
        placeholder="輸入帳號"
        class="w-full border rounded p-2 mb-2"
      />
      <input
        v-model="registerPassword"
        type="password"
        placeholder="輸入密碼"
        class="w-full border rounded p-2 mb-4"
      />
      <button
        @click="register"
        class="w-full bg-green-500 hover:bg-green-600 text-white font-medium py-2 rounded"
      >
        註冊
      </button>

      <hr class="my-6 border-gray-300" />

      <!-- 登入區 -->
      <h3 class="text-lg font-semibold mb-2 text-gray-700">登入</h3>
      <input
        v-model="loginUsername"
        type="text"
        placeholder="輸入帳號"
        class="w-full border rounded p-2 mb-2"
      />
      <input
        v-model="loginPassword"
        type="password"
        placeholder="輸入密碼"
        class="w-full border rounded p-2 mb-4"
      />
      <button
        @click="login"
        class="w-full bg-blue-500 hover:bg-blue-600 text-white font-medium py-2 rounded"
      >
        登入
      </button>
    </div>
  </div>
</template>
