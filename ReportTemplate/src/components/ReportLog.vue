<script setup lang="ts">
import type { NetworkData } from "./interface/NetworkData.interface";
import ReportSection from "./ReportSection.vue";
import IconReport from "./icons/IconReport.vue";
import IconAlarm from "./icons/IconAlarm.vue";

defineProps<{
  data: NetworkData;
}>();

const sales = [
  [
    { Year: 2018, Month: 1, Sale: 512 },
    { Year: 2018, Month: 2, Sale: 1025 },
  ],
  [
    { Year: 2017, Month: 1, Sale: 155 },
    { Year: 2017, Month: 2, Sale: 12 },
  ],
];
</script>

<template>
  <h2>日志记录</h2>
  <ReportSection>
    <template #icon>
      <IconReport />
    </template>
    <template #heading>网络故障与异常报警</template>
    <template #brief>XX条数据</template>
    <table class="table">
      <thead>
        <tr>
          <th>#</th>
          <th>2018</th>
          <th>2017</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(sale, i) in sales[0]" :key="i">
          <td>{{ sale.Month }}</td>
          <td>{{ sale.Sale }}</td>
          <td>{{ sales[1][i].Sale }}</td>
        </tr>
      </tbody>
    </table>
  </ReportSection>

  <ReportSection>
    <template #icon>
      <IconAlarm />
    </template>
    <template #heading>网络环境风险预警</template>

    This project is served and bundled with
    <a href="https://vitejs.dev/guide/features.html" target="_blank">Vite</a>.
    The recommended IDE setup is
    <a href="https://code.visualstudio.com/" target="_blank">VSCode</a> +
    <a href="https://github.com/johnsoncodehk/volar" target="_blank">Volar</a>.
    If you need to test your components and web pages, check out
    <a href="https://www.cypress.io/" target="_blank">Cypress</a> and
    <a href="https://on.cypress.io/component" target="_blank">Cypress Component Testing</a>.

    <br />

    More instructions are available in <code>README.md</code>.
  </ReportSection>
</template>

<style scoped>
h2 {
  margin: 1rem 0;
}

table {
  margin: 0.5rem 0;
  width: 100%;
}

th{
  background-color: var(--color-background-soft);
}

table,
th,
td {
  padding: 0.5rem 1rem;
  border: 1px solid var(--color-border);
  border-collapse: collapse;
}
</style>
