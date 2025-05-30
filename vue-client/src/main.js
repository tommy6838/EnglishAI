// src/main.js

import { createApp } from "vue";
import App from "./App.vue";
import axios from "axios";
import router from "./router"; // ← 新增這一行

createApp(App)
  .use(router) // ← 加這一行
  .mount("#app");

// ======= 這段是全域攔截器，一次設定全部API都有效 =======
axios.interceptors.request.use(
  (config) => {
    const token = localStorage.getItem("token");
    if (token) {
      config.headers.Authorization = `Bearer ${token}`;
    }
    return config;
  },
  (error) => Promise.reject(error)
);
// ================================================
