import type { EChartsOption } from "echarts";
import { ImageComputer, ImageSwitch } from "../images/ImageResource";

export default {
  title: {
    text: "网络拓扑图",
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
      data: [
        {
          name: "Switch A",
          id: "1",
          category: 0,
        },
        {
          name: "Switch B",
          id: "2",
          category: 0,
        },
        {
          name: "Device 1",
          id: "3",
          category: 1,
        },
        {
          name: "Device 2",
          id: "4",
          category: 1,
        },
        {
          name: "Device 3",
          id: "5",
          category: 1,
        },
        {
          name: "Device 4",
          id: "6",
          category: 1,
        },
        {
          name: "Device 5",
          id: "7",
          category: 1,
        },
      ],
      links: [
        {
          source: "1",
          target: "2",
          lineStyle: {
            color: "lightgreen",
          },
        },
        {
          source: "1",
          target: "3",
          lineStyle: {
            color: "lightgreen",
          },
        },
        {
          source: "1",
          target: "4",
          lineStyle: {
            color: "orangered",
          },
        },
        {
          source: "2",
          target: "5",
          lineStyle: {
            color: "lightgreen",
          },
        },
        {
          source: "1",
          target: "5",
          lineStyle: {
            color: "orange",
          },
        },
        {
          source: "1",
          target: "6",
        },
        {
          source: "2",
          target: "7",
        },
      ],
      lineStyle: {
        opacity: 1,
        width: 4,
      },
    },
  ],
} satisfies EChartsOption;
