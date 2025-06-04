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
        <!-- 單字 -->
        <h2 class="text-xl font-bold text-blue-600 mb-2">
          🔤 {{ wordData.word }}
        </h2>

        <!-- 詞性與音標 -->
        <p class="text-sm text-purple-600 font-semibold mb-1">
          {{ wordData.partOfSpeech }}
          <span class="text-gray-500 italic"> {{ wordData.phonetic }} </span>
          <button
            @click="speak(wordData.word)"
            class="ml-2 text-blue-500 hover:text-blue-700"
          >
            🔊
          </button>
        </p>

        <!-- 中文翻譯 -->
        <p class="text-sm text-green-600 font-medium mb-2">
          🌏 中文翻譯：{{ wordData.translation }}
        </p>

        <!-- 英文定義 -->
        <!-- 📄 英文定義 + 語意分類 + Tooltip -->
        <p v-if="wordData.definition" class="text-sm text-gray-700 mb-1">
          📄
          <span class="text-yellow-600 font-semibold">[其他解釋]</span>
          {{ wordData.definition }}
        </p>

        <!-- 英文例句與中文翻譯 -->
        <p v-if="wordData.example" class="text-sm text-gray-600 italic mb-4">
          📘 {{ wordData.example }}
          <span v-if="wordData.exampleZh" class="text-gray-500"
            >（{{ wordData.exampleZh }}）</span
          >
          <button
            @click="speak(wordData.example)"
            class="ml-2 text-blue-500 hover:text-blue-700"
          >
            🗣️
          </button>
        </p>

        <!-- 收藏按鈕 -->
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
import { ref, watch, onMounted, nextTick, defineProps, defineEmits } from "vue";
import api from "../utils/axios";
import PopupWordTooltip from "./PopupWordTooltip.vue";
import WordDetailSidebar from "./WordDetailSidebar.vue";
import { useRouter } from "vue-router";
import { wordCache } from "../utils/wordCache"; // ✅ 改為引用全域快取
import DictionaryService from "../services/DictionaryService";
import { computed } from "vue";

const props = defineProps({
  visible: Boolean,
  word: String,
  voiceUri: String,
});
const emit = defineEmits(["close"]);

const wordData = ref({});
const loading = ref(false);

watch(
  () => props.word,
  async (newWord) => {
    if (!newWord) return;
    loading.value = true;
    wordData.value = {};

    const lowerWord = newWord.toLowerCase();

    try {
      // 先從資料庫查
      const dbRes = await api.get(
        `/WordDictionary/BulkCheck?words=${lowerWord}`
      );

      if (dbRes.data.length > 0) {
        const raw = dbRes.data[0];

        // 如果 definition 或 example 為 null，要補抓 API
        const needUpdate = !raw.definition || !raw.example;

        if (needUpdate) {
          const apiRes = await DictionaryService.getWordData(lowerWord);

          // 用 API 補齊空值
          const updated = {
            word: lowerWord,
            translation: raw.translation || apiRes.translation || "",
            example: raw.example || apiRes.example || "",
            definition: raw.definition || apiRes.definition || "",
            partOfSpeech: raw.partOfSpeech || apiRes.partOfSpeech || "",
            phonetic: raw.phonetic || apiRes.phonetic || "",
            exampleZh: raw.exampleZh || apiRes.exampleZh || "",
          };

          // 自動更新到資料庫
          await api.post("/WordDictionary/BulkInsert", [updated]);
          wordData.value = updated;
        } else {
          wordData.value = raw;
        }
      } else {
        // 若資料庫查不到，直接查 API
        const result = await DictionaryService.getWordData(lowerWord);
        wordData.value = result || { word: newWord };

        // 存入資料庫
        await api.post("/WordDictionary/BulkInsert", [
          {
            word: result.word,
            translation: result.translation || "",
            example: result.example || "",
            definition: result.definition || "",
            partOfSpeech: result.partOfSpeech || "",
            phonetic: result.phonetic || "",
            exampleZh: result.exampleZh || "",
          },
        ]);
      }
    } catch (err) {
      console.error("❌ WordDetailSidebar 查詢失敗:", err);
      wordData.value = { word: newWord }; // 保底填入
    } finally {
      loading.value = false;
    }
  },
  { immediate: true }
);

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

// 判斷語意分類

// 判斷是否為常見第一解釋（例如是否和字面意思相關）

// 提供 tooltip 顯示說明
const meaningTagTooltip = computed(() => {
  switch (meaningTag.value) {
    case "體育術語":
      return "這是用在撞球、保齡球等運動中的『旋轉』技巧";
    case "語言":
      return "這是英文語言本身的意思";
    case "教學用語":
      return "這是指『例子』、用來說明或教學";
    default:
      return "";
  }
});

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
  if (scrollContainer.value) {
    scrollContainer.value.scrollTop = scrollContainer.value.scrollHeight;
  }
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
    await nextTick(); // ✅ 等待 DOM 更新完成
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
