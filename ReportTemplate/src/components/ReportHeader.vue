<script setup lang="ts">
<<<<<<< HEAD
import { ref, nextTick } from "vue";
import type { Ref } from "vue";
import moment from "moment";
import type { ReportData } from "./interface/ReportData.interface";
import { PrintStore } from "../stores/PrintStore";
import IconPrint from "./icons/IconPrint.vue";
import IconList from "./icons/IconList.vue";
import IconText from "./icons/IconText.vue";
import IconRefresh from "./icons/IconRefresh.vue";
import IconFullsize from "./icons/IconFullsize.vue";
import IconNormalsize from "./icons/IconNormalsize.vue";
import loadI18n from "../locales/load";

defineProps<{
  data: ReportData;
}>();

const __ = loadI18n();

=======
import { ref } from "vue";
import type { NetworkData } from "./interface/NetworkData.interface";
import IconRefresh from "./icons/IconRefresh.vue";
import IconPrint from "./icons/IconPrint.vue";
import IconFullsize from "./icons/IconFullsize.vue";
import IconNormalsize from "./icons/IconNormalsize.vue";

defineProps<{
  data: NetworkData;
}>();

>>>>>>> 3d122b55bd850f8abe910dcd6ff9497bba1b5a24
const refreshPage = () => {
  document.location.reload();
};

<<<<<<< HEAD
const listPanel: Ref<HTMLElement | null> = ref(null);
const ifListPanel = ref(false);
const ifFullWidth = ref(false);
const ifLargeText = ref(false);

const headerList: Ref<{ text: string; tag: string; node: Element | null }[]> =
  ref([{ text: "index", tag: "h1", node: null }]);
=======
const ifFullWidth = ref(false);
>>>>>>> 3d122b55bd850f8abe910dcd6ff9497bba1b5a24

const delay = (ms: number) => new Promise((res) => setTimeout(res, ms));

const togglePageWidth = () => {
  const element = document.getElementById("app");
  if (!element) {
    return;
  }
  if (element.style.maxWidth === "100vw") {
    element.style.maxWidth = "";
    ifFullWidth.value = false;
  } else {
    element.style.maxWidth = "100vw";
    ifFullWidth.value = true;
  }
};

<<<<<<< HEAD
const toggleFontSize = () => {
  if (ifLargeText.value) {
    document.documentElement.style.fontSize = "";
    ifLargeText.value = false;
  } else {
    document.documentElement.style.fontSize = "20px";
    ifLargeText.value = true;
  }
};

const toggleListPanel = async () => {
  if (ifListPanel.value) {
    ifListPanel.value = false;
  } else {
    headerList.value = Array.from(document.querySelectorAll("h2, h3")).map(
      (node) => ({
        text: node.textContent || "",
        tag: node.tagName,
        node: node,
      })
    );
    ifListPanel.value = true;
    await nextTick();
    listPanel.value?.focus();
  }
};

const printPage = async () => {
  PrintStore().beforePrint();
  await delay(200);
  window.print();
};

const scrollToNode = (node: Element | null) => {
  if (!node) {
    return;
  }
  node.scrollIntoView({
    behavior: "smooth",
  });
  ifListPanel.value = false;
};

const focusOutListPanel = async () => {
  if (!ifListPanel.value) {
    return;
  }
  await delay(200);
  ifListPanel.value = false;
};
=======
const printPage = async () => {
  [].forEach.call(
    document.querySelectorAll(".chart-wrapper"),
    (element: HTMLElement) => {
      element.style.width = "896px";
    }
  );
  await delay(100);
  window.print();
};
>>>>>>> 3d122b55bd850f8abe910dcd6ff9497bba1b5a24
</script>

<template>
  <div class="page-title">
<<<<<<< HEAD
    <h1 class="title">{{ __("page_title") }}</h1>
=======
    <h1 class="title">NIAS 网络数据报表</h1>
>>>>>>> 3d122b55bd850f8abe910dcd6ff9497bba1b5a24
    <div class="page-action">
      <button title="刷新页面" @click="refreshPage">
        <IconRefresh />
      </button>
<<<<<<< HEAD
      <button title="显示大纲列表" @click="toggleListPanel">
        <IconList />
      </button>
      <button title="切换字号大小" @click="toggleFontSize">
        <IconText />
      </button>
=======
>>>>>>> 3d122b55bd850f8abe910dcd6ff9497bba1b5a24
      <button title="切换页面宽度" @click="togglePageWidth">
        <IconFullsize v-if="!ifFullWidth" />
        <IconNormalsize v-if="ifFullWidth" />
      </button>
      <button title="打印报表" @click="printPage">
        <IconPrint />
      </button>
    </div>
<<<<<<< HEAD
    <Transition>
      <div
        class="page-listpanel"
        tabindex="0"
        v-show="ifListPanel"
        ref="listPanel"
        v-on:focusout="focusOutListPanel"
      >
        <ul>
          <li v-for="(header, i) in headerList" :key="i" :class="header.tag">
            <a :href="'#' + header.text" @click="scrollToNode(header.node)"
              ><span>{{ header.text }}</span></a
            >
          </li>
        </ul>
      </div>
    </Transition>
    <p class="subtitle">
      <span class="subtitle-text" title="文档说明">{{ data.Title }}</span>
      <span class="subtitle-location" title="报告地址">{{
        data.Location
      }}</span>
      <span class="subtitle-text" title="报告作者">{{ data.User }}</span>
      <time class="subtitle-time" title="报告生成时间">{{
        moment(data.CreateTime).format("yyyy-MM-DD HH:mm:ss")
      }}</time>
    </p>
    <p class="text description">
      本报表由网络智能分析引擎自动生成，包含过去一段时间内的局域网内各个设备的实际工作状态和各类通信数据。报表内容可用于了解和分析网络整体运行情况，有助于针对通信故障和网络风险进行排查和维护工作。报表中部分内容由计算机自动分析得出，可能与实际情况存在误差。
    </p>
=======
    <p class="subtitle">
      <span>XXXX站场</span>
      <time>XXXX-XX-XX XX:XX:XX</time>
      <span>XXXX</span>
    </p>
    <p class="text description">并难由照解果二满政之资亲社题较非，与队治形住青备蠢阶以子-做名。 除求太少号并到争关，指增及议七争织将常，以E虚金好杏养。 电斗三品放取族来商面，是至作题此三劳思酸，规连辰针每呜承总。 给音器类完但劳，维状府向号观题，结H声下知。</p>
>>>>>>> 3d122b55bd850f8abe910dcd6ff9497bba1b5a24
  </div>
</template>

<style scoped>
.page-title {
  position: relative;
}
<<<<<<< HEAD

=======
>>>>>>> 3d122b55bd850f8abe910dcd6ff9497bba1b5a24
.page-action {
  position: absolute;
  right: 0;
  top: 0;
  margin: 0.5rem 0;
}
<<<<<<< HEAD

.page-action > button {
  background: transparent;
  background-color: transparent;
  border: 0.075rem solid transparent;
=======
.page-action > button {
  background: transparent;
  background-color: var(--color-background-soft);
  border: 0.075rem solid var(--color-border);
>>>>>>> 3d122b55bd850f8abe910dcd6ff9497bba1b5a24
  color: var(--color-text-second);
  border-radius: 0.5rem;
  padding: 0;
  font: inherit;
  width: 2rem;
  height: 2rem;
  cursor: pointer;
  outline: inherit;
  margin-left: 0.5rem;
  padding: 0.18rem;
  transition: border-color 0.2s, color 0.2s, background-color 0.2s;
}
<<<<<<< HEAD

.page-action > button:hover {
  background-color: var(--color-background);
  border-color: var(--color-border);
  color: var(--color-text-highlight);
}

=======
.page-action > button:hover {
  background-color: var(--color-background);
  border-color: var(--color-text-highlight);
  color: var(--color-text-highlight);
}
>>>>>>> 3d122b55bd850f8abe910dcd6ff9497bba1b5a24
.page-action > button:active {
  background-color: var(--color-background-mute);
  border-color: var(--color-border-hover);
  color: var(--color-text);
}
<<<<<<< HEAD

=======
>>>>>>> 3d122b55bd850f8abe910dcd6ff9497bba1b5a24
.page-action > button > svg {
  width: 100%;
  height: 100%;
}
<<<<<<< HEAD

.page-listpanel {
  position: absolute;
  right: 0;
  top: 2rem;
  margin-top: 1rem;
  padding: 0.4rem 1rem;
  z-index: 10;
  font-size: 0.9rem;
  background: transparent;
  background-color: var(--color-background);
  border: 0.075rem solid var(--color-border-hover);
  border-radius: 0.5rem;
  outline: none;
}

.page-listpanel > ul {
  margin: 0;
  padding: 0;
}

.page-listpanel > ul > li {
  margin: 0.4rem 0;
  padding: 0;
  list-style: none;
}

.page-listpanel > ul > li.H3 {
  margin-left: 1rem;
}

.v-enter-active,
.v-leave-active {
  transform-origin: top center;
  transition: transform 0.3s, opacity 0.3s;
}

.v-enter-from,
.v-leave-to {
  transform: scale(0.8) translateY(-1rem);
  opacity: 0;
}

=======
>>>>>>> 3d122b55bd850f8abe910dcd6ff9497bba1b5a24
.subtitle {
  color: var(--color-text-second);
}

.subtitle > *::after {
  content: "•";
  font-weight: bold;
  margin: 0 0.5rem;
}
<<<<<<< HEAD

=======
>>>>>>> 3d122b55bd850f8abe910dcd6ff9497bba1b5a24
.subtitle > *:last-child::after {
  content: none;
}

<<<<<<< HEAD
p.description {
  margin: 1rem 4rem;
  font-size: 0.9rem;
}

=======
>>>>>>> 3d122b55bd850f8abe910dcd6ff9497bba1b5a24
@media print {
  .title,
  .subtitle {
    text-align: center;
  }
<<<<<<< HEAD

  .page-action,
  .page-listpanel {
=======
  .page-action {
>>>>>>> 3d122b55bd850f8abe910dcd6ff9497bba1b5a24
    display: none;
  }
}
</style>
