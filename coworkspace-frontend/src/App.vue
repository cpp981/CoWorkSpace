<template>
  <div class="app-container">
    <Home
      v-if="currentView === 'home'"
      @open-login="handleOpenLogin"
      @open-register="handleOpenRegister"
    />
    <Login
      v-if="currentView === 'login'"
      @cancel="handleCancel"
    />
    <Register
      v-if="currentView === 'register'"
      @cancel="handleCancel"
    />
  </div>
</template>

<script>
import Home from './views/Home.vue';
import Login from './views/Login.vue';
import Register from './views/Register.vue';

export default {
  name: 'App',
  components: { Home, Login, Register },
  data() {
    return {
      currentView: 'home', // Vista inicial
    };
  },
  methods: {
    handleOpenLogin() {
      console.log('Evento open-login recibido, mostrando Login.vue');
      this.currentView = 'login';
    },
    handleOpenRegister() {
      console.log('Evento open-register recibido, mostrando Register.vue');
      this.currentView = 'register';
    },
    handleCancel() {
      console.log('Evento cancel recibido, volviendo a Home.vue');
      this.currentView = 'home';
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