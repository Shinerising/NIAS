<script setup lang="ts">
import moment from "moment";
import type { ReportData } from "./interface/ReportData.interface";
import ReportSection from "./ReportSection.vue";
import IconReport from "./icons/IconReport.vue";
import IconAlarm from "./icons/IconAlarm.vue";
import IconInfo from "./icons/IconInfo.vue";
import IconWarning from "./icons/IconWarning.vue";
import IconError from "./icons/IconError.vue";

const props = defineProps<{
  data: ReportData;
}>();

type Log = {
  time: Date;
  text: string;
  type: string;
  level: 0 | 1 | 2;
};

const logs: Log[] = props.data.Log?.map((item) => ({
  time: moment.unix(item.Time ?? 0).toDate(),
  text: item.Text ?? "",
  type: item.Name == "ERROR" ? "故障信息" : item.Name == "WARNING" ? "警告信息" : "通知信息",
  level: item.Name == "ERROR" ? 2 : item.Name == "WARNING" ? 1 : 0,
})) ?? [];
</script>

<template>
  <h2>日志记录</h2>
  <ReportSection>
    <template #icon>
      <IconReport />
    </template>
    <template #heading>网络故障与异常报警</template>
    <template #brief>共包含{{ logs.length }}条数据</template>
    <table class="table">
      <thead>
        <tr>
          <th width="5%">序号</th>
          <th width="15%">时间</th>
          <th>报警文本</th>
          <th width="10%">报警类型</th>
          <th width="8%">风险等级</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="(log, i) in logs" :key="i">
          <td align="center">{{ i }}</td>
          <td align="center">
            <time>
              {{ moment(log.time).format("yyyy-MM-DD HH:mm:ss") }}
            </time>
          </td>
          <td>{{ log.text }}</td>
          <td align="center">{{ log.type }}</td>
          <td align="center" class="log-icon">
            <span class="level-info" title="提示信息" v-if="log.level == 0">
              <IconInfo />
            </span>
            <span class="level-warning" title="警告信息" v-if="log.level == 1">
              <IconWarning />
            </span>
            <span class="level-error" title="故障信息" v-if="log.level == 2">
              <IconError />
            </span>
          </td>
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
  padding: 0.5rem;
  border: 1px solid var(--color-border);
  border-collapse: collapse;
}
table {
  font-size: 0.9rem;
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
.log-icon {
  padding: 0 0.5rem;
}
.log-icon svg {
  vertical-align: top;
}
.log-icon .level-info {
  color: #03a9f4;
}
.log-icon .level-warning {
  color: #ffb300;
}
.log-icon .level-error {
  color: #c62828;
}
</style>
