<template>
  <div class="container mt-4">
    <h1>CoWorkSpace Frontend</h1>
    <div class="mb-3">
      <button
        class="btn btn-primary"
        :disabled="appStore.loading"
        @click="appStore.testConnection"
      >
        {{ appStore.loading ? 'Conectando...' : 'Probar Conexión' }}
      </button>
    </div>
    <div v-if="appStore.testMessage" class="alert alert-success">
      {{ appStore.testMessage }}
    </div>
    <div v-if="appStore.error" class="alert alert-danger">
      {{ appStore.error }}
    </div>
  </div>
</template>

<script>
import { useAppStore } from './stores/app';

export default {
  name: 'App',
  setup() {
    const appStore = useAppStore();
    return { appStore };
  },
};
</script>