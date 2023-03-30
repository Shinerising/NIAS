<script setup lang="ts">
import { ref, provide, type ComponentPublicInstance } from "vue";
import { use, type ECharts } from "echarts/core";
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

import ChartStats from "./charts/ChartStats";
import ChartSwitch from "./charts/ChartSwitch";
import ChartPort from "./charts/ChartPort";
import ChartGraph, { updateLinks } from "./charts/ChartGraph";
import ChartState from "././charts/ChartState";
import ChartStatus from "./charts/ChartStatus";

import { PrintStore } from "../stores/PrintStore";

const props = defineProps<{
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

const optionStats = ChartStats(
  "总体统计数据",
  props.data.Switch,
  props.data.Host
);
const optionGraph = ref(
  ChartGraph(
    "网络拓扑图",
    props.data.SwitchInfo,
    props.data.HostInfo,
    props.data.Connection
  )
);
const optionState = ChartState("网络状态图", props.data.Connection);
const optionSwitchList = props.data.Switch?.map((item, index) =>
  ChartSwitch(
    "工作状态曲线",
    props.data.SwitchInfo ? props.data.SwitchInfo[index] : null,
    item
  )
);
const optionSwitchPort = props.data.Switch?.map((item, index) =>
  ChartPort(
    "网口流量变化",
    props.data.SwitchInfo ? props.data.SwitchInfo[index] : null,
    item
  )
);
const optionHostList = props.data.Host?.map((item, index) =>
  ChartStatus(
    "工作状态曲线",
    props.data.HostInfo ? props.data.HostInfo[index] : null,
    item
  )
);

const refList: ECharts[] = [];

const setRef = (
  element: Element | ComponentPublicInstance | ECharts | null
) => {
  refList.push(element as ECharts);
};

PrintStore().AddPrintCallback((isPre: boolean) => {
  refList.forEach((item) => item?.resize());
  optionGraph.value.series[0].zoom = isPre ? 0.6 : 1;
});

const updateAxisPointer = (event: { dataIndex: number }) => {
  const index = event.dataIndex;
  const links = updateLinks(
    index,
    props.data.SwitchInfo?.length ?? 0,
    props.data.Connection
  );

  optionGraph.value.series[0].force.friction = 0;
  optionGraph.value.series[0].links = links;
};
</script>

<template>
  <h2>数据图表</h2>

  <ReportSection>
    <template #icon>
      <IconComputer />
    </template>
    <template #heading>网络设备工作状态总体统计数据</template>
    <div class="chart-wrapper no-break large">
      <v-chart class="chart" :option="optionStats" :ref="setRef" autoresize />
    </div>
  </ReportSection>

  <ReportSection>
    <template #icon>
      <IconHub />
    </template>
    <template #heading>局域网络拓扑图</template>
    <div class="chart-wrapper no-break two-column large">
      <div>
        <v-chart
          class="chart"
          :option="optionState"
          :ref="setRef"
          @updateAxisPointer="updateAxisPointer"
          autoresize
        />
      </div>
      <div>
        <v-chart class="chart" :option="optionGraph" :ref="setRef" autoresize />
      </div>
    </div>
  </ReportSection>

  <ReportSection>
    <template #icon>
      <IconRouter />
    </template>
    <template #heading>交换机工作状态数据</template>
    <template v-for="(option, i) in optionSwitchList" :key="i">
      <div class="chart-wrapper no-break">
        <v-chart class="chart" :option="option" :ref="setRef" autoresize />
      </div>
    </template>
  </ReportSection>

  <ReportSection>
    <template #icon>
      <IconEthernet />
    </template>
    <template #heading>交换机网络接口数据</template>
    <template v-for="(option, i) in optionSwitchPort" :key="i">
      <div class="chart-wrapper no-break">
        <v-chart class="chart" :option="option" :ref="setRef" autoresize />
      </div>
    </template>
  </ReportSection>

  <ReportSection>
    <template #icon>
      <IconChart />
    </template>
    <template #heading>网络设备通信流量数据</template>
    <template v-for="(option, i) in optionHostList" :key="i">
      <div class="chart-wrapper no-break">
        <v-chart class="chart" :option="option" :ref="setRef" autoresize />
      </div>
    </template>
  </ReportSection>
</template>

<style scoped>
.chart-wrapper {
  margin: 0 auto;
  min-height: 16rem;
  aspect-ratio: 6 / 2;
}

.chart-wrapper.large {
  aspect-ratio: 5 / 2;
}

.chart-wrapper.two-column {
  display: flex;
}

.chart-wrapper.two-column > * {
  width: 50%;
  display: flex;
  align-items: center;
  justify-content: center;
}
</style>
