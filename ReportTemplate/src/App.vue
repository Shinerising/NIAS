<script setup lang="ts">
import type { NetworkData } from "./components/interface/NetworkData.interface";
import { PrintStore } from "./stores/PrintStore";
import ReportHeader from "./components/ReportHeader.vue";
import ReportStats from "./components/ReportStats.vue";
import ReportGraph from "./components/ReportGraph.vue";
import ReportLog from "./components/ReportLog.vue";

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
      <p>NIAS 网络智能分析系统 网络数据分析报表</p>
    </div>
    <div class="copy-right">
      <p>&copy; Apollo Wayne {{ new Date().getFullYear() }}. 保留所有权利</p>
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
