import type { EChartsOption } from "echarts";
import moment from "moment";
import { GetColor } from "../colors/ColorImpact";

const startDate = new Date().getTime();
const data = [
  Array.prototype.concat(
    ...Array.from({ length: 24 * 60 }, (x, i) =>
      Array.from({ length: 24 }, () => startDate + 60000 * i)
    )
  ),
  Array.prototype.concat(
    ...Array.from({ length: 24 * 60 }, () =>
      Array.from({ length: 24 }, (_x, _i) => _i + 1)
    )
  ),
  Array.prototype.concat(
    ...Array.from({ length: 24 * 60 }, () =>
      Array.from({ length: 24 }, () => Math.pow(Math.random(), 2) * 100 || "-")
    )
  ),
];

export default {
  title: {
    text: "网口流量变化",
  },
  tooltip: {
    position: "top",
    formatter: (args: unknown) => {
      const { data, marker } = args as {
        data: [x: string, y: number, value: number];
        marker: string;
      };
      return `时间：${moment(data[0]).format("MM-DD HH:mm:ss")}<br>网口：${
        data[1]
      }<br>流量：${marker}${data[2].toFixed(2)}Mbps`;
    },
  },
  grid: {
    height: "60%",
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
  dataset: {
    dimensions: [
      { name: "time", type: "time" },
      { name: "id", type: "int" },
      { name: "value", type: "int" },
    ],
    source: data,
  },
  visualMap: {
    type: "piecewise",
    min: 0,
    max: 1024,
    orient: "horizontal",
    left: "center",
    bottom: "10%",
    pieces: [
      {
        min: -1,
        max: 0,
        label: "无数据",
        color: GetColor("unknown"),
      },
      {
        min: 0,
        max: 5,
        label: "正常",
        color: GetColor("normal"),
      },
      {
        min: 1,
        max: 50,
        label: "警告",
        color: GetColor("warning"),
      },
      {
        min: 50,
        max: 1024,
        label: "严重",
        color: GetColor("error"),
      },
    ],
    outOfRange: {
      color: GetColor("fatal"),
    },
  },
  series: [
    {
      type: "heatmap",
      label: {
        show: false,
      },
      encode: {
        x: "time",
        y: "id",
      },
      seriesLayoutBy: "row",
      progressive: 10240000,
      animation: false,
    },
  ],
} satisfies EChartsOption;
