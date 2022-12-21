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
  SVGRenderer,
  PieChart,
  BarChart,
  LineChart,
  GraphChart,
  HeatmapChart,
  GridComponent,
  TitleComponent,
  ToolboxComponent,
  TooltipComponent,
  LegendComponent,
  VisualMapComponent,
]);

provide(THEME_KEY, "light");

window.addEventListener("beforeprint", () => {
  [].forEach.call(
    document.querySelectorAll(".chart-wrapper"),
    (element: HTMLElement) => {
      element.style.width = "896px";
    }
  );
});
window.addEventListener("afterprint", () => {
  [].forEach.call(
    document.querySelectorAll(".chart-wrapper"),
    (element: HTMLElement) => {
      element.style.width = "";
    }
  );
});

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
  const data = [
    ["2000-06-05", 116],
    ["2000-06-06", 129],
    ["2000-06-07", 135],
    ["2000-06-08", 86],
    ["2000-06-09", 73],
    ["2000-06-10", 85],
    ["2000-06-11", 73],
    ["2000-06-12", 68],
    ["2000-06-13", 92],
    ["2000-06-14", 130],
    ["2000-06-15", 245],
    ["2000-06-16", 139],
    ["2000-06-17", 115],
    ["2000-06-18", 111],
    ["2000-06-19", 309],
    ["2000-06-20", 206],
    ["2000-06-21", 137],
    ["2000-06-22", 128],
    ["2000-06-23", 85],
    ["2000-06-24", 94],
    ["2000-06-25", 71],
    ["2000-06-26", 106],
    ["2000-06-27", 84],
    ["2000-06-28", 93],
    ["2000-06-29", 85],
    ["2000-06-30", 73],
    ["2000-07-01", 83],
    ["2000-07-02", 125],
    ["2000-07-03", 107],
    ["2000-07-04", 82],
    ["2000-07-05", 44],
    ["2000-07-06", 72],
    ["2000-07-07", 106],
    ["2000-07-08", 107],
    ["2000-07-09", 66],
    ["2000-07-10", 91],
    ["2000-07-11", 92],
    ["2000-07-12", 113],
    ["2000-07-13", 107],
    ["2000-07-14", 131],
    ["2000-07-15", 111],
    ["2000-07-16", 64],
    ["2000-07-17", 69],
    ["2000-07-18", 88],
    ["2000-07-19", 77],
    ["2000-07-20", 83],
    ["2000-07-21", 111],
    ["2000-07-22", 57],
    ["2000-07-23", 55],
    ["2000-07-24", 60],
  ];

  const dateList = data.map(function (item) {
    return item[0];
  });
  const valueList = data.map(function (item) {
    return item[1];
  });

  return ref({
    visualMap: [
      {
        show: false,
        type: "continuous",
        seriesIndex: 0,
        min: 0,
        max: 400,
      },
      {
        show: false,
        type: "continuous",
        seriesIndex: 1,
        dimension: 0,
        min: 0,
        max: dateList.length - 1,
      },
    ],
    title: [
      {
        left: "center",
        text: "Gradient along the y axis",
      },
      {
        top: "55%",
        left: "center",
        text: "Gradient along the x axis",
      },
    ],
    tooltip: {
      trigger: "axis",
    },
    xAxis: [
      {
        data: dateList,
      },
      {
        data: dateList,
        gridIndex: 1,
      },
    ],
    yAxis: [
      {},
      {
        gridIndex: 1,
      },
    ],
    grid: [
      {
        bottom: "60%",
      },
      {
        top: "60%",
      },
    ],
    series: [
      {
        type: "line",
        showSymbol: false,
        data: valueList,
      },
      {
        type: "line",
        showSymbol: false,
        data: valueList,
        xAxisIndex: 1,
        yAxisIndex: 1,
      },
    ],
  });
})();

const option03 = (() => {
  // prettier-ignore
  const hours = [
    '12a', '1a', '2a', '3a', '4a', '5a', '6a',
    '7a', '8a', '9a', '10a', '11a',
    '12p', '1p', '2p', '3p', '4p', '5p',
    '6p', '7p', '8p', '9p', '10p', '11p'
  ];

  // prettier-ignore
  const days = [
    'Saturday', 'Friday', 'Thursday',
    'Wednesday', 'Tuesday', 'Monday', 'Sunday'
  ];

  // prettier-ignore
  const data = [[0, 0, 5], [0, 1, 1], [0, 2, 0], [0, 3, 0], [0, 4, 0], [0, 5, 0], [0, 6, 0], [0, 7, 0], [0, 8, 0], [0, 9, 0], [0, 10, 0], [0, 11, 2], [0, 12, 4], [0, 13, 1], [0, 14, 1], [0, 15, 3], [0, 16, 4], [0, 17, 6], [0, 18, 4], [0, 19, 4], [0, 20, 3], [0, 21, 3], [0, 22, 2], [0, 23, 5], [1, 0, 7], [1, 1, 0], [1, 2, 0], [1, 3, 0], [1, 4, 0], [1, 5, 0], [1, 6, 0], [1, 7, 0], [1, 8, 0], [1, 9, 0], [1, 10, 5], [1, 11, 2], [1, 12, 2], [1, 13, 6], [1, 14, 9], [1, 15, 11], [1, 16, 6], [1, 17, 7], [1, 18, 8], [1, 19, 12], [1, 20, 5], [1, 21, 5], [1, 22, 7], [1, 23, 2], [2, 0, 1], [2, 1, 1], [2, 2, 0], [2, 3, 0], [2, 4, 0], [2, 5, 0], [2, 6, 0], [2, 7, 0], [2, 8, 0], [2, 9, 0], [2, 10, 3], [2, 11, 2], [2, 12, 1], [2, 13, 9], [2, 14, 8], [2, 15, 10], [2, 16, 6], [2, 17, 5], [2, 18, 5], [2, 19, 5], [2, 20, 7], [2, 21, 4], [2, 22, 2], [2, 23, 4], [3, 0, 7], [3, 1, 3], [3, 2, 0], [3, 3, 0], [3, 4, 0], [3, 5, 0], [3, 6, 0], [3, 7, 0], [3, 8, 1], [3, 9, 0], [3, 10, 5], [3, 11, 4], [3, 12, 7], [3, 13, 14], [3, 14, 13], [3, 15, 12], [3, 16, 9], [3, 17, 5], [3, 18, 5], [3, 19, 10], [3, 20, 6], [3, 21, 4], [3, 22, 4], [3, 23, 1], [4, 0, 1], [4, 1, 3], [4, 2, 0], [4, 3, 0], [4, 4, 0], [4, 5, 1], [4, 6, 0], [4, 7, 0], [4, 8, 0], [4, 9, 2], [4, 10, 4], [4, 11, 4], [4, 12, 2], [4, 13, 4], [4, 14, 4], [4, 15, 14], [4, 16, 12], [4, 17, 1], [4, 18, 8], [4, 19, 5], [4, 20, 3], [4, 21, 7], [4, 22, 3], [4, 23, 0], [5, 0, 2], [5, 1, 1], [5, 2, 0], [5, 3, 3], [5, 4, 0], [5, 5, 0], [5, 6, 0], [5, 7, 0], [5, 8, 2], [5, 9, 0], [5, 10, 4], [5, 11, 1], [5, 12, 5], [5, 13, 10], [5, 14, 5], [5, 15, 7], [5, 16, 11], [5, 17, 6], [5, 18, 0], [5, 19, 5], [5, 20, 3], [5, 21, 4], [5, 22, 2], [5, 23, 0], [6, 0, 1], [6, 1, 0], [6, 2, 0], [6, 3, 0], [6, 4, 0], [6, 5, 0], [6, 6, 0], [6, 7, 0], [6, 8, 0], [6, 9, 0], [6, 10, 1], [6, 11, 0], [6, 12, 2], [6, 13, 1], [6, 14, 3], [6, 15, 4], [6, 16, 0], [6, 17, 0], [6, 18, 0], [6, 19, 0], [6, 20, 1], [6, 21, 2], [6, 22, 2], [6, 23, 6]]
    .map(function (item) {
      return [item[1], item[0], item[2] || '-'];
    });

  const option = {
    tooltip: {
      position: "top",
    },
    grid: {
      height: "50%",
      top: "10%",
    },
    xAxis: {
      type: "category",
      data: hours,
      splitArea: {
        show: true,
      },
    },
    yAxis: {
      type: "category",
      data: days,
      splitArea: {
        show: true,
      },
    },
    visualMap: {
      min: 0,
      max: 10,
      calculable: true,
      orient: "horizontal",
      left: "center",
      bottom: "15%",
    },
    series: [
      {
        name: "Punch Card",
        type: "heatmap",
        data: data,
        label: {
          show: true,
        },
        emphasis: {
          itemStyle: {
            shadowBlur: 10,
            shadowColor: "rgba(0, 0, 0, 0.5)",
          },
        },
      },
    ],
  };
  return ref(option);
})();

const option04 = (() => {
  const option = {
    title: {
      text: "Basic Graph",
    },
    tooltip: {},
    series: [
      {
        type: "graph",
        layout: "none",
        symbolSize: 50,
        roam: true,
        label: {
          show: true,
        },
        edgeSymbol: ["circle", "arrow"],
        edgeSymbolSize: [4, 10],
        edgeLabel: {
          fontSize: 20,
        },
        data: [
          {
            name: "Node 1",
            x: 300,
            y: 300,
          },
          {
            name: "Node 2",
            x: 800,
            y: 300,
          },
          {
            name: "Node 3",
            x: 550,
            y: 100,
          },
          {
            name: "Node 4",
            x: 550,
            y: 500,
          },
        ],
        // links: [],
        links: [
          {
            source: 0,
            target: 1,
            symbolSize: [5, 20],
            label: {
              show: true,
            },
            lineStyle: {
              width: 5,
              curveness: 0.2,
            },
          },
          {
            source: "Node 2",
            target: "Node 1",
            label: {
              show: true,
            },
            lineStyle: {
              curveness: 0.2,
            },
          },
          {
            source: "Node 1",
            target: "Node 3",
          },
          {
            source: "Node 2",
            target: "Node 3",
          },
          {
            source: "Node 2",
            target: "Node 4",
          },
          {
            source: "Node 1",
            target: "Node 4",
          },
        ],
        lineStyle: {
          opacity: 0.9,
          width: 2,
          curveness: 0,
        },
      },
    ],
  };

  return ref(option);
})();

const option05 = (() => {
  const xAxisData = [];
  const data1 = [];
  const data2 = [];
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
      // y: 'bottom',
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
    <div class="chart-wrapper no-break">
      <v-chart class="chart" :option="option04" autoresize />
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
</style>
