<template>
  <section class="item">
    <header>
      <i>
        <slot name="icon"></slot>
      </i>
      <h3>
        <slot name="heading"></slot>
      </h3>
      <div>
        <slot name="brief"></slot>
      </div>
    </header>
    <div class="details">
      <slot></slot>
    </div>
  </section>
</template>

<style scoped>
.item {
  margin: 1rem 0;
  padding: 0.5rem 0;
  border-top: 1px solid var(--color-border);
}

header {
  display: flex;
  align-items: center;
  margin: 0.5rem 0;
}
header > i {
  margin: 0.4rem 0.8rem 0 0;
  color: var(--color-text-strong);
}
header > h3 {
  flex: 1;
}
header > div {
  margin-left: 0.5rem;
  color: var(--color-text-second);
}

.details {
  margin-top: 1rem;
}
</style>
