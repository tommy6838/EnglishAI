<template>
  <div>
    <h2>註冊</h2>
    <input v-model="registerUsername" placeholder="帳號" />
    <input v-model="registerPassword" type="password" placeholder="密碼" />
    <button @click="register">註冊</button>

    <h2>登入</h2>
    <input v-model="loginUsername" placeholder="帳號" />
    <input v-model="loginPassword" type="password" placeholder="密碼" />
    <button @click="login">登入</button>

    <p>{{ message }}</p>
  </div>
</template>

<script setup>
import { ref } from "vue";
import axios from "axios";

const registerUsername = ref("");
const registerPassword = ref("");
const loginUsername = ref("");
const loginPassword = ref("");
const message = ref("");

// 註冊
const register = async () => {
  try {
    const res = await axios.post(
      "http://localhost:5153/api/accounts/register",
      {
        email: registerUsername.value, // 小寫email
        password: registerPassword.value, // 小寫password
      }
    );
    message.value = "註冊成功，請登入";
  } catch (err) {
    message.value = err.response?.data || "註冊失敗";
  }
};

// 登入
const login = async () => {
  try {
    const res = await axios.post("http://localhost:5153/api/accounts/login", {
      loginKey: loginUsername.value, // 帳號或Email都可以
      password: loginPassword.value,
    });
    // 儲存token
    localStorage.setItem("token", res.data.token);
    message.value = "登入成功";
  } catch (err) {
    message.value = err.response?.data || "登入失敗";
  }
};
</script>
