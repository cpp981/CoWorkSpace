<template>
  <div class="app-container position-relative">
    <!-- Top user menu fijo en la esquina superior derecha cuando usuario se loguea -->
    <div
      class="position-absolute top-0 end-0 m-3"
      v-if="authStore.isAuthenticated"
    >
      <TopUserMenu
        :userName="authStore.userName"
        @preferences="goToPreferences"
        @logout="logout"
      />
    </div>

    <!-- Componente actual -->
    <component
      :is="currentViewComponent"
      :userId="authStore.userId"
      @open-login="setView('Login')"
      @open-register="setView('Register')"
      @cancel="setView('Home')"
      @login-success="handleLoginSuccess"
    />
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
import TopUserMenu from "./components/TopUserMenu.vue";
import api from "./services/api";

const authStore = useAuthStore();

// Vistas disponibles
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

const goToPreferences = () => {
  console.log("Ir a preferencias del usuario");
};

onMounted(() => {
  if (authStore.isAuthenticated && !authStore.isTokenValid) {
    authStore.logout();
    setView("Login");
  } else if (authStore.isAuthenticated) {
    handleLoginSuccess();
  }

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
