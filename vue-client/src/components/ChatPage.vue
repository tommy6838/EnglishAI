<template>
  <div class="chat-container">
    <div class="messages">
      <div
        v-for="(msg, idx) in messages"
        :key="idx"
        :class="['message', msg.role]"
      >
        <strong v-if="msg.role === 'user'">你：</strong>
        <strong v-else>AI：</strong>
        {{ msg.text }}
      </div>
    </div>
    <div class="input-row">
      <input
        v-model="input"
        @keyup.enter="sendMessage"
        placeholder="請輸入訊息..."
      />
      <button @click="sendMessage">送出</button>
    </div>
  </div>
</template>

<script setup>
import { ref } from "vue";

// 訊息陣列
const messages = ref([]);
// 使用者輸入
const input = ref("");

// 送出訊息並產生假AI回覆
function sendMessage() {
  const text = input.value.trim();
  if (!text) return;

  // 推入使用者訊息
  messages.value.push({ role: "user", text });

  // 假AI邏輯
  let aiReply = "";
  if (text.toLowerCase() === "hello") {
    aiReply = "Hi, how are you?";
  } else if (text.toLowerCase() === "how are you?") {
    aiReply = "I'm fine, thank you!";
  } else {
    aiReply = '很抱歉，我暫時只聽得懂 "Hello" 和 "How are you?"';
  }

  // 推入AI訊息
  messages.value.push({ role: "ai", text: aiReply });

  // 清空輸入框
  input.value = "";
}
</script>

<style scoped>
.chat-container {
  max-width: 500px;
  margin: 50px auto;
  border: 1px solid #eee;
  border-radius: 10px;
  padding: 20px;
  background: #fafaff;
}
.messages {
  min-height: 200px;
  margin-bottom: 10px;
}
.message {
  margin: 6px 0;
}
.message.user {
  text-align: right;
  color: #2196f3;
}
.message.ai {
  text-align: left;
  color: #4caf50;
}
.input-row {
  display: flex;
  gap: 8px;
}
input {
  flex: 1;
  padding: 8px;
}
button {
  padding: 8px 14px;
}
</style>
