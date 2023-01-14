import type { EChartsOption } from "echarts";
import { ImageComputer, ImageSwitch } from "../images/ImageResource";
import { GetColor } from "../colors/ColorImpact";
import type {
  ReportConnection,
  ReportHostInfo,
  ReportSwitchInfo,
} from "../interface/ReportData.interface";

export const updateLinks = (
  index: number,
  switchCount: number,
  connection: ReportConnection | null
): {
  source: number;
  target: number;
  value: number;
  lineStyle: {
    color: string;
  };
}[] => {
  const links: {
    source: number;
    target: number;
    value: number;
    lineStyle: {
      color: string;
    };
  }[] = [];
  try {
    const text = connection?.Line[index] ?? "";
    const data: [number, number, number, number][] = JSON.parse(`[${text}]`);
    for (const link of data) {
      if (link.length > 3) {
        links.push({
          source: link[1],
          target: link[0] === 0 ? link[2] : link[2] + switchCount,
          value: link[3],
          lineStyle: {
            color: GetColor(link[3]),
          },
        });
      }
    }
  } catch {
    return [];
  }
  return links;
};

export default (
  title: string,
  switchList: ReportSwitchInfo[] | null,
  hostList: ReportHostInfo[] | null,
  connection: ReportConnection | null
) => {
  const nodes = [
    ...(switchList?.map((item) => ({
      name: item.Name ?? "",
      category: 0,
    })) ?? []),
    ...(hostList?.map((item) => ({
      name: item.Name ?? "",
      category: 1,
    })) ?? []),
  ];
  const links = updateLinks(
    connection ? connection.Line.length - 1 : 0,
    switchList?.length ?? 0,
    connection
  );
  const repulsion = 3200 / nodes.length;
  return {
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
        zoom: 1,
        force: {
          initLayout: "circular",
          repulsion: repulsion,
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
};
