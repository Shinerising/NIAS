<script setup lang="ts">
import { format, fromUnixTime } from "date-fns";
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

const logs: Log[] =
  props.data.Log?.map((item) => ({
    time: fromUnixTime(item.Time ?? 0),
    text: item.Text ?? "",
    type:
      item.Name == "ERROR"
        ? "故障信息"
        : item.Name == "WARNING"
        ? "警告信息"
        : "通知信息",
    level: item.Name == "ERROR" ? 2 : item.Name == "WARNING" ? 1 : 0,
  })) ?? [];

const alarms: Log[] =
  props.data.Alarm?.map((item) => ({
    time: fromUnixTime(item.Time ?? 0),
    text: item.Text ?? "",
    type:
      item.Name == "ERROR"
        ? "故障信息"
        : item.Name == "WARNING"
        ? "警告信息"
        : "通知信息",
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
              {{ format(log.time, "yyyy-MM-dd HH:mm:ss") }}
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
    <template #brief>共包含{{ alarms.length }}条数据</template>
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
        <tr v-for="(log, i) in alarms" :key="i">
          <td align="center">{{ i }}</td>
          <td align="center">
            <time>
              {{ format(log.time, "yyyy-MM-dd HH:mm:ss") }}
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
