<script setup lang="ts">
import { ref } from "vue";
import type { NetworkData } from "./interface/NetworkData.interface";
import IconRefresh from "./icons/IconRefresh.vue";
import IconPrint from "./icons/IconPrint.vue";
import IconFullsize from "./icons/IconFullsize.vue";
import IconNormalsize from "./icons/IconNormalsize.vue";

defineProps<{
  data: NetworkData;
}>();

const refreshPage = () => {
  document.location.reload();
};

const ifFullWidth = ref(false);

const delay = (ms: number) => new Promise((res) => setTimeout(res, ms));

const togglePageWidth = () => {
  const element = document.getElementById("app");
  if (!element) {
    return;
  }
  if (element.style.maxWidth === "100vw") {
    element.style.maxWidth = "";
    ifFullWidth.value = false;
  } else {
    element.style.maxWidth = "100vw";
    ifFullWidth.value = true;
  }
};

const printPage = async () => {
  [].forEach.call(
    document.querySelectorAll(".chart-wrapper"),
    (element: HTMLElement) => {
      element.style.width = "896px";
    }
  );
  await delay(100);
  window.print();
};
</script>

<template>
  <div class="page-title">
    <h1 class="title">NIAS 网络数据报表</h1>
    <div class="page-action">
      <button title="刷新页面" @click="refreshPage">
        <IconRefresh />
      </button>
      <button title="切换页面宽度" @click="togglePageWidth">
        <IconFullsize v-if="!ifFullWidth" />
        <IconNormalsize v-if="ifFullWidth" />
      </button>
      <button title="打印报表" @click="printPage">
        <IconPrint />
      </button>
    </div>
    <p class="subtitle">
      <span>XXXX站场</span>
      <time>XXXX-XX-XX XX:XX:XX</time>
      <span>XXXX</span>
    </p>
    <p class="text description">并难由照解果二满政之资亲社题较非，与队治形住青备蠢阶以子-做名。 除求太少号并到争关，指增及议七争织将常，以E虚金好杏养。 电斗三品放取族来商面，是至作题此三劳思酸，规连辰针每呜承总。 给音器类完但劳，维状府向号观题，结H声下知。</p>
  </div>
</template>

<style scoped>
.page-title {
  position: relative;
}
.page-action {
  position: absolute;
  right: 0;
  top: 0;
  margin: 0.5rem 0;
}
.page-action > button {
  background: transparent;
  background-color: var(--color-background-soft);
  border: 0.075rem solid var(--color-border);
  color: var(--color-text-second);
  border-radius: 0.5rem;
  padding: 0;
  font: inherit;
  width: 2rem;
  height: 2rem;
  cursor: pointer;
  outline: inherit;
  margin-left: 0.5rem;
  padding: 0.18rem;
  transition: border-color 0.2s, color 0.2s, background-color 0.2s;
}
.page-action > button:hover {
  background-color: var(--color-background);
  border-color: var(--color-text-highlight);
  color: var(--color-text-highlight);
}
.page-action > button:active {
  background-color: var(--color-background-mute);
  border-color: var(--color-border-hover);
  color: var(--color-text);
}
.page-action > button > svg {
  width: 100%;
  height: 100%;
}
.subtitle {
  color: var(--color-text-second);
}

.subtitle > *::after {
  content: "•";
  font-weight: bold;
  margin: 0 0.5rem;
}
.subtitle > *:last-child::after {
  content: none;
}

@media print {
  .title,
  .subtitle {
    text-align: center;
  }
  .page-action {
    display: none;
  }
}
</style>
