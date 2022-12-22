<script setup lang="ts">
import type { NetworkData } from "./interface/NetworkData.interface";
import ArcCounter from "./controls/ArcCounter.vue";
import ImageComputer from "./images/ImageComputer.vue";
import ImageRouter from "./images/ImageRouter.vue";
import ImageSwitch from "./images/ImageSwitch.vue";

defineProps<{
  data: NetworkData;
}>();
</script>

<template>
  <h2>网络环境健康指数</h2>
  <div class="counter-wrapper">
    <ArcCounter :percent="0.15">
      <div class="counter-text">
        <div>网络安全</div>
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
        <div>预期风险</div>
        <div class="number">85</div>
      </div>
    </ArcCounter>
  </div>
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
ul.device-list > li > *:last-child {
  border-right: none;
}

ul.device-list > li > svg {
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
