<script setup lang="ts">
import type { ReportData } from "./components/interface/ReportData.interface";
import DemoData from "./assets/demodata.json";
import { PrintStore } from "./stores/PrintStore";
import ReportHeader from "./components/ReportHeader.vue";
import ReportStats from "./components/ReportStats.vue";
import ReportGraph from "./components/ReportGraph.vue";
import ReportLog from "./components/ReportLog.vue";
import loadI18n from "./locales/load";
import { decompressSync, strFromU8 } from "fflate";

const __ = loadI18n();

PrintStore().initialize();

const resolveData = (data: string, decompress = false) => {
  if (!decompress) {
    return JSON.parse(data);
  }
  const json = strFromU8(
    decompressSync(Uint8Array.from(atob(data), (c) => c.charCodeAt(0)))
  );
  return JSON.parse(json);
};

const rawData = document.getElementById("rawData")?.textContent;
const reportData: ReportData = rawData
  ? resolveData(
      rawData,
      document.getElementById("rawData")?.hasAttribute("compressed")
    )
  : DemoData;

const data = reportData;
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
