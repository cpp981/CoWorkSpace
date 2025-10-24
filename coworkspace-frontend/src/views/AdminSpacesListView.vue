<template>
  <div class="container py-4 mt-3">
    <!-- VISTA LISTADO -->
    <div v-if="view === 'list'">
      <h2 class="my-4 mb-5 titulo">Mis espacios</h2>
      <div v-if="spaces.length" class="row g-3">
        <div class="input-group mb-3">
          <span class="input-group-text bg-white border-end-0 border-secondary">
            <i class="bi bi-search text-muted"></i>
          </span>
          <input
            v-model="search"
            type="text"
            class="form-control border-start-0 border-secondary"
            placeholder="Buscar por nombre o ciudad..."
          />
        </div>

        <!-- bucle sobre paginatedSpaces -->
        <div class="col-md-4" v-for="space in paginatedSpaces" :key="space.id">
          <AdminSpaceCard
            :space="space"
            @view-calendar="handleViewCalendar"
            @view-management="handleViewManagement"
          />
        </div>
      </div>
      <p v-else class="text-muted">No tienes espacios registrados.</p>

      <!-- PAGINACIÓN -->
      <div class="mt-4" v-if="filteredSpaces.length > perPage">
        <GenericPagination
          :currentPage="currentPage"
          :totalPages="totalPages"
          @prev="prevPage"
          @next="nextPage"
          @goToPage="goToPage"
        />
      </div>
    </div>

    <!-- VISTA GESTIÓN (oculta el listado) -->
    <div v-else-if="view === 'management'">
      <AdminBookingManagement :space="selectedSpace" @back="goBack" />
    </div>

    <!-- Modal del calendario -->
    <div
      class="modal fade"
      id="calendarModal"
      tabindex="-1"
      aria-hidden="true"
      ref="calendarModalRef"
    >
      <div
        class="modal-dialog modal-xl modal-dialog-centered modal-dialog-scrollable"
      >
        <div class="modal-content">
          <div class="modal-header">
            <h5 class="modal-title text-primary fs-2">
              Reservas en {{ selectedSpace?.nombre }}
            </h5>
            <button
              type="button"
              class="btn-close"
              data-bs-dismiss="modal"
              aria-label="Close"
            ></button>
          </div>
          <div class="modal-body">
            <!-- GenericCalendar se renderiza dentro del modal -->
            <GenericCalendar
              v-if="selectedSpace"
              ref="calendarComponent"
              :events="calendarEvents"
              initial-view="dayGridMonth"
            />
          </div>
        </div>
      </div>
    </div>
  </div>
</template>

<script>
import { ref, onMounted, computed, nextTick, watch } from "vue";
import { useAuthStore } from "../stores/auth";
import api from "../services/api";
import AdminSpaceCard from "../components/AdminSpaceCard.vue";
import GenericCalendar from "../components/GenericCalendar.vue";
import AdminBookingManagement from "./AdminBookingManagement.vue";
import GenericPagination from "../components/GenericPagination.vue";
import { useNotyf } from "../composables/useNotyf";
import { Modal } from "bootstrap";

export default {
  components: {
    AdminSpaceCard,
    GenericCalendar,
    AdminBookingManagement,
    GenericPagination,
  },
  setup() {
    const authStore = useAuthStore();
    const spaces = ref([]);
    const search = ref("");
    const notyf = useNotyf();

    const currentPage = ref(1);
    const perPage = 6;

    const selectedSpace = ref(null);
    const calendarEvents = ref([]);
    const view = ref("list");

    const calendarModalRef = ref(null);
    const calendarComponent = ref(null);
    let modalInstance = null;

    const filteredSpaces = computed(() =>
      spaces.value.filter(
        (space) =>
          space.nombre.toLowerCase().includes(search.value.toLowerCase()) ||
          space.ciudad.toLowerCase().includes(search.value.toLowerCase())
      )
    );

    // Paginación
    const totalPages = computed(() =>
      Math.max(1, Math.ceil(filteredSpaces.value.length / perPage))
    );

    const paginatedSpaces = computed(() => {
      const start = (currentPage.value - 1) * perPage;
      return filteredSpaces.value.slice(start, start + perPage);
    });

    const goToPage = (n) => {
      if (n >= 1 && n <= totalPages.value) currentPage.value = n;
    };

    const nextPage = () => goToPage(currentPage.value + 1);
    const prevPage = () => goToPage(currentPage.value - 1);

    watch(
      () => filteredSpaces.value.length,
      () => {
        if (currentPage.value > totalPages.value) {
          currentPage.value = Math.max(1, totalPages.value);
        }
        // si la página actual queda vacía y hay resultados, volver a la primera
        if (
          filteredSpaces.value.length > 0 &&
          paginatedSpaces.value.length === 0
        ) {
          currentPage.value = 1;
        }
      }
    );

    const getColorForClient = (userId) => {
      let hash = 0;
      for (let i = 0; i < userId.toString().length; i++) {
        hash = userId.toString().charCodeAt(i) + ((hash << 5) - hash);
      }
      return `hsl(${hash % 360}, 70%, 50%)`;
    };

    const handleViewCalendar = async (space) => {
      selectedSpace.value = space;

      try {
        const res = await api.getSpaceBookings(authStore.userId, space.id);
        calendarEvents.value = res.data.map((b) => ({
          title: b.nombreCliente,
          start: b.fechaInicio,
          end: b.fechaFin,
          color: getColorForClient(b.userId),
        }));
      } catch (err) {
        notyf.error(
          err.response?.data?.message || "Error al cargar las reservas"
        );
        calendarEvents.value = [];
      }

      // ocultar la lista mientras el modal está abierto
      view.value = "modal-calendar";

      // crear instancia Modal la primera vez y listeners
      if (!modalInstance) {
        modalInstance = new Modal(calendarModalRef.value);

        // Al mostrarse el modal, forzamos render o update del calendario (FullCalendar)
        calendarModalRef.value.addEventListener("shown.bs.modal", async () => {
          await nextTick();
          const apiCal = calendarComponent.value?.getApi?.();
          if (apiCal) {
            apiCal.render();
            apiCal.updateSize?.();
          }
        });

        // Al cerrarse el modal, volvemos a la vista lista
        calendarModalRef.value.addEventListener("hidden.bs.modal", () => {
          view.value = "list";
          selectedSpace.value = null;
          calendarEvents.value = [];
        });
      }

      // mostrar el modal
      modalInstance.show();
    };

    const handleViewManagement = (space) => {
      selectedSpace.value = space;
      view.value = "management";
    };

    const goBack = () => {
      selectedSpace.value = null;
      calendarEvents.value = [];
      view.value = "list";
    };

    onMounted(async () => {
      try {
        const res = await api.getAdminSpaces(authStore.userId);
        spaces.value = res.data || [];
      } catch (err) {
        notyf.error(
          err.response?.data?.message || "Error al cargar los espacios"
        );
      }
    });

    return {
      authStore,
      spaces,
      search,
      filteredSpaces,
      handleViewCalendar,
      handleViewManagement,
      selectedSpace,
      calendarEvents,
      view,
      goBack,
      calendarModalRef,
      calendarComponent,
      paginatedSpaces,
      currentPage,
      totalPages,
      goToPage,
      nextPage,
      prevPage,
      perPage,
    };
  },
};
</script>
