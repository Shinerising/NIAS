export const ImpactColor = [
  "#FFFFFF",
  "#CFD8DC",
  "#76FF03",
  "#FFC400",
  "#FF3D00",
  "#D50000",
] as const;
export const LabelColor = [
  "#212121",
  "#607D8B",
  "#2E7D32",
  "#EF6C00",
  "#D84315",
  "#B71C1C",
] as const;
export const ImpactLevel = [
  "unknown",
  "idle",
  "normal",
  "warning",
  "error",
  "fatal",
] as const;
export type LevelType = (typeof ImpactLevel)[number];
export const GetColor = (level: number | LevelType) =>
  typeof level === "number"
    ? ImpactColor[level]
    : ImpactColor[ImpactLevel.indexOf(level)];
export const GetLabelColor = (level: number | LevelType) =>
  typeof level === "number"
    ? LabelColor[level]
    : LabelColor[ImpactLevel.indexOf(level)];
