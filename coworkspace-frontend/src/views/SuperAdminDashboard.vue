<template>
  <div class="d-flex">
    <!-- Menú lateral -->
    <GenericMenu @button-click="handleMenuClick" />

    <!-- Contenido principal del dashboard -->
    <div class="flex-grow-1 p-3">
      <Dashboard
        :title="dashboardTitle"
        :metrics="superAdminMetrics"
        :chartTitle="'Usuarios por Rol'"
        :chartData="chartData"
        :chartOptions="chartOptions"
        :detailsTitle="'Usuarios por Rol'"
        :tableHeaders="tableHeaders"
        :tableData="tableData"
        :errorMessage="errorMessage"
      >
        <template #details>
          <table
            v-if="Object.keys(stats.usersByRole).length"
            class="table table-striped"
          >
            <thead>
              <tr>
                <th>Rol</th>
                <th>Cantidad</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="(count, role) in stats.usersByRole" :key="role">
                <td>{{ role }}</td>
                <td>{{ count }}</td>
              </tr>
            </tbody>
          </table>
          <p v-else class="text-muted">No hay datos de usuarios.</p>
        </template>
      </Dashboard>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import Dashboard from "../components/Dashboard.vue";
import GenericMenu from "../components/GenericMenu.vue";
import api from "../services/api";
import { useAuthStore } from "../stores/auth";

const authStore = useAuthStore();

// Estado reactivo
const stats = ref({
  totalSpaces: 0,
  totalBookings: 0,
  totalRevenue: 0,
  totalUsers: 0,
  usersByRole: {},
});

const errorMessage = ref(null);

// Computed
const dashboardTitle = computed(
  () => `Dashboard de ${authStore.userName || ""}`
);

const superAdminMetrics = computed(() => [
  {
    label: "Espacios Totales",
    value: stats.value.totalSpaces,
    icon: "bi bi-building fs-2",
  },
  {
    label: "Reservas Totales",
    value: stats.value.totalBookings,
    icon: "bi bi-calendar-check text-success fs-2",
  },
  {
    label: "Ingresos Totales",
    value: `${stats.value.totalRevenue.toFixed(2)} €`,
    icon: "bi bi-currency-euro text-success fs-2",
  },
  {
    label: "Usuarios Totales",
    value: stats.value.totalUsers,
    icon: "bi bi-people-fill fs-2",
  },
]);

const chartData = computed(() => ({
  labels: Object.keys(stats.value.usersByRole),
  datasets: [
    {
      label: "Usuarios",
      data: Object.values(stats.value.usersByRole),
      backgroundColor: ["#007bff", "#28a745", "#dc3545", "#ffc107"],
    },
  ],
}));

const chartOptions = computed(() => ({
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: { display: true },
  },
}));

const tableHeaders = computed(() => ["Rol", "Cantidad"]);

const tableData = computed(() =>
  Object.entries(stats.value.usersByRole).map(([role, count]) => [role, count])
);

// Métodos
function handleMenuClick(button) {
  console.log("Botón del menú clicado:", button);
  // Aquí redirigimos al pulsar el botón del menú
}

// Cargar datos al montar
onMounted(async () => {
  try {
    if (!authStore.userId) {
      errorMessage.value = "Usuario no autenticado.";
      return;
    }
    console.log("API object:", api); // Depuración
    const response = await api.getSuperAdminStats(authStore.userId);
    stats.value = response.data || stats.value;
  } catch (error) {
    errorMessage.value =
      error.response?.data?.message || "Error al cargar las estadísticas.";
  }
});
</script>
