import type { EChartsOption } from "echarts";
import moment from "moment";
import { GetColor } from "../colors/ColorImpact";
import type {
  ReportSwitchInfo,
  ReportSwitch,
} from "../interface/ReportData.interface";

export default (
  title: string,
  info: ReportSwitchInfo | null,
  list: ReportSwitch
) => {
  const time: number[] = [];
  const id: number[] = [];
  const value: number[] = [];
  for (let i = 0; i < list.Time.length; i += 1) {
    const t = list.Time[i];
    for (const name of list.Port[i].split(",")) {
      time.push(t);
      id.push(parseInt(name));
    }
    if (list.PortTotalSpeed) {
      for (const rate of list.PortTotalSpeed[i].split(",")) {
        value.push(parseFloat(rate) / 1048576.0);
      }
    }
  }
  const data = [time, id, value];

  return {
    title: {
      text: `${title} • ${info?.Name ?? "未知交换机"}`,
    },
    tooltip: {
      position: "top",
      formatter: (args: unknown) => {
        const { data, marker } = args as {
          data: [time: number, id: number, value: number];
          marker: string;
        };
        return `时间：${moment.unix(data[0]).format("MM-DD HH:mm")}<br>网口：${data[1]
          }<br>流量：${Number.isNaN(data[2]) ? 0 : data[2].toFixed(2)
          }Mbps${marker}`;
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
        formatter: (value: unknown) =>
          moment.unix(value as number).format("MM-DD HH:mm"),
      },
    },
    yAxis: {
      type: "category",
    },
    dataset: {
      dimensions: [
        { name: "time", type: "time" },
        { name: "id", type: "number" },
        { name: "value", type: "float" },
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
};
