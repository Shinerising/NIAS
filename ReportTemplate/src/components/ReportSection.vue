<template>
  <section class="item">
    <header>
      <i>
        <slot name="icon"></slot>
      </i>
      <h3>
        <slot name="heading"></slot>
      </h3>
    </header>
    <div class="details">
      <slot></slot>
    </div>
  </section>
</template>

<style scoped>
.item {
  margin: 1rem 0;
  padding: 1rem 0;
  border-top: 1px solid var(--color-border);
}
</style>
