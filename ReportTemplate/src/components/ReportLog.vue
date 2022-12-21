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
    <p class="text">
      This project is served and bundled with
      <a href="https://vitejs.dev/guide/features.html" target="_blank">Vite</a>.
      The recommended IDE setup is
      <a href="https://code.visualstudio.com/" target="_blank">VSCode</a> +
      <a href="https://github.com/johnsoncodehk/volar" target="_blank">Volar</a
      >. >. If you need to test your components and web pages, check out
      <a href="https://www.cypress.io/" target="_blank">Cypress</a> and
      <a href="https://on.cypress.io/component" target="_blank"
        >Cypress Component Testing</a
      >.
    </p>
    <p class="text">
      More instructions are available in <code>README.md</code>.
    </p>
    <p class="text">
      由图圆当即花目子可阶别圆群，置况用天流较太于治如只，白管级杨已极枪图知析X。
      及之元本手常眼义，极头专果下积今，信D学U量B。
      信养务口精其土完为，众张整四于基质记速，科G露呈离规王。
      人处何算至称被最今取，无具明器美价近格常，据展SM抖为派身。
      持两术及装用反指道深却，收商备路非研论能活派，始江极蹦始派造求抗。
      小没行江员花，件P去。
    </p>
    <p class="text">
      及程深物热且边保十称，开律光七公却解看用资，分信7极或个专利。
      需后必交因严高才矿，东线性以百地花要，但好束须束式陕。
      线广你江教越省没资，领则成种人准期，往地Q期东西两。
      养无山照门集工指，革二应积育着，斯细9图界呜。
      体选进业指道极必化理太越者始正，求须增志满南豆白求知C山。
    </p>
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

th {
  background-color: var(--color-background-soft);
}

table,
th,
td {
  padding: 0.5rem 1rem;
  border: 1px solid var(--color-border);
  border-collapse: collapse;
}
table {
  border-radius: 0.5rem;
  border-style: hidden;
  box-shadow: 0 0 0 1px var(--color-border);
}
th:first-of-type {
  border-top-left-radius: 0.5rem;
}
th:last-of-type {
  border-top-right-radius: 0.5rem;
}
tr:last-of-type td:first-of-type {
  border-bottom-left-radius: 0.5rem;
}
tr:last-of-type td:last-of-type {
  border-bottom-right-radius: 0.5rem;
}
</style>
