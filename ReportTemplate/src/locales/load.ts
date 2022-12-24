import { useI18n } from "vue-i18n";
import type { MessageSchema } from "./schema";

const loadI18n = () => {
  const { t } = useI18n<{ message: MessageSchema }>({
    useScope: "global",
  });
  return t;
};

export default loadI18n;
