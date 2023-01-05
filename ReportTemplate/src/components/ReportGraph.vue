<script setup lang="ts">
import moment from "moment";
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
  ToolboxComponent,
  TooltipComponent,
  LegendComponent,
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

import { ImageComputer, ImageSwitch } from "./images/ImageResource";

defineProps<{
  data: ReportData;
}>();

/**
 * Switch Status: Line Chart
 * Switch Port: Heatmap Chart
 * Device Connection: Relationship Chart
 */

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
  ToolboxComponent,
  TooltipComponent,
  VisualMapComponent,
]);

provide(THEME_KEY, "light");

const option01 = (() => {
  return ref({
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
})();

const option02 = (() => {
  const startDate = new Date().getTime();
  const cpu = Array.from({ length: 24 * 60 }, (x, i) => [
    new Date(startDate + 60000 * i),
    Math.random() * 0.6 + 0.1,
  ]);
  const memory = Array.from({ length: 24 * 60 }, (x, i) => [
    new Date(startDate + 60000 * i),
    Math.random() * 0.3 + 0.2,
  ]);
  const temperature = Array.from({ length: 24 * 60 }, (x, i) => [
    new Date(startDate + 60000 * i),
    Math.random() * 40 + 20,
  ]);

  return ref({
    title: {
      text: "设备工作状态曲线",
    },
    tooltip: {
      trigger: "axis",
      axisPointer: {
        axis: "x",
      },
    },
    legend: {
      show: true,
    },
    xAxis: {
      type: "time",
      axisLabel: {
        formatter: (value: Date) => moment(value).format("MM-DD HH:mm"),
      },
    },
    yAxis: [
      {
        name: "CPU&内存占用率",
        type: "value",
        max: 1,
        min: 0,
        axisLabel: {
          formatter: (value: number) => (value * 100).toFixed(0) + "%",
        },
      },
      {
        name: "",
        type: "value",
        alignTicks: true,
        position: "left",
        max: 1,
        min: 0,
        show: false,
      },
      {
        name: "设备温度",
        type: "value",
        alignTicks: true,
        max: 120,
        min: 20,
        axisLabel: {
          formatter: (value: number) => value.toFixed(0) + "℃",
        },
      },
    ],
    series: [
      {
        type: "line",
        name: "CPU占用率",
        data: cpu,
        large: true,
        animation: false,
        showSymbol: false,
      },
      {
        type: "line",
        name: "内存占用率",
        yAxisIndex: 1,
        data: memory,
        large: true,
        animation: false,
        showSymbol: false,
      },
      {
        type: "line",
        name: "设备温度",
        yAxisIndex: 2,
        data: temperature,
        large: true,
        animation: false,
        showSymbol: false,
      },
    ],
  });
})();

const option03 = (() => {
  const startDate = new Date().getTime();

  const data = Array.prototype.concat(
    ...Array.from({ length: 24 }, (x, i) =>
      Array.from({ length: 24 * 60 }, (_x, _i) => [
        startDate + 60000 * _i,
        i,
        Math.random() * 1024 || "-",
      ])
    )
  );

  const option = {
    title: {
      text: "网口流量变化",
    },
    tooltip: {
      position: "top",
      formatter: (args: unknown) => {
        const { data } = args as {
          data: [x: string, y: number, value: number];
        };
        return `时间：${moment(data[0]).format("MM-DD HH:mm:ss")}<br>网口：${
          data[1]
        }<br>流量：${data[2].toFixed(2)}mbps`;
      },
    },
    grid: {
      height: "50%",
      top: "10%",
    },
    xAxis: {
      type: "category",
      splitArea: {
        show: true,
      },
      axisLabel: {
        formatter: (value: string) =>
          moment(parseInt(value)).format("MM-DD HH:mm"),
      },
    },
    yAxis: {
      type: "category",
    },
    visualMap: {
      min: 0,
      max: 1024,
      orient: "horizontal",
      left: "center",
      bottom: "15%",
      calculable: true,
      realtime: false,
      inRange: {
        color: ["white", "lightgreen", "orange", "red"],
      },
    },
    series: [
      {
        type: "heatmap",
        data: data,
        label: {
          show: false,
        },
        progressive: 10240000,
        animation: false,
      },
    ],
  };
  return ref(option);
})();

const [option04, option06] = (() => {
  const startDate = new Date();
  const generateData = () => {
    const data = [];
    for (let i = 0; i < 24 * 60; i++) {
      const l = Math.random() * 20;
      data.push([
        new Date(startDate.getTime() + i * 60000),
        l > 18 ? 3 : l > 16 ? 2 : l > 2 ? 1 : 0,
      ]);
    }
    return data;
  };
  const option01 = {
    title: {
      text: "网络拓扑图",
    },
    series: [
      {
        type: "graph",
        layout: "force",
        force: {
          initLayout: "circular",
          repulsion: 400,
          layoutAnimation: false,
          edgeLength: 100,
          friction: 0.6,
        },
        animation: false,
        symbol: "roundRect",
        symbolSize: 60,
        roam: false,
        label: {
          show: true,
          position: "bottom",
        },
        categories: [
          {
            name: "switch",
            symbol: "image://" + ImageSwitch,
          },
          {
            name: "device",
            symbol: "image://" + ImageComputer,
          },
        ],
        data: [
          {
            name: "Switch A",
            id: "1",
            category: 0,
          },
          {
            name: "Switch B",
            id: "2",
            category: 0,
          },
          {
            name: "Device 1",
            id: "3",
            category: 1,
          },
          {
            name: "Device 2",
            id: "4",
            category: 1,
          },
          {
            name: "Device 3",
            id: "5",
            category: 1,
          },
          {
            name: "Device 4",
            id: "6",
            category: 1,
          },
          {
            name: "Device 5",
            id: "7",
            category: 1,
          },
        ],
        links: [
          {
            source: "1",
            target: "2",
            lineStyle: {
              color: "lightgreen",
            },
          },
          {
            source: "1",
            target: "3",
            lineStyle: {
              color: "lightgreen",
            },
          },
          {
            source: "1",
            target: "4",
            lineStyle: {
              color: "orangered",
            },
          },
          {
            source: "2",
            target: "5",
            lineStyle: {
              color: "lightgreen",
            },
          },
          {
            source: "1",
            target: "5",
            lineStyle: {
              color: "orange",
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
        ],
        lineStyle: {
          opacity: 1,
          width: 4,
        },
      },
    ],
  };

  const option02 = {
    title: {
      text: "网络状态图",
    },
    tooltip: {
      trigger: "axis",
      axisPointer: {
        axis: "x",
      },
    },
    xAxis: {
      type: "time",
      axisLabel: {
        formatter: (value: Date) => moment(value).format("MM-DD HH:mm"),
      },
    },
    yAxis: {
      type: "category",
      splitLine: {
        show: false,
      },
      axisTick: {
        show: false,
      },
      axisLine: {
        show: false,
      },
      data: [
        {
          value: "无数据",
          textStyle: {
            color: "gray",
          },
        },
        {
          value: "正常",
          textStyle: {
            color: "green",
          },
        },
        {
          value: "警告",
          textStyle: {
            color: "orange",
          },
        },
        {
          value: "故障",
          textStyle: {
            color: "red",
          },
        },
      ],
    },
    visualMap: {
      show: false,
      pieces: [
        {
          min: -1,
          max: 0.5,
          label: "idle",
          color: "lightgray",
        },
        {
          min: 0.5,
          max: 1.5,
          label: "normal",
          color: "lightgreen",
        },
        {
          min: 1.5,
          max: 2.5,
          label: "warning",
          color: "orange",
        },
        {
          min: 2.5,
          max: 3.5,
          label: "error",
          color: "orangered",
        },
      ],
      outOfRange: {
        color: "red",
      },
    },
    series: [
      {
        data: generateData(),
        type: "scatter",
        animation: false,
        symbolSize: [1, 32],
        symbol: "rect",
        large: true,
      },
    ],
  };

  return [ref(option01), ref(option02)];
})();

const option05 = (() => {
  const xAxisData: string[] = [];
  const data1: number[] = [];
  const data2: number[] = [];
  for (let i = 0; i < 100; i++) {
    xAxisData.push("A" + i);
    data1.push((Math.sin(i / 5) * (i / 5 - 10) + i / 6) * 5);
    data2.push((Math.cos(i / 5) * (i / 5 - 10) + i / 6) * 5);
  }

  const option = {
    title: {
      text: "Bar Animation Delay",
    },
    legend: {
      data: ["bar", "bar2"],
    },
    toolbox: {
      feature: {
        magicType: {
          type: ["stack"],
        },
        dataView: {},
        saveAsImage: {
          pixelRatio: 2,
        },
      },
    },
    tooltip: {},
    xAxis: {
      data: xAxisData,
      splitLine: {
        show: false,
      },
    },
    yAxis: {},
    series: [
      {
        name: "bar",
        type: "bar",
        data: data1,
        emphasis: {
          focus: "series",
        },
        animationDelay: function (idx: number) {
          return idx * 10;
        },
      },
      {
        name: "bar2",
        type: "bar",
        data: data2,
        emphasis: {
          focus: "series",
        },
        animationDelay: function (idx: number) {
          return idx * 10 + 100;
        },
      },
    ],
    animationDelayUpdate: function (idx: number) {
      return idx * 5;
    },
  };
  return ref(option);
})();

const updateAxisPointer = (event: { axesInfo: { value: number }[] }) => {
  const xAxisInfo = event.axesInfo[0];
  if (xAxisInfo) {
    //const dimension = xAxisInfo.value + 1;
    const impactColor = (index: number) => {
      return ["lightgray", "lightgreen", "orange", "orangered"][index];
    };
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
          color: impactColor(Math.floor(Math.random() * 4)),
        },
      },
      {
        source: "1",
        target: "3",
        lineStyle: {
          color: impactColor(Math.floor(Math.random() * 4)),
        },
      },
      {
        source: "1",
        target: "4",
        lineStyle: {
          color: impactColor(Math.floor(Math.random() * 4)),
        },
      },
      {
        source: "2",
        target: "5",
        lineStyle: {
          color: impactColor(Math.floor(Math.random() * 4)),
        },
      },
      {
        source: "1",
        target: "5",
        lineStyle: {
          color: impactColor(Math.floor(Math.random() * 4)),
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
    <div class="chart-wrapper no-break">
      <v-chart class="chart" :option="option01" autoresize />
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
    <template #heading>网络设备工作状态数据</template>
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
