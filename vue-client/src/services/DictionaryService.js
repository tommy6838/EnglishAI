// services/DictionaryService.js
// 📦 統一的字典服務：封裝所有 API 呼叫與資料轉換邏輯

import axios from "axios";

const DictionaryService = {
  /**
   * 🔍 查詢英文定義（使用 dictionaryapi.dev）
   */
  async getEnglishDefinition(word) {
    try {
      const url = `https://api.dictionaryapi.dev/api/v2/entries/en/${word}`;
      const res = await axios.get(url);
      const data = res.data[0];

      return {
        word: data.word,
        phonetic: data.phonetics?.[0]?.text || "",
        audio: data.phonetics?.[0]?.audio || "",
        partOfSpeech: data.meanings?.[0]?.partOfSpeech || "",
        definition: data.meanings?.[0]?.definitions?.[0]?.definition || "",
        example: data.meanings?.[0]?.definitions?.[0]?.example || "",
        translation: null,
        loading: false,
      };
    } catch (err) {
      console.error("❌ 查詢英文定義失敗:", err);
      return null;
    }
  },

  /**
   * 🌏 查詢中文翻譯與中英例句（透過後端 Glosbe Proxy），若無則用 Lingva Translate 備援
   */
  async getChineseTranslation(word) {
    try {
      const url = `http://localhost:5153/api/glosbeapi/translate?word=${word}`;
      const res = await axios.get(url);

      console.log("🌏 Glosbe 回傳資料：", res.data);

      const translations = [];
      if (Array.isArray(res.data?.tuc)) {
        for (const item of res.data.tuc) {
          if (item.phrase?.text) {
            translations.push(item.phrase.text);
          } else if (item.meanings?.length) {
            const zhMeaning = item.meanings.find((m) =>
              ["zh", "cmn"].includes(m.language)
            );
            if (zhMeaning?.text) translations.push(zhMeaning.text);
          }
        }
      }

      const examples = res.data?.examples;
      let translation = translations?.[0] || "";
      let exampleTranslation = examples?.[0]?.second || "";

      // 🛡 若 Glosbe 無結果，使用 Lingva 做備援（不再做繁體轉換）
      if (!translation) {
        console.log("🌐 Glosbe 無翻譯，使用 Lingva 備援...");
        const lingvaRes = await axios.get(
          `http://localhost:5153/api/lingvaapi/translate?word=${word}`
        );
        translation = lingvaRes.data?.translation || "";
      }

      return {
        translation,
        example: examples?.[0]?.first || "",
        exampleTranslation,
      };
    } catch (err) {
      console.error("❌ 查詢翻譯失敗:", err);
      return {
        translation: "",
        example: "",
        exampleTranslation: "",
      };
    }
  },

  /**
   * 📦 綜合查詢：英文定義 + 中文翻譯
   */
  async getWordData(word) {
    try {
      const url = `http://localhost:5153/api/WordDictionary/Ensure?word=${word}`;
      const res = await axios.get(url);
      const data = res.data; // ✅ 你之前少了這行

      return {
        word: data.word,
        phonetic: data.phonetic || "",
        audio: "", // optional
        partOfSpeech: data.partOfSpeech || "",
        definition: data.definition || "",
        translation: data.translation || "",
        example: data.example || "",
        exampleZh: data.exampleZh || "",
      };
    } catch (err) {
      console.error("❌ 從 WordDictionary 查詢失敗:", err);
      return null;
    }
  },
};

export default DictionaryService;
