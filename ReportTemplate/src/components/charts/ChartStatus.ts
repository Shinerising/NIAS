import type { EChartsOption } from "echarts";
import moment from "moment";

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

export default {
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
  series: [
    {
      type: "line",
      name: "CPU占用率",
      data: cpu,
      animation: false,
      showSymbol: false,
    },
    {
      type: "line",
      name: "内存占用率",
      yAxisIndex: 1,
      data: memory,
      animation: false,
      showSymbol: false,
    },
    {
      type: "line",
      name: "设备温度",
      yAxisIndex: 2,
      data: temperature,
      animation: false,
      showSymbol: false,
    },
  ],
} satisfies EChartsOption;
