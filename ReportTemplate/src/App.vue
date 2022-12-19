<script setup lang="ts">
import type { NetworkData } from "./components/interface/NetworkData.interface";
import ReportHeader from "./components/ReportHeader.vue";
import ReportGraph from "./components/ReportGraph.vue";
import ReportLog from "./components/ReportLog.vue";

const rawData = document.getElementById("rawData")?.textContent;
const networkData: NetworkData = rawData ? JSON.parse(rawData) : null;
const data = networkData;
</script>

<template>
  <header>
    <div class="wrapper">
      <ReportHeader :data="data" />
    </div>
  </header>

  <main>
    <ReportLog :data="data" />
    <ReportGraph :data="data" />
  </main>

  <footer>
    <div></div>
  </footer>
</template>

<style scoped>
header {
  line-height: 1.5;
}
</style>
