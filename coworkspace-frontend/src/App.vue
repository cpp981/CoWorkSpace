<template>
  <div class="app-container">
    <component :is="currentViewComponent" :userId="authStore.userId" @open-login="setView('Login')"
      @open-register="setView('Register')" @cancel="setView('Home')" @login-success="handleLoginSuccess" />
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import { useAuthStore } from "./stores/auth";

import Home from "./views/Home.vue";
import Login from "./views/Login.vue";
import Register from "./views/UserRegister.vue";
import SuperAdminDashboard from "./views/SuperAdminDashboard.vue";
import AdminDashboard from "./views/AdminDashboard.vue";
import ProviderDashboard from "./views/ProviderDashboard.vue";
import ClientDashboard from "./views/ClientDashboard.vue";
import api from "./services/api";

const authStore = useAuthStore();

// Mapa de vistas a componentes (pasamos el componente, no solo el nombre)
const views = {
  Home,
  Login,
  Register,
  SuperAdminDashboard,
  AdminDashboard,
  ProviderDashboard,
  ClientDashboard,
};

const currentView = ref("Home");

// Computed que devuelve el componente real para <component :is="...">
const currentViewComponent = computed(() => views[currentView.value] || Home);

const setView = (view) => {
  currentView.value = views[view] ? view : "Home";
};

const handleLoginSuccess = () => {
  const roleViewMap = {
    SuperAdmin: "SuperAdminDashboard",
    Admin: "AdminDashboard",
    Provider: "ProviderDashboard",
    Client: "ClientDashboard",
  };
  const view = roleViewMap[authStore.userRole] || "Home";
  setView(view);
};

const logout = () => {
  authStore.logout();
  setView("Home");
};

onMounted(() => {
  if (authStore.isAuthenticated && !authStore.isTokenValid) {
    authStore.logout();
    setView("Login");
  } else if (authStore.isAuthenticated) {
    handleLoginSuccess();
  }

  // Interceptor para forzar logout en 401
  api.client.interceptors.response.use(
    (response) => response,
    (error) => {
      if (error.response?.status === 401) {
        authStore.logout();
        setView("Login");
      }
      return Promise.reject(error);
    }
  );
});
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