import { createApp } from "vue";
<<<<<<< HEAD
import { createPinia } from "pinia";
import App from "./App.vue";
import "./assets/main.css";
import i18n from "./locales/initialize";

const app = createApp(App);

app.use(createPinia());
app.use(i18n);

=======
//import { createPinia } from "pinia";
import App from "./App.vue";

import "./assets/main.css";

const app = createApp(App);

//app.use(createPinia());

>>>>>>> 3d122b55bd850f8abe910dcd6ff9497bba1b5a24
app.mount("#app");
