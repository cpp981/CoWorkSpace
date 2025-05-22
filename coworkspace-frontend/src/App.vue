<template>
  <div class="app-container">
    <Home
      v-if="!showLogin"
      @open-login="handleOpenLogin"
      @open-register="handleOpenRegister"
    />
    <Login
      v-if="showLogin"
      @cancel="handleCancel"
    />
  </div>
</template>

<script>
import Home from './views/Home.vue';
import Login from './views/Login.vue';

export default {
  name: 'App',
  components: { Home, Login },
  data() {
    return {
      showLogin: false,
    };
  },
  methods: {
    handleOpenLogin() {
      console.log('Evento open-login recibido, mostrando Login.vue');
      this.showLogin = true;
    },
    handleOpenRegister() {
      console.log('Evento open-register recibido (no implementado aún)');
      // Lógica de registro se implementará más adelante
    },
    handleCancel() {
      console.log('Evento cancel recibido, volviendo a Home.vue');
      this.showLogin = false;
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