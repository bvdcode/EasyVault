import i18n from "i18next";
import { en, ru } from "./locales";
import { initReactI18next } from "react-i18next";
import LanguageDetector from "i18next-browser-languagedetector";

i18n
  .use(LanguageDetector)
  .use(initReactI18next)
  .init({
    resources: {
      en: { translation: en },
      ru: { translation: ru },
    },
    fallbackLng: {
      "ru-RU": ["ru"],
      "en-US": ["en"],
      "en-GB": ["en"],
      "ru-UA": ["ru"],
      "ru-BY": ["ru"],
      default: ["en"],
    },
    debug: false,
    interpolation: {
      escapeValue: false,
    },
  });

export default i18n;
