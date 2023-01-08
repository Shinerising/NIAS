import type { EChartsOption } from "echarts";
import moment from "moment";

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

export default {
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
} satisfies EChartsOption;
