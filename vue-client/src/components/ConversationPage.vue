<template>
  <div class="conversation-page min-h-screen bg-gray-100 p-4">
    <div class="max-w-2xl mx-auto bg-white rounded-xl shadow p-6">
      <div class="flex justify-between items-center mb-4">
        <h1 class="text-2xl font-bold text-blue-500">Tailwind 成功載入！</h1>
        <select
          v-model="viewMode"
          class="border border-gray-300 rounded px-2 py-1 text-sm"
        >
          <option value="tooltip">Tooltip 模式</option>
          <option value="sidebar">Sidebar 模式</option>
        </select>
      </div>

      <h2 class="text-lg font-semibold mb-4 text-gray-700">我的對話紀錄</h2>

      <div
        ref="scrollContainer"
        class="h-[300px] overflow-y-auto space-y-4 border border-gray-200 p-4 rounded-lg bg-gray-50"
      >
        <div
          v-for="c in conversations"
          :key="c.id"
          class="bg-white shadow-sm rounded-lg p-4 border border-gray-200"
        >
          <p><strong class="text-blue-600">Q:</strong> {{ c.question }}</p>
          <p class="mt-1">
            <strong class="text-green-600">A:</strong>
            <span
              class="inline-block mr-2"
              v-for="(word, idx) in extractWords(c.answer)"
              :key="idx"
            >
              <span
                @click="handleWordClick(word, $event)"
                class="text-blue-500 hover:underline cursor-pointer"
                title="點擊查看解釋"
              >
                {{ word }}
              </span>
            </span>
          </p>
        </div>
      </div>

      <div class="mt-6 flex gap-2">
        <input
          v-model="newQuestion"
          placeholder="請輸入你的問題"
          @keyup.enter="sendQuestion"
          class="flex-1 p-2 border border-gray-300 rounded focus:outline-none focus:ring focus:border-blue-400"
        />
        <button
          @click="sendQuestion"
          class="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600 transition"
        >
          送出
        </button>
      </div>
    </div>

    <!-- Tooltip 卡片 -->
    <PopupWordTooltip
      v-if="viewMode === 'tooltip' && tooltipVisible"
      :word="selectedWord"
      :position="tooltipPosition"
      :visible="tooltipVisible"
      @close="closeTooltip"
    />

    <!-- Sidebar 面板 -->
    <WordDetailSidebar
      v-if="viewMode === 'sidebar' && sidebarVisible"
      :word="selectedWord"
      :visible="sidebarVisible"
      @close="closeSidebar"
    />
  </div>
</template>

<script setup>
import { ref, onMounted, watch, nextTick } from "vue";
import axios from "axios";
import PopupWordTooltip from "./PopupWordTooltip.vue";
import WordDetailSidebar from "./WordDetailSidebar.vue";

const conversations = ref([]);
const newQuestion = ref("");
const currentUserId = "d4badf61-5181-48ce-86cd-7a99ba604997";
const currentTopicId = 1;
const scrollContainer = ref(null);

const tooltipVisible = ref(false);
const sidebarVisible = ref(false);
const selectedWord = ref("");
const tooltipPosition = ref({ x: 0, y: 0 });
const viewMode = ref(localStorage.getItem("viewMode") || "tooltip");

watch(viewMode, (newVal) => {
  localStorage.setItem("viewMode", newVal);
});

onMounted(async () => {
  await loadConversations();
  await nextTick();
  scrollContainer.value.scrollTop = scrollContainer.value.scrollHeight;
});

async function loadConversations() {
  const res = await axios.get(
    `http://localhost:5153/api/conversations?userId=${currentUserId}`
  );
  conversations.value = res.data;
}

async function sendQuestion() {
  if (!newQuestion.value.trim()) return;

  await axios.post("http://localhost:5153/api/conversations", {
    userId: currentUserId,
    topicId: currentTopicId,
    question: newQuestion.value,
  });

  await loadConversations();
  newQuestion.value = "";
  await nextTick();
  scrollContainer.value.scrollTop = scrollContainer.value.scrollHeight;
}

function extractWords(answer) {
  return answer
    ? answer.match(/\b[a-zA-Z0-9]+(?:-[a-zA-Z0-9]+)*\b/g) || []
    : [];
}

function handleWordClick(word, event) {
  selectedWord.value = word;

  if (viewMode.value === "tooltip") {
    const rect = event.target.getBoundingClientRect();
    tooltipPosition.value = {
      x: rect.left + rect.width / 2,
      y: rect.bottom + window.scrollY + 10,
    };
    tooltipVisible.value = true;
  } else {
    sidebarVisible.value = true;
  }
}

function closeTooltip() {
  tooltipVisible.value = false;
  selectedWord.value = "";
}

function closeSidebar() {
  sidebarVisible.value = false;
  selectedWord.value = "";
}
</script>

<style scoped></style>
