<template>
  <div class="app-container">
    <component
      :is="currentView"
      :userId="authStore.userId"
      @open-login="setView('Login')"
      @open-register="setView('Register')"
      @cancel="setView('Home')"
      @login-success="handleLoginSuccess"
    />
    <!--<div v-if="authStore.isAuthenticated && authStore.isTokenValid" class="logout-container">
      <button @click="logout" class="btn btn-outline-danger">
        Cerrar Sesión
      </button>
    </div>-->
  </div>
</template>

<script>
import { useAuthStore } from './stores/auth';
import Home from './views/Home.vue';
import Login from './views/Login.vue';
import Register from './views/UserRegister.vue';
import SuperAdminDashboard from './views/SuperAdminDashboard.vue';
import AdminDashboard from './views/AdminDashboard.vue';
import ProviderDashboard from './views/ProviderDashboard.vue';
import ClientDashboard from './views/ClientDashboard.vue';
import api from './services/api';

export default {
  name: 'App',
  components: {
    Home,
    Login,
    Register,
    SuperAdminDashboard,
    AdminDashboard,
    ProviderDashboard,
    ClientDashboard,
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
      const roleViewMap = {
        SuperAdmin: 'SuperAdminDashboard',
        Admin: 'AdminDashboard',
        Provider: 'ProviderDashboard',
        Client: 'ClientDashboard',
      };
      this.currentView = roleViewMap[this.authStore.userRole] || 'Home';
    },
    logout() {
      this.authStore.logout();
      this.setView('Home');
    },
  },
  created() {
    if (this.authStore.isAuthenticated && !this.authStore.isTokenValid) {
      this.authStore.logout();
      this.setView('Login');
    } else if (this.authStore.isAuthenticated) {
      this.handleLoginSuccess();
    }
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
  display: flex;
  flex-direction: column;
}
.logout-container {
  position: fixed;
  top: 20px;
  right: 20px;
}
</style>