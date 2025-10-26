<template>
  <div class="container py-4">
    <!-- Botón de volver -->
    <button class="btn btn-outline-secondary mb-3" @click="$emit('back')">
      <i class="bi bi-caret-left me-2"></i>Mis espacios
    </button>
    <h2 class="my-4 titulo">Reservas en {{ currentSpaceName }}</h2>
    <GenericList
      :items="bookings"
      :headers="['Usuario', 'Fecha inicio', 'Fecha fin']"
      :fields="['userName', 'startTimeFormatted', 'endTimeFormatted']"
      :loading="loading"
      :show-add-button="false"
      :show-actions="false"
      :show-manage="false"
      searchPlaceholder="Buscar reserva..."
    />
  </div>
</template>

<script>
import { ref, computed, onMounted } from "vue";
import api from "../services/api";
import GenericList from "../components/GenericList.vue";

export default {
  name: "SpacesBookingView",
  components: { GenericList },
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
    const loading = ref(false);

    const filteredBookings = computed(() => {
      if (!search.value) return bookings.value;
      return bookings.value.filter((b) =>
        b.userName.toLowerCase().includes(search.value.toLowerCase())
      );
    });

    const dateOptions = {
      dateStyle: "short",
      timeStyle: "short",
      timeZone: "Europe/Madrid",
    };

    function formatDateTime(dateString) {
      const date = new Date(dateString);
      return date.toLocaleString("es-ES", dateOptions);
    }

    onMounted(async () => {
      try {
        loading.value = true;
        const response = await api.getBookingsBySpace(props.spaceId);
        bookings.value = response.data.map((b) => ({
          ...b,
          startTimeFormatted: formatDateTime(b.startTime),
          endTimeFormatted: formatDateTime(b.endTime),
        }));
        if (response.data.length > 0) {
          currentSpaceName.value = response.data[0].spaceName;
        }
      } catch (error) {
        console.error("Error cargando reservas:", error);
      } finally {
        loading.value = false;
      }
    });

    return {
      bookings,
      loading,
      currentSpaceName,
      search,
      filteredBookings,
      formatDateTime,
    };
  },
};
</script>
