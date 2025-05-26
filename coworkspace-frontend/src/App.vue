<template>
  <div class="app-container">
    <component
      :is="currentView"
      @open-login="setView('Login')"
      @open-register="setView('Register')"
      @cancel="setView('Home')"
      @login-success="handleLoginSuccess"
    />
    <div v-if="authStore.isAuthenticated && authStore.isTokenValid" class="logout-container">
      <button @click="logout" class="btn btn-outline-danger">
        Cerrar Sesión
      </button>
    </div>
  </div>
</template>

<script>
import { useAuthStore } from './stores/auth';
import Home from './views/Home.vue';
import Login from './views/Login.vue';
import Register from './views/Register.vue';
import api from './services/api';

export default {
  name: 'App',
  components: {
    Home,
    Login,
    Register,
  },
  data() {
    return {
      currentView: 'Home',
    };
  },
  setup() {
    const authStore = useAuthStore();
    return { authStore };
  },
  methods: {
    setView(view) {
      this.currentView = view;
    },
    handleLoginSuccess() {
      //this.setView('Home'); // Redirigir a Home tras login exitoso
    },
    logout() {
      this.authStore.logout();
      this.setView('Home');
    },
  },
  created() {
    // Verificar token al cargar
    if (this.authStore.isAuthenticated && !this.authStore.isTokenValid) {
      this.authStore.logout();
      this.setView('Login');
    }
    // Interceptar errores 401
    api.client.interceptors.response.use(
      (response) => response,
      (error) => {
        if (error.response?.status === 401) {
          this.authStore.logout();
          this.setView('Login');
        }
        return Promise.reject(error);
      }
    );
  },
};
</script>
<style scoped>
.app-container {
  min-height: 100vh;
  background-color: #f8f9fa;
}

.logout-container {
  position: fixed;
  top: 1rem;
  right: 1rem;
}
</style>