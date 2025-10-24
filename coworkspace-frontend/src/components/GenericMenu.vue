<template>
  <div class="sidebar border-end p-3" style="min-width: 220px">
    <!-- Logo -->
    <div class="logo mb-2 text-center" style="max-width: 220px">
      <img src="../assets/logo.png" alt="Logo" class="img-fluid rounded" />
    </div>

    <!-- Nombre del usuario conectado -->
    <div class="text-center text-light mt-4 mb-5">
      <small class="mt-4 text-light">Conectado como:</small><br />
      <strong><i class="bi bi-person-circle me-2"></i>{{ userName }}</strong>
    </div>
    <div>
      <hr class="text-center text-light text-white" />
    </div>
    <!-- Spinner de carga -->
    <div v-if="isLoading" class="d-flex justify-content-center my-4">
      <div class="spinner-border text-primary" role="status">
        <span class="visually-hidden">Cargando...</span>
      </div>
    </div>

    <!-- Menú de botones -->
    <ul v-else class="nav flex-column mb-4 mt-5">
      <li v-for="button in buttons" :key="button.label" class="nav-item mb-2">
        <button
          class="btn btn-menu text-light w-100 h-100 text-start d-flex align-items-center"
          @click="$emit('button-click', button)"
        >
          <i :class="`bi bi-${button.icon.toLowerCase()}`" class="me-2"></i>
          {{ button.label }}
          <i class="bi bi-caret-right ms-auto"></i>
        </button>
      </li>
    </ul>

    <!-- Botón de logout -->
    <div class="mt-auto">
      <button class="btn btn-outline-danger border-2 w-100" @click="logout">
        <i class="bi bi-box-arrow-right me-2"></i>
        Cerrar Sesión
      </button>
    </div>
  </div>
</template>

<script>
import { ref, onMounted } from "vue";
import axios from "axios";
import { useAuthStore } from "../stores/auth";

export default {
  name: "GenericMenu",
  setup(_, { emit }) {
    const authStore = useAuthStore();
    const buttons = ref([]);
    const isLoading = ref(true);
    const userName = authStore.userName || "Usuario";

    const fetchMenu = async () => {
      try {
        const response = await axios.get("/api/v1/menu", {
          headers: {
            Authorization: `Bearer ${authStore.accessToken}`,
          },
        });
        buttons.value = response.data;
      } catch (error) {
        console.error("Error al cargar el menú:", error);
      } finally {
        isLoading.value = false;
      }
    };

    const logout = () => {
      authStore.logout();
      window.location.href = "/";
    };

    onMounted(fetchMenu);

    return {
      buttons,
      isLoading,
      logout,
      userName,
    };
  },
};
</script>
