<script setup lang="ts">
import type { ReportData } from "./interface/ReportData.interface";
import ArcCounter from "./controls/ArcCounter.vue";
import {
  ImageComputer,
  ImageRouter,
  ImageSwitch,
} from "./images/ImageResource";

defineProps<{
  data: ReportData;
}>();

const deviceList = {
  switchList: [
    { name: "Switch A", system: "Huawei S5720", brief: "172.16.34.1" },
    { name: "Switch B", system: "Huawei S5720", brief: "172.16.34.2" },
    { name: "Switch C", system: "Huawei S5720", brief: "172.16.34.3" },
    { name: "Switch D", system: "Huawei S5720", brief: "172.16.34.4" },
  ],
  computerList: [
    { name: "Computer", system: "Windows 11", brief: "172.16.34.101" },
    { name: "Computer", system: "Windows 11", brief: "172.16.34.101" },
    { name: "Computer", system: "Windows 11", brief: "172.16.34.101" },
    { name: "Computer", system: "Windows 11", brief: "172.16.34.101" },
    { name: "Computer", system: "Windows 11", brief: "172.16.34.101" },
  ],
  hostList: [
    { name: "NPort", system: "Linux", brief: "172.16.34.201" },
    { name: "NPort", system: "Linux", brief: "172.16.34.201" },
    { name: "NPort", system: "Linux", brief: "172.16.34.201" },
  ],
};
</script>

<template>
  <h2>网络环境健康指数</h2>
  <div class="counter-wrapper">
    <ArcCounter :percent="0.15">
      <div class="counter-text">
        <div>网络稳定</div>
        <div class="number">15</div>
      </div>
    </ArcCounter>
    <div class="sepline-vertical" />
    <ArcCounter :percent="0.35">
      <div class="counter-text">
        <div>通信质量</div>
        <div class="number">35</div>
      </div>
    </ArcCounter>
    <div class="sepline-vertical" />
    <ArcCounter :percent="0.65">
      <div class="counter-text">
        <div>设备状态</div>
        <div class="number">65</div>
      </div>
    </ArcCounter>
    <div class="sepline-vertical" />
    <ArcCounter :percent="0.85">
      <div class="counter-text">
        <div>安全风险</div>
        <div class="number">85</div>
      </div>
    </ArcCounter>
  </div>
  <div class="record-box">
    <p class="text">
      <b>网络稳定指数：</b>
      <span>描述设备运行稳定程度，与网络中断次数、中断时长相关。</span>
    </p>
    <p class="text">
      在过去24小时内，局域网内部共发生通信障碍48次，涉及3台设备，中断时长共计145分钟。建议查看日志记录信息了解故障发生时间和发生位置，及时进行维护和优化工作。
    </p>
  </div>
  <div class="record-box">
    <p class="text">
      <b>通信质量指数：</b>
      <span>描述设备之间数据通信质量，与网络时延、数据流量大小相关。</span>
    </p>
    <p class="text">
      在过去24小时内，局域网内部2台设备出现短时流量大于正常值，3台设备短时网络延迟大于正常值。建议查看日志记录信息了解详细信息，及时检测设备是否存在网络工作障碍。
    </p>
  </div>
  <div class="record-box">
    <p class="text">
      <b>设备状态指数：</b>
      <span>描述网络设备运行平稳状态，与设备温度、CPU和内存占用率相关。</span>
    </p>
    <p class="text">在过去24小时内，所有设备运行基本平稳。</p>
  </div>
  <div class="record-box">
    <p class="text">
      <b>安全风险指数：</b>
      <span
        >描述网络潜在安全风险，与设备类型、操作系统版本与高风险端口相关。</span
      >
    </p>
    <p class="text">在过去24小时内，所有设备运行基本平稳。</p>
  </div>
  <hr />
  <h2>局域网络整体统计数据</h2>
  <p>交换机设备</p>
  <ul class="device-list">
    <li v-for="(item, i) in deviceList.switchList" :key="i">
      <img :src="ImageSwitch" />
      <span>{{ item.name }}</span>
      <span>{{ item.system }}</span>
      <span>{{ item.brief }}</span>
    </li>
  </ul>
  <p>计算机设备</p>
  <ul class="device-list">
    <li v-for="(item, i) in deviceList.computerList" :key="i">
      <img :src="ImageComputer" />
      <span>{{ item.name }}</span>
      <span>{{ item.system }}</span>
      <span>{{ item.brief }}</span>
    </li>
  </ul>
  <p>网络通信设备</p>
  <ul class="device-list">
    <li v-for="(item, i) in deviceList.hostList" :key="i">
      <img :src="ImageRouter" />
      <span>{{ item.name }}</span>
      <span>{{ item.system }}</span>
      <span>{{ item.brief }}</span>
    </li>
  </ul>
</template>

<style scoped>
h2 {
  margin: 1rem 0;
}

.counter-wrapper {
  margin: 2rem auto;
  display: flex;
  justify-content: center;
}

.counter-wrapper > * {
  margin: 1rem;
}

.sepline-vertical {
  border-right: 1px solid var(--color-border);
}

.counter-text > div {
  font-weight: bold;
  font-size: 1rem;
}

.counter-text > .number {
  display: block;
  font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
  font-weight: bold;
  font-size: 1.5rem;
  text-align: center;
}

.record-box {
  padding: 0.5rem 1rem;
  border: 1px solid var(--color-border);
  border-radius: 0.5rem;
  margin-bottom: 1rem;
}

.record-box span {
  color: var(--color-text-second);
}

ul.device-list {
  margin: 1rem 0;
  padding: 0;
}

ul.device-list > li {
  list-style: none;
  margin: 0.5rem 0;
  display: flex;
  align-items: center;
  border-radius: 0.5rem;
  border: 1px solid var(--color-border);
  font-size: 0.9rem;
}

ul.device-list > li:nth-child(even) {
  background-color: var(--color-background-soft);
}

ul.device-list > li > * {
  display: block;
  flex: 1;
  padding: 0.5rem;
  border-right: 1px solid var(--color-border);
}

ul.device-list > li > *:nth-child(3) {
  flex: 2;
}

ul.device-list > li > *:last-child {
  border-right: none;
}

ul.device-list > li > img {
  flex: none;
  width: 2.4rem;
  height: 2.4rem;
  margin: 0;
}

@media print {
  .sepline-vertical {
    margin: 1rem 0;
  }
}
</style>
