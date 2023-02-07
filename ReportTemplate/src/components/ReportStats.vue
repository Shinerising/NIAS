<script setup lang="ts">
<<<<<<< HEAD
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
    brief: item.Vendor,
  })),
};
=======
import type { NetworkData } from "./interface/NetworkData.interface";
import ArcCounter from "./controls/ArcCounter.vue";
import ImageComputer from "./images/ImageComputer.vue";
import ImageRouter from "./images/ImageRouter.vue";
import ImageSwitch from "./images/ImageSwitch.vue";

defineProps<{
  data: NetworkData;
}>();
>>>>>>> 3d122b55bd850f8abe910dcd6ff9497bba1b5a24
</script>

<template>
  <h2>网络环境健康指数</h2>
  <div class="counter-wrapper">
    <ArcCounter :percent="0.15">
      <div class="counter-text">
<<<<<<< HEAD
        <div>网络稳定</div>
=======
        <div>网络安全</div>
>>>>>>> 3d122b55bd850f8abe910dcd6ff9497bba1b5a24
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
<<<<<<< HEAD
        <div>安全风险</div>
=======
        <div>预期风险</div>
>>>>>>> 3d122b55bd850f8abe910dcd6ff9497bba1b5a24
        <div class="number">85</div>
      </div>
    </ArcCounter>
  </div>
<<<<<<< HEAD
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
  <hr class="top-zero" />
  <h2>局域网络整体统计数据</h2>
  <p><b>交换机设备</b></p>
  <p v-if="!deviceList.switchList?.length">未能收集到相关数据。</p>
  <ul class="device-list">
    <li v-for="(item, i) in deviceList.switchList" :key="i">
      <img :src="ImageSwitch" alt="Icon Switch Hub" />
      <span>{{ item.name || "未知" }}</span>
      <span>{{ item.address || "未知" }}</span>
      <span>{{ item.mac || "未知" }}</span>
      <span>{{ item.brief || "未知" }}</span>
    </li>
  </ul>
  <p><b>计算机设备</b></p>
  <p v-if="!deviceList.computerList?.length">未能收集到相关数据。</p>
  <ul class="device-list">
    <li v-for="(item, i) in deviceList.computerList" :key="i">
      <img :src="ImageComputer" alt="Icon Computer" />
      <span>{{ item.name || "未知" }}</span>
      <span>{{ item.address || "未知" }}</span>
      <span>{{ item.mac || "未知" }}</span>
      <span>{{ item.brief || "未知" }}</span>
    </li>
  </ul>
  <p><b>网络通信设备</b></p>
  <p v-if="!deviceList.hostList?.length">未能收集到相关数据。</p>
  <ul class="device-list">
    <li v-for="(item, i) in deviceList.hostList" :key="i">
      <img :src="ImageRouter" alt="Icon Network Router" />
      <span>{{ item.name || "未知" }}</span>
      <span>{{ item.address || "未知" }}</span>
      <span>{{ item.mac || "未知" }}</span>
      <span>{{ item.brief || "未知" }}</span>
=======
  <p class="text">
    路东分化养机全个压意小们，压圆起风议心被证正带，把专史5与鹰队家成辰。
    委走内把效越你强采派层，前部选状历Q管常油。
    论六加利传起例门团，器活个教间为识十，义状询年造率僚。
    近备月看月走越声律并厂，发政深今众精前少眼结主，叫选-类在委子扭励。
    运华派称即始层强条石先要决布治，济林龙细间特U素果该公程。
    规京力每同或金有极，今证器居串带。
  </p>
  <hr />
  <h2>局域网络整体统计数据</h2>
  <p>交换机设备</p>
  <ul class="device-list">
    <li>
      <ImageSwitch />
      <span>XXXX</span>
      <span>XXXX</span>
      <span>XXXX</span>
    </li>
    <li>
      <ImageSwitch />
      <span>XXXX</span>
      <span>XXXX</span>
      <span>XXXX</span>
    </li>
    <li>
      <ImageSwitch />
      <span>XXXX</span>
      <span>XXXX</span>
      <span>XXXX</span>
    </li>
  </ul>
  <p>计算机设备</p>
  <ul class="device-list">
    <li>
      <ImageComputer />
      <span>XXXX</span>
      <span>XXXX</span>
      <span>XXXX</span>
    </li>
    <li>
      <ImageComputer />
      <span>XXXX</span>
      <span>XXXX</span>
      <span>XXXX</span>
    </li>
    <li>
      <ImageComputer />
      <span>XXXX</span>
      <span>XXXX</span>
      <span>XXXX</span>
    </li>
    <li>
      <ImageComputer />
      <span>XXXX</span>
      <span>XXXX</span>
      <span>XXXX</span>
    </li>
  </ul>
  <p>网络通信设备</p>
  <ul class="device-list">
    <li>
      <ImageRouter />
      <span>XXXX</span>
      <span>XXXX</span>
      <span>XXXX</span>
    </li>
    <li>
      <ImageRouter />
      <span>XXXX</span>
      <span>XXXX</span>
      <span>XXXX</span>
    </li>
    <li>
      <ImageRouter />
      <span>XXXX</span>
      <span>XXXX</span>
      <span>XXXX</span>
    </li>
    <li>
      <ImageRouter />
      <span>XXXX</span>
      <span>XXXX</span>
      <span>XXXX</span>
    </li>
    <li>
      <ImageRouter />
      <span>XXXX</span>
      <span>XXXX</span>
      <span>XXXX</span>
>>>>>>> 3d122b55bd850f8abe910dcd6ff9497bba1b5a24
    </li>
  </ul>
</template>

<style scoped>
h2 {
  margin: 1rem 0;
}

.counter-wrapper {
<<<<<<< HEAD
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

=======
  margin: 2rem 0;
  display: flex;
  justify-content: center;
}
.counter-wrapper > * {
  margin: 1rem;
}
.sepline-vertical {
  border-right: 1px solid var(--color-border);
}
>>>>>>> 3d122b55bd850f8abe910dcd6ff9497bba1b5a24
.counter-text > div {
  font-weight: bold;
  font-size: 1rem;
}
<<<<<<< HEAD

=======
>>>>>>> 3d122b55bd850f8abe910dcd6ff9497bba1b5a24
.counter-text > .number {
  display: block;
  font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
  font-weight: bold;
  font-size: 1.5rem;
  text-align: center;
}

<<<<<<< HEAD
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

=======
>>>>>>> 3d122b55bd850f8abe910dcd6ff9497bba1b5a24
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
<<<<<<< HEAD

ul.device-list > li:nth-child(even) {
  background-color: var(--color-background-soft);
}

ul.device-list > li > * {
  display: block;
  flex: 2 1 0;
  width: 0;
  padding: 0.5rem;
  word-wrap: break-word;
  border-right: 1px solid var(--color-border);
}

ul.device-list > li > *:nth-child(2) {
  flex: 1 1 0;
  width: 0;
  word-wrap: break-word;
}

=======
ul.device-list > li:nth-child(even) {
  background-color: var(--color-background-soft);
}
ul.device-list > li > * {
  display: block;
  flex: 1;
  padding: 0.5rem;
  border-right: 1px solid var(--color-border);
}
>>>>>>> 3d122b55bd850f8abe910dcd6ff9497bba1b5a24
ul.device-list > li > *:last-child {
  border-right: none;
}

<<<<<<< HEAD
ul.device-list > li > img {
=======
ul.device-list > li > svg {
>>>>>>> 3d122b55bd850f8abe910dcd6ff9497bba1b5a24
  flex: none;
  width: 2.4rem;
  height: 2.4rem;
  margin: 0;
}
<<<<<<< HEAD

@media print {
  .sepline-vertical {
    margin: 1rem 0;
  }
}
=======
>>>>>>> 3d122b55bd850f8abe910dcd6ff9497bba1b5a24
</style>
