<template>
  <div>
    <h2>選擇你的學習主題</h2>
    <ul>
      <!-- 用 v-for 把主題列表 topics 一筆一筆印出來 -->
      <li v-for="topic in topics" :key="topic.id">
        <router-link :to="`/topics/${topic.id}`">
          <strong>{{ topic.name }}</strong>
        </router-link>
        <br />
        <span>{{ topic.description }}</span>
      </li>
    </ul>
    <!-- 載入中訊息 -->
    <div v-if="loading">載入中...</div>
    <!-- 錯誤訊息 -->
    <div v-if="error" style="color: red">{{ error }}</div>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import axios from "axios";

const topics = ref([]); // 存放 API 回傳的主題清單
const loading = ref(false); // 載入狀態
const error = ref(""); // 錯誤訊息

onMounted(async () => {
  loading.value = true;
  try {
    // 改成你的 API 網址（port 請和你 ASP.NET Core 專案相符）
    const res = await axios.get("http://localhost:5153/api/topics");
    topics.value = res.data;
  } catch (err) {
    error.value = "主題載入失敗，請檢查伺服器狀態";
  } finally {
    loading.value = false;
  }
});
</script>
