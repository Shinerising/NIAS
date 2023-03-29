import type { EChartsOption } from "echarts";
import type {
  ReportSwitch,
  ReportHost
} from "../interface/ReportData.interface";

export default (
  title: string,
  switchList: ReportSwitch[] | null,
  hostList: ReportHost[] | null
) => {
  let list = (switchList as { State: number[] }[])?.concat(hostList as { State: number[] }[] ?? { State: [] });
  let stateCount: [number, number, number, number, number] = list?.reduce((prev, curr) =>
    curr.State.reduce(
      (_prev, _curr) => {
        _prev[_curr] += 1;
        return prev;
      }
      , [0, 0, 0, 0, 0]
    ), [0, 0, 0, 0, 0]) ?? [0, 0, 0, 0, 0];
  let inSpeedCount: [number, number, number, number] = hostList?.reduce((prev, curr) =>
    curr.InSpeed.reduce(
      (_prev, _curr) => {
        let idx = 0;
        if (_curr < 10240) {
          idx = 0;
        }
        else if (_curr < 512000) {
          idx = 1;
        }
        else if (_curr < 10485760) {
          idx = 2;
        }
        else {
          idx = 3;
        }
        _prev[idx] += 1;
        return prev;
      }
      , [0, 0, 0, 0]
    ), [0, 0, 0, 0]) ?? [0, 0, 0, 0];
  let outSpeedCount: [number, number, number, number] = hostList?.reduce((prev, curr) =>
    curr.OutSpeed.reduce(
      (_prev, _curr) => {
        let idx = 0;
        if (_curr < 10240) {
          idx = 0;
        }
        else if (_curr < 512000) {
          idx = 1;
        }
        else if (_curr < 10485760) {
          idx = 2;
        }
        else {
          idx = 3;
        }
        _prev[idx] += 1;
        return prev;
      }
      , [0, 0, 0, 0]
    ), [0, 0, 0, 0]) ?? [0, 0, 0, 0];
  let speedCount = inSpeedCount.map((v, i) => v + outSpeedCount[i]);
  let latencyCount: [number, number, number, number] = hostList?.reduce((prev, curr) =>
    curr.Latency.reduce(
      (_prev, _curr) => {
        let idx = 0;
        if (_curr < 10) {
          idx = 0;
        }
        else if (_curr < 50) {
          idx = 1;
        }
        else if (_curr < 200) {
          idx = 2;
        }
        else {
          idx = 3;
        }
        _prev[idx] += 1;
        return prev;
      }
      , [0, 0, 0, 0]
    ), [0, 0, 0, 0]) ?? [0, 0, 0, 0];

  return {
    title: [
      {
        text: "设备工作状态统计",
        left: "5%",
      },
      {
        text: "设备网络流量统计",
        left: "35%",
      },
      {
        text: "设备网络延迟统计",
        left: "65%",
      },
    ],
    series: [
      {
        type: "pie",
        radius: "40%",
        center: ["20%", "50%"],
        animation: false,
        data: [
          { value: stateCount[0] || '-', name: "未知状态" },
          { value: stateCount[1] || '-', name: "默认状态" },
          { value: stateCount[2] || '-', name: "正常状态" },
          { value: stateCount[3] || '-', name: "警告状态" },
          { value: stateCount[4] || '-', name: "故障状态" },
        ],
      },
      {
        type: "pie",
        radius: "40%",
        center: ["50%", "50%"],
        animation: false,
        data: [
          { value: speedCount[0] || '-', name: "<10KB/s" },
          { value: speedCount[1] || '-', name: "<500KB/s" },
          { value: speedCount[2] || '-', name: "<10MB/s" },
          { value: speedCount[3] || '-', name: "≥10MB/s" },
        ],
      },
      {
        type: "pie",
        radius: "40%",
        center: ["80%", "50%"],
        animation: false,
        data: [
          { value: latencyCount[0] || '-', name: "<10ms" },
          { value: latencyCount[1] || '-', name: "<50ms" },
          { value: latencyCount[2] || '-', name: "<200ms" },
          { value: latencyCount[3] || '-', name: "≥200ms" },
        ],
      },
    ],
  } satisfies EChartsOption;
};
