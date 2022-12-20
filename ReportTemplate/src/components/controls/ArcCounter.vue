<script setup lang="ts">
const props = defineProps<{
  percent: number;
}>();

const origin = [80, 80];
const radius = 61.25;
const angle = Math.PI * 0.5 - props.percent * Math.PI * 2;
const target = [
  Math.cos(angle) * radius + origin[0],
  Math.sin(angle) * radius * -1 + origin[1],
];
const path = `M ${origin[0]} ${origin[1] - radius} A ${radius} ${radius} 0 ${
  props.percent > 0.5 ? 1 : 0
} 1 ${target[0]} ${target[1]}`;

let color = "#4CAF50";
if (props.percent < 0.25) {
  color = "#BF360C";
} else if (props.percent < 0.5) {
  color = "#F57C00";
} else if (props.percent < 0.75) {
  color = "#FDD835";
}
</script>

<template>
  <div class="counter">
    <svg xmlns="http://www.w3.org/2000/svg">
      <circle
        :cx="origin[0]"
        :cy="origin[1]"
        :r="radius"
        :fill="color"
        opacity=".15"
      />
      <path
        :d="path"
        fill="none"
        :stroke="color"
        stroke-width="12"
        stroke-linecap="round"
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
