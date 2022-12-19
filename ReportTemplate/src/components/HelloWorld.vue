<script setup lang="ts">
defineProps<{
  msg: string;
}>();
</script>
<script lang="ts">
declare interface NetworkData {
  foo: string;
}
export default {
  data() {
    const rawData = document.getElementById("rawData")?.textContent;
    const networkData: NetworkData = rawData ? JSON.parse(rawData) : null;
    console.log(networkData);
    return {
      message: "Hello World!",
      isRed: true,
      color: "green",
      data: networkData.foo,
    };
  },
  methods: {},
};
</script>

<template>
  <div class="greetings">
    <h1 class="green">{{ msg }}</h1>
    <h3>
      You’ve successfully created a project with
      <a target="_blank" href="https://vitejs.dev/">Vite</a> +
      <a target="_blank" href="https://vuejs.org/">Vue</a>.
    </h3>
    <p>{{ data }}</p>
  </div>
</template>

<style scoped>
h1 {
  font-weight: 500;
  font-size: 2.6rem;
  top: -10px;
}

h3 {
  font-size: 1.2rem;
}

.greetings h1,
.greetings h3 {
  text-align: center;
}

@media (min-width: 1024px) {
  .greetings h1,
  .greetings h3 {
    text-align: left;
  }
}
</style>
