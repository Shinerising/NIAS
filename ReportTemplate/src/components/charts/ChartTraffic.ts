import type { EChartsOption } from "echarts";
import moment from "moment";
import { GetColor } from "../colors/ColorImpact";

export default {
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
        { value: 335, name: "idle" },
        { value: 310, name: "normal" },
        { value: 234, name: "warning" },
        { value: 135, name: "error" },
      ],
    },
    {
      type: "pie",
      radius: "40%",
      center: ["50%", "50%"],
      animation: false,
      data: [
        { value: 335, name: "idle" },
        { value: 310, name: "normal" },
        { value: 234, name: "warning" },
        { value: 135, name: "error" },
      ],
    },
    {
      type: "pie",
      radius: "40%",
      center: ["80%", "50%"],
      animation: false,
      data: [
        { value: 335, name: "idle" },
        { value: 310, name: "normal" },
        { value: 234, name: "warning" },
        { value: 135, name: "error" },
      ],
    },
  ],
} satisfies EChartsOption;
