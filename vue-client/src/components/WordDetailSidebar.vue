<template>
  <div v-if="visible">
    <!-- 右側滑出面板 -->
    <div
      class="fixed right-0 top-0 h-full w-80 bg-white shadow-lg p-4 flex flex-col z-40"
    >
      <div class="flex justify-between items-center mb-4">
        <h2 class="text-xl font-bold text-blue-600">🔤 {{ wordData.word }}</h2>
        <button @click="close" class="text-gray-400 hover:text-black text-lg">
          ✕
        </button>
      </div>

      <div v-if="loading" class="text-sm text-gray-500 text-center">
        ⏳ 查詢中...
      </div>

      <div v-else-if="wordData.word">
        <p class="text-sm text-gray-600 italic mb-1">
          ({{ wordData.partOfSpeech }})
          <span class="text-gray-500">/{{ wordData.phonetic }}/</span>
          <button
            @click="speak(wordData.word)"
            class="ml-2 text-blue-500 hover:text-blue-700"
          >
            🔊
          </button>
        </p>
        <p class="text-sm text-gray-800 mb-2">
          🌏 中文翻譯：{{ wordData.translation }}
        </p>
        <p class="text-sm text-gray-500 italic mb-1">
          📄 {{ wordData.definition }}
        </p>
        <p class="text-sm text-gray-500 italic mb-4">
          📘 {{ wordData.example
          }}<span v-if="wordData.exampleZh">（{{ wordData.exampleZh }}）</span>
          <button
            @click="speak(wordData.example)"
            class="ml-2 text-blue-500 hover:text-blue-700"
          >
            🗣️
          </button>
        </p>
        <button
          @click.stop="addToFavorite(wordData.word)"
          class="bg-red-500 hover:bg-red-600 text-white text-sm px-3 py-1 rounded"
        >
          ❤️ 收藏
        </button>
      </div>

      <div v-else class="text-sm text-red-500 italic">
        ❗ 找不到這個單字的資料：{{ word }}
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, watch, onMounted, defineProps, defineEmits } from "vue";
import api from "../utils/axios";
import PopupWordTooltip from "./PopupWordTooltip.vue";
import WordDetailSidebar from "./WordDetailSidebar.vue";
import { useRouter } from "vue-router";
import { wordCache } from "../utils/wordCache"; // ✅ 改為引用全域快取

const props = defineProps({
  visible: Boolean,
  word: String,
  voiceUri: String,
});
const emit = defineEmits(["close"]);

const wordData = ref(null);
const loading = ref(false);

watch(
  () => props.word,
  async (newWord) => {
    if (!newWord) return;
    loading.value = true;
    wordData.value = null;
    const result = await DictionaryService.getWordData(newWord);
    wordData.value = result;
    loading.value = false;
  },
  { immediate: true }
);

function speak(text) {
  if (!text) return;
  const synth = window.speechSynthesis;
  const utter = new SpeechSynthesisUtterance(text);
  const voice = synth.getVoices().find((v) => v.voiceURI === props.voiceUri);
  if (voice) utter.voice = voice;
  synth.cancel();
  setTimeout(() => synth.speak(utter), 100);
}

async function addToFavorite(word) {
  try {
    await api.post("/FavoriteWords", { word });
    alert(`❤️ 已加入收藏：${word}`);
  } catch (err) {
    console.error("加入收藏失敗", err);
    alert("⚠️ 加入收藏失敗");
  }
}

function close() {
  emit("close");
}

function toQueryString(params) {
  return Object.entries(params)
    .map(([key, val]) =>
      Array.isArray(val)
        ? val
            .map((v) => `${encodeURIComponent(key)}=${encodeURIComponent(v)}`)
            .join("&")
        : `${encodeURIComponent(key)}=${encodeURIComponent(val)}`
    )
    .join("&");
}

const router = useRouter();
const token = localStorage.getItem("token");
const userId = localStorage.getItem("userId");
if (!token) router.push("/AuthPage");

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

function playSpeech(text) {
  if (!text) return;
  if (!voices.value.length) voices.value = speechSynthesis.getVoices();
  const voice = voices.value.find((v) => v.voiceURI === selectedVoiceURI.value);
  const utter = new SpeechSynthesisUtterance(text);
  if (voice) utter.voice = voice;
  if (speechSynthesis.speaking || speechSynthesis.pending)
    speechSynthesis.cancel();
  setTimeout(() => speechSynthesis.speak(utter), 100);
}

defineExpose({ playSpeech });

onMounted(async () => {
  loadVoices();
  speechSynthesis.onvoiceschanged = loadVoices;
  await preloadWordCache();
  await loadConversations();
  await nextTick();
  scrollContainer.value.scrollTop = scrollContainer.value.scrollHeight;
});

async function preloadWordCache() {
  if (!userId) return;
  try {
    const res = await api.get(`/WordCache/PreloadWords/${userId}`);
    const preloadList = res.data;
    if (!preloadList.length) return;

    const checkRes = await api.get(
      `/WordDictionary/BulkCheck?${toQueryString({ words: preloadList })}`
    );
    const existingWords = checkRes.data.map((w) => w.word.toLowerCase());

    for (const entry of checkRes.data) {
      wordCache.set(entry.word.toLowerCase(), entry);
    }

    const toQuery = preloadList.filter(
      (word) => !existingWords.includes(word.toLowerCase())
    );
    const newlyFetched = [];

    for (const word of toQuery) {
      try {
        const dictRes = await api.get(`/dictionary?word=${word}`);
        const entry = {
          word: word,
          translation: dictRes.data.translation || "",
          example: dictRes.data.example || "",
        };
        wordCache.set(word.toLowerCase(), entry);
        newlyFetched.push(entry);
      } catch (e) {
        console.warn("❌ 字典 API 查詢失敗：", word);
      }
    }

    if (newlyFetched.length > 0) {
      await api.post("/WordDictionary/BulkInsert", newlyFetched);
    }
  } catch (err) {
    console.error("🚫 預查單字失敗：", err);
  }
}

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
    if (scrollContainer.value) {
      scrollContainer.value.scrollTop = scrollContainer.value.scrollHeight;
    }
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

function getSegments(text) {
  return text.match(/\b[\w'-]+\b|\s+|[^\w\s]/g) || [];
}

function isWord(segment) {
  return /^[\w'-]+$/.test(segment);
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
