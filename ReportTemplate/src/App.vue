<script setup lang="ts">
import type { NetworkData } from "./components/interface/NetworkData.interface";
import { PrintStore } from "./stores/PrintStore";
import ReportHeader from "./components/ReportHeader.vue";
import ReportStats from "./components/ReportStats.vue";
import ReportGraph from "./components/ReportGraph.vue";
import ReportLog from "./components/ReportLog.vue";
import loadI18n from "./locales/load";

const __ = loadI18n();

PrintStore().initialize();

const rawData = document.getElementById("rawData")?.textContent;
const networkData: NetworkData = rawData ? JSON.parse(rawData) : null;
const data = networkData;
</script>

<template>
  <header>
    <ReportHeader :data="data" />
  </header>

  <hr />

  <main>
    <ReportStats :data="data" />
    <hr />
    <ReportLog :data="data" />
    <hr />
    <ReportGraph :data="data" />
  </main>

  <hr />

  <footer>
    <div class="page-detail">
      <p>{{ __("page_footer_about") }}</p>
    </div>
    <div class="powered">
      <p>
        {{ __("page_footer_powerby") }}
        <a href="https://vuejs.org" target="_blank" rel="noreferrer noopener"
          >Vue</a
        >
        &
        <a href="https://vitejs.dev" target="_blank" rel="noreferrer noopener"
          >Vite</a
        >
        &
        <a
          href="https://echarts.apache.org"
          target="_blank"
          rel="noreferrer noopener"
          >ECharts</a
        >
      </p>
    </div>
    <div class="copy-right">
      <p>
        {{
          __("page_footer_copyright", [
            "Apollo Wayne",
            new Date().getFullYear(),
          ])
        }}
      </p>
    </div>
  </footer>
</template>

<style scoped>
header {
  line-height: 1.5;
}
footer {
  margin-top: 2.5rem;
}
footer p {
  text-align: center;
  font-size: 0.8rem;
  color: var(--color-text-second);
}
</style>
