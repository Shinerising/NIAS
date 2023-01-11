import type { EChartsOption } from "echarts";
import moment from "moment";

const startDate = new Date().getTime();
const data = [
  Array.from({ length: 24 * 60 }, (x, i) => new Date(startDate + i * 60000)),
  Array.from({ length: 24 * 60 }, () => Math.random() * 50 + 3),
  Array.from({ length: 24 * 60 }, () => Math.random() * 10 + 2),
];

export default {
  title: {
    text: "设备工作状态曲线",
  },
  tooltip: {
    trigger: "axis",
    axisPointer: {
      axis: "x",
    },
    formatter: (args: unknown) => {
      const data = args as [
        {
          data: [time: Date, value0: number, value1: number, value2: number];
          marker: string;
          seriesName: string;
        },
        {
          marker: string;
          seriesName: string;
        }
      ];
      return `时间：${moment(data[0].data[0]).format("MM-DD HH:mm")}<br>${
        data[0].marker
      }${data[0].seriesName}：${data[0].data[1].toFixed(0) + "Mbps"}<br>${
        data[1].marker
      }${data[1].seriesName}：${data[0].data[2].toFixed(0) + "ms"}`;
    },
  },
  legend: {
    show: true,
  },
  xAxis: {
    type: "time",
    axisLabel: {
      formatter: (value: unknown) =>
        moment(value as Date).format("MM-DD HH:mm"),
    },
  },
  yAxis: [
    {
      name: "网络流量",
      type: "value",
      axisLabel: {
        formatter: (value: number) => value.toFixed(0) + "Mbps",
      },
    },
    {
      name: "网络时延",
      type: "value",
      alignTicks: true,
      axisLabel: {
        formatter: (value: number) => value.toFixed(0) + "ms",
      },
    },
  ],
  dataset: {
    dimensions: [
      { name: "time", type: "time" },
      { name: "rate", type: "int" },
      { name: "latency", type: "int" },
    ],
    source: data,
  },
  series: [
    {
      type: "line",
      name: "网络流量",
      encode: {
        x: "time",
        y: "rate",
      },
      seriesLayoutBy: "row",
      animation: false,
      showSymbol: false,
      sampling: "average",
      markArea: {
        itemStyle: {
          color: "rgba(255, 173, 177, 0.4)",
        },
        data: [
          [
            {
              xAxis: new Date(startDate + 42 * 60000),
            },
            {
              xAxis: new Date(startDate + 82 * 60000),
            },
          ],
        ],
      },
    },
    {
      type: "line",
      name: "网络时延",
      yAxisIndex: 1,
      encode: {
        x: "time",
        y: "latency",
      },
      seriesLayoutBy: "row",
      animation: false,
      showSymbol: false,
      sampling: "average",
    },
  ],
} satisfies EChartsOption;
