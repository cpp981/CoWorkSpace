<template>
  <div class="d-flex">
    <!-- Menú lateral -->
    <GenericMenu @button-click="handleMenuClick" />

    <!-- Contenido principal del dashboard -->
    <div class="flex-grow-1 p-3">
      <!-- Vista de ESPACIOS del proveedor -->
      <ProviderSpacesView v-if="currentView === 'spaces'" />

      <!-- Vista principal del dashboard del proveedor -->
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
              <tr 
                v-for="space in stats.spaces" 
                :key="space.spaceId"
              >
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

<script>
import { ref } from 'vue';
import Dashboard from '../components/Dashboard.vue';
import ProviderSpacesView from './ProviderSpacesView.vue';
import GenericMenu from '../components/GenericMenu.vue';
import api from '../services/api';
import { useAuthStore } from '../stores/auth';

export default {
  name: 'ProviderDashboard',
  components: {
    Dashboard,
    GenericMenu,
    ProviderSpacesView,
  },
  setup() {
    const authStore = useAuthStore();
    const currentView = ref(null); // Por defecto: dashboard
    return { authStore, currentView };
  },
  data() {
    return {
      stats: {
        totalSpaces: 0,
        totalAdmins: 0,
        totalBookings: 0,
        totalRevenue: 0,
        spaces: [],
      },
      errorMessage: null,
    };
  },
  computed: {
    dashboardTitle() {
      return `Dashboard de ${this.authStore.userName || ''}`;
    },
    providerMetrics() {
      return [
        { label: 'Espacios Totales', value: this.stats.totalSpaces },
        { label: 'Administradores', value: this.stats.totalAdmins },
        { label: 'Reservas Totales', value: this.stats.totalBookings },
        { label: 'Ingresos Totales', value: `${this.stats.totalRevenue.toFixed(2)} €` },
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
      return ['ID', 'Nombre', 'Administrador', 'Reservas', 'Ingresos'];
    },
    tableData() {
      return this.stats.spaces.map(space => [
        space.spaceId,
        space.spaceName,
        space.adminName,
        space.bookingsCount,
        `${space.revenue.toFixed(2)} €`,
      ]);
    },
  },
  async created() {
    try {
      if (!this.authStore.userId) {
        this.errorMessage = 'Usuario no autenticado.';
        return;
      }
      const response = await api.getProviderStats(this.authStore.userId);
      this.stats = response.data || this.stats;
    } catch (error) {
      this.errorMessage = error.response?.data?.message || 'Error al cargar las estadísticas.';
    }
  },
  methods: {
    handleMenuClick(button) {
      console.log('Botón del menú clicado:', button);

      // Navegación entre vistas
      if (button.action === 'showSpaces') {
        this.currentView = 'spaces';
      } else {
        this.currentView = null; // Vuelve al dashboard
      }
    },
  },
};
</script>

<style scoped>
/* Personalizaciones si es necesario */
</style>
