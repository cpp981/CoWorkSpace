<template>
  <div class="container py-4">
    <!-- VISTA LISTADO (cards) -->
    <div v-if="view === 'list'">
      <h1 class="my-4 text-primary">Espacios de {{ authStore.userName }}</h1>

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

        <div class="col-md-4" v-for="space in filteredSpaces" :key="space.id">
          <AdminSpaceCard
            :space="space"
            @view-calendar="handleViewCalendar"
            @view-management="handleViewManagement"
          />
        </div>
      </div>

      <p v-else class="text-muted">No tienes espacios registrados.</p>
    </div>

    <!-- VISTA GESTIÓN (oculta el listado) -->
    <div v-else-if="view === 'management'">
      <AdminBookingManagement :space="selectedSpace" @back="goBack" />
    </div>

    <!-- Modal del calendario (markup siempre presente, lo mostramos vía Bootstrap Modal) -->
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
import { ref, onMounted, computed, nextTick } from "vue";
import { useAuthStore } from "../stores/auth";
import api from "../services/api";
import AdminSpaceCard from "../components/AdminSpaceCard.vue";
import GenericCalendar from "../components/GenericCalendar.vue";
import AdminBookingManagement from "./AdminBookingManagement.vue";
import { useNotyf } from "../composables/useNotyf";
import { Modal } from "bootstrap";

export default {
  components: { AdminSpaceCard, GenericCalendar, AdminBookingManagement },
  setup() {
    const authStore = useAuthStore();
    const spaces = ref([]);
    const search = ref("");
    const notyf = useNotyf();

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
        console.log(res);
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

        // Al mostrarse el modal, forzamos render/update del calendario (FullCalendar)
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
          // restaurar vista a list y limpiar selección si deseas
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
      // mostramos gestión y ocultamos la lista
      view.value = "management";
    };

    const goBack = () => {
      // si venimos de gestión, volvemos a la lista
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
    };
  },
};
</script>
