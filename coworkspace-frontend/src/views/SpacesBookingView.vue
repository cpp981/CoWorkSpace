<template>
  <div class="container py-4">
    <!-- Botón de volver -->
    <button class="btn btn-outline-secondary mb-3" @click="$emit('back')">
      <i class="bi bi-caret-left me-2"></i>Mis espacios
    </button>

    <h1 class="my-4 text-primary">Reservas en: {{ currentSpaceName }}</h1>

    <!-- Barra de búsqueda -->
    <div class="input-group mb-3">
      <span class="input-group-text bg-white border-end-0 border-secondary">
        <i class="bi bi-search text-muted"></i>
      </span>
      <input v-model="search" type="text" class="form-control border-start-0 border-secondary"
        placeholder="Buscar reserva..." />
    </div>

    <table class="table table-striped mt-3" v-if="filteredBookings.length">
      <thead>
        <tr>
          <th>Usuario</th>
          <th>Fecha inicio</th>
          <th>Fecha fin</th>
        </tr>
      </thead>
      <tbody>
        <tr v-for="booking in filteredBookings" :key="booking.id">
          <td>{{ booking.userName }}</td>
          <td>{{ formatDateTime(booking.startTime) }}</td>
          <td>{{ formatDateTime(booking.endTime) }}</td>
        </tr>
      </tbody>
    </table>

    <div v-else class="alert alert-info">
      No hay reservas para este espacio.
    </div>
  </div>
</template>

<script>
import { ref, computed, onMounted } from "vue";
import api from "../services/api";

export default {
  name: "SpacesBookingView",
  props: {
    spaceId: {
      type: Number,
      required: true,
    },
    spaceName: {
      type: String,
      required: true,
    },
  },
  emits: ["back"],
  setup(props) {
    const bookings = ref([]);
    const currentSpaceName = ref(props.spaceName);
    const search = ref("");

    const filteredBookings = computed(() => {
      if (!search.value) return bookings.value;
      return bookings.value.filter((b) =>
        b.userName.toLowerCase().includes(search.value.toLowerCase())
      );
    });

    function formatDateTime(dateString) {
      const date = new Date(dateString);
      return date.toLocaleString();
    }

    onMounted(async () => {
      try {
        const response = await api.getBookingsBySpace(props.spaceId);
        bookings.value = response.data;
        if (response.data.length > 0) {
          currentSpaceName.value = response.data[0].spaceName;
        }
      } catch (error) {
        console.error("Error cargando reservas:", error);
      }
    });

    return {
      bookings,
      currentSpaceName,
      search,
      filteredBookings,
      formatDateTime,
    };
  },
};
</script>
