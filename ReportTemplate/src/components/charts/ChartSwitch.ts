import type { EChartsOption } from "echarts";
import { format, fromUnixTime } from "date-fns";
import type {
  ReportSwitchInfo,
  ReportSwitch,
} from "../interface/ReportData.interface";

export default (
  title: string,
  info: ReportSwitchInfo | null,
  list: ReportSwitch
) => {
  const data = [list.Time, list.CPU, list.REM, list.TEM];

  return {
    title: {
      text: `${title} • ${info?.Name ?? "未知交换机"}`,
    },
    tooltip: {
      trigger: "axis",
      axisPointer: {
        axis: "x",
      },
      formatter: (args: unknown) => {
        const data = args as [
          {
            data: [
              time: number,
              value0: number,
              value1: number,
              value2: number
            ];
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
        return `时间：${format(
          fromUnixTime(data[0].data[0]),
          "MM-dd HH:mm"
        )}<br>${data[0].marker}${data[0].seriesName}：${
          (data[0].data[1] * 100).toFixed(0) + "%"
        }<br>${data[1].marker}${data[1].seriesName}：${
          (data[0].data[2] * 100).toFixed(0) + "%"
        }<br>${data[2].marker}${data[2].seriesName}：${
          data[0].data[3].toFixed(2) + "℃"
        }`;
      },
    },
    legend: {
      show: true,
    },
    xAxis: {
      type: "time",
      axisLabel: {
        formatter: (value: unknown) =>
          format(fromUnixTime(value as number), "MM-dd HH:mm"),
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
        max: 100,
        min: 0,
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
    dataZoom: {
      show: true,
      realtime: false,
      labelFormatter: (value: unknown) =>
        format(fromUnixTime(value as number), "MM-dd HH:mm"),
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
};
