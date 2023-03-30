import type { EChartsOption } from "echarts";
import { format, fromUnixTime } from "date-fns";
import type {
  ReportHostInfo,
  ReportHost,
} from "../interface/ReportData.interface";

export default (
  title: string,
  info: ReportHostInfo | null,
  list: ReportHost
) => {
  const data = [
    list.Time,
    list.InSpeed.map((n) => n / 131072),
    list.OutSpeed.map((n) => n / 131072),
    list.Latency,
  ];
  const mark: [{ xAxis: number }, { xAxis: number }][] = [];
  for (let i = 0, j = 0, s = 0; i < list.State.length; i += 1) {
    const state = list.State[i];
    if (s === 0 && state > 2) {
      j = i;
      s = 1;
    } else if (s === 1 && state <= 2) {
      s = 0;
      mark.push([
        {
          xAxis: list.Time[j],
        },
        {
          xAxis: list.Time[i],
        },
      ]);
    }
  }
  return {
    title: {
      text: `${title} • ${info?.Name ?? "未知计算机"}`,
    },
    tooltip: {
      trigger: "axis",
      axisPointer: {
        axis: "x",
      },
      formatter: (args: unknown) => {
        const data = args as [
          {
            data: [number, number, number, number, number];
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
          data[0].data[1].toFixed(2) + "Mbps"
        }<br>${data[1].marker}${data[1].seriesName}：${
          data[0].data[2].toFixed(2) + "Mbps"
        }<br>${data[2].marker}${data[2].seriesName}：${
          data[0].data[3].toFixed(0) + "ms"
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
        max: 1000,
        min: 0,
        axisLabel: {
          formatter: (value: number) => value.toFixed(0) + "ms",
        },
      },
    ],
    dataset: {
      dimensions: [
        { name: "time", type: "time" },
        { name: "inrate", type: "float" },
        { name: "outrate", type: "float" },
        { name: "latency", type: "int" },
      ],
      source: data,
    },
    series: [
      {
        type: "line",
        name: "传入流量",
        encode: {
          x: "time",
          y: "inrate",
        },
        seriesLayoutBy: "row",
        animation: false,
        showSymbol: false,
        sampling: "average",
        markArea: {
          itemStyle: {
            color: "rgba(255, 173, 177, 0.4)",
          },
          data: mark,
        },
      },
      {
        type: "line",
        name: "传出流量",
        encode: {
          x: "time",
          y: "outrate",
        },
        seriesLayoutBy: "row",
        animation: false,
        showSymbol: false,
        sampling: "average",
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
};
