<template>
  <div class="d-flex">
    <!-- Menú lateral -->
    <GenericMenu @button-click="handleMenuClick" />

    <!-- Contenido principal del dashboard -->
    <div class="flex-grow-1 p-3">
      <Dashboard 
        :title="dashboardTitle" 
        :metrics="adminMetrics" 
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
                <th>Reservas</th>
                <th>Ingresos</th>
                <th>Valoración</th>
              </tr>
            </thead>
            <tbody>
              <tr 
                v-for="space in stats.spaces" 
                :key="space.spaceId"
                >
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
    </div>
  </div>
</template>

<script>
import Dashboard from '../components/Dashboard.vue';
import GenericMenu from '../components/GenericMenu.vue';
import api from '../services/api';
import { useAuthStore } from '../stores/auth';

export default {
  name: 'AdminDashboard',
  components: {
    Dashboard,
    GenericMenu,
  },
  setup() {
    return {
      authStore: useAuthStore(),
    };
  },
  data() {
    return {
      stats: {
        totalSpaces: 0,
        totalBookings: 0,
        totalRevenue: 0,
        averageRating: 0,
        spaces: [],
      },
      errorMessage: null,
    };
  },
  computed: {
    dashboardTitle() {
      return `Dashboard de ${this.authStore.userName || ''}`;
    },
    adminMetrics() {
      return [
        { label: 'Espacios Totales', value: this.stats.totalSpaces },
        { label: 'Reservas Totales', value: this.stats.totalBookings },
        { label: 'Ingresos Totales', value: `${this.stats.totalRevenue.toFixed(2)} €` },
        { label: 'Valoración Media', value: this.stats.averageRating.toFixed(1) },
      ];
    },
    chartData() {
      return {
        labels: this.stats.spaces.map(space => space.spaceName),
        datasets: [{
          label: 'Ingresos (€)',
          data: this.stats.spaces.map(space => space.revenue),
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
      return ['ID', 'Nombre', 'Reservas', 'Ingresos', 'Valoración'];
    },
    tableData() {
      return this.stats.spaces.map(space => [
        space.spaceId,
        space.spaceName,
        space.bookingsCount,
        `${space.revenue.toFixed(2)} €`,
        space.averageRating.toFixed(1),
      ]);
    },
  },
  async created() {
    try {
      if (!this.authStore.userId) {
        this.errorMessage = 'Usuario no autenticado.';
        return;
      }
      const response = await api.getAdminStats(this.authStore.userId);
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
