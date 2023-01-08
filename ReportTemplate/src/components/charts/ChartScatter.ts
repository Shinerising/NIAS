import type { EChartsOption } from "echarts";
import moment from "moment";

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

export default {
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
      formatter: (value: unknown) =>
        moment(value as Date).format("MM-DD HH:mm"),
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
} satisfies EChartsOption;
