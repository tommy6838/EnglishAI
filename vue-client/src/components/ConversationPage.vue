<template>
  <div class="conversation-page min-h-screen bg-gray-100 p-4">
    <div class="max-w-2xl mx-auto bg-white rounded-2xl shadow-lg p-6">
      <!-- Header 區塊 -->
      <div class="flex justify-between items-center mb-6">
        <h1 class="text-2xl font-bold text-blue-600">AI 英文學習平台</h1>
        <div class="flex gap-2 items-center">
          <!-- 模式切換 -->
          <select
            v-model="viewMode"
            class="border border-gray-300 rounded px-2 py-1 text-sm focus:ring focus:border-blue-400"
          >
            <option value="tooltip">Tooltip 模式</option>
            <option value="sidebar">Sidebar 模式</option>
          </select>
          <!-- 語音選擇 -->
          <select
            v-model="selectedVoiceURI"
            class="border border-gray-300 rounded px-2 py-1 text-sm focus:ring focus:border-blue-400"
          >
            <option
              v-for="voice in voices"
              :key="voice.voiceURI"
              :value="voice.voiceURI"
            >
              {{ voice.name }} ({{ voice.lang }})
            </option>
          </select>
        </div>
      </div>

      <h2 class="text-lg font-semibold mb-4 text-gray-700">我的對話紀錄</h2>

      <!-- 對話區（修正為雙氣泡顯示） -->
      <div
        ref="scrollContainer"
        class="h-[300px] overflow-y-auto space-y-4 border border-gray-200 p-4 rounded-lg bg-gray-50"
      >
        <div v-for="c in conversations" :key="c.id" class="space-y-2">
          <!-- 使用者提問氣泡 -->
          <div class="flex justify-end">
            <div
              class="bg-blue-500 text-white px-4 py-2 rounded-xl max-w-[70%] shadow"
            >
              <div class="text-xs text-blue-100 text-right mb-1">
                {{ formatTimestamp(c.createdAt) }}
              </div>
              <strong class="text-blue-100">你：</strong>
              {{ c.question }}
            </div>
          </div>

          <!-- AI 回答氣泡 -->
          <div class="flex justify-start">
            <div
              class="bg-white text-gray-800 px-4 py-2 rounded-xl max-w-[70%] shadow border border-gray-200"
            >
              <div class="text-xs text-gray-400 mb-1">
                {{ formatTimestamp(c.createdAt) }}
              </div>
              <strong class="text-green-600">AI：</strong>
              <span
                class="inline-block mr-2"
                v-for="(word, idx) in extractWords(
                  displayedAnswers[c.id] || ''
                )"
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
              <!-- 朗讀按鈕 -->
              <button
                @click="playSpeech(displayedAnswers[c.id])"
                class="ml-2 text-blue-500 hover:text-blue-700"
                title="朗讀此句"
              >
                🔊
              </button>
            </div>
          </div>
        </div>
      </div>

      <!-- 輸入區塊 -->
      <div class="mt-6 flex gap-2">
        <input
          v-model="newQuestion"
          placeholder="請輸入你的問題"
          @keyup.enter="sendQuestion"
          class="flex-1 p-2 border border-gray-300 rounded focus:outline-none focus:ring focus:border-blue-400"
        />
        <button
          @click="sendQuestion"
          class="bg-blue-500 text-white px-4 py-2 rounded hover:bg-blue-600 transition shadow"
        >
          送出
        </button>
      </div>
    </div>

    <!-- Tooltip 模式 -->
    <PopupWordTooltip
      v-if="viewMode === 'tooltip' && tooltipVisible"
      :word="selectedWord"
      :position="tooltipPosition"
      :visible="tooltipVisible"
      :voice-uri="selectedVoiceURI"
      @close="closeTooltip"
    />

    <!-- Sidebar 模式 -->
    <WordDetailSidebar
      v-if="viewMode === 'sidebar' && sidebarVisible"
      :word="selectedWord"
      :visible="sidebarVisible"
      :voice-uri="selectedVoiceURI"
      @close="closeSidebar"
    />
  </div>
</template>

<script setup>
import { ref, onMounted, nextTick } from "vue";
import api from "../utils/axios";
import PopupWordTooltip from "./PopupWordTooltip.vue";
import WordDetailSidebar from "./WordDetailSidebar.vue";
import { useRouter } from "vue-router";

const router = useRouter();
const token = localStorage.getItem("token");
if (!token) {
  router.push("/AuthPage");
}

const conversations = ref([]);
const displayedAnswers = ref({});
const newQuestion = ref("");
const currentTopicId = 1;
const scrollContainer = ref(null);

const tooltipVisible = ref(false);
const sidebarVisible = ref(false);
const selectedWord = ref("");
const tooltipPosition = ref({ x: 0, y: 0 });
const viewMode = ref(localStorage.getItem("viewMode") || "tooltip");
const voices = ref([]);
const selectedVoiceURI = ref("");

// 提供全域朗讀函式給其他元件使用
function playSpeech(text) {
  if (!text) return;

  // 保險載入語音
  if (!voices.value.length) {
    voices.value = speechSynthesis.getVoices();
  }

  const voice = voices.value.find((v) => v.voiceURI === selectedVoiceURI.value);
  const utter = new SpeechSynthesisUtterance(text);
  if (voice) utter.voice = voice;

  // 解決 Chrome 無聲問題
  if (speechSynthesis.speaking || speechSynthesis.pending) {
    speechSynthesis.cancel();
  }

  setTimeout(() => {
    speechSynthesis.speak(utter);
  }, 100);
}

defineExpose({ playSpeech });

onMounted(async () => {
  loadVoices();
  speechSynthesis.onvoiceschanged = loadVoices;

  await loadConversations();
  await nextTick();
  scrollContainer.value.scrollTop = scrollContainer.value.scrollHeight;
});

function loadVoices() {
  const loadedVoices = speechSynthesis.getVoices();
  if (loadedVoices.length > 0) {
    voices.value = loadedVoices;
    if (!selectedVoiceURI.value) {
      selectedVoiceURI.value =
        loadedVoices.find((v) => v.lang.startsWith("en"))?.voiceURI ||
        loadedVoices[0].voiceURI;
    }
  }
}

function formatTimestamp(ts) {
  return ts ? new Date(ts).toLocaleString() : "";
}

async function loadConversations() {
  const res = await api.get("/conversations");
  conversations.value = res.data;
  for (const c of res.data) {
    displayedAnswers.value[c.id] = "";
    typeAnswerEffect(c.id, c.answer);
    playSpeech(c.answer);
  }
}

async function sendQuestion() {
  if (!newQuestion.value.trim()) return;
  try {
    await api.post("/conversations", {
      TopicId: currentTopicId,
      Question: newQuestion.value.trim(),
    });
    await loadConversations();
    newQuestion.value = "";
    await nextTick();
    scrollContainer.value.scrollTop = scrollContainer.value.scrollHeight;
  } catch (err) {
    const serverError = err.response?.data;
    if (serverError?.errors) {
      const messages = Object.values(serverError.errors).flat().join("\n");
      alert("❗ 發送失敗：\n" + messages);
    } else {
      alert("❗ 發送失敗：" + (serverError?.title || err.message));
    }
  }
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

function typeAnswerEffect(id, fullText) {
  let i = 0;
  const interval = setInterval(() => {
    displayedAnswers.value[id] = fullText.slice(0, i + 1);
    i++;
    if (i >= fullText.length) clearInterval(interval);
  }, 15);
}
</script>
