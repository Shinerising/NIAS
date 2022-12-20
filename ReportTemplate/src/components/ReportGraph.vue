<script setup lang="ts">
import { use } from "echarts/core";
import { CanvasRenderer } from "echarts/renderers";
import { PieChart } from "echarts/charts";
import {
  TitleComponent,
  TooltipComponent,
  LegendComponent,
} from "echarts/components";
import VChart, { THEME_KEY } from "vue-echarts";
import { ref, provide } from "vue";

import type { NetworkData } from "./interface/NetworkData.interface";
import ReportSection from "./ReportSection.vue";
import IconComputer from "./icons/IconComputer.vue";
import IconEthernet from "./icons/IconEthernet.vue";
import IconChart from "./icons/IconChart.vue";
import IconRouter from "./icons/IconRouter.vue";
import IconHub from "./icons/IconHub.vue";

defineProps<{
  data: NetworkData;
}>();

/**
 * Switch Status: Line Chart
 * Switch Port: Heatmap Chart
 * Device Connection: Relationship Chart
 */

use([
  CanvasRenderer,
  PieChart,
  TitleComponent,
  TooltipComponent,
  LegendComponent,
]);

provide(THEME_KEY, "light");

const option = ref({
  title: {
    text: "Traffic Sources",
    left: "center",
  },
  tooltip: {
    trigger: "item",
    formatter: "{a} <br/>{b} : {c} ({d}%)",
  },
  legend: {
    orient: "vertical",
    left: "left",
    data: ["Direct", "Email", "Ad Networks", "Video Ads", "Search Engines"],
  },
  series: [
    {
      name: "Traffic Sources",
      type: "pie",
      radius: "55%",
      center: ["50%", "60%"],
      data: [
        { value: 335, name: "Direct" },
        { value: 310, name: "Email" },
        { value: 234, name: "Ad Networks" },
        { value: 135, name: "Video Ads" },
        { value: 1548, name: "Search Engines" },
      ],
      emphasis: {
        itemStyle: {
          shadowBlur: 10,
          shadowOffsetX: 0,
          shadowColor: "rgba(0, 0, 0, 0.5)",
        },
      },
    },
  ],
});
</script>

<template>
  <h2>数据图表</h2>

  <ReportSection>
    <template #icon>
      <IconHub />
    </template>
    <template #heading>局域网络拓扑图</template>
    <div>
      <p>123</p>
      <v-chart class="chart" :option="option" autoresize />
    </div>
  </ReportSection>

  <ReportSection>
    <template #icon>
      <IconRouter />
    </template>
    <template #heading>交换机工作状态数据</template>

    Get official tools and libraries for your project:
    <a target="_blank" href="https://pinia.vuejs.org/">Pinia</a>,
    <a target="_blank" href="https://router.vuejs.org/">Vue Router</a>,
    <a target="_blank" href="https://test-utils.vuejs.org/">Vue Test Utils</a>,
    and
    <a target="_blank" href="https://github.com/vuejs/devtools">Vue Dev Tools</a
    >. If you need more resources, we suggest paying
    <a target="_blank" href="https://github.com/vuejs/awesome-vue"
      >Awesome Vue</a
    >
    a visit.
  </ReportSection>

  <ReportSection>
    <template #icon>
      <IconEthernet />
    </template>
    <template #heading>交换机网络接口数据</template>

    Got stuck? Ask your question on
    <a target="_blank" href="https://chat.vuejs.org">Vue Land</a>, our official
    Discord server, or
    <a target="_blank" href="https://stackoverflow.com/questions/tagged/vue.js"
      >StackOverflow</a
    >. You should also subscribe to
    <a target="_blank" href="https://news.vuejs.org">our mailing list</a> and
    follow the official
    <a target="_blank" href="https://twitter.com/vuejs">@vuejs</a>
    twitter account for latest news in the Vue world.
  </ReportSection>

  <ReportSection>
    <template #icon>
      <IconComputer />
    </template>
    <template #heading>网络设备工作状态数据</template>

    As an independent project, Vue relies on community backing for its
    sustainability. You can help us by
    <a target="_blank" href="https://vuejs.org/sponsor/">becoming a sponsor</a>.
  </ReportSection>

  <ReportSection>
    <template #icon>
      <IconChart />
    </template>
    <template #heading>网络设备通信流量数据</template>

    As an independent project, Vue relies on community backing for its
    sustainability. You can help us by
    <a target="_blank" href="https://vuejs.org/sponsor/">becoming a sponsor</a>.
  </ReportSection>
</template>

<style scoped>
.chart {
  height: 20rem;
}
</style>
