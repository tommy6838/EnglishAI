<template>
  <div
      class="min-h-screen pt-20 px-4 bg-cover bg-center text-white flex flex-col items-center justify-center"
    style="background-image: url('https://images.unsplash.com/photo-1503676260728-1c00da094a0b');"
  >
    <!-- LOGO + 標語 -->
    <div class="text-center animate__animated animate__fadeInDown">
      <img
        src="https://cdn-icons-png.flaticon.com/512/3135/3135755.png"
        alt="Logo"
        class="w-24 h-24 mx-auto mb-4 rounded-full shadow-md bg-white p-2"
      />
      <h1 class="text-4xl md:text-5xl font-bold mb-2">
        {{ lang === 'en' ? 'Welcome to English AI' : '歡迎使用 English AI' }}
      </h1>
      <p class="text-lg text-gray-100">
        {{ lang === 'en' ? 'Your smart English learning assistant' : '您的智慧英文學習助理' }}
      </p>
    </div>

    <!-- CTA 按鈕 -->
    <div class="mt-10 animate__animated animate__fadeInUp animate__delay-1s">
      <button
        @click="navigateToLogin"
        class="bg-blue-500 hover:bg-blue-600 text-white px-6 py-3 rounded-full shadow-lg text-lg transition duration-300"
      >
        🚀 {{ lang === 'en' ? 'Start Learning' : '開始學習' }}
      </button>
    </div>

    <!-- 登入前介紹文字區塊 -->
    <div class="mt-20 max-w-3xl text-center animate__animated animate__fadeInUp animate__delay-2s">
      <h2 class="text-2xl font-semibold mb-4">
        {{ lang === 'en' ? 'Why choose us?' : '為什麼選擇我們？' }}
      </h2>
      <p class="text-md text-gray-200">
        {{ lang === 'en'
          ? 'We provide personalized learning powered by AI, helping you speak and write English with confidence.'
          : '我們透過 AI 提供個人化學習，幫助您自信地說與寫英文。' }}
      </p>
    </div>
  </div>
</template>

<script setup>
import { useRouter } from 'vue-router'
import { inject, ref, computed } from 'vue'

const router = useRouter()

const injectedLang = inject('language', ref('en'))
const lang = computed(() => injectedLang.value)

function navigateToLogin() {
  router.push('/login')
}
</script>

<style scoped>
@import 'https://cdnjs.cloudflare.com/ajax/libs/animate.css/4.1.1/animate.min.css';
</style>
