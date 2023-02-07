<script setup lang="ts">
import { ref } from "vue";

const props = defineProps<{
  percent: number;
}>();

const origin = [80, 80];
const radius = 61.25;
const perimeter = radius * Math.PI * 2;
const rotate = `rotate(-90 ${origin[0]} ${origin[1]})`;
const dasharray = ref(`0 ${perimeter}`);
const viewBox = `0 0 ${origin[0] * 2} ${origin[1] * 2}`;

setTimeout(() => {
  dasharray.value = `${perimeter * props.percent} ${
    perimeter * (1 - props.percent)
  }`;
}, 300);

let color = "#66BB6A";
if (props.percent < 0.25) {
  color = "#BF360C";
} else if (props.percent < 0.5) {
  color = "#F57C00";
} else if (props.percent < 0.75) {
  color = "#FBC02D";
}
</script>

<template>
  <div class="counter">
    <svg xmlns="http://www.w3.org/2000/svg" :viewBox="viewBox">
      <circle
        :cx="origin[0]"
        :cy="origin[1]"
        :r="radius"
        :fill="color"
        opacity=".15"
      />
      <circle
        :cx="origin[0]"
        :cy="origin[1]"
        :r="radius"
        fill="none"
        :stroke="color"
        stroke-width="12"
        :transform="rotate"
        stroke-linecap="round"
        :stroke-dasharray="dasharray"
        style="transition: stroke-dasharray 0.5s"
      />
    </svg>
    <div class="details">
      <slot></slot>
    </div>
  </div>
</template>

<style scoped>
.counter {
  position: relative;
  width: 10rem;
  height: 10rem;
}
svg {
  position: absolute;
  width: 10rem;
  height: 10rem;
}
.details {
  width: 10rem;
  height: 10rem;
  display: flex;
  align-items: center;
  justify-content: center;
}
</style>
