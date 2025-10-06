<template>
    <div class="container py-4">
        <h1 class="my-4 text-primary">Espacios de {{ authStore.userName }}</h1>

        <div v-if="spaces.length" class="row g-3">
            <!-- Barra de búsqueda -->
            <div class="input-group mb-3">
                <span class="input-group-text bg-white border-end-0 border-secondary">
                    <i class="bi bi-search text-muted"></i>
                </span>
                <input v-model="search" type="text" class="form-control border-start-0 border-secondary"
                    placeholder="Buscar por nombre o ciudad..." />
            </div>

            <div class="col-md-4" v-for="space in filteredSpaces" :key="space.id">
                <AdminSpaceCard :space="space" @view-calendar="handleViewCalendar" />
            </div>
        </div>

        <p v-else class="text-muted">No tienes espacios registrados.</p>

        <!-- Modal para el calendario -->
        <div class="modal fade" id="calendarModal" tabindex="-1" aria-labelledby="calendarModalLabel" aria-hidden="true"
            ref="calendarModalRef">
            <div class="modal-dialog modal-lg modal-dialog-centered modal-dialog-scrollable">
                <div class="modal-content">
                    <div class="modal-header">
                        <h5 class="modal-title" id="calendarModalLabel">
                            Reservas de {{ selectedSpace?.nombre }}
                        </h5>
                        <button type="button" class="btn-close" data-bs-dismiss="modal" aria-label="Close"></button>
                    </div>
                    <div class="modal-body">
                        <GenericCalendar v-if="selectedSpace" :events="calendarEvents" initial-view="timeGridWeek" />
                    </div>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
import { ref, onMounted, computed } from "vue";
import { useAuthStore } from "../stores/auth";
import api from "../services/api";
import AdminSpaceCard from "../components/AdminSpaceCard.vue";
import GenericCalendar from "../components/GenericCalendar.vue";
import { useNotyf } from "../composables/useNotyf";
import { Modal } from "bootstrap";

export default {
    components: { AdminSpaceCard, GenericCalendar },
    setup() {
        const authStore = useAuthStore();
        const spaces = ref([]);
        const search = ref("");
        const notyf = useNotyf();

        const selectedSpace = ref(null);
        const calendarEvents = ref([]);
        const calendarModalRef = ref(null);
        let modalInstance = null;

        const filteredSpaces = computed(() =>
            spaces.value.filter(
                (space) =>
                    space.nombre.toLowerCase().includes(search.value.toLowerCase()) ||
                    space.ciudad.toLowerCase().includes(search.value.toLowerCase())
            )
        );

        const handleViewCalendar = async (space) => {
            selectedSpace.value = space;

            try {
                // Obtener reservas del espacio y mostrarlas en el calendario
                const res = await api.getSpaceBookings(space.id);
                calendarEvents.value = res.data.map((b) => ({
                    title: b.nombreCliente,
                    start: b.fechaInicio,
                    end: b.fechaFin,
                    color: b.color || "#0d6efd",
                }));
            } catch (err) {
                notyf.error("Error al cargar las reservas del espacio");
                calendarEvents.value = [];
            }

            if (!modalInstance) {
                modalInstance = new Modal(calendarModalRef.value);
            }
            modalInstance.show();
        };

        onMounted(async () => {
            try {
                const res = await api.getAdminSpaces(authStore.userId);
                spaces.value = res.data || [];
            } catch (err) {
                const errors = err.response?.data?.errors;

                if (errors) {
                    for (const key in errors) {
                        errors[key].forEach((message) => notyf.error(message));
                    }
                } else {
                    notyf.error(
                        err.response?.data?.message || "Error al cargar los espacios"
                    );
                }
            }
        });

        return {
            authStore,
            spaces,
            search,
            filteredSpaces,
            handleViewCalendar,
            selectedSpace,
            calendarEvents,
            calendarModalRef,
        };
    },
};
</script>
