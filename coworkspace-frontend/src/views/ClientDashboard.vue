<template>
  <div class="d-flex">
    <!-- Menú lateral -->
    <GenericMenu @button-click="handleMenuClick" />

    <!-- Contenido principal del dashboard -->
    <div class="flex-grow-1 pt-0 mt-5">
      <Dashboard
        :metrics="clientMetrics"
        :chartTitle="'Gasto por Reserva'"
        :chartData="chartData"
        :chartOptions="chartOptions"
        :detailsTitle="'Historial de Reservas'"
        :tableHeaders="tableHeaders"
        :tableData="tableData"
        :errorMessage="errorMessage"
      >
        <template #details>
          <table v-if="stats.bookings.length" class="table table-striped">
            <thead>
              <tr>
                <th>ID</th>
                <th>Espacio</th>
                <th>Monto</th>
              </tr>
            </thead>
            <tbody>
              <tr v-for="booking in stats.bookings" :key="booking.bookingId">
                <td>{{ booking.bookingId }}</td>
                <td>{{ booking.name }}</td>
                <td>{{ booking.amount.toFixed(2) }} €</td>
              </tr>
            </tbody>
          </table>
          <p v-else class="text-muted">No hay reservas disponibles.</p>
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
  totalBookings: 0,
  totalSpent: 0,
  totalReviews: 0,
  bookings: [],
});

const errorMessage = ref(null);

// Computed
const clientMetrics = computed(() => [
  {
    label: "Reservas Totales",
    value: stats.value.totalBookings,
    icon: "bi bi-calendar-check text-success fs-2",
  },
  {
    label: "Gasto Total",
    value: `${stats.value.totalSpent.toFixed(2)} €`,
    icon: "bi bi-currency-euro text-success fs-2",
  },
  {
    label: "Reseñas Totales",
    value: stats.value.totalReviews,
    icon: "bi bi-chat-left-text text-warning fs-2",
  },
]);

const chartData = computed(() => ({
  labels: stats.value.bookings.map((booking) => `Reserva ${booking.bookingId}`),
  datasets: [
    {
      label: "Monto (€)",
      data: stats.value.bookings.map((booking) => booking.amount),
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

const tableHeaders = computed(() => ["ID", "Espacio", "Monto"]);

const tableData = computed(() =>
  stats.value.bookings.map((booking) => [
    booking.bookingId,
    booking.name,
    `${booking.amount.toFixed(2)} €`,
  ])
);

// Métodos
function handleMenuClick(button) {
  console.log("Botón del menú clicado:", button);
  // Aquí realizamos la acción de redirigir al pulsar el botón del menú lateral
}

// Cargar datos al montar
onMounted(async () => {
  try {
    if (!authStore.userId) {
      errorMessage.value = "Usuario no autenticado.";
      return;
    }
    const response = await api.getClientStats(authStore.userId);
    stats.value = response.data || stats.value;
  } catch (error) {
    errorMessage.value =
      error.response?.data?.message || "Error al cargar las estadísticas.";
  }
});
</script>
