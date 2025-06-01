<template>
  <div class="conversation-page">
    <h2>我的對話紀錄</h2>
    <ul>
      <li v-for="c in conversations" :key="c.id" class="conversation-item">
        <strong>Q:</strong> {{ c.question }}<br />
        <strong>A:</strong>
        <span
          class="word-box"
          v-for="(word, idx) in extractWords(c.answer)"
          :key="idx"
          style="margin-right: 4px"
        >
          <span
            @click="addToHistory(word)"
            style="cursor: pointer; color: blue; user-select: none"
            title="點擊紀錄瀏覽"
          >
            {{ word }}
          </span>
          <button
            class="favorite-btn"
            @click="addToFavorite(word)"
            style="margin-left: 2px; font-size: 0.9em; padding: 0 5px"
          >
            收藏
          </button>
        </span>
      </li>
    </ul>

    <!-- 對話輸入區 -->
    <div class="input-area">
      <input
        v-model="newQuestion"
        placeholder="請輸入你的問題"
        @keyup.enter="sendQuestion"
      />
      <button @click="sendQuestion">送出</button>
    </div>
  </div>
</template>

<script setup>
import { ref, onMounted } from "vue";
import axios from "axios";

const conversations = ref([]);
const newQuestion = ref("");
const currentUserId = "d4badf61-5181-48ce-86cd-7a99ba604997";
const currentTopicId = 1;

onMounted(async () => {
  await loadConversations();
});

async function loadConversations() {
  const res = await axios.get(
    `http://localhost:5153/api/conversations?userId=${currentUserId}`
  );
  conversations.value = res.data;
}

async function sendQuestion() {
  if (!newQuestion.value.trim()) return;

  // 只傳 userId, topicId, question，由後端自動產生 answer（ChatGPT 回覆）
  await axios.post("http://localhost:5153/api/conversations", {
    userId: currentUserId,
    topicId: currentTopicId,
    question: newQuestion.value,
    // 不要加 answer
  });

  await loadConversations();
  newQuestion.value = "";
}

// 保持原本
function extractWords(answer) {
  return answer
    ? answer.match(/\b[a-zA-Z0-9]+(?:-[a-zA-Z0-9]+)*\b/g) || []
    : [];
}

const addToHistory = async (word) => {
  await axios.post("http://localhost:5153/api/wordHistory", {
    userId: currentUserId,
    word: word,
  });
};

const addToFavorite = async (word) => {
  await axios.post("http://localhost:5153/api/favoriteWords", {
    userId: currentUserId,
    word: word,
  });
};
</script>

<style scoped>
.word-box {
  position: relative;
  display: inline-block;
}

.favorite-btn {
  display: none; /* 預設隱藏 */
  background: #1c88e5;
  color: #fff;
  border: none;
  border-radius: 6px;
  cursor: pointer;
}

.word-box:hover .favorite-btn {
  display: inline-block; /* 滑鼠移上去才顯示 */
}
</style>

<style scoped>
.conversation-page {
  max-width: 600px;
  margin: 30px auto;
  background: #f9f9f9;
  padding: 24px;
  border-radius: 12px;
  box-shadow: 0 2px 8px #ddd;
}
.conversation-item {
  background: #fff;
  margin-bottom: 10px;
  padding: 10px;
  border-radius: 8px;
  border: 1px solid #eee;
}
.input-area {
  margin-top: 18px;
  display: flex;
  gap: 10px;
}
input[type="text"],
input {
  flex: 1;
  padding: 6px 12px;
  border-radius: 6px;
  border: 1px solid #ccc;
}
button {
  padding: 6px 18px;
  border-radius: 6px;
  border: none;
  background: #1c88e5;
  color: #fff;
  font-weight: bold;
  cursor: pointer;
  transition: background 0.2s;
}
button:hover {
  background: #155ca6;
}
</style>
