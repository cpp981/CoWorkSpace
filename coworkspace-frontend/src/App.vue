<template>
  <div class="app-container">
    <Home
      @open-login="handleOpenLogin"
      @open-register="handleOpenRegister"
    />
  </div>
</template>

<script>
import Home from './views/Home.vue';

export default {
  name: 'App',
  components: { Home },
  methods: {
    handleOpenLogin() {
      // Lógica de login se implementará más adelante
    },
    handleOpenRegister() {
      // Lógica de registro se implementará más adelante
    },
  },
};
</script>

<style scoped>
.app-container {
  min-height: 100vh;
  display: flex;
  flex-direction: column;
}
</style>