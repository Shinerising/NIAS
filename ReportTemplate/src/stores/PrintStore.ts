import { defineStore } from "pinia";
import { ref } from "vue";

export const PrintStore = defineStore("print", () => {
  const isPrinting = ref(false);
  const callbackList: ((isPre: boolean) => void)[] = [];
  const beforePrint = () => {
    [].forEach.call(
      document.querySelectorAll(".chart-wrapper"),
      (element: HTMLElement) => {
        element.style.width = "896px";
      }
    );

    for (const callback of callbackList) {
      callback(true);
    }

    isPrinting.value = true;
  };
  const afterPrint = () => {
    [].forEach.call(
      document.querySelectorAll(".chart-wrapper"),
      (element: HTMLElement) => {
        element.style.width = "";
      }
    );

    for (const callback of callbackList) {
      callback(false);
    }

    isPrinting.value = false;
  };
  const initialize = () => {
    window.addEventListener("beforeprint", beforePrint);
    window.addEventListener("afterprint", afterPrint);
  };
  const AddPrintCallback = (callback: (isPre: boolean) => void) => {
    callbackList.push(callback);
  };

  return {
    isPrinting,
    beforePrint,
    afterPrint,
    initialize,
    AddPrintCallback,
  };
});
