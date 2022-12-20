<script setup lang="ts">
import type { NetworkData } from "./interface/NetworkData.interface";
import ArcCounter from "./controls/ArcCounter.vue";

defineProps<{
  data: NetworkData;
}>();
</script>

<template>
  <div>
    <h1>NIAS 网络数据报表</h1>
    <p class="subtitle">
      <span>XXXX站场</span>
      <time>XXXX-XX-XX XX:XX:XX</time>
      <span></span>
    </p>
    <p class="description">XXXXXX</p>
    <hr />
    <h2>局域网络整体统计数据</h2>
    <p>XXXXXX</p>
    <h2>网络环境健康指数</h2>
    <p></p>
    <div class="counter-wrapper">
      <ArcCounter :percent="0.15">
        <div class="counter-text">
          <div>网络安全</div>
          <number>15</number>
        </div>
      </ArcCounter>
      <div class="sepline-vertical" />
      <ArcCounter :percent="0.35">
        <div class="counter-text">
          <div>通信质量</div>
          <number>35</number>
        </div>
      </ArcCounter>
      <div class="sepline-vertical" />
      <ArcCounter :percent="0.65">
        <div class="counter-text">
          <div>设备状态</div>
          <number>65</number>
        </div>
      </ArcCounter>
      <div class="sepline-vertical" />
      <ArcCounter :percent="0.85">
        <div class="counter-text">
          <div>预期风险</div>
          <number>85</number>
        </div>
      </ArcCounter>
    </div>
  </div>
</template>

<style scoped>
.subtitle {
  color: var(--color-text-second);
}

.subtitle > *::after {
  content: "•";
  font-weight: bold;
  margin: 0 0.5rem;
}

h2 {
  margin: 1rem 0;
}

.counter-wrapper {
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
.counter-text > div {
  font-weight: bold;
  font-size: 0.9rem;
}
.counter-text > number {
  display: block;
  font-family: "Segoe UI", Tahoma, Geneva, Verdana, sans-serif;
  font-weight: bold;
  font-size: 1.2rem;
  text-align: center;
}
</style>
