<script setup lang="ts">
import { ref, provide } from "vue";
import { use } from "echarts/core";
import { SVGRenderer } from "echarts/renderers";
import {
  PieChart,
  BarChart,
  LineChart,
  GraphChart,
  HeatmapChart,
  ScatterChart,
} from "echarts/charts";
import {
  GridComponent,
  TitleComponent,
  TooltipComponent,
  LegendComponent,
  DatasetComponent,
  MarkAreaComponent,
  VisualMapComponent,
} from "echarts/components";
import VChart, { THEME_KEY } from "vue-echarts";

import type { ReportData } from "./interface/ReportData.interface";
import ReportSection from "./ReportSection.vue";

import IconComputer from "./icons/IconComputer.vue";
import IconEthernet from "./icons/IconEthernet.vue";
import IconChart from "./icons/IconChart.vue";
import IconRouter from "./icons/IconRouter.vue";
import IconHub from "./icons/IconHub.vue";

import ChartTraffic from "./charts/ChartTraffic";
import ChartStatus from "./charts/ChartStatus";
import ChartPort from "./charts/ChartPort";
import ChartGraph from "./charts/ChartGraph";
import ChartScatter from "./charts/ChartScatter";
import ChartBar from "./charts/ChartBar";

import { GetColor } from "./colors/ColorImpact";

defineProps<{
  data: ReportData;
}>();

use([
  SVGRenderer,
  PieChart,
  BarChart,
  LineChart,
  GraphChart,
  ScatterChart,
  HeatmapChart,
  GridComponent,
  TitleComponent,
  LegendComponent,
  DatasetComponent,
  TooltipComponent,
  MarkAreaComponent,
  VisualMapComponent,
]);

provide(THEME_KEY, "light");

const option01 = ref(ChartTraffic);
const option02 = ref(ChartStatus);
const option03 = ref(ChartPort);
const option04 = ref(ChartGraph);
const option06 = ref(ChartScatter);
const option05 = ref(ChartBar);

const updateAxisPointer = (event: { axesInfo: { value: number }[] }) => {
  const xAxisInfo = event.axesInfo[0];
  if (xAxisInfo) {
    option04.value.series[0].force = {
      initLayout: "circular",
      repulsion: 400,
      layoutAnimation: false,
      edgeLength: 100,
      friction: 0,
    };
    option04.value.series[0].links = [
      {
        source: "1",
        target: "2",
        lineStyle: {
          color: GetColor(Math.floor(Math.random() * 4) + 1),
        },
      },
      {
        source: "1",
        target: "3",
        lineStyle: {
          color: GetColor(Math.floor(Math.random() * 4) + 1),
        },
      },
      {
        source: "1",
        target: "4",
        lineStyle: {
          color: GetColor(Math.floor(Math.random() * 4) + 1),
        },
      },
      {
        source: "2",
        target: "5",
        lineStyle: {
          color: GetColor(Math.floor(Math.random() * 4) + 1),
        },
      },
      {
        source: "1",
        target: "5",
        lineStyle: {
          color: GetColor(Math.floor(Math.random() * 4) + 1),
        },
      },
      {
        source: "1",
        target: "6",
      },
      {
        source: "2",
        target: "7",
      },
    ];
  }
};
</script>

<template>
  <h2>数据图表</h2>

  <ReportSection>
    <template #icon>
      <IconHub />
    </template>
    <template #heading>局域网络拓扑图</template>
    <div class="chart-wrapper no-break two-column">
      <div>
        <v-chart
          class="chart"
          :option="option06"
          @updateAxisPointer="updateAxisPointer"
          autoresize
        />
      </div>
      <div>
        <v-chart class="chart" :option="option04" autoresize />
      </div>
    </div>
  </ReportSection>

  <ReportSection>
    <template #icon>
      <IconRouter />
    </template>
    <template #heading>交换机工作状态数据</template>
    <div class="chart-wrapper no-break">
      <v-chart class="chart" :option="option02" autoresize />
    </div>
  </ReportSection>

  <ReportSection>
    <template #icon>
      <IconEthernet />
    </template>
    <template #heading>交换机网络接口数据</template>
    <div class="chart-wrapper no-break">
      <v-chart class="chart" :option="option03" autoresize />
    </div>
  </ReportSection>

  <ReportSection>
    <template #icon>
      <IconComputer />
    </template>
    <template #heading>网络设备工作状态总体统计数据</template>
    <div class="chart-wrapper no-break">
      <v-chart class="chart" :option="option01" autoresize />
    </div>
  </ReportSection>

  <ReportSection>
    <template #icon>
      <IconChart />
    </template>
    <template #heading>网络设备通信流量数据</template>
    <div class="chart-wrapper no-break">
      <v-chart class="chart" :option="option05" autoresize />
    </div>
  </ReportSection>
</template>

<style scoped>
.chart-wrapper {
  margin: 0 auto;
  min-height: 20rem;
  aspect-ratio: 5 / 2;
}

.chart-wrapper.two-column {
  display: flex;
}

.chart-wrapper.two-column > * {
  flex-grow: 1;
  flex-basis: 0;
  min-width: 0;
}
</style>
