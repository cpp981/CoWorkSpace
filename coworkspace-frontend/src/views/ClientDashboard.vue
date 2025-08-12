<template>
  <div class="d-flex">
    <!-- Menú lateral -->
    <GenericMenu @button-click="handleMenuClick" />

    <!-- Contenido principal del dashboard -->
    <div class="flex-grow-1 p-3">
      <Dashboard 
        :title="dashboardTitle" 
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
              <tr 
                v-for="booking in stats.bookings" 
                :key="booking.bookingId"
                >
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

<script>
import Dashboard from '../components/Dashboard.vue';
import GenericMenu from '../components/GenericMenu.vue';
import api from '../services/api'; // Importar objeto completo
import { useAuthStore } from '../stores/auth';

export default {
  name: 'ClientDashboard',
  components: {
    Dashboard,
    GenericMenu,
  },
  setup() {
    return { authStore: useAuthStore() };
  },
  data() {
    return {
      stats: {
        totalBookings: 0,
        totalSpent: 0,
        totalReviews: 0,
        bookings: [],
      },
      errorMessage: null,
    };
  },
  computed: {
    dashboardTitle() {
      return `Dashboard de ${this.authStore.userName || ''}`;
    },
    clientMetrics() {
      return [
        { label: 'Reservas Totales', value: this.stats.totalBookings },
        { label: 'Gasto Total', value: `${this.stats.totalSpent.toFixed(2)} €` },
        { label: 'Reseñas Totales', value: this.stats.totalReviews },
      ];
    },
    chartData() {
      return {
        labels: this.stats.bookings.map(booking => `Reserva ${booking.bookingId}`),
        datasets: [{
          label: 'Monto (€)',
          data: this.stats.bookings.map(booking => booking.amount),
          backgroundColor: ['#007bff', '#28a745', '#dc3545', '#ffc107'],
        }],
      };
    },
    chartOptions() {
      return {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { display: true },
        },
      };
    },
    tableHeaders() {
      return ['ID', 'Espacio', 'Monto'];
    },
    tableData() {
      return this.stats.bookings.map(booking => [
        booking.bookingId,
        booking.name,
        `${booking.amount.toFixed(2)} €`,
      ]);
    },
  },
  async created() {
    try {
      if (!this.authStore.userId) {
        this.errorMessage = 'Usuario no autenticado.';
        return;
      }
      const response = await api.getClientStats(this.authStore.userId);
      this.stats = response.data || this.stats;
    } catch (error) {
      this.errorMessage = error.response?.data?.message || 'Error al cargar las estadísticas.';
    }
  },
  methods: {
    handleMenuClick(button) {
      console.log('Botón del menú clicado:', button);
      // Aquí realizamos la acción de redirigir al pulsar el botón del meú lateral
    }
  }
};
</script>

<style scoped>
/* Personalizaciones si es necesario */
</style>