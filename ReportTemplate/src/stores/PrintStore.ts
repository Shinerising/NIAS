import { defineStore } from "pinia";
import { ref } from "vue";

export const PrintStore = defineStore("print", () => {
  const isPrinting = ref(false);
  const beforePrint = () => {
    [].forEach.call(
      document.querySelectorAll(".chart-wrapper"),
      (element: HTMLElement) => {
        element.style.width = "896px";
      }
    );

    isPrinting.value = true;
  };
  const afterPrint = () => {
    [].forEach.call(
      document.querySelectorAll(".chart-wrapper"),
      (element: HTMLElement) => {
        element.style.width = "";
      }
    );

    isPrinting.value = false;
  };
  const initialize = () => {
    window.addEventListener("beforeprint", beforePrint);
    window.addEventListener("afterprint", afterPrint);
  };

  return { isPrinting, beforePrint, afterPrint, initialize };
});
