<template>
  <div class="d-flex">
    <!-- Menú lateral -->
    <GenericMenu @button-click="handleMenuClick" />

    <!-- Contenido principal del dashboard -->
    <div class="flex-grow-1 p-3">
      <Dashboard title="Dashboard de SuperAdmin" :metrics="superAdminMetrics" :chartTitle="'Usuarios por Rol'"
        :chartData="chartData" :chartOptions="chartOptions" :detailsTitle="'Usuarios por Rol'"
        :tableHeaders="tableHeaders" :tableData="tableData" :errorMessage="errorMessage">
        <template #details>
          <table v-if="Object.keys(stats.usersByRole).length" class="table table-striped">
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

<script>
import Dashboard from '../components/Dashboard.vue';
import GenericMenu from '../components/GenericMenu.vue';
import api from '../services/api'; // Importar objeto completo
import { useAuthStore } from '../stores/auth';

export default {
  name: 'SuperAdminDashboard',
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
        totalSpaces: 0,
        totalBookings: 0,
        totalRevenue: 0,
        totalUsers: 0,
        usersByRole: {},
      },
      errorMessage: null,
    };
  },
  computed: {
    superAdminMetrics() {
      return [
        { label: 'Espacios Totales', value: this.stats.totalSpaces },
        { label: 'Reservas Totales', value: this.stats.totalBookings },
        { label: 'Ingresos Totales', value: `${this.stats.totalRevenue.toFixed(2)} €` },
        { label: 'Usuarios Totales', value: this.stats.totalUsers },
      ];
    },
    chartData() {
      return {
        labels: Object.keys(this.stats.usersByRole),
        datasets: [{
          label: 'Usuarios',
          data: Object.values(this.stats.usersByRole),
          backgroundColor: ['#007bff', '#28a745', '#dc3545', '#ffc107'],
        }],
      };
    },
    chartOptions() {
      return {
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { display: true }, // Habilitar leyenda
        },
      };
    },
    tableHeaders() {
      return ['Rol', 'Cantidad'];
    },
    tableData() {
      return Object.entries(this.stats.usersByRole).map(([role, count]) => [role, count]);
    },
  },
  async created() {
    try {
      if (!this.authStore.userId) {
        this.errorMessage = 'Usuario no autenticado.';
        return;
      }
      console.log('API object:', api); // Depuración
      const response = await api.getSuperAdminStats(this.authStore.userId);
      this.stats = response.data || this.stats;
    } catch (error) {
      this.errorMessage = error.response?.data?.message || 'Error al cargar las estadísticas.';
    }
  },
  methods: {
    handleMenuClick(button){
     console.log('Botón del menú clicado:', button);
      // Aquí realizamos la acción de redirigir al pulsar el botón del meú lateral
    }
  }
};
</script>

<style scoped>
/* Personalizaciones si es necesario */
</style>