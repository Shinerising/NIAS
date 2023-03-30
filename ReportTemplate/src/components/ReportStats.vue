<script setup lang="ts">
import type { ReportData } from "./interface/ReportData.interface";
import ArcCounter from "./controls/ArcCounter.vue";
import {
  ImageComputer,
  ImageRouter,
  ImageSwitch,
} from "./images/ImageResource";

const props = defineProps<{
  data: ReportData;
}>();

const caculatePercent = (arr?: number[]) => {
  if (!arr) {
    return 0;
  }
  let result = 1;
  for (let i = 0; i < arr.length - 1; i += 2) {
    result *= arr[i + 1] > arr[i] ? (arr[i] + 1) / (arr[i + 1] + 1) : 1;
  }
  return 1 - result * 0.5;
};

const getStats = () => {
  const [healthStats, networkStats, SensorStats, PortStats] =
    props.data.Stats ?? [];
  const healthScore = caculatePercent(healthStats);
  const healthInfo =
    healthStats === undefined
      ? "暂无数据"
      : healthScore === 1
      ? "在过去24小时内，网络运行过程基本平稳。"
      : `在过去24小时内，局域网内部发生通信障碍，中断时长共计${
          healthStats[0] + healthStats[2]
        }分钟。建议查看日志记录信息了解故障发生时间和发生位置，及时进行维护和优化工作。`;
  const networkScore = caculatePercent(networkStats);
  const networkInfo =
    networkStats === undefined
      ? "暂无数据"
      : networkScore === 1
      ? "在过去24小时内，网络通信质量基本平稳。"
      : `在过去24小时内，局域网内部${
          networkStats[0] + networkStats[2]
        }分钟出现短时流量大于正常值，${
          networkStats[4]
        }分钟出现短时网络延迟大于正常值。建议查看日志记录信息了解详细信息，及时检测设备是否存在网络工作障碍。`;
  const SensorScore = caculatePercent(SensorStats);
  const SensorInfo =
    SensorStats === undefined
      ? "暂无数据"
      : SensorScore === 1
      ? "在过去24小时内，所有设备运行基本平稳。"
      : `在过去24小时内，交换机出现潜在性能问题，CPU超载持续${SensorStats[0]}分钟，内存超载持续${SensorStats[2]}分钟，设备温度超出正常温度${SensorStats[4]}分钟。建议查看设备是否处于正常工作状态，及时进行维护和优化工作。`;
  const PortScore = caculatePercent(PortStats);
  const PortInfo =
    PortStats === undefined
      ? "暂无数据"
      : PortScore === 1
      ? "在过去24小时内，暂未发现网络安全风险。"
      : `在过去24小时内，发现${PortStats[0]}个设备正在运行不安全的操作系统，同时存在${PortStats[2]}个潜在危险网络端口。建议查看各设备系统参数，及时进行系统升级和漏洞修补工作。`;

  return {
    healthScore,
    healthInfo,
    networkScore,
    networkInfo,
    SensorScore,
    SensorInfo,
    PortScore,
    PortInfo,
  };
};

const stats = getStats();

const deviceList = {
  switchList: props.data.SwitchInfo?.map((item) => ({
    name: item.Name,
    address: item.Address,
    mac: item.MACAddress,
    brief: item.Vendor,
  })),
  computerList: props.data.HostInfo?.map((item) => ({
    name: item.Name,
    address: item.Address,
    mac: item.MACAddress,
    brief: item.Vendor,
  })),
  hostList: props.data.DeviceInfo?.map((item) => ({
    name: item.Name,
    address: item.Address,
    mac: item.MACAddress,
    brief: [item.OS, item.Vendor]
      .filter((item) => item !== undefined && item !== "")
      .join(" , "),
  })),
};
</script>

<template>
  <h2>网络环境健康指数</h2>
  <div class="counter-wrapper">
    <ArcCounter :percent="stats.healthScore">
      <div class="counter-text">
        <div>网络稳定</div>
        <div class="number">{{ (stats.healthScore * 100).toFixed(0) }}</div>
      </div>
    </ArcCounter>
    <div class="sepline-vertical" />
    <ArcCounter :percent="stats.networkScore">
      <div class="counter-text">
        <div>通信质量</div>
        <div class="number">{{ (stats.networkScore * 100).toFixed(0) }}</div>
      </div>
    </ArcCounter>
    <div class="sepline-vertical" />
    <ArcCounter :percent="stats.SensorScore">
      <div class="counter-text">
        <div>设备状态</div>
        <div class="number">{{ (stats.SensorScore * 100).toFixed(0) }}</div>
      </div>
    </ArcCounter>
    <div class="sepline-vertical" />
    <ArcCounter :percent="stats.PortScore">
      <div class="counter-text">
        <div>安全风险</div>
        <div class="number">{{ (stats.PortScore * 100).toFixed(0) }}</div>
      </div>
    </ArcCounter>
  </div>
  <div class="record-box">
    <p class="text">
      <b>网络稳定指数：</b>
      <span>描述设备运行稳定程度，与网络中断次数、中断时长相关。</span>
    </p>
    <p class="text">{{ stats.healthInfo }}</p>
  </div>
  <div class="record-box">
    <p class="text">
      <b>通信质量指数：</b>
      <span>描述设备之间数据通信质量，与网络时延、数据流量大小相关。</span>
    </p>
    <p class="text">
      {{ stats.networkInfo }}
    </p>
  </div>
  <div class="record-box">
    <p class="text">
      <b>设备状态指数：</b>
      <span>描述网络设备运行平稳状态，与设备温度、CPU和内存占用率相关。</span>
    </p>
    <p class="text">
      {{ stats.SensorInfo }}
    </p>
  </div>
  <div class="record-box">
    <p class="text">
      <b>安全风险指数：</b>
      <span
        >描述网络潜在安全风险，与设备类型、操作系统版本与高风险端口相关。</span
      >
    </p>
    <p class="text">
      {{ stats.PortInfo }}
    </p>
  </div>
  <hr class="top-zero" />
  <h2>局域网络整体统计数据</h2>
  <p><b>交换机设备</b></p>
  <p v-if="!deviceList.switchList?.length">未能收集到相关数据。</p>
  <ul class="device-list">
    <li v-for="(item, i) in deviceList.switchList" :key="i">
      <img :src="ImageSwitch" alt="Icon Switch Hub" />
      <span :title="item.name || '未知'">{{ item.name || "未知" }}</span>
      <span :title="item.address || '未知'">{{ item.address || "未知" }}</span>
      <span :title="item.mac || '未知'">{{ item.mac || "未知" }}</span>
      <span :title="item.brief || '未知'">{{ item.brief || "未知" }}</span>
    </li>
  </ul>
  <p><b>计算机设备</b></p>
  <p v-if="!deviceList.computerList?.length">未能收集到相关数据。</p>
  <ul class="device-list">
    <li v-for="(item, i) in deviceList.computerList" :key="i">
      <img :src="ImageComputer" alt="Icon Computer" />
      <span :title="item.name || '未知'">{{ item.name || "未知" }}</span>
      <span :title="item.address || '未知'">{{ item.address || "未知" }}</span>
      <span :title="item.mac || '未知'">{{ item.mac || "未知" }}</span>
      <span :title="item.brief || '未知'">{{ item.brief || "未知" }}</span>
    </li>
  </ul>
  <p><b>网络通信设备</b></p>
  <p v-if="!deviceList.hostList?.length">未能收集到相关数据。</p>
  <ul class="device-list">
    <li v-for="(item, i) in deviceList.hostList" :key="i">
      <img :src="ImageRouter" alt="Icon Network Router" />
      <span :title="item.name || '未知'">{{ item.name || "未知" }}</span>
      <span :title="item.address || '未知'">{{ item.address || "未知" }}</span>
      <span :title="item.mac || '未知'">{{ item.mac || "未知" }}</span>
      <span :title="item.brief || '未知'">{{ item.brief || "未知" }}</span>
    </li>
  </ul>
</template>

<style scoped>
h2 {
  margin: 1rem 0;
}

.counter-wrapper {
  margin: 1rem auto;
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
  padding: 1rem 0;
  border-top: 1px solid var(--color-border);
}

hr.top-zero {
  margin-top: 0;
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
  flex: 2 1 0;
  width: 0;
  padding: 0.5rem;
  white-space: nowrap;
  overflow: hidden;
  text-overflow: ellipsis;
  border-right: 1px solid var(--color-border);
}

ul.device-list > li > *:nth-child(2) {
  flex: 1 1 0;
  width: 0;
  word-wrap: break-word;
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
