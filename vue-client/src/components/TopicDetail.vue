<template>
  <div>
    <h2>主題詳情頁</h2>
    <div v-if="topic">
      <strong>{{ topic.name }}</strong
      ><br />
      <span>{{ topic.description }}</span>
    </div>
    <div v-else>載入中...</div>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import axios from "axios";
import { useRoute } from "vue-router";

const route = useRoute();
const topic = ref(null);

onMounted(async () => {
  const id = route.params.id;
  const res = await axios.get(`http://localhost:5153/api/topics/${id}`);
  topic.value = res.data;
});
</script>
