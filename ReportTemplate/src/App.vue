<script lang="ts">
declare interface NetworkData {
  foo: string;
}
</script>
<script setup lang="ts">
import HelloWorld from "./components/HelloWorld.vue";
import TheWelcome from "./components/TheWelcome.vue";

const rawData = document.getElementById("rawData")?.textContent;
const networkData: NetworkData = rawData ? JSON.parse(rawData) : null;
const data = networkData.foo;
</script>

<template>
  <header>
    <div class="wrapper">
      <HelloWorld :msg="data" />
    </div>
  </header>

  <main>
    <TheWelcome />
  </main>
</template>

<style scoped>
header {
  line-height: 1.5;
}

.logo {
  display: block;
  margin: 0 auto 2rem;
}

@media (min-width: 1024px) {
  header {
    display: flex;
    place-items: center;
    padding-right: calc(var(--section-gap) / 2);
  }

  .logo {
    margin: 0 2rem 0 0;
  }

  header .wrapper {
    display: flex;
    place-items: flex-start;
    flex-wrap: wrap;
  }
}
</style>
