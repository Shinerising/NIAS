import type { EChartsOption } from "echarts";
import { ImageComputer, ImageSwitch } from "../images/ImageResource";
import { GetColor } from "../colors/ColorImpact";

const nodes = [
  {
    name: "Switch A",
    category: 0,
  },
  {
    name: "Switch B",
    category: 0,
  },
  {
    name: "Device 1",
    category: 1,
  },
  {
    name: "Device 2",
    category: 1,
  },
  {
    name: "Device 3",
    category: 1,
  },
  {
    name: "Device 4",
    category: 1,
  },
  {
    name: "Device 5",
    category: 1,
  },
];
const links = [
  {
    source: 0,
    target: 1,
    value: 1,
    lineStyle: {
      color: GetColor("idle"),
    },
  },
  {
    source: 0,
    target: 2,
    value: 2,
    lineStyle: {
      color: GetColor("normal"),
    },
  },
  {
    source: 0,
    target: 3,
    value: 4,
    lineStyle: {
      color: GetColor("error"),
    },
  },
  {
    source: 1,
    target: 4,
    value: 3,
    lineStyle: {
      color: GetColor("warning"),
    },
  },
  {
    source: 0,
    target: 4,
    value: 2,
    lineStyle: {
      color: GetColor("normal"),
    },
  },
  {
    source: 0,
    target: 5,
    value: 0,
    lineStyle: {
      color: GetColor("unknown"),
    },
  },
  {
    source: 1,
    target: 6,
    value: 0,
    lineStyle: {
      color: GetColor("unknown"),
    },
  },
];

export default {
  title: {
    text: "网络拓扑图",
  },
  tooltip: {
    formatter: (args: unknown) => {
      const { dataType, data, marker, borderColor } = args as
        | {
            dataType: "node";
            borderColor: string;
            data: { name: string };
            marker: string;
          }
        | {
            dataType: "edge";
            borderColor: string;
            data: { source: number; target: number; value: number };
            marker: string;
          };
      return dataType === "node"
        ? data.name
        : data.value === 0
        ? ""
        : `连接：${nodes[data.source].name} ― ${
            nodes[data.target].name
          }<br>状态：${marker.replace("transparent", borderColor)}${
            ["无数据", "默认", "正常", "警告", "故障"][data.value]
          }`;
    },
  },
  series: [
    {
      type: "graph",
      layout: "force",
      force: {
        initLayout: "circular",
        repulsion: 400,
        layoutAnimation: false,
        edgeLength: 100,
        friction: 0.6,
      },
      animation: false,
      symbol: "roundRect",
      symbolSize: 60,
      roam: false,
      label: {
        show: true,
        position: "bottom",
      },
      categories: [
        {
          name: "switch",
          symbol: "image://" + ImageSwitch,
        },
        {
          name: "device",
          symbol: "image://" + ImageComputer,
        },
      ],
      data: nodes,
      links: links,
      lineStyle: {
        opacity: 1,
        width: 4,
        color: GetColor("unknown"),
      },
    },
  ],
} satisfies EChartsOption;
