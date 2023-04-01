import { fileURLToPath } from "node:url";
import { defineConfig } from "vite";
import vue from "@vitejs/plugin-vue";
import { viteSingleFile } from "vite-plugin-singlefile";

// https://vitejs.dev/config/
export default defineConfig({
  plugins: [vue(), viteSingleFile()],
  resolve: {
    alias: [
      {
        find: "@",
        replacement: fileURLToPath(new URL("./src", import.meta.url)),
      },
      {
        find: "./assets/demodata.json",
        replacement: fileURLToPath(
          new URL(
            process.env.APP_ENV === "production"
              ? "./src/assets/empty.json"
              : "./src/assets/demodata.json",
            import.meta.url
          )
        ),
      },
    ],
  },
});
