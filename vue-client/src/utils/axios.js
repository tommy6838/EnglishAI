// src/utils/axios.js
import axios from "axios";

const instance = axios.create({
  baseURL: "http://localhost:5153/api", // ⚠️ 請依你的 Web API 實際埠號設定
});

// ✅ 自動加上 Authorization header
instance.interceptors.request.use((config) => {
  const token = localStorage.getItem("token"); // ← 這裡抓你剛剛看到的 token
  if (token) {
    config.headers.Authorization = `Bearer ${token}`; // ← ⚠️ 一定要 Bearer + 空格！
  }
  return config;
});

export default instance;
