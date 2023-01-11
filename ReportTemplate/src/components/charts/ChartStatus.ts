import type { EChartsOption } from "echarts";
import moment from "moment";

const startDate = new Date().getTime();
const data = [
  Array.from({ length: 24 * 60 }, (x, i) => new Date(startDate + i * 60000)),
  Array.from({ length: 24 * 60 }, () => Math.random() * 0.6 + 0.1),
  Array.from({ length: 24 * 60 }, () => Math.random() * 0.3 + 0.2),
  Array.from({ length: 24 * 60 }, () => Math.random() * 40 + 20),
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
        },
        {
          marker: string;
          seriesName: string;
        }
      ];
      return `时间：${moment(data[0].data[0]).format("MM-DD HH:mm")}<br>${
        data[0].marker
      }${data[0].seriesName}：${(data[0].data[1] * 100).toFixed(0) + "%"}<br>${
        data[1].marker
      }${data[1].seriesName}：${(data[0].data[2] * 100).toFixed(0) + "%"}<br>${
        data[2].marker
      }${data[2].seriesName}：${data[0].data[3].toFixed(2) + "℃"}`;
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
  dataset: {
    dimensions: [
      { name: "time", type: "time" },
      { name: "cpu", type: "int" },
      { name: "memory", type: "int" },
      { name: "temperature", type: "int" },
    ],
    source: data,
  },
  series: [
    {
      type: "line",
      name: "CPU占用率",
      encode: {
        x: "time",
        y: "cpu",
      },
      seriesLayoutBy: "row",
      animation: false,
      showSymbol: false,
      sampling: "average",
    },
    {
      type: "line",
      name: "内存占用率",
      yAxisIndex: 1,
      encode: {
        x: "time",
        y: "memory",
      },
      seriesLayoutBy: "row",
      animation: false,
      showSymbol: false,
      sampling: "average",
    },
    {
      type: "line",
      name: "设备温度",
      yAxisIndex: 2,
      encode: {
        x: "time",
        y: "temperature",
      },
      seriesLayoutBy: "row",
      animation: false,
      showSymbol: false,
      sampling: "average",
    },
  ],
} satisfies EChartsOption;
