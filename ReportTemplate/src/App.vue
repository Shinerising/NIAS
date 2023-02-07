<script setup lang="ts">
<<<<<<< HEAD
import type { ReportData } from "./components/interface/ReportData.interface";
import DemoData from "./assets/demodata.json";
import { PrintStore } from "./stores/PrintStore";
=======
import type { NetworkData } from "./components/interface/NetworkData.interface";
>>>>>>> 3d122b55bd850f8abe910dcd6ff9497bba1b5a24
import ReportHeader from "./components/ReportHeader.vue";
import ReportStats from "./components/ReportStats.vue";
import ReportGraph from "./components/ReportGraph.vue";
import ReportLog from "./components/ReportLog.vue";
<<<<<<< HEAD
import loadI18n from "./locales/load";

const __ = loadI18n();

PrintStore().initialize();

const rawData = document.getElementById("rawData")?.textContent;
const reportData: ReportData = rawData ? JSON.parse(rawData) : DemoData;
const data = reportData;
=======

const rawData = document.getElementById("rawData")?.textContent;
const networkData: NetworkData = rawData ? JSON.parse(rawData) : null;
const data = networkData;
>>>>>>> 3d122b55bd850f8abe910dcd6ff9497bba1b5a24
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
<<<<<<< HEAD
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
=======
      <p>NIAS 网络智能分析系统 网络数据分析报表</p>
    </div>
    <div class="copy-right">
      <p>&copy; Apollo Wayne {{ new Date().getFullYear() }}. 保留所有权利</p>
>>>>>>> 3d122b55bd850f8abe910dcd6ff9497bba1b5a24
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
