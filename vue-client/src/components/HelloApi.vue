<template>
  <div>
    <button @click="callApi">點我測試後端</button>
    <p>{{ message }}</p>
  </div>

  <div>
    <h1>Hello Api 頁面</h1>
    <router-link to="/AuthPage">
      <button>前往註冊頁面</button>
    </router-link>
  </div>
</template>

<script setup>
import { ref } from "vue";
import axios from "axios";

const message = ref("");

const callApi = async () => {
  // 請將下面的 port 改成你 ASP.NET Core 的實際 port
  const res = await axios.get("http://localhost:5153/api/users");
  message.value = res.data;
};
</script>
