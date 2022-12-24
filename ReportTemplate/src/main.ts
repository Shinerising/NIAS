import { createApp } from "vue";
import { createPinia } from "pinia";
import App from "./App.vue";
import "./assets/main.css";
import i18n from "./locales/initialize";

const app = createApp(App);

app.use(createPinia());
app.use(i18n);

app.mount("#app");
