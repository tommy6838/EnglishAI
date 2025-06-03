<script setup>
// ✅ 導入必要工具
import { ref } from "vue";
import { useRouter } from "vue-router";
import api from "../utils/axios"; // 使用封裝好的 axios，會自動加上 token

// 登入欄位綁定
const loginUsername = ref("");
const loginPassword = ref("");
const router = useRouter();

// ✅ 登入函式
const login = async () => {
  try {
    // 呼叫後端登入 API，注意欄位名稱為 LoginKey / Password
    const res = await api.post("/accounts/login", {
      LoginKey: loginUsername.value,
      Password: loginPassword.value,
    });

    // 儲存 JWT token 到 localStorage
    localStorage.setItem("token", res.data.token);
    alert("登入成功");

    // 登入成功後導向對話頁
    router.push("/ConversationPage");
  } catch (err) {
    // ❗安全處理錯誤訊息，避免 res 未定義錯誤
    const errorMessage = err.response?.data || err.message;
    alert("登入失敗: " + errorMessage);
  }
};
</script>

<template>
  <!-- 📦 畫面置中區塊 -->
  <div class="min-h-screen flex items-center justify-center bg-gray-100 p-4">
    <div class="w-full max-w-md bg-white p-8 rounded-xl shadow-lg">
      <h2 class="text-2xl font-bold text-center text-blue-600 mb-6">
        會員登入
      </h2>

      <!-- ✅ 帳號輸入 -->
      <input
        v-model="loginUsername"
        type="text"
        placeholder="輸入帳號或 Email"
        class="w-full border rounded p-2 mb-2"
      />

      <!-- ✅ 密碼輸入 -->
      <input
        v-model="loginPassword"
        type="password"
        placeholder="輸入密碼"
        class="w-full border rounded p-2 mb-4"
      />

      <!-- ✅ 登入按鈕 -->
      <button
        @click="login"
        class="w-full bg-blue-500 hover:bg-blue-600 text-white font-medium py-2 rounded"
      >
        登入
      </button>
    </div>
  </div>
</template>
