export interface ReportData {
  Title: string | null;
  User: string | null;
  Location: string | null;
  CreateTime: string | null;
  StartTime: string | null;
  EndTime: string | null;
}

const DemoData: ReportData = {
  Title: "测试页面",
  User: "测试用户",
  Location: "测试位置",
  CreateTime: Date(),
  StartTime: Date(),
  EndTime: Date(),
};
export { DemoData };
