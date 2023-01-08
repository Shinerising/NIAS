import type { EChartsOption } from "echarts";

const xAxisData: string[] = [];
const data1: number[] = [];
const data2: number[] = [];
for (let i = 0; i < 100; i++) {
  xAxisData.push("A" + i);
  data1.push((Math.sin(i / 5) * (i / 5 - 10) + i / 6) * 5);
  data2.push((Math.cos(i / 5) * (i / 5 - 10) + i / 6) * 5);
}

export default {
  title: {
    text: "Bar Animation Delay",
  },
  legend: {
    data: ["bar", "bar2"],
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
} satisfies EChartsOption;
