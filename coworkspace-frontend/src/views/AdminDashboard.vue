<template>
  <div class="d-flex">
    <!-- Menú lateral -->
    <GenericMenu @button-click="handleMenuClick" />

    <!-- Contenido principal -->
    <div class="flex-grow-1 p-3">
      <!-- Vista Dashboard -->
      <Dashboard v-if="currentView === 'dashboard'" :title="dashboardTitle" :metrics="adminMetrics"
        :chartTitle="'Ingresos por Espacio'" :chartData="chartData" :chartOptions="chartOptions"
        :detailsTitle="'Espacios Gestionados'" :tableHeaders="tableHeaders" :tableData="tableData"
        :errorMessage="errorMessage">
        <template #details>
          <table v-if="stats.spaces.length" class="table table-striped">
            <thead>
              <tr>
                <th>ID</th>
                <th>Nombre</th>
                <th>Reservas</th>
                <th>Ingresos</th>
                <th>Valoración</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="space in stats.spaces" :key="space.spaceId">
                <td>{{ space.spaceId }}</td>
                <td>{{ space.spaceName }}</td>
                <td>{{ space.bookingsCount }}</td>
                <td>{{ space.revenue.toFixed(2) }} €</td>
                <td>{{ space.averageRating.toFixed(1) }}</td>
              </tr>
            </tbody>
          </table>
          <p v-else class="text-muted">No hay espacios disponibles.</p>
        </template>
      </Dashboard>

      <!-- Vista Spaces -->
      <AdminSpacesListView v-else-if="currentView === 'spaces'" />

      <!-- Vista Clientes -->
      <AdminClientsList v-else-if="currentView === 'clientsList'" />
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from 'vue';
import Dashboard from '../components/Dashboard.vue';
import GenericMenu from '../components/GenericMenu.vue';
import AdminSpacesListView from './AdminSpacesListView.vue';
import AdminClientsList from './AdminClientsList.vue';
import api from '../services/api';
import { useAuthStore } from '../stores/auth';

const authStore = useAuthStore();
const currentView = ref("dashboard"); // por defecto mostramos dashboard
const stats = ref({
  totalSpaces: 0,
  totalBookings: 0,
  totalRevenue: 0,
  averageRating: 0,
  spaces: [],
});
const errorMessage = ref(null);

// --- Computed ---
const dashboardTitle = computed(() => `Dashboard de ${authStore.userName || ''}`);
const adminMetrics = computed(() => [
  { label: 'Espacios Totales', value: stats.value.totalSpaces },
  { label: 'Reservas Totales', value: stats.value.totalBookings },
  { label: 'Ingresos Totales', value: `${stats.value.totalRevenue.toFixed(2)} €` },
  { label: 'Valoración Media', value: stats.value.averageRating.toFixed(1) },
]);
const chartData = computed(() => ({
  labels: stats.value.spaces.map(space => space.spaceName),
  datasets: [{
    label: 'Ingresos (€)',
    data: stats.value.spaces.map(space => space.revenue),
    backgroundColor: ['#007bff', '#28a745', '#dc3545', '#ffc107'],
  }],
}));
const chartOptions = computed(() => ({
  responsive: true,
  maintainAspectRatio: false,
  plugins: {
    legend: { display: true },
  },
}));
const tableHeaders = computed(() => ['ID', 'Nombre', 'Reservas', 'Ingresos', 'Valoración']);
const tableData = computed(() =>
  stats.value.spaces.map(space => [
    space.spaceId,
    space.spaceName,
    space.bookingsCount,
    `${space.revenue.toFixed(2)} €`,
    space.averageRating.toFixed(1),
  ])
);

// --- Métodos ---
function handleMenuClick(button) {
  if (button.action === "showSpaces") {
    currentView.value = "spaces";
  }
  else if (button.action === "showClients") {
    console.log("Botón Clientes pulsado:", button);
    currentView.value = "clientsList";
  }
  else {
    currentView.value = "dashboard";
  }
}

onMounted(async () => {
  try {
    if (!authStore.userId) {
      errorMessage.value = 'Usuario no autenticado.';
      return;
    }
    const response = await api.getAdminStats(authStore.userId);
    stats.value = response.data || stats.value;
  } catch (error) {
    errorMessage.value = error.response?.data?.message || 'Error al cargar las estadísticas.';
  }
});
</script>
