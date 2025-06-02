/** @type {import('tailwindcss').Config} */
export default {
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}", // 加這行讓 Tailwind 能分析 Vue
  ],
  theme: {
    extend: {},
  },
  plugins: [],
};
