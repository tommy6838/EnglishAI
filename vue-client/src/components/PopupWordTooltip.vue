<template>
  <div
    v-if="visible"
    class="fixed inset-0 z-50 flex items-center justify-center bg-black/30"
    @click.self="close"
  >
    <div
      ref="tooltipCard"
      class="bg-white rounded-lg shadow-lg p-4 w-80 max-w-full relative cursor-move"
      :style="tooltipStyle"
      @mousedown="startDrag"
    >
      <div v-if="wordData.word">
        <div class="flex justify-between items-center mb-2">
          <h3 class="text-lg font-bold text-blue-600">
            🔤 {{ wordData.word }}
          </h3>
          <button class="text-gray-400 hover:text-black text-lg" @click="close">
            ✕
          </button>
        </div>

        <div class="flex items-center gap-2 mb-1">
          <span class="text-sm text-white bg-indigo-500 px-2 py-0.5 rounded">
            {{ wordData.pos }}
          </span>
          <span class="text-sm text-gray-600 italic">/{{ wordData.ipa }}/</span>
          <button
            @click="speak(wordData.word)"
            class="text-blue-500 hover:text-blue-700 text-lg"
          >
            🔊
          </button>
        </div>

        <p class="text-sm text-gray-700 mb-2">{{ wordData.translation }}</p>
        <p class="text-sm text-gray-500 italic mb-4">
          📄 {{ wordData.example }}
        </p>

        <button
          @click.stop="addToFavorite(wordData.word)"
          class="bg-red-500 hover:bg-red-600 text-white text-sm px-3 py-1 rounded"
        >
          ❤️ 收藏
        </button>
      </div>

      <div v-else class="text-sm text-red-500 italic p-2">
        ❗ 找不到這個單字的資料：{{ props.word }}
      </div>
    </div>
  </div>
</template>

<script setup>
import { ref, watch, computed, onMounted, onUnmounted } from "vue";
import axios from "axios";

const props = defineProps({
  word: String,
  position: Object,
  visible: Boolean,
});
const emit = defineEmits(["close"]);

const tooltipCard = ref(null);
let offset = { x: 0, y: 0 };
let isDragging = false;

function startDrag(e) {
  if (!tooltipCard.value) return;
  isDragging = true;
  offset.x = e.clientX - tooltipCard.value.offsetLeft;
  offset.y = e.clientY - tooltipCard.value.offsetTop;
  document.addEventListener("mousemove", onDrag);
  document.addEventListener("mouseup", stopDrag);
}

function onDrag(e) {
  if (isDragging && tooltipCard.value) {
    tooltipCard.value.style.left = `${e.clientX - offset.x}px`;
    tooltipCard.value.style.top = `${e.clientY - offset.y}px`;
  }
}

function stopDrag() {
  isDragging = false;
  document.removeEventListener("mousemove", onDrag);
  document.removeEventListener("mouseup", stopDrag);
}

onUnmounted(() => stopDrag());

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
  () => {
    wordData.value = dictionary[props.word.toLowerCase()] || {
      word: props.word,
      pos: "unknown",
      translation: "（尚無資料）",
      example: "No example found.",
      ipa: "",
    };
  },
  { immediate: true }
);

const tooltipStyle = computed(() => {
  return props.position
    ? {
        top: `${props.position.y}px`,
        left: `${props.position.x}px`,
        position: "absolute",
      }
    : {};
});

function close() {
  emit("close");
}

function speak(text) {
  const utterance = new SpeechSynthesisUtterance(text);
  utterance.lang = "en-US";
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

<style scoped>
/* 防止滑動過程中選取文字 */
.cursor-move {
  user-select: none;
}
</style>
