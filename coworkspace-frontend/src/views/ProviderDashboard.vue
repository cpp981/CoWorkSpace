<template>
  <div class="d-flex">
    <!-- Menú lateral -->
    <GenericMenu @button-click="handleMenuClick" />

    <!-- Contenido principal -->
    <div class="flex-grow-1 p-3">
      <!-- Vista de ESPACIOS -->
      <ProviderSpacesView
        v-if="currentView === 'spaces'"
        @view-bookings="openBookings"
      />

      <!-- Vista de RESERVAS de un espacio -->
      <SpacesBookingView
        v-else-if="currentView === 'bookings'"
        :space-id="selectedSpaceId"
        :space-name="selectedSpaceName"
        @back="currentView = 'spaces'"
      />

      <!-- Vista de Admins de un Provider -->
      <ProviderAdminList v-else-if="currentView === 'adminsList'" />

      <!-- Vista principal del dashboard -->
      <Dashboard
        v-else
        :title="dashboardTitle"
        :metrics="providerMetrics"
        :chartTitle="'Ingresos por Espacio'"
        :chartData="chartData"
        :chartOptions="chartOptions"
        :detailsTitle="'Espacios Gestionados'"
        :tableHeaders="tableHeaders"
        :tableData="tableData"
        :errorMessage="errorMessage"
      >
        <template #details>
          <table v-if="stats.spaces.length" class="table table-striped">
            <thead>
              <tr>
                <th>ID</th>
                <th>Nombre</th>
                <th>Administrador</th>
                <th>Reservas</th>
                <th>Ingresos</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="space in stats.spaces" :key="space.spaceId">
                <td>{{ space.spaceId }}</td>
                <td>{{ space.spaceName }}</td>
                <td>{{ space.adminName }}</td>
                <td>{{ space.bookingsCount }}</td>
                <td>{{ space.revenue.toFixed(2) }} €</td>
              </tr>
            </tbody>
          </table>
          <p v-else class="text-muted">No hay espacios disponibles.</p>
        </template>
      </Dashboard>
    </div>
  </div>
</template>

<script setup>
import { ref, computed, onMounted } from "vue";
import Dashboard from "../components/Dashboard.vue";
import ProviderSpacesView from "./ProviderSpacesView.vue";
import SpacesBookingView from "./SpacesBookingView.vue";
import ProviderAdminList from "./ProviderAdminList.vue";
import GenericMenu from "../components/GenericMenu.vue";
import api from "../services/api";
import { useAuthStore } from "../stores/auth";

const authStore = useAuthStore();

// Estado principal
const currentView = ref(null);
const selectedSpaceId = ref(null);
const selectedSpaceName = ref("");

const stats = ref({
  totalSpaces: 0,
  totalAdmins: 0,
  totalBookings: 0,
  totalRevenue: 0,
  spaces: [],
});
const errorMessage = ref(null);

// Computed properties
const dashboardTitle = computed(
  () => `Dashboard de ${authStore.userName || ""}`
);

const providerMetrics = computed(() => [
  {
    label: "Espacios Totales",
    value: stats.value.totalSpaces,
    icon: "bi bi-building fs-2",
  },
  {
    label: "Administradores",
    value: stats.value.totalAdmins,
    icon: "bi bi-person-badge text-primary fs-2",
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
]);

const chartData = computed(() => ({
  labels: stats.value.spaces.map((space) => space.spaceName),
  datasets: [
    {
      label: "Ingresos (€)",
      data: stats.value.spaces.map((space) => space.revenue),
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

const tableHeaders = computed(() => [
  "ID",
  "Nombre",
  "Administrador",
  "Reservas",
  "Ingresos",
]);

const tableData = computed(() =>
  stats.value.spaces.map((space) => [
    space.spaceId,
    space.spaceName,
    space.adminName,
    space.bookingsCount,
    `${space.revenue.toFixed(2)} €`,
  ])
);

// Métodos
function handleMenuClick(button) {
  if (button.action === "showSpaces") {
    currentView.value = "spaces";
  } else if (button.action === "showAdmins") {
    //console.log("Botón Administradores pulsado:", button);
    currentView.value = "adminsList";
  } else if (button.action === "showDashboard") {
    currentView.value = null; // vuelve al dashboard por defecto
  } else {
    currentView.value = null;
  }
}

function openBookings(space) {
  selectedSpaceId.value = space.id;
  selectedSpaceName.value = space.name;
  currentView.value = "bookings";
}

// Cargar datos en montaje
onMounted(async () => {
  try {
    if (!authStore.userId) {
      errorMessage.value = "Usuario no autenticado.";
      return;
    }
    const response = await api.getProviderStats(authStore.userId);
    stats.value = response.data || stats.value;
  } catch (error) {
    errorMessage.value =
      error.response?.data?.message || "Error al cargar las estadísticas.";
  }
});
</script>
