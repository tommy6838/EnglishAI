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

      <div v-if="wordData.word">
        <p class="text-sm text-gray-600 italic mb-1">
          ({{ wordData.pos }})
          <span class="text-gray-500">/{{ wordData.ipa }}/</span>
          <button
            @click="speak(wordData.word)"
            class="ml-2 text-blue-500 hover:text-blue-700"
          >
            🔊
          </button>
        </p>
        <p class="text-sm text-gray-800 mb-2">📘 {{ wordData.translation }}</p>
        <p class="text-sm text-gray-500 italic mb-4">
          📄 {{ wordData.example }}
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

const props = defineProps({
  word: String,
  visible: Boolean,
  voiceUri: String,
});
const emit = defineEmits(["close"]);

const dictionary = {
  hello: {
    word: "hello",
    pos: "interjection",
    translation: "你好；哈囉",
    example: "Hello there, Katie!",
    ipa: "həˈlō",
  },
  assist: {
    word: "assist",
    pos: "verb",
    translation: "協助、幫助",
    example: "He assisted the old man across the street.",
    ipa: "əˈsist",
  },
};

const wordData = ref({});
watch(
  () => props.word,
  async () => {
    wordData.value = dictionary[props.word?.toLowerCase()] || {
      word: props.word,
      pos: "unknown",
      translation: "（尚無資料）",
      example: "No example found.",
      ipa: "",
    };
    await nextTick();
    // 自動朗讀單字（進入時）
    speak(wordData.value.word);
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
