import { createI18n } from "vue-i18n";
import zhCN from "./zh-CN.json";
import enUS from "./en-US.json";

type MessageSchema = typeof zhCN;
const i18n = createI18n<[MessageSchema], "zh-CN" | "en-US">({
  locale: window.navigator.language,
  fallbackLocale: "zh-CN",
  allowComposition: true,
  messages: {
    "zh-CN": zhCN,
    "en-US": enUS,
  },
});

export default i18n;
