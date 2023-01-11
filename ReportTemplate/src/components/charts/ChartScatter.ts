import type { EChartsOption } from "echarts";
import moment from "moment";
import { GetColor, GetLabelColor } from "../colors/ColorImpact";

const startDate = new Date().getTime();
const data = [
  Array.from({ length: 24 * 60 }, (x, i) => new Date(startDate + i * 60000)),
  Array.from({ length: 24 * 60 }, (x, i) =>
    Math.round(3 * Math.pow(Math.random(), (i % 3) + 1))
  ),
];

export default {
  title: {
    text: "网络状态图",
  },
  tooltip: {
    trigger: "axis",
    axisPointer: {
      axis: "x",
    },
    formatter: (args: unknown) => {
      const [{ data, marker }] = args as [
        {
          data: [x: number, y: number];
          marker: string;
        }
      ];
      return `时间：${moment(data[0]).format(
        "MM-DD HH:mm"
      )}<br>状态：${marker}${["无数据", "正常", "警告", "故障"][data[1]]}`;
    },
  },
  xAxis: {
    type: "time",
    axisLabel: {
      formatter: (value: unknown) => moment(value as Date).format("HH:mm"),
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
          color: GetLabelColor("idle"),
        },
      },
      {
        value: "正常",
        textStyle: {
          color: GetLabelColor("normal"),
        },
      },
      {
        value: "警告",
        textStyle: {
          color: GetLabelColor("warning"),
        },
      },
      {
        value: "故障",
        textStyle: {
          color: GetLabelColor("error"),
        },
      },
    ],
  },
  dataset: {
    dimensions: [
      { name: "time", type: "time" },
      { name: "value", type: "int" },
    ],
    source: data,
  },
  visualMap: {
    show: false,
    pieces: [
      {
        min: -1,
        max: 0.5,
        label: "idle",
        color: GetColor("idle"),
      },
      {
        min: 0.5,
        max: 1.5,
        label: "normal",
        color: GetColor("normal"),
      },
      {
        min: 1.5,
        max: 2.5,
        label: "warning",
        color: GetColor("warning"),
      },
      {
        min: 2.5,
        max: 3.5,
        label: "error",
        color: GetColor("error"),
      },
    ],
    outOfRange: {
      color: GetColor("fatal"),
    },
  },
  series: [
    {
      type: "scatter",
      encode: {
        x: "time",
        y: "value",
      },
      seriesLayoutBy: "row",
      animation: false,
      symbolSize: [1, 32],
      symbol: "rect",
      large: true,
    },
  ],
} satisfies EChartsOption;
