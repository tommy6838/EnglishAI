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
import { ref, watch, nextTick } from "vue";
import axios from "axios";
import DictionaryService from "../services/DictionaryService";

const props = defineProps({
  word: String,
  visible: Boolean,
  voiceUri: String,
});
const emit = defineEmits(["close"]);

const wordData = ref({});
const loading = ref(false);

watch(
  () => props.word,
  async () => {
    if (!props.word) return;
    loading.value = true;
    wordData.value = {};
    try {
      const result = await DictionaryService.getWordData(props.word);
      if (result) wordData.value = result;
    } catch (err) {
      console.error("❌ Sidebar 查詢失敗:", err);
    } finally {
      loading.value = false;
      await nextTick();
      speak(props.word);
    }
  },
  { immediate: true }
);

function close() {
  emit("close");
}

function speak(text) {
  const utterance = new SpeechSynthesisUtterance(text);
  const voice = speechSynthesis
    .getVoices()
    .find((v) => v.voiceURI === props.voiceUri);
  if (voice) utterance.voice = voice;
  speechSynthesis.speak(utterance);
}

async function addToFavorite(word) {
  await axios.post("http://localhost:5153/api/favoriteWords", {
    userId: "d4badf61-5181-48ce-86cd-7a99ba604997",
    word,
  });
  alert(`✅ 已收藏 ${word}`);
}
</script>
