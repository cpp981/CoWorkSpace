<template>
  <div class="container dashboard">
    <!-- Título -->
    <h1 class="my-4 text-primary">{{ title }}</h1>

    <!-- Mensaje de error -->
    <div v-if="errorMessage" class="alert alert-danger" role="alert">
      {{ errorMessage }}
    </div>

    <!-- Métricas en cards -->
    <div class="row">
      <div
        v-for="(metric, index) in metrics"
        :key="index"
        class="col-lg-3 col-md-4 col-sm-6 mb-4"
      >
        <div class="card h-100 shadow-sm">
          <div class="card-body text-center">
            <h5 class="card-title text-muted">{{ metric.label }}</h5>
            <p class="card-text display-4">{{ metric.value }}</p>
          </div>
        </div>
      </div>
    </div>

    <!-- Gráfico -->
    <div v-if="chartData" class="row mb-4">
      <div class="col-12">
        <div class="card shadow-sm">
          <div class="card-body">
            <h5 class="card-title text-primary">{{ chartTitle }}</h5>
            <div class="chart-container">
              <Chart
                :type="'bar'"
                :data="chartData"
                :options="chartOptions"
              />
            </div>
          </div>
        </div>
      </div>
    </div>

    <!-- Detalles -->
    <div class="row">
      <div class="col-12">
        <div class="card shadow-sm">
          <div class="card-body">
            <h5 class="card-title text-primary">{{ detailsTitle }}</h5>
            <slot name="details">
              <table v-if="tableData.length" class="table table-striped">
                <thead>
                  <tr>
                    <th v-for="(header, index) in tableHeaders" :key="index">
                      {{ header }}
                    </th>
                  </tr>
                </thead>
                <tbody>
                  <tr v-for="(row, rowIndex) in tableData" :key="rowIndex">
                    <td v-for="(cell, cellIndex) in row" :key="cellIndex">
                      {{ cell }}
                    </td>
                  </tr>
                </tbody>
              </table>
              <p v-else class="text-muted">No hay datos disponibles.</p>
            </slot>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { Chart } from 'vue-chartjs';
import {
  Chart as ChartJS,
  Title,
  Tooltip,
  Legend,
  BarElement,
  CategoryScale,
  LinearScale,
} from 'chart.js';

ChartJS.register(Title, Tooltip, Legend, BarElement, CategoryScale, LinearScale);

export default {
  name: 'Dashboard',
  components: { Chart },
  props: {
    title: {
      type: String,
      required: true,
    },
    metrics: {
      type: Array,
      default: () => [],
    },
    chartTitle: {
      type: String,
      default: 'Gráfico',
    },
    chartData: {
      type: Object,
      default: null,
    },
    chartOptions: {
      type: Object,
      default: () => ({
        responsive: true,
        maintainAspectRatio: false,
        plugins: {
          legend: { display: true },
        },
      }),
    },
    detailsTitle: {
      type: String,
      default: 'Detalles',
    },
    tableHeaders: {
      type: Array,
      default: () => [],
    },
    tableData: {
      type: Array,
      default: () => [],
    },
    errorMessage: {
      type: String,
      default: null,
    },
  },
};
</script>

<style scoped>
.dashboard {
  padding: 20px 0;
  max-height: 100vh; /* Limitar altura al viewport */
  overflow-y: hidden; /* Evitar scroll vertical */
}

.card {
  border-radius: 8px;
  transition: transform 0.2s;
}

.card:hover {
  transform: translateY(-5px);
}

.card-title {
  font-size: 1rem;
  text-transform: uppercase;
}

.card-text.display-4 {
  font-size: 2rem;
  color: #007bff;
  font-weight: bold;
}

.table th {
  background-color: #f8f9fa;
}

.alert {
  margin-bottom: 20px;
}

.chart-container {
  position: relative;
  height: 300px; /* Altura fija para el gráfico */
  width: 100%;
}
</style>